using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GemBox.Spreadsheet;
using Newtonsoft.Json;
using TeplosilaWeb.App_Code;

public partial class TRV_ver : System.Web.UI.Page
{
    private const int PressureBeforeValve2x = 25;
    private const int PressureBeforeValve3x = 16;
    private const int MaxT2x = 220;
    private const int MaxT3x = 150;
    private dynamic dataFromFile;
    private double[,] convertTable;

    double[,] arrConvert1;
    double[] arrConvert2;
    double[] arrConvert3;


    public Dictionary<int, string> v_input_dict = new Dictionary<int, string>();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            convertTable = new double[2, 7] { { 1000, 3600, 60, 1, 3600, 1, 1000 }, { 1, 3.6, 0.06, 0.001, 3.6, 0.001, 1 } };

            arrConvert1 = new double[7, 7] { { 1, 0.278, 16.67, 1000, 0.278, 1000, 1 },
                                             { 3.6, 1, 60, 3600, 1, 3600, 3.6 },
                                             { 0.06, 0.0167, 1, 60, 0.0167, 60, 0.06 },
                                             { 0.001, 0.000278, 0.0167, 1, 0.000278, 1, 0.001 },
                                             { 3.6, 1, 60, 3600, 1, 3600, 3.6 },
                                             { 0.001, 0.000278, 0.0167, 1, 0.000278, 1, 0.001 },
                                             { 1, 0.278, 16.67, 1000, 0.278, 1000, 1 }
                };
            arrConvert2 = new double[5] { 1000, 1000000, 1, 1163000, 1.163 };
            arrConvert3 = new double[4] { 1000, 1, 100, 9.8067 };

            Logger.InitLogger(); //инициализация - требуется один раз в начале

