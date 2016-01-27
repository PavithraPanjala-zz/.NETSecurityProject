using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace RetailStoreWebServices
{
    /// <summary>
    /// Summary description for LoginWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class LoginWebService : System.Web.Services.WebService
    {
        MySqlConnection connection;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Login(string userName, string password)
        {
            DbConnection dbConnection = new DbConnection();
            connection = dbConnection.getConnection();
            int userRole = 0;
            try
            {
                connection.Open();
                string sqlQuery = "SELECT * FROM employee WHERE username = '" + userName + "' and password = '" + password + "'";
                MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                MySqlDataReader sdr = command.ExecuteReader();
                
                if(sdr.HasRows)
                {
                    if (sdr.Read())
                    {
                        var result = new { employeeId = sdr["employee_id"].ToString(), worksFor=sdr["works_for"].ToString(), officeId=sdr["office_id"].ToString()};
                        return new JavaScriptSerializer().Serialize(result);
                    }
                }
            }
            catch (Exception ex)
            {
                var result = new { result="connection failed"};
                return new JavaScriptSerializer().Serialize(result);
            }

            if (userRole != 0)
            {
                var result = new { result = "success", UserRole = userRole};
                return new JavaScriptSerializer().Serialize(result);
            }
            else
            {
                var result = new { result = "error", message = "LoginFailed"};
                return new JavaScriptSerializer().Serialize(result);
            }
              
            return "Hello World";
        }
    }
}
