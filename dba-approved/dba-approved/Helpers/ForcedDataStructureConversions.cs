using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace dba_approved_core.Helpers
{
    /// <summary>
    /// Because this class has not been fully tested so as to find all weaknesses, this cannot be made public yet. 
    /// It works for the basic use case we have in this library, but more complicated data structures might not pass through this effectively or efficiently
    /// </summary>
    internal static class ForcedDataStructureConversionExtensions
    {
        public static T ToObject<T>(this IDictionary<string, object> source)
           where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key)
                    .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {

            var objectType = source.GetType();

            if (IsDictionary(objectType))
            {
                return (IDictionary<string, object>)source;

            }
            if (!IsAnonymousType(objectType))
            {
                var concreteObjectProperties = objectType.GetProperties(bindingAttr);
                if (concreteObjectProperties.Any())
                    return concreteObjectProperties.ToDictionary(
                        propInfo => propInfo.Name,
                        propInfo => propInfo.GetValue(source, null)
                    );
            }
            else
            {
                var anonymousObjectProperties = objectType.GetProperties();
                if (anonymousObjectProperties.Any())
                    return anonymousObjectProperties.ToDictionary(
                        propInfo => propInfo.Name,
                        propInfo => propInfo.GetValue(source, null)
                    );
            }
            return new Dictionary<string, object>();
        }

        private static bool IsDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }
        private static bool IsAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
