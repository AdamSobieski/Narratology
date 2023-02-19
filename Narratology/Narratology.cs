using AI.Agents;
using AI.AutomatedPlanning;
using AI.Epistemology;
using AI.Epistemology.Adaptation;
using AI.Epistemology.Constraints;
using AI.Narratology.Annotation;
using AI.Narratology.Events;
using AI.Narratology.Hermeneutics;
using AI.Narratology.Pragmatics;
using AI.Narratology.Stylistics;
using System.Collections.Trees;
using System.Diagnostics.CodeAnalysis;

// 0.0.4.64

namespace System
{
    public interface ICloneable<out T> : ICloneable
        where T : ICloneable<T>
    {
        object ICloneable.Clone() => Clone();

        public new T Clone();
    }

    public interface IHasProperties
    {
        public IDataDictionary Properties { get; }
    }

    public interface IHasMetadata
    {
        public IDataDictionary Metadata { get; }
    }

    public interface IThing : IHasProperties, IHasMetadata { }
}

namespace System.Collections.Generic
{
    public interface IContainer<in T>
    {
        public bool Contains(T element);
    }

    public interface ICountable<out T> : IEnumerable<T>
    {
        public int Count { get; }
    }

    public interface IDelta<out T>
    {
        IEnumerable<T> Additions { get; }
        IEnumerable<T> Removals { get; }
    }

    public interface ISimilarity<in T>
    {
        public double Dissimilarity(T other);
    }

    public interface ISimilarityComparer<in T>
    {
        public double Dissimilarity(T x, T y);
    }

    public interface IDataDictionary : IDictionary<string, object?>
    {
        void Define(string key, Type type);
        void Define(string key, IEnumerable<IConstraint> constraints);
        bool TryGetDefinition(string key, out IEnumerable<IConstraint> constraints);

        bool TryGetValue(string key, out object? value, [NotNullWhen(true)] out IEnumerable? justifications);
        bool TrySetValue(string key, object? value, IEnumerable justifications);
    }

    public interface ITrie<T>
    {
        public interface INode
        {
            public IDictionary<T, INode> Children { get; }
            public bool IsWord { get; internal set; }
        }

        protected INode Root { get; }

        protected INode CreateNode();

        public void Add(IEnumerable<T> source)
        {
            INode current = Root;
            foreach (T element in source)
            {
                if (!current.Children.ContainsKey(element))
                {
                    INode tmp = CreateNode();
                    current.Children.Add(element, tmp);
                }
                current = current.Children[element];
            }
            current.IsWord = true;
        }
        public bool Contains(IEnumerable<T> source)
        {
            INode current = Root;
            foreach (T element in source)
            {
                if (current.Children.ContainsKey(element))
                {
                    current = current.Children[element];
                }
                else
                {
                    return false;
                }
            }
            return current.IsWord;
        }
        public void Remove(IEnumerable<T> source)
        {
            Remove(Root, source, 0);
        }
        private bool Remove(INode current, IEnumerable<T> word, int depth)
        {
            if (depth == word.Count())
            {
                current.IsWord = false;
            }
            else
            {
                T child = word.ElementAt(depth);
                if (current.Children.ContainsKey(child))
                {
                    if (Remove(current.Children[child], word, depth + 1) == false)
                    {
                        current.Children.Remove(child);
                    }
                }
            }
            if (current.Children.Count > 0)
            {
                return true;
            }
            return false;
        }
        public IEnumerable<IEnumerable<T>> StartsWith(IEnumerable<T> source)
        {
            List<IEnumerable<T>> res = new();

            INode current = Root;

            foreach (T child in source)
            {
                if (current.Children.ContainsKey(child))
                {
                    current = current.Children[child];
                }
                else
                {
                    return res;
                }
            }
            StartsWith(current, source, res);
            return res;
        }
        private void StartsWith(INode current, IEnumerable<T> source, List<IEnumerable<T>> words)
        {
            if (current.IsWord)
            {
                words.Add(source);
            }
            foreach (T key in current.Children.Keys)
            {
                StartsWith(current.Children[key], source.Append(key), words);
            }
        }
        public IEnumerable<IEnumerable<T>> StartsWith2(IEnumerable<T> source)
        {
            INode current = Root;

            foreach (T child in source)
            {
                if (current.Children.ContainsKey(child))
                {
                    current = current.Children[child];
                }
                else
                {
                    yield break;
                }
            }
            foreach (var word in StartsWith2(current, source))
            {
                yield return word;
            }
        }
        private IEnumerable<IEnumerable<T>> StartsWith2(INode current, IEnumerable<T> source)
        {
            if (current.IsWord)
            {
                yield return source;
            }
            foreach (T key in current.Children.Keys)
            {
                foreach (var word in StartsWith2(current.Children[key], source.Append(key)))
                {
                    yield return word;
                }
            }
        }
    }
}

