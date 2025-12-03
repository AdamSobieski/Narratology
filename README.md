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

public interface ICuriousInterpretationNode<TSelf, in TInput, TDifference> :
    IInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : ICuriousInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : ICuriousDifference
{
    public IEnumerable<SparqlQuery> Questions { get; }
}

public interface IPredictiveInterpretationNode<TSelf, in TInput, TDifference> :
    IInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : IPredictiveInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IPredictiveDifference
{
    public IEnumerable<SparqlPrediction> Predictions { get; }
    public float Confidence(SparqlPrediction prediction);
}

public interface IDifferenceable<TSelf, TDifference>
    where TSelf : IDifferenceable<TSelf, TDifference>
{
    public TDifference Difference(TSelf other);
    public TSelf Apply(TDifference difference);
}

public interface ISemanticDifference
{
    public SparqlUpdateCommandSet Updates { get; }
}

public interface ICuriousDifference : ISemanticDifference
{
    public IEnumerable<SparqlQuery> ResolvedQuestions { get; }
    public IEnumerable<SparqlQuery> AddedQuestions { get; }
    public IEnumerable<SparqlQuery> UnresolvedRemovedQuestions { get; }
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

## Buffering, Chunks, and Segments

Depending upon the nature of `TInput`, one could add capabilities for incremental interpreters and comprehenders to be able to buffer arriving inputs into abstract chunks or segments.

```cs
public interface IBufferingInterpretationNode<TSelf, TInput, TDifference> :
    IInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : IBufferingInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IBufferingDifference<TInput>
{
    public IEnumerable<TInput> Buffer { get; }
}

public interface IBufferingDifference<out TInput> : ISemanticDifference
{
    public IEnumerable<TInput> BufferEnqueued { get; }
    public IEnumerable<TInput> BufferDequeued { get; }
}
```

## Example

Using the above interfaces, one could implement classes resembling:

```cs
public class StoryEvent
{
    ...
}

public class StoryNodeDifference :
    ICuriousDifference,
    IAttentionalChange<SparqlQuery>,
    IPredictiveDifference,
    IAttentionalChange<SparqlPrediction>
{
    ...
}

public class StoryNode :
    IAttentionalCuriousInterpretationNode<StoryNode, StoryEvent, StoryNodeDifference>,
    IAttentionalPredictiveInterpretationNode<StoryNode, StoryEvent, StoryNodeDifference>
{
    ...
}
```
