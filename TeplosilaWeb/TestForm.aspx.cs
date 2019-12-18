using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

public partial class TestForm : System.Web.UI.Page
{

    private dynamic dataFromFile;
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        readFile(0);
    }
    private void readFile(int index)
    {
        try
        {
            string jsonText = null;
            this.dataFromFile = null;
            switch (index)
            {
                case 0:
                    jsonText = File.ReadAllText(HttpContext.Current.Server.MapPath(@"Content/data/data.txt"));
                    break;
                case 1:
                    jsonText = File.ReadAllText(HttpContext.Current.Server.MapPath(@"/Resources/data-two.txt"));
                    break;
            }
            if (jsonText != null)
            {
                this.dataFromFile = JsonConvert.DeserializeObject(jsonText);
            }
        }
        catch (Exception)
        {
            //LabelError.Text += "Проверьте пожалуйста файл с данными!");
        }
        Response.Write("ORDER_ID:" + "<br />");
    }
}