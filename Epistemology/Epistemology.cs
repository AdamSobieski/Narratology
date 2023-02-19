using AI.Epistemology.Constraints;
using AI.Epistemology.Reasoning;
using System.Collections;
using System.Collections.Graphs;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace System
{
    public interface IHasProperties
    {
        public IDataDictionary Properties { get; }
    }

    public interface IHasMetadata
    {
        public IDataDictionary Metadata { get; }
    }

    public interface IThing : IHasProperties, IHasMetadata { }

    public interface IDataDictionary : IDictionary<string, object?>
    {
        void Define(string key, Type type);
        void Define(string key, IEnumerable<IConstraint> constraints);
        bool TryGetDefinition(string key, out IEnumerable<IConstraint> constraints);

        bool TryGetValue(string key, out object? value, [NotNullWhen(true)] out IEnumerable? justifications);
        bool TrySetValue(string key, object? value, IEnumerable justifications);
    }
}

namespace System.Collections.Generic
{
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
    namespace Epistemology
    {
        internal interface ITerm
        {
            public bool Unify(object? other, IDictionary<Variable, object?> substitutions);
            public bool Contains(Variable variable, IDictionary<Variable, object?> substitutions);
        }

        public sealed class Variable : ITerm
        {
            public Variable()
            {
                Constraints = Array.Empty<IConstraint<object?>>();
            }
            public Variable(string name)
            {
                Constraints = Array.Empty<IConstraint<object?>>();
                m_name = name;
            }
            public Variable(IEnumerable<IConstraint<object?>> constraints)
            {
                Constraints = constraints.ToArray();
            }
            public Variable(IEnumerable<IConstraint<object?>> constraints, string name)
            {
                Constraints = constraints.ToArray();
                m_name = name;
            }

