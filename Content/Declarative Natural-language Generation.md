# Declarative Natural-language Generation

## Introduction

Part of the [Adaptation and Personalization](Adaptation%20and%20Personalization.md) project, presented here is that AI systems could transform content outlines (or, eventually, RDF-based semantics) into personalized natural-language content.

That is, HTML custom elements (`<ai-generate>` below) could inform software of how to prompt interoperating LLMs to transform provided outlines of content into personalized natural-language content.

## Natural-language Generation

The following examples show a declarative markup-based approach to delivering natural-language generation capabilities:

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain">You are a helpful assistant.</ai-speaker>
  <ai-audience auto="true" /> <!-- use adaptation and personalization API to access users' adaptation parameters -->
  <ai-context>...</ai-context>
  <ai-style>
    @content {
      
    }
    .bold {
      
    }
    .example {
      
    }
  </ai-style>
  <ai-semantics> <!-- outline of content -->
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
  <ai-speaker type="text/plain" src="prompt.txt" />
  <ai-audience auto="true" />
  <ai-context type="application/json" src="context.json" />
  <ai-style type="text/css" src="style.css" />
  <ai-semantics type="application/xml" src="outline-1.xml" />
</ai-generate>
```

## Natural-language Evaluation

In addition to generating natural-language in a declarative way, there is to consider its evaluation. Means of specifying evaluation criteria for resultant generated content should be expressible using declarative markup approaches.

Perhaps evaluation criteria could be expressed using the same selectors used to indicate style preferences? As considered, evaluation criteria would be accumulated based on the existence of matching elements in the content outline and processed upon resultant output content.

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain">You are a helpful assistant.</ai-speaker>
  <ai-audience auto="true" /> <!-- use adaptation and personalization API to access users' adaptation parameters -->
  <ai-context>...</ai-context>
  <ai-style>
    @content {
      
    }
    .bold {
      
    }
    .example {
      
    }
  </ai-style>
  <ai-semantics> <!-- outline of content -->
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
  <ai-evaluation>
    @content {
      
    }
    .bold {
      
    }
    .example {
      
    }
  </ai-evaluation>
</ai-generate>
```
