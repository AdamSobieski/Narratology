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

Speakers' communication objectives and audiences' reading/listening objectives could be separated from the (abstract) context component and be subsequently added to the model.

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain" src="prompt-component-1.txt" />
  <ai-speaker-objectives type="text/plain" src="prompt-component-2.txt" />
  <ai-audience auto="true" />
  <ai-audience-objectives type="text/plain" src="prompt-component-3.txt" />
  <ai-style type="text/css" src="style.css" />
  <ai-semantics type="application/xml" src="outline.xml" />
</ai-generate>
```

## Enhancing Content Outlines

Is HTML outline markup (`<ol>`, `<ul>`, `<li>`) sufficient for representing content outlines for these purposes?

Could some additional elements, e.g., `<a>`, `<term>`, and `<cite>`, be of use?

Could some attributes for additional semantics, e.g., `role` and/or `data-*` attributes, be of use?

Could some attributes for indicating adaptation-parameter-space coordinates, distributions, and/or regions be of use?

**See also**: [OML](https://en.wikipedia.org/wiki/OML_(computer_format)), [OPML](https://en.wikipedia.org/wiki/OPML), [XBEL](https://en.wikipedia.org/wiki/XBEL), and [XOXO](https://en.wikipedia.org/wiki/XOXO_(microformat)).

### Bibliographic Citations

Should bibliographies be included in content outlines or be separated into their own model components?

```html
<outline xmlns="..." xmlns:argu="...">
  <head>
    <bibliography>
      <book id="a" title="The big book of concepts" author="Murphy, Gregory" year="2004" publisher="MIT Press" />
      <book id="b" title="A study of concepts" author="Peacocke, Christopher" year="1992" publisher="MIT Press" />
      <book id="c" title="A philosophical history of the concept" editor="Schmid, Stephan and Taieb, Hamid" year="2026" publisher="Cambridge University Press" />
    </bibliography>
  </head>
  <body>
    <ol role="argu:argument">
      <li role="argu:conclusion">...</li>
      <ul role="argu:support">
        <li><cite ref="a">...</cite></li>
        <li><cite ref="b c">...</cite></li>
      </ul>
    </ol>
  </body>
</outline>
```

```html
<outline xmlns="..." xmlns:argu="...">
  <head>
    <bibliography src="bibliography.xml" />
  </head>
  <body>
    <ol role="argu:argument">
      <li role="argu:conclusion">...</li>
      <ul role="argu:support">
        <li><cite ref="bibliography.xml#a">...</cite></li>
        <li><cite ref="bibliography.xml#b bibliography.xml#c">...</cite></li>
      </ul>
    </ol>
  </body>
</outline>
```

```html
<ol role="argu:argument" xmlns="..." xmlns:argu="...">
  <li role="argu:conclusion">...</li>
  <ul role="argu:support">
    <li><cite ref="bibliography.xml#a">...</cite></li>
    <li><cite ref="bibliography.xml#b bibliography.xml#c">...</cite></li>
  </ul>
</ol>
```

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain" src="prompt-component-1.txt" />
  <ai-speaker-objectives type="text/plain" src="prompt-component-2.txt" />
  <ai-audience auto="true" />
  <ai-audience-objectives type="text/plain" src="prompt-component-3.txt" />
  <ai-style type="text/css" src="style.css" />
  <ai-semantics type="application/xml" src="outline.xml" />
  <ai-bibliography type="application/xml" src="bibliography.xml" />
</ai-generate>
```

### Adaptive Explanations

Perhaps markup representing adaptive explanations could enhance content outlines.

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain">You are a helpful assistant.</ai-speaker>
  <ai-audience auto="true" />
  <ai-context>...</ai-context>
  <ai-style>
    :root {
      
    }
    .example {
      
    }
  </ai-style>
  <ai-semantics>
    <ai-explanation>
      <ai-explanandum>
        <ol>
          <li class="example">...</li>
          <li>...</li>
          <li>...</li>
        </ol>
      </ai-explanandum>
      <ai-explanans adaptation="um('science/botany') < 0.5">
        ...
      </ai-explanans>
      <ai-explanans adaptation="0.5 <= um('science/botany') < 0.75">
        ...
      </ai-explanans>
      <ai-explanans adaptation="0.75 <= um('science/botany')">
        ...
      </ai-explanans>
    </ai-explanation>
  </ai-semantics>
  <ai-bibliography type="application/xml" src="bibliography.xml" />
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

Processing for declarative natural-language generation could occur on servers, on users' client devices, and/or on third-party cloud-based AI services.

For server-side computing scenarios, custom server controls could be of use. To provide faster page loading, custom server control logic could interoperate with page markup to enable dynamic content loading for AI-generated content. Benefits of server-side or combined client-server approaches would include the capabilities to precompute and cache outputs for adaptation-parameter-space coordinations, distributions, and regions.

## See Also

* [Natural-language Generation](https://en.wikipedia.org/wiki/Natural_language_generation)
