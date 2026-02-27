# CSVAI

## Introduction

[CSVW](https://w3c.github.io/csvw/primer/) and CSVAI metadata could, together, enable end-users to be able to converse with AI assistants, in natural-language and multimodal dialogues, to perform more data science and analysis tasks.

## What is CSVW?

Validation, conversion, display, and search of tabular data on the Web requires additional metadata that describes how the data should be interpreted.

Based on the CSVW documentation and examples, say that the following [CSV](https://en.wikipedia.org/wiki/Comma-separated_values) file were available at `http://example.org/tree-ops.csv`:
```csv
GID,On Street,Species,Trim Cycle,Inventory Date
1,ADDISON AV,Celtis australis,Large Tree Routine Prune,10/18/2010
2,EMERSON ST,Liquidambar styraciflua,Large Tree Routine Prune,6/2/2010
```

and that the following metadata file were available at `http://example.org/tree-ops.csv-metadata.json`:
```json
{
  "@context": ["http://www.w3.org/ns/csvw#", {"@language": "en"}],
  "url": "tree-ops.csv",
  "dc:title": "Tree Operations",
  "dcat:keyword": ["tree", "street", "maintenance"],
  "dc:publisher": {
    "schema:name": "Example Municipality",
    "schema:url": {"@id": "http://example.org"}
  },
  "dc:license": {"@id": "http://opendefinition.org/licenses/cc-by/"},
  "dc:modified": {"@value": "2010-12-31", "@type": "xsd:date"},
  "tableSchema": {
    "columns": [{
      "name": "GID",
      "titles": ["GID", "Generic Identifier"],
      "dc:description": "An identifier for the operation on a tree.",
      "datatype": "string",
      "required": true
    }, {
      "name": "on_street",
      "titles": "On Street",
      "dc:description": "The street that the tree is on.",
      "datatype": "string"
    }, {
      "name": "species",
      "titles": "Species",
      "dc:description": "The species of the tree.",
      "datatype": "string"
    }, {
      "name": "trim_cycle",
      "titles": "Trim Cycle",
      "dc:description": "The operation performed on the tree.",
      "datatype": "string"
    }, {
      "name": "inventory_date",
      "titles": "Inventory Date",
      "dc:description": "The date of the operation that was performed.",
      "datatype": {"base": "date", "format": "M/d/yyyy"}
    }],
    "primaryKey": "GID",
    "aboutUrl": "#gid-{GID}"
  }
}
```
The JSON-LD resource, the metadata file, describes the CSV data.

## What is CSVAI?

CSVAI would be additional metadata for enabling more AI scenarios. As CSVW utilizes JSON-LD for declaring CSV metadata in an extensible manner, the `@context` property in a metadata resource could use the `@vocab` property to reference the default CSVW namespace and define a prefix for using a CSVAI namespace.

```json
{
  "@context": {
    "@vocab": "http://www.w3.org/ns/csvw#",
    "csvai": "...",
    { "@language": "en" }
  },
  "url": "data.csv"
}
```

Preliminary ideas for a CSVAI metadata schema include:

1. Descriptive metadata.
   1. In addition to [Dublin Core](https://www.dublincore.org/) metadata, a CSVAI schema could define descriptive metadata intended for consumption by AI systems.

2. Multimodal conversational user interfaces.
   1. As envisioned, table-groups, tables, and so forth, would have default functionalities, e.g., from relational algebra, from querying languages, for making selections (e.g., columns, rows), for performing operations on selections, for creating data visualizations, and for navigating through these. 

3. Custom Functionality.
   1. Custom functionalities could be declared and described using either the [MCP Tool Schema](https://modelcontextprotocol.io/specification/2025-11-25/schema#tool) or [Function Ontology](https://fno.io/). Custom functions could be mapped with semantic classes for table-groups, tables, columns, rows, and cells.
      1. Custom functions could provide one or more implementations, e.g., JavaScript.

4. Provenance.
   1. Tables produced during AI-assisted data science and analysis could use CSVAI metadata to indicate their provenance.

## Two Motivating Use Cases

In addition to the [twenty-five use cases for CSVW](https://w3c.github.io/csvw/use-cases-and-requirements/#uc), motivating use cases for CSVAI include enabling conversational AI for data science and analysis with respect to: (1) public opinion polling and survey data, and (2) educational data such as homework exercises and activities, quizzes, and exams.

These motivating use cases both involve natural-language or multimodal questions and responses provided by populations of respondents. Questions could be text strings provided in metadata and/or external resources referenced by URL in metadata (see: [HTML](https://html.spec.whatwg.org/multipage/), [OCX](https://k12ocx.github.io/k12ocx-specs/), [OER](https://www.oerschema.org/classes/Activity), and [QTI](https://www.1edtech.org/standards/qti/index)).

It is a goal for AI assistants to be able to enhance both quantitative and qualitative data analyses. One approach to achieving this goal involves enabling AI assistants to be able to use and to reason about natural-language or multimodal questions. AI-equipped data analysts could, then, more readily combine and merge information from multiple sources, e.g., from multiple opinion polls or surveys occuring at the same time or at different points in time.

## Considered Technical Scenarios

### Loading Data Into Model Context Protocol Servers and Agents

One approach for exploration involves that dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers or agents could search for, retrieve, and load CSV data providing CSVW and CSVAI metadata. "Dynamic", as used here, means that MCP servers could send clients `tools/list_changed` notifications, lists of available [tools](https://modelcontextprotocol.io/docs/learn/server-concepts#tools) could change, during the course of natural-language or multimodal conversations.

### Generating Model Context Protocol Servers and Agents

Another approach for exploration involves that dynamic [MCP](https://modelcontextprotocol.io/docs/getting-started/intro) servers or agents could be automatically generated for and from CSV data providing CSVW and CSVAI metadata.

### Web Browsers

In the not-too-distant future, Web developers may be able to utilize [WebMCP](https://github.com/webmachinelearning/webmcp/) to deliver natural-language or multimodal conversational data science and analytics functionalities to end-users using Web browsers. Data could be stored either client-side or server-side.

## Bibliography

* [CSV on the Web: A Primer](https://w3c.github.io/csvw/primer/)
* [Model for Tabular Data and Metadata on the Web](https://w3c.github.io/csvw/syntax/)
* [Metadata Vocabulary for Tabular Data](https://w3c.github.io/csvw/metadata/)
* [Generating JSON from Tabular Data on the Web](https://w3c.github.io/csvw/csv2json/)
* [Generating RDF from Tabular Data on the Web](https://w3c.github.io/csvw/csv2rdf/)
* [Embedding Tabular Metadata in HTML](https://w3c.github.io/csvw/html-note/)
* [Use Cases and Requirements](https://w3c.github.io/csvw/use-cases-and-requirements/)
