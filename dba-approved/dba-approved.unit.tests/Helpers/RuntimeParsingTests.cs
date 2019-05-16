using System;
using System.Linq;
using dba_approved_core.Helpers;
using NUnit.Framework;

namespace dba_approved.unit.tests.Helpers
{
    [TestFixture]
    class RuntimeParsingTests
    {
        private Guid _guid = Guid.NewGuid();
        private double _number = 1234.56;
        private string _string = "Hello world";
        private string _stringWithOneLetter = "a";
        private DateTime _dateTime = DateTime.Now;
        private bool _boolean = true;

        [Test]
        public void A_preliminary_to_reset_baseline()
        {
            //The testRunner seems to take the second alphabetical test to 
            //start the fixture which is providing an unrealistic baseline for 
            //optimization comparison when running all tests.
            //This is the first test to try and fix the baseline

            Assert.True(true);
        }
        [Test]
        public void A_second_preliminary_to_reset_baseline()
        {
            //The testRunner seems to take the second alphabetical test to 
            //start the fixture which is providing an unrealistic baseline for 
            //optimization comparison when running all tests.
            //This is the second test to try and fix the baseline

            Assert.True(true);
        }
        [Test]
        public void Can_parse_nullable_guid()
        {
            var value = _guid;
            var valueAsString = _guid.ToString();

            var parser = RuntimeParsing.GetParserMethod<Guid?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }
        [Test]
        public void Can_parse_nullable_guid_with_braces()
        {
            var value = _guid;
            var valueAsString = _guid.ToString("B");

            var parser = RuntimeParsing.GetParserMethod<Guid?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_guid()
        {
            var value = _guid;
            var valueAsString = _guid.ToString();

            var parser = RuntimeParsing.GetParserMethod<Guid>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.EqualTo(default(Guid)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }
        [Test]
        public void Can_parse_guid_with_braces()
        {
            var value = _guid;
            var valueAsString = _guid.ToString("B");

            var parser = RuntimeParsing.GetParserMethod<Guid>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.EqualTo(default(Guid)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }


        [Test]
        public void Can_parse_nullable_int()
        {
            var value = (int)_number;
            var valueAsString = _number.ToString();

            var parser = RuntimeParsing.GetParserMethod<int?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_int()
        {
            var value = (int)_number;
            var valueAsString = _number.ToString();

            var parser = RuntimeParsing.GetParserMethod<int>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.EqualTo(default(int)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_nullable_long()
        {
            var value = (long)_number;
            var valueAsString = _number.ToString();

            var parser = RuntimeParsing.GetParserMethod<long?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_long()
        {
            var value = (long)_number;
            var valueAsString = _number.ToString();

            var parser = RuntimeParsing.GetParserMethod<long>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.EqualTo(default(long)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_nullable_decimal()
        {
            var value = (decimal)_number;
            var valueAsString = _number.ToString();

            var parser = RuntimeParsing.GetParserMethod<decimal?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_decimal()
        {
            var value = (decimal)_number;
            var valueAsString = _number.ToString();

            var parser = RuntimeParsing.GetParserMethod<decimal>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.EqualTo(default(decimal)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }


        [Test]
        public void Can_parse_nullable_double()
        {
            var value = _number;
            var valueAsString = _number.ToString();

            var parser = RuntimeParsing.GetParserMethod<double?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_double()
        {
            var value = _number;
            var valueAsString = _number.ToString();

            var parser = RuntimeParsing.GetParserMethod<double>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.EqualTo(default(double)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }


        [Test]
        public void Can_parse_nullable_bool()
        {
            var value = _boolean;
            var valueAsString = _boolean.ToString();

            var parser = RuntimeParsing.GetParserMethod<bool?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }
        public T WrapWithTimer<T>(Func<T> func)
        {
            var startTime = new DateTime(DateTime.Now.Ticks);
            var result = func();
            var endTime = new DateTime(DateTime.Now.Ticks);
            var totalTimeSpan = endTime - startTime;

            Console.WriteLine($"Total Milliseconds to compute: {totalTimeSpan.TotalMilliseconds}");
            Console.WriteLine($"Other Info: Seconds: {totalTimeSpan.Seconds} " +
                $"| TotalSeconds: {totalTimeSpan.TotalSeconds} " +
                $"| Milliseconds: {totalTimeSpan.Milliseconds}");
            //if (totalTimeSpan.TotalMilliseconds > 13)
            //    Assert.Fail("Took too long for test to complete.");

            return result;
        }
        [Test]
        public void Can_parse_bool()
        {
            var value = _boolean;
            var valueAsString = _boolean.ToString();

            var parsedValue = WrapWithTimer(() =>
            {
                return RuntimeParsing.ParseBool(valueAsString);
            });

            Assert.That(parsedValue, Is.Not.EqualTo(default(bool)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_bool_performance_benchmark_baseline()
        {
            var value = _boolean;
            var valueAsString = _boolean.ToString();

            bool parsedValue = default;
            WrapWithTimer(() => bool.TryParse(valueAsString, out parsedValue));

            Assert.That(parsedValue, Is.Not.EqualTo(default(bool)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }


        [Test]
        public void Can_parse_nullable_DateTime()
        {
            var value = _dateTime;
            var valueAsString = _dateTime.ToString("O");

            var parser = RuntimeParsing.GetParserMethod<DateTime?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_DateTime()
        {
            var value = _dateTime;
            var valueAsString = _dateTime.ToString("O");

            var parser = RuntimeParsing.GetParserMethod<DateTime>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.EqualTo(default(DateTime)));
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_string()
        {
            var value = _string;
            var valueAsString = _string;

            var parser = RuntimeParsing.GetParserMethod<string>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Cannot_parse_string_with_multiple_characters_as_nullable_char()
        {
            var valueAsString = _string;

            var parser = RuntimeParsing.GetParserMethod<char?>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Null);
        }

        [Test]
        public void Cannot_parse_string_with_multiple_characters_as_char()
        {
            var valueAsString = _stringWithOneLetter;

            var parser = RuntimeParsing.GetParserMethod<char>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.EqualTo(default(char)));
            Assert.That(parsedValue,Is.EqualTo(_stringWithOneLetter.ToCharArray().First()));
        }

        [Test]
        public void Can_parse_char()
        {
            var value = _string;
            var valueAsString = _string;

            var parser = RuntimeParsing.GetParserMethod<string>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_parse_char_array()
        {
            var value = _string.ToCharArray();
            var valueAsString = _string;

            var parser = RuntimeParsing.GetParserMethod<char[]>();

            var parsedValue = parser(valueAsString);
            Assert.That(parsedValue, Is.Not.Null);
            Assert.That(parsedValue, Is.Not.Empty);
            Assert.That(parsedValue, Is.EqualTo(value));
        }

        [Test]
        public void Can_get_nullable_type_for_bool()
        {
            var typeValue = typeof(bool);
            var expectedNullableTypeValue = typeof(bool?);

            var nullableTypeValue = RuntimeParsing.GetNullableType(typeValue);
            Assert.That(nullableTypeValue, Is.EqualTo(expectedNullableTypeValue));
        }
    }
}