# A CSVAI Metadata Schema

## Introduction

CSVW utilizes JSON-LD for metadata in an extensible manner. Accordingly, the `@context` property could reference the `http://www.w3.org/ns/csvw#` namespace and also a CSVAI schema.

```json
{
  "@context": {
    "csvw": "http://www.w3.org/ns/csvw#",
    "csvai": "..."
  },
  "url": "data.csv",
  "tableSchema": "metadata.json"
}
```

## A CSVAI Schema

A CSVAI schema would build on CSVW metadata. Preliminary ideas for a CSVAI schema include:

1. descriptive metadata for AI systems.
   1. in addition to using [Dublin Core](https://www.dublincore.org/) metadata, CSVAI could define descriptive metadata intended for AI systems.

2. functions could be declared and described
   1. this could be achieved by providing functions (e.g., via [the Function Ontology](https://fno.io/)) mapped to semantic classes, e.g., for cells, rows, columns, tables, and table-groups.
   2. functions could be declared in a backend-independent manner while enabling the automatic generation of dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers.

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
