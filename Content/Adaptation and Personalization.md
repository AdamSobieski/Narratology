# Adaptation and Personalization

## Introduction

The same concept, for example "[photosynthesis](https://en.wikipedia.org/wiki/Photosynthesis)", can be visited in multiple situational contexts, at many points on many educational journeys. One learner, over the course of years, might revisit this concept in a number of ways. This suggests a different approach from the traditional organization and presentation of knowledge where one concept has one article intended for all audiences and all situational contexts.

What if Web content could be adapted using _adaptation parameters_, these, for example, corresponding with categories and subcategories of scholarly and scientific knowledge?

Can Web technologies be developed to enable adaptation and personalization of content?

## Discussion

### Background

While some websites can already perform content customization for users using site-specific settings and configuration, a value proposition of the techniques under discussion is that users' preferred values for adaptation parameters could be portable across the websites of their choosing.

The techniques indicated here would be particularly useful for educational, scholarly, and scientific websites. Learners of all ages would find the Web to be more readily comprehensible to them as they read, comprehended, and learned about topics both of interest to them and pertaining to their courses and tasks.

### Standardization Benefits

Standard approaches to adaptation and personalization would enable Web servers, Web frameworks, content-management systems (or extensions to these), learning-management systems (or extensions to these), and new HTML5 custom elements to support and deliver these features.

### User Privacy

As envisioned, users would be in total control of which websites could read from and write to their stored adaptation parameters.

Users who opt out of or do not opt into making use of these features would not provide any preferences for adaptation parameters to websites. As envisioned, this would result in content being presented for the default, or average, user.

### Envisioned User Experiences

Using a toggle button (default off), users could toggle adaptation and personalization features per website. If toggled on for a website, users could, as easily as adjusting text size, adjust pages' adaptation parameters in their Web browsers. Users could be provided a list and/or expandable hierarhical tree of parameters each having a natural-language name and description and a horizontal slider to adjust its value. As or after users adjusted these horizontal sliders, the natural-language content of a webpage would be updated as a result.

Users could be notified when alternative versions of webpages &ndash; adapted and personalized to coordinates, distributions, or regions in adaptation-parameter spaces &ndash; were available. For example, clicking upon, or selecting, visible notifications would result in opening the aforementioned tree of horizontal sliders, but with points on the sliders for indicating the coordinates or regions of available alternative content.

Eventually, conversational user-experiences could be delivered for users to make use of to ask their AI assistants questions about available adaptation- and personalization-specific alternatives available for websites or their parts. User-experiences could also be provided for conversationally adjusting the envisioned trees of horizontal sliders representing adaptation parameters' values.

### Artificial Intelligence

#### Language Adaptation

Artificial-intelligence systems can "translate" content from one language into that same language, into adapted, personalized, custom-tailored variants for users.

#### Adaptive Explanation

Adaptive explanations would be one use case for these technologies. Existing content-adaptation methods include: additional explanations, prerequisite explanations, comparative explanations, explanation variants, and adaptive content sorting. Existing techniques for providing these include: conditional text, stretchtext, page variants, fragment variants, and frame-based techniques.

#### Intelligent Tutoring Systems

Intelligent tutoring systems and other educational software could, in addition to providing language adaptation and adaptive explanation features, set and calibrate values specified by the aforementioned horizontal slider bars for users, with user permissions.

## Defining Adaptation Parameters

A server could use the `/.well-known` directory to provide an RDF Turtle or a JSON-LD resource definining the available adaptation parameters for the server or for individual resources.

Such resources might resemble:
```turtle
<#1> rdf:type edu:AdaptationParameter ;
     edu:label "science/botany" ;
     edu:description "Botany, also called phytology or plant science..."@en ;
     edu:sameAs <https://en.wikipedia.org/wiki/Category:Botany> .

<#2> rdf:type edu:AdaptationParameter ;
     edu:label "science/chemistry" ;
     edu:description "Chemistry is the scientific study of the properties and behavior of matter."@en ;
     edu:sameAs <https://en.wikipedia.org/wiki/Category:Chemistry> .

<#3> rdf:type edu:AdaptationParameter ;
     edu:label "science/chemistry/organic" ;
     edu:description "Organic chemistry is a subdiscipline within chemistry studying organic..."@en ;
     edu:sameAs <https://en.wikipedia.org/wiki/Category:Organic_chemistry> ;
     edu:hasSuperParameter <#2> .
```

As considered, adaptation parameters' identifiers or labels, `/`-delimited strings, could have more than two parts.

```
science/chemistry/organic
```
```
math/calculus/integration/multidimensional
```

## HTTP Content Negotiation

```http
Accept-Adaptation: science/botany;q=0.82, science/chemistry;q=0.5
```

Wildcard symbols could also be utilized.
```http
Accept-Adaptation: science/chemistry/quantum/*;q=0.81
```

## HTTP HEAD Method

Using the HTTP HEAD method, a server could use HTTP headers to specify the adaptation parameters available for a resource and/or a document's alternative resources available for coordinates in adaptation-parameter space.

## HTML Document Metadata

HTML document metadata could be of use for specifying a document's available adaptation parameters and their current values for a document.

