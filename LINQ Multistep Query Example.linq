<Query Kind="Statements">
  <Connection>
    <ID>fa389fcc-ed69-4b31-8278-3ade764f17c3</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

// One needs to have processed information from a collection to use against the same collection
// The solution to this problem is to use multiple queries
// The early query(ies) will produce the needed information / criteria to execute against the same collection in a later query(ies)
// AKA pre-processing

// Display the employees that have the most customers to support
// Display the employee name and number of customers that the employee supports

// What is not wanted is a list of all employees sorted by number of customers supported
	
// One could create a list of all employees, with the customer support count, ordered in descending support count. But this is not requested
// Information needed:
// 1) Maximum number of customers that any particular employee is supported
// 2) Take that piece of data and compare to all employees

// Query one will generate data / information that will be used in the next query
// Get a list of employees and the count of customers each supports
// From that list, we can obtain the largest count
// Using the number, review all the employees and their counts, reporting only the busiest employees

var preProcessedEmployeeList = Employees
									.Select(e => new {
														Name = e.FirstName + " " + e.LastName,
														CustomerCount = e.SupportRepCustomers.Count()
													 }
											)
									//.Dump()
									;
									
//var highCount = preProcessedEmployeeList
//						.Max(e => e.CustomerCount)
//						.Dump()
//						;
//
//var busyEmployees = preProcessedEmployeeList
//						.Where(e => e.CustomerCount == highCount)
//						.Dump()
//						;
						
var busyEmployees = preProcessedEmployeeList
						.Where(e => e.CustomerCount == preProcessedEmployeeList.Max(e => e.CustomerCount))
						.Dump()
						;