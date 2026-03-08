# Accessibility Metadata for Interactive 3D Scenes and Objects

## Introduction

Bridging visuospatial content with semantics will enable many new and exciting technologies and features. These technologies and features pertain to both accessibility and artificial intelligence, involving interactive 3D-graphics content in digital textbooks and instructional materials, blueprints, charts, diagrams, games, maps, models, and schematics.

## Questions

Should accessibility metadata for 3D scenes, objects, and their parts be:

1. available in multiple languages?
2. hierarchical, enabling an adaptive level of detail?
3. selectable?
4. readonly or writeable (e.g., by AI assistants)?
5. static or dynamic?
6. capable of providing functionalities (e.g., via MCP)?
7. animatable?
8. embedded in or external to 3D-graphics resources?

## Discussion

### Support for Multiple Languages

There are a number of approaches for supporting multiple languages in accessibility metadata.

### Hierarchical Structure Enabling an Adaptive Level of Detail

One could add a property, `expanded`, with an enumerated value, a toggle for `open` or `closed`, or a Boolean value, for indicating whether sub-objects or sub-parts are revealed to an adaptive level of detail (see also: [`aria-expanded`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-expanded) and [`aria-level`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-level)).

If a separate, parallel tree-like hierarchy is desired for a scene's or object's metadata layer, one could use something like the ARIA [`tree`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Roles/tree_role) and [`treeitem`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Roles/treeitem_role) roles.

3D scenes may have multiple [mereological](https://plato.stanford.edu/entries/mereology/) decompositions into objects, and objects multiple such decompositions into parts, including beyond the default one or ones provided by content authors.

### Selectable

One could add a property, `selected`, with an enumerated value, a Boolean value, for indicating whether one or more objects were selected (see also: [`aria-selected`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-selected) and [`aria-activedescendant`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-activedescendant)).

### Readonly or Writeable

One could add a property, `readonly`, with an enumerated value for indicating whether an object's metadata were readonly or writeable (see also: [`aria-readonly`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-readonly)). Toggleable `expanded` and/or `selected` properties would suggest that at least some of the metadata content for loaded interactive 3D scenes or objects would be writeable.

### Static or Dynamic

In addition to accessibility metadata being capable of being selectable and writeable, it may also be dynamic. If dynamic, end-users or their AI assistants could add, remove, and modify metadata content, mapping it to 3D visuospatial content. End-users could, for example, "reify" ad-hoc selections of groups of objects or their parts and attach new metadata content to these (see also: the [DOM](https://dom.spec.whatwg.org/) and [RDFJS](https://rdf.js.org/) APIs).

### Functional

One could attach [MCP servers](https://modelcontextprotocol.io/docs/learn/server-concepts) and sets of [tools](https://modelcontextprotocol.io/docs/learn/server-concepts#tools) to 3D scenes, objects, and virtual cameras.

### Animatable

Changes to 3D scenes' and objects' metadata (e.g., [`aria-label`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-label) and [`aria-description`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-description)) could be defined in animation keyframes.

### Embedded or External

Accessibility metadata could be either embedded inline within 3D-graphics resources or external to these resources (resembling how [CSVW](https://www.w3.org/TR/tabular-data-model/) metadata resources are external to, reference, and can accompany CSV data resources).

With respect to implementation of external metadata resources, 3D scenes, objects, their parts, and animation keyframes could have embedded inline identifiers, e.g., URIs or GUIDs, and these identifiers could be referenced by external metadata resources. Some 3D-graphics formats utilize archive formats and, in these cases, metadata resources could accompany other resources in such archives.

## Envisioned Features

1. Dynamic alt text.
   1. During user-interactions or animations, the alt text available for 3D scenes or objects could be varied.
      1. This includes during motions of virtual cameras and during objects' animations.
   2. Artificial-intelligence systems could produce dynamic alt text from combinations of (accessibility) metadata and screen-captured visual renderings.

2. Conversational user experiences.
   1. This would include visual question-answering about 3D scenes, objects, and their parts.
      1. This would include question-answering about spatial relationships between and distances between objects in 3D scenes.
   2. Might writeable (accessibility) metadata about 3D scenes and objects be modifiable by AI assistants?

3. Undo and redo capabilities.
   1. 3D-graphics widgets could interoperate with the [History API](https://developer.mozilla.org/en-US/docs/Web/API/History_API) in Web browsers.

4. Accessible menus and controls.
   1. Menus and controls displayed within 3D-graphics widgets could be annotated with metadata for accessibility.

5. Automatically-generated interaction menus.
   1. In theory, menus could be generated, including in an on-the-fly manner, to enable the manipulation of interactive 3D objects.
   2. This might involve computing the unions of affordances from 3D scenes' relevant and/or selected objects.

6. Other JavaScript interoperability features.

## Implementation Ideas

1. Perhaps 3D-graphics widgets (see: `<model-viewer>`) could, for their JavaScript APIs, provide (virtual) (mutable) DOM trees such that `aria-` attributes and their values on these trees would be bidirectionally mapped with any semantic models of the 3D-graphics contents.
2. Perhaps 3D-graphics widgets (see: `<model-viewer>`) could, for their JavaScript APIs, provide knowledge graphs for their metadata such that these (mutable) graphs would be bidirectionally mapped with any semantic models of the 3D-graphics contents.
   1. `http://www.w3.org/WAI/ARIA/` is the namespace to use when representing ARIA roles, states, and properties in formats like RDF Turtle and JSON‑LD.
3. With respect to affordances and functionalities, perhaps those DOM elements or knowledge-graph nodes of 3D-graphics contents' semantic models could each have sets of MCP tools.

## Existing Formats

1. OpenUSD: https://openusd.org/release/user_guides/schemas/usdUI/AccessibilityAPI.html
2. glTF: https://equalentry.com/accessibility-gltf-objects/
3. X3D: https://www.web3d.org/x3d4

## Examples

```xml
<model-viewer alt="..." aria-label="..." aria-description="...">
  <source type="application/vnd.usdz+zip" src="resource.usdz" />
  <source type="model/gltf+json" src="resource.gltf" />
  <source type="model/x3d+xml" src="resource.x3d" />
</model-viewer>
```

```turtle
@prefix aria: <http://www.w3.org/WAI/ARIA/> .
@prefix rdf:  <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .

<#myButton> rdf:type aria:button ;
            aria:pressed "false" .
```

## See Also

* [Semantic 3D Content Accessibility Community Group](https://www.w3.org/community/semantic-3d-a11y/)
* [Accessible Player Experiences (APX)](https://accessible.games/accessible-player-experiences/)
* [Interactive Fiction Technology Foundation Accessibility Testing Report](https://accessibility.iftechfoundation.org/)
* [WAI-ARIA Overview](https://www.w3.org/WAI/standards-guidelines/aria/)

<br>

* [Mereology](https://plato.stanford.edu/entries/mereology/)
* [Location and Mereology](https://plato.stanford.edu/entries/location-mereology/)
* [Temporal Parts](https://plato.stanford.edu/entries/temporal-parts/)

<br>

* https://github.com/w3c/webai/issues/5
* https://github.com/webmachinelearning/webmcp/issues/65
* https://github.com/webmachinelearning/webmcp/issues/91
