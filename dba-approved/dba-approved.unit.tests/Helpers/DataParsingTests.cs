using dba_approved.unit.tests.InternalTestClasses;
using NUnit.Framework;
using System;
using System.Data;
using dba_approved_core.Helpers;

namespace dba_approved.unit.tests.Helpers
{
    [TestFixture]
    class DataParsingTests
{
        private DataRow _dataRow;
        private const string NullColumnName = "nullColumn";
        private const string GuidColumnName = "guidColumn";
        private const string NumberColumnName = "numberColumn";
        private const string BoolColumnName = "boolColumn";
        private const string BoolNumColumnName = "boolNumColumn";
        private const string DateTimeColumnName = "dateTimeColumn";
        private const string EnumColumnName = "enumColumn";
        private const string EnumStringColumnName = "enumStringColumn";
        private const string InvalidColumnName = "NotARealColumnName";


        [SetUp]
        public void SetUp()
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new[]
            {
                new DataColumn(NullColumnName, typeof(string)),
                new DataColumn(GuidColumnName, typeof(Guid)),
                new DataColumn(NumberColumnName, typeof(int)),
                new DataColumn(BoolColumnName, typeof(bool)),
                new DataColumn(BoolNumColumnName, typeof(int)),
                new DataColumn(DateTimeColumnName, typeof(DateTime)),
                new DataColumn(EnumColumnName, typeof(int)),
                new DataColumn(EnumStringColumnName, typeof(string)),
            });

