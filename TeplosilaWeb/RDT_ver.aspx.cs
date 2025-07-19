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
using Newtonsoft.Json.Linq;

public partial class RDT_ver : System.Web.UI.Page
{
    private const int PressureBeforeValve2x = 25;
    private const int PressureBeforeValve3x = 16;
    private const int MaxT2x = 220;
    private const int MaxT3x = 150;
    private const string JsonKeyName = "JSON_RDT_ver";
    private dynamic dataFromFile;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                Logger.InitLogger(); //инициализация - требуется один раз в начале
                AppUtils.readFile(@"Content/data/dataRDT_ver.json", JsonKeyName); //читаем json файл с данными
            } else
            {
                if(Session[JsonKeyName] == null)
                {
                    LabelError.Text = "Сессия завершена. Пожалуйста, перезагрузите страницу.";
                    return;
                }
                dataFromFile = Session[JsonKeyName];
                rdtSave.Visible = false;
            }

        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
        }
    }

    //вспомогательные функции работы с данными
    private void setKvsDataset()
    {

        if (pnRadioButtonList1.SelectedIndex != -1 && dnDropDownList1.SelectedIndex > 0 && ((eorRadioButton1.Checked && eorRadioButtonList1.SelectedIndex != -1) || 
                                                                                            (eorRadioButton2.Checked && eorRadioButtonList2.SelectedIndex != -1) ||
                                                                                            eorRadioButton3.Checked && eorRadioButtonList3.SelectedIndex != -1 ||
                                                                                            eorRadioButton4.Checked && eorRadioButtonList4.SelectedIndex != -1))
        {
            JArray jArrKvs = new JArray();

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

            jArrKvs = dataFromFile.DN_Kvs[ktName][pnVal][dnVal];          

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

            JObject dnObject = (JObject)dataFromFile["DN_Kvs"][ktName][pnVal];

            // Получаем список DN, отсортированных по возрастанию
            List<int> dnList = dnObject.Properties()
                                       .Select(p => int.Parse(p.Name))
                                       .OrderBy(x => x)
                                       .ToList();

            dnDropDownList1.Items.Clear();
            dnDropDownList1.Items.Insert(0, "выбрать");

            foreach (var item in dnList)
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

    protected void wsRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (wsRadioButtonList1.SelectedIndex == 1 || wsRadioButtonList1.SelectedIndex == 2)
        {
            wsTextBox1.Enabled = true;
            wsTextBox2.Enabled = true;
        }
        else
        {
            AppUtils.DisableTextBox(wsTextBox1);
            AppUtils.DisableTextBox(wsTextBox2);
        }

        if (wsRadioButtonList1.SelectedIndex != 3)
        {
        }
    }

    //Валидаторы элементор управления
    protected void eorCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!eorRadioButton1.Checked && !eorRadioButton2.Checked && !eorRadioButton3.Checked && !eorRadioButton3.Checked)
        {
            eorCustomValidator1.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
            return;
        }

        if (eorRadioButton1.Checked && eorRadioButtonList1.SelectedIndex == -1)
        {
            eorCustomValidator1.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
            return;
        }
        if (eorRadioButton2.Checked && eorRadioButtonList2.SelectedIndex == -1)
        {
            eorCustomValidator1.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
            return;
        }
    }

    protected void csrCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (eorCustomValidator1.IsValid)
        {
            if (csrRadioButtonList1.SelectedIndex == -1)
            {
                csrCustomValidator1.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
                return;
            }
        }
        else
        {
            args.IsValid = false;
            csrCustomValidator1.ErrorMessage = "";
        }
    }

    protected void pnCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (csrCustomValidator1.IsValid)
        {
            if (pnRadioButtonList1.SelectedIndex == -1)
            {
                pnCustomValidator1.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
                return;
            }
        }
        else
        {
            args.IsValid = false;
            pnCustomValidator1.ErrorMessage = "";
        }
    }

    protected void dnKvsCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (pnCustomValidator1.IsValid)
        {
            if (dnDropDownList1.SelectedIndex <= 0 || kvsDropDownList1.SelectedIndex <= 0)
            {
                dnKvsCustomValidator1.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
                return;
            }
        }
        else
        {
            args.IsValid = false;
            dnKvsCustomValidator1.ErrorMessage = "";
        }
    }

    protected void wsCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (dnKvsCustomValidator1.IsValid)
        {
            if (wsRadioButtonList1.SelectedIndex == -1)
            {
                wsCustomValidator1.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
                return;
            }
            if (wsRadioButtonList1.SelectedIndex == 3)
            {
                if (lpvRadioButtonList1.SelectedIndex == -1)
                {
                    wsCustomValidator1.ErrorMessage = "Необходимо выбрать тип пара";
                    args.IsValid = false;
                    return;
                }
            }
            if (wsTextBox1.Enabled || wsTextBox2.Enabled)
            {
                if (AppUtils.customConverterToDouble(wsTextBox1.Text) < 5 || AppUtils.customConverterToDouble(wsTextBox1.Text) > 65)
                {
                    wsCustomValidator1.ErrorMessage = "Неверно указано значение концентрации";
                    args.IsValid = false;
                    return;
                }

                if (AppUtils.customConverterToDouble(wsTextBox2.Text) < 0 || AppUtils.customConverterToDouble(wsTextBox2.Text) > 150)
                {
                    wsCustomValidator1.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            wsCustomValidator1.ErrorMessage = "";
        }
    }
}