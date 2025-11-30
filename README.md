## Computational Narratology

Considered, here, are some implementational approaches to computational narratology and biography.

### Situation Modeling and Event Interpretation

The following sketches explore the concepts that a situation model could be represented using a semantic graph and that a computational interpretation of an event involves producing weighted candidate updates to such a semantic situation model.

```cs
using VDS.RDF;
using VDS.RDF.Update;

public interface ISituationModeler
{
    public IInMemoryQueryableStore SituationModel { get; }
}

public interface IEventInterpreter
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IInMemoryQueryableStore currentModel, IEvent e);
}

public interface IAudience : ISituationModeler, IEventInterpreter
{
    public IEnumerable<(float Confidence, SparqlUpdateCommandSet Updates)> Interpret(IEvent e)
    {
        return this.Interpret(this.SituationModel, e);
    }
}
```