            _dataRow = dataTable.NewRow();
            _dataRow[NullColumnName] = null;
            _dataRow[GuidColumnName] = Guid.NewGuid();
            _dataRow[NumberColumnName] = 1.3;
            _dataRow[BoolColumnName] = true;
            _dataRow[BoolNumColumnName] = 1;
            _dataRow[DateTimeColumnName] = DateTime.Now;
            _dataRow[EnumColumnName] = (int)TestEnum.SecondaryOption;
            _dataRow[EnumStringColumnName] = TestEnum.SecondaryOption.ToString();
        }

        public T WrapWithTimer<T>(Func<T> func)
        {
            var startTime = new DateTime(DateTime.Now.Ticks);
            var result = func();
            var endTime = new DateTime(DateTime.Now.Ticks);
            var totalTimeSpan = endTime - startTime;

            Console.WriteLine($"Total Milliseconds to compute: {totalTimeSpan.TotalMilliseconds}");

            if (totalTimeSpan.TotalMilliseconds > 13)
                Assert.Fail("Took too long for test to complete.");

            return result;
        }

        [Test]
        public void Can_parse_nullable_Guid_null_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableGuid(NullColumnName));
            Assert.That(expectedValue, Is.Null);
        }
        [Test]
        public void Can_parse_nullable_int_null_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableInt(NullColumnName));
            Assert.That(expectedValue, Is.Null);
        }

        [Test]
        public void Can_parse_nullable_decimal_null_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableDecimal(NullColumnName));
            Assert.That(expectedValue, Is.Null);
        }

        [Test]
        public void Can_parse_nullable_double_null_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableDouble(NullColumnName));
            Assert.That(expectedValue, Is.Null);
        }

        [Test]
        public void Can_parse_nullable_bool_null_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableBool(NullColumnName));
            Assert.That(expectedValue, Is.Null);
        }

        [Test]
        public void Can_parse_nullable_datetime_null_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableDateTime(NullColumnName));
            Assert.That(expectedValue, Is.Null);
        }
        [Test]
        public void Can_parse_nullable_Guid_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableGuid(GuidColumnName));
            Assert.That(expectedValue, Is.Not.Null.Or.EqualTo(Guid.Empty));
        }
        [Test]
        public void Can_parse_nullable_int_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableInt(NumberColumnName));
            Assert.That(expectedValue, Is.GreaterThan(0));
        }

        [Test]
        public void Can_parse_nullable_decimal_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableDecimal(NumberColumnName));
            Assert.That(expectedValue, Is.GreaterThan(0));
        }

        [Test]
        public void Can_parse_nullable_double_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableDouble(NumberColumnName));
            Assert.That(expectedValue, Is.GreaterThan(0));
        }

        [Test]
        public void Can_parse_nullable_bool_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableBool(BoolColumnName));
            Assert.That(expectedValue, Is.Not.Null.Or.EqualTo(default(bool)));
        }
        [Test]
        public void Can_parse_nullable_bool_num_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableBool(BoolNumColumnName));
            Assert.That(expectedValue, Is.Not.Null.Or.EqualTo(default(bool)));
        }

        [Test]
        public void Can_parse_nullable_datetime_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseNullableDateTime(DateTimeColumnName));
            Assert.That(expectedValue, Is.Not.Null.Or.EqualTo(default(DateTime)));
        }
        [Test]
        public void Can_parse_Guid_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseGuid(GuidColumnName));
            Assert.That(expectedValue, Is.Not.EqualTo(Guid.Empty));
        }
        [Test]
        public void Can_parse_int_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseInt(NumberColumnName));
            Assert.That(expectedValue, Is.GreaterThan(0));
        }

        [Test]
        public void Can_parse_decimal_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseDecimal(NumberColumnName));
            Assert.That(expectedValue, Is.GreaterThan(0));
        }

        [Test]
        public void Can_parse_double_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseDouble(NumberColumnName));
            Assert.That(expectedValue, Is.GreaterThan(0));
        }

        [Test]
        public void Can_parse_bool_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseBool(BoolColumnName));
            Assert.That(expectedValue, Is.Not.EqualTo(default(bool)));
        }
        [Test]
        public void Can_parse_bool_num_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseBool(BoolNumColumnName));
            Assert.That(expectedValue, Is.Not.EqualTo(default(bool)));
        }

        [Test]
        public void Can_parse_datetime_value()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseDateTime(DateTimeColumnName));
            Assert.That(expectedValue, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void Can_parse_enum_from_int()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseEnumFromInt(EnumColumnName, TestEnum.DefaultOption));
            Assert.That(expectedValue, Is.Not.EqualTo(TestEnum.DefaultOption));
        }

        [Test]
        public void Can_parse_enum_from_string()
        {
            var expectedValue = WrapWithTimer(() => _dataRow.ParseEnumFromString(EnumStringColumnName, TestEnum.DefaultOption));
            Assert.That(expectedValue, Is.Not.EqualTo(TestEnum.DefaultOption));
        }


        [Test]
        public void Can_try_parse_from_string_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseString(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_string_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseString(GuidColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_NullableGuid_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableGuid(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_NullableGuid_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableGuid(GuidColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_NullableInt_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableInt(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_NullableInt_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableInt(NumberColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_NullableDecimal_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableDecimal(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_NullableDecimal_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableDecimal(NumberColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }



        [Test]
        public void Can_try_parse_from_NullableBool_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableBool(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_NullableBool_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableBool(BoolColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_NullableDateTime_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableDateTime(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_NullableDateTime_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableDateTime(DateTimeColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_NullableLong_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableLong(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_NullableLong_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseNullableLong(NumberColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }


        [Test]
        public void Can_try_parse_from_Guid_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseGuid(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_Guid_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseGuid(GuidColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_Int_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseInt(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_Int_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseInt(NumberColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_Decimal_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseDecimal(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_Decimal_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseDecimal(NumberColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }



        [Test]
        public void Can_try_parse_from_Bool_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseBool(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_Bool_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseBool(BoolColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_DateTime_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseDateTime(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_DateTime_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseDateTime(DateTimeColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

        [Test]
        public void Can_try_parse_from_Long_with_expected_failure()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseLong(InvalidColumnName, out _));
            Assert.That(doesColumnExist, Is.False);
        }


        [Test]
        public void Can_try_parse_from_Long_with_expected_success()
        {
            var doesColumnExist = WrapWithTimer(() => _dataRow.TryParseLong(NumberColumnName, out _));
            Assert.That(doesColumnExist, Is.True);
        }

    }
}
