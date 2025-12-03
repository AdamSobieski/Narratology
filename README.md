## Incremental Interpretation and Comprehension

```cs
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;
using SparqlPrediction = (VDS.RDF.Query.SparqlQuery Query,
                          VDS.RDF.Query.SparqlResultSet Result);

public interface IDifferenceable<TSelf, TDifference>
    where TSelf : IDifferenceable<TSelf, TDifference>
{
    public TDifference Difference(TSelf other);
    public TSelf Apply(TDifference difference);
}

public interface IDifference
{
    public SparqlUpdateCommandSet Updates { get; }
}

public interface ICuriousDifference : IDifference
{
    public IEnumerable<SparqlQuery> ResolvedQuestions { get; }
    public IEnumerable<SparqlQuery> AddedQuestions { get; }
    public IEnumerable<SparqlQuery> UnresolvedRemovedQuestions { get; }
}

public interface IPredictiveDifference : IDifference
{
    public IEnumerable<SparqlPrediction> ResolvedPredictionsCorrect { get; }
    public IEnumerable<SparqlPrediction> ResolvedPredictionsIncorrect { get; }
    public IReadOnlyDictionary<SparqlPrediction, float> PredictionsConfidenceChange { get; }
    public IEnumerable<SparqlPrediction> AddedPredictions { get; }
    public IEnumerable<SparqlPrediction> UnresolvedRemovedPredictions { get; }
}

public interface IAttentionalCuriousDifference : ICuriousDifference
{
    public IReadOnlyDictionary<SparqlQuery, float> QuestionsAttentionChange { get; }
}

public interface IAttentionalPredictiveDifference : IPredictiveDifference
{
    public IReadOnlyDictionary<SparqlPrediction, float> PredictionsAttentionChange { get; }
}

public interface IInterpretationNode<TSelf, in TInput, TDifference> :
    IDifferenceable<TSelf, TDifference>
    where TSelf : IInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IDifference
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

public interface IAttentionalCuriousInterpretationNode<TSelf, in TInput, TDifference> :
    ICuriousInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : ICuriousInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IAttentionalCuriousDifference
{
    public float Attention(SparqlQuery question);
}

public interface IAttentionalPredictiveInterpretationNode<TSelf, in TInput, TDifference> :
    IPredictiveInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : IPredictiveInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IAttentionalPredictiveDifference
{
    public float Attention(SparqlPrediction prediction);
}
```

Using the above interfaces, one could implement classes resembling:

```cs
public class StoryEvent
{
    ...
}

public class StoryNodeDifference :
    IAttentionalCuriousDifference,
    IAttentionalPredictiveDifference
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
