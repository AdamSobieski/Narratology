# Declarative Natural-language Generation

## Introduction

A part of the [Adaptation and Personalization](Adaptation%20and%20Personalization.md) project, presented here is that AI systems could transform content outlines (perhaps, eventually, RDF-based semantics) into personalized natural-language content.

That is, HTML custom elements (`<ai-generate>` below) could inform software of how to prompt interoperating LLMs to transform provided outlines of content into personalized natural-language content.

The following examples show a declarative markup-based approach to delivering natural-language generation capabilities:

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain">You are a helpful assistant.</ai-speaker>
  <ai-audience auto="true" /> <!-- use adaptation and personalization API to access users' adaptation parameters -->
  <ai-context>...</ai-context>
  <ai-style>
    :root {
      
    }
    .bold {
      
    }
    .example {
      
    }
  </ai-style>
  <ai-semantics>
    <!-- outline of content -->
    <ol>
      <li class="bold">...</li>
      <li>...</li>
      <li>...</li>
      <ul class="example">
        <li>...</li>
        <li>...</li>
        <li>...</li>
      </ul>
      <li>...</li>
      <li>...</li>
      <li>...</li>
    </ol>
  </ai-semantics>   
</ai-generate>
```

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain" src="prompt-component-1.txt" />
  <ai-audience auto="true" />
  <ai-context type="application/json" src="context.json" />
  <ai-style type="text/css" src="style.css" />
  <ai-semantics type="application/xml" src="outline-1.xml" />
</ai-generate>
```

Speakers' communication objectives and audiences' reading/listening objectives could be separated from the abstract context component and added to the model.

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain" src="prompt-component-1.txt" />
  <ai-speaker-objectives type="text/plain" src="prompt-component-2.txt" />
  <ai-audience auto="true" />
  <ai-audience-objectives type="text/plain" src="prompt-component-3.txt" />
  <ai-context type="application/json" src="context.json" />
  <ai-style type="text/css" src="style.css" />
  <ai-semantics type="application/xml" src="outline-1.xml" />
</ai-generate>
```

## Selecting and Styling Concepts

More theoretically, one could use CSS pseudo-elements to select concepts occurring in parts of content outlines. Envisioning knowledge-graph interoperability and using CSS namespace features, perhaps something like:

```css
@namespace wikidata url('http://www.wikidata.org/entity/');

.example::concept(url(wikidata|Q34969)) { ... }
```

A syntax could be developed to enable CSS-like selectors based on SPARQL ASK query templates.

```css
.example::concept:sparql-ask('https://sparql-endpoint.example.org', 'x', 'SPARQL query string template') { ... }
```

In addition to SPARQL, natural-language questions could be asked of interoperating AI systems about concepts occurring in parts of content outlines in order to select those concepts for styling purposes.

```css
.example::concept('Benjamin Franklin')
{
  valence: positive;
  honorific: high;
}
```

```css
.example::concept:ask('x', 'Is {{x}} a Founding Father of the United States of America?')
{
  valence: positive;
  honorific: high;
}
```

### Additive Cascade

Beyond using CSS cascade to assign singular values to style properties, there is the idea of "additive cascade" where values could be appended to list-like values as selectors matched.

* https://github.com/w3c/csswg-drafts/issues/1594

Natural-language generation instructions and post-generation checks, validation, and evaluation criteria are examples of properties' values which could be additive or list-like. Utilizing traditional-cascade and, perhaps, eventually, additive-cascade techniques, properties' singular and list-like values can be expanded into prompts provided to interoperating AI systems.

## Processing Models

Processing for declarative natural-language generation can occur on servers (documents capable of being precomputed and cached for coordinates, distributions, or regions in users' adaptation-parameter spaces), on users' client devices, and/or by third-party AI services.
