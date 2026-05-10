## Introduction

Presented, below, are some ideas about [WebVTT](https://w3c.github.io/webvtt/) and time-aligned graphs and datasets.

> [!NOTE]
> The following example shows a time-aligned graph concept.
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

> [!NOTE]
> At instant `00:42.000`, for instance, the corresponding graph or dataset is equivalent to:
>```turtle
>@prefix ex: <http://www.example.org/ns#> .
>
>ex:s1 ex:p1 ex:o1 ;
>       ex:p2 ex:o4 ;
>       ex:p3 ex:o5 .
>```

## Scripting

A metadata track, of type [`TextTrack`](https://html.spec.whatwg.org/multipage/media.html#texttrack), has an [`activeCues`](https://html.spec.whatwg.org/multipage/media.html#dom-texttrack-activecues) property, of type [`TextTrackCueList`](https://html.spec.whatwg.org/multipage/media.html#texttrackcuelist), can be used to construct a simple graph or dataset for an instant. Without using multisets for efficiency, one could merge the graphs or datasets from the active cues, at an instant, into a resultant graph or dataset.

A script, then, could add an event listener to a track's [`cuechange`](https://html.spec.whatwg.org/multipage/media.html#handler-texttrack-oncuechange) event and assemble a resultant simple graph or dataset, e.g., to display, query, or otherwise process it.

## Multisets

As a playhead progresses through a media resource with one or more text tracks, cues are entered and exited. As shown, below, cues could have graphs or datasets. When such a cue is entered, its graph or dataset is to be added to a [multiset](https://en.wikipedia.org/wiki/Multiset) of triples or quads. When such a cue is exited, its graph or dataset is to be removed from that multiset. Multisets can interface as simple graphs or datasets.

Multiset data structures store integers internally for contained elements, a.k.a., their [multiplicities](https://en.wikipedia.org/wiki/Multiplicity_(mathematics)), the number of times that elements have been added to the collection. When an element is removed from a multiset collection, e.g., a triple or quad upon the exiting of a cue, its multiplicity is decremented by one. If its multiplicity becomes equal to zero, it is removed entirely from the multiset collection.

As identical triples or quads can occur in multiple cues' graphs or datasets, multisets of triples and quads could be useful with respect to efficient implementations of providing per-instant simple graphs or datasets from the cues in a metadata track.

## Validation

One could make use of time-aligned [SHACL](https://www.w3.org/TR/shacl/) shapes graphs to define constraints or conditions for time-aligned data graphs and datasets. A time-aligned graph or dataset could express constraints or conditions which are to hold, at an instant, on itself or another graph or dataset.

To express constraints or conditions which must hold for a time-aligned data graph or dataset for the entirety of a time-based resource, one could simply use the start time and end time for the entirety of that time-based resource in one cue in the time-aligned shapes graph.

## Prefix Declaration Blocks for Prefix Directives

In the above example, a `NOTE` comment block was used as a prefix declaration block, as a place to provide prefix directives.

Perhaps one could use `STYLE` blocks to define prefix directives for use in subsequent cues?

> [!NOTE]
> The following example shows a time-aligned graph concept using a `STYLE` block type for prefix directives.
>```webvtt
>WEBVTT
>
>STYLE
>@namespace ex url('http://www.example.org/ns#');
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

Alternatively, perhaps a new WebVTT block type could be created for these scenarios: `DIRECTIVES`?

> [!NOTE]
> The following example shows a time-aligned graph concept using a new `DIRECTIVES` block type for prefix directives.
>```webvtt
>WEBVTT
>
>DIRECTIVES
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

Using these techniques, one could declare time-invariant (entire duration) semantics for how-to knowledge presented in instructional videos, e.g., using Schema.org's [`HowTo`](https://schema.org/HowTo), and utilize time-aligned knowledge to indicate which hierarchical steps and substeps were current, which were being shown in that video at an instant.

### Accessibility

Using these techniques, one could provide corresponding time-aligned [alternative text](https://en.wikipedia.org/wiki/Alt_attribute) and/or [scene graphs](https://en.wikipedia.org/wiki/Scene_graph) for visuospatial content in a video. At each instant, there could be one or more scene graphs describing the visuospatial content in a video.

### Artificial Intelligence

Providing time-aligned semantics for audio and video content has artificial-intelligence applications.

### Computational Narratology

Providing time-aligned semantics for audio and video content has computational-narratology applications.
