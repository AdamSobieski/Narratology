# Adaptation and Personalization

## Introduction

What if Web content could be adapted using _adaptation parameters_, e.g., users' estimated proficiencies in categories and subcategories of scholarly and scientific knowledge?

The same concept, "[photosynthesis](https://en.wikipedia.org/wiki/Photosynthesis)", can be visited in multiple situational contexts, on many educational journeys. One learner, over the course of many years, might revisit this concept in a variety of ways. This suggests a different approach from the traditional encyclopedic organization of knowledge where one concept has one article for all audiences and all situational contexts.

Can Web technologies be developed to enable adaptation and personalization of content?

## Discussion

### Background

While some websites can already perform content customization for users, including based on their site-specific settings and configuration, one value proposition, here, is the portability of users' settings and configuration, of their preferred values for adaptation parameters, across websites of their choosing.

Techniques indicated here would be, in particular, useful for educational websites. Learners of all ages would find the Web to be more readily comprehensible to them as they read and learned about topics both of interest to them and pertaining to their courses.

### Standardization Benefits

Standard approaches to adaptation and personalization would enable Web servers, Web frameworks, content-management systems (or extensions to these), learning-management systems (or extensions to these), and new HTML5 custom elements to support these features.

### User Privacy

As envisioned, users would be in total control of which websites could read from and write to their stored adaptation parameters.

Users who opt out of or do not opt into making use of these features would not provide any preferences for adaptation parameters to websites. As envisioned, this would result in content being presented for the default, or average, user.

### User Experiences

As envisioned, users could, as easily as adjusting text size, adjust pages' adaptation parameters in their Web browsers. Users could be provided a list and/or expandable hierarhical tree of parameters each having a natural-language name and description and a horizontal slider to adjust its value. As or after users adjusted these horizontal sliders, the natural-language content of a webpage would be updated as a result.

Eventually, conversational user-experiences could be delivered for users to ask questions about available adaptation- and personalization-specific alternatives available for documents or for their parts. Conversational user-experiences could also be provided for adjusting those horizontal sliders representing adaptation parameters' values.

### Artificial Intelligence

#### Language Adaptation

Artificial-intelligence systems can "translate" content from one language into that same language, into adapted, personalized, custom-tailored variants for users.

#### Adaptive Explanation

Adaptive explanations would be one use case for these technologies. Existing content-adaptation methods include: additional explanations, prerequisite explanations, comparative explanations, explanation variants, and adaptive content sorting. Existing techniques for providing these include: conditional text, stretchtext, page variants, fragment variants, and frame-based techniques.

#### Intelligent Tutoring Systems

Intelligent tutoring systems and other educational software could, in addition to providing language adaptation and adaptive explanation features, set and calibrate values specified by the aforementioned horizontal slider bars for users, with user permissions.

## Defining Adaptation Parameters

A server could use the `/.well-known` directory to provide an RDF Turtle or a JSON-LD resource definining the available adaptation parameters for the server or for individual resources.

```turtle
<https://en.wikipedia.org/wiki/Category:Botany> edu:label "science/botany" ;
    dc:description "Botany, also called phytology or plant science..."@en .

<https://en.wikipedia.org/wiki/Category:Chemistry> edu:label "science/chemistry" ;
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

## HTTP HEAD Method

Using the HTTP HEAD method, a server could use HTTP headers to specify the adaptation parameters available for a resource and/or a document's alternative resources available for coordinates in adaptation-parameter space.

## HTML Document Metadata

HTML document metadata could be of use for specifying a document's available adaptation parameters and their current or default values for a current document.

```html
<meta name="adaptation-parameter" content="science/botany;q=0.5" />
<meta name="adaptation-parameter" content="science/chemistry;q=0.5" />
```

HTML document metadata could be of use for specifying a document's alternatives available at other coordinates in adaptation-parameter spaces.

```html
<link rel="alternate" data-adaptation="science/botany;q=0.8, science/chemistry;q=0.5" type="text/html" href="2.html" />
<link rel="alternate" data-adaptation="science/botany;q=0.5, science/chemistry;q=0.8" type="text/html" href="3.html" />
<link rel="alternate" data-adaptation="science/botany;q=0.8, science/chemistry;q=0.8" type="text/html" href="4.html" />
```

## HTML Content Metadata and Prerequisites

The same syntax could be used to define prerequisite knowledge for articles, sections, subsections, paragraphs, and spans of content.

```html
<section data-prerequisite="science/botany;q=0.8, science/chemistry;q=0.5">
  <p>...</p>
  <p data-prerequisite="math/calculus;q=0.65">...</p>
  <p>...</p>
</section>
```

## Web Scripting

For single-page applications and other scenarios, JavaScript scripts in resources could handle and/or override events raised when users' adjusted their aforementioned horizontal sliders representing resources' adaptation parameters.

## Beyond Scalars

Must each category have a value that is a scalar? Couldn’t these be tuples for values and confidence scores? Or means and variances?

```http
Accept-Adaptation: science/botany;q=0.82;c=0.9
```

## Default Values

A default value for each adaptation parameter, if not specified by a user, could be 0.5 (perhaps with a confidence modifier of 0.0).

```http
Accept-Adaptation: *;q=0.5;c=0.0
```

## See Also

* [Adaptive Learning](https://en.wikipedia.org/wiki/Adaptive_learning)
* [Adaptive Hypermedia](https://en.wikipedia.org/wiki/Adaptive_hypermedia)
* [Personalized Learning](https://en.wikipedia.org/wiki/Personalized_learning)
* [User Modeling](https://en.wikipedia.org/wiki/User_modeling)
