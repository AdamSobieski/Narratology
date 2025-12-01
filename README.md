## Incremental Story Comprehension

The following interfaces explore that situation models could be represented using semantic datasets and that computational interpretations of (story) events could involve producing weighted candidate sets of updates for such situation models.

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
    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Commands)> Updates { get; }
}

public interface IInterpreter<in T>
{
    public IInterpretation Interpret(IInMemoryQueryableStore model, T input);
}

public partial interface IReader : ISituationModeler, IInterpreter<IEvent> { }

public static class Extensions
{
    extension(IReader reader)
    {
        public IInterpretation Interpret(IEvent input)
        {
            return reader.Interpret(reader.SituationModel, input);
        }
    }
}
```

Note that `IInterpretation` supports providing both questions (intended for a narrator) and updates (intended for a reader's situation model).

Perhaps, instead of events being incrementally processed one at a time, events could be processed a set at a time.

```cs
public partial interface IReader : ISituationModeler, IInterpreter<IEnumerable<IEvent>> { }
```

## Agentic Computational Narratology

Story readers and narrators could be artificial-intelligence agents capable of engaging with one another in natural-language dialogues.

More coming soon.
