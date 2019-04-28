using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dba_approved_core.Helpers
{
    /// <summary>
    /// Until this class if fully researched and tested for efficiency and optimization, it's a class with volatile behavior. Therefore It is not public yet.
    /// </summary>
    internal static class RuntimeParsing
    {
        public static Func<string, T> GetParserMethod<T>()
        {
            var parserMethods = new Dictionary<Type, Func<string, object>>();
            parserMethods.Add(typeof(Guid?), value =>
            {
                if (Guid.TryParse(value, out var result))
                    return result;
                if (Guid.TryParseExact(value, "B", out var bracesResult))
                    return bracesResult;
                return null;
            });
            parserMethods.Add(typeof(int?), value => TryParseNullable<int>(int.TryParse, value));
            parserMethods.Add(typeof(decimal?), value => TryParseNullable<decimal>(decimal.TryParse, value));
            parserMethods.Add(typeof(double?), value => TryParseNullable<double>(double.TryParse, value));
            parserMethods.Add(typeof(bool?), value => TryParseNullable<bool>(bool.TryParse, value.ToLower()));
            parserMethods.Add(typeof(DateTime?), value => TryParseNullable<DateTime>(DateTime.TryParse, value));
            parserMethods.Add(typeof(long?), value => TryParseNullable<long>(long.TryParse, value));

            var nullableTypes = parserMethods.Keys.ToList();
            foreach (var nullableType in nullableTypes)
            {
                var baseType = Nullable.GetUnderlyingType(nullableType);
                if (baseType != null && !parserMethods.ContainsKey(baseType))
                    parserMethods.Add(baseType, value => parserMethods[nullableType](value) ?? GetDefaultValue(baseType));
            }

            parserMethods.Add(typeof(string), value => value);

            return value => (T)Convert.ChangeType(parserMethods[typeof(T)](value), typeof(T));
        }
        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
        public static Func<string, object> GetParserMethod(Type t)
        {
            var parserMethods = new Dictionary<Type, Func<string, dynamic>>();
            parserMethods.Add(typeof(Guid?), value =>
            {
                if (Guid.TryParse(value, out var result))
                    return result;
                if (Guid.TryParseExact(value, "B", out var bracesResult))
                    return bracesResult;
                return null;
            });
            parserMethods.Add(typeof(int?), value => TryParseNullable<int>(int.TryParse, value));
            parserMethods.Add(typeof(decimal?), value => TryParseNullable<decimal>(decimal.TryParse, value));
            parserMethods.Add(typeof(double?), value => TryParseNullable<double>(double.TryParse, value));
            parserMethods.Add(typeof(bool?), value => TryParseNullable<bool>(bool.TryParse, value.ToLower()));
            parserMethods.Add(typeof(DateTime?), value => TryParseNullable<DateTime>(DateTime.TryParse, value));
            parserMethods.Add(typeof(long?), value => TryParseNullable<long>(long.TryParse, value));

            return value => parserMethods[t](value);
        }
        public static Type GetNullableType(Type type)
        {
            // Use Nullable.GetUnderlyingType() to remove the Nullable<T> wrapper if type is already nullable.
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                type = nullableType;
            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            else
                return type;
        }

        delegate bool NullableTypeParser<T>(string input, out T output);
        private static T? TryParseNullable<T>(NullableTypeParser<T> tryGetValue, string input) where T : struct
        {
            if (tryGetValue(input, out var result))
                return result;

            return null;
        }
    }
}
