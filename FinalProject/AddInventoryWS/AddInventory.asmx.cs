using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace AddInventoryWS
{
    /// <summary>
    /// Summary description for AddInventory
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AddInventory : System.Web.Services.WebService
    {

        MySqlConnection connection;
        string sqlQuery;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Boolean InventoryAdd(int storeId, int productId, int quantity)
        {
            try
            {
                DbConnection dbConnection = new DbConnection();
                connection = dbConnection.getConnection();
                connection.Open();

                sqlQuery = "SELECT quantity FROM store_product WHERE store_id=" + storeId + " AND product_id=" + productId;
                MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                MySqlDataReader sdr = command.ExecuteReader();
                if (sdr.Read())
                {
                    int newQuantity = quantity + Int32.Parse(sdr["quantity"].ToString());
                    sqlQuery = "UPDATE store_product SET quantity = " + newQuantity + " WHERE store_id=" + storeId + " AND product_id=" + productId;
                }
                else
                {
                    sqlQuery = "INSERT INTO store_product (store_id, product_id, quantity) VALUES (" + storeId + "," + productId + "," + quantity + ")";
                }
                sdr.Close();
                MySqlCommand newCommand = connection.CreateCommand();
                newCommand.CommandText = sqlQuery;
                int response = newCommand.ExecuteNonQuery();
                if (response > 0)
                {

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {

                //var result = new { result = "error", message = "LoginFailed" };
                //return new JavaScriptSerializer().Serialize(ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
