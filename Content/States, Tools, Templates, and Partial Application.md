## States

Systems' state nodes could refer to tools which are intended to be made instantaneously available to large language models or agents. One could then union over one or more such nodes representing systems' states to gather together those tools to be made instantaneously available to an LLM or agent.

## Tools

Here is an example of a contemporary tool definition, using JSON, for MCP:

```json
{
  "name": "get_weather_data",
  "title": "Weather Data Retriever",
  "description": "Get current weather data for a location",
  "inputSchema": {
    "type": "object",
    "properties": {
      "location": {
        "type": "string",
        "description": "City name or zip code"
      }
    },
    "required": ["location"]
  },
  "outputSchema": {
    "type": "object",
    "properties": {
      "temperature": {
        "type": "number",
        "description": "Temperature in celsius"
      },
      "conditions": {
        "type": "string",
        "description": "Weather conditions description"
      },
      "humidity": {
        "type": "number",
        "description": "Humidity percentage"
      }
    },
    "required": ["temperature", "conditions", "humidity"]
  }
}
```

## Templates

Systems' states could, additionally or instead, refer to templates with which to generate invokable and described tools. Templates could be instantiated, using zero, one, or more arguments, into invokable tool descriptions.

An interface for such a template could resemble:

```cs
public interface ITemplate<out TOutput>
{
    public ParameterInfo[] GetParameters();

    public TOutput Invoke(IEnumerable<KeyValuePair<string, object?>> arguments);
}
```

Considered, here, is something referenced on state nodes like `ITemplate<ToolDescription>`, where state nodes could use locally-available data to instantiate these templates to obtain described tools for LLMs or agents.

## Partial Application

One could expand on the template interface, above, or define extension methods to enable [partial application](https://en.wikipedia.org/wiki/Partial_application) on templates to create tools.

One could also use the concept of partial application upon resultant tool descriptions to provide LLMs or agents with described tools having fewer parameters.

Some useful extension methods for `ITemplate<>`, above, then, would include:

```cs
public static partial class Extensions
{
    extension<TOutput>(ITemplate<TOutput> template)
    {
        public ITemplate<TOutput> PartialApplication(IEnumerable<KeyValuePair<string, object?>> arguments)
        {
            ...
        }

        public ITemplate<TResult> Select<TResult>(Func<TOutput, TResult> function)
        {
            ...
        }
    }
}
```
