﻿namespace Naos.Core.Common
{
    using System;
    using System.Linq;

    public static class TypeExtensions
    {
        public static bool IsOfType(this object source, Type targetType)
        {
            if (source == null)
            {
                return false;
            }

            return source.GetType() == targetType;
        }

        public static bool IsNotOfType(this object source, Type targetType)
        {
            if (source == null)
            {
                return false;
            }

            return source.GetType() != targetType;
        }

        public static string PrettyName(this Type source, bool useAngleBrackets = true)
        {
            if (source.IsGenericType)
            {
                var genericOpen = useAngleBrackets ? "<" : "[";
                var genericClose = useAngleBrackets ? ">" : "]";
                var name = source.Name.Substring(0, source.Name.IndexOf('`'));
                var types = string.Join(",", source.GetGenericArguments().Select(t => t.PrettyName()));
                return $"{name}{genericOpen}{types}{genericClose}";
            }

            return source.Name;
        }

        public static string FullPrettyName(this Type source, bool useAngleBrackets = true)
        {
            if (source.IsGenericType)
            {
                var genericOpen = useAngleBrackets ? "<" : "[";
                var genericClose = useAngleBrackets ? ">" : "]";
                var name = source.FullName.Substring(0, source.Name.IndexOf('`'));
                var types = string.Join(",", source.GetGenericArguments().Select(t => t.FullPrettyName()));
                return $"{name}{genericOpen}{types}{genericClose}";
            }

            return source.FullName;
        }
    }
}
