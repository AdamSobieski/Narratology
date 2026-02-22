# CSVAI

## Introduction

[CSVW](https://w3c.github.io/csvw/primer/) utilizes [JSON-LD](https://json-ld.org/) for metadata in an extensible manner. Accordingly, the `@context` property in a metadata resource describing [CSV](https://en.wikipedia.org/wiki/Comma-separated_values) data could reference the CSVW namespace and also a CSVAI namespace.

```json
{
  "@context": {
    "csvw": "http://www.w3.org/ns/csvw#",
    "csvai": "..."
  },
  "csvw:url": "data.csv"
}
```

## A CSVAI Metadata Schema

A CSVAI schema would build on the CSVW metadata schema. Preliminary ideas for a CSVAI schema include:

1. descriptive metadata for AI systems.
   1. in addition to using [Dublin Core](https://www.dublincore.org/) metadata, CSVAI could define descriptive metadata intended for consumption by AI systems.

2. functionalities could be declared and described
   1. as envisioned, there would be default functionalities for table-groups, tables, and so forth, e.g., functionalities for conversationally selecting and navigating through data visualizations and functionalities from relational algebra and data-querying languages.
   2. custom functionalities could involve usage of the [MCP Tool Schema](https://modelcontextprotocol.io/specification/2025-06-18/schema#tool) or the [Function Ontology](https://fno.io/), perhaps mapping functions with semantic classes for table-groups, tables, columns, rows, and cells.
      1. custom functionalities could be defined for multiple backends.

## Loading Data into Model Context Protocol Servers

One approach for exploration is that dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers could search for, retrieve, and load CSV data having CSVW and CSVAI metadata.

Dynamic, in this case, means that an MCP server could send clients `tools/list_changed` notifications, the list of available [tools](https://modelcontextprotocol.io/docs/learn/server-concepts#tools) could change, during the course of a multimodal conversation.

## Generating Model Context Protocol Servers

Another approach for exploration is that dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers could be automatically generated for and from CSV data having CSVW and CSVAI metadata.

## Use Cases of Interest

In addition to the [twenty-five use cases for CSVW](https://w3c.github.io/csvw/use-cases-and-requirements/#uc), motivating use cases for enhancing CSV metadata to enable AI scenarios include delivering multimodal conversational data-science and analytical-reasoning capabilities for end-users utilizing:

1. opinion-poll and survey data

2. educational data
   1. homework exercises and activities, quizzes, and exam results

These two use cases involve natural-language or multimodal questions and answers or responses provided by populations of respondents. Questions could be text-based strings provided in metadata or external resources referenced by URL (see also: [OCX](https://k12ocx.github.io/k12ocx-specs/), [OER](https://www.oerschema.org/classes/Activity), and [QTI](https://www.1edtech.org/standards/qti/index)).

## Bibliography

* [CSV on the Web: A Primer](https://w3c.github.io/csvw/primer/)
* [Model for Tabular Data and Metadata on the Web](https://w3c.github.io/csvw/syntax/)
* [Metadata Vocabulary for Tabular Data](https://w3c.github.io/csvw/metadata/)
* [Generating JSON from Tabular Data on the Web](https://w3c.github.io/csvw/csv2json/)
* [Generating RDF from Tabular Data on the Web](https://w3c.github.io/csvw/csv2rdf/)
* [Embedding Tabular Metadata in HTML](https://w3c.github.io/csvw/html-note/)
* [Use Cases and Requirements](https://w3c.github.io/csvw/use-cases-and-requirements/)
