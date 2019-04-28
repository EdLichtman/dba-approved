# dba-approved

## dba-approved is a micro-orm created alongside requests from a dba to prioritize Stored Procedures for Security and Scalability. 
* First and foremost it uses less code to query from the database.
* There are benefits "unlocked" by using stored procedures
* * These benefits can be seen by using StoredProcedure attribute that inherits IStoredProcedureParameters. It allows you to specify specific properties and methods to be auto generated against a call to a stored procedure that matches a particular name.

These are some of the points I'll be getting across -- Will think of better descriptions once this starts to become public

Future goals: 
1) I'd like to use this with best practices in mind. A Stored Procedure Object should only contain one stored procedure. Each Stored Procedure should abide by "Single Responsibility Principle".
2) I'd like to be able to parse enumerables into a class automatically without parsing each individual property
3) I'd like to be able to parse tables into classes with normalized lists automatically without parsing each data set.
4) I'd like to separate out the IDatabaseConnection WrapQuery functionality from IDatabaseFacade so I can more effectively Unit Test
5) I'd like to implement a Core Library alongside a Sqlite, MSSQL, MySql, DB2, PostGres, etc database. This is a nano-orm, in the sense that it just wraps the database query or stored proc execution, but doesn't have features like "insert, delete, etc" like npoco has