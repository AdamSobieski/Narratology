## Computational Narratology

### Situation Modeling and Event Interpretation

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

### Concurrency

Perhaps, instead of one `IEvent` necessarily being processed at a time, a set of concurrent events, `IEnumerable<IEvent>`, could be processed.

```cs
public partial interface IReader : ISituationModeler, IInterpreter<IEnumerable<IEvent>>
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IEnumerable<IEvent> input)
    {
        return this.Interpret(this.SituationModel, input);
    }
}
```

### Scopes and Contexts

Coming soon.

### Semantic and Episodic Working Memory

Coming soon.

### Character Modeling and Social Cognition

Coming soon.

### Question-asking

How might an `IReader` instance generate and ask questions about an input event to enhance its interpretive processes?

Perhaps questions resulting from processing an arriving input event could be provided on an output data structure, `IInterpretationResult<T>`:

```cs
public partial interface IInterpreter<T>
{
    public IInterpretationResult<T> Interpret(IInMemoryQueryableStore model, T input);
}

public partial interface IReader : ISituationModeler, IInterpreter<IEvent>
{
    public IInterpretationResult<IEvent> Interpret(IEvent input)
    {
        return this.Interpret(this.SituationModel, input);
    }
}
```

Agentic approaches should also be considered. `IReader` and `INarrator`, providing sequences of story events, could be agents capable of engaging with one another in dialogues.

### Multi-agent Systems

Coming soon.
