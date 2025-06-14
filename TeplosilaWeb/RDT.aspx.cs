﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GemBox.Spreadsheet;
using Newtonsoft.Json;
using TeplosilaWeb.App_Code;

public partial class RDT : System.Web.UI.Page
{
    private const int PressureBeforeValve2x = 25;
    private const int PressureBeforeValve3x = 16;
    private const int MaxT2x = 220;
    private const int MaxT3x = 150;
    public dynamic dataFromFile;
    private double[,] convertTable;

    double[,] arrConvert1;
    double[] arrConvert2;
    double[] arrConvert3;
    double minVar = 0.000000001;

    public Dictionary<int, string> r_input_dict = new Dictionary<int, string>();

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
            LabelError.Text = "";
            fprLabelError.Text = "";
            LabelCustomValid.Visible = false;

        

            string ctrlname = Page.Request.Params["__EVENTTARGET"];
            if (ctrlname != "GridView1")
            {
                resultPanel.Visible = false;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "MyClientScript", "javascript:HideBTN()", true);
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    //------------------------------------Math Function START--------------------------------------
    /// <summary>
    /// Рассчитывает физические свойства этиленгликоля.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="d"></param>
    /// <param name="rr"></param>
    /// <param name="nn"></param>
    /// <param name="ll"></param>
    /// <param name="pr"></param>
    /// <param name="cp"></param>
    public void Etgl(double t, double d, ref double rr, /*ref double nn, ref double ll, ref double pr,*/ ref double cp)
    {
        //ll = (0.565 - 0.00303 * d) + (8.9 - 0.21 * d) * t * 0.0001;
        //nn = 0.001 / 1.05 * (0.472 - 0.00238 * d + 0.00024 * d * d + (4.65 - 0.212 * d + 0.00595 * d * d) * Math.Exp(-t / 20.5));
        rr = (1001.9 + 1.73 * d) - (0.1784 + 0.0062 * d) * t - 0.00107 * t * t;
        cp = 1000 * ((4.289 - 0.0212 * d) + (-8.84 + 1.404 * d - 0.00987 * d * d) * t / 10000);
        //nn = nn / rr;
        //pr = nn * cp * rr / ll;
    }

    /// <summary>
    /// Рассчитывает физические свойства пропиленгликоля.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="d"></param>
    /// <param name="rr"></param>
    /// <param name="nn"></param>
    /// <param name="ll"></param>
    /// <param name="pr"></param>
    /// <param name="cp"></param>
    public void Prgl(double t, double d, ref double rr, /*ref double nn, ref double ll, ref double pr,*/ ref double cp)
    {
        //ll = (0.5613 - 0.00363 * d) + (20.1 - 0.372 * d) * t * 0.0001 - (7.39 + 0.071 * d - 0.00326 * d * d) * 0.000001 * t * t;
        //nn = 1.4 * 0.001 * 0.00025 * Math.Exp(233.16 / (d + 51.48)) * Math.Exp((640.24 + 19.235 * d - 0.11645 * d * d) / (t + 150));
        rr = (999.63 + 1.4 * d - 0.0073 * d * d) - (0.08446 + 0.01715 * d - 0.0001277 * d * d) * t - 0.0029 * t * t;
        cp = 1000 * ((4.222 - 0.00752 * d - 0.00011 * d * d) + (-9.712 + 0.5964 * d) * t / 10000 + (8.383 + 0.0877 * d - 0.001457 * d * d) * 0.000001 * t * t);
        //nn = nn / rr;
        //pr = nn * cp * rr / ll;
    }

    /// <summary>
    /// Рассчитывает физические свойства воды.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="rr"></param>
    /// <param name="nn"></param>
    /// <param name="ll"></param>
    /// <param name="pr"></param>
    /// <param name="cp"></param>
    public void Water(double t, ref double rr) //, ref double nn, ref double ll, ref double pr, ref double cp)
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

    public double SteamCP()  //теплоёмкость пара
    {

        double T1 = customConverterToDouble(fprTextBox2.Text);
        double T2 = customConverterToDouble(fprTextBox3.Text);
        double p1 = (customConverterToDouble(lp5TextBox1.Text) * arrConvert3[lp5DropDownList1.SelectedIndex - 1] / arrConvert3[2]);
        double r = (T1 + 273.15) / 647.14;
        double pp = p1 / 220.64;
        double h2 = 4.186 * T2;
        double h1;

        if (lp5RadioButtonList1.SelectedIndex == 0)
        {
            h1 = (10258.8 - 20231.3 / r + 24702.8 / Math.Pow(r, 2) - 16307.3 / Math.Pow(r, 3) + 5579.31 / Math.Pow(r, 4) - 777.285 / Math.Pow(r, 5)) + pp * (-355.878 / r + 817.288 / Math.Pow(r, 2) - 845.841 / Math.Pow(r, 3)) - Math.Pow(pp, 2) * (160.276 / Math.Pow(r, 3)) + Math.Pow(pp, 3) * (-95607.5 / r + 443740 / Math.Pow(r, 2) - 767668 / Math.Pow(r, 3) + 587261 / Math.Pow(r, 4) - 167657 / Math.Pow(r, 5)) + Math.Pow(pp, 4) * (22542.8 / Math.Pow(r, 2) - 84140.2 / Math.Pow(r, 3) + 104198 / Math.Pow(r, 4) - 42886.7 / Math.Pow(r, 5));

        }
        else
        {
            h1 = 2149.17 + 15049.8 * Math.Pow(r, 3) - 38597.1 * Math.Pow(r, 4) + 38206.2 * Math.Pow(r, 5) - 14351.7 * Math.Pow(r, 6);
        }

        return h1 - h2;
    }

    public double GetAvgT()
    {
        double avg_T = 0;

        if (this.fprRadioButton1.Checked)
        {
            if (this.ws1RadioButtonList1.SelectedIndex == 0)
            {
                avg_T = customConverterToDouble(this.calcrTextBox2.Text);
            }
            else
            {
                avg_T = customConverterToDouble(this.ws1TextBox2.Text);
            }
        }
        else
        {
            avg_T = 0.5 * (customConverterToDouble(this.fprTextBox2.Text) + customConverterToDouble(this.fprTextBox3.Text));
        }

        return avg_T;
    }

    public double math_16_cp()
    {
        double cp = 0;
        double rr = 0;
        if (ws1RadioButtonList1.SelectedIndex == 0)
        {
            Water(GetAvgT(), ref rr);
            cp = WaterCP(GetAvgT()); //4.187;
        }
        else if (ws1RadioButtonList1.SelectedIndex == 1)
        {
            Etgl(GetAvgT(), customConverterToDouble(this.ws1TextBox1.Text), ref rr, ref cp);

            /*double p6 = Convert.ToDouble(ws1TextBox1.Text);
            double p7 = Math.Round(Convert.ToDouble(ws1TextBox2.Text) / 10) * 10;
            foreach (Newtonsoft.Json.Linq.JProperty el in dataFromFile.table3)
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
                    cp = tmp_t;
                }
            }*/
        }
        else if (ws1RadioButtonList1.SelectedIndex == 2)
        {
            Prgl(GetAvgT(), customConverterToDouble(this.ws1TextBox1.Text), ref rr, ref cp);
        }
        return cp / 1000; // * rr / 1000000;
    }
    private double getPSbyT(double t)
    {
        return Math.Pow(t / 103, 1 / 0.242) - 0.892;
    }

    private double getTbyPS(double ps)
    {
        return 103 * Math.Pow(ps + 0.892, 0.242);
    }

    //------------------------------------Math Function END--------------------------------------

    //------------------------------------Table Function START--------------------------------------

