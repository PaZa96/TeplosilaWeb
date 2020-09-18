﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GemBox.Spreadsheet;
using System.Data;
using System.Net;
using System.Threading;
using System.Diagnostics;
using TeplosilaWeb.App_Code;
using System.Web.UI.HtmlControls;
using System.Reflection.Emit;

public partial class TRV : System.Web.UI.Page
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

    public static Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
    public static Dictionary<int, string> r_input_dict = new Dictionary<int, string>();
    public static Dictionary<int, string> v_input_dict = new Dictionary<int, string>();

    protected void Page_Load(object sender, EventArgs e)
    {

        convertTable = new double[2, 7] { { 1000, 3600, 60, 1, 3600, 1, 1000 }, { 1, 3.6, 0.06, 0.001, 3.6, 0.001, 1 } };

        arrConvert1 =
            new double[7, 7] {
                {1, 0.278, 16.67, 1000, 0.278, 1000, 1},
                {3.6, 1, 60, 3600, 1, 3600, 3.6},
                {0.06, 0.0167, 1, 60, 0.0167, 60, 0.06},
                {0.001, 0.000278, 0.0167, 1, 0.000278, 1, 0.001},
                {3.6, 1, 60, 3600, 1, 3600, 3.6},
                {0.001, 0.000278, 0.0167, 1, 0.000278, 1, 0.001},
                {1, 0.278, 16.67, 1000, 0.278, 1000, 1}
            };
        arrConvert2 = new double[5] { 1000, 1000000, 1, 1163000, 1.163 };
        arrConvert3 = new double[4] { 1000, 1, 100, 9.8067};

        Logger.InitLogger();//инициализация - требуется один раз в начале
        Label8.Text = "";
        Label55.Visible = false;
    }

    protected void tvRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        changeImage(tvRadioButtonList1.SelectedIndex);

        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
    }

    protected void aaRadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        lpvTextBox21.Enabled = false;
        lpvTextBox21.Text = "";
        lpvTextBox2.Enabled = false;
        lpvTextBox2.Text = "";
        aaRadioButton2.Checked = false;
        aaRadioButton3.Checked = false;
        
        aa1RadioButtonList1.Enabled = true;
        aa2RadioButtonList1.Enabled = false;
        aa3RadioButtonList1.Enabled = false;
        aa3RadioButtonList1.Enabled = false;
        aa2RadioButtonList1.ClearSelection();
        aa3RadioButtonList1.ClearSelection();

        DisableDropDownLists();

        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
        AddCssClass(lpv1, "panel-hide");
        AddCssClass(lpv2, "panel-hide");
    }

    protected void aaRadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        lpvTextBox21.Enabled = false;
        lpvTextBox21.Text = "";
        lpvTextBox2.Enabled = false;
        lpvTextBox2.Text = "";
        aaRadioButton1.Checked = false;
        aaRadioButton3.Checked = false;
        aa1RadioButtonList1.Enabled = false;
        aa2RadioButtonList1.Enabled = true;
        aa3RadioButtonList1.Enabled = false;
        aa1RadioButtonList1.ClearSelection();
        aa3RadioButtonList1.ClearSelection();

        DisableDropDownLists();

        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
        AddCssClass(lpv1, "panel-hide");
        AddCssClass(lpv2, "panel-hide");
    }

    protected void aaRadioButton3_CheckedChanged(object sender, EventArgs e)
    {
        lpvTextBox21.Enabled = false;
        lpvTextBox21.Text = "";
        lpvTextBox2.Enabled = false;
        lpvTextBox2.Text = "";
        aaRadioButton2.Checked = false;
        aaRadioButton1.Checked = false;
        aa1RadioButtonList1.Enabled = false;
        aa2RadioButtonList1.Enabled = false;
        aa3RadioButtonList1.Enabled = true;
        aa2RadioButtonList1.ClearSelection();
        aa1RadioButtonList1.ClearSelection();
        DisableDropDownLists();

        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
        AddCssClass(lpv1, "panel-hide");
        AddCssClass(lpv2, "panel-hide");
    }

    private void changeImage(int index)
    {
        switch (index)
        {
            case 0:
                vPictureBox.ImageUrl = @"./Content/images/TRV-2.png";
                break;
            case 1:
                vPictureBox.ImageUrl = @"./Content/images/TRV-3.png";
                break;
            default:
                vPictureBox.ImageUrl = null;
                break;
        }
    }

    public void EnablePanel(int numberOfPanel)
    {
        switch (numberOfPanel)
        {
            case 1:
                lpvDropDownList1.Enabled = true;
                calcvDropDownList1.Enabled = true;
                calcvTextBox2.Enabled = true;
                break;
            

        }

    }

    public void DisablePanel(int numberOfPanel)
    {
        switch (numberOfPanel)
        {
            case 1:
                DisableDropDownList(lpvDropDownList1);
                DisableDropDownList(calcvDropDownList1);
                DisableTextBox(lpvTextBox1);
                DisableTextBox(calcvTextBox1);
                DisableTextBox(calcvTextBox2);
                break;
            case 2:
                DisableTextBox(fvTextBox2);
                DisableTextBox(fvTextBox3);
                DisableTextBox(fvTextBox4);
                DisableTextBox(fvTextBox5);
                DisableTextBox(fvTextBox6);
                DisableTextBox(fvTextBox7);
                DisableTextBox(fvTextBox8);
                DisableTextBox(fvTextBox9);
                break;
        }
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

     public void DisableDropDownLists()
    {
        lpvDropDownList2.Enabled = false;
        lpvDropDownList2.ClearSelection();
        lpvDropDownList21.Enabled = false;
        lpvDropDownList21.ClearSelection();
    }

    public void EnableTemperatureTable()
    {
        DisablePanel(2);

        if (aaRadioButton1.Checked)
        {
            fvTextBox2.Enabled = true;
            fvTextBox3.Enabled = true;
            fvTextBox4.Enabled = true;
            fvTextBox5.Enabled = true;
        }
        else if (aaRadioButton2.Checked)
        {
            if (tvRadioButtonList1.SelectedIndex == 0)
            {

                fvTextBox2.Enabled = true;
                fvTextBox3.Enabled = true;

            }
            else if (tvRadioButtonList1.SelectedIndex == 1)
            {
                if (aa2RadioButtonList1.SelectedIndex == 1)
                {
                    fvTextBox2.Enabled = true;
                    fvTextBox3.Enabled = true;
                }
                else if (aa2RadioButtonList1.SelectedIndex == 0)
                {
                    fvTextBox6.Enabled = true;
                    fvTextBox7.Enabled = true;
                }

            }
        }
        else if (aaRadioButton3.Checked)
        {
            if (tvRadioButtonList1.SelectedIndex == 0)
            {
                fvTextBox2.Enabled = true;
                fvTextBox3.Enabled = true;
            }
            else if (tvRadioButtonList1.SelectedIndex == 1)
            {
                if (aa3RadioButtonList1.SelectedIndex == 1)
                {
                    fvTextBox2.Enabled = true;
                    fvTextBox3.Enabled = true;
                }
                else if (aa3RadioButtonList1.SelectedIndex == 0)
                {
                    fvTextBox8.Enabled = true;
                    fvTextBox9.Enabled = true;
                }

            }
        }

    }

    public void ResetPressureLoss(RadioButtonList radioButtonList)
    {
        if (radioButtonList.SelectedIndex == 0)
        {
            lpvDropDownList2.Enabled = true;
            lpvDropDownList21.Enabled = false;
            lpvDropDownList21.ClearSelection();
            lpvTextBox21.Enabled = false;
            lpvTextBox21.Text = "";
        }
        else
        {
            lpvDropDownList2.Enabled = false;
            lpvDropDownList21.Enabled = true;
            lpvDropDownList2.ClearSelection();
            lpvTextBox2.Enabled = false;
            lpvTextBox2.Text = "";
        }

    }




    protected void ws2RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(ws2RadioButtonList1.SelectedIndex == 1 || ws2RadioButtonList1.SelectedIndex ==2)
        {
            ws2TextBox1.Enabled = true;
            ws2TextBox2.Enabled = true;
        }
        else
        {
            ws2TextBox1.Enabled = false;
            ws2TextBox2.Enabled = false;
            ws2TextBox1.Text = "";
            ws2TextBox2.Text = "";
        }
        if (ws2RadioButtonList1.SelectedIndex != -1)
        {
            EnablePanel(1);
        }
        else
        {
            DisablePanel(1);
        }
        RemoveCssClass(lpv3, "panel-hide");
        RemoveCssClass(calcv, "panel-hide");
    }

    

    protected void aa1RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetPressureLoss(aa1RadioButtonList1);
        
        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
        if(aa1RadioButtonList1.SelectedIndex == 0)
        {
            RemoveCssClass(lpv1, "panel-hide");
            AddCssClass(lpv2, "panel-hide");
        } 
        else
        {
            RemoveCssClass(lpv2, "panel-hide");
            AddCssClass(lpv1, "panel-hide");
        }
    }

    protected void aa2RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetPressureLoss(aa2RadioButtonList1);
        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
        if (aa2RadioButtonList1.SelectedIndex == 0)
        {
            RemoveCssClass(lpv1, "panel-hide");
            AddCssClass(lpv2, "panel-hide");
        }
        else
        {
            RemoveCssClass(lpv2, "panel-hide");
            AddCssClass(lpv1, "panel-hide");
        }
    }

    protected void aa3RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetPressureLoss(aa3RadioButtonList1);
        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
        if (aa3RadioButtonList1.SelectedIndex == 0)
        {
            RemoveCssClass(lpv1, "panel-hide");
            AddCssClass(lpv2, "panel-hide");
        }
        else
        {
            RemoveCssClass(lpv2, "panel-hide");
            AddCssClass(lpv1, "panel-hide");
        }
    }

    protected void lpvDropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lpvDropDownList2, lpvTextBox2))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lpvTextBox2);
        }
        SavePrevSelectedIndexDDL(lpvDropDownList2.ID, lpvDropDownList2.SelectedIndex);
    }

    protected void lpvDropDownList21_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lpvDropDownList21, lpvTextBox21))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lpvTextBox21);
        }
        SavePrevSelectedIndexDDL(lpvDropDownList21.ID, lpvDropDownList21.SelectedIndex);
    }

    protected void lpvDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lpvDropDownList1, lpvTextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lpvTextBox1);
        }
        SavePrevSelectedIndexDDL(lpvDropDownList1.ID, lpvDropDownList1.SelectedIndex);
    }

    protected void calcvDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(calcvDropDownList1, calcvTextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref calcvTextBox1);
        }
        SavePrevSelectedIndexDDL(calcvDropDownList1.ID, calcvDropDownList1.SelectedIndex);
    }

    protected void fvDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(fvDropDownList1, fvTextBox1))
        {
            convertArrDouble(arrConvert1, (sender as DropDownList), ref fvTextBox1);
        }
        SavePrevSelectedIndexDDL(fvDropDownList1.ID, fvDropDownList1.SelectedIndex);
    }

    protected void fvDropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(fvDropDownList2, fvTextBox10))
        {
            convertArr(arrConvert2, (sender as DropDownList), ref fvTextBox10);
        }
        SavePrevSelectedIndexDDL(fvDropDownList2.ID, fvDropDownList2.SelectedIndex);
    }

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
        if (!keyValuePairs.ContainsKey(id))
        {
            keyValuePairs.Add(id, key);
        }
        else
        {
            keyValuePairs[id] = key;
        }
    }

    //--------------------------------------Math Function--------------------------------------

    private void convertArrDouble(double[,] arr, DropDownList ddl, ref TextBox tb)
    {
        if (ddl.SelectedIndex > 0)
        {
            if (!String.IsNullOrWhiteSpace(tb.Text))
            {
                int jj = keyValuePairs[ddl.ID];

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
                int jj = keyValuePairs[ddl.ID];
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

                int jj = keyValuePairs[ddl.ID];
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

    protected void fvRadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        fvDropDownList1.Enabled = true;
        fvRadioButton2.Checked = false;
        DisableTextBox(fvTextBox10);
        DisableTextBox(fvTextBox11);
        fvDropDownList2.Enabled = false;
        fvDropDownList2.ClearSelection();
        DisablePanel(2);
    }

    protected void fvRadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        fvDropDownList1.Enabled = false;
        fvDropDownList1.ClearSelection();
        fvRadioButton1.Checked = false;
        DisableTextBox(fvTextBox1);
        fvDropDownList2.Enabled = true;
        DisablePanel(2);

        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
    }

    public void DisableRadioSecureFunc()
    {
        if (tdRadioButtonList1.SelectedIndex == 1 || tdRadioButtonList2.SelectedIndex == 1 || tdRadioButtonList3.SelectedIndex == 0)
        {
            tdRadioButtonList4.ClearSelection();
            tdRadioButtonList4.SelectedIndex = 1;
            tdRadioButtonList4.Items[0].Enabled = false;
        }
        else
        {
            if (tdRadioButtonList4.SelectedIndex == 0)
            {
                
            }
            else
            {
                tdRadioButtonList4.ClearSelection();
                tdRadioButtonList4.SelectedIndex = 1;
                tdRadioButtonList4.Items[0].Enabled = true;
            }
            
        }
    }



    protected void tdRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        DisableRadioSecureFunc();
    }

    protected void tdRadioButtonList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        DisableRadioSecureFunc();
    }

    protected void tdRadioButtonList3_SelectedIndexChanged(object sender, EventArgs e)
    {
        DisableRadioSecureFunc();
    }

    protected void tdRadioButtonList4_SelectedIndexChanged(object sender, EventArgs e)
    {
        DisableRadioSecureFunc();
    }


    

    private bool firstMoreSecondDouble(string s1, string s2)
    {
        if (!String.IsNullOrWhiteSpace(s1) && !String.IsNullOrWhiteSpace(s2) && !String.IsNullOrEmpty(s1) && !String.IsNullOrEmpty(s2))
        {
            if (customConverterToDouble(s1) > customConverterToDouble(s2))
            {
                return true;
            }
        }
        return false;
    }

    public double math_30_cp()
    {
        double cp = 0;
        double rr = 0;
        if (ws2RadioButtonList1.SelectedIndex == 0)
        {
            Water(GetAvgT(), ref rr);
            cp = WaterCP(GetAvgT()); // 4.187;
        }
        else if (ws2RadioButtonList1.SelectedIndex == 1)
        {
            Etgl(GetAvgT(), double.Parse(this.ws2TextBox1.Text), ref rr, ref cp);

            /*double p14 = Convert.ToDouble(ws2TextBox1.Text);
            double p15 = Math.Round(Convert.ToDouble(ws2TextBox2.Text) / 10) * 10;
            foreach (Newtonsoft.Json.Linq.JProperty el in dataFromFile.table3)
            {
                string s = 0 + "";
                string[] mas = el.Name.Split('-');
                if ((Convert.ToDouble(mas[0]) <= p14) && (Convert.ToDouble(mas[1]) >= p14))
                {
                    double tmp_t = 0.0;
                    foreach (Newtonsoft.Json.Linq.JObject val in el.Value)
                    {
                        if (Convert.ToDouble(val.GetValue("t")) == p15)
                        {
                            tmp_t = Convert.ToDouble(val.GetValue("sr"));
                        }
                    }
                    cp = tmp_t;
                }
            }*/
        }
        else if (ws2RadioButtonList1.SelectedIndex == 2)
        {
            Prgl(GetAvgT(), double.Parse(this.ws2TextBox1.Text), ref rr, ref cp);
        }
        return cp / 1000;
    }

    public void Etgl(double t, double d, ref double rr, /*ref double nn, ref double ll, ref double pr,*/ ref double cp)
    {
        //ll = (0.565 - 0.00303 * d) + (8.9 - 0.21 * d) * t * 0.0001;
        //nn = 0.001 / 1.05 * (0.472 - 0.00238 * d + 0.00024 * d * d + (4.65 - 0.212 * d + 0.00595 * d * d) * Math.Exp(-t / 20.5));
        rr = (1001.9 + 1.73 * d) - (0.1784 + 0.0062 * d) * t - 0.00107 * t * t;
        cp = 1000 * ((4.289 - 0.0212 * d) + (-8.84 + 1.404 * d - 0.00987 * d * d) * t / 10000);
        //nn = nn / rr;
        //pr = nn * cp * rr / ll;
    }

    public void Water(double t, ref double rr)//, ref double nn, ref double ll, ref double pr, ref double cp)
    {
        rr = 1001.37102 - 0.18338 * t - 0.000683948 * Math.Pow(t, 2) - 0.0000290502 * Math.Pow(t, 3) + 0.000000145068 * Math.Pow(t, 4) - 0.000000000231169 * Math.Pow(t, 5);
        //nn = 1.05 * 0.000001 * (1.76764 - 0.05177 * t + 0.000800965 * Math.Pow(t, 2) - 0.00000598494 * Math.Pow(t, 3) + 0.0000000167612 * Math.Pow(t, 4));
        //cp = 1000000 / rr * (4.23825 - 0.00331 * t + 0.0000352307 * Math.Pow(t, 2) - 0.000000216299 * Math.Pow(t, 3));
        //ll = 0.01 * (55.12817 + 0.26992 * t - 0.00179 * Math.Pow(t, 2) + 0.00000482756 * Math.Pow(t, 3) - 0.00000000632 * Math.Pow(t, 4));
        //pr = 13.04883 - 0.40964 * t + 0.00639 * Math.Pow(t, 2) - 0.0000474596 * Math.Pow(t, 3) + 0.000000132131 * Math.Pow(t, 4);
    }

    /// <summary>
    /// Расчет удельной теплоемкости воды для заданной температуры.
    /// </summary>
    /// <param name="t">Средняя температура</param>
    /// <returns></returns>
    public double WaterCP(double t)
    {
        double rr = 1001.37102 - 0.18338 * t - 0.000683948 * Math.Pow(t, 2) - 0.0000290502 * Math.Pow(t, 3) + 0.000000145068 * Math.Pow(t, 4) - 0.000000000231169 * Math.Pow(t, 5);
        //double nn = 1.05 * 0.000001 * (1.76764 - 0.05177 * t + 0.000800965 * Math.Pow(t, 2) - 0.00000598494 * Math.Pow(t, 3) + 0.0000000167612 * Math.Pow(t, 4));
        double cp = 1000000 / rr * (4.23825 - 0.00331 * t + 0.0000352307 * Math.Pow(t, 2) - 0.000000216299 * Math.Pow(t, 3));
        //double ll = 0.01 * (55.12817 + 0.26992 * t - 0.00179 * Math.Pow(t, 2) + 0.00000482756 * Math.Pow(t, 3) - 0.00000000632 * Math.Pow(t, 4));
        //double pr = 13.04883 - 0.40964 * t + 0.00639 * Math.Pow(t, 2) - 0.0000474596 * Math.Pow(t, 3) + 0.000000132131 * Math.Pow(t, 4);
        return cp;
    }

    public double GetAvgT()
    {
        double avg_T = 0;

       
        if (this.fvRadioButton1.Checked)
        {
            if (this.ws2RadioButtonList1.SelectedIndex == 0)
            {
                avg_T = double.Parse(this.calcvTextBox2.Text);
            }
            else
            {
                avg_T = customConverterToDouble(this.ws2TextBox2.Text);
            }
        }
        else
        {
           
            if (this.aaRadioButton2.Checked && this.tvRadioButtonList1.SelectedIndex == 1 && this.aa2RadioButtonList1.SelectedIndex == 0)
            {
                avg_T = 0.5 * (double.Parse(this.fvTextBox6.Text) + double.Parse(this.fvTextBox7.Text));
            }
            else if (this.aaRadioButton3.Checked && this.tvRadioButtonList1.SelectedIndex == 1 && this.aa3RadioButtonList1.SelectedIndex == 0)
            {
                avg_T = 0.5 * (double.Parse(this.fvTextBox8.Text) + double.Parse(this.fvTextBox9.Text));
            }
            else
            //if (this.tvRadioButton1.Checked || this.aaRadioButton1.Checked)
            {
                avg_T = 0.5 * (double.Parse(this.fvTextBox2.Text) + double.Parse(this.fvTextBox3.Text));
            }

        }
        

        return avg_T;
    }

    private double getPSbyT(double t)
    {
        return Math.Pow(t / 103, 1 / 0.242) - 0.892;
    }

    private double getTbyPS(double ps)
    {
        return 103 * Math.Pow(ps + 0.892, 0.242);
    }



    public void Prgl(double t, double d, ref double rr, /*ref double nn, ref double ll, ref double pr,*/ ref double cp)
    {
        //ll = (0.5613 - 0.00363 * d) + (20.1 - 0.372 * d) * t * 0.0001 - (7.39 + 0.071 * d - 0.00326 * d * d) * 0.000001 * t * t;
        //nn = 1.4 * 0.001 * 0.00025 * Math.Exp(233.16 / (d + 51.48)) * Math.Exp((640.24 + 19.235 * d - 0.11645 * d * d) / (t + 150));
        rr = (999.63 + 1.4 * d - 0.0073 * d * d) - (0.08446 + 0.01715 * d - 0.0001277 * d * d) * t - 0.0029 * t * t;
        cp = 1000 * ((4.222 - 0.00752 * d - 0.00011 * d * d) + (-9.712 + 0.5964 * d) * t / 10000 + (8.383 + 0.0877 * d - 0.001457 * d * d) * 0.000001 * t * t);
        //nn = nn / rr;
        //pr = nn * cp * rr / ll;
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
                    jsonText = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data-two.txt");
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
    }

    private void getDimsV(bool hod2, string paramDN, string paramMarkPriv, ref ExcelWorksheet wsH, ref ExcelWorksheet wsG,
            ref string paramPP54, ref string paramPP55, ref string paramPP56, ref string paramPP57, ref string paramPP58, ref string paramPP59, ref string paramPP60,
            ref string paramPP61, ref string paramPP62, ref string paramPP63, ref string paramPP65, ref string paramPP66, ref string paramPP67, ref string paramPP68)
    {
        string ColDN = "";
        string hRowMark = "";
        string gRowMarkH = "";
        string gRowMarkM = "";

        //
        switch (paramDN)
        {
            case "15":
                ColDN = "C"; break;
            case "20":
                ColDN = "D"; break;
            case "25":
                ColDN = "E"; break;
            case "32":
                ColDN = "F"; break;
            case "40":
                ColDN = "G"; break;
            case "50":
                ColDN = "H"; break;
            case "65":
                ColDN = "I"; break;
            case "80":
                ColDN = "J"; break;
            case "100":
                ColDN = "K"; break;
            case "125":
                ColDN = "L"; break;
            case "150":
                ColDN = "M"; break;
        }

        if (hod2)
        {
            //
            switch (paramMarkPriv)
            {
                //
                case "101":
                    hRowMark = "6"; gRowMarkH = "7"; gRowMarkM = "20"; break;
                case "101R":
                    hRowMark = "7"; gRowMarkH = "8"; gRowMarkM = "21"; break;
                case "34":
                    hRowMark = "8"; gRowMarkH = "9"; gRowMarkM = "22"; break;
                case "31":
                    hRowMark = "9"; gRowMarkH = "10"; gRowMarkM = "23"; break;
                case "110":
                    hRowMark = "10"; gRowMarkH = "11"; gRowMarkM = "24"; break;
                case "110R":
                    hRowMark = "11"; gRowMarkH = "12"; gRowMarkM = "25"; break;
                case "35":
                    hRowMark = "12"; gRowMarkH = "13"; gRowMarkM = "26"; break;
                case "32":
                    hRowMark = "13"; gRowMarkH = "14"; gRowMarkM = "27"; break;
                case "120":
                    hRowMark = "14"; gRowMarkH = "15"; gRowMarkM = "28"; break;
                case "120R":
                    hRowMark = "15"; gRowMarkH = "16"; gRowMarkM = "29"; break;
                case "36":
                    hRowMark = "16"; gRowMarkH = "17"; gRowMarkM = "30"; break;
                case "33":
                    hRowMark = "17"; gRowMarkH = "18"; gRowMarkM = "31"; break;
            }
        }
        else
        {
            //
            switch (paramMarkPriv)
            {
                //
                case "101":
                    hRowMark = "6"; gRowMarkH = "7"; gRowMarkM = "17"; break;
                case "101R":
                    hRowMark = "7"; gRowMarkH = "8"; gRowMarkM = "18"; break;
                case "35":
                    hRowMark = "8"; gRowMarkH = "9"; gRowMarkM = "19"; break;
                case "32":
                    hRowMark = "9"; gRowMarkH = "10"; gRowMarkM = "20"; break;
                case "110":
                    hRowMark = "10"; gRowMarkH = "11"; gRowMarkM = "21"; break;
                case "110R":
                    hRowMark = "11"; gRowMarkH = "12"; gRowMarkM = "22"; break;
                case "120":
                    hRowMark = "12"; gRowMarkH = "13"; gRowMarkM = "23"; break;
                case "36":
                    hRowMark = "13"; gRowMarkH = "14"; gRowMarkM = "24"; break;
                case "33":
                    hRowMark = "14"; gRowMarkH = "15"; gRowMarkM = "25"; break;
            }
        }

        paramPP54 = wsH.Cells[ColDN + hRowMark].Value.ToString();
        paramPP55 = wsH.Cells["N" + hRowMark].Value.ToString();
        paramPP56 = wsH.Cells["O" + hRowMark].Value.ToString();
        paramPP57 = wsH.Cells["P" + hRowMark].Value.ToString();
        paramPP58 = wsH.Cells["Q" + hRowMark].Value.ToString();
        paramPP59 = wsH.Cells["R" + hRowMark].Value.ToString();
        paramPP60 = wsH.Cells["S" + hRowMark].Value.ToString();
        paramPP61 = wsH.Cells["T" + hRowMark].Value.ToString();
        paramPP62 = wsH.Cells["U" + hRowMark].Value.ToString();
        paramPP63 = wsH.Cells["V" + hRowMark].Value.ToString();

        paramPP65 = wsG.Cells[ColDN + "4"].Value.ToString();
        paramPP66 = wsG.Cells[ColDN + "5"].Value.ToString();
        paramPP67 = wsG.Cells[ColDN + gRowMarkH].Value.ToString();
        paramPP68 = wsG.Cells[ColDN + gRowMarkM].Value.ToString();
    }

    private string getPrivodName(string privMark)
    {
        switch (privMark)
        {
            case "101": return "TSL-1600-25- 1-230-IP67";
            case "101R": return "TSL-1600-25- 1R-230-IP67";
            case "34": return "TW500- XD220-S.12";
            case "31": return "TW500- XD24-S.12";
            case "110": return "TSL-2200-40- 1-230-IP67";
            case "110R": return "TSL-2200-40- 1R-230-IP67";
            case "120": return "TSL-3000-60- 1-230-IP67";
            case "120R": return "TSL-3000-60- 1R-230-IP67";
            case "35": return "TW1001- XD220-S.14";
            case "32": return "TW1001- XD24-S.14";
            case "9": return "ST 0.1 498.1- OIIAF/00";
            case "13": return "ST 1 491.1- O7KAE/00";
            case "36": return "TW3000- XD220-S.14";
            case "33": return "TW3000- XD24-S.14";
            case "-": return "вариантов нет";
            default: return null;
        }
    }

    private Dictionary<string, string[]> generatedTableV(Dictionary<string, double> g_dict)
    {
        LabelError.Text = "";
        /*BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB*/
        double Kv = 0, Gkl = 0, dPkl = 0, dPto = 0, g = 0;
        int DN = 0;
        double Kv_start = 0;
        double tmpKv = 0;
        string tmpA = "";
        Dictionary<string, string[]> listResult = new Dictionary<string, string[]>();
        listResult.Add("A", new string[] { });
        listResult.Add("C", new string[] { });
        listResult.Add("B", new string[] { });
        listResult.Add("D", new string[] { });
        listResult.Add("I1", new string[] { });
        listResult.Add("I2", new string[] { });
        listResult.Add("I", new string[] { });
        listResult.Add("I3", new string[] { });
        listResult.Add("F", new string[] { });
        listResult.Add("G", new string[] { });
        listResult.Add("M", new string[] { });
        //listResult.Add("K", new string[] { });
        //listResult.Add("L", new string[] { });

        listResult.Add("PP54", new string[] { });
        listResult.Add("PP55", new string[] { });
        listResult.Add("PP56", new string[] { });
        listResult.Add("PP57", new string[] { });
        listResult.Add("PP58", new string[] { });
        listResult.Add("PP59", new string[] { });
        listResult.Add("PP60", new string[] { });
        listResult.Add("PP61", new string[] { });
        listResult.Add("PP62", new string[] { });
        listResult.Add("PP63", new string[] { });
        listResult.Add("PP65", new string[] { });
        listResult.Add("PP66", new string[] { });
        listResult.Add("PP67", new string[] { });
        listResult.Add("PP68", new string[] { });

        Gkl = g_dict["p30"];

        dPkl = g_dict["p62"]; //Convert.ToDouble(this.lpvTextBox1.Text) * arrConvert3[this.lpvComboBox1.SelectedIndex - 1];

        dPto = g_dict["p61"];//Convert.ToDouble(this.lpvTextBox2.Text) * arrConvert3[this.lpvComboBox2.SelectedIndex - 1];

        /*
        double middle_T = 0;

        if (this.fvRadioButton1.Checked)
        {
            if (this.ws2RadioButton1.Checked)
            {
                middle_T = double.Parse(this.calcvTextBox2.Text);
            }
            else
            {
                middle_T = Convert.ToDouble(this.ws2TextBox2.Text);
            }
        }
        else
        {
            if (tvRadioButtonList1.SelectedIndex == 0 || this.aaRadioButton1.Checked)
            {
                middle_T = 0.5 * (double.Parse(this.fvTextBox2.Text) + double.Parse(this.fvTextBox3.Text));
            }
            else if (this.aaRadioButton2.Checked)
            {
                middle_T = 0.5 * (double.Parse(this.fvTextBox6.Text) + double.Parse(this.fvTextBox7.Text));
            }
            else if (this.aaRadioButton3.Checked)
            {
                middle_T = 0.5 * (double.Parse(this.fvTextBox8.Text) + double.Parse(this.fvTextBox9.Text));
            }
        }
        */

        if (this.ws2RadioButtonList1.SelectedIndex == 0)
        {
            Water(GetAvgT(), ref g);
        }
        else if (ws2RadioButtonList1.SelectedIndex == 1)
        {
            double p6 = customConverterToDouble(this.ws2TextBox1.Text);
            double p7 = Math.Round(GetAvgT() / 10) * 10;
            double cp = 0;
            Etgl(p7, p6, ref g, ref cp);
        }
        else if (ws2RadioButtonList1.SelectedIndex == 2)
        {
            double p6 = customConverterToDouble(this.ws2TextBox1.Text);
            double p7 = Math.Round(GetAvgT() / 10) * 10;
            double cp = 0;
            Prgl(p7, p6, ref g, ref cp);
        }

        /*
        if (this.ws2RadioButton1.Checked)
        {
            //g = 1000;
            Water(double.Parse(this.calcvTextBox2.Text), ref g);
        }
        else
        {
            double p6 = Convert.ToDouble(this.ws2TextBox1.Text);
            double p7 = Math.Round(Convert.ToDouble(this.ws2TextBox2.Text) / 10) * 10;
            /*foreach (Newtonsoft.Json.Linq.JProperty el in dataFromFile.table4)
            {
                string s = 0 + "";
                string[] mas = el.Name.Split('-');
                if ((Convert.ToDouble(mas[0]) <= p6) && (Convert.ToDouble(mas[1]) >= p6))
                {
                    double tmp_t = 0.0;
                    foreach (Newtonsoft.Json.Linq.JObject val in el.Value)
                    {
                        if (Convert.ToDouble(val.GetValue("t")) == p7)
                        {
                            tmp_t = Convert.ToDouble(val.GetValue("sr"));
                        }
                    }
                    g = tmp_t;
                }
            }* /
            Etgl(p7, p6, ref g);
        }*/

        if (dPkl > dPto)
        {
            Kv_start = Kv = g_dict["vKv"] * (Gkl * 0.01) / (Math.Sqrt(dPkl * 0.001 * g));
        }
        else
        {
            Kv_start = Kv = g_dict["vKv"] * (Gkl * 0.01) / (Math.Sqrt(dPto * 0.001 * g));
        }

        Newtonsoft.Json.Linq.JArray tablev = null;
        Newtonsoft.Json.Linq.JArray tableDN = null;
        Newtonsoft.Json.Linq.JArray tablev_7 = null;
        if (tvRadioButtonList1.SelectedIndex == 0)
        {
            tablev = dataFromFile.table5v;
            tableDN = dataFromFile.table10;
            if (double.Parse(g_dict["p35"].ToString()) <= 150) tablev_7 = dataFromFile.tablev_71;
            else tablev_7 = dataFromFile.tablev_71t;
        }
        else
        {
            tablev = dataFromFile.table6v;
            tableDN = dataFromFile.table11;
            tablev_7 = dataFromFile.tablev_713;
        }
        double col_B = (rpvRadioButtonList1.SelectedIndex == 0) ? Convert.ToDouble(tablev[0]) : Convert.ToDouble(tablev[tablev.Count - 1]); //выбор начальной пропускной способности
        int col_C = Convert.ToInt32(tableDN[tableDN.Count - 1]); //выбор начального максимального диаметра

        bool exit_t = false;


        if (rpvRadioButtonList1.SelectedIndex == 0)
        {
            if (col_B == Convert.ToDouble(tablev[0])) //выбор пропускной способности
            {
                foreach (double el in tablev)
                {
                    if ((el >= col_B) && (el <= Kv))
                    {
                        col_B = el;
                    }
                }
            }
            else
            {
                double col_Bt = Convert.ToDouble(tablev[tablev.Count - 1]);
                foreach (double el in tablev)
                {
                    if ((el <= col_Bt) && (el >= Kv) && (el > col_B))
                    {
                        col_Bt = el;
                    }
                }
                col_B = col_Bt;
            }

            if (col_B == Convert.ToDouble(tablev[0]))
                Kv = col_B;
            //exit_t = true;


            if (Kv < col_B)
            {
                exit_t = true;
                var _List = new List<string>();

                if (listResult.ContainsKey("B"))
                {
                    _List.AddRange(listResult["B"]);
                }

                _List.AddRange(new string[] { "Решение не найдено" });
                listResult["B"] = _List.ToArray();
                return listResult;
            }
            else
            {
                Kv = Math.Round(col_B, 2);

                var _List = new List<string>();

                if (listResult.ContainsKey("B"))
                {
                    _List.AddRange(listResult["B"]);
                }

                _List.AddRange(new string[] { Kv.ToString() });
                listResult["B"] = _List.ToArray();
            }
        }
        else
        {
            if (col_B == Convert.ToDouble(tablev[tablev.Count - 1]))
            {
                foreach (double el in tablev)
                {
                    if ((el <= col_B) && (el >= Kv))
                    {
                        col_B = el;
                    }
                }
            }
            else
            {
                double col_Bt = Convert.ToDouble(tablev[tablev.Count - 1]);
                foreach (double el in tablev)
                {
                    if ((el <= col_Bt) && (el >= Kv) && (el > col_B))
                    {
                        col_Bt = el;
                    }
                }
                col_B = col_Bt;
            }

            if (col_B == Convert.ToDouble(tablev[tablev.Count - 1]))
                Kv = col_B;
            //exit_t = true;


            if (Kv > col_B)
            {
                exit_t = true;
                var _List = new List<string>();

                if (listResult.ContainsKey("B"))
                {
                    _List.AddRange(listResult["B"]);
                }

                _List.AddRange(new string[] { "Решение не найдено" });
                listResult["B"] = _List.ToArray();
                return listResult;
            }
            else
            {
                Kv = Math.Round(col_B, 2);

                var _List = new List<string>();

                if (listResult.ContainsKey("B"))
                {
                    _List.AddRange(listResult["B"]);
                }

                _List.AddRange(new string[] { Kv.ToString() });
                listResult["B"] = _List.ToArray();
            }
        }


        /*/BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB*/

        /*AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA*/
        /*CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/


        if (true)
        {
            List<string> listA = new List<string>(),
                listB = new List<string>(),
                listC = new List<string>(),
                //listK = new List<string>();
                //listL = new List<string>(),
                listM = new List<string>();
            foreach (Newtonsoft.Json.Linq.JObject ob in tablev_7)
            {
                if (Convert.ToDouble(ob.GetValue("b")) == Kv)
                {
                    DN = int.Parse(ob.GetValue("c").ToString());
                    tmpKv = Kv;
                    listA.Add(ob.GetValue("a").ToString());
                    listB.Add(ob.GetValue("b").ToString());
                    listC.Add(ob.GetValue("c").ToString());
                    //listK.Add((DN <= 100)? ((this.aaRadioButton1.Checked)? "2,4 (25)" : "6 (10)") : ob.GetValue("k").ToString());
                    //listL.Add(ob.GetValue("l").ToString());
                    listM.Add(ob.GetValue("m").ToString());
                }
            }

            var a_List = new List<string>();
            if (listResult.ContainsKey("A"))
            {
                a_List.AddRange(listResult["A"]);
            }

            a_List.AddRange(listA);
            listResult["A"] = a_List.ToArray();

            if (listResult.ContainsKey("B"))
            {
                listResult["B"] = listB.ToArray();
            }

            var c_List = new List<string>();
            if (listResult.ContainsKey("C"))
            {
                c_List.AddRange(listResult["C"]);
            }

            c_List.AddRange(listC);
            listResult["C"] = c_List.ToArray();

            /*
            var k_List = new List<string>();
            if (listResult.ContainsKey("K"))
            {
                k_List.AddRange(listResult["K"]);
            }

            k_List.AddRange(listK);
            listResult["K"] = k_List.ToArray();


            var l_List = new List<string>();
            if (listResult.ContainsKey("L"))
            {
                l_List.AddRange(listResult["L"]);
            }

            l_List.AddRange(listL);
            listResult["L"] = l_List.ToArray();
            */

            var m_List = new List<string>();
            if (listResult.ContainsKey("M"))
            {
                m_List.AddRange(listResult["M"]);
            }

            m_List.AddRange(listM);
            listResult["M"] = m_List.ToArray();

        }
        /*/CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/
        /*/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA*/

        double C = Convert.ToDouble(listResult["C"][listResult["C"].Count() - 1]),
                V = Gkl * convertTable[1, 5] * Math.Pow((18.8 / C), 2);


        double Pf = 1;

        while (!exit_t && (V >= g_dict["vmax"]))
        {
            if (exit_t)
                break;
            else
            {
                // DN ближайший больший из table10 выбор диаметра DN

                if (col_C == Convert.ToDouble(tableDN[tableDN.Count - 1]))
                {
                    foreach (int el in tableDN)
                    {
                        if ((el <= col_C) && (el > DN))
                        {
                            col_C = el;
                        }
                    }
                }
                else
                {
                    int col_Ct = Convert.ToInt32(tableDN[tableDN.Count - 1]);
                    foreach (int el in tableDN)
                    {
                        if ((el <= col_Ct) && (el >= DN) && (el > col_C))
                        {
                            col_Ct = el;
                        }
                    }
                    col_C = col_Ct;
                }

                bool meetEnd = false;

                if (col_C == Convert.ToDouble(tableDN[tableDN.Count - 1]))
                {
                    exit_t = true;

                    foreach (string keyValue in listResult["C"])
                    {
                        if (keyValue.Equals(tableDN[tableDN.Count - 1].ToString()))
                            meetEnd = true;
                    }
                }

                if (meetEnd) break;


                if (DN > col_C)
                {
                    exit_t = true;
                    var _List = new List<string>();

                    if (listResult.ContainsKey("C"))
                    {
                        _List.AddRange(listResult["C"]);
                    }

                    _List.AddRange(new string[] { "Решение не найдено" });
                    listResult["C"] = _List.ToArray();
                    return listResult;
                }
                else
                {
                    DN = col_C;

                    var _List = new List<string>();

                    if (listResult.ContainsKey("C"))
                    {
                        _List.AddRange(listResult["C"]);
                    }

                    _List.AddRange(new string[] { DN.ToString() });
                    listResult["C"] = _List.ToArray();
                }
                /*/CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/

                /*AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA*/
                /*CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/

                if (true)
                {
                    List<string> listA = new List<string>(),
                        listB = new List<string>(),
                        //listK = new List<string>(),
                        //listL = new List<string>(),
                        listM = new List<string>();


                    tmpKv = 0.16;
                    tmpA = "";
                    foreach (Newtonsoft.Json.Linq.JObject ob in tablev_7)
                    {
                        if (Convert.ToDouble(ob.GetValue("c")) == DN)
                        {
                            if (Kv_start > Convert.ToDouble(ob.GetValue("b")) && tmpKv < Convert.ToDouble(ob.GetValue("b")))
                            {
                                tmpKv = Convert.ToDouble(ob.GetValue("b"));
                            }
                        }
                    }

                    foreach (Newtonsoft.Json.Linq.JObject ob in tablev_7)
                    {
                        if (Convert.ToDouble(ob.GetValue("c")) == DN && Convert.ToDouble(ob.GetValue("b")) == tmpKv)
                        {
                            tmpKv = Convert.ToDouble(ob.GetValue("b"));
                            tmpA = ob.GetValue("a").ToString();
                            listA.Add(tmpA.ToString());
                            listB.Add(tmpKv.ToString());
                            //listK.Add((DN <= 100) ? ((this.aaRadioButton1.Checked) ? "2,4 (25)" : "6 (10)") : ob.GetValue("k").ToString());
                            //listL.Add(ob.GetValue("l").ToString());
                            listM.Add(ob.GetValue("m").ToString());
                        }
                    }

                    if (listA.Count() == 0)
                    {
                        foreach (Newtonsoft.Json.Linq.JObject ob in tablev_7)
                        {
                            if (Convert.ToDouble(ob.GetValue("c")) == DN && Convert.ToDouble(ob.GetValue("b")) > tmpKv)
                            {
                                tmpKv = Convert.ToDouble(ob.GetValue("b"));
                                tmpA = ob.GetValue("a").ToString();
                                listA.Add(tmpA.ToString());
                                listB.Add(tmpKv.ToString());
                                //listK.Add((DN <= 100) ? ((this.aaRadioButton1.Checked) ? "2,4 (25)" : "6 (10)") : ob.GetValue("k").ToString());
                                //listL.Add(ob.GetValue("l").ToString());
                                listM.Add(ob.GetValue("m").ToString());
                                break;
                            }
                        }
                    }

                    var a_List = new List<string>();
                    if (listResult.ContainsKey("A"))
                    {
                        a_List.AddRange(listResult["A"]);
                    }

                    a_List.AddRange(listA);
                    listResult["A"] = a_List.ToArray();

                    var b_List = new List<string>();
                    if (listResult.ContainsKey("B"))
                    {
                        b_List.AddRange(listResult["B"]);
                    }

                    b_List.AddRange(listB);
                    listResult["B"] = b_List.ToArray();

                    /*
                    var k_List = new List<string>();
                    if (listResult.ContainsKey("K"))
                    {
                        k_List.AddRange(listResult["K"]);
                    }

                    k_List.AddRange(listK);
                    listResult["K"] = k_List.ToArray();


                    var l_List = new List<string>();
                    if (listResult.ContainsKey("L"))
                    {
                        l_List.AddRange(listResult["L"]);
                    }

                    l_List.AddRange(listL);
                    listResult["L"] = l_List.ToArray();
                    */

                    var m_List = new List<string>();
                    if (listResult.ContainsKey("M"))
                    {
                        m_List.AddRange(listResult["M"]);
                    }

                    m_List.AddRange(listM);
                    listResult["M"] = m_List.ToArray();


                    if (listA.Count == 0)
                    {
                        var c_List = new List<string>();
                        if (listResult.ContainsKey("C"))
                        {
                            c_List.AddRange(listResult["C"]);
                            c_List.RemoveAt(c_List.Count() - 1);
                        }

                        listResult["C"] = c_List.ToArray();
                    }
                }
                /*/CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/
                /*/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA*/
            }

            V = Gkl * convertTable[1, 5] * Math.Pow((18.8 / DN), 2);
        }


        /*FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF*/
        /*GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG*/
        /*IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII*/
        List<string> listI = new List<string>(),
            listI1 = new List<string>(),
            listI2 = new List<string>(),
            listI3 = new List<string>(),
            listF = new List<string>(),
            listG = new List<string>(),
            listD = new List<string>();

        listI.AddRange(listResult["I"]);
        listI1.AddRange(listResult["I1"]);
        listI2.AddRange(listResult["I2"]);
        listI3.AddRange(listResult["I3"]);
        listF.AddRange(listResult["F"]);
        listG.AddRange(listResult["G"]);
        listD.AddRange(listResult["D"]);

        for (int i = 0; i < listResult["C"].Count(); i++)
        {

            /*DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD*/
            //Pf = g / 1000 * (Math.Pow(Gkl, 2) * 0.1) / (Math.Pow(double.Parse(listResult["B"].GetValue(i).ToString()), 2) * g);
            Pf = (Math.Pow(Gkl, 2) * 0.1) / (Math.Pow(double.Parse(listResult["B"].GetValue(i).ToString()), 2) * g);
            double dPf = Pf / 100;
            Pf = Math.Round(dPf, 2); /*Перевод с кПа в бар*/

            listD.Add(Pf.ToString());
            /*/DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD*/


            C = Convert.ToDouble(listResult["C"][i]);
            V = Gkl * convertTable[1, 5] * Math.Pow((18.8 / C), 2);

            if (V <= g_dict["vmax"] || V >= 7)
            {
                exit_t = true;
            }

            listI.Add(Math.Round(V, 2).ToString());

            double Ia = dPf / (dPf + dPto / 100);

            listI1.Add(Math.Round(Ia, 2).ToString());

            if (Ia >= 0.5)
            {
                listI2.Add("хорошее");
            }
            else
            {
                listI2.Add("плохое");
            }

            if (V > 3 && V <= 5 && spvRadioButtonList1.SelectedIndex == 1 ) //g_dict["vmax"])
            {
                listI3.Add("возможен шум");
            }
            else if (V > 5) // g_dict["vmax"])
            {
                listI3.Add("возможен эрозийный износ клапана");
            }

            else if (V < 1.5)
            {
                listI3.Add("возможен колебательный режим регулирования");
            }
            else
            {
                listI3.Add("нет");
            }

            if (!String.IsNullOrWhiteSpace(this.calcvTextBox1.Text) && !String.IsNullOrWhiteSpace(this.calcvTextBox2.Text))
            {
                double dn = 0.0;
                double ps = 0.0;
                foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table8)
                {
                    if (C == Convert.ToDouble(ob.GetValue("dn")))
                    {
                        dn = Convert.ToDouble(ob.GetValue("z"));
                        break;
                    }
                }

                double t1 = customConverterToDouble(this.calcvTextBox2.Text);
                Newtonsoft.Json.Linq.JObject max = dataFromFile.table9v[dataFromFile.table9v.Count - 1];
                foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9v)
                {
                    if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1))
                    {
                        max = ob;
                    }
                }
                ps = Convert.ToDouble(max.GetValue("ps"));

                double F = Math.Round((dn * ((customConverterToDouble(this.calcvTextBox1.Text) * arrConvert3[this.calcvDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - ps)), 2);
                listF.Add(F.ToString());

                string G_str = "Нет";
                if (F < Pf)
                    G_str = "Угрожает опасность кавитации";
                //if (F < (customConverterToDouble(this.lpvTextBox1.Text) * arrConvert3[this.calcvDropDownList1.SelectedIndex - 1] / arrConvert3[2]))
                //    G_str = "Угрожает опасность кавитации";

                listG.Add(G_str);
            }
        }
        listResult["I"] = listI.ToArray();
        listResult["I1"] = listI1.ToArray();
        listResult["I2"] = listI2.ToArray();
        listResult["I3"] = listI3.ToArray();
        listResult["F"] = listF.ToArray();
        listResult["G"] = listG.ToArray();
        listResult["D"] = listD.ToArray();
        /*/IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII*/
        /*/GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG*/
        /*/FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF*/

        //


        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        
        if (!File.Exists(HttpContext.Current.Server.MapPath(@"Content/properties/htrv.xlsx")))
        {
            LabelError.Text += "Не найден файл характеристик";

            return null;
        }
        else if (!File.Exists(HttpContext.Current.Server.MapPath(@"Content/properties/htrv3.xlsx")))
        {
            LabelError.Text += "Не найден файл характеристик";
            return null;
        }
        else if (!File.Exists(HttpContext.Current.Server.MapPath(@"Content/properties/gtrv.xlsx")))
        {
            LabelError.Text += "Не найден файл габаритов";
            return null;
        }
        else if (!File.Exists(HttpContext.Current.Server.MapPath(@"Content/properties/gtrv3.xlsx")))
        {
            LabelError.Text += "Не найден файл габаритов";
            return null;
        }

        ExcelFile efHtrv = ExcelFile.Load(HttpContext.Current.Server.MapPath(((tvRadioButtonList1.SelectedIndex == 0) ? "Content\\properties\\htrv.xlsx" : "\\Content\\properties\\htrv3.xlsx")));
        ExcelWorksheet wsHtrv = efHtrv.Worksheets[0];

        ExcelFile efGtrv = ExcelFile.Load(HttpContext.Current.Server.MapPath(((tvRadioButtonList1.SelectedIndex == 0) ? ((customConverterToDouble(g_dict["p35"].ToString()) > 150) ? "Content\\properties\\gtrvt.xlsx" : "Content\\properties\\gtrv.xlsx") : "Content\\properties\\gtrv3.xlsx")));
        ExcelWorksheet wsGtrv = efGtrv.Worksheets[0];

        //ws.Cells["C4"].Value = r_input_dict[3];

        string tmpMarkPriv = "";
        string tmpPriv = "";
        string tmpPP54 = "";
        string tmpPP55 = "";
        string tmpPP56 = "";
        string tmpPP57 = "";
        string tmpPP58 = "";
        string tmpPP59 = "";
        string tmpPP60 = "";
        string tmpPP61 = "";
        string tmpPP62 = "";
        string tmpPP63 = "";

        string tmpPP65 = "";
        string tmpPP66 = "";
        string tmpPP67 = "";
        string tmpPP68 = "";

        List<string> listPP54 = new List<string>(),
            listPP55 = new List<string>(),
            listPP56 = new List<string>(),
            listPP57 = new List<string>(),
            listPP58 = new List<string>(),
            listPP59 = new List<string>(),
            listPP60 = new List<string>(),
            listPP61 = new List<string>(),
            listPP62 = new List<string>(),
            listPP63 = new List<string>(),
            listPP65 = new List<string>(),
            listPP66 = new List<string>(),
            listPP67 = new List<string>(),
            listPP68 = new List<string>();

        //for (int i = 0; i < listResult.ElementAt(2).Value.Count(); i++)
        for (int i = 0; i < listResult["C"].Count(); i++)
        {

            tmpMarkPriv = tmpPriv = tmpPP54 = tmpPP55 = tmpPP56 = tmpPP57 = tmpPP58 = tmpPP59 = tmpPP60 = tmpPP61 = tmpPP62 = tmpPP63 = tmpPP65 = tmpPP66 = tmpPP67 = tmpPP68 = "";

            // TRV
            if (tvRadioButtonList1.SelectedIndex == 0)
            {
                // 230 VAC ; 3-pos ; no ; no
                if (/*listResult["C"].ElementAt(i).Equals("15") && */tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 0 && tdRadioButtonList3.SelectedIndex == 1 && tdRadioButtonList4.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "101"; break;
                        case "20":
                            tmpMarkPriv = "101"; break;
                        case "25":
                            tmpMarkPriv = "101"; break;
                        case "32":
                            tmpMarkPriv = "101"; break;
                        case "40":
                            tmpMarkPriv = "101"; break;
                        case "50":
                            tmpMarkPriv = "101"; break;
                        case "65":
                            tmpMarkPriv = "110"; break;
                        case "80":
                            tmpMarkPriv = "110"; break;
                        case "100":
                            tmpMarkPriv = "110"; break;
                        case "125":
                            tmpMarkPriv = "120"; break;
                        case "150":
                            tmpMarkPriv = "120"; break;
                    }
                }
                // 230 VAC ; 3-pos ; no ; yes
                else if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 0 && tdRadioButtonList3.SelectedIndex == 1 && tdRadioButtonList4.SelectedIndex == 0)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "101R"; break;
                        case "20":
                            tmpMarkPriv = "101R"; break;
                        case "25":
                            tmpMarkPriv = "101R"; break;
                        case "32":
                            tmpMarkPriv = "101R"; break;
                        case "40":
                            tmpMarkPriv = "101R"; break;
                        case "50":
                            tmpMarkPriv = "101R"; break;
                        case "65":
                            tmpMarkPriv = "110R"; break;
                        case "80":
                            tmpMarkPriv = "110R"; break;
                        case "100":
                            tmpMarkPriv = "110R"; break;
                        case "125":
                            tmpMarkPriv = "-"; break;
                        case "150":
                            tmpMarkPriv = "-"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 230 VAC ; 3-pos ; yes ; no
                else if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 0 && tdRadioButtonList3.SelectedIndex == 0 && tdRadioButtonList4.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "34"; break;
                        case "20":
                            tmpMarkPriv = "34"; break;
                        case "25":
                            tmpMarkPriv = "34"; break;
                        case "32":
                            tmpMarkPriv = "34"; break;
                        case "40":
                            tmpMarkPriv = "34"; break;
                        case "50":
                            tmpMarkPriv = "34"; break;
                        case "65":
                            tmpMarkPriv = "35"; break;
                        case "80":
                            tmpMarkPriv = "35"; break;
                        case "100":
                            tmpMarkPriv = "35"; break;
                        case "125":
                            tmpMarkPriv = "36"; break;
                        case "150":
                            tmpMarkPriv = "36"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 230 VAC ; analog ; no ; no
                else if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 1 && tdRadioButtonList3.SelectedIndex == 1 && tdRadioButtonList4.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "34"; break;
                        case "20":
                            tmpMarkPriv = "34"; break;
                        case "25":
                            tmpMarkPriv = "34"; break;
                        case "32":
                            tmpMarkPriv = "34"; break;
                        case "40":
                            tmpMarkPriv = "34"; break;
                        case "50":
                            tmpMarkPriv = "34"; break;
                        case "65":
                            tmpMarkPriv = "35"; break;
                        case "80":
                            tmpMarkPriv = "35"; break;
                        case "100":
                            tmpMarkPriv = "35"; break;
                        case "125":
                            tmpMarkPriv = "36"; break;
                        case "150":
                            tmpMarkPriv = "36"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 230 VAC ; analog ; yes ; no
                else if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 1 && tdRadioButtonList3.SelectedIndex == 0 && tdRadioButtonList4.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "34"; break;
                        case "20":
                            tmpMarkPriv = "34"; break;
                        case "25":
                            tmpMarkPriv = "34"; break;
                        case "32":
                            tmpMarkPriv = "34"; break;
                        case "40":
                            tmpMarkPriv = "34"; break;
                        case "50":
                            tmpMarkPriv = "34"; break;
                        case "65":
                            tmpMarkPriv = "35"; break;
                        case "80":
                            tmpMarkPriv = "35"; break;
                        case "100":
                            tmpMarkPriv = "35"; break;
                        case "125":
                            tmpMarkPriv = "36"; break;
                        case "150":
                            tmpMarkPriv = "36"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 24 VAC/VDC 
                else if (tdRadioButtonList1.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "31"; break;
                        case "20":
                            tmpMarkPriv = "31"; break;
                        case "25":
                            tmpMarkPriv = "31"; break;
                        case "32":
                            tmpMarkPriv = "31"; break;
                        case "40":
                            tmpMarkPriv = "31"; break;
                        case "50":
                            tmpMarkPriv = "31"; break;
                        case "65":
                            tmpMarkPriv = "32"; break;
                        case "80":
                            tmpMarkPriv = "32"; break;
                        case "100":
                            tmpMarkPriv = "32"; break;
                        case "125":
                            tmpMarkPriv = "33"; break;
                        case "150":
                            tmpMarkPriv = "33"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
            }
            // TRV-3
            else if (tvRadioButtonList1.SelectedIndex == 1)
            {
                // 230 VAC ; 3-pos ; no ; no
                if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 0 && tdRadioButtonList3.SelectedIndex == 1 && tdRadioButtonList4.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "101"; break;
                        case "20":
                            tmpMarkPriv = "101"; break;
                        case "25":
                            tmpMarkPriv = "101"; break;
                        case "32":
                            tmpMarkPriv = "101"; break;
                        case "40":
                            tmpMarkPriv = "101"; break;
                        case "50":
                            tmpMarkPriv = "101"; break;
                        case "65":
                            tmpMarkPriv = "110"; break;
                        case "80":
                            tmpMarkPriv = "110"; break;
                        case "100":
                            tmpMarkPriv = "110"; break;
                        case "125":
                            tmpMarkPriv = "120"; break;
                        case "150":
                            tmpMarkPriv = "120"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 230 VAC ; 3-pos ; no ; yes
                else if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 0 && tdRadioButtonList3.SelectedIndex == 1 && tdRadioButtonList4.SelectedIndex == 0)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "101R"; break;
                        case "20":
                            tmpMarkPriv = "101R"; break;
                        case "25":
                            tmpMarkPriv = "101R"; break;
                        case "32":
                            tmpMarkPriv = "101R"; break;
                        case "40":
                            tmpMarkPriv = "101R"; break;
                        case "50":
                            tmpMarkPriv = "101R"; break;
                        case "65":
                            tmpMarkPriv = "110R"; break;
                        case "80":
                            tmpMarkPriv = "110R"; break;
                        case "100":
                            tmpMarkPriv = "110R"; break;
                        case "125":
                            tmpMarkPriv = "-"; break;
                        case "150":
                            tmpMarkPriv = "-"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 230 VAC ; 3-pos ; yes ; no
                else if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 0 && tdRadioButtonList3.SelectedIndex == 0 && tdRadioButtonList4.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "35"; break;
                        case "20":
                            tmpMarkPriv = "35"; break;
                        case "25":
                            tmpMarkPriv = "35"; break;
                        case "32":
                            tmpMarkPriv = "35"; break;
                        case "40":
                            tmpMarkPriv = "35"; break;
                        case "50":
                            tmpMarkPriv = "36"; break;
                        case "65":
                            tmpMarkPriv = "36"; break;
                        case "80":
                            tmpMarkPriv = "36"; break;
                        case "100":
                            tmpMarkPriv = "36"; break;
                        case "125":
                            tmpMarkPriv = "36"; break;
                        case "150":
                            tmpMarkPriv = "36"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 230 VAC ; analog ; no ; no
                else if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 1 && tdRadioButtonList3.SelectedIndex == 1 && tdRadioButtonList4.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "35"; break;
                        case "20":
                            tmpMarkPriv = "35"; break;
                        case "25":
                            tmpMarkPriv = "35"; break;
                        case "32":
                            tmpMarkPriv = "35"; break;
                        case "40":
                            tmpMarkPriv = "35"; break;
                        case "50":
                            tmpMarkPriv = "36"; break;
                        case "65":
                            tmpMarkPriv = "36"; break;
                        case "80":
                            tmpMarkPriv = "36"; break;
                        case "100":
                            tmpMarkPriv = "36"; break;
                        case "125":
                            tmpMarkPriv = "36"; break;
                        case "150":
                            tmpMarkPriv = "36"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 230 VAC ; analog ; yes ; no
                else if (tdRadioButtonList1.SelectedIndex == 0 && tdRadioButtonList2.SelectedIndex == 1 && tdRadioButtonList3.SelectedIndex == 0 && tdRadioButtonList4.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "35"; break;
                        case "20":
                            tmpMarkPriv = "35"; break;
                        case "25":
                            tmpMarkPriv = "35"; break;
                        case "32":
                            tmpMarkPriv = "35"; break;
                        case "40":
                            tmpMarkPriv = "35"; break;
                        case "50":
                            tmpMarkPriv = "36"; break;
                        case "65":
                            tmpMarkPriv = "36"; break;
                        case "80":
                            tmpMarkPriv = "36"; break;
                        case "100":
                            tmpMarkPriv = "36"; break;
                        case "125":
                            tmpMarkPriv = "36"; break;
                        case "150":
                            tmpMarkPriv = "36"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
                // 24 VAC/VDC 
                else if (tdRadioButtonList1.SelectedIndex == 1)
                {
                    switch (listResult["C"].ElementAt(i))
                    {
                        case "15":
                            tmpMarkPriv = "32"; break;
                        case "20":
                            tmpMarkPriv = "32"; break;
                        case "25":
                            tmpMarkPriv = "32"; break;
                        case "32":
                            tmpMarkPriv = "32"; break;
                        case "40":
                            tmpMarkPriv = "32"; break;
                        case "50":
                            tmpMarkPriv = "33"; break;
                        case "65":
                            tmpMarkPriv = "33"; break;
                        case "80":
                            tmpMarkPriv = "33"; break;
                        case "100":
                            tmpMarkPriv = "33"; break;
                        case "125":
                            tmpMarkPriv = "33"; break;
                        case "150":
                            tmpMarkPriv = "33"; break;
                        default:
                            tmpMarkPriv = null; break;
                    }
                }
            }

            if (tmpMarkPriv is null) return null;
            else tmpPriv = getPrivodName(tmpMarkPriv);
            if (tmpPriv is null) return null;

            if (tmpMarkPriv == "-")
            {

                listResult["A"].SetValue("-", i);
                listResult["B"].SetValue("-", i);
                listResult["C"].SetValue("-", i);
                listResult["D"].SetValue("-", i);
                listResult["I"].SetValue("-", i);
                listResult["I1"].SetValue("-", i);
                listResult["I2"].SetValue("-", i);
                listResult["I3"].SetValue("-", i);
                listResult["F"].SetValue("-", i);
                listResult["G"].SetValue("-", i);
                listResult["M"].SetValue(tmpPriv, i);

            }
            else
            {
                //
                getDimsV(tvRadioButtonList1.SelectedIndex == 0, listResult["C"].ElementAt(i), tmpMarkPriv, ref wsHtrv, ref wsGtrv,
                            ref tmpPP54, ref tmpPP55, ref tmpPP56, ref tmpPP57, ref tmpPP58, ref tmpPP59, ref tmpPP60,
                            ref tmpPP61, ref tmpPP62, ref tmpPP63, ref tmpPP65, ref tmpPP66, ref tmpPP67, ref tmpPP68);

                System.Text.RegularExpressions.Regex regex = null;

              
                if (tvRadioButtonList1.SelectedIndex == 0 && (customConverterToDouble(g_dict["p35"].ToString()) <= 150)) regex = new System.Text.RegularExpressions.Regex(@"(TRV-[0-9]+-[0-9,.]+-)");
                else if (tvRadioButtonList1.SelectedIndex == 0 && (customConverterToDouble(g_dict["p35"].ToString()) > 150)) regex = new System.Text.RegularExpressions.Regex(@"(TRV-T-[0-9]+-[0-9,.]+-)");

                else regex = new System.Text.RegularExpressions.Regex(@"(TRV-3-[0-9]+-[0-9,.]+-)");

                System.Text.RegularExpressions.Match match = regex.Match(listResult["A"].ElementAt(i));
                if (match.Success && !(tmpMarkPriv.Equals("-")))
                {
                    listResult["A"].SetValue(match.Value + tmpMarkPriv, i);
                    listResult["M"].SetValue(tmpPriv, i);
                }
            }

            //
            listPP54.Add(tmpPP54);
            listPP55.Add(tmpPP55);
            listPP56.Add(tmpPP56);
            listPP57.Add(tmpPP57);
            listPP58.Add(tmpPP58);
            listPP59.Add(tmpPP59);
            listPP60.Add(tmpPP60);
            listPP61.Add(tmpPP61);
            listPP62.Add(tmpPP62);
            listPP63.Add(tmpPP63);
            listPP65.Add(tmpPP65);
            listPP66.Add(tmpPP66);
            listPP67.Add(tmpPP67);
            listPP68.Add(tmpPP68);

        }

        listResult["PP54"] = listPP54.ToArray();
        listResult["PP55"] = listPP55.ToArray();
        listResult["PP56"] = listPP56.ToArray();
        listResult["PP57"] = listPP57.ToArray();
        listResult["PP58"] = listPP58.ToArray();
        listResult["PP59"] = listPP59.ToArray();
        listResult["PP60"] = listPP60.ToArray();
        listResult["PP61"] = listPP61.ToArray();
        listResult["PP62"] = listPP62.ToArray();
        listResult["PP63"] = listPP63.ToArray();
        listResult["PP65"] = listPP65.ToArray();
        listResult["PP66"] = listPP66.ToArray();
        listResult["PP67"] = listPP67.ToArray();
        listResult["PP68"] = listPP68.ToArray();

        return listResult;
    }

    private void mapInputParametersV(ref Dictionary<int, string> v_in_dict)
    {
        //
        v_in_dict.Add(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        v_in_dict.Add(1, DateTime.Now.ToShortDateString().ToString());
        v_in_dict.Add(2, "-"); // Объект добавляется в диалоговом окне при сохранении

        IEnumerable<RadioButton> ie_rb = null;

        
        v_in_dict.Add(3, spvRadioButtonList1.Items[spvRadioButtonList1.SelectedIndex].Text);

        v_in_dict.Add(4, rpvRadioButtonList1.Items[rpvRadioButtonList1.SelectedIndex].Text);

        string aaText = "";

        if (aaRadioButton1.Checked)
        {
            aaText = aaRadioButton1.Text;
        } 
        else if (aaRadioButton2.Checked)
        {
            aaText = aaRadioButton2.Text;
        }
        else if (aaRadioButton3.Checked)
        {
            aaText = aaRadioButton3.Text;
        }
        
        v_in_dict.Add(5, aaText);

        if (this.aa1RadioButtonList1.SelectedIndex == 0) v_in_dict.Add(6, aa1RadioButtonList1.Items[0].Text);
        else if (this.aa1RadioButtonList1.SelectedIndex == 1) v_in_dict.Add(6, aa1RadioButtonList1.Items[1].Text);
        else if (aa2RadioButtonList1.SelectedIndex == 0) v_in_dict.Add(6, aa2RadioButtonList1.Items[0].Text);
        else if (aa2RadioButtonList1.SelectedIndex == 1) v_in_dict.Add(6, aa1RadioButtonList1.Items[1].Text);
        else if (aa3RadioButtonList1.SelectedIndex == 0) v_in_dict.Add(6, aa3RadioButtonList1.Items[0].Text);
        else if (aa3RadioButtonList1.SelectedIndex == 1) v_in_dict.Add(6, aa3RadioButtonList1.Items[1].Text);

        
        v_in_dict.Add(7, tvRadioButtonList1.Items[tvRadioButtonList1.SelectedIndex].Text);

        v_in_dict.Add(8, "Marka"); // Марка добавляется в диалоговом окне при сохранении

        
        v_in_dict.Add(9, ws2RadioButtonList1.Items[ws2RadioButtonList1.SelectedIndex].Text + " " + ((this.ws2TextBox1.Enabled) ? (this.ws2TextBox1.Text + " %, " + this.ws2TextBox2.Text + " °С") : ""));

        v_in_dict.Add(10, (this.lpvTextBox2.Enabled) ? this.lpvTextBox2.Text : "-");
        v_in_dict.Add(11, (this.lpvTextBox2.Enabled) ? this.lpvDropDownList2.Text : "-");

        v_in_dict.Add(12, (this.lpvTextBox21.Enabled) ? this.lpvTextBox21.Text : "-");
        v_in_dict.Add(13, (this.lpvTextBox21.Enabled) ? this.lpvDropDownList21.Text : "-");

        v_in_dict.Add(14, (this.lpvTextBox1.Enabled) ? this.lpvTextBox1.Text : "-");
        v_in_dict.Add(15, (this.lpvTextBox1.Enabled) ? this.lpvDropDownList1.Text : "-");

        v_in_dict.Add(16, (this.calcvTextBox1.Enabled) ? this.calcvTextBox1.Text : "-");
        v_in_dict.Add(17, (this.calcvTextBox1.Enabled) ? this.calcvDropDownList1.Text : "-");

        v_in_dict.Add(18, (this.calcvTextBox2.Text != "") ? this.calcvTextBox2.Text : "-");

        // пар

        v_in_dict.Add(19, "-");
        v_in_dict.Add(20, "-");

        v_in_dict.Add(21, "-");
        v_in_dict.Add(22, "-");

        v_in_dict.Add(23, "-");

        // пар

        v_in_dict.Add(24, (this.fvTextBox2.Enabled) ? this.fvTextBox2.Text : "-");
        v_in_dict.Add(25, (this.fvTextBox3.Enabled) ? this.fvTextBox3.Text : "-");
        v_in_dict.Add(26, (this.fvTextBox4.Enabled) ? this.fvTextBox4.Text : "-");
        v_in_dict.Add(27, (this.fvTextBox5.Enabled) ? this.fvTextBox5.Text : "-");
        v_in_dict.Add(28, (this.fvTextBox6.Enabled) ? this.fvTextBox6.Text : "-");
        v_in_dict.Add(29, (this.fvTextBox7.Enabled) ? this.fvTextBox7.Text : "-");
        v_in_dict.Add(30, (this.fvTextBox8.Enabled) ? this.fvTextBox8.Text : "-");
        v_in_dict.Add(31, (this.fvTextBox9.Enabled) ? this.fvTextBox9.Text : "-");

        v_in_dict.Add(32, (this.fvTextBox10.Enabled) ? this.fvTextBox10.Text : "-");
        v_in_dict.Add(33, (this.fvTextBox10.Enabled) ? this.fvDropDownList2.Text : "-");

        if (this.fvTextBox10.Enabled)
        {
            v_in_dict[34] = this.fvTextBox11.Text;
            v_in_dict[35] = "кг/ч";
        }
        else if (this.fvTextBox1.Enabled)
        {
            v_in_dict[34] = this.fvTextBox1.Text;
            v_in_dict[35] = this.fvDropDownList1.Text;
        }


        if (tdRadioButtonList1.SelectedIndex == 0) v_in_dict[36] = tdRadioButtonList1.Items[0].Text;
        else v_in_dict[36] = tdRadioButtonList1.Items[1].Text;

        if (tdRadioButtonList2.SelectedIndex == 0) v_in_dict[37] = tdRadioButtonList2.Items[0].Text;
        else v_in_dict[37] = tdRadioButtonList2.Items[1].Text;

        if (tdRadioButtonList3.SelectedIndex == 0) v_in_dict[38] = tdRadioButtonList3.Items[0].Text;
        else v_in_dict[38] = tdRadioButtonList3.Items[1].Text;

        if (tdRadioButtonList4.SelectedIndex == 0) v_in_dict[39] = tdRadioButtonList4.Items[0].Text;
        else v_in_dict[39] = tdRadioButtonList4.Items[1].Text;



        if (Convert.ToDouble(v_in_dict[18]) > 150) v_in_dict[40] = "220 ˚С";
        else v_in_dict[40] = "150 ˚С";
        //if (this.tvRadioButton1.Checked) v_in_dict[40] = "220 ˚С";
        //else v_in_dict[40] = "150 ˚С";


        v_in_dict.Add(41, "16 бар");


        v_in_dict.Add(42, "-");
        v_in_dict.Add(43, "-");
        v_in_dict.Add(44, "-");
        v_in_dict.Add(45, "-");
        v_in_dict.Add(46, "-");
        v_in_dict.Add(47, "-");
        v_in_dict.Add(48, "-");
        v_in_dict.Add(49, "-");
        v_in_dict.Add(51, "-");
        v_in_dict.Add(52, "-");
    }

    private void ResetColorToAllControls()
    {
        fvTextBox1.BackColor = SystemColors.Window;
        fvTextBox2.BackColor = SystemColors.Window;
        fvTextBox3.BackColor = SystemColors.Window;
        fvTextBox4.BackColor = SystemColors.Window;
        fvTextBox5.BackColor = SystemColors.Window;
        fvTextBox6.BackColor = SystemColors.Window;
        fvTextBox7.BackColor = SystemColors.Window;
        fvTextBox8.BackColor = SystemColors.Window;
        fvTextBox9.BackColor = SystemColors.Window;
    }

    protected void vButton_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) { return; }
        try { 
        //ResetColorToAllControls();
        DisableTextBox(objTextBox1);
        objTextBox1.Enabled = false;
        GridView2.Columns.Clear();
        GridView2.DataSource = null;
        GridView2.DataBind();
        GridView2.SelectedIndex = -1;
        readFile(0);
        Dictionary<string, double> g_dict = new Dictionary<string, double>();
        v_input_dict.Clear();
        LabelError.Text = "";

        if (spvRadioButtonList1.SelectedIndex != -1)
        {
            if (spvRadioButtonList1.SelectedIndex == 0) g_dict.Add("vmax", 5); else g_dict.Add("vmax", 3);

            if (rpvRadioButtonList1.SelectedIndex != -1)
            {
                if (rpvRadioButtonList1.SelectedIndex == 0) g_dict.Add("vKv", 1.0); else g_dict.Add("vKv", 1.2);

                if (aaRadioButton1.Checked || aaRadioButton2.Checked || aaRadioButton3.Checked)
                {
                    if ((aaRadioButton1.Checked && aa1RadioButtonList1.SelectedIndex != -1)
                        || (aaRadioButton2.Checked && aa2RadioButtonList1.SelectedIndex != -1)
                        || (aaRadioButton3.Checked && aa3RadioButtonList1.SelectedIndex != -1))
                    {

                        if (tvRadioButtonList1.SelectedIndex != -1)
                        {
                            if (tvRadioButtonList1.SelectedIndex == 0) g_dict.Add("vTMax", 220); else g_dict.Add("vTMax", 150);

                            if (ws2RadioButtonList1.SelectedIndex != -1)
                            {
                                if (ws2RadioButtonList1.SelectedIndex == 1|| ws2RadioButtonList1.SelectedIndex == 2)
                                {
                                    Double p14 = -1;
                                    Double p15 = -1;
                                    try
                                    {
                                        p14 = customConverterToDouble(ws2TextBox1.Text);
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text += "Не указано значение концентрации ";
                                        return;
                                    }

                                    if (p14 < 5 || p14 > 65)
                                    {
                                        LabelError.Text += "Неверно указано значение концентрации ";
                                        return;
                                    }
                                    else
                                    {
                                        g_dict.Add("p14", p14);
                                    }


                                    try
                                    {
                                        p15 = customConverterToDouble(ws2TextBox2.Text);
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text += "Не указано значение температуры ";
                                        return;
                                    }

                                    if (p15 < 0 || p15 > 150)
                                    {
                                        LabelError.Text += "Неверно указано значение температуры ";
                                        return;
                                    }
                                    else
                                    {
                                        g_dict.Add("p15", p15);
                                    }
                                }

                                double checkValue;

                                try
                                {
                                    if (this.lpvTextBox2.Enabled)
                                    {
                                        checkValue = customConverterToDouble(this.lpvTextBox2.Text);

                                        if (!(checkValue > 0))
                                        {
                                            LabelError.Text += "Неверно указано значение давления";
                                            return;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    LabelError.Text += "Неверно указано значение давления";
                                    return;
                                }

                                try
                                {
                                    if (this.lpvTextBox21.Enabled)
                                    {
                                        checkValue = customConverterToDouble(this.lpvTextBox21.Text);

                                        if (!(checkValue > 0))
                                        {
                                            LabelError.Text += "Неверно указано значение давления";
                                            return;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    LabelError.Text += "Неверно указано значение давления";
                                    return;
                                }

                                try
                                {
                                    if (this.lpvTextBox1.Enabled)
                                    {
                                        checkValue = customConverterToDouble(this.lpvTextBox1.Text);

                                        if (!(checkValue > 0))
                                        {
                                            LabelError.Text += "Неверно указано значение давления";
                                            return;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    LabelError.Text += "Неверно указано значение давления";
                                    return;
                                }

                                try
                                {
                                    if (this.lpvTextBox2.Enabled)
                                    {
                                        checkValue = customConverterToDouble(this.lpvTextBox2.Text);

                                        if (!(checkValue > 0))
                                        {
                                            LabelError.Text += "Неверно указано значение давления";
                                            return;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    LabelError.Text += "Неверно указано значение давления";
                                    return;
                                }

                                try
                                {
                                    if (this.calcvTextBox1.Enabled)
                                    {
                                        checkValue = customConverterToDouble(this.calcvTextBox1.Text);

                                        if (!(checkValue > 0))
                                        {
                                            LabelError.Text += "Неверно указано значение давления";
                                            return;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    LabelError.Text += "Неверно указано значение давления";
                                    return;
                                }


                                if (fvRadioButton1.Checked || fvRadioButton2.Checked)
                                {

                                    Double p30 = 0;
                                    if (fvRadioButton2.Checked)
                                    {

                                        double checkVal;

                                        try
                                        {
                                            if (this.fvTextBox2.Enabled)
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox2.Text);
                                                if (!(checkVal > 0))
                                                {
                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указано значение температуры";
                                            return;
                                        }

                                        try
                                        {
                                            if (this.fvTextBox3.Enabled)
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox3.Text);
                                                if (!(checkVal > 0))
                                                {
                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указано значение температуры";
                                            return;
                                        }

                                        try
                                        {
                                            if (this.fvTextBox4.Enabled)
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox4.Text);
                                                if (!(checkVal > 0))
                                                {
                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указано значение температуры";
                                            return;
                                        }

                                        try
                                        {
                                            if (this.fvTextBox5.Enabled)
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox5.Text);
                                                if (!(checkVal > 0))
                                                {
                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указано значение температуры";
                                            return;
                                        }
                                        try
                                        {
                                            if (this.fvTextBox6.Enabled)
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox6.Text);
                                                if (!(checkVal > 0))
                                                {
                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указано значение температуры";
                                            return;
                                        }

                                        try
                                        {
                                            if (this.fvTextBox7.Enabled)
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox7.Text);
                                                if (!(checkVal > 0))
                                                {
                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указано значение температуры";
                                            return;
                                        }

                                        try
                                        {
                                            if (this.fvTextBox8.Enabled)
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox8.Text);
                                                if (!(checkVal > 0))
                                                {
                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указано значение температуры";
                                            return;
                                        }

                                        try
                                        {
                                            if (this.fvTextBox9.Enabled)
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox9.Text);
                                                if (!(checkVal > 0))
                                                {
                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указано значение температуры";
                                            return;
                                        }

                                        Double dt = 0;
                                        if (aaRadioButton1.Checked)
                                        {
                                            if (!(this.firstMoreSecondDouble(this.fvTextBox2.Text, this.fvTextBox3.Text)))
                                            {
                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }
                                            else if (!(this.firstMoreSecondDouble(this.fvTextBox4.Text, this.fvTextBox5.Text)))
                                            {
                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }
                                            /*else if (!(this.firstMoreSecondDouble(this.fvTextBox2.Text, this.fvTextBox3.Text) &&
                                                this.firstMoreSecondDouble(this.fvTextBox4.Text, this.fvTextBox5.Text)))
                                            {
                                                this.fvTextBox3.BackColor = Color.LightPink;
                                                this.fvTextBox4.BackColor = Color.LightPink;
                                                this.fvTextBox5.BackColor = Color.LightPink;

                                                ValidationAlertC(fvTextBox2, "Неверно заданы температуры для вычисления расхода", 5000);
                                                return;
                                            }*/
                                            else
                                            {
                                                dt = (customConverterToDouble(fvTextBox2.Text) - customConverterToDouble(fvTextBox3.Text)) > (customConverterToDouble(fvTextBox4.Text) - customConverterToDouble(fvTextBox5.Text)) ?
                                                    (customConverterToDouble(fvTextBox4.Text) - customConverterToDouble(fvTextBox5.Text)) :
                                                    (customConverterToDouble(fvTextBox2.Text) - customConverterToDouble(fvTextBox3.Text));

                                                    LabelError.Text = dt.ToString();
                                            }
                                        }
                                        else if ((aaRadioButton2.Checked && tvRadioButtonList1.SelectedIndex == 0)
                                            || (aaRadioButton2.Checked && tvRadioButtonList1.SelectedIndex == 1 && aa2RadioButtonList1.SelectedIndex == 1)
                                            || (aaRadioButton3.Checked && tvRadioButtonList1.SelectedIndex == 0)
                                            || (aaRadioButton3.Checked && tvRadioButtonList1.SelectedIndex == 1 && aa3RadioButtonList1.SelectedIndex == 1))
                                        {
                                            if (!firstMoreSecondDouble(fvTextBox2.Text, fvTextBox3.Text))
                                            {
                                                //fvTextBox3.BackColor = Color.LightPink;

                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }
                                            else
                                            {
                                                dt = (customConverterToDouble(fvTextBox2.Text) - customConverterToDouble(fvTextBox3.Text));
                                            }
                                        }
                                        else if (aaRadioButton2.Checked && tvRadioButtonList1.SelectedIndex == 1 && aa2RadioButtonList1.SelectedIndex == 0)
                                        {
                                            //double checkVal;

                                            try
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox6.Text);
                                            }
                                            catch (Exception)
                                            {
                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }

                                            try
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox7.Text);
                                            }
                                            catch (Exception)
                                            {
                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }


                                            if (!firstMoreSecondDouble(fvTextBox6.Text, fvTextBox7.Text))
                                            {
                                                //fvTextBox7.BackColor = Color.LightPink;

                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }
                                            else
                                            {
                                                dt = (customConverterToDouble(fvTextBox6.Text) - customConverterToDouble(fvTextBox7.Text));
                                            }
                                        }
                                        else if (aaRadioButton3.Checked && tvRadioButtonList1.SelectedIndex == 1 && aa3RadioButtonList1.SelectedIndex == 0)
                                        {
                                            try
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox8.Text);
                                            }
                                            catch (Exception)
                                            {
                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }

                                            try
                                            {
                                                checkVal = customConverterToDouble(this.fvTextBox9.Text);
                                            }
                                            catch (Exception)
                                            {
                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }

                                            if (!firstMoreSecondDouble(fvTextBox8.Text, fvTextBox9.Text))
                                            {
                                                //fvTextBox9.BackColor = Color.LightPink;

                                                LabelError.Text += "Неверно указано значение температуры";
                                                return;
                                            }
                                            else
                                            {
                                                dt = (customConverterToDouble(fvTextBox8.Text) - customConverterToDouble(fvTextBox9.Text));
                                            }
                                        }


                                        if (fvTextBox2.Enabled && customConverterToDouble(fvTextBox2.Text) > g_dict["vTMax"])
                                        {
                                            LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет"; 
                                            return;
                                        }
                                        else if (fvTextBox3.Enabled && customConverterToDouble(fvTextBox3.Text) > g_dict["vTMax"])
                                        {
                                            LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет"; 
                                            return;
                                        }
                                        else if (fvTextBox4.Enabled && customConverterToDouble(fvTextBox4.Text) > g_dict["vTMax"])
                                        {
                                            LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет";
                                            return;
                                        }
                                        else if (fvTextBox5.Enabled && customConverterToDouble(fvTextBox5.Text) > g_dict["vTMax"])
                                        {
                                            LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет";
                                            return;
                                        }
                                        else if (fvTextBox6.Enabled && customConverterToDouble(fvTextBox6.Text) > g_dict["vTMax"])
                                        {
                                            LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет";
                                            return;
                                        }
                                        else if (fvTextBox7.Enabled && customConverterToDouble(fvTextBox7.Text) > g_dict["vTMax"])
                                        {
                                            LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет";
                                            return;
                                        }
                                        else if (fvTextBox8.Enabled && customConverterToDouble(fvTextBox8.Text) > g_dict["vTMax"])
                                        {
                                            LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет";
                                            return;
                                        }
                                        else if (fvTextBox9.Enabled && customConverterToDouble(fvTextBox9.Text) > g_dict["vTMax"])
                                        {
                                            LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет";
                                            return;
                                        }

                                        if (!String.IsNullOrWhiteSpace(fvTextBox10.Text))
                                        {   
                                            p30 = Math.Round(((customConverterToDouble(fvTextBox10.Text) * arrConvert2[fvDropDownList2.SelectedIndex - 1] * 3.6) / (math_30_cp() * dt)), 2);
                                            fvTextBox11.Text = p30.ToString();
                                            fvTextBox11.Enabled = true;
                                        }
                                        else
                                        {
                                            LabelError.Text += "Не задана тепловая мощность";
                                            return;
                                        }
                                        if (!(customConverterToDouble(this.fvTextBox10.Text) > 0))
                                        {
                                            LabelError.Text += "Введите числовое значение больше нуля";
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        double checkVal;
                                        try
                                        {
                                            checkVal = customConverterToDouble(this.fvTextBox1.Text);
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text += "Неверно указан расход через клапан";
                                            return;
                                        }

                                        if (!String.IsNullOrWhiteSpace(fvTextBox1.Text))
                                        {
                                            p30 = (customConverterToDouble(fvTextBox1.Text) * arrConvert1[(fvDropDownList1.SelectedIndex - 1), 5]);
                                        }
                                        else
                                        {
                                            LabelError.Text += "Не задан расход через клапан";
                                            return;
                                        }
                                        if (!(customConverterToDouble(this.fvTextBox1.Text) > 0))
                                        {
                                            LabelError.Text += "Введите числовое значение больше нуля";
                                            return;
                                        }
                                    }

                                    g_dict.Add("p30", p30);


                                    if (!String.IsNullOrWhiteSpace(lpvTextBox1.Text))
                                    {
                                        if (!String.IsNullOrWhiteSpace((lpvDropDownList2.Enabled) ? lpvTextBox2.Text : lpvTextBox21.Text))
                                        {
                                            if (!String.IsNullOrWhiteSpace(calcvTextBox1.Text))
                                            {
                                                double p35 = 0;
                                                try
                                                {
                                                    p35 = customConverterToDouble(calcvTextBox2.Text);
                                                }
                                                catch (Exception) { }

                                                if (p35 <= 0)
                                                {

                                                    LabelError.Text += "Неверно указано значение температуры";
                                                    return;
                                                }
                                                else if (p35 > g_dict["vTMax"])
                                                {

                                                    LabelError.Text += "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет";
                                                    return;
                                                }
                                                else g_dict.Add("p35", p35);

                                                double p61 = 0;
                                                try
                                                {
                                                    if ((aaRadioButton1.Checked && aa1RadioButtonList1.SelectedIndex == 0)
                                                        || (aaRadioButton2.Checked && aa2RadioButtonList1.SelectedIndex == 0)
                                                        || (aaRadioButton3.Checked && aa3RadioButtonList1.SelectedIndex == 0))
                                                        p61 = customConverterToDouble(lpvTextBox2.Text) * arrConvert3[lpvDropDownList2.SelectedIndex - 1];

                                                    else if ((aaRadioButton1.Checked && aa1RadioButtonList1.SelectedIndex == 1)
                                                        || (aaRadioButton2.Checked && aa2RadioButtonList1.SelectedIndex == 1)
                                                        || (aaRadioButton3.Checked && aa3RadioButtonList1.SelectedIndex == 1))
                                                        p61 = customConverterToDouble(lpvTextBox21.Text) * arrConvert3[lpvDropDownList21.SelectedIndex - 1];
                                                }
                                                catch (Exception) { }

                                                //if (p61 / arrConvert3[2] > 16)
                                                //{
                                                //    if ((aaRadioButton1.Checked && aa1RadioButtonList1.SelectedIndex == 0)
                                                //        || (aaRadioButton2.Checked && aa2RadioButtonList1.SelectedIndex == 0)
                                                //        || (aaRadioButton3.Checked && aa3RadioButtonList1.SelectedIndex == 0))
                                                //        LabelError.Text += "На давление свыше 16 бар вариантов нет";

                                                //    else if ((aaRadioButton1.Checked && aa1RadioButtonList1.SelectedIndex == 1)
                                                //        || (aaRadioButton2.Checked && aa2RadioButtonList1.SelectedIndex == 1)
                                                //        || (aaRadioButton3.Checked && aa3RadioButtonList1.SelectedIndex == 1))
                                                //        LabelError.Text += "На давление свыше 16 бар вариантов нет";

                                                //    return;
                                                //}
                                                //else g_dict.Add("p61", p61);
                                                g_dict.Add("p61", p61);

                                                double p62 = 0;

                                                try
                                                {
                                                    if (lpvTextBox1.Enabled)
                                                        p62 = customConverterToDouble(lpvTextBox1.Text) * arrConvert3[lpvDropDownList1.SelectedIndex - 1];
                                                }
                                                catch (Exception) { }

                                                //if (p62 / arrConvert3[2] > 16)
                                                //{
                                                //    LabelError.Text += "На давление свыше 16 бар вариантов нет";

                                                //    return;
                                                //}
                                                //else g_dict.Add("p62", p62);
                                                g_dict.Add("p62", p62);

                                                double p63 = 0;
                                                try
                                                {
                                                    if (calcvTextBox1.Enabled)
                                                        p63 = customConverterToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1];
                                                }
                                                catch (Exception) { }

                                                //if (p63 / arrConvert3[2] > 16)
                                                //{
                                                //    LabelError.Text += "На давление свыше 16 бар вариантов нет";

                                                //    return;
                                                //}
                                                //else g_dict.Add("p63", p63);
                                                g_dict.Add("p63", p63);

                                                if (!(p63 > p62))
                                                {
                                                    LabelError.Text += "Неверно указано значение давления";

                                                    return;
                                                }

                                                /*if (p35 < 7 || p35 > 150)
                                                {
                                                    LabelError.Text += "Не задана температура для расчета клапана на кавитацию";
                                                    return;
                                                }
                                                if (ws2RadioButtonList1.SelectedIndex == 0)
                                                {
                                                    ws2ResultLabel.Text = "Рабочая среда - вода";
                                                    maxt2ResultLabel.Text = "Максимальная температура - 150 °С";
                                                }
                                                else if (ws2RadioButtonList1.SelectedIndex == 1)
                                                {
                                                    ws2ResultLabel.Text = "Рабочая среда - этиленгликоль " + ws2TextBox1.Text + " %, " + ws2TextBox2.Text + " °С";
                                                    maxt2ResultLabel.Text = "Максимальная температура - 150 °С";
                                                }
                                                else if (ws2RadioButtonList1.SelectedIndex == 2)
                                                {
                                                    ws2ResultLabel.Text = "Рабочая среда - пропиленгликоль " + ws2TextBox1.Text + " %, " + ws2TextBox2.Text + " °С";
                                                    maxt2ResultLabel.Text = "Максимальная температура - 150 °С";
                                                }*/


                                                ws2ResultLabel.Visible = true;
                                                maxp2ResultLabel.Visible = true;
                                                maxt2ResultLabel.Visible = true;
                                                ws2ResultLabel.Text = "Рабочая среда - " + (ws2RadioButtonList1.SelectedIndex == 0 ? "вода" : ((ws2RadioButtonList1.SelectedIndex == 1 ? "этиленгликоль " : "пропиленгликоль ") + g_dict["p14"] + "%, " + g_dict["p15"] + " °С"));
                                                //maxt2ResultLabel.Text = "Максимальная температура - " + g_dict["vTMax"].ToString() + " °С";
                                                this.maxt2ResultLabel.Text = "Максимальная температура - " + ((double.Parse(g_dict["p35"].ToString()) > 150) ? "220" : "150") + " °С";
                                                maxp2ResultLabel.Text = "Максимальное рабочее давление - 16 бар";


                                                double t1_check = customConverterToDouble(calcvTextBox2.Text);
                                                Newtonsoft.Json.Linq.JObject max_check = dataFromFile.table9v[dataFromFile.table9v.Count - 1];
                                                foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9v)
                                                {
                                                    if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max_check.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1_check))
                                                    {
                                                        max_check = ob;
                                                    }
                                                }
                                                //double ps_check = Convert.ToDouble(max_check.GetValue("ps"));

                                                if (((customConverterToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - getPSbyT(t1_check)) <= 0)
                                                {
                                                    LabelError.Text += "Указанная температура выше температуры парообразования. При указанной температуре в трубопроводе движется пар";
                                                    return;
                                                }

                                                mapInputParametersV(ref v_input_dict);

                                                Dictionary<string, string[]> gtr = generatedTableV(g_dict);

                                                if (gtr is null) return;

                                                string[] titles = new string[] {
                                            "Марка регулирующего клапана",
                                            "Номинальный диаметр DN, мм",
                                            "Пропускная cпособность Kvs, м3/ч",
                                            "Фактические потери давления на полностью открытом клапане при заданном расходе ∆Рф,\n бар\n",

                                            "Внешний авторитет клапана",
                                            "Качество регулирования",

                                            "Скорость в выходном сечении клапана V, м/с",

                                            "Шум, некачественное регулирование",

                                            "Предельно допустимый перепад давлений ∆Pпред, бар",
                                            "Кавитация",
                                            //"Скорость перемещения штока сек/мм (мм/мин)",
                                            //"Максимально допустимый перепад давления на клапане, преодолеваемый приводом, бар, не более",
                                            "Обозначение привода"
                                            ,"PP54"
                                            ,"PP55"
                                            ,"PP56"
                                            ,"PP57"
                                            ,"PP58"
                                            ,"PP59"
                                            ,"PP60"
                                            ,"PP61"
                                            ,"PP62"
                                            ,"PP63"
                                            ,"PP65"
                                            ,"PP66"
                                            ,"PP67"
                                            ,"PP68"
                                        };
                                                //DataGridViewColumn column;
                                                ///*column = new DataGridViewCheckBoxColumn();
                                                //column.DataPropertyName = "Column0";
                                                //column.Name = "Column0";
                                                //column.HeaderText = "";
                                                //dataGridView2.Columns.Add(column);*/

                                                //for (int i = 0; i < titles.Count(); i++)
                                                //{
                                                //    column = new DataGridViewTextBoxColumn();
                                                //    column.DataPropertyName = "Column" + i.ToString();
                                                //    column.Name = "Column" + i.ToString();
                                                //    column.HeaderText = titles[i];
                                                //    if (i > 10) column.Visible = false;
                                                //    dataGridView2.Columns.Add(column);
                                                //}

                                                DataTable dt = new DataTable();
                                                DataRow dr;
                                                //for (int i = 0; i < titles.Count(); i++)       

                                                for (int i = 0; i < titles.Count(); i++)
                                                {
                                                    dt.Columns.Add(new DataColumn(titles[i]));
                                                }

                                                int maxCount = -1;
                                                for (int i = 0; i < gtr.Count(); i++)
                                                {
                                                    if (maxCount < gtr.ElementAt(i).Value.Count())
                                                    {
                                                        maxCount = gtr.ElementAt(i).Value.Count();
                                                    }
                                                }

                                                for (int i = 0; i < maxCount; i++)
                                                {
                                                    dr = dt.NewRow();

                                                    dt.Rows.Add(dr);
                                                    GridView2.DataSource = dt;
                                                    GridView2.DataBind();

                                                    for (int j = 0; j < gtr.Count(); j++)
                                                    {
                                                        int index = -1;
                                                        switch (gtr.ElementAt(j).Key)
                                                        {
                                                            case "A": index = 0; break;
                                                            case "B": index = 2; break;
                                                            case "C": index = 1; break;
                                                            case "D": index = 3; break;

                                                            case "I1": index = 4; break;
                                                            case "I2": index = 5; break;

                                                            case "I": index = 6; break;

                                                            case "I3": index = 7; break;

                                                            case "F": index = 8; break;
                                                            case "G": index = 9; break;
                                                            //case "K": index = 11; break;
                                                            //case "L": index = 12; break;
                                                            case "M": index = 10; break;

                                                            case "PP54": index = 11; break;
                                                            case "PP55": index = 12; break;
                                                            case "PP56": index = 13; break;
                                                            case "PP57": index = 14; break;
                                                            case "PP58": index = 15; break;
                                                            case "PP59": index = 16; break;
                                                            case "PP60": index = 17; break;
                                                            case "PP61": index = 18; break;
                                                            case "PP62": index = 19; break;
                                                            case "PP63": index = 20; break;
                                                            case "PP65": index = 21; break;
                                                            case "PP66": index = 22; break;
                                                            case "PP67": index = 23; break;
                                                            case "PP68": index = 24; break;
                                                        }
                                                        index++;
                                                        if (gtr.ElementAt(j).Value.Count() > i)
                                                        {
                                                            string tmp = gtr.ElementAt(j).Value[i];
                                                            if (String.IsNullOrWhiteSpace(tmp))
                                                            {
                                                                if (GridView2.Rows.Count > 1)
                                                                {
                                                                    dt.Rows[GridView2.Rows.Count - 1][index - 1] = dt.Rows[GridView2.Rows.Count - 2][index - 1];
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dt.Rows[GridView2.Rows.Count - 1][index - 1] = tmp;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (GridView2.Rows.Count > 1)
                                                            {
                                                                dt.Rows[GridView2.Rows.Count - 1][index - 1] = dt.Rows[GridView2.Rows.Count - 2][index - 1];
                                                            }
                                                        }
                                                    }

                                                        GridView2.DataSource = dt;
                                                        GridView2.DataBind();

                                                       
                                                        
                                                    }

                                            }
                                            else
                                            {
                                                LabelError.Text += "Неверно указано значение давления";
                                                return;
                                            }

                                        }
                                        else
                                        {
                                            LabelError.Text += "Неверно указано значение давления";
                                            return;
                                        }

                                    }
                                    else
                                    {
                                        LabelError.Text += "Неверно указано значение давления";
                                        return;
                                    }
                                }
                                else
                                {
                                    Label8.Text += "Не задан расход через клапан";
                                    return;
                                }
                            }
                            else
                            {
                                LabelError.Text += "Не выбрана рабочая среда";
                                return;
                            }

                        }
                        else
                        {
                            LabelError.Text += "Не выбран тип регулирующего клапана";
                            return;
                        }
                    }
                    else
                    {
                        if (aaRadioButton1.Checked)
                        {
                            LabelError.Text += "Не выбрана схема присоединения регулирующего клапана";
                        }
                        else if (aaRadioButton2.Checked)
                        {
                            LabelError.Text += "Не выбрана схема присоединения регулирующего клапана";
                        }
                        else if (aaRadioButton3.Checked)
                        {
                            LabelError.Text += "Не выбрана схема присоединения регулирующего клапана";
                        }

                        return;
                    }
                }
                else
                {
                    LabelError.Text += "Не выбрана область применения (система) регулирующего клапана";
                    return;
                }
            }
            else
            {
                LabelError.Text += "Не указано наличие регулятора перепада давления";
                return;
            }

        }
        else
        {
            LabelError.Text += "Не выбрано место установки регулирующего клапана";
            return;
        }

        Label52.Visible = true;
        LabelError.Text = "";
        GridView2.Enabled = true;
        this.GridView2.Visible = true;
        this.GridView2.Height = 250;
        this.Button2.Visible = true;
        this.Button2.Enabled = true;
        //this.Button3.Visible = true;
        //this.Button3.Enabled = true;

        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
            
        }

    }

    //-----------------------------Validators--------------------------------------------------


    protected void spvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (spvRadioButtonList1.SelectedIndex == -1)
        {
            spvCustomValidator1.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
        }
    }

    protected void rpvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (spvCustomValidator1.IsValid)
        {
            if (rpvRadioButtonList1.SelectedIndex == -1)
            {
                rpvCustomValidator1.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
            }
        }
        else
        {
            args.IsValid = false;
            rpvCustomValidator1.ErrorMessage = "";
        }
    }

    protected void aaCustomValidator8_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (rpvCustomValidator1.IsValid)
        {
            if (aaRadioButton1.Checked || aaRadioButton2.Checked || aaRadioButton3.Checked)
            {
                if (aa1RadioButtonList1.SelectedIndex > -1 || aa2RadioButtonList1.SelectedIndex > -1 || aa3RadioButtonList1.SelectedIndex > -1)
                {
                    args.IsValid = true;
                    return;
                }
                else
                {
                    aaCustomValidator8.ErrorMessage = "Выберите необходимое значение";
                    args.IsValid = false;
                    return;
                }
            }
            else
            {
                aaCustomValidator8.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
                return;
            }
        }
        else
        {
            aaCustomValidator8.ErrorMessage = "";
            args.IsValid = false;
            return;
        }
    }

    protected void tv1CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (aaCustomValidator8.IsValid)
        {
            if (tvRadioButtonList1.SelectedIndex == -1)
            {
                tv1CustomValidator1.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
                return;
            }
        }
        else
        {
            args.IsValid = false;
            tv1CustomValidator1.ErrorMessage = "";
            return;
        }
    }

    protected void ws2CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (tv1CustomValidator1.IsValid)
        {
            if (ws2RadioButtonList1.SelectedIndex == -1)
            {
                ws2CustomValidator1.ErrorMessage = "Выберите необходимое значение";
                args.IsValid = false;
                return;
            }
        }
        else
        {
            args.IsValid = false;
            ws2CustomValidator1.ErrorMessage = "";
        }
    }

    protected void CustomValidator16_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (ws2CustomValidator1.IsValid)
        {
            if (ws2RadioButtonList1.SelectedIndex == 1 || ws2RadioButtonList1.SelectedIndex == 2)
            {
                if (ws2TextBox1.Enabled == false || checkTextBoxEmpty(ws2TextBox1))
                {
                    CustomValidator16.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(ws2TextBox1.Text) < 5 || customConverterToDouble(ws2TextBox1.Text) > 65)
                {
                    CustomValidator16.ErrorMessage = "Значение должно находится в диапазоне от 5% до 65%";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator16.ErrorMessage = "";
        }
    }

    protected void CustomValidator17_ServerValidate(object source, ServerValidateEventArgs args)
    {
        
        if (CustomValidator16.IsValid)
        {
            if (ws2RadioButtonList1.SelectedIndex == 1 || ws2RadioButtonList1.SelectedIndex == 2)
            {
                if (ws2TextBox2.Enabled == false || checkTextBoxEmpty(ws2TextBox2))
                {
                    CustomValidator17.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(ws2TextBox2.Text) < 0 || customConverterToDouble(ws2TextBox2.Text) > MaxT3x)
                {
                    CustomValidator17.ErrorMessage = "Значение должно находится в диапазоне от 0&#8451 до 150&#8451";
                    args.IsValid = false;
                    return;
                }
            }
            
        }
        else
        {
            args.IsValid = false;
            CustomValidator17.ErrorMessage = "";
        }
    }

    protected void CustomValidator18_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator17.IsValid && ws2CustomValidator1.IsValid)
        {
            if (lpvDropDownList2.Enabled)
            {
                if (lpvTextBox2.Enabled == false || checkTextBoxEmpty(lpvTextBox2))
                {
                    CustomValidator18.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lpvTextBox2.Text) <= 0)
                {
                    CustomValidator18.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator18.ErrorMessage = "";
        }
        return;
    }

    protected void CustomValidator19_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator17.IsValid)
        {
            if (lpvDropDownList21.Enabled)
            {
                if (lpvTextBox21.Enabled == false || checkTextBoxEmpty(lpvTextBox21))
                {
                    CustomValidator19.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lpvTextBox21.Text) <= 0)
                {
                    CustomValidator19.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator19.ErrorMessage = "";
        }
        return;
    }

    protected void lpvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if(CustomValidator18.IsValid && CustomValidator19.IsValid)
        {
            if (lpvDropDownList1.Enabled)
            {
                if (lpvTextBox1.Enabled == false || checkTextBoxEmpty(lpvTextBox1))
                {
                    lpvCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lpvTextBox1.Text) <= 0)
                {
                    lpvCustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            lpvCustomValidator1.ErrorMessage = "";
            args.IsValid = false;
            return;
        }

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

                if (convertArrToBar(arrConvert3, calcvDropDownList1, calcvTextBox1) > PressureBeforeValve3x)
                {
                    calcvCustomValidator1.ErrorMessage = "На давление свыше 16 бар вариантов нет";
                    args.IsValid = false;
                    return;
                }

                if (customConverterToDouble(lpvTextBox1.Text) * arrConvert3[lpvDropDownList1.SelectedIndex - 1] >= customConverterToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1])
                {
                    Label55.Visible = true;
                    args.IsValid = false;
                    calcvCustomValidator1.ErrorMessage = "";
                    Label56.Visible = false;
                    return;
                }
                else
                {
                    Label55.Visible = false;
                    calcvCustomValidator1.Visible = true;
                }
                
                if(lpvTextBox21.Text != "")
                {
                    if ((customConverterToDouble(lpvTextBox21.Text) * arrConvert3[lpvDropDownList21.SelectedIndex - 1] + customConverterToDouble(lpvTextBox1.Text) * arrConvert3[lpvDropDownList1.SelectedIndex - 1]) >= customConverterToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1])
                    {
                        Label56.Visible = true;
                        args.IsValid = false;
                        calcvCustomValidator1.ErrorMessage = "";
                        return;
                    }
                    else
                    {
                        Label56.Visible = false;
                        calcvCustomValidator1.Visible = true;
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
            if (tvRadioButtonList1.SelectedIndex == 0)
            {
                if (customConverterToDouble(calcvTextBox2.Text) > MaxT2x)
                {
                    calcvCustomValidator2.ErrorMessage = "На температуру свыше 220&#8451; вариантов нет";
                    args.IsValid = false;
                    return;
                }
            }
            else
            {
                if (customConverterToDouble(calcvTextBox2.Text) > MaxT3x)
                {
                    calcvCustomValidator2.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                    args.IsValid = false;
                    return;
                }
            }

            if (((customConverterToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - getPSbyT(customConverterToDouble(calcvTextBox2.Text))) <= 0)
            {
                calcvCustomValidator2.ErrorMessage = "Указанная температура выше температуры парообразования. При указанной температуре в трубопроводе движется пар";
                args.IsValid = false;
                return;
            }
        }
        else
        {
            calcvCustomValidator2.ErrorMessage = "";
            args.IsValid = false;
            return;
        }

    }

    protected void CustomValidator12_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (calcvCustomValidator2.IsValid)
        {
            if (fvDropDownList1.Enabled)
            {
                if (fvTextBox1.Enabled == false || checkTextBoxEmpty(fvTextBox1))
                {
                    CustomValidator12.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(fvTextBox1.Text) <= 0)
                {
                    CustomValidator12.ErrorMessage = "Неверно указано значение расхода";
                    args.IsValid = false;

                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator12.ErrorMessage = "";
        }
    }

    protected void CustomValidator20_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (tvCustomValidator1.IsValid)
        {
            if (fvDropDownList2.Enabled)
            {
                if (fvTextBox10.Enabled == false || checkTextBoxEmpty(fvTextBox10))
                {
                    CustomValidator20.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(fvTextBox10.Text) <= 0)
                {
                    CustomValidator20.ErrorMessage = "Неверно указано значение тепловой мощности";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator20.ErrorMessage = "";
        }
    }

    protected void tvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (calcvCustomValidator2.IsValid)
        {
            if (fvRadioButton2.Checked)
            {
                if (!CheckValidTextBox(fvTextBox2, tvCustomValidator1, args)) { args.IsValid = false; return; };
                if (!CheckValidTextBox(fvTextBox3, tvCustomValidator1, args)) { args.IsValid = false; return; };
                if (!CheckValidTextBox(fvTextBox4, tvCustomValidator1, args)) { args.IsValid = false; return; };
                if (!CheckValidTextBox(fvTextBox5, tvCustomValidator1, args)) { args.IsValid = false; return; };
                if (!CheckValidTextBox(fvTextBox6, tvCustomValidator1, args)) { args.IsValid = false; return; };
                if (!CheckValidTextBox(fvTextBox7, tvCustomValidator1, args)) { args.IsValid = false; return; };
                if (!CheckValidTextBox(fvTextBox8, tvCustomValidator1, args)) { args.IsValid = false; return; };
                if (!CheckValidTextBox(fvTextBox9, tvCustomValidator1, args)) { args.IsValid = false; return; };

                if (!CompareValidTextBox(fvTextBox2, fvTextBox3, args)) { args.IsValid = false; return; };
                if (!CompareValidTextBox(fvTextBox4, fvTextBox5, args)) { args.IsValid = false; return; };
                if (!CompareValidTextBox(fvTextBox6, fvTextBox7, args)) { args.IsValid = false; return; };
                if (!CompareValidTextBox(fvTextBox8, fvTextBox9, args)) { args.IsValid = false; return; };
            }
        }
        else
        {
            args.IsValid = false;
            tvCustomValidator1.ErrorMessage = "";
        }
        
    }
    
    //----------------------------Support Functions---------------------------------

    public void ValidateTemperature(RadioButtonList radioButtonList, TextBox textBox, ServerValidateEventArgs args, CustomValidator customValidator)
    {
        if (radioButtonList.SelectedIndex == 0)
        {
            if (customConverterToDouble(textBox.Text) > MaxT2x)
            {
                customValidator.ErrorMessage = "На температуру свыше 220&#8451; вариантов нет";
                args.IsValid = false;

            }
        }
        else
        {
            if (customConverterToDouble(textBox.Text) > MaxT3x)
            {
                customValidator.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                args.IsValid = false;
            }
        }
    }

    public bool CheckValidTextBox(TextBox textBox, CustomValidator customValidator, ServerValidateEventArgs args)
    {
        if (textBox.Enabled)
        {
            if (!String.IsNullOrWhiteSpace(textBox.Text))
            {
                if (customConverterToDouble(textBox.Text) <= 0)
                {
                    tvCustomValidator1.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return false;
                }

                if (tvRadioButtonList1.SelectedIndex == 0)
                {
                    if (customConverterToDouble(textBox.Text) > MaxT2x)
                    {
                        tvCustomValidator1.ErrorMessage = "На температуру свыше 220&#8451; вариантов нет";
                        args.IsValid = false;
                        return false;
                    }
                }
                else
                {
                    if (customConverterToDouble(textBox.Text) > MaxT3x)
                    {
                        tvCustomValidator1.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                        args.IsValid = false;
                        return false;
                    }
                }
            }
            else
            {
                tvCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                args.IsValid = false;
                return false;
            }

        }

        return true;
    }

    public bool CompareValidTextBox(TextBox textBox1, TextBox textBox2, ServerValidateEventArgs args)
    {
        if (textBox1.Enabled && textBox2.Enabled)
        {
            if (customConverterToDouble(textBox2.Text) > customConverterToDouble(textBox1.Text) || customConverterToDouble(textBox2.Text) == customConverterToDouble(textBox1.Text)) 
            {
                tvCustomValidator1.ErrorMessage = "Неверно указано значение температуры";
                args.IsValid = false;
                return false;
            }
        }

        return true;
    }

    protected void fvTextBox10_TextChanged(object sender, EventArgs e)
    {

    }

    protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
    {
        Label53.Visible = true;
        objTextBox1.Enabled = true;
        objTextBox1.Visible = true;
        string clientScript = "javascript:ShowBTN();";
        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyClientScript", clientScript);

    }

    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int index = GridView2.SelectedIndex;
        
        
    }




    protected void Button2_Click(object sender, EventArgs e)
    {

        this.readFile(0);
        if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
        {
            v_input_dict[2] = objTextBox1.Text;
        }
        else
        {
            v_input_dict[2] = "-";
        }
            
        int pos = 42;

        for (int i = 1; i < GridView2.SelectedRow.Cells.Count; i++)
        {
            
            if (pos == 52)
            {
                v_input_dict[pos] = GridView2.SelectedRow.Cells[i].Text;
                v_input_dict[pos + 1] = GridView2.SelectedRow.Cells[i].Text;
                pos++;
            }
            else if (pos >= 64) v_input_dict[pos + 1] = GridView2.SelectedRow.Cells[i].Text;
            else v_input_dict[pos] = GridView2.SelectedRow.Cells[i].Text;

            pos++;
                
        }

        v_input_dict[8] = v_input_dict[42];
        string fileName = ConvertCommaToPoint(v_input_dict[42]);


        if (fileName == "&nbsp;")
        {
            fileName = "Регуляторов не найдено";
        }

        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

        if (!File.Exists(HttpContext.Current.Server.MapPath(@"./Content/templates/templateTRV.xlsx")))
        {
            LabelError.Text += "Не найден файл шаблона";
            return;
        }

        ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath(@"./Content/templates/templateTRV.xlsx"));

        ExcelWorksheet ws = ef.Worksheets[0];

        ws.PrintOptions.TopMargin = 0.1 / 2.54;
        ws.PrintOptions.BottomMargin = 0.1 / 2.54;
        ws.PrintOptions.LeftMargin = 1 / 2.54;
        ws.PrintOptions.RightMargin = 0.78 / 2.54;

        for (int i = 1; i < 50; i++)
        {

            if (i == 10 || i == 12 || i == 14 || i == 16 || i == 18 || i == 20 || i == 22 || i == 23 || i == 24 || i == 25 || i == 26 || i == 27 || i == 28 || i == 29 || i == 30 || i == 31 || i == 32 || i == 34 || i == 43 || i == 44 || i == 45 || i == 46 || i == 48 || i == 50 || i == 52 || i == 54 || i == 57 || i == 63 || i == 64 || i == 65 || i == 66 || i == 67 || i == 68)
            {
                v_input_dict[i] = ConvertPointToComma(v_input_dict[i]);
            }

            if (v_input_dict[i] == "&nbsp;")
            {
                v_input_dict[i] = "-";
            }
        }

        ws.Cells["K46"].Value = v_input_dict[0];

        ws.Cells["J2"].Value = v_input_dict[1];
        ws.Cells["B3"].Value = v_input_dict[2];

        ws.Cells["C4"].Value = v_input_dict[3];
        ws.Cells["J4"].Value = v_input_dict[4];
        ws.Cells["C5"].Value = v_input_dict[5];
        ws.Cells["J5"].Value = v_input_dict[6].Replace("(через теплообменник)", "");
        ws.Cells["C6"].Value = v_input_dict[7];
        ws.Cells["J6"].Value = v_input_dict[8];

        ws.Cells["C8"].Value = v_input_dict[9];

        ws.Cells["E9"].Value = v_input_dict[10];
        ws.Cells["F9"].Value = v_input_dict[11];
        ws.Cells["J9"].Value = v_input_dict[12];
        ws.Cells["K9"].Value = v_input_dict[13];

        ws.Cells["E10"].Value = v_input_dict[14];
        ws.Cells["F10"].Value = v_input_dict[15];
        ws.Cells["E11"].Value = v_input_dict[16];
        ws.Cells["F11"].Value = v_input_dict[17];
        ws.Cells["E12"].Value = v_input_dict[18];

        // пар
        ws.Cells["J10"].Value = "-";
        ws.Cells["K10"].Value = "-";
        ws.Cells["J11"].Value = "-";
        ws.Cells["K11"].Value = "-";
        ws.Cells["J12"].Value = "-";
        // пар

        ws.Cells["E14"].Value = v_input_dict[24];
        ws.Cells["I14"].Value = v_input_dict[25];
        ws.Cells["E15"].Value = v_input_dict[26];
        ws.Cells["I15"].Value = v_input_dict[27];
        ws.Cells["E16"].Value = v_input_dict[28];
        ws.Cells["I16"].Value = v_input_dict[29];
        ws.Cells["E17"].Value = v_input_dict[30];
        ws.Cells["I17"].Value = v_input_dict[31];

        ws.Cells["I18"].Value = v_input_dict[32];
        ws.Cells["K18"].Value = v_input_dict[33];
        ws.Cells["I19"].Value = v_input_dict[34];
        ws.Cells["K19"].Value = v_input_dict[35];

        ws.Cells["C21"].Value = v_input_dict[36];
        ws.Cells["C22"].Value = v_input_dict[37];

        ws.Cells["J21"].Value = v_input_dict[38];
        ws.Cells["J22"].Value = v_input_dict[39];

        ws.Cells["E24"].Value = v_input_dict[40];
        ws.Cells["E25"].Value = v_input_dict[41];

        ws.Cells["A28"].Value = v_input_dict[42];
        ws.Cells["B28"].Value = v_input_dict[43];
        ws.Cells["C28"].Value = v_input_dict[44];
        ws.Cells["D28"].Value = v_input_dict[45];
        ws.Cells["E28"].Value = v_input_dict[46];
        ws.Cells["F28"].Value = v_input_dict[47];
        ws.Cells["G28"].Value = v_input_dict[48];
        ws.Cells["H28"].Value = v_input_dict[49];
        ws.Cells["I28"].Value = v_input_dict[50];
        ws.Cells["J28"].Value = v_input_dict[51];
        ws.Cells["K28"].Value = v_input_dict[52];

        ws.Cells["A32"].Value = v_input_dict[53];
        ws.Cells["B32"].Value = v_input_dict[54];
        ws.Cells["C32"].Value = v_input_dict[55];
        ws.Cells["D32"].Value = v_input_dict[56];
        ws.Cells["E32"].Value = v_input_dict[57];
        ws.Cells["F32"].Value = v_input_dict[58];
        ws.Cells["G32"].Value = v_input_dict[59];
        ws.Cells["H32"].Value = v_input_dict[60];
        ws.Cells["I32"].Value = v_input_dict[61];
        ws.Cells["J32"].Value = v_input_dict[62];
        ws.Cells["K32"].Value = v_input_dict[63];

        ws.Cells["G37"].Value = v_input_dict[65];
        ws.Cells["G38"].Value = v_input_dict[66];
        ws.Cells["G39"].Value = v_input_dict[67];
        ws.Cells["G40"].Value = v_input_dict[68];


        ws.Pictures.Add(HttpContext.Current.Server.MapPath("\\Content\\images\\trv\\" + ((v_input_dict[7] == this.tvRadioButtonList1.Items[tvRadioButtonList1.SelectedIndex].Text) ? "Габаритный TRV и TRV-P.png" : "Габаритный TRV-3.png")), "A37", "B46");
       

        string path = HttpContext.Current.Server.MapPath("\\Files\\TRV\\PDF\\" + DateTime.Now.ToString("dd-MM-yyyy"));
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        if (!dirInfo.Exists)
        {
            dirInfo.Create();
            
        }


        string filePath = path + "\\" + fileName + ".pdf";

        ef.Save(filePath);

        WaitDownload(50);
        LabelError.Text += "dfs";
        FileInfo file = new FileInfo(filePath);
        if (file.Exists)
        {
            LabelError.Text += "ok";
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename="+ file.Name);
            Response.TransmitFile(file.FullName);
           
        }
        LabelError.Text += "End";
    }

    

    static void WaitDownload(int second)
    {
        Stopwatch sw = new Stopwatch(); // sw cotructor
        sw.Start(); // starts the stopwatch
        for (int i = 0; ; i++)
        {
            if (i % 100000 == 0) // if in 100000th iteration (could be any other large number
                                 // depending on how often you want the time to be checked) 
            {
                sw.Stop(); // stop the time measurement
                if (sw.ElapsedMilliseconds > second) // check if desired period of time has elapsed
                {
                    break; // if more than 5000 milliseconds have passed, stop looping and return
                           // to the existing code
                }
                else
                {
                    sw.Start(); // if less than 5000 milliseconds have elapsed, continue looping
                                // and resume time measurement
                }
            }
        }
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        v_input_dict[2] = objTextBox1.Text;
        v_input_dict[8] = "-";

        int pos = 42;



        for (int i = 1; i < GridView2.SelectedRow.Cells.Count; i++)
        {

            if (pos == 52)
            {
                v_input_dict[pos] = GridView2.SelectedRow.Cells[i].Text;
                v_input_dict[pos + 1] = GridView2.SelectedRow.Cells[i].Text;
                pos++;
            }
            else if (pos >= 64) v_input_dict[pos + 1] = GridView2.SelectedRow.Cells[i].Text;
            else v_input_dict[pos] = GridView2.SelectedRow.Cells[i].Text;


            pos++;

        }

        v_input_dict[8] = v_input_dict[42];

        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

        if (!File.Exists(HttpContext.Current.Server.MapPath(@"./Content/templates/templateTRV.xlsx")))
        {
            LabelError.Text += "Не найден файл шаблона";
            return;
        }

        ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath(@"./Content/templates/templateTRV.xlsx"));

        ExcelWorksheet ws = ef.Worksheets[0];

        ws.PrintOptions.TopMargin = 0.1 / 2.54;
        ws.PrintOptions.BottomMargin = 0.1 / 2.54;
        ws.PrintOptions.LeftMargin = 1 / 2.54;
        ws.PrintOptions.RightMargin = 0.78 / 2.54;

        ws.Cells["K46"].Value = v_input_dict[0];

        ws.Cells["J2"].Value = v_input_dict[1];
        ws.Cells["B3"].Value = v_input_dict[2];

        ws.Cells["C4"].Value = v_input_dict[3];
        ws.Cells["J4"].Value = v_input_dict[4];
        ws.Cells["C5"].Value = v_input_dict[5];
        ws.Cells["J5"].Value = v_input_dict[6];
        ws.Cells["C6"].Value = v_input_dict[7];
        ws.Cells["J6"].Value = v_input_dict[8];

        ws.Cells["C8"].Value = v_input_dict[9];

        ws.Cells["E9"].Value = v_input_dict[10];
        ws.Cells["F9"].Value = v_input_dict[11];
        ws.Cells["J9"].Value = v_input_dict[12];
        ws.Cells["K9"].Value = v_input_dict[13];

        ws.Cells["E10"].Value = v_input_dict[14];
        ws.Cells["F10"].Value = v_input_dict[15];
        ws.Cells["E11"].Value = v_input_dict[16];
        ws.Cells["F11"].Value = v_input_dict[17];
        ws.Cells["E12"].Value = v_input_dict[18];

        // пар
        ws.Cells["J10"].Value = "-";
        ws.Cells["K10"].Value = "-";
        ws.Cells["J11"].Value = "-";
        ws.Cells["K11"].Value = "-";
        ws.Cells["J12"].Value = "-";
        // пар

        ws.Cells["E14"].Value = v_input_dict[24];
        ws.Cells["I14"].Value = v_input_dict[25];
        ws.Cells["E15"].Value = v_input_dict[26];
        ws.Cells["I15"].Value = v_input_dict[27];
        ws.Cells["E16"].Value = v_input_dict[28];
        ws.Cells["I16"].Value = v_input_dict[29];
        ws.Cells["E17"].Value = v_input_dict[30];
        ws.Cells["I17"].Value = v_input_dict[31];

        ws.Cells["I18"].Value = v_input_dict[32];
        ws.Cells["K18"].Value = v_input_dict[33];
        ws.Cells["I19"].Value = v_input_dict[34];
        ws.Cells["K19"].Value = v_input_dict[35];

        ws.Cells["C21"].Value = v_input_dict[36];
        ws.Cells["C22"].Value = v_input_dict[37];

        ws.Cells["J21"].Value = v_input_dict[38];
        ws.Cells["J22"].Value = v_input_dict[39];

        ws.Cells["E24"].Value = v_input_dict[40];
        ws.Cells["E25"].Value = v_input_dict[41];

        ws.Cells["A28"].Value = v_input_dict[42];
        ws.Cells["B28"].Value = v_input_dict[43];
        ws.Cells["C28"].Value = v_input_dict[44];
        ws.Cells["D28"].Value = v_input_dict[45];
        ws.Cells["E28"].Value = v_input_dict[46];
        ws.Cells["F28"].Value = v_input_dict[47];
        ws.Cells["G28"].Value = v_input_dict[48];
        ws.Cells["H28"].Value = v_input_dict[49];
        ws.Cells["I28"].Value = v_input_dict[50];
        ws.Cells["J28"].Value = v_input_dict[51];
        ws.Cells["K28"].Value = v_input_dict[52];

        ws.Cells["A32"].Value = v_input_dict[53];
        ws.Cells["B32"].Value = v_input_dict[54];
        ws.Cells["C32"].Value = v_input_dict[55];
        ws.Cells["D32"].Value = v_input_dict[56];
        ws.Cells["E32"].Value = v_input_dict[57];
        ws.Cells["F32"].Value = v_input_dict[58];
        ws.Cells["G32"].Value = v_input_dict[59];
        ws.Cells["H32"].Value = v_input_dict[60];
        ws.Cells["I32"].Value = v_input_dict[61];
        ws.Cells["J32"].Value = v_input_dict[62];
        ws.Cells["K32"].Value = v_input_dict[63];

        ws.Cells["G37"].Value = v_input_dict[65];
        ws.Cells["G38"].Value = v_input_dict[66];
        ws.Cells["G39"].Value = v_input_dict[67];
        ws.Cells["G40"].Value = v_input_dict[68];

        ws.Pictures.Add(HttpContext.Current.Server.MapPath("\\Content\\images\\trv\\" + ((v_input_dict[7] == this.tvRadioButtonList1.Items[tvRadioButtonList1.SelectedIndex].Text) ? "TRV-2.png" : "TRV-3.png")), "A37", "B46");



        string path = HttpContext.Current.Server.MapPath("\\Files\\TRV\\Excel\\" + DateTime.Now.ToString("dd-MM-yyyy"));
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        if (!dirInfo.Exists)
        {
            dirInfo.Create();
        }

        string fileName = "";

        if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
        {
            fileName = objTextBox1.Text;
        }
        else
        {
            fileName += DateTime.Now.ToString("dd-MM-yyyy");
        }

        string filePath = path + "\\" + fileName + ".xlsx";

        ef.Save(filePath);
        WaitDownload(50);




        FileInfo file = new FileInfo(filePath);
        if (file.Exists)
        {
            Response.ContentType = "application/x-msexcel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
            Response.TransmitFile(file.FullName);
         
        }


    }

    public void RemoveCssClass(HtmlGenericControl controlInstance, string css)
    {
        var strCssClass = controlInstance.Attributes["class"];
        controlInstance.Attributes["class"] = string.Join(" ", strCssClass.Split(' ').Where(x => x != css).ToArray());
    }

    public void AddCssClass(HtmlGenericControl controlInstance, string css)
    {
        var strCssClass = controlInstance.Attributes["class"];
        controlInstance.Attributes["class"] += (" " + css);
    }

    protected void spvRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        tdRadioButtonList1.Enabled = true;
        tdRadioButtonList2.Enabled = true;
        tdRadioButtonList3.Enabled = true;
        tdRadioButtonList4.Enabled = true;

        RemoveCssClass(tdRBL, "panel-hide");
    }
    private bool checkTextBoxEmpty(TextBox tb)
    {
        return tb.Text == "";
    }


    public string ConvertPointToComma(string tb)
    {
        string afterConvert = "";

        if (tb.IndexOf(".") != -1)
        {
            afterConvert = tb.Replace(".", ",");
        }
        else
        {
            afterConvert = tb;
        }

        return afterConvert;
    }

    public string ConvertCommaToPoint(string tb)
    {
        string afterConvert = "";

        if (tb.IndexOf(",") != -1)
        {
            afterConvert = tb.Replace(",", ".");
        }
        else
        {
            afterConvert = tb;
        }

        return afterConvert;
    }







}