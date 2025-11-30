## Computational Narratology

Considered, here, are some implementational approaches to computational narratology.

### Situation Modeling and Event Interpretation

The following simple sketches explore the concepts that a situation model could be represented using a semantic graph and that a computational interpretation of an event involves producing weighted candidate updates to such a semantic situation model.

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

How might an `IEventInterpreter` or `Audience` produce questions about a story or event with which to enhance interpretations and any corresponding updating of situation models?
