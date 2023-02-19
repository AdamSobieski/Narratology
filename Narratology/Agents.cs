using AI.Epistemology;
using System.Collections;

namespace AI
{
    namespace Agents
    {
        public interface IAgent : IThing
        {
            public IStatementCollection Beliefs { get; }
            public IEnumerable Desires { get; }
            public IEnumerable Intentions { get; }
        }
    }
}
