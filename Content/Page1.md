## Introduction

Using the framework presented, below, one could implement classes resembling:

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
    IProcedureDifferenceable<ReaderState>,
    IHasModel<ReaderState, ReaderState.SituationModel>,
    IHasQuestions<SparqlQuery>,
    IHasPredictions<SparqlPrediction>,
    IHasBuffers,
    IHasAttention<SparqlQuery>,
    IHasAttention<SparqlPrediction>,
    IHasConfidence<SparqlPrediction>,
    ICommunicator<ReaderState, SparqlQuery, SparqlResultSet>,
    IProcedural<ReaderState>
{
    public class SituationModel :
        IAskable<SparqlQuery>,
        ISelectable<SparqlQuery, SparqlResult>,
        IConstructable<SparqlQuery, IGraph>,
        ITellable<Quad>,
        IUpdateable<SparqlUpdateCommand>,
        IUpdateable<SparqlUpdateCommandSet>
    {
        ...
    }

    public SituationModel Model
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

    public async IAsyncEnumerable<ReaderState> Interpret(StoryChunk input, CancellationToken token = default) { ... }

    public IProcedure<ReaderState> DifferenceFrom(ReaderState other) { ... }

    public float GetAttention(SparqlQuery item) { ... }

    public float GetAttention(SparqlPrediction item) { ... }

    public void SetAttention(SparqlQuery item, float value) { ... }

    public void SetAttention(SparqlPrediction item, float value) { ... }

    public float GetConfidence(SparqlPrediction item) { ... }

    public void SetConfidence(SparqlPrediction item, float value) { ... }

    public async Task<ReaderState> Prompt(SparqlQuery query, CancellationToken token = default) { ... }

    public bool TryGetContent([NotNullWhen(true)] out SparqlResultSet? result) { ... }

    ...
}
```

## Incremental Interpretation and Comprehension

```cs
public interface IInterpreter<TSelf, in TInput>
    where TSelf : IInterpreter<TSelf, TInput>
{
    public IAsyncEnumerable<TSelf> Interpret(TInput input, CancellationToken cancellationToken = default);
}
```

## Differencing

```cs
public interface IProcedureDifferenceable<in TSelf>
    where TSelf : IProcedureDifferenceable<TSelf>
{
    public IProcedure<TSelf> DifferenceFrom(TSelf other);
}

public interface IEventDifferenceable<in TSelf, out TEvent>
    where TSelf : IEventDifferenceable<TSelf, TEvent>
{
    public IEnumerable<TEvent> DifferenceFrom(TSelf other);
}
```

### Procedures

```cs
public interface IProcedure
{
    public Task Execute(object arg, CancellationToken cancellationToken = default);
}

public interface IProcedure<in TElement> : IProcedure
{
    public Task Execute(TElement arg, CancellationToken cancellationToken = default);
}

public interface IProcedure<in TElement, TResult> : IProcedure<TElement>
{
    public new Task<TResult> Execute(TElement arg, CancellationToken cancellationToken = default);
}

public sealed class ActionProcedure<TElement> : IProcedure<TElement>
{
    public ActionProcedure(Action<TElement, CancellationToken> action)
    {
        m_action = action;
    }

    Action<TElement, CancellationToken> m_action;

    public Task Execute(TElement arg, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => m_action(arg, cancellationToken), cancellationToken);
    }

    Task IProcedure.Execute(object arg, CancellationToken cancellationToken)
    {
        if (arg is TElement element)
        {
            return Execute(element, cancellationToken);
        }
        else
        {
            throw new ArgumentException();
        }
    }
}

public sealed class FuncProcedure<TElement, TResult> : IProcedure<TElement, TResult>
{
    public FuncProcedure(Func<TElement, CancellationToken, TResult> function)
    {
        m_function = function;
    }

    Func<TElement, CancellationToken, TResult> m_function;

    public Task<TResult> Execute(TElement arg, CancellationToken cancellationToken = default)
    {
        return Task<TResult>.Run(() => m_function(arg, cancellationToken), cancellationToken);
    }

    Task IProcedure<TElement>.Execute(TElement arg, CancellationToken cancellationToken)
    {
        return Execute(arg, cancellationToken);
    }

    Task IProcedure.Execute(object arg, CancellationToken cancellationToken)
    {
        if (arg is TElement element)
        {
            return Execute(element, cancellationToken);
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

    public async Task Execute(TElement arg, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var procedure in Procedures)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await procedure.Execute(arg, cancellationToken);
        }
    }

    Task IProcedure.Execute(object arg, CancellationToken cancellationToken)
    {
        if (arg is TElement element)
        {
            return Execute(element, cancellationToken);
        }
        else
        {
            throw new ArgumentException();
        }
    }
}
```

### Events

```cs
public interface IApply
{
    public void Apply(object @event);
}

public interface IApply<in TEvent> : IApply
{
    public void Apply(TEvent @event);
}

public interface ICloneable<out TSelf> : ICloneable
    where TSelf : ICloneable<TSelf>
{
    public new TSelf Clone();
}

