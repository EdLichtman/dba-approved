using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace dba_approved_core.Helpers
{
        /// <summary>
        /// Returns a dynamically generated function to parse out your intended simple data type.
        /// (Simple as opposed to Primitive because it also parses strings, Guids, etc)
        ///
        /// Can currently convert:
        /// Guid (braces and non-braces)
        /// Guid?
        /// int
        /// int?
        /// long
        /// long?
        /// decimal
        /// decimal?
        /// double
        /// double?
        /// bool
        /// bool?
        /// DateTime
        /// DateTime?
        /// char
        /// char?
        /// char[]
        /// string
        /// </summary>
        /// <typeparam name="T">Simple Data Type to get the string parsing function</typeparam>
        /// <returns>
        /// A function that takes in a string and returns a value of type T
        /// </returns>
    public static class RuntimeParsing
    {
        public static Func<string, T> GetParserMethod<T>()
        {

            return value => (T)GetParserMethod(typeof(T))(value);
        }
        public static bool ParseBool(string value)
        {
            return Parse(value, bool.Parse);
        }
        private static T Parse<T>(string value, Func<string, T> parse)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;
            return parse(value);
        }
        
        public static Func<string, object> GetParserMethod(Type t) 
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
            parserMethods.Add(typeof(int?), value => (int?)TryParseNullable<double>(double.TryParse, value));
            parserMethods.Add(typeof(long?), value => (long?)TryParseNullable<double>(double.TryParse, value));
            parserMethods.Add(typeof(decimal?), value => TryParseNullable<decimal>(decimal.TryParse, value));
            parserMethods.Add(typeof(double?), value => TryParseNullable<double>(double.TryParse, value));
            parserMethods.Add(typeof(bool?), value => TryParseNullable<bool>(bool.TryParse, value?.ToLower()));
            parserMethods.Add(typeof(DateTime?), value => TryParseNullable<DateTime>(DateTime.TryParse, value));
            parserMethods.Add(typeof(char?), value => TryParseNullable<char>(char.TryParse, value));

            parserMethods.Add(typeof(Guid), value => parserMethods[typeof(Guid?)](value) ?? default(Guid));
            parserMethods.Add(typeof(int), value => parserMethods[typeof(int?)](value) ?? default(int));
            parserMethods.Add(typeof(long), value => parserMethods[typeof(long?)](value) ?? default(long));
            parserMethods.Add(typeof(decimal), value => parserMethods[typeof(decimal?)](value) ?? default(decimal));
            parserMethods.Add(typeof(double), value => parserMethods[typeof(double?)](value) ?? default(double));
            parserMethods.Add(typeof(bool), value => parserMethods[typeof(bool?)](value) ?? default(bool));
            parserMethods.Add(typeof(DateTime), value => parserMethods[typeof(DateTime?)](value) ?? default(DateTime));
            parserMethods.Add(typeof(char), value => parserMethods[typeof(char?)](value) ?? default(char));

            parserMethods.Add(typeof(string), value => value);
            parserMethods.Add(typeof(char[]), value => string.IsNullOrWhiteSpace(value) ? new char[] { } : value.ToCharArray());

            if (parserMethods.ContainsKey(t))
                return value => parserMethods[t](value);

            throw new KeyNotFoundException($"The requested type: {t.Name} does not exist in Runtime Parsing Extensions.");
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
