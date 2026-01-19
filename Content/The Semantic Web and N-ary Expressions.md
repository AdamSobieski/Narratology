# The Semantic Web and N-ary Expressions

## Introduction

How could n-ary predicate-calculus expressions be represented in formats like RDF Turtle and TriG?

Firstly, one could use a special predicate, `calculus:holdsFor`, to relate n-ary predicates to lists of arguments.
```turtle
@prefix calculus: <http://predicate-calculus.org/> .
@prefix ex: <http://example.org/> .

ex:p1 calculus:holdsFor (ex:r ex:s ex:t) ,
                        (ex:u ex:v ex:w) .
ex:p2 calculus:holdsFor (ex:x ex:y ex:z) .  
```

Secondly, one could use containing named graphs as subjects, n-ary predicates as predicates, and lists of arguments as objects.
```turtle
@prefix ex: <http://example.org/> .

ex:graph {
  ex:graph ex:p1 (ex:r ex:s ex:t) ,
                 (ex:u ex:v ex:w) ;
           ex:p2 (ex:x ex:y ex:z) .
}
```

Thirdly, one could use blank nodes as subjects, n-ary predicates as predicates, and lists of arguments as objects.
```turtle
@prefix ex: <http://example.org/> .

ex:graph {
  [] ex:p1 (ex:r ex:s ex:t) ,
           (ex:u ex:v ex:w) ;
     ex:p2 (ex:x ex:y ex:z) .
}
```

Fourthly, one could use constant values as subjects, e.g., `true` and `false`.

```turtle
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex:graph {
  "true"^^xsd:boolean ex:p1 (ex:r ex:s ex:t) ,
                            (ex:u ex:v ex:w) ;
                      ex:p2 (ex:x ex:y ex:z) .
}
```

Fifthly, one could invert the statements, flipping the subjects and objects, from approach four, above.

```turtle
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex:graph {
  (ex:r ex:s ex:t) ex:p1 "true"^^xsd:boolean .
  (ex:u ex:v ex:w) ex:p1 "true"^^xsd:boolean .
  (ex:x ex:y ex:z) ex:p2 "true"^^xsd:boolean .
}
```