public interface ICloneable<out TSelf, in TEvent> : ICloneable<TSelf>
    where TSelf : ICloneable<TSelf, TEvent>
{
    public TSelf Clone(IEnumerable<TEvent> events);
}
```

### Procedure-related Extensions

```cs
public interface IProcedural<in TOperand, out TElement> { }

public interface IProcedural<TOperand> : IProcedural<TOperand, TOperand> { }

public interface ICustomCreateProcedure<in TOperand, out TElement>
{
    public IProcedure<TOperand> CreateProcedure(Action<TElement> action);
    public IProcedure<TOperand> CreateProcedure(Action<TElement, CancellationToken> action);
    public IProcedure<TOperand, TResult> CreateProcedure<TResult>(Func<TElement, TResult> function);
    public IProcedure<TOperand, TResult> CreateProcedure<TResult>(Func<TElement, CancellationToken, TResult> function);
}

public interface IHasMapping<in TOperand, out TElement>
{
    public Func<TOperand, CancellationToken, TElement> Map { get; }
}

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
                return new ActionProcedure<TOperand>((TOperand o, CancellationToken c) =>
                    action((TElement)(object)o!));
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
                return new FuncProcedure<TOperand, TResult>((TOperand o, CancellationToken c) =>
                    function((TElement)(object)o!));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public IProcedure<TOperand> CreateProcedure(Action<TElement, CancellationToken> action)
        {
            if (procedural is ICustomCreateProcedure<TOperand, TElement> custom)
            {
                return custom.CreateProcedure(action);
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new ActionProcedure<TOperand>((TOperand o, CancellationToken c) =>
                    action((TElement)(object)o!, c));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public IProcedure<TOperand, TResult> CreateProcedure<TResult>(Func<TElement, CancellationToken, TResult> function)
        {
            if (procedural is ICustomCreateProcedure<TOperand, TElement> custom)
            {
                return custom.CreateProcedure<TResult>(function);
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new FuncProcedure<TOperand, TResult>((TOperand o, CancellationToken c) =>
                    function((TElement)(object)o!, c));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public IProcedural<TOperand, TResult> Map<TResult>(Func<TElement, TResult> map)
        {
            if(procedural is IHasMapping<TOperand, TElement> hasMap)
            {
                var pmap = hasMap.Map;
                return new Mapping<TOperand, TResult>((TOperand o, CancellationToken c) => map(pmap(o, c)));
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new Mapping<TOperand, TResult>((TOperand o, CancellationToken c) =>
                    map((TElement)(object)o!));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public IProcedural<TOperand, TResult> Map<TResult>(Func<TElement, CancellationToken, TResult> map)
        {
            if (procedural is IHasMapping<TOperand, TElement> hasMap)
            {
                var pmap = hasMap.Map;
                return new Mapping<TOperand, TResult>((TOperand o, CancellationToken c) =>
                    map(pmap(o, c), c));
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new Mapping<TOperand, TResult>((TOperand o, CancellationToken c) =>
                    map((TElement)(object)o!, c));
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
    public Mapping(Func<TOperand, CancellationToken, TResult> map)
    {
        m_map = map;
    }

    Func<TOperand, CancellationToken, TResult> m_map;

    public Func<TOperand, CancellationToken, TResult> Map
    {
        get
        {
            return m_map;
        }
    }

    public IProcedure<TOperand> CreateProcedure(Action<TResult> action)
    {
        return new ActionProcedure<TOperand>((TOperand o, CancellationToken c) => action(m_map(o, c)));
    }

    public IProcedure<TOperand> CreateProcedure(Action<TResult, CancellationToken> action)
    {
        return new ActionProcedure<TOperand>((TOperand o, CancellationToken c) => action(m_map(o, c), c));
    }

    public IProcedure<TOperand, TOutput> CreateProcedure<TOutput>(Func<TResult, TOutput> function)
    {
        return new FuncProcedure<TOperand, TOutput>((TOperand o, CancellationToken c) =>
            function(m_map(o, c)));
    }

    public IProcedure<TOperand, TOutput> CreateProcedure<TOutput>(Func<TResult, CancellationToken, TOutput> function)
    {
        return new FuncProcedure<TOperand, TOutput>((TOperand o, CancellationToken c) =>
            function(m_map(o, c), c));
    }
}
```

## Semantics

```cs
public interface IHasModel<TSelf, out TModel>
    where TSelf : IHasModel<TSelf, TModel>
{
    public TModel Model { get; }
}

public interface IHasModel<TSelf, out TModel, in TKey, out TValue> : IHasModel<TSelf, TModel>
    where TSelf : IHasModel<TSelf, TModel, TKey, TValue>
    where TModel : IReadOnlyMap<TKey, TValue>
{ }

public interface IAskable<in TQuestion>
{
    public Task<bool> Ask(TQuestion question, CancellationToken cancellationToken = default);
}

public interface ISelectable<in TQuestion, out TResponse>
{
    public IAsyncEnumerable<TResponse> Select(TQuestion question, CancellationToken cancellationToken = default);
}

public interface IConstructable<in TQuestion, out TResponse>
{
    public IAsyncEnumerable<TResponse> Construct(TQuestion question, CancellationToken cancellationToken = default);
}

public interface ITellable<in TStatement>
{
    public void Assert(TStatement statement);
    public void Retract(TStatement statement);
}

public interface IUpdateable<in TUpdate>
{
    public void Update(TUpdate update);
}
```

### Metadata

```cs
public interface IHasIdentifier<out TId>
{
    public TId Id { get; }
}

// Here, TAbout might be a graph
public interface IHasAbout<TSelf, out TAbout>
    where TSelf : IHasAbout<TSelf, TAbout>
{
    public TAbout About { get; }
}

// Here, TAbout might be a dataset, TKey might be an IRefNode?, TValue might be a graph
public interface IHasAbout<TSelf, out TAbout, in TKey, out TValue> : IHasAbout<TSelf, TAbout>
    where TSelf : IHasAbout<TSelf, TAbout, TKey, TValue>
    where TAbout : IReadOnlyMap<TKey, TValue>
{ }
```

### Contraints

```cs
public interface IConstraint<in TInput>
{
    public bool Validate(TInput input);
}

public interface IConstraint<in TInput, TResult> : IConstraint<TInput>
{
    public bool Validate(TInput input, out TResult result);
}
```

### Extensibility Example

```cs
public static partial class Extensions
{
    extension<TSelf, TAbout, TKey, TValue>(IHasAbout<TSelf, TAbout, TKey, TValue> self)
        where TSelf : IHasAbout<TSelf, TAbout, TKey, TValue>, IHasIdentifier<INode>
        where TAbout : IReadOnlyMap<TKey, TValue>
        where TValue : ITellable<Triple>
    {
        public void Assert(INode predicate, INode @object, TKey graphKey)
        {
            TSelf thing = (TSelf)self;
            thing.About[graphKey].Assert(new Triple(thing.Id, predicate, @object));
        }

        public void Retract(INode predicate, INode @object, TKey graphKey)
        {
            TSelf thing = (TSelf)self;
            thing.About[graphKey].Retract(new Triple(thing.Id, predicate, @object));
        }
    }
}
```

### Related Collections

```cs
public interface IReadOnlyMap<in TKey, TValue>
{
    public TValue this[TKey key] { get; }

    public bool ContainsKey(TKey key);
    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value);
}

public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyMap<TKey, TValue>
{
    public IEnumerable<TKey> Keys { get; }
    public IEnumerable<TValue> Values { get; }
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

## Working Memory and Buffers

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

## Concurrency

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

    public async Task Execute(TElement arg, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(Procedures.Select(p => p.Execute(arg, cancellationToken)));
    }

    Task IProcedure.Execute(object arg, CancellationToken cancellationToken)
    {
        if (arg is TElement element)
        {
            return Execute(element, cancellationToken);
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

### Overlayability

```cs
public interface IOverlayable<out TOverlay>
{
    public TOverlay CreateOverlay();
}
```

### Collections

Collections, e.g., of type `ICollection<>` and `IDictionary<,>`, could be developed to support an overlay pattern where overlaid collections would reference parent collections, noting additions and removals, to implement collection interfaces without having to copy all of the elements of their parent collections.

In theory, an overlay-manager component could asynchronously schedule the copying of elements from collections to descendent collections with overlays, removing references to overlaid collections to enable memory storage efficiency.

### Semantic Overlays

Semantic models can be described as being collections of statements, triples or quads. Instead of having to copy a semantic model each time that a descendent interpretation state is created, a _semantic overlay_ system could be developed where subsequent nodes could, internally, provide overlays atop their predecessors' datasets.

In theory, semantic interpretation states could, using an overlay-manager component, asynchronously migrate their predecessors' semantic datasets into their overlaid datasets, deferring any needed copying processes until times convenient to systems.

## Cognitive Workflow and Timelines

The `IDifferenceable<>` and `IProcedure<>` pattern, sketched above, could be expanded into a fuller _cognitive workflow_ system for describing and visualizing processes of simulated cognition occurring as a result of the processing of inputs and sequences of inputs.

Alternatively, a _cognitive timeline_ system could be explored to provide multiple concurrent tracks of activities for describing and visualizing modeled and simulated processes.

## Communication and Question-answering

While `IHasModel<,>` provides a model which could be queried or otherwise inspected, an interface can be created for a second variety of presenting prompts or questions to systems, one where state changes are expected of systems when responding.

```cs
public interface ICommunicator<TSelf, in TInput, TOutput>
    where TSelf : ICommunicator<TSelf, TInput, TOutput>
{
    public Task<TSelf> Prompt(TInput prompt, CancellationToken cancellationToken = default);

    public bool TryGetContent([NotNullWhen(true)] out TOutput? content);
}

public interface ISequentialCommunicator<TSelf, in TInput, TOutput>
    where TSelf : ISequentialCommunicator<TSelf, TInput, TOutput>
{
    public Task<TSelf> Prompt(TInput prompt, CancellationToken cancellationToken = default);

    public bool TryGetContent([NotNullWhen(true)] out TOutput? content);

    public Task<TSelf> Continue(CancellationToken cancellationToken = default);
}
```
