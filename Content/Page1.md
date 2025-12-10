## Introduction

Using the framework presented, below, one can implement classes resembling:

```cs
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using VDS.RDF;
using VDS.RDF.Query;

using SparqlPrediction = (VDS.RDF.Query.SparqlQuery Query, VDS.RDF.Query.SparqlResultSet Result);

public class StoryChunk : ITree<StoryChunk>
{
    public StoryChunk? Parent
    {
        get { ... }
    }

    public IReadOnlyList<StoryChunk> Children
    {
        get { ... }
    }

    ...
}

public class ReaderState :
    IInterpreter<ReaderState, StoryChunk>,
    IDifferenceable<ReaderState>,
    IHasSemanticModel<ReaderState, IAskable<SparqlQuery, SparqlResultSet>>,
    IHasQuestions<SparqlQuery>,
    IHasPredictions<SparqlPrediction>,
    IHasBuffers,
    IHasAttention<SparqlQuery>,
    IHasAttention<SparqlPrediction>,
    IHasConfidence<SparqlPrediction>,
    ICommunicator<ReaderState, SparqlQuery, SparqlResultSet>,
    IProcedural<ReaderState>
{
    public IAskable<SparqlQuery, SparqlResultSet> Model
    {
        get { ... }
    }

    public ICollection<SparqlQuery> Questions
    {
        get { ... }
    }

    public ICollection<SparqlPrediction> Predictions
    {
        get { ... }
    }

    public IBufferSystem Buffers
    {
        get { ... }
    }

    public async IAsyncEnumerable<ReaderState> Interpret(StoryChunk input) { ... }

    public async Task<IProcedure<ReaderState>> DifferenceFrom(ReaderState other) { ... }

    public float GetAttention(SparqlQuery item) { ... }

    public float GetAttention(SparqlPrediction item) { ... }

    public void SetAttention(SparqlQuery item, float value) { ... }

    public void SetAttention(SparqlPrediction item, float value) { ... }

    public float GetConfidence(SparqlPrediction item) { ... }

    public void SetConfidence(SparqlPrediction item, float value) { ... }

    public async Task<ReaderState> Prompt(SparqlQuery query) { ... }

    public bool TryGetContent([NotNullWhen(true)] out SparqlResultSet? result) { ... }

    ...
}
```

## Incremental Interpretation and Comprehension

```cs
public interface IInterpreter<TSelf, in TInput>
    where TSelf : IInterpreter<TSelf, TInput>
{
    public IAsyncEnumerable<TSelf> Interpret(TInput input);
}
```

## Differencing

```cs
public interface IDifferenceable<TSelf>
    where TSelf : IDifferenceable<TSelf>
{
    public Task<IProcedure<TSelf>> DifferenceFrom(TSelf other);
}
```

### Procedures

```cs
public interface IProcedural<in TOperand, out TElement> { }

public interface IProcedural<TOperand> : IProcedural<TOperand, TOperand> { }

public interface ICustomCreateProcedure<in TOperand, out TElement>
{
    public IProcedure<TOperand> CreateProcedure(Action<TElement> action);
    public IProcedure<TOperand, TResult> CreateProcedure<TResult>(Func<TElement, TResult> function);
}

public interface IHasMapping<in TOperand, out TElement>
{
    public Func<TOperand, TElement> Map { get; }
}

public interface IProcedure
{
    public Task Execute(object arg);
}

public interface IProcedure<in TElement> : IProcedure
{
    public Task Execute(TElement arg);
}

public interface IProcedure<in TElement, TResult> : IProcedure<TElement>
{
    public new Task<TResult> Execute(TElement arg);
}

public sealed class DelegateProcedure<TElement> : IProcedure<TElement>
{
    public DelegateProcedure(Action<TElement> action)
    {
        m_action = action;
    }

    Action<TElement> m_action;

    public Task Execute(TElement arg)
    {
        return Task.Run(() => m_action(arg));
    }

    Task IProcedure.Execute(object arg)
    {
        if(arg is TElement element)
        {
            return Execute(element);
        }
        else
        {
            throw new ArgumentException();
        }
    }
}

public sealed class DelegateProcedure<TElement, TResult> : IProcedure<TElement, TResult>
{
    public DelegateProcedure(Func<TElement, TResult> function)
    {
        m_function = function;
    }

    Func<TElement, TResult> m_function;

    public Task<TResult> Execute(TElement arg)
    {
        return Task<TResult>.Run(() => m_function(arg));
    }

    Task IProcedure<TElement>.Execute(TElement arg)
    {
        return Execute(arg);
    }

    Task IProcedure.Execute(object arg)
    {
        if (arg is TElement element)
        {
            return Execute(element);
        }
        else
        {
            throw new ArgumentException();
        }
    }
}

public sealed class CompoundProcedure<TElement> : IProcedure<TElement>
{
    public CompoundProcedure(IEnumerable<IProcedure<TElement>> procedures)
    {
        Procedures = procedures;
    }

    public IEnumerable<IProcedure<TElement>> Procedures { get; }

    public async Task Execute(TElement arg)
    {
        foreach (var procedure in Procedures)
        {
            await procedure.Execute(arg);
        }
    }

    Task IProcedure.Execute(object arg)
    {
        if (arg is TElement element)
        {
            return Execute(element);
        }
        else
        {
            throw new ArgumentException();
        }
    }
}
```

