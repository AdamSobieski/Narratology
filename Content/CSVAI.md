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

A CSVAI schema would build on CSVW metadata. Preliminary ideas for a CSVAI schema include:

1. descriptive metadata for AI systems.
   1. in addition to using [Dublin Core](https://www.dublincore.org/) metadata, CSVAI could define descriptive metadata intended for consumption by AI systems.

2. functionalities could be declared and described
   1. as envisioned, there could be default functionalities for automatically-generated dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers, functionalities for conversationally selecting and navigating through data visualizations and functionalities from relational algebra and data-querying languages.
   2. custom functionalities might involve the [Function Ontology](https://fno.io/), perhaps mapping functions with semantic classes for cells, rows, columns, tables, and table-groups.
      1. custom functionalities could be declared for multiple backends.

## Generating Dynamic Model Context Protocol Servers

A CSVAI schema could enable and enhance AI scenarios including enabling the automatic generation of dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers for CSV data. Dynamic, in this case, means that a generated MCP server could send clients `tools/list_changed` notifications, indicating that the list of available [tools](https://modelcontextprotocol.io/docs/learn/server-concepts#tools) had changed, during the course of a multimodal conversation.

## Use Cases of Interest

In addition to the [twenty-five use cases for CSVW](https://w3c.github.io/csvw/use-cases-and-requirements/#uc), motivating use cases for enhancing CSV metadata to enable AI scenarios include delivering multimodal conversational data-science and analytical-reasoning capabilities for end-users utilizing:

1. opinion-poll and survey data

2. educational data
   1. homework exercises and activities, quizzes, and exam results

## Bibliography

* [CSV on the Web: A Primer](https://w3c.github.io/csvw/primer/)
* [Model for Tabular Data and Metadata on the Web](https://w3c.github.io/csvw/syntax/)
* [Metadata Vocabulary for Tabular Data](https://w3c.github.io/csvw/metadata/)
* [Generating JSON from Tabular Data on the Web](https://w3c.github.io/csvw/csv2json/)
* [Generating RDF from Tabular Data on the Web](https://w3c.github.io/csvw/csv2rdf/)
* [Embedding Tabular Metadata in HTML](https://w3c.github.io/csvw/html-note/)
* [Use Cases and Requirements](https://w3c.github.io/csvw/use-cases-and-requirements/)
