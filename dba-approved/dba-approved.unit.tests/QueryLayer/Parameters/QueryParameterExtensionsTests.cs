using NUnit.Framework;
using System;
using System.Data.SqlClient;
using dba_approved_core.QueryLayer.Parameters;
using System.Collections.Generic;
using dba_approved.unit.tests.InternalTestClasses.QueryLayer.Parameters.StoredProcedures;
using System.Data;

namespace dba_approved.unit.tests.QueryLayer.Parameters
{
    [TestFixture]
    class QueryParameterExtensionsTests
    {
        private SqlCommand _sqlCommand;
        [SetUp]
        public void SetUp()
        {
            _sqlCommand = new SqlCommand("SELECT 1 FROM FOO WHERE ProductId IN (@productIds) AND OrderId IN (@orderIds) AND UserId in (@userIds)");
            var implicitlyNamed = "implicitlyNamed";
            string dbNull = null;

            Console.WriteLine("Adding Initial Parameters");
            WrapWithTimer(() =>

                _sqlCommand.AddParameters(new
                {
                    stronglyNamed = "stronglyNamed",
                    implicitlyNamed,
                    equalSignInString = "EqualSign=InString",
                    commaInString = "comma,InString",
                    onlyColonInString = ":",
                    dbNull,
                    onlyCommaInString = ",",
                    randomInt = 1,
                    commaAndQuotesInString = "comma\",\"InString",
                    orderIds = new object[]
                    {
                        1,
                        2
                    }
                })
            );

            Console.WriteLine("Adding int Array");

            WrapWithTimer(() => _sqlCommand.AddParameterArray("@productIds", new object[]
            {
                1,
                2
            }));


            var dictionaryOfArrays = new Dictionary<string, object>();
            dictionaryOfArrays.Add("@userIds", new List<int>
            {
                1,
                2
            });

            Console.WriteLine("Adding Dictionary of Arrays");
            WrapWithTimer(() => _sqlCommand.AddParameters(dictionaryOfArrays));

        }

        public T WrapWithTimer<T>(Func<T> func)
        {
            var startTime = new DateTime(DateTime.Now.Ticks);
            var result = func();
            var endTime = new DateTime(DateTime.Now.Ticks);
            var totalTimeSpan = endTime - startTime;

            Console.WriteLine($"Total Milliseconds to compute: {totalTimeSpan.Milliseconds}");

            return result;
        }
        public void WrapWithTimer(Action action)
        {
            var startTime = new DateTime(DateTime.Now.Ticks);
            action();
            var endTime = new DateTime(DateTime.Now.Ticks);
            var totalTimeSpan = endTime - startTime;

            Console.WriteLine($"Total Milliseconds to compute: {totalTimeSpan.TotalMilliseconds}");

            if (totalTimeSpan.TotalMilliseconds > 25)
                Assert.Fail("Took too long for test to complete.");
        }