### Extensions

```cs
public static partial class Extensions
{
    extension<TOperand, TElement>(IProcedural<TOperand, TElement> procedural)
    {
        public IProcedure<TOperand> CreateProcedure(Action<TElement> action)
        {
            if (procedural is ICustomCreateProcedure<TOperand, TElement> custom)
            {
                return custom.CreateProcedure(action);
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new DelegateProcedure<TOperand>((TOperand o) => action((TElement)(object)o!));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public IProcedure<TOperand, TResult> CreateProcedure<TResult>(Func<TElement, TResult> function)
        {
            if (procedural is ICustomCreateProcedure<TOperand, TElement> custom)
            {
                return custom.CreateProcedure<TResult>(function);
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new DelegateProcedure<TOperand, TResult>((TOperand o) => function((TElement)(object)o!));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public IProcedural<TOperand, TResult> Map<TResult>(Func<TElement, TResult> map)
        {
            if (procedural is IHasMapping<TOperand, TElement> hasMap)
            {
                var proceduralMap = hasMap.Map;
                return new Mapping<TOperand, TResult>((TOperand o) => map(proceduralMap(o)));
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new Mapping<TOperand, TResult>((TOperand o) => map((TElement)(object)o!));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}

class Mapping<TOperand, TResult> :
    IProcedural<TOperand, TResult>,
    ICustomCreateProcedure<TOperand, TResult>,
    IHasMapping<TOperand, TResult>
{
    public Mapping(Func<TOperand, TResult> map)
    {
        m_map = map;
    }

    Func<TOperand, TResult> m_map;

    public Func<TOperand, TResult> Map
    {
        get
        {
            return m_map;
        }
    }

    public IProcedure<TOperand> CreateProcedure(Action<TResult> action)
    {
        return new DelegateProcedure<TOperand>((TOperand o) => action(m_map(o)));
    }

    public IProcedure<TOperand, TOutput> CreateProcedure<TOutput>(Func<TResult, TOutput> function)
    {
        return new DelegateProcedure<TOperand, TOutput>((TOperand o) => function(m_map(o)));
    }
}
```

## Semantic Modeling

```cs
public interface IHasSemanticModel<TSelf, out TModel>
    where TSelf : IHasSemanticModel<TSelf, TModel>
{
    public TModel Model { get; }
}

public interface IAskable<in TQuestion, out TResponse>
{
    public TResponse Ask(TQuestion question);
}
```

### Metadata

```cs
public interface IHasMetadata<TSelf, out TMetadata>
    where TSelf : IHasMetadata<TSelf, TMetadata>
{
    public TMetadata About { get; }
}

public interface IHasIdentifier<out TId>
{
    public TId Id { get; }
}
```

## Curiosity

```cs
public interface IHasQuestions<TQuestion>
{
    public ICollection<TQuestion> Questions { get; }
}
```

## Prediction

```cs
public interface IHasPredictions<TPrediction>
{
    public ICollection<TPrediction> Predictions { get; }
}
```

## Confidence

```cs
public interface IHasConfidence<in TElement>
{
    public float GetConfidence(TElement item);
    public void SetConfidence(TElement item, float value);
}
```

## Attention

One could add capabilities for systems to simulate the distribution or allocation of attention to things, e.g., to their questions and predictions. This would be one means of prioritizing or sorting systems' questions and predictions.

