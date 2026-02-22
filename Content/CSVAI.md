# CSVAI

## Introduction

[CSVW](https://w3c.github.io/csvw/primer/) utilizes [JSON-LD](https://json-ld.org/) for declaring metadata in an extensible manner. Accordingly, the `@context` property in a metadata resource describing [CSV](https://en.wikipedia.org/wiki/Comma-separated_values) data could reference both the CSVW namespace and a CSVAI namespace.

```json
{
  "@context": {
    "csvw": "http://www.w3.org/ns/csvw#",
    "csvai": "..."
  },
  "csvw:url": "data.csv"
}
```

CSVW and CSVAI metadata can, together, enable end-users to be able to converse with AI assistants, in natural-language and multimodal dialogues, to perform data science and analytics tasks.

## A CSVAI Metadata Schema

A CSVAI schema would be designed to augment metadata using the CSVW schema. Preliminary ideas for a CSVAI metadata schema include:

1. Descriptive metadata for AI systems.
   1. In addition to using [Dublin Core](https://www.dublincore.org/) metadata, CSVAI could define descriptive metadata intended for consumption by AI systems.

2. Functionalities could be declared and described.
   1. As envisioned, there would be default functionalities for table-groups, tables, and so forth, for conversationally selecting and navigating through data visualizations, functionalities from relational algebra, and from data-querying languages.
   2. Custom functionalities could be expressed using either the [MCP Tool Schema](https://modelcontextprotocol.io/specification/2025-11-25/schema#tool) or [Function Ontology](https://fno.io/). Custom functions could be mapped with semantic classes for table-groups, tables, columns, rows, and cells.
      1. Custom functions could provide one or more implementations, e.g., JavaScript.

## Loading Data Into Model Context Protocol Servers

One approach for exploration involves that dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers could search for, retrieve, and load CSV data providing CSVW and CSVAI metadata. "Dynamic", as used here, means that MCP servers could send clients `tools/list_changed` notifications, lists of available [tools](https://modelcontextprotocol.io/docs/learn/server-concepts#tools) could change, during the course of natural-language or multimodal conversations.

## Generating Model Context Protocol Servers

Another approach for exploration involves that dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers could be automatically generated for and from CSV data providing CSVW and CSVAI metadata.

## Web-browser Scenarios

In the not-too-distant future, Web developers may be able to utilize [WebMCP](https://github.com/webmachinelearning/webmcp/) to deliver natural-language or multimodal conversational data science and analytics functionalities to end-users using Web browsers. Data could be stored either client-side or server-side.

## Use Cases

In addition to the [twenty-five use cases for CSVW](https://w3c.github.io/csvw/use-cases-and-requirements/#uc), two motivating use cases for CSVAI are listed below.

1. Opinion-poll and survey data.

2. Educational data.
   1. Homework exercises and activities, quizzes, and exam results.

These two use cases involve natural-language or multimodal questions and responses provided by populations of respondents. Questions could be text-based strings provided in metadata or external resources referenced by URL (see also: [OCX](https://k12ocx.github.io/k12ocx-specs/), [OER](https://www.oerschema.org/classes/Activity), and [QTI](https://www.1edtech.org/standards/qti/index)).

## Bibliography

* [CSV on the Web: A Primer](https://w3c.github.io/csvw/primer/)
* [Model for Tabular Data and Metadata on the Web](https://w3c.github.io/csvw/syntax/)
* [Metadata Vocabulary for Tabular Data](https://w3c.github.io/csvw/metadata/)
* [Generating JSON from Tabular Data on the Web](https://w3c.github.io/csvw/csv2json/)
* [Generating RDF from Tabular Data on the Web](https://w3c.github.io/csvw/csv2rdf/)
* [Embedding Tabular Metadata in HTML](https://w3c.github.io/csvw/html-note/)
* [Use Cases and Requirements](https://w3c.github.io/csvw/use-cases-and-requirements/)
