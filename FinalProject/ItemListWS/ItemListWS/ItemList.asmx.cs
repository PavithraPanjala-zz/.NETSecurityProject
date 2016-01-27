using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace ItemListWS
{
    /// <summary>
    /// Summary description for ItemList
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ItemList : System.Web.Services.WebService
    {

        MySqlConnection connection;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ListItems(int productId, string office, int officeId)
        {
            try
            {
                DbConnection dbConnection = new DbConnection();
                connection = dbConnection.getConnection();
                connection.Open();
                List<int> storeList = new List<int>();
                storeList = getChildStoreIds(officeId, office);

                string sqlQuery = "SELECT * FROM product WHERE product_id = " + productId;
                MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                MySqlDataReader sdr = command.ExecuteReader();

                string productName = "";
                string productDescription = "";

                while (sdr.Read())
                {
                    productName = sdr["product_name"].ToString();
                    productDescription = sdr["product_description"].ToString();
                }
                sdr.Close();


                sqlQuery = "SELECT * FROM store_product WHERE product_id =" + productId + " AND store_id IN(" + store(storeList) + ")";

                MySqlCommand newCommand = new MySqlCommand(sqlQuery, connection);
                sdr = newCommand.ExecuteReader();

                if (sdr.HasRows)
                {
                    var result = new List<inventory>();

                    while (sdr.Read())
                    {
                        result.Add(new inventory { productId = Int32.Parse(sdr["product_id"].ToString()), productName = productName, productDescription = productDescription, 
                            price = float.Parse(sdr["price"].ToString()), quantity = Int32.Parse(sdr["quantity"].ToString()), storeId=Int32.Parse(sdr["store_id"].ToString()), effectiveDate=sdr["effective_date"].ToString(),
                                                  effectivePrice=float.Parse(sdr["effective_price"].ToString()), discontinue=bool.Parse(sdr["discontinue"].ToString())});
                    }
                    sdr.Close();
                    return new JavaScriptSerializer().Serialize(result);
                }
            }
            catch (Exception ex)
            {
                return new JavaScriptSerializer().Serialize(ex.Message);
            }

            

            return "Hello World";
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

    public class inventory
    {
        public int storeId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public int quantity { get; set; }
        public float price { get; set; }
        public string effectiveDate { get; set; }
        public float effectivePrice { get; set; }
        public bool discontinue { get; set; }
    }
}
