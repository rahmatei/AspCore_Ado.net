using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoProject
{
    public class SqlSample
    {
        private SqlConnection sqlConnection;
        public SqlSample()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.InitialCatalog = "OnlineShopDb";
            builder.DataSource = ".";
            builder.Password = "12345678";
            builder.UserID = "sa";
            builder.Encrypt = false;
            builder.TrustServerCertificate = true;
            builder.CommandTimeout = 100;

            sqlConnection = new SqlConnection(builder.ConnectionString);
        }

        public void FirstSample()
        {
            string connectionString = "Server=.;Initial catalog=OnlineShopDb;User Id=sa; Password=12345678; TrustServerCertificate = True";
            SqlConnection connection = new SqlConnection(connectionString);

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "select * from Categories";
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"ID:{reader["ID"]}\t\t Name:{reader["CategoryName"]}");

            }
            connection.Close();
        }

        public void ConnectionBuilder()
        {
            SqlConnectionStringBuilder builder=new SqlConnectionStringBuilder();
            builder.InitialCatalog = "OnlineShopDb";
            builder.DataSource = ".";
            builder.Password = "12345678";
            builder.UserID = "sa";
            builder.Encrypt = false;
            builder.TrustServerCertificate = true;
            builder.CommandTimeout = 100;

            SqlConnection sqlConnection = new SqlConnection(builder.ConnectionString);
            sqlConnection.Open();
            Console.WriteLine(sqlConnection.Database);
            Console.WriteLine(sqlConnection.DataSource);
            Console.WriteLine(sqlConnection.CommandTimeout);
            sqlConnection.Close();
        }


        public void TestCommand()
        {
            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
                CommandText = "select * from Categories"
            };
        sqlConnection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"ID:{reader["ID"]}\t\t Name:{reader["CategoryName"]}");

            }
            sqlConnection.Close();
        }

        public void TestReader()
        {
            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
                CommandText = "select * from Categories"
            };
            sqlConnection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write(reader.GetName(i));
                    Console.Write(":");
                    Console.Write(reader.GetValue(i));
                    Console.Write("\t");
                }
                Console.WriteLine();
            }
            sqlConnection.Close();
        }
        public void TestReaderMultiple()
        {
            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
                CommandText = "select * from Categories; select * from Products"
            };
            sqlConnection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            do
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader.GetName(i));
                        Console.Write(":");
                        Console.Write(reader.GetValue(i));
                        Console.Write("\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("".PadLeft(100, '-'));
            } while (reader.NextResult());
            sqlConnection.Close();
        }
        public void AddProduct (int categoryId,string productName,string description,int price)
        {
            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
                CommandText = $"insert into Products (CategoryId,ProductName,Description,Price) values ({categoryId},'{productName}','{description}',{price})"
            };
            sqlConnection.Open();
            int result =sqlCommand.ExecuteNonQuery();

            Console.WriteLine($"Affected Row {result}");
        }
        public void AddProductWithParameter(int categoryId, string productName, string description, int price)
        {
            SqlParameter categoryParam = new SqlParameter
            {
                ParameterName = "@categoryId",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Input,
                Value = categoryId
            };
            SqlParameter productNameParam = new SqlParameter
            {
                ParameterName = "@productName",
                DbType = DbType.String,
                Direction = ParameterDirection.Input,
                Value = productName
            };
            SqlParameter descriptionParam = new SqlParameter
            {
                ParameterName = "@description",
                DbType = DbType.String,
                Direction = ParameterDirection.Input,
                Value = description
            };
            SqlParameter priceParam = new SqlParameter
            {
                ParameterName = "@price",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Input,
                Value = price
            };
            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
                CommandText = $"insert into Products (CategoryId,ProductName,Description,Price) values (@categoryId,@productName,@description,@price)"
            };
            sqlCommand.Parameters.Add(categoryParam);
            sqlCommand.Parameters.Add(productNameParam);
            sqlCommand.Parameters.Add(descriptionParam);
            sqlCommand.Parameters.Add(priceParam);
            sqlConnection.Open();
            int result = sqlCommand.ExecuteNonQuery();
            Console.WriteLine($"Affected Row {result}");
        }

        public void AddTransaction(string categoryName, int categoryId, string productName, string description, int price)
        {
            SqlTransaction sqlTransaction = null;


            SqlCommand sqlCommandCategory = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
                CommandText = $"insert into Categories (categoryName) values ('{categoryName}')"
            };

            SqlCommand sqlCommandProduct = new SqlCommand
            {
                Connection = sqlConnection,
                CommandType = CommandType.Text,
                CommandText = $"insert into Products (CategoryId,ProductName,Description,Price) values ({categoryId},'{productName}','{description}',{price})"
            };
            sqlConnection.Open();
            try
            {
                sqlTransaction = sqlConnection.BeginTransaction();

                int result = sqlCommandProduct.ExecuteNonQuery();
                result += sqlCommandCategory.ExecuteNonQuery();
                sqlTransaction.Commit();

                Console.WriteLine($"Affected Row {result}");
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                sqlTransaction.Rollback();
                sqlConnection.Close();
            }
        }
    }
}