namespace AI
{
    namespace Narratology
    {
        public interface INarrative : IThing
        {
            public IFabula Fabula { get; }
            public IEnumerable<ISyuzhet> Syuzhets { get; }
        }

        public interface IFabula : IThing
        {
            public IEventSet Events { get; }
        }

        public interface ISyuzhet : IThing
        {
            public IEventSequence Events { get; }

            public IEnumerable<INarration> Narrations { get; }
        }

        public interface INarration : IThing
        {
            public IEvent Event { get; }

            public IPlan Plan { get; }

            public IEnumerable<IInterpretation> IntendedInterpretations { get; }

            public IEnumerable<IText> Realizations { get; }
        }

        public interface IText : IThing
        {
            public ISegment CreateSegment(ISelection selection);
        }

        public interface INarrator : IAgent
        {
            public IAsyncEnumerable<ISyuzhet> Create(IFabula fabula, IStyle style);
            public IAsyncEnumerable<INarration> Create(ISyuzhet syuzhet, IStyle style);
            public IAsyncEnumerable<IText> Create(INarration narration, IStyle style);
        }

        public interface INarratee : IAgent
        {
            public IAsyncEnumerable<IInterpretation> Interpret(IText text);
        }
    }

    namespace Narratology.Aesthetics
    {
        public interface ICriterion : IEquatable<ICriterion> { }

        public interface IMultipleCriteria : IReadOnlyDictionary<ICriterion, IComparable> { }
    }

    namespace Narratology.Aesthetics.Morality
    {

    }

    namespace Narratology.Annotation
    {
        public interface ISegment : IThing
        {
            public IText Text { get; }

            public ISelection Selection { get; }
        }

        public interface ISelection : ITreeParented<ISelection>
        {
            public IText Text { get; }

            public int CompareStartToStart(ISelection other);
            public int CompareStartToEnd(ISelection other);
            public int CompareEndToStart(ISelection other);
            public int CompareEndToEnd(ISelection other);
        }
    }

