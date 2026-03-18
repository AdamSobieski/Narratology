# Adaptation and Personalization

## Introduction

What if Web content could be adapted using _adaptation parameters_, e.g., users' estimated proficiencies in categories and subcategories of scholarly and scientific knowledge?

The same concept, "[photosynthesis](https://en.wikipedia.org/wiki/Photosynthesis)", can be visited in multiple situational contexts, on many educational journeys. One learner, over the course of many years, might revisit this concept in a variety of ways. This suggests a different approach from the traditional encyclopedic organization of knowledge where one concept has one article for all audiences and all situational contexts.

Can Web technologies be developed to enable adaptation of personalization of content?

## Envisioned User Experiences

Users could, as easily as adjusting text size, adjust pages' adaptation parameters in their Web browsers. Users could be provided a list and/or expandable hierarhical tree of parameters each having a natural-language name and description and a horizontal slider to adjust its value. As or after users adjusted these horizontal sliders, the natural-language content of a webpage would be updated as a result.

Adaptive explanations would be one use case for these user experiences.

## Defining Adaptation Parameters

A server could use the `/.well-known` directory to provide an RDF Turtle or a JSON-LD resource definining the available adaptation parameters for the server.

```turtle
<https://en.wikipedia.org/wiki/Category:Botany> rdfs:label "science/botany" ;
    dc:description "Botany, also called phytology or plant science..."@en .

<https://en.wikipedia.org/wiki/Category:Chemistry> rdfs:label "science/chemistry" ;
    dc:description "Chemistry is the scientific study of the properties and behavior of matter."@en .
```

## Adaptation Parameter Identifiers

Note that identifiers for adaptation parameters could be `/`-delimited strings capable of having more than two parts.

```
science/chemistry/organic
```
```
math/calculus/integration/multidimensional
```

In theory, wildcards could also be used.
```
science/chemistry/quantum/*
```


## HTTP Content Negotiation

```http
Accept-Adaptation: science/botany;q=0.82, science/chemistry;q=0.5
```

## HTML Document Metadata

HTML document metadata could be of use for specifying a document's adaptation parameters and their current or default values.

```html
<meta name="adaptation-parameter" content="science/botany;q=0.5" />
<meta name="adaptation-parameter" content="science/chemistry;q=0.5" />
```

## HTML Content Prerequisites

The same syntax could be used to define prerequisite knowledge for articles, sections, subsections, paragraphs, and spans of content.

```html
<section data-prerequisite="science/botany;q=0.8, science/chemistry;q=0.5">
  <p>...</p>
  <p data-prerequisite="math/calculus;q=0.65">...</p>
  <p>...</p>
</section>
```

## Beyond Scalar Values

Must each category have a value that is a scalar? Couldn’t these be tuples for values and confidence scores? Or means and variances?

```http
Accept-Adaptation: science/botany;q=0.82;c=0.9
```

## Artificial Intelligence

Artificial-intelligence systems can "translate" content from one language into that same language, into adapted, personalized, custom-tailored variants for users.
