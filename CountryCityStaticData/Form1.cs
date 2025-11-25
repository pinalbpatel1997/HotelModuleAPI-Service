using CountryCityStaticData.Interface;
using CountryCityStaticData.Models;
using CountryCityStaticData.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CountryCityStaticData
{
    public partial class Form1 : Form
    {
        IDBConnection db;
        MainRepository _common;
        public Form1()
        {
            InitializeComponent();
            db = new DBConnectionRepository();
            _common = new MainRepository(db);
            try
            {
                //LoadCountryCityStaticData();
                //LoadCountryCodeStaticData();
                LoadCurrencyStaticData();
            }
            catch (Exception)
            {
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        #region Country / city Static Data
        private void LoadCountryCityStaticData()
        {
            try
            {
                _common.GetCountryCityDataAPI();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        #region Country Code Static Data
        private void LoadCountryCodeStaticData()
        {
            try
            {
                _common.GetCountryCodeDataAPI();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region LoadCurrencies
        private void LoadCurrencyStaticData()
        {
            try
            {
                _common.getCurrencyAPI();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }
}
