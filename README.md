## Incremental Interpretation and Comprehension

```cs
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;
using SparqlPrediction = (float Confidence,
                          VDS.RDF.Query.SparqlQuery Query,
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
    public IEnumerable<(SparqlQuery Old, SparqlQuery New)> UpdatedQuestions { get; }
    public IEnumerable<(float Priority, SparqlQuery Query)> NewQuestions { get; }
}

public interface IPredictiveDifference : IDifference
{
    public IEnumerable<SparqlPrediction> CorrectlyResolvedPredictions { get; }
    public IEnumerable<SparqlPrediction> IncorrectlyResolvedPredictions { get; }
    public IEnumerable<(SparqlPrediction Old, SparqlPrediction New)> UpdatedPredictions { get; }
    public IEnumerable<(float Priority, SparqlPrediction Prediction)> NewPredictions { get; }
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
    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }
}

public interface IPredictiveInterpretationNode<TSelf, in TInput, TDifference> :
    IInterpretationNode<TSelf, TInput, TDifference>
    where TSelf : IPredictiveInterpretationNode<TSelf, TInput, TDifference>
    where TDifference : IPredictiveDifference
{
    public IEnumerable<(float Priority, SparqlPrediction Prediction)> Predictions { get; }
}
```

Using the above interfaces, one could implement classes resembling:

```cs
public class StoryEvent
{
    ...
}

public class StoryInterpretationNodeDifference :
    ICuriousDifference,
    IPredictiveDifference
{
    ...
}

public class StoryInterpretationNode :
    ICuriousInterpretationNode<StoryInterpretationNode, StoryEvent, StoryInterpretationNodeDifference>,
    IPredictiveInterpretationNode<StoryInterpretationNode, StoryEvent, StoryInterpretationNodeDifference>
{
    ...
}
```
