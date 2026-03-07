# Accessibility Metadata for Interactive 3D Scenes and Objects

## Introduction

Bridging visuospatial content with semantics will enable many new and interesting technologies and features. These include new accessibility-related and artificial-intelligence-related features involving interactive 3D-graphics content in digital textbooks and instructional materials, blueprints, diagrams, maps, models, and schematics.

## Guiding Questions

Should accessibility metadata for 3D scenes and objects be:

1. available in multiple languages?
2. hierarchical, enabling an adaptive level of detail?
3. capable of expressing user selections?
4. readonly or additionally writeable (e.g., by AI assistants)?
5. capable of providing functionalities (e.g., via MCP)?
6. time-variable, or animatable?
7. embedded in or external to 3D-graphics resources?

## Discussion

### Support for Multiple Languages

There are a number of approaches for supporting multiple languages in JSON- and JSON-LD-based accessibility metadata.

### Hierarchical Structure Enabling an Adaptive Level of Detail

One could add a property, `expanded`, with an enumerated value, a toggle for `open` or `closed`, or a Boolean value, for indicating whether sub-objects or sub-parts are revealed to an adaptive level of detail (see also: [`aria-expanded`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-expanded) and [`aria-level`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-level)). If a separate, parallel tree-like hierarchy is desired for a scene's or object's metadata layer, one could use something like the [ARIA `tree` role](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Roles/tree_role).

### Selectability

One could add a property, `selected`, with an enumerate value, a Boolean value, for indicating whether an object was currently selected (see also: [`aria-selected`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-selected) and [`aria-activedescendant`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-activedescendant)).

### Readability and Writeability

One could add a property, `readonly`, with an enumerated value for indicating whether an object's metadata were readonly or writeable (see also: [`aria-readonly`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-readonly)). Toggleable `expanded` and/or `selected` properties would suggest that at least some of the metadata content for loaded interactive 3D scenes or objects would be writeable.

### Functionality

One could attach [MCP servers](https://modelcontextprotocol.io/docs/learn/server-concepts) and sets of [tools](https://modelcontextprotocol.io/docs/learn/server-concepts#tools) to 3D scenes, objects, and virtual cameras.

### Animation

Changes to accessibility metadata for 3D scenes and objects (e.g., [`aria-label`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-label) and [`aria-description`](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Attributes/aria-description)) could be defined in animation keyframes.

### Embedded or External Metadata

Accessibility metadata could be either embedded inline within 3D-graphics resources or external to these resources (resembling how CSVW metadata resources are external to, reference, and accompany CSV data resources).

With respect to implementation of external metadata resources, 3D scenes, objects, and their parts could have embedded inline identifiers, e.g., URIs or GUIDs, and these identifiers could be referenced by external metadata.

## Envisioned Features

1. Dynamic alt text.
   1. During user-interactions or animations, the alt text available for 3D scenes or objects could be varied.
      1. This includes during motions of virtual cameras and during objects' animations.
   2. Artificial-intelligence systems could produce dynamic alt text from combinations of (accessibility) metadata and screen-captured visual renderings.

2. Conversational user experiences.
   1. This would include visual question-answering about 3D scenes, objects, and their parts.
      1. This would include question-answering about spatial relationships between and distances between objects in 3D scenes.
   2. Might writeable (accessibility) metadata about 3D scenes and objects be modifiable by AI assistants?

3. Undo capabilities.

4. Accessible menus and controls.
   1. Menus and controls displayed within 3D-graphics widgets could be annotated with metadata for accessibility.

5. Automatically-generated interaction menus.
   1. In theory, menus could be generated, including in an on-the-fly manner, for visually-impaired users to be able to manipulate interactive 3D objects.
   2. This might involve calculating the union of those affordances from relevant and/or selected 3D objects.

6. JavaScript interoperability features.

## Existing Formats

1. OpenUSD: https://openusd.org/release/user_guides/schemas/usdUI/AccessibilityAPI.html
2. glTF: https://equalentry.com/accessibility-gltf-objects/
3. X3D: https://www.web3d.org/x3d4

## Examples

### HTML5 Custom Elements

```xml
<model-viewer alt="..." aria-label="..." aria-description="...">
  <source type="application/vnd.usdz+zip" src="resource.usdz" />
  <source type="model/gltf+json" src="resource.gltf" />
  <source type="model/x3d+xml" src="resource.x3d" />
</model-viewer>
```

## See Also

* [Semantic 3D Content Accessibility Community Group](https://www.w3.org/community/semantic-3d-a11y/)
* [Interactive Fiction Accessibility Testing Report](https://accessibility.iftechfoundation.org/)
* https://github.com/w3c/webai/issues/5
* https://github.com/webmachinelearning/webmcp/issues/65
* https://github.com/webmachinelearning/webmcp/issues/91
