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

    //математические функции

    private void convertArrDouble(double[,] arr, DropDownList ddl, ref TextBox tb)
    {
        if (ddl.SelectedIndex > 0)
        {
            if (!String.IsNullOrWhiteSpace(tb.Text))
            {
                int jj = Convert.ToInt32(Session[ddl.ID]);

                if (jj > 0)
                {
                    tb.Text = (customConverterToDouble((tb.Text.Replace(".", ","))) * arr[(jj - 1), (ddl.SelectedIndex - 1)]).ToString().Replace(",", ".");
                }
            }
        }
    }

    private void convertArr(double[] arr, DropDownList ddl, ref TextBox tb)
    {
        if (ddl.SelectedIndex > 0)
        {
            if (!String.IsNullOrWhiteSpace(tb.Text))
            {
                int jj = Convert.ToInt32(Session[ddl.ID]);
                tb.Text = (customConverterToDouble(tb.Text.Replace(".", ",")) * arr[jj - 1] / arr[ddl.SelectedIndex - 1]).ToString().Replace(",", ".");
            }
        }
    }

    private double convertArrToBar(double[] arr, DropDownList ddl, TextBox tb)
    {
        double result = 0;

        if (ddl.SelectedIndex > 0)
        {
            if (!String.IsNullOrWhiteSpace(tb.Text))
            {

                int jj = Convert.ToInt32(Session[ddl.ID]);
                if (jj > 0)
                {
                    result = (customConverterToDouble(tb.Text) * arr[jj - 1] / arr[2]);
                }
            }
        }
        return result;
    }

    public double customConverterToDouble(string tb)
    {

        double afterConvert;

        if (tb.IndexOf(".") != -1)
        {
            string beforeConvert = tb.Replace(".", ",");
            afterConvert = Convert.ToDouble(beforeConvert);
        }
        else
        {
            afterConvert = Convert.ToDouble(tb);
        }

        return afterConvert;
    }

    private double getPSbyT(double t)
    {
        return Math.Pow(t / 103, 1 / 0.242) - 0.892;
    }

    private double getTbyPS(double ps)
    {
        return 103 * Math.Pow(ps + 0.892, 0.242);
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
        if (pnRadioButtonList1.SelectedIndex != -1 && ((tvRadioButton1.Checked && tvRadioButtonList1.SelectedIndex != -1) || (tvRadioButton2.Checked && tvRadioButtonList2.SelectedIndex != -1)))
        {
            Newtonsoft.Json.Linq.JArray jArrDN = new Newtonsoft.Json.Linq.JArray();
            string ktName = "";
            string pnVal = pnRadioButtonList1.SelectedValue;

            if (tvRadioButton1.Checked)
            {
                ktName = tvRadioButtonList1.SelectedValue;
                jArrDN = dataFromFile.DN2[ktName][pnVal];
            }

            if (tvRadioButton2.Checked)
            {
                ktName = tvRadioButtonList2.SelectedValue;
                jArrDN = dataFromFile.DN3[ktName][pnVal];
            }

            dnDropDownList1.Items.Clear();
            dnDropDownList1.Items.Insert(0, "выбрать");

            foreach (var item in jArrDN)
            {
                dnDropDownList1.Items.Add(new ListItem(item.ToString(), item.ToString()));
            }
        }
    }

    private void setKvsDataset()
    {
        if (pnRadioButtonList1.SelectedIndex != -1 && dnDropDownList1.SelectedIndex > 0 && ((tvRadioButton1.Checked && tvRadioButtonList1.SelectedIndex != -1) || (tvRadioButton2.Checked && tvRadioButtonList2.SelectedIndex != -1)))
        {
            Newtonsoft.Json.Linq.JArray jArrKvs = new Newtonsoft.Json.Linq.JArray();
            string ktName = "";
            string pnVal = pnRadioButtonList1.SelectedValue;
            string dnVal = dnDropDownList1.SelectedValue;


            if (tvRadioButton1.Checked)
            {
                ktName = tvRadioButtonList1.SelectedValue;
                jArrKvs = dataFromFile.Kvs2[ktName][pnVal][dnVal];
            }

            if (tvRadioButton2.Checked)
            {
                ktName = tvRadioButtonList2.SelectedValue;
                jArrKvs = dataFromFile.Kvs3[ktName][pnVal][dnVal];
            }

            kvsDropDownList1.Items.Clear();
            kvsDropDownList1.Items.Insert(0, "выбрать");

            foreach (var item in jArrKvs)
            {
                kvsDropDownList1.Items.Add(new ListItem(item.ToString(), item.ToString()));
            }
        }
    }



    //вспомогательные функции 
    public bool SetEnableTextBox(DropDownList dropDownList, TextBox textBox)
    {
        bool flag = false;

        if (dropDownList.SelectedIndex == 0)
        {
            DisableTextBox(textBox);
            flag = false;
        }
        else
        {
            textBox.Enabled = true;
            flag = true;
        }

        return flag;
    }
    private void SavePrevSelectedIndexDDL(string id, int key)
    {
        Session[id] = key;
    }
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

    private bool checkTextBoxEmpty(TextBox tb)
    {
        return tb.Text == "";
    }

    public void EnablePanel1()
    {
        if((tvRadioButton1.Checked == true && tvRadioButtonList1.SelectedIndex != -1 && pnRadioButtonList1.SelectedIndex != -1) || 
            (tvRadioButton2.Checked == true && tvRadioButtonList2.SelectedIndex != -1 && pnRadioButtonList1.SelectedIndex != -1))
        {
            
            setKvsDataset();
            DisableDropDownList(kvsDropDownList1);
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

            fvDropDownList1.Enabled = true;
            lpvDropDownList21.Enabled = true;
            calcvDropDownList1.Enabled = true;
            DisableDropDownList(lpv5DropDownList1);
            DisableTextBox(lpv5TextBox1);
            DisableTextBox(lpv5TextBox3); 
            calcvTextBox2.Enabled = true;

            //управление блоками
            aaPanel2.Visible = true;
            aaPanel3.Visible = true;
            aaPanel5.Visible = true;
            aaPanel4.Visible = false;
        }
        else
        {
            lpvRadioButtonList1.Enabled = true;
            fvDropDownList1.Enabled = true;
            lpv5DropDownList1.Enabled = true;
            DisableDropDownList(lpvDropDownList21);
            DisableDropDownList(calcvDropDownList1);
            DisableTextBox(lpvTextBox21);
            DisableTextBox(calcvTextBox1);
            DisableTextBox(calcvTextBox2);
            DisableTextBox(calcvTextBox2);
            lpv5TextBox3.Enabled = true;

            //управление блоками
            aaPanel4.Visible = true;
            aaPanel5.Visible = true;
            aaPanel2.Visible = false;
            aaPanel3.Visible = false;
        }
    }

    protected void lpvDropDownList21_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lpvDropDownList21, lpvTextBox21))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lpvTextBox21);
        }
        SavePrevSelectedIndexDDL(lpvDropDownList21.ID, lpvDropDownList21.SelectedIndex);
    }

    protected void calcvDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(calcvDropDownList1, calcvTextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref calcvTextBox1);
        }
        SavePrevSelectedIndexDDL(calcvDropDownList1.ID, calcvDropDownList1.SelectedIndex);
    }

    protected void lpv5DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lpv5DropDownList1, lpv5TextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lpv5TextBox1);
        }
        SavePrevSelectedIndexDDL(lpv5DropDownList1.ID, lpv5DropDownList1.SelectedIndex);
    }

    protected void fvDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(fvDropDownList1, fvTextBox1))
        {
            convertArrDouble(arrConvert1, (sender as DropDownList), ref fvTextBox1);
        }
        SavePrevSelectedIndexDDL(fvDropDownList1.ID, fvDropDownList1.SelectedIndex);
    }

    protected void kvsDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    //Валидаторы элементов
    protected void tvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!tvRadioButton1.Checked && !tvRadioButton2.Checked)
        {
            tvCustomValidator1.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
            return;
        }

        if (tvRadioButton1.Checked && tvRadioButtonList1.SelectedIndex == -1)
        {
            tvCustomValidator1.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
            return;
        }
        if (tvRadioButton2.Checked && tvRadioButtonList2.SelectedIndex == -1)
        {
            tvCustomValidator1.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
            return;
        }

    }

    protected void pnCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (tvCustomValidator1.IsValid)
        {
            if (pnRadioButtonList1.SelectedIndex == -1)
            {
                pnCustomValidator1.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
                return;
            }
        } else
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

        } else
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
                }
            }
        }
        else
        {
            args.IsValid = false;
            wsCustomValidator1.ErrorMessage = "";
        }
    }

    protected void lpvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (wsCustomValidator1.IsValid)
        {
            if (lpvDropDownList21.Enabled)
            {
                if (lpvTextBox21.Enabled == false || checkTextBoxEmpty(lpvTextBox21))
                {
                    lpvCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lpvTextBox21.Text) <= 0)
                {
                    lpvCustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
            }
            if(calcvTextBox1.Enabled == true && !checkTextBoxEmpty(calcvTextBox1) && customConverterToDouble(lpvTextBox21.Text) >= customConverterToDouble(calcvTextBox1.Text))
            {
                lpvCustomValidator1.ErrorMessage = "Потери давления на регулируемом участке превышают давление перед клапаном";
                args.IsValid = false;
                return;
            }
        }
        else
        {
            args.IsValid = false;
            lpvCustomValidator1.ErrorMessage = "";
        }
        return;
    }

    protected void calcvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        calcvCustomValidator1.Visible = true;
        if (lpvCustomValidator1.IsValid)
        {
            if (calcvDropDownList1.Enabled)
            {

                if (calcvTextBox1.Enabled == false || checkTextBoxEmpty(calcvTextBox1))
                {
                    calcvCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(calcvTextBox1.Text) <= 0)
                {
                    calcvCustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }

                if (customConverterToDouble(calcvTextBox1.Text) > customConverterToDouble(pnRadioButtonList1.SelectedValue))
                {
                    calcvCustomValidator1.ErrorMessage = "Давление перед клапаном больше номинального давления клапана. Указанный клапан не подойдет";
                    args.IsValid = false;
                    return;
                }

                if (tvRadioButtonList1.SelectedIndex == 0)
                {
                    if (convertArrToBar(arrConvert3, calcvDropDownList1, calcvTextBox1) > PressureBeforeValve2x)
                    {
                        calcvCustomValidator1.ErrorMessage = "На давление свыше 25 бар вариантов нет";
                        args.IsValid = false;
                        return;
                    }
                }
                else
                {
                    if (convertArrToBar(arrConvert3, calcvDropDownList1, calcvTextBox1) > PressureBeforeValve3x)
                    {
                        calcvCustomValidator1.ErrorMessage = "На давление свыше 16 бар вариантов нет";
                        args.IsValid = false;
                        return;
                    }
                }
            }
        }
        else
        {
            calcvCustomValidator1.ErrorMessage = "";
            args.IsValid = false;
            return;
        }
    }

    protected void calcvCustomValidator2_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (calcvCustomValidator1.IsValid)
        {
            if (calcvTextBox2.Enabled)
            {
                if (checkTextBoxEmpty(calcvTextBox2))
                {
                    calcvCustomValidator2.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(calcvTextBox2.Text) <= 0)
                {
                    calcvCustomValidator2.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return;
                }
                if (tvRadioButtonList1.SelectedIndex == 1 && customConverterToDouble(calcvTextBox2.Text) > MaxT2x)
                {
                    calcvCustomValidator2.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного клапана. Указанный клапан не подойдет";
                    args.IsValid = false;
                    return; 
                }
                
                if ((tvRadioButtonList1.SelectedIndex == 0 || tvRadioButtonList2.SelectedIndex == 0) && customConverterToDouble(calcvTextBox2.Text) > MaxT3x)
                {
                    calcvCustomValidator2.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного клапана. Указанный клапан не подойдет";
                    args.IsValid = false;
                    return;
                }
                

                if (((customConverterToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - getPSbyT(customConverterToDouble(calcvTextBox2.Text))) <= 0)
                {
                    calcvCustomValidator2.ErrorMessage = "Указанная температура выше температуры парообразования. При указанной температуре в трубопроводе движется пар";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            calcvCustomValidator2.ErrorMessage = "";
            args.IsValid = false;
            return;
        }
    }

    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (wsCustomValidator1.IsValid) { 

            if (lpv5DropDownList1.Enabled)
            {
                if (lpv5TextBox1.Enabled == false || checkTextBoxEmpty(lpv5TextBox1))
                {
                    CustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lpv5TextBox1.Text) <= 0)
                {
                    CustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lpv5TextBox1.Text) > customConverterToDouble(pnRadioButtonList1.SelectedValue))
                {
                    CustomValidator1.ErrorMessage = "Давление перед клапаном больше номинального давления клапана. Указанный клапан не подойдет";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            CustomValidator1.ErrorMessage = "";
            args.IsValid = false;
            return;
        }
    }

    protected void CustomValidator3_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator1.IsValid)
        {
            if (lpvRadioButtonList1.SelectedIndex == 0)
            {
                if (wsRadioButtonList1.SelectedIndex == 3)
                {
                    if (lpv5TextBox3.Enabled == false || checkTextBoxEmpty(lpv5TextBox3))
                    {
                        CustomValidator3.ErrorMessage = "Необходимо заполнить поле";
                        args.IsValid = false;
                        return;
                    }
                    if (customConverterToDouble(lpv5TextBox3.Text) <= 0)
                    {
                        CustomValidator3.ErrorMessage = "Неверно указано значение температуры";
                        args.IsValid = false;
                        return;
                    }

                    if (customConverterToDouble(lpv5TextBox3.Text) > MaxT2x)
                    {
                        CustomValidator3.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного клапана. Указанный клапан не подойдет";
                        args.IsValid = false;
                        return;
                    }

                    if (customConverterToDouble(lpv5TextBox3.Text) < (100 * Math.Pow((customConverterToDouble(lpv5TextBox1.Text) * arrConvert3[lpv5DropDownList1.SelectedIndex - 1] / arrConvert3[2]) + 1, 0.25)))
                    {
                        CustomValidator3.ErrorMessage = "Указанная температура ниже температуры парообразования. При указанной температуре в трубопроводе движется жидкость";
                        args.IsValid = false;
                        return;
                    }

                }

            }
        }
        else
            {
            CustomValidator3.ErrorMessage = "";
            args.IsValid = false;
            return;
        }
    }

    protected void fvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (calcvCustomValidator2.IsValid && CustomValidator3.IsValid && CustomValidator3.IsValid)
        {
            if (fvDropDownList1.Enabled)
            {
                if (fvTextBox1.Enabled == false || checkTextBoxEmpty(fvTextBox1))
                {
                    fvCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(fvTextBox1.Text) <= 0)
                {
                    fvCustomValidator1.ErrorMessage = "Неверно указано значение расхода";
                    args.IsValid = false;

                }
            }
        }
        else
        {
            args.IsValid = false;
            fvCustomValidator1.ErrorMessage = "";
        }
    }
}