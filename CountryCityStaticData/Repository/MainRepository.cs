using CountryCityStaticData.Interface;
using CountryCityStaticData.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CountryCityStaticData
{
    internal class MainRepository
    {
        private readonly IDBConnection _db;

        public MainRepository(IDBConnection db)
        {
            _db = db;
        }
        #region Country / City Static Data
        public void GetCountryCityDataAPI()
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
                string responseCountry = CallApi(apiUrl + "countries");
                if (!string.IsNullOrEmpty(responseCountry))
                {
                    var result = JsonConvert.DeserializeObject<CountryCity>(responseCountry);
                    if (result.error == false)
                    {
                        if (result.data != null)
                        {
                            int srNo = 0;
                            DataTable dt = createCountryCityTable();
                            for (int i = 0; i < result.data.Count; i++)
                            {
                                DataRow drCountry = dt.NewRow();
                                drCountry["SrNo"] = ++srNo;
                                drCountry["GroupName"] = "country";
                                drCountry["GroupValue"] = result.data[i].country;
                                drCountry["hasChild"] = result.data[i].cities.Count > 0 ? 1 : 0;
                                drCountry["ParentId"] = 0;
                                drCountry["iso2"] = result.data[i].iso2;
                                drCountry["iso3"] = result.data[i].iso3;
                                dt.Rows.Add(drCountry);
                                DataTable dtCountry = AddCountryCityStaticData(dt);
                                dt.Clear();
                                int index = 0;
                                if (dtCountry != null && dtCountry.Rows.Count > 0)
                                {
                                    index = Convert.ToInt32( dtCountry.Rows[0]["Id"]);
                                }
                                if (result.data[i].cities != null && result.data[i].cities.Count > 0 )
                                {
                                    int srNo1 = 0;
                                    List<string> cityList = result.data[i].cities;
                                    for (int j = 0; j < cityList.Count; j++)
                                    {
                                        DataRow drCities = dt.NewRow();
                                        drCities["SrNo"] = ++srNo1;
                                        drCities["GroupName"] = "city";
                                        drCities["GroupValue"] = cityList[j];
                                        drCities["hasChild"] =  0;
                                        drCities["ParentId"] = index;
                                        dt.Rows.Add(drCities);
                                    }
                                    DataTable dtCities = AddCountryCityStaticData(dt);
                                }
                                dt.Clear();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private DataTable AddCountryCityStaticData(DataTable dt)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                new SqlParameter("@countyCityData", dt)
                };
                DataTable dt1 = _db.GetDataTableSync("AddUpdateCountryCityData", para);
                return dt1;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private DataTable createCountryCityTable()
        {
            DataTable dt  = new DataTable();
            dt.Columns.Add("SrNo", typeof(int));
            dt.Columns.Add("GroupName", typeof(string));
            dt.Columns.Add("GroupValue", typeof(string));
            dt.Columns.Add("hasChild", typeof(int));
            dt.Columns.Add("ParentId", typeof(int));
            dt.Columns.Add("iso2", typeof(string));
            dt.Columns.Add("iso3", typeof(string));
            return dt;
        }

        #endregion

        #region Country Code 

        internal void GetCountryCodeDataAPI()
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
                string responseCountry = CallApi(apiUrl + "countries/codes");
                if (!string.IsNullOrEmpty(responseCountry))
                {
                    var result = JsonConvert.DeserializeObject<CountryCode>(responseCountry);
                    if (result.error == false)
                    {
                        if (result.data != null)
                        {
                            for (int i = 0; i < result.data.Count; i++)
                            {
                                string CountryName = result.data[i].name;
                                string iso2 = result.data[i].code;
                                string Code = result.data[i].dial_code;
                                AddCountryCodeStaticData(CountryName, iso2, Code);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void AddCountryCodeStaticData(string CountryName, string iso2, string Code)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                new SqlParameter("@CountryName", CountryName),
                new SqlParameter("@iso2", iso2),
                new SqlParameter("@Code", Code)
                };
                DataTable dt1 = _db.GetDataTableSync("AddUpdateCountryCodeData", para);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Currency Static Data
        internal void getCurrencyAPI()
        {
            //try
            //{
            //    string apiUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            //    string responseCountry = CallApi(apiUrl + "countries/codes");
            //    if (!string.IsNullOrEmpty(responseCountry))
            //    {
            //        var result = JsonConvert.DeserializeObject<CountryCode>(responseCountry);
            //        if (result.error == false)
            //        {
            //            if (result.data != null)
            //            {
            //                for (int i = 0; i < result.data.Count; i++)
            //                {
            //                    string CountryName = result.data[i].name;
            //                    string iso2 = result.data[i].code;
            //                    string Code = result.data[i].dial_code;
            //                    AddCountryCodeStaticData(CountryName, iso2, Code);
            //                }
            //            }
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
        }
        #endregion

        #region Common Methods
        public List<T> ConvertJsonToList<T>(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
        public string CallApi(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    var response = client.SendAsync(request).Result; // sync
                    response.EnsureSuccessStatusCode();

                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            
        }
        #endregion

    }
}