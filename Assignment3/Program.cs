// See https://aka.ms/new-console-template for more information


using Microsoft.Data.SqlClient;

Console.WriteLine("Hello, World!");

var cmdText = @"SELECT c.Name, c.Species, a.Name as Actor
                FROM Characters AS c
                JOIN Actors AS a ON c.ActorId = a.Id
                ORDER BY c.Name";

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var connectionString = configuration.GetConnectionString("Kanban");
//Console.WriteLine(connectionString);

using var connection = new SqlConnection(connectionString);
using var command = new SqlCommand(cmdText, connection);

connection.Open();
Console.WriteLine("hej");

using var reader = command.ExecuteReader();
connection.Close();