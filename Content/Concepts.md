# Concepts

## Introduction

A vision for _conceptual computing_ is that, beyond people and artificial-intelligence agents using natural language to engage in dialogues with one another, people and artificial-intelligence systems could create, work with, analyze, visualize, save, load, copy, perform complex operations upon, and share resultant [concepts](https://en.wikipedia.org/wiki/Concept) with one another.

## Interfaces

With respect to modeling concepts, starting simply, a concept can be an interface providing an `IServiceProvider`:

```cs
public interface IConcept
{
    IServiceProvider ServiceProvider { get; }
}
```

## Services

Initial services envisioned for concepts include those relating to definitions, provenance, and whether concepts contain instances.

### Definition

A definition is a potentially model-specific (e.g., Claude or Gemma), multimodal, natural-language document defining the concept. It may include an [intensional](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions) definition, an [ostensive](https://en.wikipedia.org/wiki/Ostensive_definition) definition (positive and negative examples), and more.

```cs
public interface IConceptDefinitionProvider
{
    object? GetDefinition(IConcept concept, string model, ContentType contentType, CultureInfo language, Type? type = null);
}
```

### Provenance

The provenance of a concept is envisioned as detailing its origins, from operations involving other concepts.

```cs
public interface IConceptProvenanceProvider
{
    object? GetProvenance(IConcept concept, Type? type = null);
}
```

### Contains

Is an object an element of the concept? Does the concept contain the object as an instance?

```cs
public interface IConceptContainsProvider
{
    ConfidenceValue<double>? Contains(IConcept concept, object? instance);
}
```

## Extension Methods

For the initial set of services, indicated above, extension method can be provided for `IConcept`:

```cs
public static partial class Extensions
{
    extension(IConcept concept)
    {
        public object? GetDefinition(string model, ContentType contentType, CultureInfo language, Type? type = null)
        {
            var service = (IConceptDefinitionProvider?)concept.ServiceProvider.GetService(typeof(IConceptDefinitionProvider));
            if (service == null) return null;

            return service.GetDefinition(concept, model, contentType, language, type);
        }
        public object? GetProvenance(Type? type = null)
        {
            var service = (IConceptProvenanceProvider?)concept.ServiceProvider.GetService(typeof(IConceptProvenanceProvider));
            if (service == null) return null;

            return service.GetProvenance(concept, type);
        }
        public ConfidenceValue<double>? Contains(object? instance)
        {
            var service = (IConceptContainsProvider?)concept.ServiceProvider.GetService(typeof(IConceptContainsProvider));
            if (service == null) return null;

            return service.Contains(concept, instance);
        }
    }
}
```

_More coming soon!_
