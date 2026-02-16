# The Semantic Web, Arrays, and Vectors

## Introduction

How can arrays and vectors be represented in languages like RDF Turtle and TriG?

## Using Literals

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

### String Interpolation

While there exist many syntax possibilities with respect to [string interpolation](https://en.wikipedia.org/wiki/String_interpolation), one is shown below.

```turtle
@prefix cdt: <http://w3id.org/awslabs/neptune/SPARQL-CDTs/> .
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex:graph {
  "true"^^xsd:boolean ex:p1 $"[{ex:r}, {ex:s}, {ex:t}]"^^cdt:List ,
                            $"[{ex:u}, {ex:v}, {ex:w}]"^^cdt:List ;
                      ex:p2 $"[{ex:x}, {ex:y}, {ex:z}]"^^cdt:List .
}
```

## Typed Collections

```turtle
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex:graph {
  ex:thing ex:hasArray (ex:r ex:s ex:t)^^rdf:Array ,
                       (ex:r ex:s ex:t)^^rdf:Array ,
                       (ex:r ex:s ex:t)^^rdf:Array .
}
```

```turtle
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex:graph {
  ex:thing ex:hasVector ("1.0"^^xsd:float "2.0"^^xsd:float "3.0"^^xsd:float)^^rdf:Vector ,
                        ("4.0"^^xsd:float "5.0"^^xsd:float "6.0"^^xsd:float)^^rdf:Vector ,
                        ("7.0"^^xsd:float "8.0"^^xsd:float "9.0"^^xsd:float)^^rdf:Vector .
}
```

## See Also

* https://awslabs.github.io/SPARQL-CDTs/spec/latest.html
