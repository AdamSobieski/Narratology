# Declarative Natural-language Generation

## Introduction

Part of the [Adaptation and Personalization](Adaptation%20and%20Personalization.md) project, presented here is that AI systems could transform content outlines (or, perhaps eventually, RDF-based semantics) into personalized natural-language content.

That is, HTML custom elements (`<ai-generate>` below) could inform software of how to prompt interoperating LLMs to transform provided outlines of content into personalized natural-language content.

## Natural-language Generation

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
  <ai-speaker type="text/plain" src="prompt-1.txt" />
  <ai-audience auto="true" />
  <ai-context type="application/json" src="context.json" />
  <ai-style type="text/css" src="style.css" />
  <ai-semantics type="application/xml" src="outline-1.xml" />
</ai-generate>
```

## Selecting and Styling Concepts

In theory, one could use CSS pseudo-elements to select &ndash; for styling purposes &ndash; concepts occurring in parts of content outlines.

```css
.example::concept('George Washington') {
  
}
```

Perhaps this expressiveness could be used with CSS variables.

```css
.example::concept(var(--subject)) {
  
}
```

## Communication Objectives and their Evaluation

The evaluation of a portion of content can be stated as evaluating: (1) whether communication objectives, including and beyond the expression of the content in the outline, were achieved by the natural language generated for a situational context, and (2) whether audience's objectives were achieved by reading or listening to the content.

Accordingly, speakers' communication objectives and audiences' reading/listening objectives could be separated from the abstract context component and added to the model.

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain" src="prompt-1.txt" />
  <ai-speaker-objectives type="text/plain" src="prompt-2.txt" />
  <ai-audience auto="true" />
  <ai-audience-objectives type="text/plain" src="prompt-3.txt" />
  <ai-context type="application/json" src="context.json" />
  <ai-style type="text/css" src="style.css" />
  <ai-semantics type="application/xml" src="outline-1.xml" />
</ai-generate>
```
