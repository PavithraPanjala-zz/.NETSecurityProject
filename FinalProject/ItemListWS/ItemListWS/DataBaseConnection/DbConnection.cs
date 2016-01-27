using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for DbConnection
/// </summary>
public class DbConnection
{
    MySqlConnection connection;
    
    string connectionString =
       "Data Source=handson-mysql;Database=cs725; User Id=kahmed; Password='admin'";
    string connectionStringCS =
       "Data Source=rmysql.cs.odu.edu;Database=ppanjal; User Id=ppanjal; Password='g3e7mrQw'";
    
    public DbConnection()
	{
        
	}
    public MySqlConnection getConnection()
    {
        using (connection = new MySqlConnection(connectionStringCS))
        {
            // Response.Write("MySql connection Succesfull");
            return connection;
        }
    }

    public void  closeConnection()
    {
        connection.Close();

    }
}