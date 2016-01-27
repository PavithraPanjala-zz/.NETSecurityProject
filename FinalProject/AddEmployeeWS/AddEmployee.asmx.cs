using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace AddEmployeeWS
{
    /// <summary>
    /// Summary description for AddEmployee
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AddEmployee : System.Web.Services.WebService
    {

        MySqlConnection connection;
        string sqlQuery;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Boolean EmployeeAdd(string name, string ssn, string address, string gender, string jobTitle, float salary, string worksFor, int officeId, string startDate, string username, string password)
        {
            try
            {
                DbConnection dbConnection = new DbConnection();
                connection = dbConnection.getConnection();
                string dateFormat = DateTime.Now.ToString("yyyy-MM-dd");
                sqlQuery = "INSERT INTO employee (name, ssn, address, gender, job_title, salary, works_for, office_id, start_date, username, password) VALUES ('" + name + "', '" +
                    ssn + "', '" + address + "', '" + gender + "', '" + jobTitle + "', " + salary + ", '" + worksFor + "', " + officeId + ", '" + startDate + "', '" + username + "', '" + 
                    password + "')";
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = sqlQuery;
                connection.Open();
                int response = command.ExecuteNonQuery();
                if (response > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
