## Incremental Interpretation and Comprehension

The following model explores that situation models or world models could be represented using semantic datasets and that the computational interpretation of sequences, e.g., sequences of story events, could involve producing weighted candidate sets of updates to nodes' models.

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
    public float Confidence { get; }
    public SparqlUpdateCommandSet Updates { get; }

    public IEnumerable<SparqlQuery> ResolvedQuestions { get; }
    public IEnumerable<(float Priority, SparqlQuery Query)> NewQuestions { get; }

    public IEnumerable<Exception> Errors { get; }
}

public interface IInterpreter<out THIS, in T> : IModeler
    where THIS : IInterpreter<THIS, T>
{
    public IEnumerable<IInterpretation> Interpret(T input);

    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }

    public THIS? Parent { get; }
    public IReadOnlyCollection<THIS> Children { get; }
    public THIS CreateChild(IInterpretation interpretation);

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
            foreach (var interpretation in node.Interpret(input))
            {
                if (!interpretation.Errors.Any())
                {
                    var child = node.CreateChild(interpretation);
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
