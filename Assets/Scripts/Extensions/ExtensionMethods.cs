using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Extensions
{
    public static class ExtensionMethods
    {
        public static bool IsMonoBehaviour(this Type t)
        {
            return typeof(MonoBehaviour).IsAssignableFrom(t);
        }

        public static IEnumerable<Type> GetTypesWithCustomAttribute<TAttribute>(this Assembly assembly) where TAttribute : Attribute
        {
            return assembly.GetTypes().Where(t => t.GetCustomAttribute<TAttribute>() != null);
        }
    }
}
