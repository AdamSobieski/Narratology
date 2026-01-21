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

### Comparisons and Differences

Services can provide [comparisons](https://en.wikipedia.org/wiki/Comparison) and differences between concepts, be they proximate concepts to one another or a greater distance apart.

```cs
public interface IConceptComparisonService
{
    object? Compare(IConcept concept, IConcept other, ContentType contentType, CultureInfo language, Type? type = null);
}
```

Would these services, providing comparisons between pairs of concepts, be acontextual or contextual?

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
        public object? Compare(IConcept other, ContentType contentType, CultureInfo language, Type? type = null)
        {
            var service = (IConceptDifferenceService?)concept.Services.GetService(typeof(IConceptDifferenceService));
            if (service == null) return null;

            return service.Compare(concept, other, contentType, language, type);
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

## Retrieving Concepts

Techniques are being designed and explored for client systems to retrieve concepts from artificial-intelligence systems. For examples:

```cs
IEnumerable<IConcept> candidates = system.GetConcept(
    context: context,
    positiveExamples: [img1, img2, img3],
    negativeExamples: [img4],
    definitionType: "text/markdown",
    definition: "This is a natural-language concept definition.",
    constraints: []
);
```

```cs
IEnumerable<IConcept> candidates = system.GetConcept(
    context: context,
    vectors: [vector]
);
```

Note that positive and negative examples need not only be images. They could be builtin datatypes, objects (e.g., JSON), file-based resources, text or multimedia documents, 3D models, knowledge graphs, concepts, sets of concepts, and more &ndash; any things from which concepts could be inferred from examples.

Concept-retrieval processes could be [session-based](https://en.wikipedia.org/wiki/Session_(computer_science)), incremental and conversational, adhering to a protocol. In addition to accepting and potentially generating [chat histories](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/chat-history?pivots=programming-language-csharp) during these processes, e.g., per candidate concept, concept-retrieval requests could provide additional context cues (e.g., state vectors) to artificial-intelligence systems.

Concept storage and retrieval services could be centralized, Web-based platforms or, instead, decentralized, distributed, peer-to-peer systems [(Zaarour and Curry, 2022)](https://www.sciencedirect.com/science/article/pii/S0167739X22000590). Both approaches can enable multiple means of indexing, addressing, and searching for concept definitions.

Suitable extensible formats for storing and transmitting concept definitions between systems include [MIME](https://en.wikipedia.org/wiki/MIME), [XML](https://en.wikipedia.org/wiki/XML), [XHTML](https://en.wikipedia.org/wiki/XHTML), [JSON](https://en.wikipedia.org/wiki/JSON), and combinations of these, e.g., [MHTML](https://en.wikipedia.org/wiki/MHTML) (see also: [RFC 2392](https://www.rfc-editor.org/rfc/rfc2392)). Structured concept-definition documents might include sections of hypertext, multimedia, mathematics, structured knowledge, and, perhaps, JavaScript, while referring to stylesheets for presentation.

Concept definitions will be able to express combinations of aspects of [definitions](https://en.wikipedia.org/wiki/Definition), e.g., textual, [ostensive](https://en.wikipedia.org/wiki/Ostensive_definition), [intensional](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions), and [extensional](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions).

<details>
<summary>Click here to toggle view of a rough-draft extensible concept-definition using XML.</summary>
<br>

```xml
<concept version="1.0" xmlns="..." xmlns:html="http://www.w3.org/1999/xhtml">
  <part rel="text-definition" type="application/xhtml+xml" lang="en">
    <html:p>This is natural-language hypertext definitional content.</html:p>
    <html:p>It can be comprised of multiple paragraphs of content.</html:p>
  </part>
  <part rel="structured-definition" type="text/turtle">
    ...
  </part>
  <part rel="examples" type="multipart/related">
    <part rel="positive-examples" type="multipart/related">
      <part rel="example" type="image/png" src="1.png" />
      <part rel="example" type="image/png" src="2.png" />
      <part rel="example" type="image/png" src="3.png" />
    </part>
    <part rel="negative-examples" type="multipart/related">
      <part rel="example" type="image/png" src="4.png" />
    </part>
  </part>
  <part rel="structured-relationships" type="text/turtle">
    ...
  </part>
  <part rel="structured-mappings" type="text/turtle">
    ...
  </part>
  <part rel="vector-collection" type="multipart/related">
    <part rel="vector" type="application/octet-stream" system="(model: openai.gpt-oss-safeguard-20b)" src="1.vec" />
    <part rel="vector" type="application/octet-stream" system="(model: anthropic.claude-sonnet-4-20250514)" src="2.vec" />
    <part rel="vector" type="application/octet-stream" system="(model: google.gemma-3-27b-it)" src="3.vec" />
  </part>
  <part rel="provenance" type="text/turtle">
    ...
  </part>
</concept>
```

</details>

## Creating and Building Concepts

Techniques are being designed and explored for both building concept-retrieval queries for AI systems and building concepts using method chaining and fluent interfaces.
