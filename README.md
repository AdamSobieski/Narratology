## Incremental Interpretation and Comprehension

```cs
using System.Collections;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;

using SparqlPrediction = (VDS.RDF.Query.SparqlQuery Query,
                          VDS.RDF.Query.SparqlResultSet Result);

public interface IInterpretationNode<TSelf, in TInput>
    where TSelf : IInterpretationNode<TSelf, TInput>
{
    public IEnumerable<TSelf> Interpret(TInput input);
}

public abstract class Operation { }

public interface IDifferenceable<TSelf>
    where TSelf : IDifferenceable<TSelf>
{
    public IEnumerable<Operation> Difference(TSelf other);
    public TSelf Apply(IEnumerable<Operation> difference);
}

public interface ISemanticNode<TSelf, in TInput> :
    IInterpretationNode<TSelf, TInput>,
    IDifferenceable<TSelf>
    where TSelf : ISemanticNode<TSelf, TInput>
{
    public IInMemoryQueryableStore Model { get; }
}

public sealed class SemanticOperation : Operation
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
public interface ICuriousNode<TSelf, in TInput> :
    ISemanticNode<TSelf, TInput>
    where TSelf : ICuriousNode<TSelf, TInput>
{
    public IEnumerable<SparqlQuery> Questions { get; }
}

public sealed class CuriousOperation : Operation
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
public interface IPredictiveNode<TSelf, in TInput> :
    ISemanticNode<TSelf, TInput>
    where TSelf : IPredictiveNode<TSelf, TInput>
{
    public IEnumerable<SparqlPrediction> Predictions { get; }
    public float Confidence(SparqlPrediction prediction);
}

public sealed class PredictiveOperation : Operation
{
    public enum OperationStatus
    {
        Added,
        Removed,
        Resolved,
        ConfidenceChanged
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

## Attention and Focus

One could add capabilities for systems to simulate the distribution or allocation of attention to things, e.g., to their questions and predictions. This would be one means of prioritizing or sorting systems' questions and predictions.

```cs
public interface IAttentionalNode<TSelf, in TInput> :
    ISemanticNode<TSelf, TInput>
    where TSelf : IAttentionalNode<TSelf, TInput>
{
    public float Attention(object value);
}

public sealed class AttentionalOperation : Operation
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

public interface IBufferSystem
    : IReadOnlyList<IBuffer>
{ }

public interface IBufferingNode<TSelf, TInput> :
    ISemanticNode<TSelf, TInput>
    where TSelf : IBufferingNode<TSelf, TInput>
{
    public IBufferSystem Buffers { get; }
}

public sealed class BufferSystemOperation : Operation
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

With respect to concurrency regarding operations affecting differencing, one could add the following to interpret a set of `Operation` instances as occurring concurrently:

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

More specifically, events from different story threads could be interwoven together and presented to a system sequentially.

Alternatively, a system could have multiple incremental interpreters and comprehenders, one per cognitive executive task, and could task-switch between these when story threads switched in a narration.

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

public class Reader :
    ICuriousNode<Reader, StoryChunk>,
    IPredictiveNode<Reader, StoryChunk>,
    IAttentionalNode<Reader, StoryChunk>,
    IBufferingNode<Reader, StoryChunk>
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

    public IEnumerable<Reader> Interpret(StoryChunk input) { ... }

    public IEnumerable<Operation> Difference(Reader other) { ... }

    public Reader Apply(IEnumerable<Operation> difference) { ... }

    public float Attention(object value) { ... }

    public float Confidence(SparqlPrediction prediction) { ... }

    ...
}
```
