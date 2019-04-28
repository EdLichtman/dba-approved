using dba_approved_core.Helpers;
using dba_approved_core.QueryLayer.Parameters.StoredProcedures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace dba_approved_core.QueryLayer.Parameters
{
    /// <summary>
    /// Utility Library to help apply SqlParameters to SqlCommands
    /// </summary>
    public static class QueryParameterExtensions
    {
        /// <summary>
        /// Adds SqlParameters to a SqlCommand. By using a dictionary of values you can dynamically populate an object, and then apply it to your query.
        /// 
        /// This method accepts arrays and expected null values. If you give it an array it will replace the parameter in the SqlCommand text.
        /// You may choose whether or not to prepend @ to your parameter key, it will make no difference.
        ///
        /// Note: The Dictionary must be {string,object} or else the C# compiler will cast it as an anonymous object.
        /// </summary>
        /// <param name="command">The SqlCommand to which you are applying these parameters</param>
        /// <param name="values">The KeyValuePairs which are to parameterize your SqlCommand</param>
        public static void AddParameters(this SqlCommand command, IDictionary<string, object> values)
        {
            if (values == null)
                return;
            foreach (var parameter in values)
            {
                var key = (parameter.Key.StartsWith("@")) ? parameter.Key : $"@{parameter.Key}";

                if (parameter.Value is IList)
                {
                    var parameterValues = ((IList)parameter.Value).Cast<object>().ToList();
                    command.AddParameterArray(key, parameterValues);
                }
                else
                    command.AddParameter(key, parameter.Value);


            }
        }

        /// <summary>
        /// Adds SqlParameters to a SqlCommand. By using an object of anonymous type the C# compiler will accept implicitly named properties.
        ///
        /// This method accepts arrays and expected null values. If you give it an array it will replace the parameter in the SqlCommand text.
        /// You may choose whether or not to prepend @ to your parameter key, it will make no difference.
        /// 
        /// Note: If you are attempting a dictionary that is not {string,object} the C# compiler will cast it as an anonymous object.
        /// If you are parameterizing a dictionary make sure it is {string,object}.
        /// </summary>
        /// <param name="command">The SqlCommand to which you are applying these parameters</param>
        /// <param name="values">The anonymous object which are to parameterize your SqlCommand</param>
        public static void AddParameters(this SqlCommand command, object values)
        {
            if (values == null)
                return;

            AddParameters(command, values.AsDictionary());
        }
        /// <summary>
        /// Adds SqlParameters to a SqlCommand. Uses Generic(T) class. By using a strongly-typed class we can list exclusions and re-used an already-created poco.
        ///
        /// This method accepts arrays and expected null values. If you give it an array it will replace the parameter in the SqlCommand text.
        /// You may choose whether or not to prepend @ to your parameter key, it will make no difference.
        /// 
        /// </summary>
        /// <param name="command">The SqlCommand to which you are applying these parameters</param>
        /// <param name="values">The poco object which is to parameterize your SqlCommand</param>
        /// <param name="getExcludedPropertyNames">A function that will allow you to specify propertyNames (using nameof() in c#6) so that you can use an already built object without creating a new one.</param>
        public static void AddParameters<T>(this SqlCommand command, T values, Func<T, HashSet<string>> getExcludedPropertyNames = null)
        {
            if (values == null)
                return;

            var propertyNameExclusions = new HashSet<string>();

            var isParameterObjectWithExclusions = getExcludedPropertyNames != null;


            if (isParameterObjectWithExclusions)
            {
                var objectForProperties = (T)Activator.CreateInstance<T>();
                propertyNameExclusions = getExcludedPropertyNames(objectForProperties);
            }

            var parameters = values.AsDictionary();

            foreach (var excludedProperty in propertyNameExclusions)
            {
                if (parameters.ContainsKey(excludedProperty))
                    parameters.Remove(excludedProperty);
            }

            AddParameters(command, parameters);
        }

        /// <summary>
        /// Adds SqlParameters to a SqlCommand of CommandType "StoredProcedure". Uses Generic(T) class. By using a strongly-typed class and operating with "IStoredProcedureParameters",
        /// we can list inclusions and re-use an already-created poco, potentially across multiple stored procedures.
        /// 
        /// </summary>
        /// <param name="command">The SqlCommand to which you are applying these parameters</param>
        /// <param name="values">The poco object which is to parameterize your SqlCommand</param>
        public static void AddStoredProcedureParameters<T>(this SqlCommand command, T values) where T : IStoredProcedureParameters
        {
            if (values == null)
                return;

            var parameters = new Dictionary<string, object>();
            var isQueryStoredProcedure = command.CommandType == CommandType.StoredProcedure;

            if (!isQueryStoredProcedure)
                throw new InvalidOperationException("StoredProcedureParameters must be operating on a stored procedure command type.");

            var objectForValues = (T)Activator.CreateInstance<T>();

            var objectType = objectForValues.GetType();

            var globalStoredProcedureAttribute =
                (StoredProcedureAttribute)Attribute.GetCustomAttribute(objectType, typeof(StoredProcedureAttribute));

            if (globalStoredProcedureAttribute == null)
                throw new InvalidOperationException("StoredProcedure Attribute is required to append stored procedure parameters.");

            var globalStoredProcedureName = globalStoredProcedureAttribute.StoredProcedureName;



            var storedProcedureAttributeProperties = objectType.GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(StoredProcedureParameterAttribute), false));

            var storedProcedureName = command.CommandText;

            var specifiedDefaults = (values.GetDefaults() ?? new List<DefaultStoredProcedureParameterValue>())
                .Where(elem => elem.StoredProcedureName == storedProcedureName)
                .ToList();


            foreach (var storedProcedureAttributeProperty in storedProcedureAttributeProperties)
            {
                var additions = GetStoredProceduresParametersFromMember(
                    storedProcedureName,
                    globalStoredProcedureName,
                    storedProcedureAttributeProperty,
                    () => storedProcedureAttributeProperty.GetValue(values),
                    specifiedDefaults);

                foreach (var addition in additions)
                    parameters.Add(addition.Key, addition.Value);
            }

            var storedProcedureAttributeMethods = objectType.GetMethods()
                .Where(prop => Attribute.IsDefined(prop, typeof(StoredProcedureParameterAttribute), false));

            foreach (var storedProcedureAttributeMethod in storedProcedureAttributeMethods)
            {
                var additions = GetStoredProceduresParametersFromMember(
                    storedProcedureName,
                    globalStoredProcedureName,
                    storedProcedureAttributeMethod,
                    () => storedProcedureAttributeMethod.Invoke(values, null),
                    specifiedDefaults);

                foreach (var addition in additions)
                    parameters.Add(addition.Key, addition.Value);
            }

            AddParameters(command, parameters);


        }

        private static IDictionary<string, object> GetStoredProceduresParametersFromMember(
            string storedProcedureName,
            string globalStoredProcedureName,
            MemberInfo member,
            Func<object> getValueFunction,
            IList<DefaultStoredProcedureParameterValue> defaultsValues)
        {

            var attributes = Attribute.GetCustomAttributes(member);
            var parameters = new Dictionary<string, object>();
            foreach (var attribute in attributes)
            {
                if (attribute is StoredProcedureParameterAttribute)
                {
                    var attributeWithData = (StoredProcedureParameterAttribute)attribute;
                    if (storedProcedureName == globalStoredProcedureName)
                    {
                        var value = getValueFunction();
                        var isValueNull = value == null;
                        var isIgnored = false;
                        if (attributeWithData.IgnoreIfNull)
                            isIgnored = isValueNull;
                        else if (isValueNull)
                        {
                            value = attributeWithData.DefaultValueIfNull
                                    ?? defaultsValues.FirstOrDefault(elem => elem.MemberName == member.Name)?.DefaultValue;
                        }
                        if (!isIgnored)
                        {
                            if (!string.IsNullOrEmpty(attributeWithData.StoredProcedureParameterAlias))
                                parameters.Add(attributeWithData.StoredProcedureParameterAlias, value);
                            else
                                parameters.Add(member.Name, value);
                        }
                    }

                }
            }

            return parameters;
        }

        /// <summary>
        /// Add an array of SqlParameters to a SqlCommand. It will replace the key in the query you provide with an array of parameters and 
        /// then append the SqlParameters.
        ///
        /// You may choose whether or not to prepend @ to your parameter key, it will make no difference.
        /// </summary>
        /// <param name="command">The SqlCommand to which you are applying these parameters</param>
        /// <param name="parameterName">The name of the parameter you wish to replace in your SQL Text. @ optional</param>
        /// <param name="objects">The array of objects which you are parameterizing</param>
        public static void AddParameterArray(this SqlCommand command, string parameterName, IList<object> objects)
        {
            if (objects == null)
                return;
            var parametersNames = new List<string>();
            for (var i = 0; i < objects.Count; i++)
            {
                var newParameterName = $"{parameterName}{i}";
                parametersNames.Add(newParameterName);
                AddParameter(command, newParameterName, objects[i]);
            }

            command.CommandText = command.CommandText.Replace(parameterName, string.Join(",", parametersNames));
        }

        /// <summary>
        /// Add a SqlParameter to a SqlCommand. If you pass in null it will attempt a DBNULL
        /// </summary>
        /// <param name="command">The SqlCommand to which you are applying this parameter</param>
        /// <param name="name">The name of the parameter. @ optional</param>
        /// <param name="value">The value of the parameter</param>
        public static void AddParameter(this SqlCommand command, string name, object value)
        {
            var key = (name.StartsWith("@")) ? name : $"@{name}";
            command.Parameters.AddWithValue(key, value ?? DBNull.Value);
        }

        /// <summary>
        /// Add a SqlParameter to a SqlCommand. If you pass in null it will not attempt to add anything
        /// </summary>
        /// <param name="command">The SqlCommand to which you are applying this parameter</param>
        /// <param name="name">The name of the parameter. @ optional</param>
        /// <param name="value">The value of the parameter</param>
        public static void AddParameterIfNotNull(this SqlCommand command, string name, object value)
        {
            if (value != null)
            {
                var key = (name.StartsWith("@")) ? name : $"@{name}";
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    command.Parameters.AddWithValue(key, value);
            }
        }
    }
}
