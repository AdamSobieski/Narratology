## Incremental Story Comprehension

The following model explores that situation models could be represented using semantic datasets and that computational interpretations of sequences of events could involve producing weighted candidate sets of updates for such situation models.

```cs
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Update;

public interface ISituationModeler
{
    public IInMemoryQueryableStore SituationModel { get; }
}

public interface IInterpretation
{
    public IEnumerable<Exception> Errors { get; }

    public IEnumerable<SparqlQuery> AnsweredQuestions { get; }
    public IEnumerable<(float Priority, SparqlQuery Query)> NewQuestions { get; }

    public float Confidence { get; }
    public SparqlUpdateCommandSet Commands { get; }
}

public interface IInterpreter<in T> : ISituationModeler
{
    public IEnumerable<IInterpretation> Interpret(T input);
}

public interface IInterpreterTreeNode<out THIS, in T> : IInterpreter<T>
    where THIS : IInterpreterTreeNode<THIS, T>
{
    public float Confidence { get; }

    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }

    public THIS? Parent { get; }
    public IReadOnlyCollection<THIS> Children { get; }
    public THIS CreateChild(IInterpretation interpretation);

    public THIS Commit(float confidence = 1.0f);
    public void Rollback(IEnumerable<Exception> reason);
}

public interface IValidator<in T>
{
    public IEnumerable<Exception> Validate(T value);
}
```

Using the above model, one could implement:

```cs
public class Reader : IInterpreterTreeNode<Reader, IEvent> { ... }
```

Using the above model, one could implement extension methods resembling:

```cs
public static class Extensions
{
    extension<THIS, T>(IInterpreterTreeNode<THIS, T> node)
        where THIS : IInterpreterTreeNode<THIS, T>
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

        ...
    }
}
```

## Agentic Computational Narratology

Story readers and narrators could be artificial-intelligence agents capable of engaging with one another in natural-language dialogues.
