## Incremental Interpretation and Comprehension

```cs
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;

using SparqlPrediction = (VDS.RDF.Query.SparqlQuery Query,
                          VDS.RDF.Query.SparqlResultSet Result);

public interface IInterpretationNode<TSelf, in TInput, TDifference> :
    IDifferenceable<TSelf, TDifference>
    where TSelf : IInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : ISemanticDifference
{
    public IInMemoryQueryableStore Model { get; }

    public IEnumerable<TSelf> Interpret(TInput input);
}

public interface ISemanticDifference
{
    public SparqlUpdateCommandSet Updates { get; }
}

public interface IDifferenceable<TSelf, TDifference>
    where TSelf : IDifferenceable<TSelf, TDifference>
{
    public TDifference Difference(TSelf other);
    public TSelf Apply(TDifference difference);
}
```

## Curiosity

```cs
public interface ICuriousInterpretationNode<TSelf, in TInput, TDifference> :
    IInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : ICuriousInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : ICuriousDifference
{
    public IEnumerable<SparqlQuery> Questions { get; }
}

public interface ICuriousDifference : ISemanticDifference
{
    public IEnumerable<SparqlQuery> ResolvedQuestions { get; }
    public IEnumerable<SparqlQuery> AddedQuestions { get; }
    public IEnumerable<SparqlQuery> UnresolvedRemovedQuestions { get; }
}
```

## Prediction

```cs
public interface IPredictiveInterpretationNode<TSelf, in TInput, TDifference> :
    IInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : IPredictiveInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IPredictiveDifference
{
    public IEnumerable<SparqlPrediction> Predictions { get; }
    public float Confidence(SparqlPrediction prediction);
}

public interface IPredictiveDifference : ISemanticDifference
{
    public IEnumerable<SparqlPrediction> ResolvedPredictionsCorrect { get; }
    public IEnumerable<SparqlPrediction> ResolvedPredictionsIncorrect { get; }
    public IEnumerable<SparqlPrediction> AddedPredictions { get; }
    public IEnumerable<SparqlPrediction> UnresolvedRemovedPredictions { get; }
    public float ConfidenceChange(SparqlPrediction prediction);
}
```

## Attention and Focus

One could add capabilities for systems to simulate the distribution or allocation of attention to things, e.g., to their questions and predictions. This would be one means of prioritizing or sorting systems' questions and predictions.

```cs
public interface IAttentional<in T>
{
    public float Attention(T value);
}

public interface IAttentionalChange<in T>
{
    public float AttentionChange(T value);
}

public interface IAttentionalCuriousInterpretationNode<TSelf, in TInput, TDifference> :
    ICuriousInterpretationNode<TSelf, TInput, TDifference>,
    IAttentional<SparqlQuery>
    where TSelf : IAttentionalCuriousInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : ICuriousDifference, IAttentionalChange<SparqlQuery>
{ }

public interface IAttentionalPredictiveInterpretationNode<TSelf, in TInput, TDifference> :
    IPredictiveInterpretationNode<TSelf, TInput, TDifference>,
    IAttentional<SparqlPrediction>
    where TSelf : IAttentionalPredictiveInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IPredictiveDifference, IAttentionalChange<SparqlPrediction>
{ }
```

## Working Memory Buffers, Chunks, Segments, and Compression

Depending upon the nature of `TInput`, one could add capabilities for incremental interpreters and comprehenders to be able to buffer arriving inputs, perhaps to form them into chunks or segments.

```cs
public interface IShortTermBufferingInterpretationNode<TSelf, TInput, TDifference> :
    IInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : IShortTermBufferingInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IShortTermBufferingDifference<TInput>
{
    public IReadOnlyCollection<TInput> ShortTermBuffer { get; }
}

public interface IShortTermBufferingDifference<out TInput> : ISemanticDifference
{
    public IReadOnlyCollection<TInput> ShortTermBufferAdded { get; }
    public IReadOnlyCollection<TInput> ShortTermBufferRemoved { get; }
}
```

