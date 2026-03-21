# Declarative Natural-language Generation

Part of the [Adaptation and Personalization](Adaptation%20and%20Personalization.md) project, presented here is that AI systems could transform content outlines or RDF-based semantics into personalized natural-language content.

The following examples show how a declarative markup-based approach to delivering these capabilities might look:

```html
<ai-generate output="text/html">
  <ai-speaker type="text/plain">You are a helpful assistant.</ai-speaker>
  <ai-audience auto="true" /> <!-- use adaptation and personalization API to access users' adaptation parameters -->
  <ai-context>...</ai-context>
  <ai-style>
    @content {
      
    }
    @concept {
      
    }
    @note {
      
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
