## Incremental Interpretation and Comprehension

The following interfaces show that situation models or world models can be represented using semantic datasets and that the incremental interpretation and comprehension of sequences, e.g., sequences of story events, can be implemented by producing sequences of weighted candidate updates to nodes' models.

```cs
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;
using SparqlPrediction = (VDS.RDF.Query.SparqlQuery Query, VDS.RDF.Query.SparqlResultSet Result);

namespace Narratology
{
public interface IInterpretation
{
    public SparqlUpdateCommandSet Updates { get; }
    public IEnumerable<Exception> Errors { get; }
}

public interface ICuriousInterpretation : IInterpretation
{
    public IEnumerable<SparqlQuery> ResolvedQuestions { get; }
    public IEnumerable<(SparqlQuery Old, SparqlQuery New)> UpdatedQuestions { get; }
    public IEnumerable<(float Priority, SparqlQuery Query)> NewQuestions { get; }
}

public interface IPredictiveInterpretation : IInterpretation
{
    public IEnumerable<SparqlPrediction> CorrectlyResolvedPredictions { get; }
    public IEnumerable<SparqlPrediction> IncorrectlyResolvedPredictions { get; }
    public IEnumerable<(SparqlPrediction Old, SparqlPrediction New)> UpdatedPredictions { get; }
    public IEnumerable<(float Salience, SparqlPrediction Prediction)> NewPredictions { get; }
}

public interface IInterpreter<out TSelf, in TInput, TInterpretation>
    where TSelf : IInterpreter<TSelf, TInput, TInterpretation>
    where TInterpretation : IInterpretation
{
    public float Confidence { get; }
    public IInMemoryQueryableStore Model { get; }

    public IEnumerable<(float Confidence, TInterpretation Interpretation)> Interpret(TInput input);

    public TSelf? Parent { get; }
    public IReadOnlyCollection<TSelf> Children { get; }
    public TSelf CreateChild(float confidence, TInterpretation interpretation);

    public TSelf Commit(float confidence = 1.0f);
    public void Rollback(IEnumerable<Exception> reason);
}

public interface ICuriousInterpreter<out TSelf, in TInput, TInterpretation> :
    IInterpreter<TSelf, TInput, TInterpretation>
    where TSelf : ICuriousInterpreter<TSelf, TInput, TInterpretation>
    where TInterpretation : ICuriousInterpretation
{
    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }
}

public interface IPredictiveInterpreter<out TSelf, in TInput, TInterpretation> :
    IInterpreter<TSelf, TInput, TInterpretation>
    where TSelf : IPredictiveInterpreter<TSelf, TInput, TInterpretation>
    where TInterpretation : IPredictiveInterpretation
{
    public IEnumerable<(float Salience, SparqlPrediction Prediction)> Predictions { get; }
}
```

One could then implement:

```cs
public class StoryEvent
{
    ...
}

public class StoryEventInterpretation :
    ICuriousInterpretation,
    IPredictiveInterpretation
{
    ...
}

public class StoryReader :
    ICuriousInterpreter<StoryReader, StoryEvent, StoryEventInterpretation>,
    IPredictiveInterpreter<StoryReader, StoryEvent, StoryEventInterpretation>
{
    ...
}
```

One could also implement extension methods resembling:

```cs
public static class Extensions
{
    extension<TSelf, TInput, TInterpretation>(IInterpreter<TSelf, TInput, TInterpretation> node)
        where TSelf : IInterpreter<TSelf, TInput, TInterpretation>
        where TInterpretation : IInterpretation
    {
        public IEnumerable<TSelf> Foo(TInput input, Func<TSelf, IEnumerable<Exception>> validator)
        {
            foreach (var (confidence, interpretation) in node.Interpret(input))
            {
                if (!interpretation.Errors.Any())
                {
                    var child = node.CreateChild(node.Confidence * confidence, interpretation);
                    var errors = validator(child);

                    if (!errors.Any())
                    {
                        yield return child;
                    }
                    else
                    {
                        child.Rollback(errors);
                    }
                }
            }
        }

        public IEnumerable<(float Score, TSelf Node)> Foo(TInput input, Func<TSelf, float> scorer)
        {
            foreach (var (confidence, interpretation) in node.Interpret(input))
            {
                if (!interpretation.Errors.Any())
                {
                    var child = node.CreateChild(node.Confidence * confidence, interpretation);
                    var score = scorer(child);

                    if (score > 0.0f && score <= 1.0f)
                    {
                        yield return (score, child);
                    }
                    else
                    {
                        child.Rollback([]);
                    }
                }
            }
        }
    }
}
```

## Computational Poetics

When does it make sense for two or more interpretations to be simultaneously valid? When can interpretations be combined? When are interpretations mutually exclusive?

How can a system's focus and attention be distributed across tree nodes representing incremental interpretations and comprehensions?

## Computational Narratology

Coming soon.

### Erotetics

Coming soon.

### Prediction and Infilling

Coming soon.

## Cognition, Working Memory, and Telemetry

Should artificial-intelligence systems be able to answer introspective questions about their processes and procedures of incremental interpretation and comprehension?

How might logs or telemetry obtained during incremental interpretation and comprehension, analogously resembling a working memory buffer, be consulted during introspective question-answering?

## Agentic Artificial Intelligence

Coming soon.
