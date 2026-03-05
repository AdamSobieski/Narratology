# Accessibility Metadata for Interactive 3D Scenes and Objects

## Guiding Questions

Should accessibility metadata for 3D scenes and objects be:

1. available in multiple languages?
2. hierarchical, enabling an adaptive level of detail?
3. capable of expressing user selections?
4. readonly or additionally writeable (e.g., by AI assistants)?
5. capable of providing functionalities (e.g., via MCP)?
6. time-variable, or animatable?

## Introduction

### Support for Multiple Languages

There are a number of approaches for supporting multiple languages in JSON- and JSON-LD-based accessibility metadata.

### Hierarchical Structure Enabling an Adaptive Level of Detail

One could add a property, `expanded`, with an enumerated value, a toggle for `open` or `closed`, or a Boolean value, for indicating whether sub-objects or sub-parts are revealed to an adaptive level of detail (see also: `aria-expanded`). If a separate, parallel tree-like hierarchy is desired for a scene's or object's metadata layer, one could use something like [ARIA `tree` role](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/Reference/Roles/tree_role).

### User Selections

One could add a property, `selected`, with an enumerate value, a Boolean value, for indicating whether an object was currently selected (see also: `aria-selected`).

### Readonly and Writeable

One could add a property, `readonly`, with an enumerated value for indicating whether an object's metadata were readonly or writeable. Toggleable `expanded` and/or `selected` properties would suggest that at least some of the metadata content for loaded interactive 3D scenes or objects would be writeable.

### Adding Functionalities

One could attach MCP servers and sets of tools to 3D scenes, objects, and virtual cameras.

### Animatability

Coming soon.

## Considered Features

1. Dynamic alt text. During user-interactions or animations, that alt text available for 3D scenes or objects could be varied. This includes during motions of virtual cameras and during animations, e.g., rotations of objects, where different parts of objects would be visible to a virtual camera as a result.
   1. Artificial-intelligence systems could obtain these dynamic data from combinations of (accessibility) metadata and screen-captured visual renderings of 3D scenes or objects.
2. AI-enabled conversational user experiences for interacting with 3D scenes and objects.

## Case Studies

1. OpenUSD: https://openusd.org/release/user_guides/schemas/usdUI/AccessibilityAPI.html
2. glTF: https://equalentry.com/accessibility-gltf-objects/
3. X3D: https://www.web3d.org/x3d4

## Examples

### HTML5 Custom Elements

```xml
<model-viewer alt="..." aria-label="..." aria-description="..." aria-urgency="...">
  <source type="application/vnd.usdz+zip" src="resource.usdz" />
  <source type="model/gltf+json" src="resource.gltf" />
  <source type="model/x3d+xml" src="resource.x3d" />
</model-viewer>
```
