using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace RemoveEmployeeWS
{
    /// <summary>
    /// Summary description for RemoveEmployee
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RemoveEmployee : System.Web.Services.WebService
    {

        MySqlConnection connection;
        string sqlQuery;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Boolean EmployeeRemove(int employeeId)
        {
            try
            {
                DbConnection dbConnection = new DbConnection();
                connection = dbConnection.getConnection();
                string dateFormat = DateTime.Now.ToString("yyyy-MM-dd");
                sqlQuery = "UPDATE employee SET end_date = '" + dateFormat + "' WHERE employee_id = " + employeeId;
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
