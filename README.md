# Computational Narratology

## Complex Event Processing

How might the processing and comprehension of stories' [fabulae](https://en.wikipedia.org/wiki/Fabula_and_syuzhet) relate to other existing techniques for processing and analyzing various other kinds of chronologically-sorted sequences of events, e.g., [complex event processing](https://en.wikipedia.org/wiki/Complex_event_processing)?

## State-based Systems

Representing systems' [states](https://en.wikipedia.org/wiki/State_(computer_science)) is of fundamental importance to computer science. Possibilities, in these regards, include uses of [graph nodes](https://en.wikipedia.org/wiki/Vertex_(graph_theory)), [knowledge graphs](https://en.wikipedia.org/wiki/Knowledge_graph), and [embedding vectors](https://en.wikipedia.org/wiki/Embedding_(machine_learning)).

Considered, here, are sets of intercommunicating stateful systems, e.g., [finite-state machines](https://en.wikipedia.org/wiki/Finite-state_machine) and [automata](https://en.wikipedia.org/wiki/Automata_theory), and how such concurrent systems can be brought to bear to address the challenges of processing and comprehending stories.

Will artificial-intelligence systems processing incrementally-arriving story events represent their state progressions as simple sequences of graph nodes, knowledge graphs, or embedding vectors? Might, instead, systems' states be composite, each state containing multiple graph nodes, knowledge graphs, or embedding vectors? Regardless of whether systems' states are simple or composite, as systems' states are progressed through as a result of processing incoming story events, how might deltas or [differences](https://en.wikipedia.org/wiki/Data_differencing) between consecutive states be obtained and analyzed?

## Automata Learning and Grammar Induction

### Recurrent Neural Networks

This project will explore extracting automata from [ONNX](https://en.wikipedia.org/wiki/Open_Neural_Network_Exchange) [RNNs](https://en.wikipedia.org/wiki/Recurrent_neural_network) using [ML.NET](https://github.com/dotnet/machinelearning).

* Aichernig, Bernhard K., Sandra König, Cristinel Mateis, Andrea Pferscher, and Martin Tappler. "Learning minimal automata with recurrent neural networks." _Software and Systems Modeling_ 23, no. 3 (2024): 625-655.

* Giles, C. Lee, Clifford B. Miller, Dong Chen, Hsing-Hen Chen, Guo-Zheng Sun, and Yee-Chun Lee. "Learning and extracting finite state automata with second-order recurrent neural networks." _Neural Computation_ 4, no. 3 (1992): 393-405.

* Weiss, Gail, Yoav Goldberg, and Eran Yahav. "Extracting automata from recurrent neural networks using queries and counterexamples (extended version)." _Machine Learning_ 113, no. 5 (2024): 2877-2919.

* Zhang, Xiyue, Xiaoning Du, Xiaofei Xie, Lei Ma, Yang Liu, and Meng Sun. "Decision-guided weighted automata extraction from recurrent neural networks." In _Proceedings of the AAAI Conference on Artificial Intelligence_, vol. 35, no. 13, pp. 11699-11707. 2021.

## Automata and Large Language Models

Firstly, systems can be considered where automata states map with or route to different LLMs or agents. Edges or transitions would have prioritized rules for determining which LLM to invoke next. As described in [(Petit, Pachot, Conan-Vrinat, and Dubarry, 2024)](https://arxiv.org/abs/2409.13693), different LLMs or agents mapped with automaton states could share conversation histories or sessions.

Secondly, beyond performing symbol-matching, object-processing, or rule-processing upon automata edges, some edges might consult LLMs, e.g., with natural-language questions about complex inputs such as dialogue content or story events.

Thirdly, state-based approaches can simplify managing those tools instantaneously available to LLMs or agents. Those tools to be made instantaneously available could be those referenced by the one or more nodes representing applications' or systems' states.

## Generative and Agentic Narratology

_Coming soon._
