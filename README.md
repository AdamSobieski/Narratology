## Incremental Interpretation and Comprehension

```cs
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;

using SparqlPrediction = (VDS.RDF.Query.SparqlQuery Query,
                          VDS.RDF.Query.SparqlResultSet Result);

public interface INode<TSelf, in TInput>
    where TSelf : INode<TSelf, TInput>
{
    public IEnumerable<TSelf> Process(TInput input);
}

public interface IDifferenceable<TSelf, TDifference>
where TSelf : IDifferenceable<TSelf, TDifference>
{
    public TDifference Difference(TSelf other);
    public TSelf Apply(TDifference difference);
}

public interface IInterpretationNode<TSelf, TDifference, in TInput> :
    INode<TSelf, TInput>,
    IDifferenceable<TSelf, TDifference>
    where TSelf : IInterpretationNode<TSelf, TDifference, TInput>
    where TDifference : ISemanticDifference
{
    public IInMemoryQueryableStore Model { get; }
}

public interface ISemanticDifference
{
    public SparqlUpdateCommandSet Updates { get; }
}
```

## Curiosity

```cs
public interface ICuriousInterpretationNode<TSelf, TDifference, in TInput> :
    IInterpretationNode<TSelf, TDifference, TInput>
    where TSelf : ICuriousInterpretationNode<TSelf, TDifference, TInput>
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
public interface IPredictiveInterpretationNode<TSelf, TDifference, in TInput> :
    IInterpretationNode<TSelf, TDifference, TInput>
    where TSelf : IPredictiveInterpretationNode<TSelf, TDifference, TInput>
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

public interface IAttentionalCuriousInterpretationNode<TSelf, TDifference, in TInput> :
    ICuriousInterpretationNode<TSelf, TDifference, TInput>,
    IAttentional<SparqlQuery>
    where TSelf : IAttentionalCuriousInterpretationNode<TSelf, TDifference, TInput>
    where TDifference : ICuriousDifference, IAttentionalChange<SparqlQuery>
{ }

public interface IAttentionalPredictiveInterpretationNode<TSelf, TDifference, in TInput> :
    IPredictiveInterpretationNode<TSelf, TDifference, TInput>,
    IAttentional<SparqlPrediction>
    where TSelf : IAttentionalPredictiveInterpretationNode<TSelf, TDifference, TInput>
    where TDifference : IPredictiveDifference, IAttentionalChange<SparqlPrediction>
{ }
```

## Working Memory, Buffers, Chunks, and Compression

Depending upon the nature of the input, `T1`, one could add capabilities for incremental interpreters and comprehenders to be able to buffer arriving inputs, perhaps to form them into chunks or segments.

```cs
public interface IBufferingInterpretationNode<TSelf, TDifference, T1> :
    IInterpretationNode<TSelf, TDifference, T1>
    where TSelf : IBufferingInterpretationNode<TSelf, TDifference, T1>
    where TDifference : IBufferingDifference<T1>
{
    public IReadOnlyCollection<T1> Buffer1 { get; }
}

public interface IBufferingDifference<out T1> : ISemanticDifference
{
    public IReadOnlyCollection<T1> Buffer1Added { get; }
    public IReadOnlyCollection<T1> Buffer1Removed { get; }
}
```

Depending upon the nature of the input, `T1`, one could also "compress" sequences of inputs into chunks or segments to store in secondary buffers and could subsequently "decompress" individual chunks or segments back into input sequences as needed.

A system could, for example, "compress" some of the contents of its `Buffer1` of type `T1` into its `Buffer2` of type `T2`.

```cs
public interface IBufferingInterpretationNode<TSelf, TDifference, T1, T2> :
    IBufferingInterpretationNode<TSelf, TDifference, T1>
    where TSelf : IBufferingInterpretationNode<TSelf, TDifference, T1, T2>
    where TDifference : IBufferingDifference<T1, T2>
{
    public IReadOnlyCollection<T2> Buffer2 { get; }

    public T2 Compress(IEnumerable<T1> sequence);
    public IEnumerable<T1> Decompress(T2 chunk);
}

public interface IBufferingDifference<out T1, out T2> :
    IBufferingDifference<T1>
{
    public IReadOnlyCollection<T2> Buffer2Added { get; }
    public IReadOnlyCollection<T2> Buffer2Removed { get; }
}
```

```cs
public interface IBufferingInterpretationNode<TSelf, TDifference, T1, T2, T3> :
    IBufferingInterpretationNode<TSelf, TDifference, T1, T2>
    where TSelf : IBufferingInterpretationNode<TSelf, TDifference, T1, T2, T3>
    where TDifference : IBufferingDifference<T1, T2>
{
    public IReadOnlyCollection<T3> Buffer3 { get; }

    public T3 Compress(IEnumerable<T2> sequence);
    public IEnumerable<T2> Decompress(T3 chunk);
}

public interface IBufferingDifference<out T1, out T2, out T3> :
    IBufferingDifference<T1, T2>
{
    public IReadOnlyCollection<T3> Buffer3Added { get; }
    public IReadOnlyCollection<T3> Buffer3Removed { get; }
}
```

It may be the case that `T1` equals `T2` equals `T3`, for example if that input type compresses to itself, for instance if it inherits from `ITree<>`:

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

## Examples

Using the interfaces presented, above, one could implement classes resembling:

```cs
public class StoryChunk :
  ITree<StoryChunk>
{
    ...
}

public class ReaderNode :
  IAttentionalCuriousInterpretationNode<ReaderNode, ReaderNodeDifference, StoryChunk>,
  IAttentionalPredictiveInterpretationNode<ReaderNode, ReaderNodeDifference, StoryChunk>,
  IBufferingInterpretationNode<ReaderNode, ReaderNodeDifference, StoryChunk, StoryChunk, StoryChunk>
{
    ...
}

public class ReaderNodeDifference :
  ICuriousDifference,
  IAttentionalChange<SparqlQuery>,
  IPredictiveDifference,
  IAttentionalChange<SparqlPrediction>,
  IBufferingDifference<StoryChunk, StoryChunk, StoryChunk>
{
    ...
}
```