```html
<meta name="adaptation-parameter" content="science/botany;q=0.5" />
<meta name="adaptation-parameter" content="science/chemistry;q=0.5" />
```

HTML document metadata could be of use for specifying a document's alternatives available at other coordinates in or for other regions in adaptation-parameter spaces.

```html
<link rel="alternate" data-adaptation="science/botany;q=0.8, science/chemistry;q=0.5" type="text/html" href="2.html" />
<link rel="alternate" data-adaptation="science/botany;q=0.5, science/chemistry;q=0.8" type="text/html" href="3.html" />
<link rel="alternate" data-adaptation="science/botany;q=0.8, science/chemistry;q=0.8" type="text/html" href="4.html" />
```

## HTML Attributes and Fine-grained Control

Resembling HTML's [`translate`](https://www.w3.org/International/questions/qa-translate-flag) attribute, an attribute could be used to provide a fine-grained level of control with which to indicate whether specific content should be adapted or personalized.

Beyond a Boolean control over adaptation and personalization, perhaps a vocabulary for more detailed hints could be developed.

## HTML Attributes and Content Prerequisites

The same syntax (or some other syntax) could be used to specify prerequisite knowledge for articles, sections, subsections, paragraphs, and spans of content.

```html
<section data-prerequisite="science/botany;q=0.8, science/chemistry;q=0.5">
  <p>...</p>
  <p data-prerequisite="math/calculus;q=0.65">...</p>
  <p>...
    <span class="term" data-prerequisite="science/botany;q=0.6">...</span>
    ...
  </p>
</section>
```

## HTML Custom Elements

Resembling HTML media tags, the `<source>` tag, and the `media` attribute, for the following custom element, `<adaptive-content>`, a browser would select the first or best matching `<content-source>` tag using the `data-prerequisite` attribute.

```html
<adaptive-content>
  <content-source data-prerequisite="...">...</content-source>
  <content-source data-prerequisite="...">...</content-source>
  <content-source data-prerequisite="...">...</content-source>
</adaptive-content>
```

## Scripting

For single-page applications and other scenarios, JavaScript scripts in resources could handle and/or override events raised when users' adjusted their aforementioned envisioned horizontal sliders representing resources' adaptation parameters.

## Beyond Scalars

Must each category have a value that is a scalar? Couldn’t these be tuples for values and confidence scores? Or means and variances?

```http
Accept-Adaptation: science/botany;q=0.82;c=0.9
```

## User-modeling Stereotypes and Default Values

A default value for each adaptation parameter, if not specified by a user, could be 0.5 (perhaps with a confidence modifier of 0.0). This value could be defined to correspond with the average user.

```http
Accept-Adaptation: *;q=0.5;c=0.0
```

It might be useful to be able to succinctly express different average values for different demographics, e.g., average reading levels for students in grades K-12, students in university, and personnel employed in different categories of the workforce.

It might be useful to provide a means for users to be able to succinctly indicate one or more demographics for these purposes, so as to be able to succinctly express adaptation parameters' values relative to demographics-based user-model stereotypes.

For example, a contemporary botanist with expressed preferences for a better-than-average organic-chemistry reading level and a lower-than-average ecology reading level (for a contemporary botanist) might be able to succinctly express these adaptation parameters in a manner resembling:

```http
Accept-Adaptation-Stereotype: botanist;y=2026
Accept-Adaptation: science/chemistry/organic;r=+0.15, science/biology/ecology;r=-0.1
```

## Open Questions

1. With respect to artificial-intelligence systems "translating", adapting, personalizing, or custom-tailoring content for users, how should users' expressed preferences, these being coordinates, distributions, or regions in adaptation-parameter spaces, be mapped to natural-language user descriptions provided to these AI systems in the form of prompts?
2. How should AI-adapted content best be evaluated?
   1. AI adaptations of content should preserve semantics, voice, style, and tone.
3. How can AI systems help people to create (adaptive hypermedia) content for one or more described intended audiences?
4. How can software tools help users to quickly and easily calibrate and/or modify their adaptation-parameter preferences?
5. Which other syntax possibilities exist for specifying `data-prerequisite` attributes' values?
   1. Perhaps ones based on the [media queries](https://www.w3.org/TR/mediaqueries-4/) syntax, ones capable of numerical comparisons, conjunctions, and disjunctions?
6. Might fine-grained control of adaptation and personalization, this potentially involving detailed hints, be a topic for HTML style?
   1. Content in `<q>`, `<blockquote>`, and `<cite>` elements, for instance, might have special default values.
7. Might there be expressiveness for categories of adaptation parameters to have differing access-control and permissions requirements?
8. Could adaptation parameters encompass, beyond users' knowledge-related preferences, users' goals, intents, task contexts, or other preferences?

## See Also

* [Adaptive Hypermedia](https://en.wikipedia.org/wiki/Adaptive_hypermedia)
* [Adaptive Learning](https://en.wikipedia.org/wiki/Adaptive_learning)
* [Personalized Learning](https://en.wikipedia.org/wiki/Personalized_learning)
* [User Modeling](https://en.wikipedia.org/wiki/User_modeling)
