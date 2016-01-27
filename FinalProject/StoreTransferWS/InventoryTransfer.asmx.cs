using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace StoreTransferWS
{
    /// <summary>
    /// Summary description for StoreTransfer
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class StoreTransfer : System.Web.Services.WebService
    {

        MySqlConnection connection;
        string sqlQuery;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String TransferToStore(int productId, int quantity, int fromStoreId, int toStoreId, float price)
        {
            try
            {
                int currentQuantityFromStore = 0;
                int currentQuantityToStore = 0;
                DbConnection dbConnection = new DbConnection();
                connection = dbConnection.getConnection();
                connection.Open();

                sqlQuery = "SELECT quantity FROM store_product WHERE product_id = " + productId + " AND store_id = " + fromStoreId;

                
                MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                MySqlDataReader sdr = command.ExecuteReader();
                if (sdr.Read())
                {
                    currentQuantityFromStore = Int32.Parse(sdr["quantity"].ToString());
                    if (quantity > currentQuantityFromStore)
                    {
                        return "234";
                    }
                }

                sdr.Close();

                sqlQuery = "SELECT quantity FROM store_product WHERE product_id = " + productId + " AND store_id = " + toStoreId;

                MySqlCommand fromCommand = new MySqlCommand(sqlQuery, connection);
                sdr = fromCommand.ExecuteReader();

                if(sdr.Read())
                {
                    currentQuantityToStore = Int32.Parse(sdr["quantity"].ToString());
                }
                sdr.Close();

                sqlQuery = "UPDATE store_product SET quantity=" + (currentQuantityFromStore - quantity) + " WHERE store_id=" + fromStoreId + " AND product_id=" + productId;
                MySqlCommand newCommand = new MySqlCommand(sqlQuery, connection);
                newCommand.ExecuteNonQuery();

                sqlQuery = "SELECT * FROM store_product WHERE store_id=" + toStoreId + " AND product_id=" + productId;
                MySqlCommand checkCommand = new MySqlCommand(sqlQuery, connection);
                sdr = checkCommand.ExecuteReader();
                if(sdr.HasRows)
                {
                    sqlQuery = "UPDATE store_product SET quantity=" + (currentQuantityToStore + quantity) + " WHERE store_id=" + toStoreId + " AND product_id=" + productId;
                }
                else
                {
                    sqlQuery = "INSERT INTO store_product (store_id, product_id, quantity, price) VALUES (" + toStoreId + ", " + productId + ", " + quantity + ", " + price + ")";
                }
                sdr.Close();
                
                MySqlCommand newNewCommand = new MySqlCommand(sqlQuery, connection);
                newNewCommand.ExecuteNonQuery();
                return "123";
            }
            catch (Exception ex)
            {
                return new JavaScriptSerializer().Serialize(ex.Message);
                //return false;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
