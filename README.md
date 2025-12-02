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

public interface IUpdate
{
    public SparqlUpdateCommandSet Updates { get; }
    public IEnumerable<Exception> Errors { get; }
}

public interface ICuriousUpdate : IUpdate
{
    public IEnumerable<SparqlQuery> ResolvedQuestions { get; }
    public IEnumerable<(float Priority, SparqlQuery Query)> NewQuestions { get; }
}

public interface IUpdateable<out THIS, in TInput, TUpdate> : IModeler
    where THIS : IUpdateable<THIS, TInput, TUpdate>
    where TUpdate : IUpdate
{
    public IEnumerable<(float Confidence, TUpdate Update)> Update(TInput input);

    public THIS? Parent { get; }
    public IReadOnlyCollection<THIS> Children { get; }
    public THIS CreateChild(float confidence, TUpdate update);

    public THIS Commit(float confidence = 1.0f);
    public void Rollback(IEnumerable<Exception> reason);
}

public interface ICuriousUpdateable<out THIS, in TInput, TUpdate> : IUpdateable<THIS, TInput, TUpdate>
    where THIS : IUpdateable<THIS, TInput, TUpdate>
    where TUpdate : ICuriousUpdate
{
    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }
}
```

One could then implement:

```cs
public class StoryReader : ICuriousUpdateable<StoryReader, StoryEvent, ICuriousUpdate> { ... }
```

One could also implement extension methods resembling:

```cs
public static class Extensions
{
    extension<THIS, TInput, TUpdate>(IUpdateable<THIS, TInput, TUpdate> node)
        where THIS : IUpdateable<THIS, TInput, TUpdate>
        where TUpdate : IUpdate
    {
        public IEnumerable<THIS> Process(TInput input, Func<THIS, IEnumerable<Exception>> validator)
        {
            foreach (var (confidence, update) in node.Update(input))
            {
                if (!update.Errors.Any())
                {
                    var child = node.CreateChild(node.Confidence * confidence, update);
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

        public IEnumerable<(float Score, THIS Node)> Process(TInput input, Func<THIS, float> scorer)
        {
            foreach (var (confidence, update) in node.Update(input))
            {
                if (!update.Errors.Any())
                {
                    var child = node.CreateChild(node.Confidence * confidence, update);
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

## Cognition, Working Memory, and Telemetry

Should artificial-intelligence systems be able to answer introspective questions about their processes and procedures of incremental interpretation and comprehension?

How might logs or telemetry obtained during incremental interpretation and comprehension, analogously resembling a working memory buffer, be consulted during introspective question-answering?

## Agentic Artificial Intelligence

Coming soon.
