using System.Data;
using AdoProject;
using Microsoft.Data.SqlClient;


SqlSample sqlSample = new SqlSample();
sqlSample.AddTransaction("test",1,"note30","tttttt",500);
Console.ReadKey();