```cs
public interface IHasAttention<in TElement>
{
    public float GetAttention(TElement item);
    public void SetAttention(TElement item, float value);
}
```

## Working Memory, Buffers, Chunks, and Compression

Depending upon the nature of the input, one could add capabilities for incremental interpreters and comprehenders to be able to buffer arriving inputs. One could also "compress" buffered sequences of inputs into chunks to store these chunks in secondary buffers and subsequently "decompress" these chunks back into input sequences, in primary buffers, as needed. That is, a system could "compress" some of the contents of its primary buffer into a secondary buffer. Similarly, tertiary buffers &ndash; and beyond &ndash; could be considered.

A buffer system could, then, might resemble:

```cs
public interface IBuffer :
    ICollection
{
    public Type ElementType { get; }
}

public interface IBufferSystem
    : IReadOnlyList<IBuffer>
{ }

public interface IHasBuffers
{
    public IBufferSystem Buffers { get; }
}
```

## Concurrency, Threads, and Multitasking

Approaches to incremental interpretation and comprehension can tackle concurrency, threads, and multitasking in a number of ways.

With respect to concurrency regarding procedures affecting differencing, one could add the following to express a set of `IProcedure<>` instances as occurring concurrently:

```cs
public sealed class ConcurrentProcedure<TElement> : IProcedure<TElement>
{
    public ConcurrentProcedure(IEnumerable<IProcedure<TElement>> procedures)
    {
        Procedures = procedures;
    }

    public IEnumerable<IProcedure<TElement>> Procedures { get; }

    public Task Execute(TElement arg)
    {
        return Task.WhenAll(Procedures.Select(p => p.Execute(arg)));
    }

    Task IProcedure.Execute(object arg)
    {
        if (arg is TElement element)
        {
            return Execute(element);
        }
        else
        {
            throw new ArgumentException();
        }
    }
}
```

With respect to processing concurrent happenings, depending upon the nature of the type of input, one input object instance could describe multiple happenings simultaneously.

With respect to story threads in a narration, events from different story threads could be presented to a system sequentially.

A system could have multiple incremental interpreters and comprehenders, one per cognitive executive task, and could task-switch between them when story threads switched in a narration.

## Overlays

### Collection Overlays

Collections, e.g., of type `ICollection<>` and `IDictionary<,>`, could be developed to support an overlay pattern where overlaid collections would reference parent collections, noting additions and removals, to implement collection interfaces without having to copy all of the elements of their parent collections.

In theory, an overlay-manager component could asynchronously schedule the copying of elements from collections to descendent collections with overlays, removing references to overlaid collections to enable memory storage efficiency.

### Semantic Overlays

Semantic models can be described as being collections of statements, triples or quads. Instead of having to copy a semantic model each time that a descendent interpretation state is created, a _semantic overlay_ system could be developed where subsequent nodes could, internally, provide overlays atop their predecessors' datasets while also, for example, implementing the `IInMemoryQueryableStore` interface.

In theory, semantic interpretation states could, using an overlay-manager component, asynchronously migrate their predecessors' semantic datasets into their overlaid datasets, deferring any needed copying processes until times convenient to systems.

## Cognitive Workflow and Timelines

The `IDifferenceable<>` and `IProcedure<>` pattern, sketched above, could be expanded into a fuller _cognitive workflow_ system for describing and visualizing modeled and simulated processes of cognition occurring as a result of the processing of inputs and sequences of inputs.

Alternatively, a _cognitive timeline_ system could be explored to provide multiple concurrent tracks of activities for describing and visualizing modeled and simulated processes.

## Communication and Question-answering

While `ISemanticState<,>` provides a `Model` property which could be queried or otherwise inspected, an interface can be created for a second variety of presenting prompts or questions to systems, one where state changes are expected of systems when responding.

```cs
public interface ICommunicator<TSelf, in TInput, TOutput>
    where TSelf : ICommunicator<TSelf, TInput, TOutput>
{
    public Task<TSelf> Prompt(TInput prompt);

    public bool TryGetContent([NotNullWhen(true)] out TOutput? content);
}

public interface ISequentialCommunicator<TSelf, in TInput, TOutput>
    where TSelf : ISequentialCommunicator<TSelf, TInput, TOutput>
{
    public Task<TSelf> Prompt(TInput prompt);

    public bool TryGetContent([NotNullWhen(true)] out TOutput? content);

    public Task<TSelf> Continue();
}
```
