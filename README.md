## Computational Narratology

Considered, here, are some implementational approaches to computational narratology.

### Situation Modeling and Event Interpretation

The following simple sketches explore the concepts that a situation model could be represented using a semantic dataset and that a computational interpretation of an event could involve producing weighted candidate update sets for such a situation model.

```cs
using VDS.RDF;
using VDS.RDF.Update;

public partial interface ISituationModeler
{
    public IInMemoryQueryableStore SituationModel { get; }
}

public partial interface IEventInterpreter
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IInMemoryQueryableStore currentModel, IEvent e);
}

public partial interface IAudience : ISituationModeler, IEventInterpreter
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IEvent e)
    {
        return this.Interpret(this.SituationModel, e);
    }
}
```

### Question-asking

How might an `IEventInterpreter` or `Audience` produce questions about an event to enhance interpretation and any corresponding updating of situation models?

A callback function could be provided:

```cs
public partial interface IEventInterpreter
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IInMemoryQueryableStore currentModel, IEvent e, QuestionCallback callback);
}

public partial interface IAudience : ISituationModeler, IEventInterpreter
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IEvent e, QuestionCallback callback);
}
```

Alternatively, an event could be created on `IAudience`:

```cs
public partial interface IAudience : ISituationModeler, IEventInterpreter
{
    public event QuestionEventHandler OnQuestionAsked;
}
```

Alternatively, questions could be provided on an output data structure:

```cs
public partial interface IEventInterpreter
{
    public IEventInterpretationResult Interpret(IInMemoryQueryableStore currentModel, IEvent e);
}

public partial interface IAudience : ISituationModeler, IEventInterpreter
{
    public IEventInterpretationResult Interpret(IEvent e);
}
```

Alternatively, `IAudience` could receive an `INarrator` instance in its `Interpret()` method to ask questions of:

```cs
public partial interface IEventInterpreter
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IInMemoryQueryableStore currentModel, IEvent e, INarrator narrator);
}

public partial interface IAudience : ISituationModeler, IEventInterpreter
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IEvent e, INarrator narrator);
}
```

Alternatively, agentic approaches could be considered; `IAudience` and `INarrator` could be agents capable of engaging in dialogue.

### Multi-agent Systems

Coming soon.