Depending upon the nature of `TInput`, one could also "compress" sequences of inputs into chunks or segments to store in secondary buffers and could subsequently "decompress" individual chunks or segments back into input sequences as needed.

A system could, for example, "compress" some of the contents of its `ShortTermBuffer` of type `TInput` into its `MediumTermBuffer` of type `TChunk`, `TSegment`, or `TEpisode`.

```cs
public interface ICompressor<TFrom, out TTo>
{
    public TTo Compress(IEnumerable<TFrom> inputs);
}

public interface IDecompressor<out TFrom, in TTo>
{
    public IEnumerable<TFrom> Decompress(TTo input);
}
```

```cs
public interface IMediumTermBufferingInterpretationNode<TSelf, TInput, TChunk, TDifference> :
    IShortTermBufferingInterpretationNode<TSelf, TInput, TDifference>,
    ICompressor<TInput, TChunk>,
    IDecompressor<TInput, TChunk>
    where TSelf : IMediumTermBufferingInterpretationNode<TSelf, TInput, TChunk, TDifference>
    where TDifference : IMediumTermBufferingDifference<TInput, TChunk>
{
    public IReadOnlyCollection<TChunk> MediumTermBuffer { get; }
}

public interface IMediumTermBufferingDifference<out TInput, out TChunk> :
    IShortTermBufferingDifference<TInput>
{
    public IReadOnlyCollection<TChunk> MediumTermBufferAdded { get; }
    public IReadOnlyCollection<TChunk> MediumTermBufferRemoved { get; }
}
```

Notice that a type could be closed under compression and decompression operations: `ICompressor<T, T>` and `IDecompressor<T, T>`.

For example, such a type could inherit from:

```cs
public interface ITree<out TSelf>
{
    TSelf? Parent { get; }
    public IReadOnlyCollection<TSelf> Children { get; }
}
```

## Concurrency, Threads, and Multitasking

Approaches to incremental interpretation and comprehension can tackle concurrency, threads, and multitasking in a number of ways.

Firstly, depending upon the nature of `TInput`, one input could describe multiple happenings simultaneously.

Secondly, events from different story threads could be interwoven together and presented to a system serially.

Thirdly, a system could have multiple incremental interpreters and comprehenders, one per cognitive executive task, and could task-switch between these when story threads switched in a narration.

## Advanced Differencing, Actions, and Lambda Expressions

Revisiting differencing, obtaining differences or deltas between instances of a type and being able to apply these to object instances, in theory, a difference between two instances of some types could provide a sequence of simple operations to inspect and/or to perform upon a clone of one object to obtain an object equivalent to another.

That is, one could simplify differences to a more abstract representation, perhaps in a manner resembling:

```cs
public interface IOperationSequenceDifference<TOperation>
{
    public IReadOnlyCollection<TOperation> Operations { get; }
}
```
or:
```cs
public interface IOperationSequenceDifference<TSelf>
{
    public IReadOnlyCollection<Action<TSelf>> Operations { get; }
}
```
or:
```cs
public interface IOperationSequenceDifference<TSelf>
{
    public IReadOnlyCollection<Expression<Action<TSelf>>> Operations { get; }
}
```

## Examples

Using the interfaces presented, above, one could implement classes resembling:

```cs
public class StoryChunk : ITree<StoryChunk>
{
    ...
}

public class StoryNode :
    IAttentionalCuriousInterpretationNode<StoryNode, StoryChunk, StoryNodeDifference>,
    IAttentionalPredictiveInterpretationNode<StoryNode, StoryChunk, StoryNodeDifference>,
    IMediumTermBufferingInterpretationNode<StoryNode, StoryChunk, StoryChunk, StoryNodeDifference>
{
    ...
}

public class StoryNodeDifference :
    ICuriousDifference,
    IAttentionalChange<SparqlQuery>,
    IPredictiveDifference,
    IAttentionalChange<SparqlPrediction>,
    IMediumTermBufferingDifference<StoryChunk, StoryChunk>
{
    ...
}
```
