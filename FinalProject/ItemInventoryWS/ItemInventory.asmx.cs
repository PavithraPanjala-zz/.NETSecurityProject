using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace ItemInventoryWS
{
    /// <summary>
    /// Summary description for ItemInventory
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ItemInventory : System.Web.Services.WebService
    {

        MySqlConnection connection;
        string sqlQuery;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String PriceChange(int productId, String office, int officeId)
        {
            try
            {
                DbConnection dbConnection = new DbConnection();
                connection = dbConnection.getConnection();
                connection.Open();
                List<int> storeList = new List<int>();
                storeList = getChildStoreIds(officeId, office);
                sqlQuery = "SELECT * FROM store JOIN (SELECT * FROM store_product WHERE product_id=" + productId + " AND store_id IN (" + 
                    store(storeList) + ")) sp WHERE store.store_id=sp.store_id";

                MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                MySqlDataReader sdr = command.ExecuteReader();

                return "123";
            }
            catch (Exception ex)
            {
                return new JavaScriptSerializer().Serialize(ex.Message);
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
