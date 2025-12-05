## Introduction

```cs
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;

using SparqlPrediction = (VDS.RDF.Query.SparqlQuery Query,
                          VDS.RDF.Query.SparqlResultSet Result);

public interface IDifferenceable<TSelf>
    where TSelf : IDifferenceable<TSelf>
{
    public Task<Operation?> DifferenceFrom(TSelf other);
    public Task<TSelf> Apply(Operation? difference);
}

public abstract class Operation { }

public sealed class CompoundOperation :
    Operation
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
```

## Incremental Interpretation and Comprehension

```cs
public interface IInterpretationState<TSelf, in TInput>
    where TSelf : IInterpretationState<TSelf, TInput>
{
    public IAsyncEnumerable<TSelf> Interpret(TInput input);
}
```

## Semantics

```cs
public interface ISemanticState<TSelf> :
    IDifferenceable<TSelf>
    where TSelf : ISemanticState<TSelf>
{
    public IInMemoryQueryableStore Model { get; }
}

public sealed class SemanticOperation :
    Operation
{
    public SemanticOperation
    (
        SparqlUpdateCommandSet updateCommands
    )
    {
        UpdateCommands = updateCommands;
    }

    public SparqlUpdateCommandSet UpdateCommands { get; }
}
```

## Curiosity

```cs
public interface ICuriousState<TSelf> :
    IDifferenceable<TSelf>
    where TSelf : ICuriousState<TSelf>
{
    public IEnumerable<SparqlQuery> Questions { get; }
}

public sealed class CuriousOperation :
    Operation
{
    public enum OperationStatus
    {
        Added,
        Removed,
        Resolved
    }

    public CuriousOperation
    (
        OperationStatus status,
        SparqlQuery question
    )
    {
        Status = status;
        Question = question;
    }

    public OperationStatus Status { get; }
    public SparqlQuery Question { get; }
}
```

## Prediction

```cs
public interface IPredictiveState<TSelf> :
    IDifferenceable<TSelf>
    where TSelf : IPredictiveState<TSelf>
{
    public IEnumerable<SparqlPrediction> Predictions { get; }
    public float Confidence(SparqlPrediction prediction);
}

public sealed class PredictiveOperation :
    Operation
{
    public enum OperationStatus
    {
        Added,
        Removed,
        Changed,
        Resolved
    }

    public PredictiveOperation
    (
        OperationStatus status,
        SparqlPrediction prediction,
        float confidenceChange = 0.0f
    )
    {
        Status = status;
        Prediction = prediction;
        ConfidenceChange = confidenceChange;
    }

    public OperationStatus Status { get; }
    public SparqlPrediction Prediction { get; }
    public float ConfidenceChange { get; }
}
```

## Attention

One could add capabilities for systems to simulate the distribution or allocation of attention to things, e.g., to their questions and predictions. This would be one means of prioritizing or sorting systems' questions and predictions.

```cs
public interface IAttentionalState<TSelf> :
    IDifferenceable<TSelf>
    where TSelf : IAttentionalState<TSelf>
{
    public float Attention(object value);
}

public sealed class AttentionalOperation :
    Operation
{
    public AttentionalOperation
    (
        object value,
        float attentionChange
    )
    {
        Value = value;
        AttentionChange = attentionChange;
    }

    public object Value { get; }
    public float AttentionChange { get; }
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

public interface IBufferSystem :
    IReadOnlyList<IBuffer>
{ }

public interface IBufferingState<TSelf> :
    IDifferenceable<TSelf>
    where TSelf : IBufferingState<TSelf>
{
    public IBufferSystem Buffers { get; }
}

public sealed class BufferSystemOperation :
    Operation
{
    public enum OperationStatus
    {
        Added,
        Removed,
        Compression,
        Decompression
    }

    public BufferSystemOperation
    (
        OperationStatus status,
        int? fromBuffer,
        IEnumerable? fromSequence,
        int? toBuffer,
        IEnumerable? toSequence
    )
    {
        Status = status;
        FromBuffer = fromBuffer;
        FromSequence = fromSequence;
        ToBuffer = toBuffer;
        ToSequence = toSequence;
    }

    public OperationStatus Status { get; }

    public int? FromBuffer { get; }
    public int? ToBuffer { get; }
    public IEnumerable? FromSequence { get; }
    public IEnumerable? ToSequence { get; }
}
```

## Concurrency, Threads, and Multitasking

Approaches to incremental interpretation and comprehension can tackle concurrency, threads, and multitasking in a number of ways.

With respect to concurrency regarding operations affecting differencing, one could add the following to express a set of `Operation` instances as occurring concurrently:

```cs
public sealed class ConcurrentOperation :
    Operation
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

While `ISemanticState<,>` provides a `Model` property of type `IInMemoryQueryableStore` which could be queried or otherwise inspected, an interface can be created for a second variety of presenting questions to systems, asking questions where state changes are expected of systems, where modeled and simulated procesess of cognition are expected to occur, during the processes of answering the questions.

```cs
public interface ICommunicatorState<TSelf, in TInput, TOutput> :
    IDifferenceable<TSelf>
    where TSelf : ICommunicatorState<TSelf, TInput, TOutput>
{
    public Task<TSelf> Prompt(TInput prompt);

    public bool HasContent { get; }
    public bool GetContent([NotNullWhen(true)] out TOutput? content);
}

public interface ISequentialCommunicatorState<TSelf, in TInput, TOutput> :
    IDifferenceable<TSelf>
    where TSelf : ISequentialCommunicatorState<TSelf, TInput, TOutput>
{
    public Task<TSelf> Prompt(TInput prompt);

    public bool HasContent { get; }
    public bool GetContent([NotNullWhen(true)] out TOutput? content);

    public Task<TSelf> Continue();
}

public interface IQueryableState<TSelf> :
    ICommunicatorState<TSelf, SparqlQuery, SparqlResultSet>
    where TSelf : IQueryableState<TSelf>
{ }
```

One could provide extension methods in a manner resembling:

```cs
public static partial class Extensions
{
    extension<TSelf, TInput, TOutput>
        (ISequentialCommunicatorState<TSelf, TInput, TOutput> state)
        where TSelf : ISequentialCommunicatorState<TSelf, TInput, TOutput>
    {
        public async IAsyncEnumerable<(TSelf State, TOutput Value)>
            PromptAsyncEnumerable(TInput prompt)
        {
            TSelf current = await state.Prompt(prompt);

            while(current.HasContent)
            {
                if(current.GetContent(out TOutput? value))
                {
                    yield return (current, value);
                }
                else
                {
                    throw new Exception();
                }

                current = await current.Continue();
            }
        }
    }
}
```

## Examples

Using the interfaces presented, above, one could implement classes resembling:

```cs
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
    ISemanticState<ReaderState>,
    ICuriousState<ReaderState>,
    IPredictiveState<ReaderState>,
    IBufferingState<ReaderState>,
    IAttentionalState<ReaderState>,
    IQueryableState<ReaderState>
{
    public IInMemoryQueryableStore Model
    {
        get { ... }
    }

    public IEnumerable<SparqlQuery> Questions
    {
        get { ... }
    }

    public IEnumerable<SparqlPrediction> Predictions
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

    public float Attention(object value) { ... }

    public float Confidence(SparqlPrediction prediction) { ... }

    public async Task<ReaderState> Prompt(SparqlQuery query) { ... }

    public bool GetContent([NotNullWhen(true)] out SparqlResultSet? result) { ... }

    ...
}
```
