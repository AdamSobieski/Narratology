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

## Representing Concept Definitions

Suitable extensible formats for storing and transmitting concept definitions between systems include [MIME](https://en.wikipedia.org/wiki/MIME), [XML](https://en.wikipedia.org/wiki/XML), [XHTML](https://en.wikipedia.org/wiki/XHTML), [JSON](https://en.wikipedia.org/wiki/JSON), and combinations of these, e.g., [MHTML](https://en.wikipedia.org/wiki/MHTML) (see also: [RFC 2392](https://www.rfc-editor.org/rfc/rfc2392)). Structured concept-definition documents might include sections of hypertext, multimedia, mathematics, structured knowledge, and, perhaps, JavaScript, while referring to stylesheets for presentation.

Approaches for expressing concept definitions should be able to express multiple approaches to [definition](https://en.wikipedia.org/wiki/Definition) simultaneously and in combination. For example: textual, [ostensive](https://en.wikipedia.org/wiki/Ostensive_definition), [intensional](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions), [extensional](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions), the [Ramsey-Lewis method](https://en.wikipedia.org/wiki/Ramsey%E2%80%93Lewis_method), and other approaches for defining algorithmically-obtained concepts, e.g., [cluster analysis](https://en.wikipedia.org/wiki/Cluster_analysis) and [conceptual clustering](https://en.wikipedia.org/wiki/Conceptual_clustering).

```xml
<definition xmlns="..." version="1.0" definiendum="main">
  <resource id="main">
    <part rel="text" type="application/xhtml+xml" lang="en">
      <![CDATA[
        <html xmlns="http://www.w3.org/1999/xhtml" xmlns:x="...">
          <p>This is natural-language concept-definitional content.</p>
          <p>It can contain multiple paragraphs of hypertext.</p>
          <p>It could refer to the definiendum by <x:term x:href="res:main">keyword</x:term>.</p>
          <p>It could refer to <x:term x:href="res:ref-1">other</x:term> concepts.</p>
        </html>
      ]]>
    </part>
    <part rel="ramsey-lewis" type="multipart/related">
      <part rel="theory" type="text/uri-list">
        <![CDATA[...]]>
      </part>
      <part rel="statements" type="application/xhtml+xml" lang="en">
        <![CDATA[
          <html xmlns="http://www.w3.org/1999/xhtml" xmlns:x="...">
            <ul>
              <li>
                <p>This is one sentence of a theory with a <x:term x:href="res:main">keyword</x:term> in context.</p>
              </li>
              <li>
                <p>Sentences with <x:term x:href="res:main">keyword</x:term>, here, are assertions of a theory.</p>
              </li>
              <li>
                <p>Can use <x:term x:href="res:ref-1">other</x:term> <x:term x:href="res:main">keywords</x:term>.</p>
              </li>
              <li>
                <p>And <x:term x:href="res:ref-2">yet other</x:term> <x:term x:href="res:main">keywords</x:term>.</p>
                <p>Concepts referred to, here, should also appear in the structured-knowledge sections, below.</p>
              </li>
            </ul>
          </html>
        ]]>
      </part>
    </part>
    <part rel="structured-definition" type="text/turtle" profile="...">
      <![CDATA[...]]>
    </part>
    <part rel="relationships" type="text/turtle">
      <![CDATA[
        @prefix ex: <http://www.example.org/> .
      
        <res:main> ex:relation1 <res:ref-1> ;
                   ex:relation2 <res:ref-2> .
      ]]>
    </part>
    <part rel="mappings" type="text/turtle">
      <![CDATA[...]]>
    </part>
    <part rel="provenance" type="text/turtle">
      <![CDATA[...]]>
    </part>
    <part rel="examples" type="multipart/related">
      <part rel="positive" type="multipart/related">
        <part rel="example" type="multipart/alternative">
          <part type="image/png" hash="urn:hash::md5:..." src="https://www.example.org/media/picture-1.png" />
          <part type="image/png" hash="urn:hash::md5:..." src="https://www.mirror.org/media/picture-1.png" />
        </part>
        <part rel="example" type="image/png" src="picture-2.png" />
        <part rel="example" type="image/png" src="picture-3.png" />
      </part>
      <part rel="negative" type="multipart/related">
        <part rel="example" type="image/png" src="picture-4.png" />
      </part>
    </part>
    <part rel="occurrences" type="multipart/related">
      <part rel="occurrence" type="application/ld+json" profile="http://www.w3.org/ns/anno.jsonld">
        <![CDATA[
          {
            "@context": "http://www.w3.org/ns/anno.jsonld",
            "type": "Annotation",
            "target": {
              "source": "res:ref3#peacocke1992study",
              "selector": {
                "type": "FragmentSelector",
                "value": "page=10",
                "refinedBy": {
                  "type": "TextQuoteSelector",
                  "exact": "keyword",
                  "prefix": "text before the ",
                  "suffix": " and text after it"
                }
              }
            }
          }
        ]]>
      </part>
      <part rel="occurrence" type="application/ld+json" profile="http://www.w3.org/ns/anno.jsonld">
        <![CDATA[
          {
            "@context": "http://www.w3.org/ns/anno.jsonld",
            "type": "Annotation",
            "target": {
              "source": "res:ref3#murphy2004big",
              "selector": {
                "type": "FragmentSelector",
                "value": "page=21",
                "refinedBy": {
                  "type": "TextQuoteSelector",
                  "exact": "keyword",
                  "prefix": "some text before the ",
                  "suffix": " and more text after it"
                }
              }
            }
          }
        ]]>
      </part>
    </part>
    <part rel="vector" type="multipart/alternative">
      <part type="application/octet-stream" system="(model: openai.gpt-oss-safeguard-20b)" src="1.vec" />
      <part type="application/octet-stream" system="(model: anthropic.claude-sonnet-4-20250514)" src="2.vec" />
      <part type="application/octet-stream" system="(model: google.gemma-3-27b-it)" src="3.vec" />
    </part>
  </resource>
  <resource id="ref-1">
    <part rel="location" type="text/uri-list">
      <![CDATA[
        https://www.example.org/concepts/resource.xml
        https://www.mirror.org/concepts/resource.xml        
      ]]>
    </part>
  </resource>
  <resource id="ref-2">
    <part rel="vector" type="multipart/alternative">
      <part type="application/octet-stream" system="(model: openai.gpt-oss-safeguard-20b)" src="4.vec" />
      <part type="application/octet-stream" system="(model: anthropic.claude-sonnet-4-20250514)" src="5.vec" />
      <part type="application/octet-stream" system="(model: google.gemma-3-27b-it)" src="6.vec" />
    </part>
  </resource>
  <resource id="ref-3">
    <part rel="references" type="text/x-bibtex">
      <![CDATA[
        @book{peacocke1992study,
        title={A study of concepts},
        author={Peacocke, Christopher},
        year={1992},
        publisher={MIT Press}}
        
        @book{murphy2004big,
        title={The big book of concepts},
        author={Murphy, Gregory},
        year={2004},
        publisher={MIT press}}
      ]]>
    </part>
  </resource>
</definition>
```

## Creating and Building Concepts Programmatically

Techniques are being designed and explored for both building concept-retrieval queries for AI systems and building concepts using method chaining and fluent interfaces.
