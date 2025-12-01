## Incremental Story Comprehension

The following model explores that situation models could be represented using semantic datasets and that computational interpretations of (story) events could involve producing weighted candidate sets of updates for such situation models.

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
    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Commands)> Updates { get; }
}

public interface IInterpreter<in T> : ISituationModeler
{
    public IInterpretation Interpret(T input);
}

public partial interface IReader : IInterpreter<IEvent> { }
```

Note that `IInterpretation` supports providing both questions (e.g., intended for a narrator) and updates (e.g., intended for a reader's situation model) in response to interpreting an input.

## Agentic Computational Narratology

Story readers and narrators could be artificial-intelligence agents capable of engaging with one another in natural-language dialogues.

More coming soon.
