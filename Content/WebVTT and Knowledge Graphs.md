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
>ex:s1 ex:p0 ex:o0 .
>
>00:22.000 --> 00:27.000
>ex:s1 ex:p1 ex:o1 .
>ex:s1 ex:p2 ex:o2 .
>
>00:40.000 --> 00:43.000
>ex:s1 ex:p1 ex:o3 .
>ex:s1 ex:p2 ex:o4 .
>
>00:58.000 --> 02:00.000
>ex:s1 ex:p1 ex:o5 .
>ex:s1 ex:p2 ex:o6 .
>```

## Considered Use Cases

### Instructional Video

Using such techniques, one could declare time-invariant semantics for how-to knowledge presented in videos, e.g., using Schema.org's [`HowTo`](https://schema.org/HowTo), and utilize time-aligned knowledge to indicate which hierarchical steps and substeps were current, which were being shown in that video, at an instant.

### Scene Graphs

Using such techniques, one could provide a corresponding time-aligned scene graph for the visuospatial content in videos. At each instant, there could be a scene graph describing the visual contents in a video.
