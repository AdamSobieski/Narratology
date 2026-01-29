# Concepts

## Introduction

A vision for _conceptual computing_ is that, beyond people and artificial-intelligence agents using natural language to engage in dialogues with one another, people and artificial-intelligence systems could create, work with, analyze, visualize, save, load, copy, perform complex operations upon, and share resultant [concepts](https://en.wikipedia.org/wiki/Concept) with one another.

## Interfaces

Concepts can be modeled using an interface providing `Services`, via [`IServiceProvider`](https://learn.microsoft.com/en-us/dotnet/api/system.iserviceprovider?view=net-10.0).

```cs
public interface IConcept
{
    IServiceProvider Services { get; }
}
```

Concept-related functionalities can be provided via extension methods which attempt to retrieve and subsequently utilize services by interface.

## Searching for and Retrieving Concepts

Techniques are being designed and explored for client systems to search for and retrieve concepts from artificial-intelligence systems.

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

Note that positive and negative examples need not only be images. They could be builtin datatypes, objects (e.g., JSON), file-based resources, text or multimedia documents, selections thereof, 3D models, knowledge graphs, concepts, sets of concepts, and more.

Concept storage and retrieval services could be centralized, Web-based platforms or, instead, decentralized, distributed, peer-to-peer systems [(Zaarour and Curry, 2022)](https://www.sciencedirect.com/science/article/pii/S0167739X22000590). Both approaches can enable multiple means of indexing, addressing, and searching for concept definitions.

Concept-retrieval processes could be [session-based](https://en.wikipedia.org/wiki/Session_(computer_science)), incremental and conversational.

## Creating and Building Concepts Programmatically

Techniques are being developed to enable developers to programmatically create and build concepts and concept-related queries utilizing method chaining and fluent interfaces.

## Representing Concept Definitions

Suitable extensible formats for storing and transmitting concept definitions between systems include [MIME](https://en.wikipedia.org/wiki/MIME), [XML](https://en.wikipedia.org/wiki/XML), [XHTML](https://en.wikipedia.org/wiki/XHTML), [JSON](https://en.wikipedia.org/wiki/JSON), and combinations of these, e.g., [MHTML](https://en.wikipedia.org/wiki/MHTML) (see also: [RFC 2392](https://www.rfc-editor.org/rfc/rfc2392)). Structured concept-definition documents might include sections of hypertext, multimedia, mathematics, structured knowledge, and, perhaps, JavaScript, while referring to stylesheets for presentation.

Concept definitions should be able to include multiple approaches to [definition](https://en.wikipedia.org/wiki/Definition) simultaneously and in combination.

For example: textual, [ostensive](https://en.wikipedia.org/wiki/Ostensive_definition) (multimedia) examples, [ostensive](https://en.wikipedia.org/wiki/Ostensive_definition) text selections from cited works, [concordances](https://en.wikipedia.org/wiki/Concordance_(publishing)), [intensional definitions](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions), [extensional definitions](https://en.wikipedia.org/wiki/Extensional_and_intensional_definitions), the [Ramsey-Lewis method](https://en.wikipedia.org/wiki/Ramsey%E2%80%93Lewis_method), and other approaches for defining algorithmically-obtained concepts, e.g., [cluster analysis](https://en.wikipedia.org/wiki/Cluster_analysis) and [conceptual clustering](https://en.wikipedia.org/wiki/Conceptual_clustering).

Here is an example of an extensible format for representing concept definitions.

```xml
<definition xmlns="..." version="1.0" definiendum="main">
  <part id="main" rel="concept" type="multipart/related">
    <part rel="text" type="application/xhtml+xml" lang="en">
      <![CDATA[
        <html xmlns="http://www.w3.org/1999/xhtml" xmlns:x="...">
          <p>This is natural-language concept-definitional content.</p>
          <p>It can contain multiple paragraphs of hypertext.</p>
          <p>It could refer to the definiendum by <x:term x:href="ref:main">keyword</x:term>.</p>
          <p>It could refer to <x:term x:href="ref:ref-1">other</x:term> concepts.</p>
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
                <p>This is one sentence of a theory with a <x:term x:href="ref:main">keyword</x:term> in context.</p>
              </li>
              <li>
                <p>Sentences with <x:term x:href="ref:main">keyword</x:term>, here, are assertions of a theory.</p>
              </li>
              <li>
                <p>Can use <x:term x:href="ref:ref-1">other</x:term> <x:term x:href="ref:main">keywords</x:term>.</p>
              </li>
              <li>
                <p>And <x:term x:href="ref:ref-2">yet other</x:term> <x:term x:href="ref:main">keywords</x:term>.</p>
                <p>Concepts referred to, here, should also appear in the structured-knowledge sections, below.</p>
              </li>
            </ul>
          </html>
        ]]>
      </part>
    </part>
    <part rel="structured-definition" type="text/turtle">
      <![CDATA[...]]>
    </part>
    <part rel="relationships" type="text/turtle">
      <![CDATA[
        @prefix ex: <http://www.example.org/> .
      
        <ref:main> ex:relation1 <ref:ref-1> ;
                   ex:relation2 <ref:ref-2> .
      ]]>
    </part>
    <part rel="mappings" type="text/turtle">
      <![CDATA[
        @prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .
        
        <ref:main> rdfs:seeAlso <https://en.wiktionary.org/wiki/keyword> .
      ]]>
    </part>
    <part rel="provenance" type="text/turtle">
      <![CDATA[...]]>
    </part>
    <part rel="metadata" type="text/turtle">
      <![CDATA[...]]>
    </part>
    <part rel="examples" type="multipart/related">
      <part rel="positive" type="multipart/related">
        <part rel="example image" type="multipart/alternative">
          <part type="image/png" hash="urn:hash::md5:..." src="https://www.example.org/media/picture-1.png" />
          <part type="image/png" hash="urn:hash::md5:..." src="https://www.mirror.org/media/picture-1.png" />
        </part>
        <part rel="example image" type="image/png" src="picture-2.png" />
        <part rel="example image" type="image/png" src="picture-3.png" />
        <part rel="example selection" type="application/ld+json" profile="http://www.w3.org/ns/anno.jsonld">
          <![CDATA[
            {
              "@context": "http://www.w3.org/ns/anno.jsonld",
              "type": "Annotation",
              "target": {
                "source": "ref:biblio#peacocke1992study",
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
        <part rel="example selection" type="application/ld+json" profile="http://www.w3.org/ns/anno.jsonld">
          <![CDATA[
            {
              "@context": "http://www.w3.org/ns/anno.jsonld",
              "type": "Annotation",
              "target": {
                "source": "ref:biblio#murphy2004big",
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
      <part rel="negative" type="multipart/related">
        <part rel="example image" type="image/png" src="picture-4.png" />
      </part>
    </part>
    <part rel="vector" type="multipart/alternative">
      <part type="application/octet-stream" system="(model: openai.gpt-oss-safeguard-20b)" src="1.vec" />
      <part type="application/octet-stream" system="(model: anthropic.claude-sonnet-4-20250514)" src="2.vec" />
      <part type="application/octet-stream" system="(model: google.gemma-3-27b-it)" src="3.vec" />
    </part>
  </part>
  <part id="ref-1" rel="concept" type="multipart/related">
    <part rel="location" type="text/uri-list">
      <![CDATA[
        https://www.example.org/concepts/resource.xml
        https://www.mirror.org/concepts/resource.xml        
      ]]>
    </part>
  </part>
  <part id="ref-2" rel="concept" type="multipart/related">
    <part rel="vector" type="multipart/alternative">
      <part type="application/octet-stream" system="(model: openai.gpt-oss-safeguard-20b)" src="4.vec" />
      <part type="application/octet-stream" system="(model: anthropic.claude-sonnet-4-20250514)" src="5.vec" />
      <part type="application/octet-stream" system="(model: google.gemma-3-27b-it)" src="6.vec" />
    </part>
  </part>
  <part id="biblio" rel="bibliography" type="text/x-bibtex">
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
</definition>
```

Aspects to be included or expanded upon include: [ontologies](https://en.wikipedia.org/wiki/Ontology_(information_science)), [features](https://en.wikipedia.org/wiki/Feature_(machine_learning)), [predicates](https://en.wikipedia.org/wiki/Predicate_(logic)), [properties](https://en.wikipedia.org/wiki/Property_(philosophy)), binary and n-ary [relations](https://en.wikipedia.org/wiki/Relation_(philosophy)), [reification](https://en.wikipedia.org/wiki/Reification_(knowledge_representation)), and [onomasiological hints](https://en.wikipedia.org/wiki/Onomasiology).

### Client-side Scripting and Template Processing

Adding client-side [scripting](https://en.wikipedia.org/wiki/Scripting_language) and [template processing](https://en.wikipedia.org/wiki/Template_processor) capabilities to the concept-definition representation format, above, are being explored. These would allow reusable, cached templates and their accompanying scripts, or helper functions, to be utilized to produce components of concept definitions.

```xml
<part type="multipart/related" output="application/xhtml+xml">
  <part rel="template" type="text/x-handlebars-template">
    <![CDATA[...]]>
  </part>
  <part rel="script" type="text/javascript">
    <![CDATA[...]]>
  </part>
  <part rel="data" type="application/json">
    <![CDATA[...]]>
  </part>
</part>
```

or:

```xml
<part type="multipart/template" output="application/xhtml+xml">
  <part rel="template" type="text/x-handlebars-template">
    <![CDATA[...]]>
  </part>
  <part rel="script" type="text/javascript">
    <![CDATA[...]]>
  </part>
  <part rel="data" type="application/json">
    <![CDATA[...]]>
  </part>
</part>
```

or, perhaps:

```xml
<part type="multipart/template;output='application/xhtml+xml'">
  <part rel="template" type="text/x-handlebars-template">
    <![CDATA[...]]>
  </part>
  <part rel="script" type="text/javascript">
    <![CDATA[...]]>
  </part>
  <part rel="data" type="application/json">
    <![CDATA[...]]>
  </part>
</part>
```

where, then, the following would be possible:

```xml
<part type="multipart/template;output='application/xhtml+xml'">
  <part rel="template" type="text/x-handlebars-template" src="https://www.templates.org/template-123.hbs" />
  <part rel="script" type="text/javascript" src="https://www.templates.org/template-123.js" />
  <part rel="data" type="application/json">
    <![CDATA[...]]>
  </part>
</part>
```

Interestingly, while template processing is often used with HTML (`application/xhtml+xml`), it could also be used with knowledge graphs (`text/turtle`). In these regards, one can see also: [OTTR](https://www.ottr.xyz/).

### Defining Concepts by Relation to Other Concepts

Drawing inspiration including from [object-oriented modeling](https://en.wikipedia.org/wiki/Object-oriented_modeling), concepts could be defined including by means of being related to one another.

[Comparisons](https://en.wikipedia.org/wiki/Comparison) (or [constrasts](https://en.wikipedia.org/wiki/Contrast_(linguistics))) could be usefully expressed between concepts. [Differences](https://en.wikipedia.org/wiki/Data_differencing) or deltas could be expressed between concepts' definitions and their definitions' parts. However, this task would seemingly be confounded by the extensible, multi-format, nature of the approaches to defining concepts under consideration here.

[Connotation-](https://en.wikipedia.org/wiki/Connotation), [valence](https://en.wikipedia.org/wiki/Valence_(psychology))- or [sentiment](https://en.wikipedia.org/wiki/Sentiment_analysis)-related variations across elements of a set of concepts, for example, could be modeled, with these subtlely varying concepts each extending a shared, base concept while, perhaps, also being definitionally related to one another. This would allow more subtle concept definitions to be built atop one or more more gross ones.

For example, a "stubborn" person could be described as being "strong-willed" or "pig-headed". Although these have a same literal meaning ("stubborn"), "strong-willed" connotes admiration for the level of someone's will (a positive connotation), while "pig-headed" connotes frustration in dealing with someone (a negative connotation).

One could define a base definition pertaining to the concept of "stubbornness" and then define two (or more) concept definitions extending upon it, e.g., "strong-willed" and "pig-headed", each of these extending concept definitions differing from &ndash; and building upon &ndash; one or more base concepts.

## Research Questions

1. How can artificial-intelligence systems, e.g., large language models, be of use for creating and explicating concept definitions?
   1. How can knowledge be extracted from these systems?
2. How can concept definitions be stored, indexed, searched for, and retrieved?
3. How can concept definitions benefit artificial-intelligence systems, agents, and multi-agent systems?
   1. How can (curated and revised) concept definitions be of use for fine-tuning and/or training artificial-intelligence systems?
4. How can concepts be compared, contrasted, and otherwise disambiguated pairwise?
   1. How can context factor into these operations?
5. How can operations be performed on concepts such as [abstraction](https://en.wikipedia.org/wiki/Abstraction), [analogy](https://en.wikipedia.org/wiki/Analogy), [blend](https://en.wikipedia.org/wiki/Conceptual_blending), [combination](https://en.wikipedia.org/wiki/Conceptual_combination), [complement](https://en.wikipedia.org/wiki/Complement_(set_theory)), [intersection](https://en.wikipedia.org/wiki/Intersection_(set_theory)), and [union](https://en.wikipedia.org/wiki/Union_(set_theory))?
   1. How can context factor into these operations?

## See Also

* https://en.wikipedia.org/wiki/Concept
* https://plato.stanford.edu/entries/concepts/
