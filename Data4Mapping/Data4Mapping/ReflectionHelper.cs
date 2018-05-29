using System;
using System.Reflection;

namespace Data4Mapping
{
    internal static class ReflectionHelper
    {
        public static TAttribute FindAttribute<TAttribute>(Type type)
        {
            var attr = type.GetCustomAttributes(false);
            return FindAttribute<TAttribute>(attr);
        }

        public static TAttribute FindAttribute<TAttribute>(PropertyInfo property)
        {
            var attr = Attribute.GetCustomAttributes(property);
            // ReSharper disable CoVariantArrayConversion
            return FindAttribute<TAttribute>(attr);
            // ReSharper restore CoVariantArrayConversion
        }

        public static TAttribute FindAttribute<TAttribute>(object[] attributes)
        {
            foreach (object obj in attributes)
                if (obj is TAttribute) return (TAttribute)obj;
            return default(TAttribute);
        }
    }
}