    private bool get25BarFlag()
    {
        bool flag25Bar = false;

        if (ws1RadioButtonList1.SelectedIndex != 3)
        {
            flag25Bar = (convertArrToBar(arrConvert3, lp1DropDownList3, lp1TextBox3) > PressureBeforeValve3x) || //Регулятор перепада давления
                        (convertArrToBar(arrConvert3, lp1DropDownList4, lp1TextBox4) > PressureBeforeValve3x) ||
                        (convertArrToBar(arrConvert3, lp2DropDownList1, lp2TextBox1) > PressureBeforeValve3x) ||  //Регулятор давления "после себя"
                        (convertArrToBar(arrConvert3, lp2DropDownList2, lp2TextBox2) > PressureBeforeValve3x) ||
                        (convertArrToBar(arrConvert3, lp3DropDownList1, lp3TextBox1) > PressureBeforeValve3x) || //Регулятор давления "до себя"
                        (convertArrToBar(arrConvert3, lp3DropDownList2, lp3TextBox2) > PressureBeforeValve3x) ||
                        (convertArrToBar(arrConvert3, lp4DropDownList2, lp4TextBox2) > PressureBeforeValve3x) || //Регулятор "перепуска"
                        (convertArrToBar(arrConvert3, calcrDropDownList1, calcrTextBox1) > PressureBeforeValve3x); // Расчет регулятора давления на кавитацию:
        } else
        {
            if(customConverterToDouble(lp5TextBox3.Text) > 120)
            {
                if ((convertArrToBar(arrConvert3, lp5DropDownList1, lp5TextBox1) <= (16 - 0.04 * (customConverterToDouble(lp5TextBox3.Text) - 120))))
                {
                    flag25Bar = false;
                }
                else if ((convertArrToBar(arrConvert3, lp5DropDownList1, lp5TextBox1) <= (25 - 0.025 * (customConverterToDouble(lp5TextBox3.Text) - 120))))
                {
                    flag25Bar = true;
                }
                else
                {
                    flag25Bar = true;
                }
            } 
            else 
            {
                flag25Bar = (convertArrToBar(arrConvert3, lp5DropDownList1, lp5TextBox1) > PressureBeforeValve3x) || //Регулятор давления "после себя" Пар
                     (convertArrToBar(arrConvert3, lp5DropDownList2, lp5TextBox2) > PressureBeforeValve3x) ||
                     (convertArrToBar(arrConvert3, calcrDropDownList1, calcrTextBox1) > PressureBeforeValve3x); // Расчет регулятора давления на кавитацию:
            } 
            
        }
         
        return flag25Bar;
    }
    private Dictionary<string, string[]> generatedTableR(Dictionary<string, double> g_dict)
    {
        try
        {

            /*BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB*/
            double Kv = 0, Gpg = 0, dPg = 0, g = 0, p1 = 0, p2 = 0, V = 0, T1 = 0;
            int DN = 0;
            double Kv_start = 0;
            double tmpKv = 0;
            string tmpA = "";
            bool flag25Bar = get25BarFlag();

            Newtonsoft.Json.Linq.JArray table5 = null;
            Newtonsoft.Json.Linq.JArray table11 = null;

            Dictionary<string, string[]> listResult = new Dictionary<string, string[]>();
            listResult.Add("A", new string[] { });
            listResult.Add("B", new string[] { });
            listResult.Add("C", new string[] { });
            listResult.Add("I", new string[] { });
            listResult.Add("F", new string[] { });
            listResult.Add("E", new string[] { });

            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                listResult.Add("D", new string[] { });
                listResult.Add("G", new string[] { });
                listResult.Add("K", new string[] { });
            }

            Gpg = g_dict["p16"];

            if (eorRadioButtonList1.SelectedIndex == 0)
            {
                dPg = customConverterToDouble(this.lp1TextBox1.Text) * arrConvert3[this.lp1DropDownList1.SelectedIndex - 1];
            }
            else if (eorRadioButtonList1.SelectedIndex == 1)
            {
                if (ws1RadioButtonList1.SelectedIndex == 3)
                {
                    dPg = customConverterToDouble(this.lp5TextBox1.Text) * arrConvert3[this.lp5DropDownList1.SelectedIndex - 1] - customConverterToDouble(this.lp5TextBox2.Text) * arrConvert3[this.lp5DropDownList2.SelectedIndex - 1];
                }
                else
                {
                    dPg = customConverterToDouble(this.lp2TextBox1.Text) * arrConvert3[this.lp2DropDownList1.SelectedIndex - 1] - customConverterToDouble(this.lp2TextBox2.Text) * arrConvert3[this.lp2DropDownList2.SelectedIndex - 1];
                }
            }
            else if (eorRadioButtonList1.SelectedIndex == 2)
            {
                dPg = customConverterToDouble(this.lp3TextBox1.Text) * arrConvert3[this.lp3DropDownList1.SelectedIndex - 1] - customConverterToDouble(this.lp3TextBox2.Text) * arrConvert3[this.lp3DropDownList2.SelectedIndex - 1];
            }
            else if (eorRadioButtonList1.SelectedIndex == 3)
            {
                dPg = customConverterToDouble(this.lp4TextBox2.Text) * arrConvert3[this.lp4DropDownList2.SelectedIndex - 1];
            }

            if (ws1RadioButtonList1.SelectedIndex == 0)
            {
                Water(GetAvgT(), ref g);
            }
            else if (ws1RadioButtonList1.SelectedIndex == 1)
            {
                double p6 = customConverterToDouble(this.ws1TextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Etgl(p7, p6, ref g, ref cp);
            }
            else if (ws1RadioButtonList1.SelectedIndex == 2)
            {
                double p6 = customConverterToDouble(this.ws1TextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Prgl(p7, p6, ref g, ref cp);
            }
            else if (ws1RadioButtonList1.SelectedIndex == 2)
            {
                double p6 = customConverterToDouble(this.ws1TextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Prgl(p7, p6, ref g, ref cp);
            }

            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                Kv = 1.2 * (Gpg * 0.01) / (Math.Sqrt(dPg * 0.001 * g));
            }
            else //Расчетная максимальная пропускная способность пара
            {
                p1 = (customConverterToDouble(lp5TextBox1.Text) * arrConvert3[lp5DropDownList1.SelectedIndex - 1] / arrConvert3[2]);
                p2 = (customConverterToDouble(lp5TextBox2.Text) * arrConvert3[lp5DropDownList2.SelectedIndex - 1] / arrConvert3[2]);

                if (lp5RadioButtonList1.SelectedIndex == 0)
                {
                    T1 = customConverterToDouble(lp5TextBox3.Text);
                }
                else
                {
                    T1 = Math.Round(100 * Math.Pow((customConverterToDouble(lp5TextBox1.Text) * arrConvert3[lp5DropDownList1.SelectedIndex - 1] / arrConvert3[2]) + 1, 0.25));
                }

                if ((p1 - p2) <= (0.5 * (p1 + 1)))
                {
                    Kv = 1.3 * ((Gpg / 461) * Math.Sqrt((T1 + 273) / ((p1 - p2) * (p2 + 1))));
                }
                else
                {
                    Kv = 1.3 * (Gpg / (230 * (p1 + 1))) * Math.Sqrt(T1 + 273);
                }
            }

            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                Kv = 1.2 * (Gpg * 0.01) / (Math.Sqrt(dPg * 0.001 * g));
            }
            else //Расчетная максимальная пропускная способность пара
            {
                p1 = (customConverterToDouble(lp5TextBox1.Text) * arrConvert3[lp5DropDownList1.SelectedIndex - 1] / arrConvert3[2]);
                p2 = (customConverterToDouble(lp5TextBox2.Text) * arrConvert3[lp5DropDownList2.SelectedIndex - 1] / arrConvert3[2]);

                if(lp5RadioButtonList1.SelectedIndex == 0)
                {
                    T1 = customConverterToDouble(lp5TextBox3.Text);
                }
                else
                {
                    T1 = Math.Round(100 * Math.Pow((customConverterToDouble(lp5TextBox1.Text) * arrConvert3[lp5DropDownList1.SelectedIndex - 1] / arrConvert3[2]) + 1, 0.25));
                }
                
                if ((p1 - p2) <= (0.5 * (p1 + 1)))
                {
                    Kv = 1.3 * ((Gpg / 461) * Math.Sqrt((T1 + 273) / ((p1 - p2) * (p2 + 1))));
                }
                else
                {
                    Kv = 1.3 * (Gpg / (230 * (p1 + 1))) * Math.Sqrt(T1 + 273);
                }
            }

            this.calcCapacityLabelVal.Text = Math.Round(Kv, 2).ToString() + " м³/ч";
            this.calcCapacityLabelVal.Visible = true;
            this.calcCapacityLabel.Visible = true;

            /*IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII*/

            double I = 0;
            Newtonsoft.Json.Linq.JObject tmpI = null;

            if (flag25Bar)
            {
                if (eorRadioButtonList1.SelectedIndex == 0) //Регулятор перепада давления
                {
                    I = customConverterToDouble(this.lp1TextBox2.Text) * arrConvert3[this.lp1DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                    if (g_dict["p35"] > MaxT3x || customConverterToDouble(this.fprTextBox2.Text) > MaxT3x)
                    {
                        table5 = dataFromFile.table5sbt25;
                        table11 = dataFromFile.table11sbt25;
                    }
                    else
                    {
                        table5 = dataFromFile.table525;

                        if (ws1RadioButtonList1.SelectedIndex != 3)
                        {
                            table11 = dataFromFile.table1125;
                        }
                        else
                        {
                            table11 = dataFromFile.table11sbt25;
                        }
                    }
                }

                else if (eorRadioButtonList1.SelectedIndex == 1) //Регулятор давления после себя
                {
                    if (ws1RadioButtonList1.SelectedIndex != 3)
                    {
                        I = customConverterToDouble(this.lp2TextBox2.Text) * arrConvert3[this.lp2DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                    }
                    else
                    {
                        I = customConverterToDouble(this.lp5TextBox2.Text) * arrConvert3[this.lp5DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                    }

                    if (g_dict["p35"] > MaxT3x || customConverterToDouble(this.fprTextBox2.Text) > MaxT3x)
                    {
                        table5 = dataFromFile.table5sbt25;
                        table11 = dataFromFile.table11sbt25;

                    }
                    else
                    {
                        table5 = dataFromFile.table525;

                        if (ws1RadioButtonList1.SelectedIndex != 3)
                        {
                            table11 = dataFromFile.table1125;
                        }
                        else
                        {
                            table11 = dataFromFile.table11sbt25;
                        }
                    }

                }
                else if (eorRadioButtonList1.SelectedIndex == 2) //Регулятор давления до себя
                {
                    I = customConverterToDouble(this.lp3TextBox1.Text) * arrConvert3[this.lp3DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                    table5 = dataFromFile.table525;
                    table11 = dataFromFile.table1125;
                }
                else if (eorRadioButtonList1.SelectedIndex == 3) //Регулятор перепуска
                {
                    I = customConverterToDouble(this.lp4TextBox2.Text) * arrConvert3[this.lp4DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                    table5 = dataFromFile.table525;
                    table11 = dataFromFile.table1125;
                }
            }
            else
            {
                if (eorRadioButtonList1.SelectedIndex == 0) //Регулятор перепада давления
                {
                    I = customConverterToDouble(this.lp1TextBox2.Text) * arrConvert3[this.lp1DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                    if (g_dict["p35"] > MaxT3x || customConverterToDouble(this.fprTextBox2.Text) > MaxT3x)
                    {
                        table5 = dataFromFile.table5sbt;
                        table11 = dataFromFile.table11sbt;
                    }
                    else
                    {
                        table5 = dataFromFile.table5;

                        if (ws1RadioButtonList1.SelectedIndex != 3)
                        {
                            table11 = dataFromFile.table11;
                        }
                        else
                        {
                            table11 = dataFromFile.table11sbt;
                        }
                    }
                }

                else if (eorRadioButtonList1.SelectedIndex == 1) //Регулятор давления после себя
                {
                    if (ws1RadioButtonList1.SelectedIndex != 3)
                    {
                        I = customConverterToDouble(this.lp2TextBox2.Text) * arrConvert3[this.lp2DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                    }
                    else
                    {
                        I = customConverterToDouble(this.lp5TextBox2.Text) * arrConvert3[this.lp5DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                    }

                    if (g_dict["p35"] > MaxT3x || customConverterToDouble(this.fprTextBox2.Text) > MaxT3x)
                    {
                        table5 = dataFromFile.table5sbt;
                        table11 = dataFromFile.table11sbt;

                    }
                    else
                    {
                        table5 = dataFromFile.table5;

                        if (ws1RadioButtonList1.SelectedIndex != 3)
                        {
                            table11 = dataFromFile.table11;
                        }
                        else
                        {
                            table11 = dataFromFile.table11sbt;
                        }
                    }

                }
                else if (eorRadioButtonList1.SelectedIndex == 2) //Регулятор давления до себя
                {
                    I = customConverterToDouble(this.lp3TextBox1.Text) * arrConvert3[this.lp3DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                    table5 = dataFromFile.table5;
                    table11 = dataFromFile.table11;
                }
                else if (eorRadioButtonList1.SelectedIndex == 3) //Регулятор перепуска
                {
                    I = customConverterToDouble(this.lp4TextBox2.Text) * arrConvert3[this.lp4DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                    table5 = dataFromFile.table5;
                    table11 = dataFromFile.table11;
                }
            }

            double col_B = Convert.ToDouble(table5[table5.Count - 1]);
            int col_C = Convert.ToInt32(table11[table11.Count - 1]);

            if (I < (((eorRadioButtonList1.SelectedIndex == 2 || eorRadioButtonList1.SelectedIndex == 3)) ? 0.08 : 0.08) || I > 15.8)
            {
                var _List = new List<string>();
                if (listResult.ContainsKey("I"))
                {
                    _List.AddRange(listResult["I"]);
                }

                _List.AddRange(new string[] { "Решение по диапазону настройки не найдено" });
                listResult["I"] = _List.ToArray();

                return listResult;
            }
            else
            {
                //foreach (Newtonsoft.Json.Linq.JObject el in ((this.eorRadioButton3.Checked || this.eorRadioButton4.Checked) ? dataFromFile.table63 : dataFromFile.table61))
                foreach (Newtonsoft.Json.Linq.JObject el in dataFromFile.table61)
                {
                    double min = Convert.ToDouble(el.GetValue("min"));
                    double max = Convert.ToDouble(el.GetValue("max"));
                    if ((min <= I) && (max >= I))
                    {
                        if (tmpI != null)
                        {
                            double old_min = Convert.ToDouble(tmpI.GetValue("min"));
                            double old_max = Convert.ToDouble(tmpI.GetValue("max"));
                            double tmpSer = (min + (max - min) / 2);
                            double tmpSer1 = (old_min + (old_max - old_min) / 2);
                            double tmpSr = 0.0;
                            double tmpSr1 = 0.0;
                            if (I > tmpSer)
                            {
                                tmpSr = I - tmpSer;
                            }
                            else
                            {
                                tmpSr = tmpSer - I;
                            }

                            if (I > tmpSer1)
                            {
                                tmpSr1 = I - tmpSer1;
                            }
                            else
                            {
                                tmpSr1 = tmpSer1 - I;
                            }
                            if (tmpSr < tmpSr1)
                            {
                                tmpI = el;
                            }
                        }
                        else
                        {
                            tmpI = el;
                        }
                    }
                }

                var _List = new List<string>();
                if (listResult.ContainsKey("I"))
                {
                    _List.AddRange(listResult["I"]);
                }

                _List.AddRange(new string[] { "" + tmpI.GetValue("min") + "..." + tmpI.GetValue("max") });
                listResult["I"] = _List.ToArray();
            }
            /*/IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII*/

            bool exit_t = false;
           

            if (col_B == Convert.ToDouble(table5[table5.Count - 1]))
            {
                foreach (double el in table5)
                {
                    if ((el <= col_B) && (el >= Kv))
                    {
                        col_B = el;
                    }
                }
            }
            else
            {
                double col_Bt = Convert.ToDouble(table5[table5.Count - 1]);
                foreach (double el in table5)
                {
                    if ((el <= col_Bt) && (el >= Kv) && (el > col_B))
                    {
                        col_Bt = el;
                    }
                }
                col_B = col_Bt;
            }

            if (col_B == Convert.ToDouble(table5[table5.Count - 1]))
                exit_t = true;

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
            /*/BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB*/

            /*AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA*/
            /*CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/
            Newtonsoft.Json.Linq.JArray table = null;

            if (flag25Bar)
            {
                if (eorRadioButtonList1.SelectedIndex == 0)
                {
                    if (ws1RadioButtonList1.SelectedIndex != 3 && (g_dict["p35"] > MaxT3x || customConverterToDouble(fprTextBox2.Text) > MaxT3x))
                    {
                        table = dataFromFile.table75H25;
                    }
                    else
                    {
                        table = dataFromFile.table7125;
                    }

                }
                else if (eorRadioButtonList1.SelectedIndex == 1)
                {
                    if (ws1RadioButtonList1.SelectedIndex != 3)
                    {
                        if (g_dict["p35"] > MaxT3x || customConverterToDouble(fprTextBox2.Text) > MaxT3x)
                        {
                            table = dataFromFile.table7525;
                        }
                        else
                        {
                            table = dataFromFile.table7225;
                        }
                    }
                    else
                    {

                        table = dataFromFile.table7525;

                    }

                }
                else if (eorRadioButtonList1.SelectedIndex == 2)
                {
                    table = dataFromFile.table7325;
                }
                else if (eorRadioButtonList1.SelectedIndex == 3)
                {
                    table = dataFromFile.table7425;
                }
            } 
            else
            {
                if (eorRadioButtonList1.SelectedIndex == 0)
                {
                    if (ws1RadioButtonList1.SelectedIndex != 3 && (g_dict["p35"] > MaxT3x || customConverterToDouble(fprTextBox2.Text) > MaxT3x))
                    {
                        table = dataFromFile.table75H;
                    }
                    else
                    {
                        table = dataFromFile.table71;
                    }

                }
                else if (eorRadioButtonList1.SelectedIndex == 1)
                {
                    if (ws1RadioButtonList1.SelectedIndex != 3)
                    {
                        if (g_dict["p35"] > MaxT3x || customConverterToDouble(fprTextBox2.Text) > MaxT3x)
                        {
                            table = dataFromFile.table75;
                        }
                        else
                        {
                            table = dataFromFile.table72;
                        }
                    }
                    else
                    {

                        table = dataFromFile.table75;

                    }

                }
                else if (eorRadioButtonList1.SelectedIndex == 2)
                {
                    table = dataFromFile.table73;
                }
                else if (eorRadioButtonList1.SelectedIndex == 3)
                {
                    table = dataFromFile.table74;
                }
            }
            if (tmpI != null)
            {
                List<string> listA = new List<string>(),
                  listB = new List<string>(),
                  listC = new List<string>();

                foreach (Newtonsoft.Json.Linq.JObject ob in table)
                {
                    if ((Convert.ToDouble(ob.GetValue("prop")) == Kv) &&
                        (Convert.ToDouble(ob.GetValue("min")) == Convert.ToDouble(tmpI.GetValue("min")) &&
                            Convert.ToDouble(ob.GetValue("max")) == Convert.ToDouble(tmpI.GetValue("max"))))
                    {
                        listA.Add(ob.GetValue("name").ToString());
                        listB.Add(ob.GetValue("prop").ToString());
                        listC.Add(ob.GetValue("d").ToString());
                        DN = int.Parse(ob.GetValue("d").ToString());
                        tmpKv = Kv;
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

            }
            /*/CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/
            /*/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA*/

            double C;
            if (listResult["C"].Count() >0){
                C = Convert.ToDouble(listResult["C"][listResult["C"].Count() - 1]);
            }
            else {

                var _List = new List<string>();

                if (listResult.ContainsKey("B"))
                {
                    _List.AddRange(listResult["B"]);
                }
                _List.AddRange(new string[] { "Решение не найдено" });
                listResult["B"] = _List.ToArray();
                return listResult;
            }


            
            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                V = Gpg / g * Math.Pow((18.8 / C), 2);
            }
            else
            {
                V = (Gpg * (T1 + 273)) / Math.Pow((C / 18.8), 2) / (219 * (p2 + 1));
            }

            double cDN = 0;
            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                 cDN = 18.8 / Math.Sqrt(((g  * 3) / Gpg));   //ИТП
            }
            else
            {
                if(lp5RadioButtonList1.SelectedIndex == 0)
                {
                    cDN = 18.8 * Math.Sqrt((Gpg * (T1 + 273)) / (219 * (p2 + 1) * 60));
                }
                else
                {
                    cDN = 18.8 * Math.Sqrt((Gpg * (T1 + 273)) / (219 * (p2 + 1) * 40));
                }
            }

            calcDNLabelVal.Text = Math.Round(cDN, 2).ToString() + " мм";
            calcDNLabel.Visible = true;
            calcDNLabelVal.Visible = true;


            double Pf = 1;

            while (!exit_t && (V >= g_dict["vmax"]))
            {
                if (exit_t)
                    break;
                else
                {
                    // DN ближайший больший из table11
                   

                    if (col_C == Convert.ToDouble(table11[table11.Count - 1]))
                    {
                        foreach (int el in table11)
                        {
                            if ((el <= col_C) && (el > DN))
                            {
                                col_C = el;
                            }
                        }
                    }
                    else
                    {
                        int col_Ct = Convert.ToInt32(table11[table11.Count - 1]);
                        foreach (int el in table11)
                        {
                            if ((el <= col_Ct) && (el >= DN) && (el > col_C))
                            {
                                col_Ct = el;
                            }
                        }
                        col_C = col_Ct;
                    }

                    bool meetEnd = false;

                    if (col_C == Convert.ToDouble(table11[table11.Count - 1]))
                    {

                        exit_t = true;

                        foreach (string keyValue in listResult["C"])
                        {
                            if (keyValue.Equals(table11[table11.Count - 1].ToString()))
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

                    if (tmpI != null)
                    {
                        List<string> listA = new List<string>(),
                        listB = new List<string>();


                        if (ws1RadioButtonList1.SelectedIndex != 3)
                        {
                            Kv_start = 1.2 * (Gpg * 0.01) / (Math.Sqrt(dPg * 0.001 * g));
                        }
                        else //Расчетная максимальная пропускная способность пара
                        {
                            if ((p1 - p2) <= (0.5 * (p1 + 1)))
                            {
                                Kv_start = 1.3 * ((Gpg / 461) * Math.Sqrt((T1 + 273) / ((p1 - p2) * (p2 + 1))));
                            }
                            else
                            {
                                Kv_start = 1.3 * (Gpg / (230 * (p1 + 1))) * Math.Sqrt(T1 + 273);
                            }
                        }

                        tmpKv = 300;
                        tmpA = "";
                        foreach (Newtonsoft.Json.Linq.JObject ob in table)
                        {
                            if ((Convert.ToDouble(ob.GetValue("d")) == DN) &&
                                (Convert.ToDouble(ob.GetValue("min")) == Convert.ToDouble(tmpI.GetValue("min")) &&
                                    Convert.ToDouble(ob.GetValue("max")) == Convert.ToDouble(tmpI.GetValue("max"))))
                            {
                                if (Kv_start < Convert.ToDouble(ob.GetValue("prop")) && tmpKv >= Convert.ToDouble(ob.GetValue("prop")))
                                {
                                    tmpKv = Convert.ToDouble(ob.GetValue("prop"));
                                    tmpA = ob.GetValue("name").ToString();    
                                }
                                //listA.Add(ob.GetValue("name").ToString());
                                //listB.Add(ob.GetValue("prop").ToString());
                                //DN = int.Parse(ob.GetValue("d").ToString());

                            }
                        }

                        if(tmpA != "")
                        {
                            listA.Add(tmpA.ToString());
                            listB.Add(tmpKv.ToString());

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
                        }
                    }
                    /*/CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/
                    /*/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA*/

                }

                if (ws1RadioButtonList1.SelectedIndex != 3)
                {
                    V = Gpg / g * Math.Pow((18.8 / DN), 2);
                }
                else
                {
                    V = (Gpg * (T1 + 273)) / Math.Pow(( DN / 18.8), 2) / ((219 * (p2 + 1)));
                }
            }

            /*FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF*/
            /*GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG*/
            List<string> listD = new List<string>(),
                listF = new List<string>(),
                listG = new List<string>(),
                listE = new List<string>(),
                listK = new List<string>();

            
            listF.AddRange(listResult["F"]);
            listE.AddRange(listResult["E"]);

            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                listD.AddRange(listResult["D"]);
                listG.AddRange(listResult["G"]);
                listK.AddRange(listResult["K"]);
            }
            

            for (int i = 0; i < listResult["C"].Count(); i++)
            {
                if (ws1RadioButtonList1.SelectedIndex != 3)
                {
                    /*DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD*/
                    Pf = (Math.Pow(Gpg, 2) * 0.1) / (Math.Pow(double.Parse(listResult["B"].GetValue(i).ToString()), 2) * g);
                    Pf = Math.Round(Pf / 100, 2); /*Перевод с кПа в бар*/
                    //listResult["D"] = new string[] { Pf.ToString() };

                    listD.Add(Pf.ToString());
                }
                /*
                var d_List = new List<string>();
                if (listResult.ContainsKey("D"))
                {
                    d_List.AddRange(listResult["D"]);
                }

                d_List.AddRange(new string[] { Pf.ToString() });
                listResult["D"] = d_List.ToArray();*/
                /*/DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD*/

                C = Convert.ToDouble(listResult["C"][i]);

                if (ws1RadioButtonList1.SelectedIndex != 3)
                {
                    V = Gpg / g * Math.Pow((18.8 / C), 2);
                }
                else
                {
                    V = (Gpg * (T1 + 273)) / Math.Pow((C / 18.8), 2) / ((219 * (p2 + 1)));
                }

                if (V < g_dict["vmax"])
                    exit_t = true;

                listF.Add(Math.Round(V, 2).ToString());

                if (ws1RadioButtonList1.SelectedIndex != 3)
                {
                    if (V > 3 && V <= 5)
                        listE.Add("возможен шум");
                    else if (V > 5) listE.Add("возможен эрозийный износ клапана");
                    else if (V < 1.5)
                    {
                        listE.Add("возможен колебательный режим регулирования");
                    }
                    else
                    {
                        listE.Add("нет");
                    }
                }
                else
                {
                    if (V > 40 && lp5RadioButtonList1.SelectedIndex == 1)
                        listE.Add("возможен шум");
                    else if (V > 60 && lp5RadioButtonList1.SelectedIndex == 0)
                        listE.Add("возможен шум");
                    else
                    {
                        listE.Add("нет");
                    }
                }



                if (!String.IsNullOrWhiteSpace(this.calcrTextBox1.Text) && !String.IsNullOrWhiteSpace(this.calcrTextBox2.Text))
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

                    double t1 = customConverterToDouble(this.calcrTextBox2.Text);
                    Newtonsoft.Json.Linq.JObject max = dataFromFile.table9[dataFromFile.table9.Count - 1];
                    foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9)
                    {
                        if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1))
                        {
                            max = ob;
                        }
                    }
                    ps = getPSbyT(t1);

                    double G = Math.Round((dn * ((customConverterToDouble(this.calcrTextBox1.Text) * arrConvert3[this.calcrDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - ps)), 2);
                    listG.Add(G.ToString());


                    string K = "Нет";
                    if (G < Pf) //double.Parse(listResult["D"].GetValue(i).ToString()) )//Pf)
                        K = "Угрожает опасность кавитации";
                    if (eorRadioButtonList1.SelectedIndex == 0 && (G < g_dict["p25"]))
                        K = "Угрожает опасность кавитации";
                    if (eorRadioButtonList1.SelectedIndex == 1 && (G < (g_dict["p26"] - g_dict["p28"])))
                        K = "Угрожает опасность кавитации";
                    if (eorRadioButtonList1.SelectedIndex == 2 && (G < (g_dict["p30"] - g_dict["p32"])))
                        K = "Угрожает опасность кавитации";
                    if (eorRadioButtonList1.SelectedIndex == 3 && (G < g_dict["p19"]))
                        K = "Угрожает опасность кавитации";
                    listK.Add(K);
                }
            }

            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                listResult["D"] = listD.ToArray();
                listResult["G"] = listG.ToArray();
                listResult["K"] = listK.ToArray();
            }
            else
            {
                int indexNo = listE.IndexOf("нет");
                List<string> listA = new List<string>(),
                  listB = new List<string>(),
                  listC = new List<string>();

                listA.AddRange(listResult["A"]);
                listB.AddRange(listResult["B"]);
                listC.AddRange(listResult["C"]);

                if (indexNo != -1)
                {
                    if (listB.Count - 1 > indexNo + 1)
                    {
                        listA.RemoveRange(indexNo + 1, listA.Count - indexNo - 1);
                        listB.RemoveRange(indexNo + 1, listB.Count - indexNo - 1);
                        listC.RemoveRange(indexNo + 1, listC.Count - indexNo - 1);
                        listE.RemoveRange(indexNo + 1, listE.Count - indexNo - 1);
                        listF.RemoveRange(indexNo + 1, listF.Count - indexNo - 1);

                        listResult["A"] = listA.ToArray();
                        listResult["B"] = listB.ToArray();
                        listResult["C"] = listC.ToArray();
                    }
                }
            }

            listResult["F"] = listF.ToArray();
            listResult["E"] = listE.ToArray();
            /*/GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG*/
            /*/FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF*/

            return listResult;
        }
        catch (Exception e)
        {
            Logger.Log.Error(e);
            return null;
        }

    }

    private void mapInputParametersR(ref Dictionary<int, string> r_in_dict)
    {
        try
        {
            //
            r_in_dict.Add(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            r_in_dict.Add(1, DateTime.Now.ToShortDateString().ToString());
            r_in_dict.Add(2, "-"); // Объект добавляется в диалоговом окне при сохранении

            bool flag25Bar = get25BarFlag();

            IEnumerable<RadioButton> ie_rb = null;

            r_in_dict.Add(3, "-"); //было ИТП ЦТП

            r_in_dict.Add(4, eorRadioButtonList1.Items[eorRadioButtonList1.SelectedIndex].Text);

            r_in_dict.Add(5, "Marka"); // Марка добавляется в диалоговом окне при сохранении

            if (ws1RadioButtonList1.SelectedIndex == 3)
            {
                if (lp5RadioButtonList1.SelectedIndex == 0)
                {
                    r_in_dict.Add(6, "Водяной пар перегретый");
                }
                else
                {
                    r_in_dict.Add(6, "Водяной пар насыщенный");
                }
            }
            else
            {
                r_in_dict.Add(6, ws1RadioButtonList1.Items[ws1RadioButtonList1.SelectedIndex].Text + " " + ((this.ws1TextBox1.Enabled) ? (this.ws1TextBox1.Text + " %, " + this.ws1TextBox2.Text + " °С") : ""));

            }


            r_in_dict.Add(7, (this.lp1TextBox1.Enabled) ? this.lp1TextBox1.Text : "-");
            r_in_dict.Add(8, (this.lp1TextBox1.Enabled) ? this.lp1DropDownList1.Text : "-");

            r_in_dict.Add(9, (this.lp1TextBox2.Enabled) ? this.lp1TextBox2.Text : "-");
            r_in_dict.Add(10, (this.lp1TextBox2.Enabled) ? this.lp1DropDownList2.Text : "-");

            r_in_dict.Add(11, (this.lp1TextBox3.Enabled) ? this.lp1TextBox3.Text : "-");
            r_in_dict.Add(12, (this.lp1TextBox3.Enabled) ? this.lp1DropDownList3.Text : "-");

            r_in_dict.Add(13, (this.lp1TextBox4.Enabled) ? this.lp1TextBox4.Text : "-");
            r_in_dict.Add(14, (this.lp1TextBox4.Enabled) ? this.lp1DropDownList4.Text : "-");

            r_in_dict.Add(15, (this.lp1TextBox5.Text != "") ? this.lp1TextBox5.Text : "-");

            r_in_dict.Add(16, (this.lp2TextBox1.Enabled) ? this.lp2TextBox1.Text : "-");
            r_in_dict.Add(17, (this.lp2TextBox1.Enabled) ? this.lp2DropDownList1.Text : "-");

            r_in_dict.Add(18, (this.lp2TextBox2.Enabled) ? this.lp2TextBox2.Text : "-");
            r_in_dict.Add(19, (this.lp2TextBox2.Enabled) ? this.lp2DropDownList2.Text : "-");

            // для пара еще нет,  уже и не будет
            r_in_dict.Add(20, "-");
            r_in_dict.Add(21, "-");

            r_in_dict.Add(22, "-");
            r_in_dict.Add(23, "-");

            r_in_dict.Add(24, "-");
            // для пара еще нет

            r_in_dict.Add(25, (this.lp3TextBox1.Enabled) ? this.lp3TextBox1.Text : "-");
            r_in_dict.Add(26, (this.lp3TextBox1.Enabled) ? this.lp3DropDownList1.Text : "-");

            r_in_dict.Add(27, (this.lp3TextBox2.Enabled) ? this.lp3TextBox2.Text : "-");
            r_in_dict.Add(28, (this.lp3TextBox2.Enabled) ? this.lp3DropDownList2.Text : "-");

            r_in_dict.Add(29, (this.lp4TextBox2.Enabled) ? this.lp4TextBox2.Text : "-");
            r_in_dict.Add(30, (this.lp4TextBox2.Enabled) ? this.lp4DropDownList2.Text : "-");

            r_in_dict.Add(31, (this.calcrTextBox1.Enabled) ? this.calcrTextBox1.Text : "-");
            r_in_dict.Add(32, (this.calcrDropDownList1.Enabled) ? this.calcrDropDownList1.Text : "-");

            r_in_dict.Add(33, (this.calcrTextBox2.Text != "") ? this.calcrTextBox2.Text : "-");

            r_in_dict.Add(38, "-");
            r_in_dict.Add(381, "-");

            if (this.fprTextBox1.Enabled)
            {
                r_in_dict[38] = this.fprTextBox1.Text;
                r_in_dict[381] = this.fprDropDownList1.Text;

                //r_input_dict.Add(38, (this.fprTextBox1.Enabled) ? this.fprTextBox1.Text : "-");
                //r_input_dict.Add(381, (this.fprTextBox1.Enabled) ? this.fprDropDownList1.Text : "-");
            }

            r_in_dict.Add(34, (this.fprTextBox2.Enabled) ? this.fprTextBox2.Text : "-");
            r_in_dict.Add(35, (this.fprTextBox3.Enabled) ? this.fprTextBox3.Text : "-");

            r_in_dict.Add(36, (this.fprTextBox4.Enabled) ? this.fprTextBox4.Text : "-");
            r_in_dict.Add(37, (this.fprTextBox4.Enabled) ? this.fprDropDownList2.Text : "-");

            if (this.fprTextBox4.Enabled)
            {
                r_in_dict[38] = this.fprTextBox5.Text;
                r_in_dict[381] = "кг/ч";

                //r_input_dict.Add(38, (this.fprTextBox4.Enabled) ? this.fprTextBox5.Text : "-");
                //r_input_dict.Add(381, (this.fprTextBox4.Enabled) ? "кг/ч" : "-");
            }
           

            if (eorRadioButtonList1.SelectedIndex > 1)
            {
                r_in_dict.Add(39, "150 ˚С");
            }
            else
            {
                if (ws1RadioButtonList1.SelectedIndex != 3)
                {
                    if (ws1RadioButtonList1.SelectedIndex == 0)
                    {
                        if (customConverterToDouble(this.calcrTextBox2.Text) <= MaxT3x && customConverterToDouble(this.fprTextBox2.Text) <= MaxT3x)
                        {
                            r_in_dict.Add(39, "150˚С");
                        }
                        else
                        {
                            r_in_dict.Add(39, "220˚С");
                        }
                    }
                    else
                    {
                        r_in_dict.Add(39, "150˚С");
                    }
                }
                else
                {
                    r_in_dict.Add(39, "220˚С");
                }
            }

            if (flag25Bar)
            {
                r_in_dict.Add(40, "25 бар");
            }
            else
            {
                r_in_dict.Add(40, "16 бар");
            }
            

            r_in_dict.Add(41, "-");
            r_in_dict.Add(42, "-");
            r_in_dict.Add(43, "-");
            r_in_dict.Add(44, "-");
            r_in_dict.Add(45, "-");
            r_in_dict.Add(46, "-");
            r_in_dict.Add(47, "-");
            r_in_dict.Add(48, "-");
            r_in_dict.Add(49, "-");

            r_in_dict.Add(51, "-");
            r_in_dict.Add(52, "-");
            r_in_dict.Add(53, "-");
            r_in_dict.Add(54, "-");

            //для пара

            r_in_dict.Add(55, (this.lp5TextBox1.Enabled) ? this.lp5TextBox1.Text : "-");
            r_in_dict.Add(56, (this.lp5TextBox1.Enabled) ? this.lp5DropDownList1.Text : "-");

            r_in_dict.Add(57, (this.lp5TextBox2.Enabled) ? this.lp5TextBox2.Text : "-");
            r_in_dict.Add(58, (this.lp5TextBox2.Enabled) ? this.lp5DropDownList2.Text : "-");

            if (ws1RadioButtonList1.SelectedIndex == 3)
            {
                if (lp5RadioButtonList1.SelectedIndex == 0)
                {
                    r_in_dict.Add(59, "Водяной пар перегретый");
                }
                else
                {
                    r_in_dict.Add(59, "Водяной пар насыщенный");
                }
            }
            else
            {
                r_in_dict.Add(59, "-");
            }

            if (ws1RadioButtonList1.SelectedIndex == 3)
            {
                r_in_dict.Add(60, this.lp5TextBox3.Text);
            }
            else
            {
                r_in_dict.Add(60, "-");
            }
            
            if (lp5RadioButtonList1.SelectedIndex == 0)
            {
                r_in_dict.Add(61, (Label38.Text));
            }

            Session["r_input_dict"] = r_in_dict;

        }
        catch (Exception e)
        {
            Logger.Log.Error(e);
        }
    }

    private void getDimsR(ref Dictionary<int, string> r_in_dict)
    {
        var vr = dataFromFile;
        Newtonsoft.Json.Linq.JArray tableDimR = null;
        if (eorRadioButtonList1.SelectedIndex == 0 || eorRadioButtonList1.SelectedIndex == 1)
        {
            var vr1 = dataFromFile;

            if (r_input_dict[39] == "220˚С")
            {
                if (r_in_dict[41].Contains("-0.1-"))
                    tableDimR = dataFromFile.table24;
                else tableDimR = dataFromFile.table25;
            } 
            else
            {
                if (r_in_dict[41].Contains("-0.1-"))
                    tableDimR = dataFromFile.table20;
                else tableDimR = dataFromFile.table21;
            }
            
        }
        else
        {
            if (r_in_dict[41].Contains("-0.1-"))
                tableDimR = dataFromFile.table22;
            else tableDimR = dataFromFile.table23;
        }

        foreach (Newtonsoft.Json.Linq.JObject el in tableDimR)
        {
            string d = el.GetValue("d").ToString();
            if (d.Equals(r_in_dict[42]))
            {
                r_in_dict[51] = el.GetValue("l").ToString();
                r_in_dict[52] = el.GetValue("h1").ToString();
                r_in_dict[53] = el.GetValue("h").ToString();
                r_in_dict[54] = el.GetValue("m").ToString();
            }
        }
    }

    //------------------------------------Table Function END--------------------------------------

    //------------------------------------File Function START--------------------------------------

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
                dataFromFile = JsonConvert.DeserializeObject(jsonText);
            }
        }
        catch (Exception e)
        {
            Logger.Log.Error(e);

        }
    }

    private void changeImage(int index)
    {
        switch (index)
        {
            case 0:
                rPictureBox.ImageUrl = "~/Content/images/RDT-RDT-P.png";
                break;
            case 1:
                rPictureBox.ImageUrl = "~/Content/images/RDT-RDT-P.png";
                break;
            case 2:
                rPictureBox.ImageUrl = "~/Content/images/RDT-S-RDT-B.png";
                break;
            case 3:
                rPictureBox.ImageUrl = "~/Content/images/RDT-S-RDT-B.png";
                break;

            default:
                rPictureBox.ImageUrl = null;
                break;
        }
        rPictureBox.Visible = true;
    }

    //------------------------------------File Function END--------------------------------------

    //------------------------------------Validation Function START--------------------------------------

    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator8.IsValid)
        {
            if (lp1DropDownList3.Enabled)
            {
                if (lp1TextBox3.Enabled == false || checkTextBoxEmpty(lp1TextBox3))
                {
                    CustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lp1TextBox3.Text) < minVar)
                {
                    CustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }

                if (ws1RadioButtonList1.SelectedIndex != 3)
                {
                    if (convertArrToBar(arrConvert3, lp1DropDownList3, lp1TextBox3) > PressureBeforeValve2x)
                    {
                        CustomValidator1.ErrorMessage = "На давление свыше 25 бар вариантов нет";
                        args.IsValid = false;
                        return;
                    }
                } else
                {
                    if (convertArrToBar(arrConvert3, lp1DropDownList3, lp1TextBox3) > PressureBeforeValve3x)
                    {
                        CustomValidator1.ErrorMessage = "На давление свыше 16 бар вариантов нет";
                        args.IsValid = false;
                        return;
                    }
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator1.ErrorMessage = "";
        }
    }

    protected void CustomValidator2_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator1.IsValid)
        {
            if (lp1DropDownList4.Enabled)
            {

                if (lp1TextBox4.Enabled == false || checkTextBoxEmpty(lp1TextBox4))
                {
                    CustomValidator2.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lp1TextBox4.Text) < minVar)
                {
                    CustomValidator2.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (convertArrToBar(arrConvert3, lp1DropDownList4, lp1TextBox4) >= convertArrToBar(arrConvert3, lp1DropDownList3, lp1TextBox3))
                {
                    CustomValidator2.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }

                double p17, p19, p21, p23;

                p17 = customConverterToDouble(this.lp1TextBox1.Text) * arrConvert3[this.lp1DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                p19 = customConverterToDouble(this.lp1TextBox2.Text) * arrConvert3[this.lp1DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                p21 = customConverterToDouble(this.lp1TextBox3.Text) * arrConvert3[this.lp1DropDownList3.SelectedIndex - 1] / arrConvert3[2];
                p23 = customConverterToDouble(this.lp1TextBox4.Text) * arrConvert3[this.lp1DropDownList4.SelectedIndex - 1] / arrConvert3[2];

                if (!((p17 + p19) <= (p21 - p23)))
                {
                    CustomValidator2.ErrorMessage = "";
                    LabelCustomValid.Visible = true;
                    args.IsValid = false;
                    return;
                }
                else
                {
                    LabelCustomValid.Visible = false;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator2.ErrorMessage = "";
            return;
        }

    }
    protected void CustomValidator3_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (lp2DropDownList1.Enabled)
        {
            if (lp2TextBox1.Enabled == false || checkTextBoxEmpty(lp2TextBox1))
            {
                CustomValidator3.ErrorMessage = "Необходимо заполнить поле";
                args.IsValid = false;
                return;
            }
            if (customConverterToDouble(lp2TextBox1.Text) < minVar)
            {
                CustomValidator3.ErrorMessage = "Неверно указано значение давления";
                args.IsValid = false;
                return;
            }
            
            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                if (convertArrToBar(arrConvert3, lp2DropDownList1, lp2TextBox1) > PressureBeforeValve2x)
                {
                    CustomValidator3.ErrorMessage = "На давление свыше 25 бар вариантов нет";
                    args.IsValid = false;
                }
            }
            else
            {
                if (convertArrToBar(arrConvert3, lp2DropDownList1, lp2TextBox1) > PressureBeforeValve3x)
                {
                    CustomValidator3.ErrorMessage = "На давление свыше 16 бар вариантов нет";
                    args.IsValid = false;
                }
            }
        }
    }

    protected void CustomValidator4_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator3.IsValid)
        {
            if (lp2DropDownList2.Enabled)
            {
                if (lp2TextBox2.Enabled == false || checkTextBoxEmpty(lp2TextBox2))
                {
                    CustomValidator4.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lp2TextBox2.Text) < minVar)
                {
                    CustomValidator4.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (convertArrToBar(arrConvert3, lp2DropDownList2, lp2TextBox2) >= convertArrToBar(arrConvert3, lp2DropDownList1, lp2TextBox1))
                {
                    CustomValidator4.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator4.ErrorMessage = "";
        }
    }

    protected void CustomValidator5_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (lp3DropDownList1.Enabled)
        {
            if (lp3TextBox1.Enabled == false || checkTextBoxEmpty(lp3TextBox1))
            {
                CustomValidator5.ErrorMessage = "Необходимо заполнить поле";
                args.IsValid = false;
                return;
            }
            if (customConverterToDouble(lp3TextBox1.Text) < minVar)
            {
                CustomValidator5.ErrorMessage = "Неверно указано значение давления";
                args.IsValid = false;
                return;
            }
            
            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                if (convertArrToBar(arrConvert3, lp3DropDownList1, lp3TextBox1) > PressureBeforeValve2x)
                {
                    CustomValidator5.ErrorMessage = "На давление свыше 25 бар вариантов нет";
                    args.IsValid = false;
                }
            }
            else
            {
                if (convertArrToBar(arrConvert3, lp3DropDownList1, lp3TextBox1) > PressureBeforeValve3x)
                {
                    CustomValidator5.ErrorMessage = "На давление свыше 16 бар вариантов нет";
                    args.IsValid = false;
                }
            }
        }
    }

    protected void CustomValidator6_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator5.IsValid)
        {
            if (lp3DropDownList2.Enabled)
            {
                if (lp3TextBox2.Enabled == false || checkTextBoxEmpty(lp3TextBox2))
                {
                    CustomValidator6.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lp3TextBox2.Text) < minVar)
                {
                    CustomValidator6.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (convertArrToBar(arrConvert3, lp3DropDownList2, lp3TextBox2) >= convertArrToBar(arrConvert3, lp3DropDownList1, lp3TextBox1))
                {
                    CustomValidator6.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator6.ErrorMessage = "";
        }
    }

    protected void CustomValidator7_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (lp4DropDownList2.Enabled)
        {
            if (lp4TextBox2.Enabled == false || checkTextBoxEmpty(lp4TextBox2))
            {
                CustomValidator7.ErrorMessage = "Необходимо заполнить поле";
                args.IsValid = false;
                return;
            }
            if (customConverterToDouble(lp4TextBox2.Text) < minVar)
            {
                CustomValidator7.ErrorMessage = "Неверно указано значение давления";
                args.IsValid = false;
                return;
            }
            
            if (ws1RadioButtonList1.SelectedIndex != 3)
            {
                if (convertArrToBar(arrConvert3, lp4DropDownList2, lp4TextBox2) > PressureBeforeValve2x)
                {
                    CustomValidator7.ErrorMessage = "На давление свыше 25 бар вариантов нет";
                    args.IsValid = false;
                }
            }
            else
            {
                if (convertArrToBar(arrConvert3, lp4DropDownList2, lp4TextBox2) > PressureBeforeValve3x)
                {
                    CustomValidator7.ErrorMessage = "На давление свыше 16 бар вариантов нет";
                    args.IsValid = false;
                }
            }

            lp1TextBox4.Text = ConvertPointToComma(lp1TextBox4.Text);
        }

        return;
    }

