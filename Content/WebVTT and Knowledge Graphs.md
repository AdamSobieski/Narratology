## Introduction

Presented, below, are some ideas about [WebVTT](https://w3c.github.io/webvtt/) and time-aligned knowledge graphs, i.e., "timed Turtle" and "timed TriG".

A time-aligned graph is shown in the following example. As a playhead progresses through a multimedia resource, for example, triples are both added and removed from a corresponding time-aligned knowledge graph. Each cue declares that a time-aligned graph should contain its contents from its start time to its end time. At each instant, then, there is a graph or dataset defined.

> [!NOTE]
> Example 1
>```webvtt
>WEBVTT
>
>NOTE
>@prefix ex: <http://www.example.org/ns#> .
>
>00:00.000 --> 02:00.000
>ex:s1 ex:p1 ex:o1 .
>
>00:22.000 --> 00:27.000
>ex:s1 ex:p2 ex:o2 .
>ex:s1 ex:p3 ex:o3 .
>
>00:40.000 --> 00:43.000
>ex:s1 ex:p2 ex:o4 .
>ex:s1 ex:p3 ex:o5 .
>
>00:58.000 --> 02:00.000
>ex:s1 ex:p2 ex:o6 .
>ex:s1 ex:p3 ex:o7 .
>```

## Considered Use Cases

### Education

Using these techniques, one could declare time-invariant semantics for how-to knowledge presented in instructional videos, e.g., using Schema.org's [`HowTo`](https://schema.org/HowTo), and utilize time-aligned knowledge to indicate which hierarchical steps and substeps were current, which were being shown in that video, at an instant.

### Accessibility

Using these techniques, one could provide corresponding time-aligned [alternative text](https://en.wikipedia.org/wiki/Alt_attribute) and/or [scene graphs](https://en.wikipedia.org/wiki/Scene_graph) for visuospatial content in a video. At each instant, there could be one or more scene graphs describing the visuospatial content in a video.

### Artificial Intelligence

Providing time-aligned semantics for video content has artificial-intelligence applications.

### Computational Narratology

Providing time-aligned semantics for video content has computational-narratology applications.
