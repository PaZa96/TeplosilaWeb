using GemBox.Spreadsheet;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using TeplosilaWeb.App_Code;

public partial class CalcPrice : System.Web.UI.Page
{
    private string _token;

    private const string StateKey = "CalcPriceState";

    protected void Page_Load (object sender, EventArgs e) {
        try {
            if (!IsPostBack) {
                if (string.IsNullOrEmpty (hfToken.Value)) {
                    hfToken.Value = Guid.NewGuid ().ToString ();
                    _token = hfToken.Value;
                }

                

                LoadRate ();


            } else {
                _token = hfToken.Value;

     
            }
        } catch (Exception ex) {
            Logger.Log.Error (ex);
        }
    }

    private void LoadRate()
    {
        try
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.GetEncoding("windows-1251");

                string xml =
                    client.DownloadString("https://www.cbr.ru/scripts/XML_daily.asp");

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNode node =
                    doc.SelectSingleNode("//Valute[@ID='R01090B']");

                if (node != null)
                {
                    string nominal =node["Nominal"].InnerText;

                    string value =node["Value"].InnerText;

                    decimal nominalValue =Convert.ToDecimal(nominal, CultureInfo.GetCultureInfo("ru-RU"));

                    decimal rateValue = Convert.ToDecimal(value,CultureInfo.GetCultureInfo("ru-RU"));

                    // Курс RUB за 1 BYN
                    decimal rate =
                        rateValue / nominalValue;

                    txtRate.Text =rate.ToString("0.##",CultureInfo.InvariantCulture);

                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Error(ex);
        }
    }
    protected void btnRefreshRate_Click(object sender, EventArgs e)
    {
        LoadRate(); 
    }
}