        [Test]
        public void Can_add_one_strongly_named_parameter()
        {
            var parameter = _sqlCommand.Parameters["@stronglyNamed"].Value;
            var expectedValue = "stronglyNamed";

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }
        [Test]
        public void Can_add_type_other_than_string()
        {
            var parameter = _sqlCommand.Parameters["@randomInt"].Value;
            var expectedValue = 1;

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Can_add_one_implicitly_named_parameter()
        {
            var parameter = _sqlCommand.Parameters["@implicitlyNamed"].Value;
            var expectedValue = "implicitlyNamed";

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Can_add_one_parameter_with_equal_sign_in_string()
        {
            var parameter = _sqlCommand.Parameters["@equalSignInString"].Value;
            var expectedValue = "EqualSign=InString";

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Can_add_one_parameter_with_comma_in_string()
        {
            var parameter = _sqlCommand.Parameters["@commaInString"].Value;
            var expectedValue = "comma,InString";

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Can_add_one_parameter_with_comma_and_quotes_in_string()
        {
            var parameter = _sqlCommand.Parameters["@commaAndQuotesInString"].Value;
            var expectedValue = "comma\",\"InString";

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Can_add_one_parameter_with_only_comma_in_string()
        {
            var parameter = _sqlCommand.Parameters["@onlyCommaInString"].Value;
            var expectedValue = ",";

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }


        [Test]
        public void Can_add_one_parameter_with_only_colon_in_string()
        {
            var parameter = _sqlCommand.Parameters["@onlyColonInString"].Value;
            var expectedValue = ":";

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Can_add_one_null_parameter()
        {
            var parameter = _sqlCommand.Parameters["@dbNull"].Value;
            var expectedValue = DBNull.Value;

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Can_add_array_of_objects()
        {
            var parameter = _sqlCommand.Parameters["@productIds0"].Value;
            var expectedValue = 1;

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Adding_array_of_objects_replaces_command_text_ParameterName()
        {
            var sqlCommandText = _sqlCommand.CommandText;

            Assert.That(sqlCommandText, Does.Contain("(@productIds0,@productIds1)"));
        }

        [Test]
        public void Can_add_array_of_objects_through_object_parameterization()
        {
            var parameter = _sqlCommand.Parameters["@orderIds0"].Value;
            var expectedValue = 1;

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Adding_array_of_objects_replaces_command_text_ParameterName_through_object_parameterization()
        {
            var sqlCommandText = _sqlCommand.CommandText;

            Assert.That(sqlCommandText, Does.Contain("(@orderIds0,@orderIds1)"));
        }
        [Test]
        public void Can_add_array_of_objects_through_dictionary_parameterization()
        {
            var parameter = _sqlCommand.Parameters["@userIds0"].Value;
            var expectedValue = 1;

            Assert.That(parameter, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Adding_array_of_objects_replaces_command_text_ParameterName_through_dictionary_parameterization()
        {
            var sqlCommandText = _sqlCommand.CommandText;

            Assert.That(sqlCommandText, Does.Contain("(@userIds0,@userIds1)"));
        }

        [Test]
        public void Can_add_generic_T_object_without_element_exclusion()
        {
            var sqlCommand = new SqlCommand();
            sqlCommand.AddParameters(new QueryParametersExtensionsAbstractTestObject());

            Assert.That(sqlCommand.Parameters.Contains($"@{nameof(QueryParametersExtensionsAbstractTestObject.ExclusionProperty)}"));
        }


        [Test]
        public void Can_add_generic_T_object_with_element_exclusion()
        {
            var sqlCommand = new SqlCommand();
            sqlCommand.AddParameters(new QueryParametersExtensionsAbstractTestObject(), getExcludedPropertyNames: obj => new HashSet<string>()
            {
                nameof(obj.ExclusionProperty)
            });

            Assert.That(sqlCommand.Parameters.Contains($"@{nameof(QueryParametersExtensionsAbstractTestObject.ExclusionProperty)}"), Is.Not.True);
        }

        [Test]
        public void Can_add_Alias_to_property_of_poco_when_running_stored_procedure()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(new QueryParametersExtensionsAbstractTestObject()); });

            Assert.That(sqlCommand.Parameters.Contains($"@{QueryParametersExtensionsAbstractTestObject.StoredProcedureAlias}"));
        }

        [Test]
        public void Does_not_add_property_name_when_specifying_parameter_names_for_storedProcs()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(new QueryParametersExtensionsAbstractTestObject()); });

            Assert.That(sqlCommand.Parameters.Contains($"@{nameof(QueryParametersExtensionsAbstractTestObject.AliasableProperty)}"), Is.Not.True);
        }
        [Test]
        public void Does_not_add_ignored_if_null_property_if_property_is_null()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(new QueryParametersExtensionsAbstractTestObject()); });

            Assert.That(sqlCommand.Parameters.Contains($"@{nameof(QueryParametersExtensionsAbstractTestObject.IgnoreIfNullProperty)}"), Is.Not.True);
        }

        [Test]
        public void Can_add_ignored_if_null_property_if_property_is_not_null()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            WrapWithTimer(() => {
                sqlCommand.AddStoredProcedureParameters(new QueryParametersExtensionsAbstractTestObject
                {
                    IgnoreIfNullProperty = "test"
                });
            });

            Assert.That(sqlCommand.Parameters.Contains($"@{QueryParametersExtensionsAbstractTestObject.StoredProcedureAliasIgnoreNull}"));
        }

        [Test]
        public void Does_not_add_non_included_properties_when_specifying_parameter_names_for_storedProcs()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(new QueryParametersExtensionsAbstractTestObject()); });

