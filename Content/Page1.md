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
    IInterpretationState<ReaderState, StoryChunk>,
    IDifferenceable<ReaderState>,
    ISemanticModelState<ReaderState, IInMemoryQueryableStore>,
    ICuriousState<ReaderState, SparqlQuery>,
    IPredictiveState<ReaderState, SparqlPrediction>,
    IBufferingState<ReaderState>,
    IAttentionalState<ReaderState, SparqlQuery>,
    IAttentionalState<ReaderState, SparqlPrediction>,
    IConfidenceState<ReaderState, SparqlPrediction>,
    IQueryableState<ReaderState>
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

    public bool HasContent
    {
        get { ... }
    }

    public async IAsyncEnumerable<ReaderState> Interpret(StoryChunk input) { ... }

    public async Task<Operation?> Difference(ReaderState other) { ... }

    public async Task<ReaderState> Apply(Operation? difference) { ... }

    public float GetAttention(SparqlQuery item) { ... }

    public float GetAttention(SparqlPrediction item) { ... }

    public void SetAttention(SparqlQuery item, float value) { ... }

    public void SetAttention(SparqlPrediction item, float value) { ... }

    public float GetConfidence(SparqlPrediction item) { ... }

    public void SetConfidence(SparqlPrediction item, float value) { ... }

    public async Task<ReaderState> Prompt(SparqlQuery query) { ... }

    public bool GetContent([NotNullWhen(true)] out SparqlResultSet? result) { ... }

    ...
}
```

## Differencing

```cs
public interface IDifferenceable<TSelf>
    where TSelf : IDifferenceable<TSelf>
{
    public Task<Operation?> DifferenceFrom(TSelf other);
    public Task<TSelf> Apply(Operation? difference);
}
```

### Operations

```cs
public abstract class Operation { }

public sealed class CompoundOperation : Operation
{
    public CompoundOperation
    (
        IEnumerable<Operation> operations
    )
    {
        Operations = operations;
    }

    public IEnumerable<Operation> Operations { get; }
}

public class LambdaExpressionOperation : Operation
{
    public LambdaExpressionOperation(LambdaExpression lambda)
    {
        LambdaExpression = lambda;
        m_delegate = null;
    }

    public LambdaExpression LambdaExpression { get; }
        
    private Delegate? m_delegate;

    public Delegate Compiled
    {
        get
        {
            if (m_delegate == null)
            {
                m_delegate = LambdaExpression.Compile();
            }
            return m_delegate;
        }
    }
}
```

### Extensions

```cs
public static partial class Extensions
{
    extension<TSelf>(IDifferenceable<TSelf> differenceable)
        where TSelf : IDifferenceable<TSelf>
    {
        public Operation CreateOperation(Expression<Action<TSelf>> expression)
        {
            return new LambdaExpressionOperation(expression);
        }
    }
}
```

## Incremental Interpretation and Comprehension

```cs
public interface IInterpretationState<TSelf, in TInput>
    where TSelf : IInterpretationState<TSelf, TInput>
{
    public IAsyncEnumerable<TSelf> Interpret(TInput input);
}
```

## Semantic Modeling

```cs
public interface ISemanticState<TSelf, out TModel> : IDifferenceable<TSelf>
    where TSelf : ISemanticState<TSelf, TModel>
{
    public TModel Model { get; }
}
```

## Curiosity

```cs
public interface ICuriousState<TSelf, TQuestion> : IDifferenceable<TSelf>
    where TSelf : ICuriousState<TSelf, TQuestion>
{
    public ICollection<TQuestion> Questions { get; }
}
```

## Prediction

```cs
public interface IPredictiveState<TSelf, TPrediction> : IDifferenceable<TSelf>
    where TSelf : IPredictiveState<TSelf, TPrediction>
{
    public ICollection<TPrediction> Predictions { get; }
}
```

## Confidence

```cs
public interface IConfidenceState<TSelf, in TElement> : IDifferenceable<TSelf>
    where TSelf : IConfidenceState<TSelf, TElement>
{
    public float GetConfidence(TElement item);
    public void SetConfidence(TElement item, float value);
}
```

## Attention

One could add capabilities for systems to simulate the distribution or allocation of attention to things, e.g., to their questions and predictions. This would be one means of prioritizing or sorting systems' questions and predictions.

```cs
public interface IAttentionalState<TSelf, in TElement> : IDifferenceable<TSelf>
    where TSelf : IAttentionalState<TSelf, TElement>
{
    public float GetAttention(TElement item);
    public void SetAttention(TElement item, float value);
}
```

## Working Memory, Buffers, Chunks, and Compression

Depending upon the nature of the input, one could add capabilities for incremental interpreters and comprehenders to be able to buffer arriving inputs. One could also "compress" buffered sequences of inputs into chunks to store these chunks in secondary buffers and subsequently "decompress" these chunks back into input sequences, in primary buffers, as needed. That is, a system could "compress" some of the contents of its primary buffer into a secondary buffer. Similarly, tertiary buffers &ndash; and beyond &ndash; could be considered.

A buffer system could, then, might resemble:

```cs
public interface IBuffer : ICollection
{
    public Type ElementType { get; }
}

