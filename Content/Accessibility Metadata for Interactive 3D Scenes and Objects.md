# Accessibility Metadata for Interactive 3D Scenes and Objects

## Guiding Questions

Should accessibility metadata for 3D scenes and objects be:

1. hierarchical, enabling an adaptive level of detail?
2. readonly or additionally writeable, e.g., by AI assistants?
3. time-variable, or animatable?

With respect to question #1, one could add a property, `expanded`, with an enumerated value, a toggle for `open` or `closed`, for indicating whether sub-objects or sub-parts are revealed to an adaptive level of detail. If a separate hierarchy is desired for a metadata layer, one could provide objects with `id`s and reference these when creating a tree described in metadata.

With respect to question #2, one could add a property, `readonly`, with an enumerated value for indicating whether an object's metadata were readonly or writeable. A contextually toggleable `expanded` property, however, suggests that metadata content for loaded 3D scenes or objects would be mutable or writeable.

Question #3 is the most challenging.

## Case Studies

1. glTF: https://equalentry.com/accessibility-gltf-objects/
2. OpenUSD: https://openusd.org/release/user_guides/schemas/usdUI/AccessibilityAPI.html
3. X3D: https://www.web3d.org/x3d4

## Examples

### HTML5 Custom Elements

```xml
<3d-graphics alt="..." aria-label="..." aria-description="..." aria-urgency="...">
  <source type="model/gltf+json" src="resource.gltf" />
  <source type="application/vnd.usdz+zip" src="resource.usdz" />
</3d-graphics>
```

### X3D

```xml
<x3d width="800" height="600">
  <scene>
    <meta name="label" content="..." />
    <meta name="description" content="..." />
    <meta name="urgency" content="..." />
  </scene>
</x3d>
```
