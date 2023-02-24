using AI.Epistemology;
using AI.Epistemology.Constraints;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace AI.AutomatedPlanning
{
    public interface IDomain
    {
        public IReadOnlyList<Type> Types { get; }
        public IReadOnlyList<IOperator> Operators { get; }
        public IReadOnlyList<Symbol> Predicates { get; }
    }

    public interface IProblem
    {
        public IDomain Domain { get; }
        public IState Initial { get; }
        public IReadOnlyList<IConstraint<IState>> Goal { get; } // or public IState Goal { get; } ?
        public IEnumerable Objects { get; }

        public IReadOnlyList<IConstraint<IQueryable<IState>>> Constraints { get; }
        public IReadOnlyList<IConstraint<IQueryable<IState>>> Preferences { get; }

        //... public IInspectableFunc<IQueryable<IState>, IComparable> Metric { get; }
    }

    public interface IOperator
    {
        public IDomain Domain { get; }
        public IReadOnlyList<IConstraint> Constraints { get; }
        public bool CanInvoke(object?[]? args, [NotNullWhen(false)] out Exception? reason);
        public IAction Invoke(object?[]? args);
        public IReadOnlyList<IMethodGenerator>? Preconditions { get; }
        public IReadOnlyList<IMethodGenerator>? Effects { get; }
    }

    public interface IAction
    {
        public IOperator Operator { get; }
        public IReadOnlyList<object?> Arguments { get; }
        public IReadOnlyList<IConstraint<IState>> Preconditions { get; }
        public IReadOnlyList<IInspectableAction<IState>> Effects { get; }
        // public IComparable Cost { get; }
    }

    public interface IPlan
    {
        public IReadOnlyList<IAction> Actions { get; }

        // public IComparable Metric { get; }
    }

    public interface ISolver
    {
        public IAsyncEnumerable<IPlan> Solve(IProblem problem, CancellationToken cancellationToken = default);
    }

    public interface IState : IInvariant
    {
        public IStatementCollection Content { get; }

        public bool TryGetNext(IDelta<Statement> delta, out IState? state)
        {
            return TryGetNext(delta.Removals, delta.Additions, out state);
        }
        public bool TryGetNext(IEnumerable<Statement> removals, IEnumerable<Statement> additions, out IState? state);
    }

    public static class StateTrajectory
    {
        public static class Constraints
        {
            public static IConstraint<IQueryable<T>> Always<T>(Expression<Func<T, bool>> predicate, string name)
            {
                throw new NotImplementedException();
            }
            public static IConstraint<IQueryable<T>> Sometime<T>(Expression<Func<T, bool>> predicate, string name)
            {
                throw new NotImplementedException();
            }

            //...
        }

        public static bool Always<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.All(predicate);
        }
        public static bool Sometime<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Any(predicate);
        }
        public static bool AtMostOnce<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int counter = 0;
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    if (++counter > 1) return false;
                }
            }
            return true;
        }
        public static bool AtEnd<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            bool any = false;
            T? last = default;

            foreach (var element in source)
            {
                any = true;
                last = element;
            }

            if (!any) return true;
            return predicate(last!);
        }
        public static bool Within<T>(this IEnumerable<T> source, int count, Func<T, bool> predicate)
        {
            return source.Take(count).Any(predicate);
        }
        public static bool SometimeAfter<T>(this IEnumerable<T> source, Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            bool found = false;
            foreach (var element in source)
            {
                if (!found)
                {
                    if (predicate1(element)) found = true;
                }
                else if (predicate2(element)) return true;
            }
            return !found;
        }
        public static bool SometimeBefore<T>(this IEnumerable<T> source, Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            bool found = false;
            foreach (var element in source)
            {
                if (!found)
                {
                    if (predicate1(element)) return false;
                    if (predicate2(element)) found = true;
                }
                else break;
            }
            return true;
        }
        public static bool AlwaysWithin<T>(this IEnumerable<T> source, int count, Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            int index = -1;
            int counter = 0;
            foreach (var element in source)
            {
                if (predicate2(element))
                {
                    if (index >= 0)
                    {
                        if (counter - index > count)
                        {
                            return false;
                        }
                        index = -1;
                    }
                }
                else if (predicate1(element))
                {
                    if (index < 0)
                    {
                        index = counter;
                    }
                }
                ++counter;
            }
            return index < 0;
        }
        public static bool HoldAfter<T>(this IEnumerable<T> source, int count, Func<T, bool> predicate)
        {
            return source.Skip(count).Always(predicate);
        }
        public static bool HoldDuring<T>(this IEnumerable<T> source, int from, int to, Func<T, bool> predicate)
        {
            return source.Skip(from).Take(to - from).Always(predicate);
        }

        //...

        public static bool Always<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source.All(predicate);
        }
        public static bool Sometime<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
        {
            return source.Any(predicate);
        }
    }
}