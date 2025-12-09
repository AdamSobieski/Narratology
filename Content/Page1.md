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
    IHasSemanticModel<IInMemoryQueryableStore>,
    IHasQuestions<SparqlQuery>,
    IHasPredictions<SparqlPrediction>,
    IHasBuffers,
    IHasAttention<SparqlQuery>,
    IHasAttention<SparqlPrediction>,
    IHasConfidence<SparqlPrediction>,
    ICommunicator<ReaderState, SparqlQuery, SparqlResultSet>,
    IOperational<ReaderState>
{
    public IInMemoryQueryableStore Model
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

    public async Task<Operation<ReaderState>> DifferenceFrom(ReaderState other) { ... }

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

## Differencing

```cs
public interface IDifferenceable<TSelf>
    where TSelf : IDifferenceable<TSelf>
{
    public Task<Operation<TSelf>> DifferenceFrom(TSelf other);
}
```

### Operations

```cs
public interface IOperational<TOperand, TElement> { }

public interface IOperational<TOperand> : IOperational<TOperand, TOperand> { }

public interface ICustomCreateOperation<TOperand, TElement>
{
    public Operation<TOperand> CreateOperation(Action<TElement> action);
}

public interface IHasOperationalMap<TOperand, TElement>
{
    public Func<TOperand, TElement> OperationalMap { get; }
}

public interface IOperation
{
    public Task Execute(object arg);
}

public interface IOperation<in TElement> : IOperation
{
    public Task Execute(TElement arg);
}

public abstract class Operation<TElement> : IOperation<TElement>
{
    Task IOperation.Execute(object arg)
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

    public abstract Task Execute(TElement arg);
}

public sealed class ActionOperation<TElement> : Operation<TElement>
{
    public ActionOperation(Action<TElement> action)
    {
        m_action = action;
    }

    Action<TElement> m_action;

    public sealed override Task Execute(TElement arg)
    {
        return Task.Run(() => m_action(arg));
    }
}

public sealed class LambdaExpressionOperation<TElement> : Operation<TElement>
{
    public LambdaExpressionOperation(Expression<Action<TElement>> lambda)
    {
        LambdaExpression = lambda;
        m_delegate = null;
    }

    public Expression<Action<TElement>> LambdaExpression { get; }

    Action<TElement>? m_delegate;

    public sealed override Task Execute(TElement arg)
    {
        m_delegate ??= LambdaExpression.Compile();
        return Task.Run(() => m_delegate(arg));
    }
}

public sealed class CompoundOperation<TElement> : Operation<TElement>
{
    public CompoundOperation(IEnumerable<Operation<TElement>> operations)
    {
        Operations = operations;
    }

    public IEnumerable<Operation<TElement>> Operations { get; }

    public sealed override async Task Execute(TElement arg)
    {
        foreach (var operation in Operations)
        {
            await operation.Execute(arg);
        }
    }
}
```

### Extensions

```cs
public static partial class Extensions
{
    extension<TOperand, TElement>(IOperational<TOperand, TElement> operational)
    {
        public Operation<TOperand> CreateOperation(Action<TElement> action)
        {
            if (operational is ICustomCreateOperation<TOperand, TElement> custom)
            {
                return custom.CreateOperation(action);
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new ActionOperation<TOperand>((TOperand o) => action((TElement)(object)o!));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public IOperational<TOperand, TResult> Map<TResult>(Func<TElement, TResult> map)
        {
            if (operational is IHasOperationalMap<TOperand, TElement> hasMap)
            {
                var operationalMap = hasMap.OperationalMap;
                return new Map<TOperand, TResult>((TOperand o) => map(operationalMap(o)));
            }
            else if (typeof(TElement).IsAssignableFrom(typeof(TOperand)))
            {
                return new Map<TOperand, TResult>((TOperand o) => map((TElement)(object)o!));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}

class Map<TOperand, TResult> : 
    IOperational<TOperand, TResult>,
    ICustomCreateOperation<TOperand, TResult>,
    IHasOperationalMap<TOperand, TResult>
{
    public Map(Func<TOperand, TResult> map)
    {
        m_map = map;
    }

    Func<TOperand, TResult> m_map;

    public Func<TOperand, TResult> OperationalMap
    {
        get
        {
            return m_map;
        }
    }

    public Operation<TOperand> CreateOperation(Action<TResult> action)
    {
        return new ActionOperation<TOperand>((TOperand o) => action(m_map(o)));
    }
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

## Semantic Modeling

```cs
public interface IHasSemanticModel<out TModel>
{
    public TModel Model { get; }
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

With respect to concurrency regarding operations affecting differencing, one could add the following to express a set of `Operation` instances as occurring concurrently:

```cs
public sealed class ConcurrentOperation<TElement> : Operation<TElement>
{
    public ConcurrentOperation(IEnumerable<Operation<TElement>> operations)
    {
        Operations = operations;
    }

    public IEnumerable<Operation<TElement>> Operations { get; }

    public sealed override Task Execute(TElement arg)
    {
        return Task.WhenAll(Operations.Select(o => o.Execute(arg)));
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

The `IDifferenceable<>` and `Operation` pattern, sketched above, could be expanded into a fuller _cognitive workflow_ system for describing and visualizing modeled and simulated processes of cognition occurring as a result of the processing of inputs and sequences of inputs.

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
