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

Initial services envisioned for concepts include those pertaining to definitions, differences, provenance, related concepts, and whether concepts contain provided individual object instances.

### Definitions

Concepts' definitions are model-specific (e.g., Claude, Gemma), multimodal, natural-language documents. They may include [intensional](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions) components, [ostensive](https://en.wikipedia.org/wiki/Ostensive_definition) components (positive and negative examples), and more.

```cs
public interface IConceptDefinitionService
{
    object? GetDefinition(IConcept concept, string model, ContentType contentType, CultureInfo language, Type? type = null);
}
```

As envisioned, obtaining a concept's definition resembles [content negotiation](https://en.wikipedia.org/wiki/Content_negotiation) with model-specificity added in.

### Differences

Services can provide differences between concepts, be they proximate concepts to one another or some distance apart.

```cs
public interface IConceptDifferenceService
{
    object? GetDifference(IConcept concept, IConcept other, ContentType contentType, CultureInfo language, Type? type = null);
}
```

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
        public object? GetDifference(IConcept other, ContentType contentType, CultureInfo language, Type? type = null)
        {
            var service = (IConceptDifferenceService?)concept.Services.GetService(typeof(IConceptDifferenceService));
            if (service == null) return null;

            return service.GetDifference(concept, other, contentType, language, type);
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

## Retrieving Concepts from Artificial-intelligence Systems

Techniques are being designed and explored for client systems to retrieve concepts from artificial-intelligence systems. For example:

```cs
IEnumerable<IConcept> candidates = ai.GetConcept(
    context: context,
    positiveExamples: [img1, img2, img3],
    negativeExamples: [img4],
    definitionType: "text/markdown",
    definition: "This is a natural-language concept definition.",
    constraints: []
);
```

Concept retrieval processes could be [session-based](https://en.wikipedia.org/wiki/Session_(computer_science)), incremental and conversational, adhering to a protocol. In addition to accepting and potentially generating [chat histories](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/chat-history?pivots=programming-language-csharp) during these processes, e.g., per candidate concept, concept-retrieval requests could provide additional context cues (e.g., state vectors) to artificial-intelligence systems.

Considered formats suitable for storing and transmitting concept-retrieval requests and resultant concept definitions between systems include [MIME](https://en.wikipedia.org/wiki/MIME), [XHTML](https://en.wikipedia.org/wiki/XHTML), and combinations of these, e.g., [MHTML](https://en.wikipedia.org/wiki/MHTML) (see also: [RFC 2392](https://www.rfc-editor.org/rfc/rfc2392)). Structured concept-definition documents might include sections of hypertext, multimedia, mathematics, structured knowledge, and, perhaps, JavaScript, while referring to stylesheets for presentation.

## Building Concepts

Techniques are being designed and explored for both building concept-retrieval queries for AI systems and building concepts using method chaining and fluent interfaces.

_More coming soon!_
