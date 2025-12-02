## Incremental Interpretation and Comprehension

The following interfaces show that situation models or world models can be represented using semantic datasets and that the incremental interpretation and comprehension of sequences, e.g., sequences of story events, can be implemented by producing sequences of weighted candidate updates to nodes' models.

```cs
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;

public interface IModeler
{
    public float Confidence { get; }
    public IInMemoryQueryableStore Model { get; }
}

public interface IInterpretation
{
    public IReadOnlyCollection<IInterpretation> Composition { get; }

    public SparqlUpdateCommandSet Updates { get; }

    public IEnumerable<SparqlQuery> ResolvedQuestions { get; }
    public IEnumerable<(float Priority, SparqlQuery Query)> NewQuestions { get; }

    public IEnumerable<Exception> Errors { get; }
}

public interface IInterpreter<out THIS, in T> : IModeler
    where THIS : IInterpreter<THIS, T>
{
    public IEnumerable<(float Confidence, IInterpretation Interpretation)> Interpret(T input);

    public IInterpretation Combine(params IInterpretation[] interpretations);

    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }

    public THIS? Parent { get; }
    public IReadOnlyCollection<THIS> Children { get; }
    public THIS CreateChild(float confidence, IInterpretation interpretation);

    public THIS Commit(float confidence = 1.0f);
    public void Rollback(IEnumerable<Exception> reason);
}
```

One could then implement:

```cs
public class StoryReader : IInterpreter<StoryReader, StoryEvent> { ... }
```

One could also implement extension methods resembling:

```cs
public interface IValidator<in T>
{
    public IEnumerable<Exception> Validate(T value);
}

public interface IScorer<in T>
{
    public float Score(T value);
}

public static class Extensions
{
    extension<THIS, T>(IInterpreter<THIS, T> node)
        where THIS : IInterpreter<THIS, T>
    {
        public IEnumerable<THIS> Process(T input, IValidator<THIS> validator)
        {
            foreach (var (confidence, interpretation) in node.Interpret(input))
            {
                if (!interpretation.Errors.Any())
                {
                    var child = node.CreateChild(confidence, interpretation);
                    var errors = validator.Validate(child);

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

        public IEnumerable<(float Score, THIS Node)> Process(T input, IScorer<THIS> scorer)
        {
            foreach (var interpretation in node.Interpret(input))
            {
                if (!interpretation.Errors.Any())
                {
                    var child = node.CreateChild(interpretation);
                    var score = scorer.Score(child);

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

        ...
    }
}
```

## Computational Poetics

When does it make sense for two or more interpretations to be simultaneously valid? When can interpretations be combined? When are interpretations mutually exclusive?

## Computational Narratology

Coming soon.

## Working Memory and Telemetry

Should artificial-intelligence systems be able to answer questions about their processes and procedures of incremental interpretation and comprehension?

Should logging or telemetry be processed during incremental interpretation and comprehension, analogously resembling a working memory buffer pertaining to cognitive processes and procedures?

## Agentic Artificial Intelligence

Coming soon.
