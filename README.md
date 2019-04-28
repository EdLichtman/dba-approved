# dba-approved

## dba-approved is a micro-orm created alongside requests from a dba to prioritize Stored Procedures for Security and Scalability. 
* First and foremost it uses less code to query from the database.
* There are benefits "unlocked" by using stored procedures
* * These benefits can be seen by using StoredProcedure attribute that inherits IStoredProcedureParameters. It allows you to specify specific properties and methods to be auto generated against a call to a stored procedure that matches a particular name.

These are some of the points I'll be getting across -- Will think of better descriptions once this starts to become public

Future goals: 
1) I'd like to be able to parse enumerables into a class automatically without parsing each individual property
2) I'd like to be able to parse tables into classes with normalized lists automatically without parsing each data set.
3) I'd like to rename library to "nanOrm" to represent that it's a nano-orm.
4) I'd like to create a IViewQueryParameters, that has GetAppendedSql(), as well as GetFilterParameters. 
4) * IViewQueryParameters should probably have attributes such as the attributes used to assign values to their parsed equivalents
4) * * i.e. if getTransaction has a return value "Cost", then the TransactionPoco should have a property "Cost" with an attribute parameter mapping: \[Column("Cost")\]. IViewQueryParameters should implement same logic, but on Fetch it will reflect upon the properties that exist on the poco and form the Select Query from them.
4) * IViewQueryParameters should have View attribute with name of View.
4) * IViewQueryParameters should have a function "SetAppendedSql(string sql)" or something that then sets Where, Group By, Having, Order by, etc. It looks like if we want to be able to use complicated concepts like Coalesce, then we can't have our cake and eat it too by auto-generating C# code or by somehow parsing out where clauses. In addition, if we have a Column attribute we should probably have a ViewQueryParameter Attribute, with an ignoreIfNull flag, and also should only add if it detects @parameter in the appendedSql