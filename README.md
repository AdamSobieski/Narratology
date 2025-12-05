## Incremental Interpretation and Comprehension

```cs
using System.Collections;
using System.ComponentModel;
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
    public IReadOnlyList<SparqlQuery> ResolvedQuestions { get; }
    public IReadOnlyList<SparqlQuery> AddedQuestions { get; }
    public IReadOnlyList<SparqlQuery> UnresolvedRemovedQuestions { get; }
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
    public IReadOnlyList<SparqlPrediction> ResolvedPredictionsCorrect { get; }
    public IReadOnlyList<SparqlPrediction> ResolvedPredictionsIncorrect { get; }
    public IReadOnlyList<SparqlPrediction> AddedPredictions { get; }
    public IReadOnlyList<SparqlPrediction> UnresolvedRemovedPredictions { get; }
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
{
    public bool TryCompress
    (
        object context,
        int fromBuffer,
        int toBuffer,
        IEnumerable sequence,
        out object chunk
    );

    public bool TryDecompress
    (
        object context,
        int fromBuffer,
        int toBuffer,
        object chunk,
        out IEnumerable sequence
    );
}

public interface IBufferingInterpretationNode<TSelf, TDifference, TInput> :
    IInterpretationNode<TSelf, TDifference, TInput>
    where TSelf : IBufferingInterpretationNode<TSelf, TDifference, TInput>
    where TDifference : IBufferingDifference
{
    public IBufferSystem Buffers { get; }
}

public interface IBufferingDifference : ISemanticDifference
{
    public IReadOnlyList<(int Buffer,
        CollectionChangeEventArgs Action)> BufferChanges { get; }
}
```

## Concurrency, Threads, and Multitasking

Approaches to incremental interpretation and comprehension can tackle concurrency, threads, and multitasking in a number of ways.

Firstly, depending upon the nature of the type of input, one input object instance could describe multiple happenings simultaneously.

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
  IBufferingInterpretationNode<ReaderNode, ReaderNodeDifference, StoryChunk>
{
    ...
}

public class ReaderNodeDifference :
  ICuriousDifference,
  IAttentionalChange<SparqlQuery>,
  IPredictiveDifference,
  IAttentionalChange<SparqlPrediction>,
  IBufferingDifference
{
    ...
}
```
