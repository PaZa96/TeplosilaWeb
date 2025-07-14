using GemBox.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeplosilaWeb.App_Code;
using System.IO;

public partial class RDT_ver : System.Web.UI.Page
{
    private const int PressureBeforeValve2x = 25;
    private const int PressureBeforeValve3x = 16;
    private const int MaxT2x = 220;
    private const int MaxT3x = 150;
    private dynamic dataFromFile;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Logger.InitLogger(); //инициализация - требуется один раз в начале

            this.readFile(); //читаем json файл с данными
            //resultPanel.Visible = false;
            //rdtSave.Visible = false;
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
            string jsonText = File.ReadAllText(HttpContext.Current.Server.MapPath(@"Content/data/dataRDT_ver.json"));
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

    private void setKvsDataset()
    {
        if (pnRadioButtonList1.SelectedIndex != -1 && dnDropDownList1.SelectedIndex > 0 && ((eorRadioButton1.Checked && eorRadioButtonList1.SelectedIndex != -1) || 
                                                                                            (eorRadioButton2.Checked && eorRadioButtonList2.SelectedIndex != -1) ||
                                                                                            eorRadioButton3.Checked && eorRadioButtonList3.SelectedIndex != -1 ||
                                                                                            eorRadioButton4.Checked && eorRadioButtonList4.SelectedIndex != -1))
        {
            Newtonsoft.Json.Linq.JArray jArrKvs = new Newtonsoft.Json.Linq.JArray();
            string ktName = "";
            string pnVal = pnRadioButtonList1.SelectedValue;
            string dnVal = dnDropDownList1.SelectedValue;

            if (eorRadioButtonList1.Enabled && eorRadioButtonList1.SelectedIndex >= 0)
                ktName = eorRadioButtonList1.SelectedValue;

            if (eorRadioButtonList2.Enabled && eorRadioButtonList2.SelectedIndex >= 0)
                ktName = eorRadioButtonList2.SelectedValue;

            if (eorRadioButtonList3.Enabled && eorRadioButtonList3.SelectedIndex >= 0)
                ktName = eorRadioButtonList3.SelectedValue;

            if (eorRadioButtonList4.Enabled && eorRadioButtonList4.SelectedIndex >= 0)
                ktName = eorRadioButtonList4.SelectedValue;

            jArrKvs = dataFromFile.Kvs[ktName][pnVal][dnVal];
            

            kvsDropDownList1.Items.Clear();
            kvsDropDownList1.Items.Insert(0, "выбрать");

            foreach (var item in jArrKvs)
            {
                kvsDropDownList1.Items.Add(new ListItem(item.ToString(), item.ToString()));
            }
        }
    }
    private void setDNDataset()
    {
        if (pnRadioButtonList1.SelectedIndex != -1 && ((eorRadioButton1.Checked && eorRadioButtonList1.SelectedIndex != -1) ||
                                                        (eorRadioButton2.Checked && eorRadioButtonList2.SelectedIndex != -1) ||
                                                        eorRadioButton3.Checked && eorRadioButtonList3.SelectedIndex != -1 ||
                                                        eorRadioButton4.Checked && eorRadioButtonList4.SelectedIndex != -1))
        {
            Newtonsoft.Json.Linq.JArray jArrDN = new Newtonsoft.Json.Linq.JArray();
            string ktName = "";
            string pnVal = pnRadioButtonList1.SelectedValue;

            if (eorRadioButtonList1.Enabled && eorRadioButtonList1.SelectedIndex >= 0)
                ktName = eorRadioButtonList1.SelectedValue;

            if (eorRadioButtonList2.Enabled && eorRadioButtonList2.SelectedIndex >= 0)
                ktName = eorRadioButtonList2.SelectedValue;

            if (eorRadioButtonList3.Enabled && eorRadioButtonList3.SelectedIndex >= 0)
                ktName = eorRadioButtonList3.SelectedValue;

            if (eorRadioButtonList4.Enabled && eorRadioButtonList4.SelectedIndex >= 0)
                ktName = eorRadioButtonList4.SelectedValue;

            jArrDN = dataFromFile.DN[ktName][pnVal];

            dnDropDownList1.Items.Clear();
            dnDropDownList1.Items.Insert(0, "выбрать");

            foreach (var item in jArrDN)
            {
                dnDropDownList1.Items.Add(new ListItem(item.ToString(), item.ToString()));
            }
        }
    }

    //вспомогательные функции

    private void SetRadioButtonGroupState(RadioButton selectedRadio, RadioButtonList selectedList)
    {
        // Все радио-кнопки
        var allRadioButtons = new[] { eorRadioButton1, eorRadioButton2, eorRadioButton3, eorRadioButton4 };

        // Все списки
        var allRadioLists = new[] { eorRadioButtonList1, eorRadioButtonList2, eorRadioButtonList3, eorRadioButtonList4};

        // Перебираем все элементы
        for (int i = 0; i < allRadioButtons.Length; i++)
        {
            bool isSelected = allRadioButtons[i] == selectedRadio;

            allRadioButtons[i].Checked = isSelected;
            allRadioLists[i].Enabled = isSelected;

            // (опционально) снимаем выбор из списков, если не активны
            if (!isSelected)
            {
                allRadioLists[i].ClearSelection();
            }
        }
    }

    public void EnableDNPanel()
    {
        if (((eorRadioButton1.Checked == true && eorRadioButtonList1.SelectedIndex != -1) ||
            (eorRadioButton2.Checked == true && eorRadioButtonList2.SelectedIndex != -1) ||
            (eorRadioButton3.Checked == true && eorRadioButtonList3.SelectedIndex != -1) ||
            (eorRadioButton4.Checked == true && eorRadioButtonList4.SelectedIndex != -1)) && pnRadioButtonList1.SelectedIndex != -1)
        {
            setDNDataset();
            dnDropDownList1.Enabled = true;
            AppUtils.DisableDropDownList(kvsDropDownList1);
        }
        else
        {
            AppUtils.DisableDropDownList(dnDropDownList1);
            AppUtils.DisableDropDownList(kvsDropDownList1);
        }
    }

    //обработчики

    protected void eorRadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        SetRadioButtonGroupState(eorRadioButton1, eorRadioButtonList1);
        pnRadioButtonList1.Enabled = true;
    }

    protected void eorRadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        SetRadioButtonGroupState(eorRadioButton2, eorRadioButtonList2);
        pnRadioButtonList1.Enabled = true;
    }

    protected void eorRadioButton3_CheckedChanged(object sender, EventArgs e)
    {
        SetRadioButtonGroupState(eorRadioButton3, eorRadioButtonList3);
        eorRadioButtonList3.SelectedIndex = 0;
        pnRadioButtonList1.Enabled = true;
    }

    protected void eorRadioButton4_CheckedChanged(object sender, EventArgs e)
    {
        SetRadioButtonGroupState(eorRadioButton4, eorRadioButtonList4);
        eorRadioButtonList4.SelectedIndex = 0;
        pnRadioButtonList1.Enabled = true;
    }
    protected void eorRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnableDNPanel();
    }

    protected void eorRadioButtonList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnableDNPanel();
    }

    protected void eorRadioButtonList3_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnableDNPanel();
    }

    protected void eorRadioButtonList4_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnableDNPanel();
    }

    protected void dnDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (dnDropDownList1.SelectedIndex > 0)
        {
            kvsDropDownList1.Enabled = true;
            setKvsDataset();
        }
        else
        {
            AppUtils.DisableDropDownList(kvsDropDownList1);
        }
    }

    protected void kvsDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void pnRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        EnableDNPanel();
    }
}