namespace System
{
    public struct OneOf<T1, T2>
    {
        public OneOf(T1 value)
        {
            m_type = typeof(T1);
            m_value = value;
        }
        public OneOf(T2 value)
        {
            m_type = typeof(T2);
            m_value = value;
        }

        private Type m_type;
        private object? m_value;

        public Type GetValueType()
        {
            return m_type;
        }
        public bool TryGetValue(out T1? value)
        {
            if (m_type == typeof(T1))
            {
                value = (T1?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public bool TryGetValue(out T2? value)
        {
            if (m_type == typeof(T2))
            {
                value = (T2?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public override int GetHashCode()
        {
            return m_value?.GetHashCode() ?? 0;
        }
    }

    public struct OneOf<T1, T2, T3>
    {
        public OneOf(T1 value)
        {
            m_type = typeof(T1);
            m_value = value;
        }
        public OneOf(T2 value)
        {
            m_type = typeof(T2);
            m_value = value;
        }
        public OneOf(T3 value)
        {
            m_type = typeof(T3);
            m_value = value;
        }

        private Type m_type;
        private object? m_value;

        public Type GetValueType()
        {
            return m_type;
        }
        public bool TryGetValue(out T1? value)
        {
            if (m_type == typeof(T1))
            {
                value = (T1?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public bool TryGetValue(out T2? value)
        {
            if (m_type == typeof(T2))
            {
                value = (T2?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public bool TryGetValue(out T3? value)
        {
            if (m_type == typeof(T3))
            {
                value = (T3?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public override int GetHashCode()
        {
            return m_value?.GetHashCode() ?? 0;
        }
    }

    public struct OneOf<T1, T2, T3, T4>
    {
        public OneOf(T1 value)
        {
            m_type = typeof(T1);
            m_value = value;
        }
        public OneOf(T2 value)
        {
            m_type = typeof(T2);
            m_value = value;
        }
        public OneOf(T3 value)
        {
            m_type = typeof(T3);
            m_value = value;
        }
        public OneOf(T4 value)
        {
            m_type = typeof(T4);
            m_value = value;
        }

        private Type m_type;
        private object? m_value;

        public Type GetValueType()
        {
            return m_type;
        }
        public bool TryGetValue(out T1? value)
        {
            if (m_type == typeof(T1))
            {
                value = (T1?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public bool TryGetValue(out T2? value)
        {
            if (m_type == typeof(T2))
            {
                value = (T2?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public bool TryGetValue(out T3? value)
        {
            if (m_type == typeof(T3))
            {
                value = (T3?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public bool TryGetValue(out T4? value)
        {
            if (m_type == typeof(T4))
            {
                value = (T4?)m_value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public override int GetHashCode()
        {
            return m_value?.GetHashCode() ?? 0;
        }
    }

    public static class OneOf
    {
        public static TResult Match<T1, T2, TResult>(this OneOf<T1, T2> oneof, Func<T1?, TResult> selector1, Func<T2?, TResult> selector2)
        {
            if (oneof.TryGetValue(out T1? value1))
            {
                return selector1(value1);
            }
            else if (oneof.TryGetValue(out T2? value2))
            {
                return selector2(value2);
            }
            else throw new Exception();
        }
        public static void Switch<T1, T2>(this OneOf<T1, T2> oneof, Action<T1?> action1, Action<T2?> action2)
        {
            if (oneof.TryGetValue(out T1? value1))
            {
                action1(value1);
            }
            else if (oneof.TryGetValue(out T2? value2))
            {
                action2(value2);
            }
            else throw new Exception();
        }
        public static TResult Match<T1, T2, T3, TResult>(this OneOf<T1, T2, T3> oneof, Func<T1?, TResult> selector1, Func<T2?, TResult> selector2, Func<T3?, TResult> selector3)
        {
            if (oneof.TryGetValue(out T1? value1))
            {
                return selector1(value1);
            }
            else if (oneof.TryGetValue(out T2? value2))
            {
                return selector2(value2);
            }
            else if (oneof.TryGetValue(out T3? value3))
            {
                return selector3(value3);
            }
            else throw new Exception();
        }
        public static void Switch<T1, T2, T3>(this OneOf<T1, T2, T3> oneof, Action<T1?> action1, Action<T2?> action2, Action<T3?> action3)
        {
            if (oneof.TryGetValue(out T1? value1))
            {
                action1(value1);
            }
            else if (oneof.TryGetValue(out T2? value2))
            {
                action2(value2);
            }
            else if (oneof.TryGetValue(out T3? value3))
            {
                action3(value3);
            }
            else throw new Exception();
        }
        public static TResult Match<T1, T2, T3, T4, TResult>(this OneOf<T1, T2, T3, T4> oneof, Func<T1?, TResult> action1, Func<T2?, TResult> action2, Func<T3?, TResult> action3, Func<T4?, TResult> action4)
        {
            if (oneof.TryGetValue(out T1? value1))
            {
                return action1(value1);
            }
            else if (oneof.TryGetValue(out T2? value2))
            {
                return action2(value2);
            }
            else if (oneof.TryGetValue(out T3? value3))
            {
                return action3(value3);
            }
            else if (oneof.TryGetValue(out T4? value4))
            {
                return action4(value4);
            }
            else throw new Exception();
        }
        public static void Switch<T1, T2, T3, T4>(this OneOf<T1, T2, T3, T4> oneof, Action<T1?> action1, Action<T2?> action2, Action<T3?> action3, Action<T4?> action4)
        {
            if (oneof.TryGetValue(out T1? value1))
            {
                action1(value1);
            }
            else if (oneof.TryGetValue(out T2? value2))
            {
                action2(value2);
            }
            else if (oneof.TryGetValue(out T3? value3))
            {
                action3(value3);
            }
            else if (oneof.TryGetValue(out T4? value4))
            {
                action4(value4);
            }
            else throw new Exception();
        }
    }
}