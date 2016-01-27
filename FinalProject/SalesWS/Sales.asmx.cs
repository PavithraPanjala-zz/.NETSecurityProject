using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace SalesWS
{
    /// <summary>
    /// Summary description for Sales
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Sales : System.Web.Services.WebService
    {

        MySqlConnection connection;
        string sqlQuery;
        int currentQuantity;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Sale(int storeId, int productId, int quantity, int employeeId)
        {


            try
            {
                DbConnection dbConnection = new DbConnection();
                connection = dbConnection.getConnection();
                DateTime saleDate = DateTime.Now;

                sqlQuery = "SELECT * FROM store_product WHERE product_id = " + productId + " AND store_id = " + storeId;
                
                connection.Open();
                MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                MySqlDataReader sdr = command.ExecuteReader();
                if (sdr.Read())
                {
                    currentQuantity = Int32.Parse(sdr["quantity"].ToString());
                    if (quantity > currentQuantity)
                    {
                        return "Insufficient Quantity";
                    }
                    if ((Boolean)sdr["discontinue"])
                    {
                        return "Product Discontinued";
                    }
                }

                sdr.Close();
                sqlQuery = "INSERT INTO sales (store_id, product_id, quantity, sale_date, employee_id) VALUES (" + storeId + ", " + productId + ", " + quantity + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "'," + employeeId + ")";
                MySqlCommand newCommand = new MySqlCommand(sqlQuery, connection);
                int response = newCommand.ExecuteNonQuery();
                if (response > 0)
                {
                    int newQuantity = currentQuantity - quantity;
                    sqlQuery = "UPDATE store_product SET quantity = " + newQuantity + " WHERE store_id=" + storeId + " AND product_id=" + productId;
                    MySqlCommand newNewCommand = new MySqlCommand(sqlQuery, connection);
                    newNewCommand.ExecuteNonQuery();
                    return "Success";
                }
                return "Failure";
               
            }
            catch (Exception ex)
            {
                //return new JavaScriptSerializer().Serialize(ex.Message);
                return ex.Message;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