    namespace Narratology.Causality
    {
        public interface ICausalReasoner : IThing
        {
            public bool? Caused(IEnumerable<IEvent> x, IEvent y);
            public bool? Caused(IEnumerable<IEvent> x, IEnumerable<IEvent> y)
            {
                foreach (var e in y)
                {
                    bool? iterand = Caused(x, e);
                    if (!iterand.HasValue)
                    {
                        return null;
                    }
                    else if (!iterand.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public interface IPlot : IThing
        {
            public IEventSequence Events { get; }
        }
    }

    namespace Narratology.Characters
    {
        public interface ICharacter : IAgent { }

        public interface ITrait { }

        public interface IModel
        {
            public double Probability(IEvent behavior, (IState State, double Weight) context);
        }

        public interface ITraitBasedModel : IModel
        {
            public IAlternatives<ITrait> Traits((IState State, double Weight) context);
        }
    }

    namespace Narratology.Coherence
    {

    }

    namespace Narratology.Completion
    {
        public interface IPrediction
        {
            public IAlternatives<IEnumerable<IEvent>> Predict(IEnumerable<IEvent> events /*, (IScope Scope, double Weight) context */);
        }

        public interface IInfilling
        {
            public IAlternatives<IEnumerable<IEvent>> Infill(IEnumerable<IEvent> before, IEnumerable<IEvent> after /*, (IScope Scope, double Weight) context */);
        }

        public interface IScript : IPrediction, IInfilling
        {
            // https://en.wikipedia.org/wiki/Behavioral_script
            // https://en.wikipedia.org/wiki/Schema_(psychology)

            public IAlternatives<bool?> Contains(IEnumerable<IEvent> events);
        }

        public interface IScriptCollection : ILookup<IEnumerable<IEvent>, IScript, IState>
        {
            public IAlternatives<IEnumerable<IEvent>> Combine(IEnumerable<IAlternatives<IEnumerable<IEvent>>> alternatives);
        }
    }

    namespace Narratology.Drama
    {
        public interface IConflict
        {
            // https://en.wikipedia.org/wiki/Conflict_(process)
        }
    }

    namespace Narratology.Events
    {
        public interface IEvent : IThing
        {
            // https://plato.stanford.edu/entries/events/
            // https://en.wikipedia.org/wiki/Thematic_relation

            public int? CompareStartToStart(IEvent other);
            public int? CompareStartToEnd(IEvent other);
            public int? CompareEndToStart(IEvent other);
            public int? CompareEndToEnd(IEvent other);
        }

        public interface IEventSequence : IReadOnlyList<IEvent>, IContainer<IEvent>, IEquatable<IEventSequence> { }

        public interface IEventSet : IContainer<IEvent>, ICountable<IEvent>, IEquatable<IEventSet>
        {
            IEventSet ExceptWith(IEventSet other);
            IEventSet IntersectWith(IEventSet other);
            bool IsProperSubsetOf(IEventSet other);
            bool IsProperSupersetOf(IEventSet other);
            bool IsSubsetOf(IEventSet other);
            bool IsSupersetOf(IEventSet other);
            bool Overlaps(IEventSet other);
            bool SetEquals(IEventSet other);
            IEventSet SymmetricExceptWith(IEventSet other);
            IEventSet UnionWith(IEventSet other);
        }
    }

    namespace Narratology.Hermeneutics
    {
        public interface IInterpretation : IThing
        {
            public IEvent Event { get; }

            public IText Text { get; }

            public IContent Content { get; }
        }

        public interface IContent : IReadOnlyDictionary<IEventSequence, IThing>
        {
            public IThing this[params IEvent[] events]
            {
                get;
            }

            public IEnumerable<IEventSequence> KeysStartingWith(IEvent @event)
            {
                return Keys.Where(k => k.Count > 0 && @event.Equals(k[0]));
            }
            public IEnumerable<IEventSequence> KeysEndingWith(IEvent @event)
            {
                return Keys.Where(k => k.Count > 0 && @event.Equals(k[k.Count - 1]));
            }
            public IEnumerable<IEventSequence> KeysContaining(IEvent @event)
            {
                return Keys.Where(k => k.Contains(@event));
            }
        }
    }

    namespace Narratology.Hermeneutics.Semiotics
    {

    }

    namespace Narratology.Hermeneutics.Thematics
    {

    }

    namespace Narratology.Hermeneutics.Thematics.Allegory
    {

    }

    namespace Narratology.Pragmatics
    {
        public interface IState : IInvariant
        {
            public IStatementCollection Content { get; }

            public bool TryGetNext(IDelta<Statement> delta, out IState? state)
            {
                return TryGetNext(delta.Removals, delta.Additions, out state);
            }
            public bool TryGetNext(IEnumerable<Statement> removals, IEnumerable<Statement> additions, out IState? state);
        }
    }

    namespace Narratology.Stylistics
    {
        public interface IStyle : IThing
        {
            public IStatementCollection Content { get; }

            public object? GetTechnique(Type techniqueType);
        }
    }
}