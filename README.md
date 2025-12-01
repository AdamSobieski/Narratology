## Computational Narratology

### Situation Models and Event Interpretation

The following simple sketches explore the concepts that a situation model could be represented using a semantic dataset and that a computational interpretation of an event could involve producing weighted candidate update sets for such a situation model.

```cs
using VDS.RDF;
using VDS.RDF.Update;

public interface ISituationModeler
{
    public IInMemoryQueryableStore SituationModel { get; }
}

public interface IInterpreter<in T>
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IInMemoryQueryableStore model, T input);
}

public interface IReader : ISituationModeler, IInterpreter<IEvent>
{
    public virtual IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IEvent input)
    {
        return this.Interpret(this.SituationModel, input);
    }
}
```

Perhaps, instead of one event being processed at a time, a set of events could be processed at a time.

```cs
public partial interface IReader : ISituationModeler, IInterpreter<IEnumerable<IEvent>>
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IEnumerable<IEvent> input)
    {
        return this.Interpret(this.SituationModel, input);
    }
}
```

### Character Modeling and Social Cognition

Coming soon.

### Semantic and Episodic Working Memory

Coming soon.

### Question-asking

To enhance its interpretive processes, how might an `IReader` instance generate and efficiently, parsimoniously, ask questions about input events?

Perhaps questions pertinent to processing input events could be provided on an output data structure, `IInterpretation`, with these questions intended for a narrator or event provider. Questions could be structured queries intended to be processed against the narrator's situation model.

```cs
public partial interface IInterpretation
{
    public IEnumerable<(float Priority, SparqlQuery Query)> Questions { get; }
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Result { get; }
}

public partial interface IInterpreter<in T>
{
    public IInterpretation Interpret(IInMemoryQueryableStore model, T input);
}

public partial interface IReader : ISituationModeler, IInterpreter<IEvent>
{
    public IInterpretation Interpret(IEvent input)
    {
        return this.Interpret(this.SituationModel, input);
    }
}
```

Agentic approaches should also be considered. Story readers and narrators could be agents capable of engaging with one another in natural-language dialogues.

### Agentic Systems

Coming soon.