            Assert.That(sqlCommand.Parameters.Contains($"@{nameof(QueryParametersExtensionsAbstractTestObject.ExclusionProperty)}"), Is.Not.True);
        }

        [Test]
        public void Can_add_aliased_methods_with_no_required_signature_to_stored_procs()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var obj = new QueryParametersExtensionsAbstractTestObject();

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(obj); });

            Assert.That(sqlCommand.Parameters.Contains($"@{QueryParametersExtensionsAbstractTestObject.StoredProcedureMethodAlias}"));
        }

        [Test]
        public void Can_invoke_aliased_method_values_with_no_required_signature_to_stored_procs()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var obj = new QueryParametersExtensionsAbstractTestObject();

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(obj); });

            Assert.That(sqlCommand.Parameters[$"@{QueryParametersExtensionsAbstractTestObject.StoredProcedureMethodAlias}"].Value, Is.EqualTo(obj.GetAliasedMethodProperty()));
        }

        [Test]
        public void Can_add_methods_with_no_required_signature_to_stored_procs()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var obj = new QueryParametersExtensionsAbstractTestObject();

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(obj); });

            Assert.That(sqlCommand.Parameters.Contains($"@{nameof(QueryParametersExtensionsAbstractTestObject.GetMethodProperty)}"));
        }

        [Test]
        public void Can_invoke_method_values_with_no_required_signature_to_stored_procs()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var obj = new QueryParametersExtensionsAbstractTestObject();

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(obj); });

            Assert.That(sqlCommand.Parameters[$"@{nameof(QueryParametersExtensionsAbstractTestObject.GetMethodProperty)}"].Value, Is.EqualTo(obj.GetMethodProperty()));
        }

        [Test]
        public void Can_assign_default_value_if_member_value_is_null()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var obj = new QueryParametersExtensionsAbstractTestObject();

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(obj); });

            Assert.That(sqlCommand.Parameters[$"@{nameof(QueryParametersExtensionsAbstractTestObject.TestDefaultedProperty)}"].Value,
                Is.EqualTo(QueryParametersExtensionsAbstractTestObject.StoredProcedurePropertyNullDefaultConstantValue));
        }

        [Test]
        public void Can_assign_default_value_to_more_complex_data_type_if_member_value_is_null()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var obj = new QueryParametersExtensionsAbstractTestObject();

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(obj); });

            Assert.That(sqlCommand.Parameters[$"@{nameof(QueryParametersExtensionsAbstractTestObject.TestComplexDefaultedProperty)}"].Value,
                Is.EqualTo(QueryParametersExtensionsAbstractTestObject.StoredProcedureComplexPropertyNullDefaultValue));
        }

        [Test]
        public void Can_reuse_same_value_for_multiple_parameters()
        {
            var sqlCommand = new SqlCommand(QueryParametersExtensionsAbstractTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var obj = new QueryParametersExtensionsAbstractTestObject();

            WrapWithTimer(() => { sqlCommand.AddStoredProcedureParameters(obj); });

            Assert.That(sqlCommand.Parameters[$"@{QueryParametersExtensionsAbstractTestObject.DuplicateParameterName1}"].Value,
                Is.EqualTo(sqlCommand.Parameters[$"@{QueryParametersExtensionsAbstractTestObject.DuplicateParameterName2}"].Value));
        }


        [Test]
        public void Can_efficiently_add_many_store_procedure_parameters_via_reflection()
        {
            var sqlCommand = new SqlCommand(ExtraLargeQueryParametersExtensionsAbstractGlobalTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            var start = DateTime.Now;
            sqlCommand.AddStoredProcedureParameters(new ExtraLargeQueryParametersExtensionsAbstractGlobalTestObject());
            var end = DateTime.Now;

            var totalProcessingTime = end - start;
            Console.WriteLine($"Time to process large storedProcedureParameters: {totalProcessingTime.TotalMilliseconds} Milliseconds");

            Assert.That(totalProcessingTime.TotalMilliseconds < 30);
        }

        [Test]
        public void Can_efficiently_add_few_store_procedure_parameters_via_reflection()
        {
            var sqlCommand = new SqlCommand(ExtraLargeQueryParametersExtensionsAbstractGlobalTestObject.GlobalStoredProcedureName);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            var start = DateTime.Now;
            sqlCommand.AddStoredProcedureParameters(new QueryParametersExtensionsAbstractTestObject());
            var end = DateTime.Now;

            var totalProcessingTime = end - start;
            Console.WriteLine($"Time to process large storedProcedureParameters: {totalProcessingTime.TotalMilliseconds} Milliseconds");

            Assert.That(totalProcessingTime.TotalMilliseconds < 30);
        }
    }
}