            this.readFile(); //читаем json файл с данными
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    //вспомогательные функции работы с данными
    private void readFile()
    {
        try
        {
            string jsonText = File.ReadAllText(HttpContext.Current.Server.MapPath(@"Content/data/dataTRV_ver.json"));
            this.dataFromFile = null;
   
            if (jsonText != null)
            {
                this.dataFromFile = JsonConvert.DeserializeObject(jsonText);
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
        }
    }

    private void setDNDataset()
    {
        Newtonsoft.Json.Linq.JArray jArrDN = new Newtonsoft.Json.Linq.JArray();

        if (tvRadioButton1.Checked)
        {
            if(pnRadioButtonList1.SelectedIndex == 0)
            {
                switch (tvRadioButtonList1.SelectedIndex)
                {
                    case 0:
                        jArrDN = dataFromFile.DN16trv;
                        break;
                    case 1:
                        jArrDN = dataFromFile.DN25trv;
                        break;
                    default:
                        break;
                }
            } else
            {
                switch (tvRadioButtonList1.SelectedIndex)
                {
                    case 0:
                        jArrDN = dataFromFile.DN16trvt;
                        break;
                    case 1:
                        jArrDN = dataFromFile.DN25trvt;
                        break;
                    default:
                        break;
                }
            }
            
        }

        if (tvRadioButton2.Checked)
        {
            if (pnRadioButtonList1.SelectedIndex == 0)
            {
                switch (tvRadioButtonList2.SelectedIndex)
                {
                    case 0:
                        jArrDN = dataFromFile.DN16trv3;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //для trv3 -25
            }
        }

        dnDropDownList1.Items.Clear();
        dnDropDownList1.Items.Insert(0, "выбрать");

        foreach (var item in jArrDN)
        {
            dnDropDownList1.Items.Add(new ListItem(item.ToString(), item.ToString()));
        }
    }

    private void setKvsDataset()
    {
        Newtonsoft.Json.Linq.JArray jArrKvs = new Newtonsoft.Json.Linq.JArray();

        if (tvRadioButton1.Checked)
        {
            if (pnRadioButtonList1.SelectedIndex == 0)
            {
                switch (tvRadioButtonList1.SelectedIndex)
                {
                    case 0:
                        jArrKvs = dataFromFile.DN16trv;
                        break;
                    case 1:
                        jArrKvs = dataFromFile.DN25trv;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (tvRadioButtonList1.SelectedIndex)
                {
                    case 0:
                        jArrKvs = dataFromFile.DN16trvt;
                        break;
                    case 1:
                        jArrKvs = dataFromFile.DN25trvt;
                        break;
                    default:
                        break;
                }
            }

        }

        if (tvRadioButton2.Checked)
        {
            if (pnRadioButtonList1.SelectedIndex == 0)
            {
                switch (tvRadioButtonList2.SelectedIndex)
                {
                    case 0:
                        jArrKvs = dataFromFile.DN16trv3;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //для trv3 -25
            }
        }

        kvsDropDownList1.Items.Clear();
        kvsDropDownList1.Items.Insert(0, "выбрать");

        foreach (var item in jArrKvs)
        {
            kvsDropDownList1.Items.Add(new ListItem(item.ToString(), item.ToString()));
        }
    }

    //вспомогательные функции 
    public void DisableTextBox(TextBox textBox)
    {
        textBox.Enabled = false;
        textBox.Text = "";
    }

    public void DisableDropDownList(DropDownList dropDownList)
    {
        dropDownList.Enabled = false;
        dropDownList.ClearSelection();
    }

    public void DisableRadioButtonList(RadioButtonList radioButtonList)
    {
        radioButtonList.Enabled = false;
        radioButtonList.ClearSelection();
    }

    public void EnablePanel1()
    {
        if((tvRadioButton1.Checked == true && tvRadioButtonList1.SelectedIndex != -1 && pnRadioButtonList1.SelectedIndex != -1) || 
            (tvRadioButton2.Checked == true && tvRadioButtonList2.SelectedIndex != -1 && pnRadioButtonList1.SelectedIndex != -1))
        {
            setDNDataset();
            dnDropDownList1.Enabled = true;
        } 
        else
        {
            DisableDropDownList(dnDropDownList1);
            DisableDropDownList(kvsDropDownList1);
        }
    }

    private void ManageWSRBL()
    {
        if(tvRadioButton1.Checked && tvRadioButtonList1.SelectedIndex == 1)
        {
            wsRadioButtonList1.Items[3].Enabled = true;
        } else
        {
            wsRadioButtonList1.Items[3].Enabled = false;
        }
    }

    //обработчики элементов
    protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        tvRadioButton2.Checked = false;
        tvRadioButtonList1.Enabled = true;
        DisableRadioButtonList(tvRadioButtonList2);

        pnRadioButtonList1.Enabled = true;
        pnRadioButtonList1.Items[1].Enabled = true;

        EnablePanel1();
        ManageWSRBL();
    }

    protected void tvRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnablePanel1();
        ManageWSRBL();
    }

    protected void tvRadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        tvRadioButton1.Checked = false;
        tvRadioButtonList2.Enabled = true;
        tvRadioButtonList2.SelectedIndex = 0;
        DisableRadioButtonList(tvRadioButtonList1);

        pnRadioButtonList1.Enabled = true;
        pnRadioButtonList1.SelectedIndex = 0;
        pnRadioButtonList1.Items[1].Enabled = false;
        pnRadioButtonList1.SelectedIndex = pnRadioButtonList1.SelectedIndex == 1 ? -1 : 0;

        EnablePanel1();
        ManageWSRBL();
    }

    protected void tvRadioButtonList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnablePanel1();
        ManageWSRBL();
    }

    protected void pnRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {

        EnablePanel1();
    }

    protected void dnDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(dnDropDownList1.SelectedIndex > 0)
        {
            kvsDropDownList1.Enabled = true;
            setKvsDataset();
        } else
        {
            DisableDropDownList(kvsDropDownList1);
        }
        
    }

    protected void wsRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (wsRadioButtonList1.SelectedIndex == 1 || wsRadioButtonList1.SelectedIndex == 2)
        {
            wsTextBox1.Enabled = true;
            wsTextBox2.Enabled = true;
        }
        else
        {
            DisableTextBox(wsTextBox1);
            DisableTextBox(wsTextBox2);
        }

        if (wsRadioButtonList1.SelectedIndex != 3)
        {
            DisableRadioButtonList(lpvRadioButtonList1);
        }
        else
        {
            lpvRadioButtonList1.Enabled = true;
        }
    }
}