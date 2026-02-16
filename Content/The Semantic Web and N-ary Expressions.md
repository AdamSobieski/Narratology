# The Semantic Web and N-ary Expressions

## Introduction

How can n-ary propositional statements be represented in languages like RDF Turtle and TriG?

## Using Lists

One could use a special predicate, `calculus:holdsFor`, to relate n-ary predicates to lists of arguments.
```turtle
@prefix calculus: <http://predicate-calculus.org/> .
@prefix ex: <http://example.org/> .

ex:graph {
  ex:p1 calculus:holdsFor (ex:r ex:s ex:t) ,
                          (ex:u ex:v ex:w) .
  ex:p2 calculus:holdsFor (ex:x ex:y ex:z) .
}
```

One could use containing named graphs as subjects, n-ary predicates as predicates, and lists of arguments as objects.
```turtle
@prefix ex: <http://example.org/> .

ex:graph {
  ex:graph ex:p1 (ex:r ex:s ex:t) ,
                 (ex:u ex:v ex:w) ;
           ex:p2 (ex:x ex:y ex:z) .
}
```

One could use blank nodes as subjects, n-ary predicates as predicates, and lists of arguments as objects.
```turtle
@prefix ex: <http://example.org/> .

ex:graph {
  [] ex:p1 (ex:r ex:s ex:t) ,
           (ex:u ex:v ex:w) ;
     ex:p2 (ex:x ex:y ex:z) .
}
```

One could use constant values, e.g. `true` and `false`, as literal objects.

```turtle
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex:graph {
  (ex:r ex:s ex:t) ex:p1 "true"^^xsd:boolean .
  (ex:u ex:v ex:w) ex:p1 "true"^^xsd:boolean .
  (ex:x ex:y ex:z) ex:p2 "true"^^xsd:boolean .
}
```

One could use constant values, e.g., `true` and `false`, as literal subjects.

```turtle
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex:graph {
  "true"^^xsd:boolean ex:p1 (ex:r ex:s ex:t) ,
                            (ex:u ex:v ex:w) ;
                      ex:p2 (ex:x ex:y ex:z) .
}
```

## Using Literals

One could use literals to encode arrays or lists of arguments.

```turtle
@prefix cdt: <http://w3id.org/awslabs/neptune/SPARQL-CDTs/> .
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex:graph {
  "true"^^xsd:boolean ex:p1 "[<http://example.org/r>, <http://example.org/s>, <http://example.org/t>]"^^cdt:List ,
                            "[<http://example.org/u>, <http://example.org/v>, <http://example.org/w>]"^^cdt:List ;
                      ex:p2 "[<http://example.org/x>, <http://example.org/y>, <http://example.org/z>]"^^cdt:List .
}
```

## See Also

* https://www.w3.org/TR/swbp-n-aryRelations/
* https://awslabs.github.io/SPARQL-CDTs/spec/latest.html
