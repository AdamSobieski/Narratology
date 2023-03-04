using AI.Epistemology;
using AI.Epistemology.Constraints;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AI
{
    namespace Agents
    {
        public interface IDataDictionary : IDictionary<string, object?>
        {
            bool TrySetDefinition(string key, IEnumerable<IConstraint> constraints);
            bool TryGetDefinition(string key, out IEnumerable<IConstraint> constraints);

            bool TrySetValue(string key, object? value, IEnumerable justifications);
            bool TryGetValue(string key, out object? value, [NotNullWhen(true)] out IEnumerable? justifications);
        }

        public interface IHasProperties
        {
            public IDataDictionary Properties { get; }
        }

        public interface IHasMetadata
        {
            public IDataDictionary Metadata { get; }
        }

        public interface IAgent : IHasProperties, IHasMetadata
        {
            public IStatementCollection Beliefs { get; }
            public IEnumerable Desires { get; }
            public IEnumerable Intentions { get; }
        }
    }
}