    protected void CustomValidator8_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator10.IsValid)
        {
            if (lp1DropDownList2.Enabled)
            {
                if (lp1TextBox2.Enabled == false || checkTextBoxEmpty(lp1TextBox2))
                {
                    CustomValidator8.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lp1TextBox2.Text) < minVar)
                {
                    CustomValidator8.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator8.ErrorMessage = "";
        }
        return;
    }

    protected void CustomValidator9_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator2.IsValid && CustomValidator4.IsValid && CustomValidator6.IsValid && CustomValidator7.IsValid )
        {
            if (calcrDropDownList1.Enabled)
            {
                if (calcrTextBox1.Enabled == false || checkTextBoxEmpty(calcrTextBox1))
                {
                    CustomValidator9.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(calcrTextBox1.Text) < minVar)
                {
                    CustomValidator9.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                
                if (ws1RadioButtonList1.SelectedIndex != 3)
                {
                    if (convertArrToBar(arrConvert3, calcrDropDownList1, calcrTextBox1) > PressureBeforeValve2x)
                    {
                        CustomValidator9.ErrorMessage = "На давление свыше 25 бар вариантов нет";
                        args.IsValid = false;
                    }
                }
                else
                {
                    if (convertArrToBar(arrConvert3, calcrDropDownList1, calcrTextBox1) > PressureBeforeValve3x)
                    {
                        CustomValidator9.ErrorMessage = "На давление свыше 16 бар вариантов нет";
                        args.IsValid = false;
                    }
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator9.ErrorMessage = "";
        }
        return;
    }

    protected void CustomValidator10_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator17.IsValid && CustomValidator22.IsValid)
        {
            if (lp1DropDownList1.Enabled)
            {
                if (lp1TextBox1.Enabled == false || checkTextBoxEmpty(lp1TextBox1))
                {
                    CustomValidator10.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }

                if (customConverterToDouble(lp1TextBox1.Text) < minVar)
                {
                    CustomValidator10.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator10.ErrorMessage = "";
        }
    }
    protected void CustomValidator11_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator9.IsValid)
        {
            if (calcrTextBox2.Enabled != false)
            {
                if (calcrTextBox2.Enabled == false || checkTextBoxEmpty(calcrTextBox2))
                {
                    CustomValidator11.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(calcrTextBox2.Text) < minVar)
                {
                    CustomValidator11.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return;
                }
                if(eorRadioButtonList1.SelectedIndex > 1)
                {
                    if (customConverterToDouble(calcrTextBox2.Text) > MaxT3x)
                    {
                        CustomValidator11.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                        args.IsValid = false;
                        return;
                    }
                }
                else
                {
                    if (this.ws1RadioButtonList1.SelectedIndex == 0)
                    {
                        if (customConverterToDouble(calcrTextBox2.Text) > MaxT2x)
                        {
                            CustomValidator11.ErrorMessage = "На температуру свыше 220&#8451; вариантов нет";
                            args.IsValid = false;
                            return;
                        }

                    }
                    else
                    {
                        if (customConverterToDouble(calcrTextBox2.Text) > MaxT3x)
                        {
                            CustomValidator11.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                            args.IsValid = false;
                            return;
                        }
                    }

                }

                if (((customConverterToDouble(this.calcrTextBox1.Text) * arrConvert3[this.calcrDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - getPSbyT(customConverterToDouble(this.calcrTextBox2.Text))) <= 0)
                {
                    CustomValidator11.ErrorMessage = "Указанная температура выше температуры парообразования. При указанной температуре в трубопроводе движется пар";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator11.ErrorMessage = "";
        }
    }

    protected void CustomValidator12_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator11.IsValid && CustomValidator20.IsValid)
        {
            if (fprDropDownList1.Enabled)
            {
                if (fprTextBox1.Enabled == false || checkTextBoxEmpty(fprTextBox1))
                {
                    CustomValidator12.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(fprTextBox1.Text) < minVar)
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
    protected void CustomValidator13_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (fprRadioButton2.Checked)
        {
            if (CustomValidator9.IsValid)
            {
                if (fprTextBox2.Enabled == false || checkTextBoxEmpty(fprTextBox2))
                {
                    CustomValidator13.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(fprTextBox2.Text) < minVar)
                {
                    CustomValidator13.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return;
                }
                if (eorRadioButtonList1.SelectedIndex > 1)
                {
                    if (customConverterToDouble(fprTextBox2.Text) > MaxT3x)
                    {
                        CustomValidator13.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                        args.IsValid = false;
                        return;
                    }
                }
                else
                {
                    if (this.ws1RadioButtonList1.SelectedIndex == 0)
                    {
                        if (customConverterToDouble(fprTextBox2.Text) > MaxT2x)
                        {
                            CustomValidator13.ErrorMessage = "На температуру свыше 220&#8451; вариантов нет";
                            args.IsValid = false;
                            return;
                        }
                    }
                    else
                    {
                        if (customConverterToDouble(fprTextBox2.Text) > MaxT3x)
                        {
                            CustomValidator13.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                            args.IsValid = false;
                            return;
                        }
                    }
                }
            }
            else
            {
                args.IsValid = false;
                CustomValidator13.ErrorMessage = "";
            }
        }
    }
    protected void CustomValidator14_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (fprRadioButton2.Checked)
        {
            if (CustomValidator13.IsValid)
            {
                if (fprTextBox3.Enabled == false || checkTextBoxEmpty(fprTextBox3))
                {
                    CustomValidator14.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(fprTextBox3.Text) < minVar)
                {
                    CustomValidator14.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return;
                }
                if (eorRadioButtonList1.SelectedIndex > 1)
                {
                    if (customConverterToDouble(fprTextBox3.Text) > MaxT3x)
                    {
                        CustomValidator14.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                        args.IsValid = false;
                        return;
                    }
                }
                else
                {
                    if (this.ws1RadioButtonList1.SelectedIndex == 0)
                    {
                        if (customConverterToDouble(fprTextBox3.Text) > MaxT2x)
                        {
                            CustomValidator14.ErrorMessage = "На температуру свыше 220&#8451; вариантов нет";
                            args.IsValid = false;
                            return;
                        }
                    }
                    else
                    {
                        if (customConverterToDouble(fprTextBox3.Text) > MaxT3x)
                        {
                            CustomValidator14.ErrorMessage = "На температуру свыше 150&#8451; вариантов нет";
                            args.IsValid = false;
                            return;
                        }
                    }
                }
                if (customConverterToDouble(fprTextBox3.Text) >= customConverterToDouble(fprTextBox2.Text))
                {
                    CustomValidator14.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return;
                }
            }
            else
            {
                args.IsValid = false;
                CustomValidator14.ErrorMessage = "";
            }
        }
    }
    protected void CustomValidator15_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator14.IsValid)
        {
            if (fprDropDownList2.Enabled)
            {
                if (fprTextBox4.Enabled == false || checkTextBoxEmpty(fprTextBox4))
                {
                    CustomValidator15.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(fprTextBox4.Text) < minVar)
                {
                    CustomValidator15.ErrorMessage = "Неверно указано значение тепловой мощности";
                    args.IsValid = false;

                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator15.ErrorMessage = "";
        }
    }

    protected void CustomValidator16_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator22.IsValid)
        {
            if (ws1RadioButtonList1.SelectedIndex == 1 || ws1RadioButtonList1.SelectedIndex == 2)
            {
                if (ws1TextBox1.Enabled == false || checkTextBoxEmpty(ws1TextBox1))
                {
                    CustomValidator16.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(ws1TextBox1.Text) < 5 || customConverterToDouble(ws1TextBox1.Text) > 65)
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
        if (ws1RadioButtonList1.SelectedIndex == 1 || ws1RadioButtonList1.SelectedIndex == 2)
        {
            if (CustomValidator16.IsValid)
            {
                if (ws1TextBox2.Enabled == false || checkTextBoxEmpty(ws1TextBox2))
                {
                    CustomValidator17.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(ws1TextBox2.Text) < 0 || customConverterToDouble(ws1TextBox2.Text) > MaxT3x)
                {
                    CustomValidator17.ErrorMessage = "Значение должно находится в диапазоне от 0&#8451 до 150&#8451";
                    args.IsValid = false;
                    return;
                }

            }
            else
            {
                args.IsValid = false;
                CustomValidator17.ErrorMessage = "";
            }
        }
    }

    protected void CustomValidator18_ServerValidate(object source, ServerValidateEventArgs args) //пар
    {
        if (CustomValidator21.IsValid)
        {
            if (lp5DropDownList1.Enabled)
            {
                if (lp5TextBox1.Enabled == false || checkTextBoxEmpty(lp5TextBox1))
                {
                    CustomValidator18.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lp5TextBox1.Text) < minVar)
                {
                    CustomValidator18.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if(eorRadioButtonList1.SelectedIndex == 1 && ws1RadioButtonList1.SelectedIndex == 3)
                {
                    if (convertArrToBar(arrConvert3, lp5DropDownList1, lp5TextBox1) > PressureBeforeValve2x)
                    {
                        CustomValidator18.ErrorMessage = "На давление свыше 25 бар вариантов нет";
                        args.IsValid = false;
                    }
                } 
                else
                {
                    if (convertArrToBar(arrConvert3, lp5DropDownList1, lp5TextBox1) > PressureBeforeValve3x)
                    {
                        CustomValidator18.ErrorMessage = "На давление свыше 16 бар вариантов нет";
                        args.IsValid = false;
                    }
                }
                
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator18.ErrorMessage = "";
        }
    }

    protected void CustomValidator19_ServerValidate(object source, ServerValidateEventArgs args) //пар
    {
        if (CustomValidator18.IsValid)
        {
            if (lp5DropDownList2.Enabled)
            {
                if (lp5TextBox2.Enabled == false || checkTextBoxEmpty(lp5TextBox2))
                {
                    CustomValidator19.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (customConverterToDouble(lp5TextBox2.Text) < minVar)
                {
                    CustomValidator19.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (convertArrToBar(arrConvert3, lp5DropDownList2, lp5TextBox2) >= convertArrToBar(arrConvert3, lp5DropDownList1, lp5TextBox1))
                {
                    CustomValidator19.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator19.ErrorMessage = "";
        }
    }

    protected void CustomValidator20_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator19.IsValid)
        {
            if (lp5RadioButtonList1.SelectedIndex == 0)
            {
            
                if (ws1RadioButtonList1.SelectedIndex == 3)
                {
                    if (lp5TextBox3.Enabled == false || checkTextBoxEmpty(lp5TextBox3))
                    {
                        CustomValidator20.ErrorMessage = "Необходимо заполнить поле";
                        args.IsValid = false;
                        return;
                    }
                    if (customConverterToDouble(lp5TextBox3.Text) < minVar)
                    {
                        CustomValidator20.ErrorMessage = "Неверно указано значение температуры";
                        args.IsValid = false;
                        return;
                    }

                    if (customConverterToDouble(lp5TextBox3.Text) > MaxT2x)
                    {
                        CustomValidator20.ErrorMessage = "На температуру свыше 220°С вариантов нет";
                        args.IsValid = false;
                        return;
                    }

                    if (customConverterToDouble(lp5TextBox3.Text) < (100 * Math.Pow((customConverterToDouble(lp5TextBox1.Text) * arrConvert3[lp5DropDownList1.SelectedIndex - 1] / arrConvert3[2]) + 1, 0.25)))
                    {
                        CustomValidator20.ErrorMessage = "Указанная температура ниже температуры парообразования. При указанной температуре в трубопроводе движется жидкость";
                        args.IsValid = false;
                        return;
                    }

                    if ((customConverterToDouble(lp5TextBox1.Text) > (25 - 0.025 * (customConverterToDouble(lp5TextBox3.Text) - 120))) && customConverterToDouble(lp5TextBox3.Text) > 120)
                    {
                        CustomValidator20.ErrorMessage = "При указанном давлении P'1 и температуре Т1 нужен корпус с Ру больше 25 бар, вариантов нет";
                        args.IsValid = false;
                        return;
                    }
                }
            }
            
        }
        else
        {
            args.IsValid = false;
            CustomValidator20.ErrorMessage = "";
        }
    }

    protected void CustomValidator21_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (ws1RadioButtonList1.SelectedIndex == 3)
        {
            
            if (lp5RadioButtonList1.SelectedIndex == -1)
            {
                CustomValidator21.ErrorMessage = "Необходимо выбрать тип пара";
                args.IsValid = false;
                return;
            }
        }
      
    }

    protected void CustomValidator22_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (ws1RadioButtonList1.SelectedIndex == -1)
        {
            CustomValidator22.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
            return;
        }
    }

    //------------------------------------Validation Function END--------------------------------------

    //------------------------------------Support Function START--------------------------------------

    private bool checkTextBoxEmpty(TextBox tb)
    {
        return tb.Text == "";
    }

    public void DisableTextBox(TextBox textBox)
    {
        textBox.Enabled = false;
        textBox.Text = "";
    }

    public bool SetEnableTextBox(DropDownList dropDownList, TextBox textBox)
    {
        bool flag = false;
        try { 

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
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
        return flag;

    }

    private void SavePrevSelectedIndexDDL(string id, int key)
    {
        try
        {
            Session[id] = key;
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
   
    }

    private void convertArrDouble(double[,] arr, DropDownList ddl, ref TextBox tb)
    {
        if (ddl.SelectedIndex > 0)
        {
            if (!String.IsNullOrWhiteSpace(tb.Text))
            {
                int jj = Convert.ToInt32(Session[ddl.ID]);

                if (jj > 0)
                {
                    tb.Text = (customConverterToDouble((tb.Text).Replace(".", ",")) * arr[(jj - 1), (ddl.SelectedIndex - 1)]).ToString().Replace(",", ".");
                }
            }
        }
    }

    private void convertArr(double[] arr, DropDownList ddl, ref TextBox tb)
    {
        try
        {
            if (ddl.SelectedIndex > 0)
            {
                if (!String.IsNullOrWhiteSpace(tb.Text))
                {
                    int jj = Convert.ToInt32(Session[ddl.ID]);
                    tb.Text = (customConverterToDouble((tb.Text).Replace(".", ",")) * arr[jj - 1] / arr[ddl.SelectedIndex - 1]).ToString().Replace(",", ".");
                }
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

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
        double afterConvert = 0;

        if (tb != "" && tb != null)
        {
            if (tb.IndexOf(".") != -1)
            {
                string beforeConvert = tb.Replace(".", ",");
                afterConvert = Convert.ToDouble(beforeConvert);
            }
            else
            {
                afterConvert = Convert.ToDouble(tb);
            }
        }
        return afterConvert;
    }

    public void lp1ControlEnable(bool flag)
    {
        dropDownListEnable(lp1DropDownList1, flag);
        textBoxDisable(lp1TextBox1);
        dropDownListEnable(lp1DropDownList2, flag);
        textBoxDisable(lp1TextBox2);
        dropDownListEnable(lp1DropDownList3, flag);
        textBoxDisable(lp1TextBox3);
        dropDownListEnable(lp1DropDownList4, flag);
        textBoxDisable(lp1TextBox4);
        textBoxDisable(lp1TextBox5);
    }
    public void lp2ControlEnable(bool flag)
    {
        dropDownListEnable(lp2DropDownList1, flag);
        textBoxDisable(lp2TextBox1);
        dropDownListEnable(lp2DropDownList2, flag);
        textBoxDisable(lp2TextBox2);
    }
    public void lp3ControlEnable(bool flag)
    {
        dropDownListEnable(lp3DropDownList1, flag);
        textBoxDisable(lp3TextBox1);
        dropDownListEnable(lp3DropDownList2, flag);
        textBoxDisable(lp3TextBox2);
    }
    public void lp4ControlEnable(bool flag)
    {
        dropDownListEnable(lp4DropDownList2, flag);
        textBoxDisable(lp4TextBox2);
    }
    public void lp5ControlEnable(bool flag)
    {
        dropDownListEnable(lp5DropDownList1, flag);
        textBoxDisable(lp5TextBox1);
        dropDownListEnable(lp5DropDownList2, flag);
        textBoxDisable(lp5TextBox2);
        lp5TextBox3.Text = "";
        lp5RadioButtonList1.Enabled = flag;
        lp5RadioButtonList1.SelectedIndex = -1;
    }

    public void dropDownListEnable(DropDownList dropDownList, bool flag)
    {
        dropDownList.Enabled = flag;
        if (flag != true) 
        {
            dropDownList.ClearSelection();
        };
        
    }

    public void textBoxEnabled(TextBox textBox, bool flag)
    {
        textBox.Enabled = flag;
        if (flag == false)
        {
            textBox.Text = String.Empty;
        }
       
    }
    public void textBoxDisable(TextBox textBox)
    {
        if (LabelSteam.Text == "Y")
        {
            textBox.Enabled = false;
            textBox.Text = String.Empty;
        }
        

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

    //------------------------------------Support Function END--------------------------------------

    //------------------------------------Event Function START--------------------------------------

    protected void rButton_Click(object sender, EventArgs e)
    {

        if (!Page.IsValid) { return; }
        try
        {
            resultPanel.Visible = true;
            objTextBox1.Enabled = false;
            objTextBox1.Visible = false;
            Label53.Visible = false;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "MyClientScript", "javascript:HideBTN()", true);
            GridView1.Columns.Clear();
            GridView1.DataSource = null;
            GridView1.DataBind();
            GridView1.SelectedIndex = -1;

            this.readFile(0);
            Dictionary<string, double> g_dict = new Dictionary<string, double>();
            double g = 0;
            r_input_dict.Clear();
           
            g_dict.Add("vmax", 3);

            if (eorRadioButtonList1.SelectedIndex != -1)
            {
                if (ws1RadioButtonList1.SelectedIndex != -1)
                {

                    if (this.ws1RadioButtonList1.SelectedIndex == 1 || ws1RadioButtonList1.SelectedIndex == 2)
                    {
                            
                        Double p6 = -1;
                        Double p7 = -1;
                        try
                        {
                            p6 = customConverterToDouble(this.ws1TextBox1.Text);
                        }
                        catch (Exception)
                        {
                            LabelError.Text = "Не указано значение концентрации ";
                            return;
                        }

                        if (p6 < 5 || p6 > 65)
                        {
                            LabelError.Text = "Неверно указано значение концентрации ";
                            return;
                        }
                        else
                        {
                            g_dict.Add("p6", p6);
                        }

                        try
                        {
                            p7 = customConverterToDouble(this.ws1TextBox2.Text);
                        }
                        catch (Exception)
                        {
                            LabelError.Text = "Не указано значение температуры ";
                            return;
                        }

                        g_dict.Add("p7", p7);
                            
                    }

                    if (fprRadioButton1.Checked || fprRadioButton2.Checked)
                    {

                        if (ws1RadioButtonList1.SelectedIndex == 0)
                        {
                            Water(GetAvgT(), ref g);
                        }
                        else if (ws1RadioButtonList1.SelectedIndex == 1)
                        {
                            double pl6 = customConverterToDouble(this.ws1TextBox1.Text);
                            double pl7 = Math.Round(GetAvgT() / 10) * 10;
                            double cp = 0;
                            Etgl(pl7, pl6, ref g, ref cp);
                        }
                        else if (ws1RadioButtonList1.SelectedIndex == 2)
                        {
                            double pl6 = customConverterToDouble(this.ws1TextBox1.Text);
                            double pl7 = Math.Round(GetAvgT() / 10) * 10;
                            double cp = 0;
                            Prgl(pl7, pl6, ref g, ref cp);
                        }

                        double checkVal;

                        try
                        {
                            if (this.fprRadioButton1.Checked)
                            {
                                checkVal = customConverterToDouble(this.fprTextBox1.Text);
                                if (!(checkVal > 0))
                                {
                                    LabelError.Text = "Введите числовое значение больше нуля";
                                    return;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            LabelError.Text = "Неверно указано значение расхода через регулятор давления";
                            return;
                        }

                        try
                        {
                            if (this.fprRadioButton2.Checked)
                            {
                                checkVal = customConverterToDouble(this.fprTextBox2.Text);
                            }
                        }
                        catch (Exception)
                        {
                            LabelError.Text = "Неверно указано значение температуры";
                            return;
                        }

                        try
                        {
                            if (this.fprRadioButton2.Checked)
                            {
                                checkVal = customConverterToDouble(this.fprTextBox3.Text);
                            }
                        }
                        catch (Exception)
                        {
                            LabelError.Text = "Неверно указано значение температуры";
                            return;
                        }

                        try
                        {
                            if (this.fprRadioButton2.Checked)
                            {
                                checkVal = customConverterToDouble(this.fprTextBox4.Text);
                                if (!(checkVal > 0))
                                {
                                    LabelError.Text = "Введите числовое значение больше нуля";
                                    return;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            LabelError.Text = "Неверно указано значение тепловой мощности";
                            return;
                        }

                    if (this.fprRadioButton1.Checked && this.checkTextBoxEmpty(this.fprTextBox1))
                    {
                        LabelError.Text = "Не задан расход через регулятор давления";
                        return;
                    }
                    else if (this.fprRadioButton2.Checked && this.checkTextBoxEmpty(this.fprTextBox2))
                    {
                        LabelError.Text = "Не задано значение температуры";
                        return;
                    }
                       
                    else if (this.fprRadioButton2.Checked && this.checkTextBoxEmpty(this.fprTextBox3))
                    {
                        LabelError.Text = "Не задано значение температуры";
                        return;
                    }
                    else if (this.fprRadioButton2.Checked && customConverterToDouble(this.fprTextBox2.Text) <= customConverterToDouble(this.fprTextBox3.Text))
                    {
                        LabelError.Text = "Неверно указано значение температуры";
                        return;
                    }
                        
                    else if (this.fprRadioButton2.Checked && this.checkTextBoxEmpty(this.fprTextBox4))
                    {
                        LabelError.Text = "Не задано значение тепловой мощности";
                        return;
                    }
                    else
                    {
                        Double p16, p25 = 0;
                        if (this.fprRadioButton2.Checked)
                        {
                            try
                            {
                                    if (ws1RadioButtonList1.SelectedIndex != 3) { 
                                p16 = Math.Round((customConverterToDouble(this.fprTextBox4.Text) * arrConvert2[this.fprDropDownList2.SelectedIndex - 1]) * 3.6 / (this.math_16_cp() * (customConverterToDouble(this.fprTextBox2.Text) - customConverterToDouble(this.fprTextBox3.Text))), 2);

                                    }
                                    else
                                    {
                                        p16 = Math.Round((customConverterToDouble(this.fprTextBox4.Text) * arrConvert2[this.fprDropDownList2.SelectedIndex - 1]) * 3.6 / SteamCP(), 2);

                                    }
                                }
                                catch (Exception)
                                {
                                    LabelError.Text = "Неверно указано значение тепловой мощности";
                                    return;
                                }
                                this.fprTextBox5.Text = p16.ToString();
                                
                            }
                            else
                            {
                                    
                                if(fprDropDownList1.SelectedIndex > 4)
                                {
                                    p16 = (customConverterToDouble(this.fprTextBox1.Text) * arrConvert1[(this.fprDropDownList1.SelectedIndex - 1), 5]);

                                } 
                                else
                                {
                                    p16 = (customConverterToDouble(this.fprTextBox1.Text) * arrConvert1[(this.fprDropDownList1.SelectedIndex - 1), 5] * (g / 1000));
                                }
                            }

                            g_dict.Add("p16", p16);

                            if (eorRadioButtonList1.SelectedIndex == 0)
                            {
                                if (this.checkTextBoxEmpty(this.lp1TextBox1))
                                {
                                    LabelError.Text = "Неверно указано значение давления";

                                    return;
                                }
                                else if (this.checkTextBoxEmpty(this.lp1TextBox2))
                                {
                                    LabelError.Text = "Неверно указано значение давления";
                                    CustomValidator1.ErrorMessage = "Неверно указано значение давления";
                                    return;
                                }
                                else if (this.checkTextBoxEmpty(this.lp1TextBox3))
                                {
                                    LabelError.Text = "Неверно указано значение давления";
                                    return;
                                }
                                else if (this.checkTextBoxEmpty(this.lp1TextBox4))
                                {
                                    LabelError.Text = "Неверно указано значение давления";
                                    return;
                                }
                                else
                                {

                                    double p17, p19, p21, p23;

                                    try
                                    {
                                        p17 = customConverterToDouble(this.lp1TextBox1.Text) * arrConvert3[this.lp1DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p19 = customConverterToDouble(this.lp1TextBox2.Text) * arrConvert3[this.lp1DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p21 = customConverterToDouble(this.lp1TextBox3.Text) * arrConvert3[this.lp1DropDownList3.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p23 = customConverterToDouble(this.lp1TextBox4.Text) * arrConvert3[this.lp1DropDownList4.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    if (!(p17 > 0))
                                    {
                                        LabelError.Text = "Введите числовое значение больше нуля";
                                        return;
                                    }
                                    else if (!(p19 > 0))
                                    {
                                        LabelError.Text = "Введите числовое значение больше нуля";
                                        return;
                                    }
                                    else if (!(p21 > 0))
                                    {
                                        LabelError.Text = "Введите числовое значение больше нуля";
                                        return;
                                    }
                                    else if (!(p23 > 0))
                                    {
                                        LabelError.Text = "Введите числовое значение больше нуля";
                                        return;
                                    }

                                    if (!(p21 <= 25))
                                    {
                                        LabelError.Text = "На давление свыше 25 бар вариантов нет";
                                        return;
                                    }
                                    else if (!(p23 < p21))
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }
                                    else if (!((p17 + p19) <= (p21 - p23)))
                                    {
                                        LabelError.Text = "Суммарные потери давления на регуляторе и регулируемом участке превышают допустимый перепад давлений на вводе";
                                        return;
                                    }
                                    else
                                    {

                                        g_dict.Add("p17", p17);
                                        g_dict.Add("p19", p19);
                                        g_dict.Add("p21", p21);
                                        g_dict.Add("p23", p23);

                                        p25 = Math.Round(p21 - p23 - p19, 2);
                                        g_dict.Add("p25", p25);
                                        lp1TextBox5.Text = p25.ToString();
                                    }
                                }
                            }
                            else if (eorRadioButtonList1.SelectedIndex == 1)
                            {
                                if (ws1RadioButtonList1.SelectedIndex < 3)
                                {
                                    if (this.checkTextBoxEmpty(this.lp2TextBox1))
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }
                                    else if (this.checkTextBoxEmpty(this.lp2TextBox2))
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }
                                    else
                                    {

                                        double p26, p28;

                                        try
                                        {
                                            p26 = customConverterToDouble(this.lp2TextBox1.Text) * arrConvert3[this.lp2DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text = "Неверно указано значение давления";
                                            return;
                                        }

                                        try
                                        {
                                            p28 = customConverterToDouble(this.lp2TextBox2.Text) * arrConvert3[this.lp2DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                                        }
                                        catch (Exception)
                                        {
                                            LabelError.Text = "Неверно указано значение давления";
                                            return;
                                        }

                                        if (!(p26 > 0))
                                        {
                                            LabelError.Text = "Введите числовое значение больше нуля";
                                            return;
                                        }
                                        else if (!(p28 > 0))
                                        {
                                            LabelError.Text = "Введите числовое значение больше нуля";
                                            return;
                                        }

                                        if (!(p26 <= 25))
                                        {
                                            LabelError.Text = "На давление свыше 25 бар вариантов нет";
                                            return;
                                        }
                                        else if (!(p26 > p28))
                                        {
                                            LabelError.Text = "Неверно указано значение давления";
                                            return;
                                        }
                                        else
                                        {
                                            //this.calcrTextBox1.Text = p26.ToString();

                                            g_dict.Add("p26", p26);
                                            g_dict.Add("p28", p28);
                                        }
                                    }
                                }
                                else
                                {
                                    double p56, p58;

                                    try
                                    {
                                        p56 = customConverterToDouble(this.lp5TextBox1.Text) * arrConvert3[this.lp5DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p58 = customConverterToDouble(this.lp5TextBox2.Text) * arrConvert3[this.lp5DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    g_dict.Add("p56", p56);
                                    g_dict.Add("p58", p58);

                                    if (lp5RadioButtonList1.SelectedIndex == 1)
                                    {
                                        lp5TextBox3.Text = (Math.Round(100 * Math.Pow((customConverterToDouble(lp5TextBox1.Text) * arrConvert3[lp5DropDownList1.SelectedIndex - 1] / arrConvert3[2]) + 1, 0.25))).ToString();
                                    }

                                    if (customConverterToDouble(lp5TextBox3.Text) > MaxT2x)
                                    {
                                        CustomValidator20.ErrorMessage = "На температуру свыше 220°С вариантов нет";
                                        CustomValidator20.IsValid = false;
                                        return;
                                    }

                                    g_dict.Add("p61", customConverterToDouble(this.lp5TextBox3.Text));
                                }
                            }
                            else if (eorRadioButtonList1.SelectedIndex == 2)
                            {
                                if (this.checkTextBoxEmpty(this.lp3TextBox1))
                                {
                                    LabelError.Text = "Неверно указано значение давления";
                                    return;
                                }
                                else if (this.checkTextBoxEmpty(this.lp3TextBox2))
                                {
                                    LabelError.Text = "Неверно указано значение давления";
                                    return;
                                }
                                else
                                {

                                    double p30, p32;

                                    try
                                    {
                                        p30 = customConverterToDouble(this.lp3TextBox1.Text) * arrConvert3[this.lp3DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p32 = customConverterToDouble(this.lp3TextBox2.Text) * arrConvert3[this.lp3DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    if (!(p30 > 0))
                                    {
                                        LabelError.Text = "Введите числовое значение больше нуля";
                                        return;
                                    }
                                    else if (!(p32 > 0))
                                    {
                                        LabelError.Text = "Введите числовое значение больше нуля";
                                        return;
                                    }

                                    if (!(p30 <= 25))
                                    {
                                        LabelError.Text = "На давление свыше 25 бар вариантов нет";
                                        return;
                                    }
                                    else if (!(p30 > p32))
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }
                                    else
                                    {
                                        //this.calcrTextBox1.Text = p30.ToString();

                                        g_dict.Add("p30", p30);
                                        g_dict.Add("p32", p32);
                                    }
                                }
                            }
                            else if (eorRadioButtonList1.SelectedIndex == 3)
                            {
                                if (this.checkTextBoxEmpty(this.lp4TextBox2))
                                {
                                    LabelError.Text = "Неверно указано значение давления";
                                    return;
                                }
                                else
                                {

                                    double p19;

                                    try
                                    {
                                        p19 = customConverterToDouble(this.lp4TextBox2.Text) * arrConvert3[this.lp4DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    if (!(p19 > 0))
                                    {
                                        LabelError.Text = "Введите числовое значение больше нуля";
                                        return;
                                    }

                                    if (!(p19 <= 25))
                                    {
                                        LabelError.Text = "На давление свыше 25 бар вариантов нет";
                                        return;
                                    }
                                    else
                                    {
                                        g_dict.Add("p19", p19);
                                    }
                                }
                            }
                            if (ws1RadioButtonList1.SelectedIndex != 3)
                            {
                                try
                                {
                                    double ptemp = customConverterToDouble(this.calcrTextBox1.Text);
                                }
                                catch (Exception)
                                {
                                    LabelError.Text = "Неверно указано значение давления";
                                    return;
                                }

                                if (customConverterToDouble(this.calcrTextBox1.Text) <= 0)
                                {

                                    LabelError.Text = "Неверно указано значение давления";
                                    return;
                                }
                                else if ((customConverterToDouble(this.calcrTextBox1.Text) * arrConvert3[this.calcrDropDownList1.SelectedIndex - 1] / arrConvert3[2]) > 25)
                                {

                                    LabelError.Text = "На давление свыше 25 бар вариантов нет";
                                    return;
                                }



                                double p35 = 0;
                                try
                                {
                                    p35 = customConverterToDouble(this.calcrTextBox2.Text);
                                }
                                catch (Exception)
                                {
                                    LabelError.Text = "Неверно указано значение температуры";
                                    return;
                                }
                                if (p35 <= 0)
                                {

                                    LabelError.Text = "Неверно указано значение температуры";
                                    return;
                                }
                                if (eorRadioButtonList1.SelectedIndex != 1)
                                {
                                    if (p35 > 220)
                                    {
                                        LabelError.Text = "На температуру свыше 220°С вариантов нет";
                                        return;
                                    }
                                }
                                else
                                {
                                    if (p35 > 220)
                                    {
                                        LabelError.Text = "На температуру свыше 220°С вариантов нет";
                                        return;
                                    }
                                }

                                g_dict.Add("p35", p35);

                            }
                            else {
                                double p35 = 0;
                                p35 = customConverterToDouble(this.lp5TextBox3.Text);
                                g_dict.Add("p35", p35);
                                
                            }

                            if (ws1RadioButtonList1.SelectedIndex == 0)
                            {
                                this.ws1ResultLabel.Text = "Рабочая среда - вода";
                            }
                            else if (ws1RadioButtonList1.SelectedIndex == 1)
                            {
                                this.ws1ResultLabel.Text = "Рабочая среда - этиленгликоль " + g_dict["p6"] + "%, " + g_dict["p7"] + " °С";
                            }
                            else if (ws1RadioButtonList1.SelectedIndex == 2)
                            {
                                this.ws1ResultLabel.Text = "Рабочая среда - пропиленгликоль " + g_dict["p6"] + "%, " + g_dict["p7"] + " °С";
                            }
                            else
                            {
                                if (lp5RadioButtonList1.SelectedIndex == 0)
                                {
                                    this.ws1ResultLabel.Text = "Рабочая среда - Водяной пар перегретый";
                                }
                                else
                                {
                                    this.ws1ResultLabel.Text = "Рабочая среда - Водяной пар насыщенный";
                                }
                                    
                            }
                            bool flag25Bar = get25BarFlag();

                            if (eorRadioButtonList1.SelectedIndex > 1)
                            {
                                this.maxt1ResultLabel.Text = "Максимальная температура - 150 °С";
                                if (flag25Bar)
                                {
                                    this.maxp1ResultLabel.Text = "Максимальное рабочее давление - 25 бар";
                                } else
                                {
                                    this.maxp1ResultLabel.Text = "Максимальное рабочее давление - 16 бар";
                                }

                                labelOptyV.Text = "Оптимальная скорость в выходном сечении регулятора: 2 - 3 м / с для ИТП; 2 - 5 м / с для ЦТП.";
                            } 
                            else
                            {
                                if (ws1RadioButtonList1.SelectedIndex != 3)
                                {
                                    if (this.ws1RadioButtonList1.SelectedIndex == 0)
                                    {
                                        if (g_dict["p35"] <= MaxT3x && customConverterToDouble(this.fprTextBox2.Text) <= MaxT3x)
                                        {
                                            this.maxt1ResultLabel.Text = "Максимальная температура - 150 °С";
                                        }
                                        else
                                        {
                                            this.maxt1ResultLabel.Text = "Максимальная температура - 220 °С";
                                        }
                                    }
                                    else
                                    {
                                        this.maxt1ResultLabel.Text = "Максимальная температура - 150 °С";
                                    }
                                }
                                else
                                {

                                    this.maxt1ResultLabel.Text = "Максимальная температура - 220 °С";

                                }

                                if (flag25Bar)
                                {
                                    this.maxp1ResultLabel.Text = "Максимальное рабочее давление - 25 бар";
                                }
                                else
                                {
                                    this.maxp1ResultLabel.Text = "Максимальное рабочее давление - 16 бар";
                                }

                                if (ws1RadioButtonList1.SelectedIndex != 3)
                                {
                                    labelOptyV.Text = "Оптимальная скорость в выходном сечении регулятора: 2 - 3 м / с для ИТП; 2 - 5 м / с для ЦТП.";
                                }
                                else
                                {
                                    labelOptyV.Text = "Оптимальная скорость в выходном сечении регулятора: 40 м/с для насыщенного пара; 60 м/с для перегретого пара.";
                                } 
                            }

                            if (ws1RadioButtonList1.SelectedIndex != 3)
                            {
                                double t1_check = customConverterToDouble(this.calcrTextBox2.Text);
                                Newtonsoft.Json.Linq.JObject max_check = dataFromFile.table9[dataFromFile.table9.Count - 1];
                                foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9)
                                {
                                    if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max_check.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1_check))
                                    {
                                        max_check = ob;
                                    }
                                }
                                //double ps_check = Convert.ToDouble(max_check.GetValue("ps"));

                                if (((customConverterToDouble(this.calcrTextBox1.Text) * arrConvert3[this.calcrDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - getPSbyT(t1_check)) <= 0)
                                {
                                    LabelError.Text = "Указанная температура выше температуры парообразования. При указанной температуре в трубопроводе движется пар";
                                    return;
                                }
                            }

                            mapInputParametersR(ref r_input_dict);

                            Dictionary<string, string[]> gtr = this.generatedTableR(g_dict);

                            /*GridView1.Columns.Clear();
                            GridView1.Rows.Clear();
                            GridView1.Refresh();*/
                            string[] titles;
                            if (ws1RadioButtonList1.SelectedIndex != 3)
                            {
                                titles = new string[] {
                                    "Марка регулятора давления",
                                    "Номинальный диаметр DN, мм",
                                    "Пропускная способность Kvs,\nм³/ч",
                                    "Фактические потери давления на полностью открытом клапане при заданном расходе ∆Рф,\nбар\n",
                                    "Диапазон настройки,\nбар",
                                    "Скорость в выходном сечении регулятора V,\nм/с",
                                    "Шум, некачественное регулирование",
                                    "Предельно допустимый перепад давлений на регуляторе ∆Pпред,\nбар",
                                    "Кавитация"
                                };
                            }
                            else
                            {
                                titles = new string[] {
                                    "Марка регулятора давления",
                                    "Номинальный диаметр DN,\nмм",
                                    "Пропускная способность Kvs,\nм³/ч",
                                    "Диапазон настройки,\nбар",
                                    "Скорость в выходном сечении регулятора V,\nм/с",
                                    "Шум, некачественное регулирование"
                                };
                            }

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
                                GridView1.DataSource = dt;
                                GridView1.DataBind();

                                for (int j = 0; j < gtr.Count(); j++)
                                {
                                    int index = -1;
                                    switch (gtr.ElementAt(j).Key)
                                    {
                                        case "A":
                                            index = 0;
                                            break;
                                        case "B":
                                            index = 2;
                                            break;
                                        case "C":
                                            index = 1;
                                            break;
                                        case "D":
                                            index = 3;
                                            break;
                                        case "I":
                                            if (ws1RadioButtonList1.SelectedIndex != 3) { index = 4; } else { index = 3; }
                                            break;
                                        case "F":
                                            if (ws1RadioButtonList1.SelectedIndex != 3) { index = 5; } else { index = 4; }
                                            break;
                                        case "E":
                                            if (ws1RadioButtonList1.SelectedIndex != 3) { index = 6; } else { index = 5; }
                                            break;
                                        case "G":
                                            index = 7;
                                            break;
                                        case "K":
                                            index = 8;
                                            break;
                                    }

                                    if (gtr.ElementAt(j).Value.Count() > i)
                                    {
                                        string tmp = gtr.ElementAt(j).Value[i];
                                        if (String.IsNullOrWhiteSpace(tmp))
                                        {
                                            if (GridView1.Rows.Count > 1)
                                            {
                                                dt.Rows[GridView1.Rows.Count - 1][index] = dt.Rows[GridView1.Rows.Count - 2][index];

                                            }
                                        }
                                        else
                                        {
                                            dt.Rows[GridView1.Rows.Count - 1][index] = tmp;
                                        }
                                    }
                                    else
                                    {
                                        if (GridView1.Rows.Count > 1)
                                        {
                                            dt.Rows[GridView1.Rows.Count - 1][index] = dt.Rows[GridView1.Rows.Count - 2][index];
                                        }
                                    }
                                }
                                GridView1.DataSource = dt;
                                GridView1.DataBind();
                            }

                        }
                    }
                    else
                    {
                        fprLabelError.Text = "Не выбран расход через регулятор давления";
                        return;
                    }
                }
                else
                {
                    LabelError.Text = "Не выбрана рабочая среда";
                    return;
                }
            }
            else
            {
                LabelError.Text = "Не выбрано исполнение регулятора";
                return;
            }
           
            LabelError.Text = "";
            Label52.Visible = true;
            maxp1ResultLabel.Visible = true;
            maxt1ResultLabel.Visible = true;
            ws1ResultLabel.Visible = true;
            labelOptyV.Visible = true;
            GridView1.Enabled = true;
            GridView1.Visible = true;
            GridView1.Height = 250;
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

    protected void lp1DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (SetEnableTextBox(lp1DropDownList1, lp1TextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp1TextBox1);
        }
        SavePrevSelectedIndexDDL(lp1DropDownList1.ID, lp1DropDownList1.SelectedIndex);
    }

    protected void lp1DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (SetEnableTextBox(lp1DropDownList2, lp1TextBox2))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp1TextBox2);
        }
        SavePrevSelectedIndexDDL(lp1DropDownList2.ID, lp1DropDownList2.SelectedIndex);
    }

    protected void lp1DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp1DropDownList3, lp1TextBox3))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp1TextBox3);
        }
        SavePrevSelectedIndexDDL(lp1DropDownList3.ID, lp1DropDownList3.SelectedIndex);
    }

    protected void lp1DropDownList4_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp1DropDownList4, lp1TextBox4))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp1TextBox4);
        }
        SavePrevSelectedIndexDDL(lp1DropDownList4.ID, lp1DropDownList4.SelectedIndex);
    }

    protected void lp2DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp2DropDownList1, lp2TextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp2TextBox1);
        }
        SavePrevSelectedIndexDDL(lp2DropDownList1.ID, lp2DropDownList1.SelectedIndex);
    }

    protected void lp2DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp2DropDownList2, lp2TextBox2))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp2TextBox2);
        }
        SavePrevSelectedIndexDDL(lp2DropDownList2.ID, lp2DropDownList2.SelectedIndex);
    }

    protected void lp3DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp3DropDownList1, lp3TextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp3TextBox1);
        }
        SavePrevSelectedIndexDDL(lp3DropDownList1.ID, lp3DropDownList1.SelectedIndex);
    }

    protected void lp3DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp3DropDownList2, lp3TextBox2))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp3TextBox2);
        }
        SavePrevSelectedIndexDDL(lp3DropDownList2.ID, lp3DropDownList2.SelectedIndex);
    }

    protected void lp4DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp4DropDownList2, lp4TextBox2))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp4TextBox2);
        }
        SavePrevSelectedIndexDDL(lp4DropDownList2.ID, lp4DropDownList2.SelectedIndex);
    }

    protected void lp5DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp5DropDownList1, lp5TextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp5TextBox1);
        }
        SavePrevSelectedIndexDDL(lp5DropDownList1.ID, lp5DropDownList1.SelectedIndex);
    }

    protected void lp5DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(lp5DropDownList2, lp5TextBox2))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref lp5TextBox2);
        }
        SavePrevSelectedIndexDDL(lp5DropDownList2.ID, lp5DropDownList2.SelectedIndex);
    }

    protected void calcrDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(calcrDropDownList1, calcrTextBox1))
        {
            convertArr(arrConvert3, (sender as DropDownList), ref calcrTextBox1);
        }
        SavePrevSelectedIndexDDL(calcrDropDownList1.ID, calcrDropDownList1.SelectedIndex);
    }

    protected void fprDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(fprDropDownList1, fprTextBox1))
        {
            convertArrDouble(arrConvert1, (sender as DropDownList), ref fprTextBox1);
        }
        SavePrevSelectedIndexDDL(fprDropDownList1.ID, fprDropDownList1.SelectedIndex);
    }

    protected void fprDropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SetEnableTextBox(fprDropDownList2, fprTextBox4))
        {
            convertArr(arrConvert2, (sender as DropDownList), ref fprTextBox4);
        }
        SavePrevSelectedIndexDDL(fprDropDownList2.ID, fprDropDownList2.SelectedIndex);
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
       
        if(eorRadioButtonList1.SelectedIndex == 1) //после себя
        {
            if (lp5TextBox1.Enabled)
            {
                GenerateSteamExel();
            }
            else
            {
                GeneratePosleExel();
            }
        }
        else if (eorRadioButtonList1.SelectedIndex == 2) //Регулятор давления "до себя"
        {
            GenerateDoExel();
        }
        else if (eorRadioButtonList1.SelectedIndex == 3) //Регулятор "перепуска" 
        {
            GeneratePerepyskaExel();
        }
        else
        {
            GenerateOtherExel(); //Регулятор перепада давления 
        }
    }

    public void GenerateSteamExel()
    {
        try
        {
            r_input_dict = (Dictionary<int, string>)Session["r_input_dict"];

            this.readFile(0);

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            r_input_dict[20] = this.calcDNLabelVal.Text;
            r_input_dict[21] = this.calcCapacityLabelVal.Text;

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = ConvertCommaToPoint(r_input_dict[41]);
            if (fileName == "&nbsp;")
            {
                fileName = "Регуляторов не найдено";
            }

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDTSteam.xlsx")))
            {
                LabelError.Text = "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDTSteam.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            for (int i = 1; i < 61; i++)
            {
                if (i != 50)
                {
                    if (i == 6 || i == 38 || i == 42 || i == 43 || i == 46 || i == 55 || i == 57 || i == 60)
                    {
                        r_input_dict[i] = ConvertPointToComma(r_input_dict[i]);

                    }

                    if (r_input_dict[i] == "&nbsp;")
                    {
                        r_input_dict[i] = "-";
                    }
                }
            }

            ws.Cells["J2"].Value = r_input_dict[1];
            ws.Cells["C3"].Value = r_input_dict[2];
            ws.Cells["C4"].Value = r_input_dict[4];
            ws.Cells["C5"].Value = r_input_dict[5];
            ws.Cells["C8"].Value = r_input_dict[6];


            ws.Cells["I10"].Value = r_input_dict[55];
            ws.Cells["K10"].Value = r_input_dict[56];

            ws.Cells["I11"].Value = r_input_dict[57];
            ws.Cells["K11"].Value = r_input_dict[58];

            ws.Cells["I12"].Value = r_input_dict[60];

            ws.Cells["I14"].Value = r_input_dict[38];
            ws.Cells["K14"].Value = r_input_dict[381];

            ws.Cells["E17"].Value = r_input_dict[39];
            ws.Cells["J17"].Value = r_input_dict[20];
            ws.Cells["E18"].Value = r_input_dict[40];
            ws.Cells["J18"].Value = r_input_dict[21];

            ws.Cells["A21"].Value = r_input_dict[41];
            ws.Cells["C21"].Value = r_input_dict[42];
            ws.Cells["E21"].Value = r_input_dict[43];
            ws.Cells["G21"].Value = r_input_dict[44];
            ws.Cells["H21"].Value = r_input_dict[45];
            ws.Cells["J21"].Value = r_input_dict[46];
        


            getDimsR(ref r_input_dict);
            
            ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT-S и RDT-B.jpg"), "A25");

            ws.Cells["F25"].Value = r_input_dict[51];
            ws.Cells["F26"].Value = r_input_dict[52];
            ws.Cells["F27"].Value = r_input_dict[53];
            ws.Cells["F28"].Value = r_input_dict[54];


            string path = HttpContext.Current.Server.MapPath("~/Files/RDT/PDF/" + DateTime.Now.ToString("dd-MM-yyyy"));
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }



            int j = 1;
            string tempName = fileName;

            link1:

            if (File.Exists(path + "/" + tempName + ".pdf"))
            {
                tempName = fileName + "_" + j;
                j++;
                goto link1;
            }

            fileName = tempName;
            string filePath = path + "/" + fileName + ".pdf";

            ef.Save(filePath);

            WaitDownload(50);

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.TransmitFile(file.FullName);
                Response.Flush();
                //Response.Close();
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    public void GeneratePosleExel()
    {

        try
        {
            r_input_dict = (Dictionary<int, string>)Session["r_input_dict"];

            this.readFile(0);

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            r_input_dict[20] = this.calcDNLabelVal.Text;
            r_input_dict[21] = this.calcCapacityLabelVal.Text;

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = ConvertCommaToPoint(r_input_dict[41]);
            if (fileName == "&nbsp;")
            {
                fileName = "Регуляторов не найдено";
            }

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-P,RDT-PH.xlsx")))
            {
                LabelError.Text = "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-P,RDT-PH.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            for (int i = 1; i < 50; i++)
            {

                if (i == 6 || i == 7 || i == 9 || i == 11 || i == 13 || i == 16 || i == 18 || i == 20 || i == 22 || i == 23 || i == 25 || i == 27 || i == 29 || i == 31 || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38 || i == 42 || i == 43 || i == 44 || i == 46 || i == 48)
                {
                    r_input_dict[i] = ConvertPointToComma(r_input_dict[i]);

                }

                if (r_input_dict[i] == "&nbsp;")
                {
                    r_input_dict[i] = "-";
                }

            }

            ws.Cells["J2"].Value = r_input_dict[1];
            ws.Cells["C3"].Value = r_input_dict[2];
            ws.Cells["C4"].Value = r_input_dict[4];
            ws.Cells["C5"].Value = r_input_dict[5];
            ws.Cells["C8"].Value = r_input_dict[6];

            ws.Cells["I10"].Value = r_input_dict[16];
            ws.Cells["K10"].Value = r_input_dict[17];
            ws.Cells["I11"].Value = r_input_dict[18];
            ws.Cells["K11"].Value = r_input_dict[19];

            ws.Cells["I13"].Value = r_input_dict[31];
            ws.Cells["K13"].Value = r_input_dict[32];
            ws.Cells["I14"].Value = r_input_dict[33];

            ws.Cells["I16"].Value = r_input_dict[34];
            ws.Cells["I17"].Value = r_input_dict[35];
            ws.Cells["I18"].Value = r_input_dict[36];
            ws.Cells["K18"].Value = r_input_dict[37];
            ws.Cells["I19"].Value = r_input_dict[38];
            ws.Cells["K19"].Value = r_input_dict[381];

            ws.Cells["E22"].Value = r_input_dict[39];
            ws.Cells["J22"].Value = r_input_dict[20];
            ws.Cells["E23"].Value = r_input_dict[40];
            ws.Cells["J23"].Value = r_input_dict[21];

            ws.Cells["A26"].Value = r_input_dict[41];
            ws.Cells["B26"].Value = r_input_dict[42];
            ws.Cells["C26"].Value = r_input_dict[43];
            ws.Cells["D26"].Value = r_input_dict[44];
            ws.Cells["F26"].Value = r_input_dict[45];
            ws.Cells["G26"].Value = r_input_dict[46];
            ws.Cells["H26"].Value = r_input_dict[47];
            ws.Cells["I26"].Value = r_input_dict[48];
            ws.Cells["K26"].Value = r_input_dict[49];

            getDimsR(ref r_input_dict);

            if ((r_input_dict[4] == this.eorRadioButtonList1.Items[0].Text) || (r_input_dict[4] == eorRadioButtonList1.Items[1].Text))
            {
                if (r_input_dict[4] == eorRadioButtonList1.Items[1].Text && (customConverterToDouble(calcrTextBox2.Text) > MaxT3x || customConverterToDouble(this.fprTextBox2.Text) > MaxT3x))
                {
                    ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT-H, RDT-PH.jpg"), "A30");
                }
                else
                {
                    ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT и RDT-P.jpg"), "A30");
                }
            }
            else
            {
                ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT-S и RDT-B.jpg"), "A30");
            }

            ws.Cells["F30"].Value = r_input_dict[51];
            ws.Cells["F31"].Value = r_input_dict[52];
            ws.Cells["F32"].Value = r_input_dict[53];
            ws.Cells["F33"].Value = r_input_dict[54];


            string path = HttpContext.Current.Server.MapPath("~/Files/RDT/PDF/" + DateTime.Now.ToString("dd-MM-yyyy"));
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            int j = 1;
            string tempName = fileName;

            link1:

            if (File.Exists(path + "/" + tempName + ".pdf"))
            {
                tempName = fileName + "_" + j;
                j++;
                goto link1;
            }

            fileName = tempName;
            string filePath = path + "/" + fileName + ".pdf";

            ef.Save(filePath);

            WaitDownload(50);

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.TransmitFile(file.FullName);
                Response.Flush();
                //Response.Close();
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    public void GenerateDoExel()
    {
        try
        {
            r_input_dict = (Dictionary<int, string>)Session["r_input_dict"];

            this.readFile(0);

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            r_input_dict[20] = this.calcDNLabelVal.Text;
            r_input_dict[21] = this.calcCapacityLabelVal.Text;

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = ConvertCommaToPoint(r_input_dict[41]);
            if (fileName == "&nbsp;")
            {
                fileName = "Регуляторов не найдено";
            }

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-S.xlsx")))
            {
                LabelError.Text = "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-S.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            for (int i = 1; i < 50; i++)
            {

                if (i == 6 || i == 7 || i == 9 || i == 11 || i == 13 || i == 16 || i == 18 || i == 20 || i == 22 || i == 23 || i == 25 || i == 27 || i == 29 || i == 31 || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38 || i == 42 || i == 43 || i == 44 || i == 46 || i == 48)
                {
                    r_input_dict[i] = ConvertPointToComma(r_input_dict[i]);

                }

                if (r_input_dict[i] == "&nbsp;")
                {
                    r_input_dict[i] = "-";
                }

            }

            ws.Cells["J2"].Value = r_input_dict[1];
            ws.Cells["C3"].Value = r_input_dict[2];
            ws.Cells["C4"].Value = r_input_dict[4];
            ws.Cells["C5"].Value = r_input_dict[5];
            ws.Cells["C8"].Value = r_input_dict[6];

            ws.Cells["I10"].Value = r_input_dict[25];
            ws.Cells["K10"].Value = r_input_dict[26];
            ws.Cells["I11"].Value = r_input_dict[27];
            ws.Cells["K11"].Value = r_input_dict[28];

            ws.Cells["I13"].Value = r_input_dict[31];
            ws.Cells["K13"].Value = r_input_dict[32];
            ws.Cells["I14"].Value = r_input_dict[33];

            ws.Cells["I16"].Value = r_input_dict[34];
            ws.Cells["I17"].Value = r_input_dict[35];
            ws.Cells["I18"].Value = r_input_dict[36];
            ws.Cells["K18"].Value = r_input_dict[37];
            ws.Cells["I19"].Value = r_input_dict[38];
            ws.Cells["K19"].Value = r_input_dict[381];

            ws.Cells["E22"].Value = r_input_dict[39];
            ws.Cells["J22"].Value = r_input_dict[20];
            ws.Cells["E23"].Value = r_input_dict[40];
            ws.Cells["J23"].Value = r_input_dict[21];

            ws.Cells["A26"].Value = r_input_dict[41];
            ws.Cells["B26"].Value = r_input_dict[42];
            ws.Cells["C26"].Value = r_input_dict[43];
            ws.Cells["D26"].Value = r_input_dict[44];
            ws.Cells["F26"].Value = r_input_dict[45];
            ws.Cells["G26"].Value = r_input_dict[46];
            ws.Cells["H26"].Value = r_input_dict[47];
            ws.Cells["I26"].Value = r_input_dict[48];
            ws.Cells["K26"].Value = r_input_dict[49];

            getDimsR(ref r_input_dict);

            ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT-S и RDT-B.jpg"), "A30");

            ws.Cells["F30"].Value = r_input_dict[51];
            ws.Cells["F31"].Value = r_input_dict[52];
            ws.Cells["F32"].Value = r_input_dict[53];
            ws.Cells["F33"].Value = r_input_dict[54];


            string path = HttpContext.Current.Server.MapPath("~/Files/RDT/PDF/" + DateTime.Now.ToString("dd-MM-yyyy"));
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            int j = 1;
            string tempName = fileName;

            link1:

            if (File.Exists(path + "/" + tempName + ".pdf"))
            {
                tempName = fileName + "_" + j;
                j++;
                goto link1;
            }

            fileName = tempName;
            string filePath = path + "/" + fileName + ".pdf";

            ef.Save(filePath);

            WaitDownload(50);

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.TransmitFile(file.FullName);
                Response.Flush();
                //Response.Close();
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    public void GeneratePerepyskaExel()
    {
        try
        {
            r_input_dict = (Dictionary<int, string>)Session["r_input_dict"];

            this.readFile(0);

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            r_input_dict[20] = this.calcDNLabelVal.Text;
            r_input_dict[21] = this.calcCapacityLabelVal.Text;

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = ConvertCommaToPoint(r_input_dict[41]);
            if (fileName == "&nbsp;")
            {
                fileName = "Регуляторов не найдено";
            }

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-B.xlsx")))
            {
                LabelError.Text = "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-B.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            for (int i = 1; i < 50; i++)
            {

                if (i == 6 || i == 7 || i == 9 || i == 11 || i == 13 || i == 16 || i == 18 || i == 20 || i == 22 || i == 23 || i == 25 || i == 27 || i == 29 || i == 31 || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38 || i == 42 || i == 43 || i == 44 || i == 46 || i == 48)
                {
                    r_input_dict[i] = ConvertPointToComma(r_input_dict[i]);

                }

                if (r_input_dict[i] == "&nbsp;")
                {
                    r_input_dict[i] = "-";
                }

            }

            ws.Cells["J2"].Value = r_input_dict[1];
            ws.Cells["C3"].Value = r_input_dict[2];
            ws.Cells["C4"].Value = r_input_dict[4];
            ws.Cells["C5"].Value = r_input_dict[5];
            ws.Cells["C8"].Value = r_input_dict[6];

            ws.Cells["I10"].Value = r_input_dict[29];
            ws.Cells["K10"].Value = r_input_dict[30];

            ws.Cells["I12"].Value = r_input_dict[31];
            ws.Cells["K12"].Value = r_input_dict[32];
            ws.Cells["I13"].Value = r_input_dict[33];

            ws.Cells["I15"].Value = r_input_dict[34];
            ws.Cells["I16"].Value = r_input_dict[35];
            ws.Cells["I17"].Value = r_input_dict[36];
            ws.Cells["K17"].Value = r_input_dict[37];
            ws.Cells["I18"].Value = r_input_dict[38];
            ws.Cells["K18"].Value = r_input_dict[381];

            ws.Cells["E21"].Value = r_input_dict[39];
            ws.Cells["J21"].Value = r_input_dict[20];
            ws.Cells["E22"].Value = r_input_dict[40];
            ws.Cells["J22"].Value = r_input_dict[21];

            ws.Cells["A25"].Value = r_input_dict[41];
            ws.Cells["B25"].Value = r_input_dict[42];
            ws.Cells["C25"].Value = r_input_dict[43];
            ws.Cells["D25"].Value = r_input_dict[44];
            ws.Cells["F25"].Value = r_input_dict[45];
            ws.Cells["G25"].Value = r_input_dict[46];
            ws.Cells["H25"].Value = r_input_dict[47];
            ws.Cells["I25"].Value = r_input_dict[48];
            ws.Cells["K25"].Value = r_input_dict[49];

            getDimsR(ref r_input_dict);
           
            ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT-S и RDT-B.jpg"), "A29");
            
            ws.Cells["F29"].Value = r_input_dict[51];
            ws.Cells["F30"].Value = r_input_dict[52];
            ws.Cells["F31"].Value = r_input_dict[53];
            ws.Cells["F32"].Value = r_input_dict[54];


            string path = HttpContext.Current.Server.MapPath("~/Files/RDT/PDF/" + DateTime.Now.ToString("dd-MM-yyyy"));
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            int j = 1;
            string tempName = fileName;

            link1:

            if (File.Exists(path + "/" + tempName + ".pdf"))
            {
                tempName = fileName + "_" + j;
                j++;
                goto link1;
            }

            fileName = tempName;
            string filePath = path + "/" + fileName + ".pdf";

            ef.Save(filePath);

            WaitDownload(50);

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.TransmitFile(file.FullName);
                Response.Flush();
                //Response.Close();
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    public void GenerateOtherExel()
    {
        try
        {
            r_input_dict = (Dictionary<int, string>)Session["r_input_dict"];

            this.readFile(0);

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            r_input_dict[20] = this.calcDNLabelVal.Text;
            r_input_dict[21] = this.calcCapacityLabelVal.Text;

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = ConvertCommaToPoint(r_input_dict[41]);
            if (fileName == "&nbsp;")
            {
                fileName = "Регуляторов не найдено";
            }

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT,RDT-H.xlsx")))
            {
                LabelError.Text = "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT,RDT-H.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            for (int i = 1; i < 50; i++)
            {

                if (i == 6 || i == 7 || i == 9 || i == 11 || i == 13 || i == 16 || i == 18 || i == 20 || i == 22 || i == 23 || i == 25 || i == 27 || i == 29 || i == 31 || i == 33 || i == 34 || i == 35 || i == 36 || i == 37 || i == 38 || i == 42 || i == 43 || i == 44 || i == 46 || i == 48)
                {
                    r_input_dict[i] = ConvertPointToComma(r_input_dict[i]);

                }

                if (r_input_dict[i] == "&nbsp;")
                {
                    r_input_dict[i] = "-";
                }

            }

            ws.Cells["J2"].Value = r_input_dict[1];
            ws.Cells["C3"].Value = r_input_dict[2];
            ws.Cells["C4"].Value = r_input_dict[4];
            ws.Cells["C5"].Value = r_input_dict[5];
            ws.Cells["C8"].Value = r_input_dict[6];
   
            
            //ws.Cells["I10"].Value = r_input_dict[7];
            //ws.Cells["K10"].Value = r_input_dict[8];
            ws.Cells["I11"].Value = r_input_dict[9];
            ws.Cells["K11"].Value = r_input_dict[10];
            ws.Cells["I12"].Value = r_input_dict[11];
            ws.Cells["K12"].Value = r_input_dict[12];
            ws.Cells["I13"].Value = r_input_dict[13];
            ws.Cells["K13"].Value = r_input_dict[14];
            ws.Cells["I14"].Value = r_input_dict[15];

            ws.Cells["I16"].Value = r_input_dict[31];
            ws.Cells["K16"].Value = r_input_dict[32];
            ws.Cells["I17"].Value = r_input_dict[33];

            ws.Cells["I19"].Value = r_input_dict[34];
            ws.Cells["I20"].Value = r_input_dict[35];
            ws.Cells["I21"].Value = r_input_dict[36];
            ws.Cells["K21"].Value = r_input_dict[37];

            ws.Cells["I22"].Value = r_input_dict[38];
            ws.Cells["K22"].Value = r_input_dict[381];

            ws.Cells["E25"].Value = r_input_dict[39];
            ws.Cells["J25"].Value = r_input_dict[20];
            ws.Cells["E26"].Value = r_input_dict[40];
            ws.Cells["J26"].Value = r_input_dict[21];

            ws.Cells["A29"].Value = r_input_dict[41];
            ws.Cells["B29"].Value = r_input_dict[42];
            ws.Cells["C29"].Value = r_input_dict[43];
            ws.Cells["D29"].Value = r_input_dict[44];
            ws.Cells["F29"].Value = r_input_dict[45];
            ws.Cells["G29"].Value = r_input_dict[46];
            ws.Cells["H29"].Value = r_input_dict[47];
            ws.Cells["I29"].Value = r_input_dict[48];
            ws.Cells["K29"].Value = r_input_dict[49];

            getDimsR(ref r_input_dict);


            if ((r_input_dict[4] == this.eorRadioButtonList1.Items[1].Text))
            {
                if (r_input_dict[4] == eorRadioButtonList1.Items[1].Text && customConverterToDouble(calcrTextBox2.Text) > 150 && ws1RadioButtonList1.SelectedIndex == 0)
                {
                    ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT-S и RDT-B.jpg"), "A33");
                }
                else
                {
                    ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT и RDT-P.jpg"), "A33");
                }
            }
            else if (r_input_dict[4] == this.eorRadioButtonList1.Items[0].Text)
            {
                if ((customConverterToDouble(calcrTextBox2.Text) > 150 || customConverterToDouble(this.fprTextBox2.Text) > 150) && ws1RadioButtonList1.SelectedIndex == 0)
                {
                    ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT-H, RDT-PH.jpg"), "A33");
                }
                else
                {
                    ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT и RDT-P.jpg"), "A33");
                }
            }
            else
            {
                ws.Pictures.Add(HttpContext.Current.Server.MapPath("~/Content/images/rdt/Габаритный RDT-S и RDT-B.jpg"), "A33");
            }

            ws.Cells["F33"].Value = r_input_dict[51];
            ws.Cells["F34"].Value = r_input_dict[52];
            ws.Cells["F35"].Value = r_input_dict[53];
            ws.Cells["F36"].Value = r_input_dict[54];


            string path = HttpContext.Current.Server.MapPath("~/Files/RDT/PDF/" + DateTime.Now.ToString("dd-MM-yyyy"));
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            int j = 1;
            string tempName = fileName;

            link1:

            if (File.Exists(path + "/" + tempName + ".pdf"))
            {
                tempName = fileName + "_" + j;
                j++;
                goto link1;
            }

            fileName = tempName;
            string filePath = path + "/" + fileName + ".pdf";

            ef.Save(filePath);

            WaitDownload(50);

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.TransmitFile(file.FullName);
                Response.Flush();
                //Response.Close();
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    public void switchPane(HtmlGenericControl paneId)
    {
        lpPane1.Visible = false;
        lpPane2.Visible = false;
        lpPane3.Visible = false;
        lpPane4.Visible = false;

        Label20.Visible = true;
        paneId.Visible = true;
    }

    protected void eorRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        changeImage(eorRadioButtonList1.SelectedIndex);
        LabelSteam.Text = "Y";
        switch (eorRadioButtonList1.SelectedIndex)
        {
            case 0:
                lp1ControlEnable(true);
                lp2ControlEnable(false);
                lp3ControlEnable(false);
                lp4ControlEnable(false);
                lp5ControlEnable(false);

                RemoveCssClass(lp1, "panel-hide");
                AddCssClass(lp2, "panel-hide");
                AddCssClass(lp3, "panel-hide");
                AddCssClass(lp4, "panel-hide");
                AddCssClass(lp5, "panel-hide");

                ws1RadioButtonList1.Items[3].Enabled = false;
                ws1RadioButtonList1.SelectedIndex = -1;

                switchPane(lpPane1);

                break;
            case 1:
                lp1ControlEnable(false);
                lp2ControlEnable(false);
                lp3ControlEnable(false);
                lp4ControlEnable(false);
                lp5ControlEnable(false);

                AddCssClass(lp1, "panel-hide");
                AddCssClass(lp2, "panel-hide");
                AddCssClass(lp3, "panel-hide");
                AddCssClass(lp4, "panel-hide");
                AddCssClass(lp5, "panel-hide");

                if (eorRadioButtonList1.SelectedIndex == 1)
                {
                    ws1RadioButtonList1.Items[3].Enabled = true;
                }
                ws1RadioButtonList1.SelectedIndex = -1;

                switchPane(lpPane2);
                break;
            case 2:
                lp1ControlEnable(false);
                lp2ControlEnable(false);
                lp3ControlEnable(true);
                lp4ControlEnable(false);
                lp5ControlEnable(false);

                AddCssClass(lp1, "panel-hide");
                AddCssClass(lp2, "panel-hide");
                RemoveCssClass(lp3, "panel-hide");
                AddCssClass(lp4, "panel-hide");
                AddCssClass(lp5, "panel-hide");

                ws1RadioButtonList1.Items[3].Enabled = false;
                ws1RadioButtonList1.SelectedIndex = -1;
                switchPane(lpPane3);

                break;
            case 3:
                lp1ControlEnable(false);
                lp2ControlEnable(false);
                lp3ControlEnable(false);
                lp4ControlEnable(true);
                lp5ControlEnable(false);

                AddCssClass(lp1, "panel-hide");
                AddCssClass(lp2, "panel-hide");
                AddCssClass(lp3, "panel-hide");
                RemoveCssClass(lp4, "panel-hide");
                AddCssClass(lp5, "panel-hide");

                ws1RadioButtonList1.Items[3].Enabled = false;
                ws1RadioButtonList1.SelectedIndex = -1;
                switchPane(lpPane4);

                break;

            case 4:

                lp1ControlEnable(false);
                lp2ControlEnable(false);
                lp3ControlEnable(false);
                lp4ControlEnable(false);
                lp5ControlEnable(true);

                AddCssClass(lp1, "panel-hide");
                AddCssClass(lp2, "panel-hide");
                AddCssClass(lp3, "panel-hide");
                AddCssClass(lp4, "panel-hide");
                RemoveCssClass(lp5, "panel-hide");

                ws1RadioButtonList1.Items[3].Enabled = false;
                ws1RadioButtonList1.SelectedIndex = -1;
                switchPane(lpPane2);
                break;
        }

        LabelSteam.Text = "N";
        fprRadioButton1.Enabled = true;
        fprRadioButton2.Enabled = true;
        RemoveCssClass(fpr1, "panel-hide");
        RemoveCssClass(fpr2, "panel-hide");
        DisableTextBox(ws1TextBox1);
        DisableTextBox(ws1TextBox2);
        fpr1.Visible = false;
        fpr2.Visible = false;
        calcr.Visible = false;
    }

    protected void fprRadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        if (fprRadioButton1.Checked)
        {
            fprRadioButton2.Checked = false;
            textBoxEnabled(fprTextBox1, false);
            textBoxEnabled(fprTextBox2, false);
            textBoxEnabled(fprTextBox3, false);
            textBoxEnabled(fprTextBox4, false);
            textBoxEnabled(fprTextBox5, false);
            dropDownListEnable(fprDropDownList1, true);
            dropDownListEnable(fprDropDownList2, false);
            RemoveCssClass(fpr1_1, "panel-hide");
            AddCssClass(fpr2_1, "panel-hide");
        }
    }

    protected void fprRadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        if (fprRadioButton2.Checked)
        {
            fprRadioButton1.Checked = false;
            textBoxEnabled(fprTextBox1, false);
            textBoxEnabled(fprTextBox2, true);
            textBoxEnabled(fprTextBox3, true);
            textBoxEnabled(fprTextBox4, false);
            textBoxEnabled(fprTextBox5, false);
            dropDownListEnable(fprDropDownList2, true);
            dropDownListEnable(fprDropDownList1, false);
            AddCssClass(fpr1_1, "panel-hide");
            RemoveCssClass(fpr2_1, "panel-hide");
        }
    }

    protected void ws1RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (ws1RadioButtonList1.SelectedIndex == 1 || ws1RadioButtonList1.SelectedIndex == 2)
        {
            ws1TextBox1.Enabled = true;
            ws1TextBox2.Enabled = true;
            AddCssClass(lp5, "panel-hide");
            lp5ControlEnable(false);

            if(eorRadioButtonList1.SelectedIndex == 1)
            {
                lp1ControlEnable(false);
                lp2ControlEnable(true);
                lp3ControlEnable(false);
                lp4ControlEnable(false);
                lp5ControlEnable(false);

                AddCssClass(lp1, "panel-hide");
                RemoveCssClass(lp2, "panel-hide");
                AddCssClass(lp3, "panel-hide");
                AddCssClass(lp4, "panel-hide");
                AddCssClass(lp5, "panel-hide");
            }
        }
        else
        {
            ws1TextBox1.Enabled = false;
            ws1TextBox2.Enabled = false;
            ws1TextBox1.Text = "";
            ws1TextBox2.Text = "";
        }

        calcrDropDownList1.Enabled = true;
        calcrTextBox2.Enabled = true;
        RemoveCssClass(calcr, "panel-hide");
        

        if (eorRadioButtonList1.SelectedIndex == 1 && ws1RadioButtonList1.SelectedIndex == 0)
        {
            lp1ControlEnable(false);
            lp2ControlEnable(true);
            lp3ControlEnable(false);
            lp4ControlEnable(false);
            lp5ControlEnable(false);

            AddCssClass(lp1, "panel-hide");
            RemoveCssClass(lp2, "panel-hide");
            AddCssClass(lp3, "panel-hide");
            AddCssClass(lp4, "panel-hide");
            AddCssClass(lp5, "panel-hide");
        }
        else if (eorRadioButtonList1.SelectedIndex == 1 && ws1RadioButtonList1.SelectedIndex == 3)
        {
            lp1ControlEnable(false);
            lp2ControlEnable(false);
            lp3ControlEnable(false);
            lp4ControlEnable(false);
            lp5ControlEnable(true);

            AddCssClass(lp1, "panel-hide");
            AddCssClass(lp2, "panel-hide");
            AddCssClass(lp3, "panel-hide");
            AddCssClass(lp4, "panel-hide");
            RemoveCssClass(lp5, "panel-hide");

            calcrDropDownList1.Enabled = false;
            calcrTextBox2.Enabled = false;
            calcrTextBox1.Text = "";
            calcrTextBox2.Text = "";
            AddCssClass(calcr, "panel-hide");
        }

        if (ws1RadioButtonList1.SelectedIndex != 3)
        {
            fprDropDownList1.Items[1].Enabled = true;
            fprDropDownList1.Items[2].Enabled = true;
            fprDropDownList1.Items[3].Enabled = true;
            fprDropDownList1.Items[4].Enabled = true;
            fprDropDownList1.Items[5].Enabled = true;
            lp5RadioButtonList1.Enabled = false;
            lp5RadioButtonList1.SelectedIndex = -1;
            lp5TextBox3.Enabled = false;
            
            fprRadioButton2.Enabled = true;

            if (LabelSteam.Text == "Y")
            {
                DisableTextBox(fprTextBox1);
                fprRadioButton1.Checked = false;
                dropDownListEnable(fprDropDownList1, false);
                LabelSteam.Text = "N";
                AddCssClass(fpr1_1, "panel-hide");
            }
            calcr.Visible = true;
            fpr1.Visible = true;
            fpr2.Visible = true;
            Label31.Visible = true;
        }
        else
        {
            LabelSteam.Text = "Y";
            lp1ControlEnable(false);
            lp2ControlEnable(false);
            lp3ControlEnable(false);
            lp4ControlEnable(false);
            fprDropDownList1.Items[1].Enabled = false;
            fprDropDownList1.Items[2].Enabled = false;
            fprDropDownList1.Items[3].Enabled = false;
            fprDropDownList1.Items[4].Enabled = false;
            fprDropDownList1.Items[5].Enabled = false;
            lp5RadioButtonList1.Enabled = true;
            fprRadioButton2.Checked = false;
            fprRadioButton2.Enabled = false;
            fprRadioButton2.Checked = false;
            textBoxEnabled(fprTextBox1, false);
            textBoxEnabled(fprTextBox2, false);
            textBoxEnabled(fprTextBox3, false);
            textBoxEnabled(fprTextBox4, false);
            textBoxEnabled(fprTextBox5, false);
            textBoxDisable(calcrTextBox1);
            textBoxDisable(calcrTextBox2);
            dropDownListEnable(calcrDropDownList1, false);
            dropDownListEnable(fprDropDownList1, true);
            fprDropDownList1.SelectedIndex = -1;
            dropDownListEnable(fprDropDownList2, false);
            RemoveCssClass(fpr1_1, "panel-hide");
            AddCssClass(fpr2_1, "panel-hide");
            fprRadioButton1.Checked = true;
            calcr.Visible = false;
            fpr1.Visible = true;
            fpr2.Visible = false;
            Label31.Visible = true;
        }

    }

    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        string clientScript = "javascript:ShowBTN();";
        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "MyClientScript", clientScript);
        objTextBox1.Visible = true;
        objTextBox1.Enabled = true;
        Label53.Visible = true;
    }

    //------------------------------------Event Function END--------------------------------------

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

    protected void lp5RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lp5RadioButtonList1.SelectedIndex == 1)
        {
            lp5TextBox3.Enabled = false;
        }
        else
        {
            lp5TextBox3.Enabled = true;
        }
    }
}