# The Semantic Web, Arrays, and Tensors

## Introduction

How can arrays and tensors, e.g., numerical vectors, be represented in RDF languages like Turtle and TriG?

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

```turtle
@prefix tensor: <https://w3id.org/rdf-tensor/vocab#> .
@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .
@prefix ex: <http://example.org/> .

ex2:graph {
 ex:thing ex:hasTensor
  "{\"type\": \"float32\", \"shape\": [3, 2], \"data\": [0.1, 1.2, 2.2, 3.2, 4.1, 5.4e2]}"^^tensor:NumericDataTensor ,
  "{\"type\": \"int32\", \"shape\": [1, 2, 2, 2], \"data\": [1, 3, 4, 12, 22, 32, 41, 5]}"^^tensor:NumericDataTensor .
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

* [The Semantic Web and N-ary Expressions](The%20Semantic%20Web%20and%20N-ary%20Expressions.md)
* https://awslabs.github.io/SPARQL-CDTs/spec/latest.html
* https://neverblink-oss.github.io/rdf-tensor/
