using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountryCityStaticData.Models
{
    #region Country / city Static Data
    public class data
    {
        public string iso2 { get; set; }
        public string iso3 { get; set; }
        public string country { get; set; }
        public List<string> cities { get; set; }
    }

    public class CountryCity
    {
        public bool error { get; set; }
        public string msg { get; set; }
        public List<data> data { get; set; }
    }
    #endregion

    #region Country Code Static Data
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Codes
    {
        public string name { get; set; }
        public string code { get; set; }
        public string dial_code { get; set; }
    }

    public class CountryCode
    {
        public bool error { get; set; }
        public string msg { get; set; }
        public List<Codes> data { get; set; }
    }


    #endregion

}