public interface IBufferSystem : IReadOnlyList<IBuffer>
{ }

public interface IBufferingState<TSelf> : IDifferenceable<TSelf>
    where TSelf : IBufferingState<TSelf>
{
    public IBufferSystem Buffers { get; }
}
```

## Concurrency, Threads, and Multitasking

Approaches to incremental interpretation and comprehension can tackle concurrency, threads, and multitasking in a number of ways.

With respect to concurrency regarding operations affecting differencing, one could add the following to express a set of `Operation` instances as occurring concurrently:

```cs
public sealed class ConcurrentOperation : Operation
{
    public ConcurrentOperation
    (
        IEnumerable<Operation> operations
    )
    {
        Operations = operations;
    }

    public IEnumerable<Operation> Operations { get; }
}
```

With respect to processing concurrent happenings, depending upon the nature of the type of input, one input object instance could describe multiple happenings simultaneously.

With respect to story threads in a narration, events from different story threads could be presented to a system sequentially.

A system could have multiple incremental interpreters and comprehenders, one per cognitive executive task, and could task-switch between them when story threads switched in a narration.

## Semantic Overlays

Instead of having to copy a semantic model for each interpretation of each input, a _semantic overlay_ framework could be developed where subsequent nodes could, internally, provide overlays atop their predecessors' datasets while implementing the `IInMemoryQueryableStore` interface.

Semantic interpretation nodes could, then, manually or automatically, asynchronously migrate their predecessors' semantic datasets into themselves, deferring any needed copying processes until convenient to systems.

## Cognitive Workflow and Timelines

The `IDifferenceable<>` and `Operation` pattern, sketched above, could be expanded into a fuller _cognitive workflow_ system for describing and visualizing modeled and simulated processes of cognition occurring as a result of the processing of inputs and sequences of inputs.

Alternatively, a _cognitive timeline_ system could be explored to provide multiple concurrent tracks of activities for describing and visualizing modeled and simulated processes.

## Communication and Question-answering

While `ISemanticState<,>` provides a `Model` property which could be queried or otherwise inspected, an interface can be created for a second variety of presenting prompts or questions to systems, one where state changes are expected of systems when responding.

```cs
public interface ICommunicatorState<TSelf, in TInput, TOutput> : IDifferenceable<TSelf>
    where TSelf : ICommunicatorState<TSelf, TInput, TOutput>
{
    public Task<TSelf> Prompt(TInput prompt);

    public bool HasContent { get; }
    public bool GetContent([NotNullWhen(true)] out TOutput? content);
}

public interface ISequentialCommunicatorState<TSelf, in TInput, TOutput> : IDifferenceable<TSelf>
    where TSelf : ISequentialCommunicatorState<TSelf, TInput, TOutput>
{
    public Task<TSelf> Prompt(TInput prompt);

    public bool HasContent { get; }
    public bool GetContent([NotNullWhen(true)] out TOutput? content);

    public Task<TSelf> Continue();
}

public interface IQueryableState<TSelf> : ICommunicatorState<TSelf, SparqlQuery, SparqlResultSet>
    where TSelf : IQueryableState<TSelf>
{ }
```
