<Query Kind="Program">
  <Connection>
    <ID>6cd0a226-8c31-42ad-a971-381fcc6d904b</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

void Main()
{
	// Nested Queries: A query within a query [...]
	// Sometimes referred to as SubQueries
	
	// List all sales support employees showing their FullName (last, first), Title, Phone
	// For each employee, show a list of customers they support
	// Show the customer FullName(last, first), City, State
	
	// Employee 1, Title, Phone
	// 		Customer 2000, City, State
	//		Customer 2001, City, State
	//		Customer 2002, City, State
	// Employee 2, Title, Phone
	// 		Customer 2020, City, State
	//		Customer 2021, City, State
	//		Customer 2022, City, State
	
	// There appears to be two seperate lists that need to be within one final data set collection
	// Need: 
	// 		 A list of employees
	// 		 A list of customers
	
	// Concern: the lists are intermixed
	
	// C# point of view in a class definition
	// This is a composite class
	// The class is describing an employee
	// Each instance of the employee will have a list of employeeCustomers
	
	// Class: EmployeeList
	// Properties: FullName, Title, Phone, CollectionOfCustomers (List<T>)
	
	// Class: CustomerList
	// Properties: FullName, City, State
	
	var results = Employees
					.Where (e => e.Title.Contains("Sales Support"))
					.Select (e => new EmployeeItem
										{
											FullName = e.LastName + ", " + e.FirstName,
											Title = e.Title,
											Phone = e.Phone,
											CustomerList = e.SupportRepCustomers
															.Select(c => new CustomerItem
																				{
																					FullName = c.LastName + ", " + c.FirstName,
																					City = c.City,
																					State = c.State
																				}
																	)
										}
							);
	results.Dump();
	
	// List all Albums that are from 1990
	// Display the AlbumTitle and Artist Name
	// For each Albumn display its Tracks
	var nineties = Albums
					.Where (a => a.ReleaseYear == 1990)
					.Select (a => new 
									{
										Title = a.Title,
										Artist = a.Artist.Name,
										Tracks = a.Tracks
													.Select(t => new 
																	{
																		Song = t.Name,
																		Genre = t.Genre.Name
																	}
															)
									}
							);
	nineties.Dump();
}

public class CustomerItem
{
	public string FullName {get; set;}
	public string City {get; set;}
	public string State {get; set;}
}

public class EmployeeItem
{
	public string FullName {get; set;}
	public string Title {get; set;}
	public string Phone {get; set;}
	public IEnumerable<CustomerItem> CustomerList {get; set;}
}
