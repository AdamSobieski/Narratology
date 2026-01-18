# Concepts

## Introduction

A vision for _conceptual computing_ is that, beyond people and artificial-intelligence agents using natural language to engage in dialogues with one another, people and artificial-intelligence systems could create, work with, analyze, visualize, save, load, copy, perform complex operations upon, and share resultant [concepts](https://en.wikipedia.org/wiki/Concept) with one another.

## Interfaces

With respect to modeling concepts, starting simply, a concept can be an interface providing `Services`, an `IServiceProvider`. In this way, concepts can be worked with independently of implementational details such as any [feature vectors](https://en.wikipedia.org/wiki/Feature_(machine_learning)#Feature_vectors) or [embedding vectors](https://en.wikipedia.org/wiki/Embedding_(machine_learning)).

```cs
public interface IConcept
{
    IServiceProvider Services { get; }
}
```

## Services

Initial services envisioned for concepts include those pertaining to definitions, provenance, related concepts, and whether the concepts contain provided individual object instances.

### Definition

Concepts' definitions are model-specific (e.g., Claude, Gemma), multimodal, natural-language documents. They may include [intensional](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions) components, [ostensive](https://en.wikipedia.org/wiki/Ostensive_definition) components (positive and negative examples), and more.

```cs
public interface IConceptDefinitionService
{
    object? GetDefinition(IConcept concept, string model, ContentType contentType, CultureInfo language, Type? type = null);
}
```

As envisioned, obtaining a concept's definition resembles [content negotiation](https://en.wikipedia.org/wiki/Content_negotiation) with model-specificity added in.

### Provenance

The provenance of a concept is envisioned as including operations involving other concepts.

```cs
public interface IConceptProvenanceService
{
    object? GetProvenance(IConcept concept, Type? type = null);
}
```

### Related Concepts

```cs
public interface IConceptRelatedConceptsService
{
    IEnumerable<ConfidenceValueTriple<double>> GetRelatedConcepts(IConcept concept, object relationship);
}
```

### Contains

```cs
public interface IConceptContainsService
{
    ConfidenceValue<double> Contains(IConcept concept, object? instance);
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
            var service = (IConceptDefinitionService?)concept.Services.GetService(typeof(IConceptDefinitionService));
            if (service == null) return null;

            return service.GetDefinition(concept, model, contentType, language, type);
        }
        public object? GetProvenance(Type? type = null)
        {
            var service = (IConceptProvenanceService?)concept.Services.GetService(typeof(IConceptProvenanceService));
            if (service == null) return null;

            return service.GetProvenance(concept, type);
        }
        public IEnumerable<ConfidenceValueTriple<double>>? GetRelatedConcepts(object relationship)
        {
            var service = (IConceptRelatedConceptsService?)concept.Services.GetService(typeof(IConceptRelatedConceptsService));
            if (service == null) return null;

            return service.GetRelatedConcepts(concept, relationship);
        }
        public ConfidenceValue<double>? Contains(object? instance)
        {
            var service = (IConceptContainsService?)concept.Services.GetService(typeof(IConceptContainsService));
            if (service == null) return null;

            return service.Contains(concept, instance);
        }
    }
}
```

## Obtaining Concepts from Artificial-intelligence Systems

Techniques are being designed and explored for retrieving concepts from AI systems. For example:

```cs
IConcept concept = ai.GetConcept(
    positiveExamples: [img1, img2, img3],
    negativeExamples: [img4],
    definitionType: "text/markdown",
    definition: "This is a definition."
);
```

## Building Concepts

Techniques are being designed and explored for both building concept-retrieval queries for AI systems and building concepts using method chaining and fluent interfaces.

_More coming soon!_
