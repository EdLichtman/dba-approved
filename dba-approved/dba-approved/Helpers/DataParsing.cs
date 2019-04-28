using System;
using System.Data;
using System.Linq;

namespace dba_approved_core.Helpers
{
    public static class DataParsing
    {
        private static T? ParseNullableT<T>(DataRow row, string columnName) where T : struct
        {
            var nullableType = RuntimeParsing.GetNullableType(typeof(T));
            var parser = RuntimeParsing.GetParserMethod(nullableType);
            var columnAsString = row.ParseString(columnName);
            if (!string.IsNullOrWhiteSpace(columnAsString))
            {
                var result = parser(columnAsString);
                return (T?)result;
            }

            return default(T?);
        }
        private static bool TryParseFieldNullableT<T>(DataRow row, string columnName, out T? value) where T : struct
        {
            var nullableType = RuntimeParsing.GetNullableType(typeof(T));
            var parser = RuntimeParsing.GetParserMethod(nullableType);
            var doesColumnExist = row.TryParseString(columnName, out var columnAsString);
            if (doesColumnExist)
            {
                if (!string.IsNullOrWhiteSpace(columnAsString))
                    value = (T?)parser(columnAsString);
                else
                    value = default(T?);
            }
            else
                value = default(T?);

            return doesColumnExist;
        }

        private delegate bool TryParseNullableFunction<T>(DataRow row, string columnName, out T? value) where T : struct;
        private static bool TryParseConcrete<T>(this DataRow row, string columnName, TryParseNullableFunction<T> func, out T value)
            where T : struct
        {
            var isSuccess = func(row, columnName, out var nullableValue);
            if (!nullableValue.HasValue)
                value = default(T);
            else
                value = nullableValue.Value;
            return isSuccess;
        }

        public static Guid? ParseNullableGuid(this DataRow row, string columnName) =>
            ParseNullableT<Guid>(row, columnName);

        public static bool TryParseNullableGuid(this DataRow row, string columnName, out Guid? value) =>
            TryParseFieldNullableT(row, columnName, out value);

        public static int? ParseNullableInt(this DataRow row, string columnName) =>
            ParseNullableT<int>(row, columnName);

        public static bool TryParseNullableInt(this DataRow row, string columnName, out int? value) =>
            TryParseFieldNullableT(row, columnName, out value);

        public static decimal? ParseNullableDecimal(this DataRow row, string columnName) =>
            ParseNullableT<decimal>(row, columnName);

        public static bool TryParseNullableDecimal(this DataRow row, string columnName, out decimal? value) =>
            TryParseFieldNullableT(row, columnName, out value);

        public static double? ParseNullableDouble(this DataRow row, string columnName) =>
            ParseNullableT<double>(row, columnName);

        public static bool TryParseNullableDouble(this DataRow row, string columnName, out double? value) =>
            TryParseFieldNullableT(row, columnName, out value);

        public static bool? ParseNullableBool(this DataRow row, string columnName)
        {
            if (TryParseFieldNullableT<int>(row, columnName, out var returnVal) && returnVal.HasValue)
                return returnVal.Value == 1;
            return ParseNullableT<bool>(row, columnName);
        }

        public static bool TryParseNullableBool(this DataRow row, string columnName, out bool? value) =>
            TryParseFieldNullableT(row, columnName, out value);

        public static DateTime? ParseNullableDateTime(this DataRow row, string columnName) =>
            ParseNullableT<DateTime>(row, columnName);

        public static bool TryParseNullableDateTime(this DataRow row, string columnName, out DateTime? value) =>
            TryParseFieldNullableT(row, columnName, out value);

        public static long? ParseNullableLong(this DataRow row, string columnName)
            => ParseNullableT<long>(row, columnName);

        public static bool TryParseNullableLong(this DataRow row, string columnName, out long? value) =>
            TryParseFieldNullableT(row, columnName, out value);

        public static Guid ParseGuid(this DataRow row, string columnName) =>
            ParseNullableGuid(row, columnName) ?? Guid.Empty;


        public static bool TryParseGuid(this DataRow row, string columnName, out Guid value) =>
            TryParseConcrete(row, columnName, TryParseNullableGuid, out value);

        public static int ParseInt(this DataRow row, string columnName) =>
            row.ParseNullableInt(columnName) ?? default(int);

        public static bool TryParseInt(this DataRow row, string columnName, out int value) =>
            TryParseConcrete(row, columnName, TryParseNullableInt, out value);


        public static decimal ParseDecimal(this DataRow row, string columnName) =>
            ParseNullableDecimal(row, columnName) ?? default(decimal);

        public static bool TryParseDecimal(this DataRow row, string columnName, out decimal value) =>
            TryParseConcrete(row, columnName, TryParseNullableDecimal, out value);

        public static double ParseDouble(this DataRow row, string columnName) =>
            ParseNullableDouble(row, columnName) ?? default(double);
        public static bool TryParseDouble(this DataRow row, string columnName, out double value) =>
            TryParseConcrete(row, columnName, TryParseNullableDouble, out value);

        public static bool ParseBool(this DataRow row, string columnName) =>
            ParseNullableBool(row, columnName) ?? default(bool);

        public static bool TryParseBool(this DataRow row, string columnName, out bool value) =>
            TryParseConcrete(row, columnName, TryParseNullableBool, out value);

        public static DateTime ParseDateTime(this DataRow row, string columnName) =>
            ParseNullableDateTime(row, columnName) ?? default(DateTime);
        public static bool TryParseDateTime(this DataRow row, string columnName, out DateTime value) =>
            TryParseConcrete(row, columnName, TryParseNullableDateTime, out value);

        public static long ParseLong(this DataRow row, string columnName)
            => ParseNullableLong(row, columnName) ?? default(long);

        public static bool TryParseLong(this DataRow row, string columnName, out long value) =>
            TryParseConcrete(row, columnName, TryParseNullableLong, out value);

        public static T ParseEnumFromInt<T>(this DataRow row, string columnName, T defaultValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            var valueAsInt = row.ParseInt(columnName);
            T objectAsEnum = (T)(object)valueAsInt;

            T[] suitableOptions = (T[])Enum.GetValues(typeof(T)).Cast<T>();
            if (suitableOptions.Contains(objectAsEnum))
                return objectAsEnum;
            return defaultValue;
        }

        public static T ParseEnumFromString<T>(this DataRow row, string columnName, T defaultValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            var valueAsString = row.ParseString(columnName);
            if (Enum.TryParse<T>(valueAsString, out var value))
                return value;
            return defaultValue;
        }
        public static string ParseString(this DataRow row, string columnName)
        {
            return row[columnName].ToString();
        }
        public static bool TryParseString(this DataRow row, string columnName, out string value)
        {
            var doesColumnExist = row.Table.Columns.Contains(columnName);
            value = doesColumnExist ? row[columnName].ToString() : null;
            return doesColumnExist;
        }
    }
}