            public IReadOnlyList<IConstraint<object?>> Constraints { get; }
            private readonly string? m_name;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool ITerm.Unify(object? other, IDictionary<Variable, object?> substitutions)
            {
                if (!Constraints.All(constraint => constraint.Invoke(other))) return false;

                if (other is ITerm term)
                {
                    if (object.ReferenceEquals(this, term))
                    {
                        return true;
                    }

                    if (substitutions.TryGetValue(this, out object? value))
                    {
                        if (value is ITerm otherTerm)
                        {
                            return otherTerm.Unify(term, substitutions);
                        }
                    }

                    if (term.Contains(this, substitutions))
                    {
                        return false;
                    }

                    substitutions.Add(this, term);
                    return true;
                }
                else
                {
                    if (substitutions.TryGetValue(this, out object? value))
                    {
                        if (value is ITerm otherTerm)
                        {
                            return otherTerm.Unify(other, substitutions);
                        }
                        else
                        {
                            return object.Equals(other, value);
                        }
                    }
                    else
                    {
                        substitutions.Add(this, other);
                        return true;
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool ITerm.Contains(Variable variable, IDictionary<Variable, object?> substitutions)
            {
                if (object.ReferenceEquals(this, variable)) return true;

                if (substitutions.TryGetValue(this, out object? value))
                {
                    if (value is ITerm otherTerm)
                    {
                        return otherTerm.Contains(variable, substitutions);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            public Statement Invoke(params object?[] args)
            {
                return new Statement(this, args);
            }

            public override string? ToString()
            {
                return m_name ?? base.ToString();
            }
        }

        public sealed class Symbol : ITerm
        {
            public Symbol(string fullname)
            {
                FullName = fullname;
                Arity = 0;
                Constraints = Array.Empty<IConstraint>();
            }
            public Symbol(string fullname, int arity)
            {
                if (arity < 0) throw new ArgumentException("Arity must be greater than or equal to zero.", nameof(arity));

                FullName = fullname;
                Arity = arity;
                Constraints = Array.Empty<IConstraint>();
            }
            public Symbol(string fullname, int arity, Type[] types)
            {
                if (arity < 0) throw new ArgumentException("Arity must be greater than or equal to zero.", nameof(arity));
                if (arity != types.Length) throw new ArgumentException("Number of types provided must equal arity.", nameof(types));

                FullName = fullname;
                Arity = arity;
                Constraints = Constraint.GenerateTypeOrVariableConstraints(types).ToArray();
            }
            public Symbol(string fullname, int arity, IEnumerable<IConstraint> constraints)
            {
                if (arity < 0) throw new ArgumentException("Arity must be greater than or equal to zero.", nameof(arity));

                FullName = fullname;
                Arity = arity;
                Constraints = constraints.ToArray();
            }

            public string? Namespace
            {
                get
                {
                    var dot = FullName.LastIndexOf('.');
                    if (dot > 0)
                    {
                        return FullName.Substring(0, dot);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public string Name
            {
                get
                {
                    var dot = FullName.LastIndexOf('.');
                    if (dot > 0)
                    {
                        return FullName.Substring(dot + 1);
                    }
                    else
                    {
                        return FullName;
                    }
                }
            }
            public string FullName { get; }

            public int Arity { get; }

            public IReadOnlyList<IConstraint> Constraints { get; }

            public bool CanInvoke(object?[]? args, [NotNullWhen(false)] out Exception? reason)
            {
                try
                {
                    if (args == null) throw new ArgumentNullException(nameof(args));
                    if (Arity == 0) throw new ArgumentException("This symbol has zero arity.");
                    if (args.Length != Arity) throw new ArgumentException($"Expected {Arity} arguments.", nameof(args));

                    foreach (var constraint in Constraints)
                    {
                        if (!constraint.Invoke(args))
                        {
                            reason = new ConstraintNotSatisfiedException(constraint);
                            return false;
                        }
                    }
                    reason = null;
                    return true;
                }
                catch (Exception error)
                {
                    reason = error;
                    return false;
                }
            }
            public Statement Invoke(params object?[]? args)
            {
                if (CanInvoke(args, out Exception? reason))
                {
                    return new Statement(this, args ?? Array.Empty<object>());
                }
                else
                {
                    throw reason;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool ITerm.Contains(Variable variable, IDictionary<Variable, object?> substitutions)
            {
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool ITerm.Unify(object? other, IDictionary<Variable, object?> substitutions)
            {
                if (other is Variable v)
                {
                    return ((ITerm)v).Unify(this, substitutions);
                }
                else
                {
                    return object.Equals(this, other);
                }
            }
        }

        public sealed class Statement : ITerm
        {
            static readonly Symbol s_and = new("AI.Epistemology.And", 2);
            static readonly Symbol s_or = new("AI.Epistemology.Or", 2);
            static readonly Symbol s_not = new("AI.Epistemology.Not", 1);

            public static Statement And(object? x, object? y)
            {
                return s_and.Invoke(x, y);
            }
            public static Statement Or(object? x, object? y)
            {
                return s_or.Invoke(x, y);
            }
            public static Statement Not(object? x)
            {
                return s_not.Invoke(x);
            }

            internal Statement(Variable variable, object?[] args)
            {
                Predicate = variable;
                Arguments = args;
            }
            internal Statement(Symbol symbol, object?[] args)
            {
                Predicate = symbol;
                Arguments = args;
            }

            public object Predicate { get; }
            public IReadOnlyList<object?> Arguments { get; }

            public bool IsGround
            {
                get
                {
                    if (Predicate is Variable) return false;
                    int count = Arguments.Count;
                    for (int index = 0; index < count; ++index)
                    {
                        var a = Arguments[index];

                        if (a is Statement s)
                        {
                            if (!s.IsGround) return false;
                        }
                        else if (a is Variable)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Matches(Statement other)
            {
                return ((ITerm)this).Unify(other, new Dictionary<Variable, object?>(0));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Matches(Statement other, IDictionary<Variable, object?> substitutions)
            {
                return ((ITerm)this).Unify(other, substitutions);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool ITerm.Contains(Variable variable, IDictionary<Variable, object?> substitutions)
            {
                if (object.ReferenceEquals(Predicate, variable)) return true;
                int count = Arguments.Count;
                for (int index = 0; index < count; ++index)
                {
                    if (Arguments[index] is ITerm term)
                    {
                        if (term.Contains(variable, substitutions)) return true;
                    }
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool ITerm.Unify(object? other, IDictionary<Variable, object?> substitutions)
            {
                if (other == null) return false;

                if (other is Variable v)
                {
                    return ((ITerm)v).Unify(this, substitutions);
                }
                else if (other is Statement s)
                {
                    int count = this.Arguments.Count;
                    if (count != s.Arguments.Count) return false;

                    if (!((ITerm)Predicate).Unify(s.Predicate, substitutions)) return false;
                    for (int index = 0; index < count; ++index)
                    {
                        var a = this.Arguments[index];
                        var o = s.Arguments[index];

                        if (a is ITerm term)
                        {
                            if (!term.Unify(o, substitutions)) return false;
                        }
                        else if (o is ITerm otherTerm)
                        {
                            if (!otherTerm.Unify(a, substitutions)) return false;
                        }
                        else
                        {
                            if (!object.Equals(a, o)) return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public override string ToString()
            {
                string? name;

                if (Predicate is Symbol symbol)
                {
                    name = symbol.Name;
                }
                else if (Predicate is Variable variable)
                {
                    name = variable.ToString();
                }
                else
                {
                    throw new Exception();
                }


                return name + "(" + string.Join(", ", Arguments.Select(a => a?.ToString())) + ")";
            }
        }

        public interface IStatementCollection : IQueryable<Statement>, IInvariant, ICloneable<IStatementCollection>
        {
            public interface IQueryResult : IReadOnlyList<Statement>
            {
                object? this[Variable key] { get; }

                IEnumerable<Variable> Keys { get; }
                IEnumerable<object?> Values { get; }

                bool ContainsKey(Variable key);
                bool TryGetValue(Variable key, [MaybeNullWhen(false)] out object? value);
            }

            public IEdge<IStatementCollection, IStatementCollection, IReasoner>? Binding { get; }

            public bool Contains(Statement statement);
            public bool Contains(Statement statement, out IEnumerable? derivations);

            // to do: optimize
            public virtual IEnumerable<IQueryResult> Query(IReadOnlyList<Statement> query, bool clone = true)
            {
                int count = query.Count;

                var structure = new EnumerationStructure(count);

                if (count == 1)
                {
                    foreach (var s0 in this)
                    {
                        structure = structure.Set(0, s0);
                        if (structure.Matches(query, 0))
                        {
                            yield return clone ? structure.Clone() : structure;
                        }
                        structure = structure.Clear();
                    }
                }
                else if (count == 2)
                {
                    foreach (var s0 in this)
                    {
                        structure = structure.Set(0, s0);
                        var tmp0 = structure.Clone();

                        if (structure.Matches(query, 0))
                        {
                            foreach (var s1 in this)
                            {
                                structure = structure.Set(1, s1);
                                var tmp1 = structure.Clone();

                                if (structure.Matches(query, 1))
                                {
                                    yield return clone ? structure.Clone() : structure;
                                }

                                structure = tmp1;
                            }
                        }

                        structure = tmp0;
                    }
                }
                else if (count == 3)
                {
                    foreach (var s0 in this)
                    {
                        structure = structure.Set(0, s0);
                        var tmp0 = structure.Clone();

                        if (structure.Matches(query, 0))
                        {
                            foreach (var s1 in this)
                            {
                                structure = structure.Set(1, s1);
                                var tmp1 = structure.Clone();

                                if (structure.Matches(query, 1))
                                {
                                    foreach (var s2 in this)
                                    {
                                        structure = structure.Set(2, s2);
                                        var tmp2 = structure.Clone();

                                        if (structure.Matches(query, 2))
                                        {
                                            yield return clone ? structure.Clone() : structure;
                                        }

                                        structure = tmp2;
                                    }
                                }

                                structure = tmp1;
                            }
                        }

                        structure = tmp0;
                    }
                }
                else if (count == 4)
                {
                    foreach (var s0 in this)
                    {
                        structure = structure.Set(0, s0);
                        var tmp0 = structure.Clone();

                        if (structure.Matches(query, 0))
                        {
                            foreach (var s1 in this)
                            {
                                structure = structure.Set(1, s1);
                                var tmp1 = structure.Clone();

                                if (structure.Matches(query, 1))
                                {
                                    foreach (var s2 in this)
                                    {
                                        structure = structure.Set(2, s2);
                                        var tmp2 = structure.Clone();

                                        if (structure.Matches(query, 2))
                                        {
                                            foreach (var s3 in this)
                                            {
                                                structure = structure.Set(3, s3);
                                                var tmp3 = structure.Clone();

                                                if (structure.Matches(query, 3))
                                                {
                                                    yield return clone ? structure.Clone() : structure;
                                                }

                                                structure = tmp3;
                                            }
                                        }

                                        structure = tmp2;
                                    }
                                }

                                structure = tmp1;
                            }
                        }

                        structure = tmp0;
                    }
                }
                else if (count == 5)
                {
                    foreach (var s0 in this)
                    {
                        structure = structure.Set(0, s0);
                        var tmp0 = structure.Clone();

                        if (structure.Matches(query, 0))
                        {
                            foreach (var s1 in this)
                            {
                                structure = structure.Set(1, s1);
                                var tmp1 = structure.Clone();

                                if (structure.Matches(query, 1))
                                {
                                    foreach (var s2 in this)
                                    {
                                        structure = structure.Set(2, s2);
                                        var tmp2 = structure.Clone();

                                        if (structure.Matches(query, 2))
                                        {
                                            foreach (var s3 in this)
                                            {
                                                structure = structure.Set(3, s3);
                                                var tmp3 = structure.Clone();

                                                if (structure.Matches(query, 3))
                                                {
                                                    foreach (var s4 in this)
                                                    {
                                                        structure = structure.Set(4, s4);
                                                        var tmp4 = structure.Clone();

                                                        if (structure.Matches(query, 4))
                                                        {
                                                            yield return clone ? structure.Clone() : structure;
                                                        }

                                                        structure = tmp4;
                                                    }
                                                }

                                                structure = tmp3;
                                            }
                                        }

                                        structure = tmp2;
                                    }
                                }

                                structure = tmp1;
                            }
                        }

                        structure = tmp0;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsReadOnly { get; }

            public void Update(IDelta<Statement> delta)
            {
                Update(delta.Removals, delta.Additions);
            }
            public void Update(IEnumerable<Statement> removals, IEnumerable<Statement> additions);
        }

        internal sealed class EnumerationStructure : IStatementCollection.IQueryResult
        {
            public EnumerationStructure(int count)
            {
                m_statements = new Statement[count];
                m_substitutions = new Dictionary<Variable, object?>();
            }
            public EnumerationStructure(Statement[] statements, IDictionary<Variable, object?> substitutions)
            {
                m_statements = statements;
                m_substitutions = substitutions;
            }

            private readonly Statement[] m_statements;
            private readonly IDictionary<Variable, object?> m_substitutions;

            public IEnumerable<Variable> Keys => m_substitutions?.Keys ?? Enumerable.Empty<Variable>();

            public IEnumerable<object?> Values => m_substitutions?.Values ?? Enumerable.Empty<object?>();

            public int Count => m_statements.Length;

            public Statement this[int index] => m_statements[index];

            public object? this[Variable key] => m_substitutions?[key] ?? throw new KeyNotFoundException();

            public bool ContainsKey(Variable key)
            {
                return m_substitutions.ContainsKey(key);
            }

            public bool TryGetValue(Variable key, [MaybeNullWhen(false)] out object? value)
            {
                return m_substitutions.TryGetValue(key, out value);
            }

            public IEnumerator<Statement> GetEnumerator()
            {
                return m_statements.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return m_statements.GetEnumerator();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Matches(IReadOnlyList<Statement> query)
            {
                int count = query.Count;
                if (count != m_statements.Length) throw new ArgumentException();

                for (int index = 0; index < count; ++index)
                {
                    if (!query[index].Matches(m_statements[index], m_substitutions)) return false;
                }
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Matches(IReadOnlyList<Statement> query, int index)
            {
                return query[index].Matches(m_statements[index], m_substitutions);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public EnumerationStructure Set(int index, Statement _x)
            {
                m_statements[index] = _x;
                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public EnumerationStructure Clear()
            {
                m_substitutions.Clear();
                return this;
            }

            public EnumerationStructure Clone()
            {
                if (m_substitutions.Count == 0)
                {
                    return new EnumerationStructure((Statement[])m_statements.Clone(), new Dictionary<Variable, object?>());
                }
                else
                {
                    return new EnumerationStructure((Statement[])m_statements.Clone(), new Dictionary<Variable, object?>(m_substitutions));
                }
            }
        }
    }

    namespace Epistemology.Adaptation
    {
        public interface IAlternatives<T> : IEnumerable<(T Value, double Weight)>
        {
            public bool IsReadOnly { get; }

            public void Add((T Value, double Weight) alternative);
            public void Remove((T Value, double Weight) alternative);

            public bool SupportsFeedback { get; }

            public void Increase(T value, double intensity = 0.5d);
            public void Decrease(T value, double intensity = 0.5d);

            public void Select(T value, double intensity = 0.5d);
            public void Select(IEnumerable<T> values, double intensity = 0.5d);
        }

        public interface ILookup<TKey, TValue, TState>
        {
            public IAlternatives<TValue> Lookup((TKey Key, double Weight) input, (TState State, double Weight) context);

            public IAlternatives<TValue> Combine(IEnumerable<IAlternatives<TValue>> alternatives);
        }
    }

    namespace Epistemology.Argumentation
    {

    }

    namespace Epistemology.Argumentation.Proof
    {

    }

    namespace Epistemology.Constraints
    {
        public sealed class Constraint : IConstraint
        {
            public static IEnumerable<IConstraint> GenerateTypeConstraints(Type[] types)
            {
                int length = types.Length;
                List<ParameterExpression> parameters = new();
                List<Expression> typeisexprs = new();
                for (int index = 0; index < length; ++index)
                {
                    var p = System.Linq.Expressions.Expression.Parameter(typeof(object));
                    parameters.Add(p);
                    typeisexprs.Add(System.Linq.Expressions.Expression.TypeIs(p, types[index]));
                }
                for (int index = 0; index < length; ++index)
                {
                    yield return new Constraint(System.Linq.Expressions.Expression.Lambda(typeisexprs[index], parameters), types[index].FullName ?? string.Empty);
                }
            }
            public static IEnumerable<IConstraint> GenerateTypeOrVariableConstraints(Type[] types)
            {
                int length = types.Length;
                List<ParameterExpression> parameters = new();
                List<Expression> typeisexprs = new();

                for (int index = 0; index < length; ++index)
                {
                    var p = System.Linq.Expressions.Expression.Parameter(typeof(object));
                    parameters.Add(p);
                    typeisexprs.Add(
                        System.Linq.Expressions.Expression.OrElse(
                            System.Linq.Expressions.Expression.TypeIs(p, types[index]),
                            System.Linq.Expressions.Expression.TypeIs(p, typeof(Variable))
                            )
                        );
                }
                for (int index = 0; index < length; ++index)
                {
                    yield return new Constraint(System.Linq.Expressions.Expression.Lambda(typeisexprs[index], parameters), types[index].FullName ?? string.Empty);
                }
            }
            public static IEnumerable<IConstraint<T>> GenerateTypeOrVariableConstraints<T>(Type[] types)
            {
                int length = types.Length;
                List<ParameterExpression> parameters = new();
                List<Expression> typeisexprs = new();

                for (int index = 0; index < length; ++index)
                {
                    var p = System.Linq.Expressions.Expression.Parameter(typeof(object));
                    parameters.Add(p);
                    typeisexprs.Add(
                        System.Linq.Expressions.Expression.OrElse(
                            System.Linq.Expressions.Expression.TypeIs(p, types[index]),
                            System.Linq.Expressions.Expression.TypeIs(p, typeof(Variable))
                            )
                        );
                }
                for (int index = 0; index < length; ++index)
                {
                    yield return new Constraint<T>(System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(typeisexprs[index], parameters), types[index].FullName ?? string.Empty);
                }
            }


            public Constraint(LambdaExpression lambda, string name)
            {
                Expression = lambda;
                m_delegate = null;
                m_name = name;
            }

            public LambdaExpression Expression { get; }
            private Delegate? m_delegate;
            private readonly string m_name;

            public string Name => m_name;

            public bool Invoke(object?[]? args)
            {
                if (args == null) throw new ArgumentNullException(nameof(args));
                m_delegate ??= Expression.Compile();
                return (bool)(m_delegate.DynamicInvoke(args) ?? false);
            }

            object? IInspectableDelegate.Invoke(object?[]? args)
            {
                return Invoke(args);
            }
        }

        public sealed class Constraint<T> : IConstraint<T>
        {
            public Constraint(Expression<Func<T, bool>> lambda, string name)
            {
                Expression = lambda;
                m_delegate = null;
                m_name = name;
            }

            public Expression<Func<T, bool>> Expression { get; }
            private Delegate? m_delegate;
            private readonly string m_name;

            public string Name => m_name;

            LambdaExpression IInspectableDelegate.Expression => Expression;

            public bool Invoke(object?[]? args)
            {
                if (args == null) throw new ArgumentNullException(nameof(args));
                m_delegate ??= Expression.Compile();
                return (bool)(m_delegate.DynamicInvoke(args) ?? false);
            }

            object? IInspectableDelegate.Invoke(object?[]? args)
            {
                return Invoke(args);
            }

            public bool Invoke(T arg)
            {
                m_delegate ??= Expression.Compile();
                return ((Func<T, bool>)m_delegate!)(arg);
            }
        }

        public interface IConstraint : IInspectableDelegate
        {
            public string? Name { get; }

            public new bool Invoke(object?[]? args);
        }

        public interface IConstraint<T> : IConstraint, IInspectableFunc<T, bool> { }

        public interface IConstraint<T1, T2> : IConstraint, IInspectableFunc<T1, T2, bool> { }

        public interface IConstraint<T1, T2, T3> : IConstraint, IInspectableFunc<T1, T2, T3, bool> { }

        public interface IConstraint<T1, T2, T3, T4> : IConstraint, IInspectableFunc<T1, T2, T3, T4, bool> { }

        public interface IConstraint<T1, T2, T3, T4, T5> : IConstraint, IInspectableFunc<T1, T2, T3, T4, T5, bool> { }

        public interface IConstraint<T1, T2, T3, T4, T5, T6> : IConstraint, IInspectableFunc<T1, T2, T3, T4, T5, T6, bool> { }

        public interface IConstraint<T1, T2, T3, T4, T5, T6, T7> : IConstraint, IInspectableFunc<T1, T2, T3, T4, T5, T6, T7, bool> { }

        public interface IConstraint<T1, T2, T3, T4, T5, T6, T7, T8> : IConstraint, IInspectableFunc<T1, T2, T3, T4, T5, T6, T7, T8, bool> { }

        public sealed class ConstraintNotSatisfiedException : Exception
        {
            public ConstraintNotSatisfiedException(IConstraint constraint)
                : base($"Constraint {constraint.Name} not satisfied.")
            {
                Constraint = constraint;
            }

            public ConstraintNotSatisfiedException(IConstraint constraint, string message)
                : base(message)
            {
                Constraint = constraint;
            }

            public IConstraint Constraint { get; }
        }

        public interface IInvariant
        {
            public bool IsValid { get; }
        }

        public static class Optimization
        {
            public static Delegate Compile(this IEnumerable<IConstraint> constraints)
            {
                throw new NotImplementedException();
            }
            public static Func<T, bool> Compile<T>(this IEnumerable<IConstraint<T>> constraints)
            {
                throw new NotImplementedException();
            }
            public static Delegate CompileAssertion(this IEnumerable<IConstraint> constraints)
            {
                throw new NotImplementedException();
            }
            public static Action<T> CompileAssertion<T>(this IEnumerable<IConstraint<T>> constraints)
            {
                throw new NotImplementedException();
            }
        }
    }

    namespace Epistemology.Reasoning
    {
        public interface IRule : IInspectableFunc<IQueryable<Statement>, IQueryable<Statement>>
        {
            public string? Name { get; }

            public IReadOnlyList<Statement> Consequent { get; }

            public IReadOnlyList<Statement> Antecedent { get; }
        }

        public interface IReasoner
        {
            public IReadOnlyList<IConstraint<IQueryable<Statement>>> Constraints { get; }
            public IReadOnlyList<IRule> Rules { get; }

            public IStatementCollection Bind(IStatementCollection source);
        }
    }

    namespace Epistemology.Reasoning.Analogical
    {

    }
}