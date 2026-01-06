# Computational Narratology

## Introduction

A collection of documents presenting ideas supporting computational narratology include:

* [Automata](Content/Automata.md) &ndash; interfaces enabling general-purpose automata, acceptors, and transducers.
* [Constraints](Content/Constraints.md) &ndash; constraint-logic system with a fluent constraint-builder.
* [Knowledge](Content/Knowledge.md) &ndash; knowledgebase system, rule-building, and inference.

## Project Research Topics

This project will explore:

* incremental story comprehension, prediction, and generation with large language models.

* stateful systems and their interoperations with large language models.
  * Firstly, beyond performing symbol-matching, object-processing, or rule-processing upon automaton edges, some edges might consult LLMs, e.g., with natural-language questions about complex inputs such as dialogue content or story events.
  * Secondly, systems can be considered where automaton states map with or route to different LLMs or agents. Systems' edges or transitions could be prioritized rules for determining which LLM to invoke next. As described in [Petit, Pachot, Conan-Vrinat, and Dubarry (2024)](https://arxiv.org/abs/2409.13693), different LLMs or agents mapped with automaton states could share conversation histories and session data.
  * Thirdly, state-based approaches can simplify managing those tools instantaneously available to LLMs or agents. Those tools to be made instantaneously available could be those referenced by the one or more nodes representing applications' or systems' states.

* various approaches, pedagogical (black-box) and compositional (white-box), for extracting automata-based systems from ([ONNX](https://en.wikipedia.org/wiki/Open_Neural_Network_Exchange)) [recurrent neural networks](https://en.wikipedia.org/wiki/Recurrent_neural_network) using [ML.NET](https://github.com/dotnet/machinelearning) software libraries.
