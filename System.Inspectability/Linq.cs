namespace System.Linq
{
    public static class Inspectability
    {
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> collection, IInspectableFunc<TSource, bool> predicate)
        {
            return collection.Where(predicate.Invoke);
        }
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> collection, IInspectableFunc<TSource, bool> predicate)
        {
            return collection.Where(predicate.Expression);
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> collection, IInspectableFunc<TSource, TResult> selector)
        {
            return collection.Select(selector.Invoke);
        }
        public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> collection, IInspectableFunc<TSource, TResult> selector)
        {
            return collection.Select(selector.Expression);
        }
    }
}
