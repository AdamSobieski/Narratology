namespace AI
{
    namespace DecisionMaking.MultipleCriteria
    {
        public interface ICriterion : IEquatable<ICriterion> { }

        public interface IMultipleCriteria : IReadOnlyDictionary<ICriterion, IComparable> { }
    }
}
