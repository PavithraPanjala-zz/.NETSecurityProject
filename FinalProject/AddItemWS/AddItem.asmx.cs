using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace AddItemWS
{
    /// <summary>
    /// Summary description for ChangePrice
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AddItem : System.Web.Services.WebService
    {
        MySqlConnection connection;
        string sqlQuery;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Boolean ItemAdd(String productName, String productDescription, float price, int productId, String office, int officeId)
        {


            try
            {
                DbConnection dbConnection = new DbConnection();
                connection = dbConnection.getConnection();
                connection.Open();
                List<int> storeList = new List<int>();
                storeList = getChildStoreIds(officeId, office);
                //store(storeList);

                if (storeList.Count > 0)
                {
                    if (productId == 0)
                    {
                        sqlQuery = "INSERT INTO product (product_id, product_name, product_description) VALUES (NULL, '" + productName + "', '" + productDescription + "')";
                        MySqlCommand newCommand = connection.CreateCommand();
                        newCommand.CommandText = sqlQuery;

                        newCommand.ExecuteNonQuery();
                    }
                   foreach (int eachStore in storeList)
                    {
                        sqlQuery = "SELECT * FROM store_product WHERE product_id=" + productId + " AND store_id=" + eachStore;
                        MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                        MySqlDataReader sdr = command.ExecuteReader();

                        if (!sdr.HasRows)
                        {
                            sdr.Close();
                            sqlQuery = "INSERT INTO store_product (store_id, product_id, price) VALUES (" + eachStore + ", " + productId + ", " + price + ")";
                            MySqlCommand newCommand = connection.CreateCommand();
                            newCommand.CommandText = sqlQuery;

                            newCommand.ExecuteNonQuery();
                        }
                            
                        sdr.Close();
                        
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
                //return new JavaScriptSerializer().Serialize(ex.Message); ;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<int> getChildStoreIds(int storeId, string office)
        {
            List<int> storeList = new List<int>();
            String sqlQuery = "";
            if (office == "Zonal")
            {
                sqlQuery = "SELECT store_id FROM store WHERE zoid = " + storeId;
            }
            else if (office == "Regional")
            {
                sqlQuery = "SELECT rs.store_id FROM zonal_office z JOIN store rs ON(rs.zoid = z.zoid) WHERE z.roid = " + storeId;
            }
            else if (office == "National")
            {
                sqlQuery = "select store_id from regional_office re join zonal_office z on(z.roid = re.roid) join store rs on(rs.zoid = z.zoid) where re.noid = " + storeId;
            }
            else
            {
                storeList.Add(storeId);
                return storeList;
            }

            MySqlCommand command = new MySqlCommand(sqlQuery, connection);
            MySqlDataReader sdr = command.ExecuteReader();

            while (sdr.Read())
            {
                storeList.Add(Int32.Parse(sdr["store_id"].ToString()));
            }
            sdr.Close();

            return storeList;
        }

        public String store(List<int> storeList)
        {
            String queryStoreList = "";

            foreach (int store in storeList)
            {
                queryStoreList = queryStoreList + "," + store;
            }
            queryStoreList = queryStoreList.Substring(1, queryStoreList.Length - 1);

            return queryStoreList;
        }
    }
}
