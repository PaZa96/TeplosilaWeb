using GemBox.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
        arrConvert3 = new double[4] { 1000, 1, 100, 10 };

        Logger.InitLogger();//инициализация - требуется один раз в начале

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


        if (this.fprRadioButton1.Checked)
        {
            if (this.ws1RadioButtonList1.SelectedIndex == 0)
            {
                avg_T = double.Parse(this.calcrTextBox2.Text);
            }
            else
            {
                avg_T = Convert.ToDouble(this.ws1TextBox2.Text);
            }
        }
        else
        {
            avg_T = 0.5 * (double.Parse(this.fprTextBox2.Text) + double.Parse(this.fprTextBox3.Text));
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
            Etgl(GetAvgT(), double.Parse(this.ws1TextBox1.Text), ref rr, ref cp);

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
            Prgl(GetAvgT(), double.Parse(this.ws1TextBox1.Text), ref rr, ref cp);
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

    private Dictionary<string, string[]> generatedTableR(Dictionary<string, double> g_dict)
    {
        try
        {

            /*BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB*/
            double Kv = 0, Gpg = 0, dPg = 0, g = 0;
            int DN = 0;
            double Kv_start = 0;
            double tmpKv = 0;
            string tmpA = "";

            Dictionary<string, string[]> listResult = new Dictionary<string, string[]>();
            listResult.Add("A", new string[] { });
            listResult.Add("B", new string[] { });
            listResult.Add("C", new string[] { });
            listResult.Add("D", new string[] { });
            listResult.Add("I", new string[] { });
            listResult.Add("F", new string[] { });
            listResult.Add("E", new string[] { });
            listResult.Add("G", new string[] { });
            listResult.Add("K", new string[] { });
            Gpg = g_dict["p16"];
            if (eorRadioButtonList1.SelectedIndex == 0)
            {
                dPg = Convert.ToDouble(this.lp1TextBox1.Text) * arrConvert3[this.lp1DropDownList1.SelectedIndex - 1];
            }
            else if (eorRadioButtonList1.SelectedIndex == 1)
            {
                dPg = Convert.ToDouble(this.lp2TextBox1.Text) * arrConvert3[this.lp2DropDownList1.SelectedIndex - 1] - Convert.ToDouble(this.lp2TextBox2.Text) * arrConvert3[this.lp2DropDownList2.SelectedIndex - 1];
            }
            else if (eorRadioButtonList1.SelectedIndex == 2)
            {
                dPg = Convert.ToDouble(this.lp3TextBox1.Text) * arrConvert3[this.lp3DropDownList1.SelectedIndex - 1] - Convert.ToDouble(this.lp3TextBox2.Text) * arrConvert3[this.lp3DropDownList2.SelectedIndex - 1];
            }
            else if (eorRadioButtonList1.SelectedIndex == 3)
            {
                dPg = Convert.ToDouble(this.lp4TextBox2.Text) * arrConvert3[this.lp4DropDownList2.SelectedIndex - 1];
            }

            /*if (dPg < 15) 
            {
                dPg = 80;
            }*/

            /*
            double middle_T = 0;

            if (this.fprRadioButton1.Checked)
            {
                if (this.ws1RadioButton1.Checked)
                {
                        middle_T = double.Parse(this.calcrTextBox2.Text);
                }
                else
                {
                        middle_T = Convert.ToDouble(this.ws1TextBox2.Text);
                }
            }
            else
            {
                middle_T = 0.5 * (double.Parse(this.fprTextBox2.Text) + double.Parse(this.fprTextBox3.Text));                
            }*/

            if (ws1RadioButtonList1.SelectedIndex == 0)
            {
                Water(GetAvgT(), ref g);
            }
            else if (ws1RadioButtonList1.SelectedIndex == 1)
            {
                double p6 = Convert.ToDouble(this.ws1TextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Etgl(p7, p6, ref g, ref cp);
            }
            else if (ws1RadioButtonList1.SelectedIndex == 2)
            {
                double p6 = Convert.ToDouble(this.ws1TextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Prgl(p7, p6, ref g, ref cp);
            }

            /*
            if (this.ws1RadioButton1.Checked)
            {
                //g = 1000;
                if (this.fprRadioButton1.Checked)
                {
                    Water(double.Parse(this.calcrTextBox2.Text), ref g);
                }
                else
                {
                    Water(0.5 * (double.Parse(this.fprTextBox2.Text) + double.Parse(this.fprTextBox3.Text)), ref g);
                }
            }
            else 
            {
                double p6 = Convert.ToDouble(this.ws1TextBox1.Text);
                double p7 = Math.Round(Convert.ToDouble(this.ws1TextBox2.Text) / 10) * 10;
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

            Kv = 1.2 * (Gpg * 0.01) / (Math.Sqrt(dPg * 0.001 * g));
            Newtonsoft.Json.Linq.JArray table5 = dataFromFile.table5;
            Newtonsoft.Json.Linq.JArray table10 = dataFromFile.table10;
            double col_B = Convert.ToDouble(table5[table5.Count - 1]);
            int col_C = Convert.ToInt32(table10[table10.Count - 1]);


            /*IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII*/

            double I = 0;
            Newtonsoft.Json.Linq.JObject tmpI = null;

            if (eorRadioButtonList1.SelectedIndex == 0)
            {
                I = Convert.ToDouble(this.lp1TextBox2.Text) * arrConvert3[this.lp1DropDownList2.SelectedIndex - 1] / arrConvert3[2];
            }
            else if (eorRadioButtonList1.SelectedIndex == 1)
            {
                I = Convert.ToDouble(this.lp2TextBox2.Text) * arrConvert3[this.lp2DropDownList2.SelectedIndex - 1] / arrConvert3[2];
            }
            else if (eorRadioButtonList1.SelectedIndex == 2)
            {
                I = Convert.ToDouble(this.lp3TextBox1.Text) * arrConvert3[this.lp3DropDownList1.SelectedIndex - 1] / arrConvert3[2];
            }
            else if (eorRadioButtonList1.SelectedIndex == 3)
            {
                I = Convert.ToDouble(this.lp4TextBox2.Text) * arrConvert3[this.lp4DropDownList2.SelectedIndex - 1] / arrConvert3[2];
            }

            if (I < (((eorRadioButtonList1.SelectedIndex == 2 || eorRadioButtonList1.SelectedIndex == 3)) ? 0.16 : 0.08) || I > 15.8)
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
            if (eorRadioButtonList1.SelectedIndex == 0)
            {
                table = dataFromFile.table71;
            }
            else if (eorRadioButtonList1.SelectedIndex == 1)
            {
                table = dataFromFile.table72;
            }
            else if (eorRadioButtonList1.SelectedIndex == 2)
            {
                table = dataFromFile.table73;
            }
            else if (eorRadioButtonList1.SelectedIndex == 3)
            {
                table = dataFromFile.table74;
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

            double C = Convert.ToDouble(listResult["C"][listResult["C"].Count() - 1]),
                    V = Gpg * convertTable[1, 5] * Math.Pow((18.8 / C), 2);



            double Pf = 1;

            while (!exit_t && (V >= g_dict["vmax"]))
            {
                if (exit_t)
                    break;
                else
                {
                    // DN ближайший больший из table10

                    if (col_C == Convert.ToDouble(table10[table10.Count - 1]))
                    {
                        foreach (int el in table10)
                        {
                            if ((el <= col_C) && (el > DN))
                            {
                                col_C = el;
                            }
                        }
                    }
                    else
                    {
                        int col_Ct = Convert.ToInt32(table10[table10.Count - 1]);
                        foreach (int el in table10)
                        {
                            if ((el <= col_Ct) && (el >= DN) && (el > col_C))
                            {
                                col_Ct = el;
                            }
                        }
                        col_C = col_Ct;
                    }

                    bool meetEnd = false;

                    if (col_C == Convert.ToDouble(table10[table10.Count - 1]))
                    {

                        exit_t = true;

                        foreach (string keyValue in listResult["C"])
                        {
                            if (keyValue.Equals(table10[table10.Count - 1].ToString()))
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

                        Kv_start = 1.2 * (Gpg * 0.01) / (Math.Sqrt(dPg * 0.001 * g));
                        tmpKv = 300;
                        tmpA = "";
                        foreach (Newtonsoft.Json.Linq.JObject ob in table)
                        {
                            if ((Convert.ToDouble(ob.GetValue("d")) == DN) &&
                                (Convert.ToDouble(ob.GetValue("min")) == Convert.ToDouble(tmpI.GetValue("min")) &&
                                Convert.ToDouble(ob.GetValue("max")) == Convert.ToDouble(tmpI.GetValue("max"))))
                            {
                                if (Kv_start < Convert.ToDouble(ob.GetValue("prop")) && tmpKv > Convert.ToDouble(ob.GetValue("prop")))
                                {
                                    tmpKv = Convert.ToDouble(ob.GetValue("prop"));
                                    tmpA = ob.GetValue("name").ToString();
                                }
                                //listA.Add(ob.GetValue("name").ToString());
                                //listB.Add(ob.GetValue("prop").ToString());
                                //DN = int.Parse(ob.GetValue("d").ToString());

                            }
                        }

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
                    /*/CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC*/
                    /*/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA*/

                }

                V = Gpg * convertTable[1, 5] * Math.Pow((18.8 / DN), 2);

            }

            /*FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF*/
            /*GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG*/
            List<string> listD = new List<string>(),
                listF = new List<string>(),
                listG = new List<string>(),
                listE = new List<string>(),
                listK = new List<string>();

            listD.AddRange(listResult["D"]);
            listF.AddRange(listResult["F"]);
            listG.AddRange(listResult["G"]);
            listE.AddRange(listResult["E"]);
            listK.AddRange(listResult["K"]);


            for (int i = 0; i < listResult["C"].Count(); i++)
            {
                /*DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD*/
                Pf = (Math.Pow(Gpg, 2) * 0.1) / (Math.Pow(double.Parse(listResult["B"].GetValue(i).ToString()), 2) * g);
                Pf = Math.Round(Pf / 100, 2); /*Перевод с кПа в бар*/
                                              //listResult["D"] = new string[] { Pf.ToString() };

                listD.Add(Pf.ToString());

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
                V = Gpg * convertTable[1, 5] * Math.Pow((18.8 / Convert.ToDouble(listResult["C"][i])), 2);

                if (V < g_dict["vmax"])
                    exit_t = true;

                listF.Add(Math.Round(V, 2).ToString());
                if (V > g_dict["vmax"])
                {
                    listE.Add((this.sprRadioButtonList1.SelectedIndex == 0) ? "возможен эрозийный износ клапана" : "возможен шум");
                }
                else if (V < 1.5)
                {
                    listE.Add("возможен колебательный режим регулирования");
                }
                else
                {
                    listE.Add("нет");
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

                    double t1 = Convert.ToDouble(this.calcrTextBox2.Text);
                    Newtonsoft.Json.Linq.JObject max = dataFromFile.table9[dataFromFile.table9.Count - 1];
                    foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9)
                    {
                        if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1))
                        {
                            max = ob;
                        }
                    }
                    ps = Convert.ToDouble(max.GetValue("ps"));

                    double G = Math.Round((dn * ((Convert.ToDouble(this.calcrTextBox1.Text) * arrConvert3[this.calcrDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - ps)), 2);
                    listG.Add(G.ToString());

                    string K = "Нет";
                    if (G < Pf)//double.Parse(listResult["D"].GetValue(i).ToString()) )//Pf)
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
            listResult["D"] = listD.ToArray();
            listResult["F"] = listF.ToArray();
            listResult["E"] = listE.ToArray();
            listResult["G"] = listG.ToArray();
            listResult["K"] = listK.ToArray();
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
        try { 
        //
        r_in_dict.Add(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        r_in_dict.Add(1, DateTime.Now.ToShortDateString().ToString());
        r_in_dict.Add(2, "-"); // Объект добавляется в диалоговом окне при сохранении

        IEnumerable<RadioButton> ie_rb = null;

        r_in_dict.Add(3, sprRadioButtonList1.Items[sprRadioButtonList1.SelectedIndex].Text);

        r_in_dict.Add(4, eorRadioButtonList1.Items[eorRadioButtonList1.SelectedIndex].Text);


        r_in_dict.Add(5, "Marka"); // Марка добавляется в диалоговом окне при сохранении


        r_in_dict.Add(6, ws1RadioButtonList1.Items[ws1RadioButtonList1.SelectedIndex].Text + " " + ((this.ws1TextBox1.Enabled) ? (this.ws1TextBox1.Text + " %, " + this.ws1TextBox2.Text + " °С") : ""));

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

        // для пара еще нет
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


        r_in_dict.Add(39, "150 ˚С");
        r_in_dict.Add(40, "16 бар");

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
            if (r_in_dict[41].Contains("-0.1-"))
                tableDimR = dataFromFile.table20;
            else tableDimR = dataFromFile.table21;
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
                rPictureBox.ImageUrl = @"./Content/images/RDT-RDT-P.jpg";
                break;
            case 1:
                rPictureBox.ImageUrl = @"./Content/images/RDT-RDT-P.jpg";
                break;
            case 2:
                rPictureBox.ImageUrl = @"./Content/images/RDT-S-RDT-B.jpg";
                break;
            case 3:
                rPictureBox.ImageUrl = @"./Content/images/RDT-S-RDT-B.jpg";
                break;

            default:
                rPictureBox.ImageUrl = null;
                break;
        }
    }

    //------------------------------------File Function END--------------------------------------


    //------------------------------------Validation Function START--------------------------------------

    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (convertArrToBar(arrConvert3, lp1DropDownList3, lp1TextBox3) > PressureBeforeValve3x)
        {
            CustomValidator1.ErrorMessage = "На давление свыше 16 бар вариантов нет";
            args.IsValid = false;
        }
    }

    protected void CustomValidator2_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (convertArrToBar(arrConvert3, lp1DropDownList4, lp1TextBox4) >= convertArrToBar(arrConvert3, lp1DropDownList3, lp1TextBox3))
        {
            CustomValidator2.ErrorMessage = "Неверно указано значение давления";
            args.IsValid = false;

        }
    }
    protected void CustomValidator3_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (convertArrToBar(arrConvert3, lp2DropDownList1, lp2TextBox1) > PressureBeforeValve3x)
        {
            CustomValidator3.ErrorMessage = "На давление свыше 16 бар вариантов нет";
            args.IsValid = false;
        }
    }

    protected void CustomValidator4_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (convertArrToBar(arrConvert3, lp2DropDownList2, lp2TextBox2) >= convertArrToBar(arrConvert3, lp2DropDownList1, lp2TextBox1))
        {
            CustomValidator4.ErrorMessage = "Неверно указано значение давления";
            args.IsValid = false;
        }
    }

    protected void CustomValidator5_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (convertArrToBar(arrConvert3, lp3DropDownList1, lp3TextBox1) > PressureBeforeValve3x)
        {
            CustomValidator5.ErrorMessage = "На давление свыше 16 бар вариантов нет";
            args.IsValid = false;
        }
    }

    protected void CustomValidator6_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (convertArrToBar(arrConvert3, lp3DropDownList2, lp3TextBox2) >= convertArrToBar(arrConvert3, lp3DropDownList1, lp3TextBox1))
        {
            CustomValidator6.ErrorMessage = "Неверно указано значение давления";
            args.IsValid = false;
        }
    }

    protected void CustomValidator7_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (convertArrToBar(arrConvert3, lp4DropDownList2, lp4TextBox2) > PressureBeforeValve3x)
        {
            CustomValidator7.ErrorMessage = "На давление свыше 16 бар вариантов нет";
            args.IsValid = false;
        }
    }

    protected void CustomValidator8_ServerValidate(object source, ServerValidateEventArgs args)
    {
        double var1 = convertArrToBar(arrConvert3, lp1DropDownList1, lp1TextBox1) + convertArrToBar(arrConvert3, lp1DropDownList2, lp1TextBox2);
        double var2 = convertArrToBar(arrConvert3, lp1DropDownList3, lp1TextBox3) - convertArrToBar(arrConvert3, lp1DropDownList4, lp1TextBox4);
        if (var1 > var2)
        {
            CustomValidator8.ErrorMessage = "Неверно указано значение давления";
            args.IsValid = false;
        }
    }

    protected void CustomValidator9_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (convertArrToBar(arrConvert3, calcrDropDownList1, calcrTextBox1) > PressureBeforeValve3x)
        {
            CustomValidator9.ErrorMessage = "На давление свыше 16 бар вариантов нет";
            args.IsValid = false;
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

    private void convertArrDouble(double[,] arr, DropDownList ddl, ref TextBox tb)
    {
        if (ddl.SelectedIndex > 0)
        {
            if (!String.IsNullOrWhiteSpace(tb.Text))
            {
                int jj = keyValuePairs[ddl.ID];

                if (jj > 0)
                {
                    tb.Text = (customConverterToDouble(tb.Text) * arr[(jj - 1), (ddl.SelectedIndex - 1)]).ToString().Replace(",", ".");
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
                tb.Text = (customConverterToDouble(tb.Text) * arr[jj - 1] / arr[ddl.SelectedIndex - 1]).ToString().Replace(",", ".");
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

    public void lp1ControlEnable(bool flag)
    {
        dropDownListEnable(lp1DropDownList1, flag);
        dropDownListEnable(lp1DropDownList2, flag);
        dropDownListEnable(lp1DropDownList3, flag);
        dropDownListEnable(lp1DropDownList4, flag);
    }
    public void lp2ControlEnable(bool flag)
    {
        dropDownListEnable(lp2DropDownList1, flag);
        dropDownListEnable(lp2DropDownList2, flag);
    }
    public void lp3ControlEnable(bool flag)
    {
        dropDownListEnable(lp3DropDownList1, flag);
        dropDownListEnable(lp3DropDownList2, flag);
    }
    public void lp4ControlEnable(bool flag)
    {
        dropDownListEnable(lp4DropDownList2, flag);
    }

    public void dropDownListEnable(DropDownList dropDownList, bool flag)
    {
        dropDownList.Enabled = flag;
        dropDownList.ClearSelection();
    }

    public void textBoxEnabled(TextBox textBox, bool flag)
    {
        textBox.Enabled = flag;
        textBox.Text = "";
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
        try { 
        objTextBox1.Enabled = false;
        GridView1.Columns.Clear();
        GridView1.DataSource = null;
        GridView1.DataBind();

        this.readFile(0);
        Dictionary<string, double> g_dict = new Dictionary<string, double>();
        r_input_dict.Clear();

        if (sprRadioButtonList1.SelectedIndex != -1)
        {
            if (this.sprRadioButtonList1.SelectedIndex == 0) g_dict.Add("vmax", 5); else g_dict.Add("vmax", 3);

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
                            p6 = Convert.ToDouble(this.ws1TextBox1.Text);
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
                            p7 = Convert.ToDouble(this.ws1TextBox2.Text);
                        }
                        catch (Exception)
                        {
                            LabelError.Text = "Не указано значение температуры ";
                            return;
                        }

                        if (p7 < 0 || p7 > 150)
                        {
                            LabelError.Text = "Неверно указано значение температуры ";
                            return;
                        }
                        else
                        {
                            g_dict.Add("p7", p7);
                        }
                    }

                    if (fprRadioButton1.Checked || fprRadioButton2.Checked)
                    {
                        double checkVal;

                        try
                        {
                            if (this.fprRadioButton1.Checked)
                            {
                                checkVal = Convert.ToDouble(this.fprTextBox1.Text);
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
                                checkVal = Convert.ToDouble(this.fprTextBox2.Text);
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
                                checkVal = Convert.ToDouble(this.fprTextBox3.Text);
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
                                checkVal = Convert.ToDouble(this.fprTextBox4.Text);
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
                        else if (this.fprRadioButton2.Checked && (Convert.ToDouble(this.fprTextBox2.Text) > 150))
                        {
                            LabelError.Text = "На температуру свыше 150°С вариантов нет";
                            return;
                        }
                        else if (this.fprRadioButton2.Checked && this.checkTextBoxEmpty(this.fprTextBox3))
                        {
                            LabelError.Text = "Не задано значение температуры";
                            return;
                        }
                        else if (this.fprRadioButton2.Checked && Convert.ToDouble(this.fprTextBox2.Text) <= Convert.ToDouble(this.fprTextBox3.Text))
                        {
                            LabelError.Text = "Неверно указано значение температуры";
                            return;
                        }
                        else if (this.fprRadioButton2.Checked && (Convert.ToDouble(this.fprTextBox3.Text) > 150))
                        {
                            LabelError.Text = "На температуру свыше 150°С вариантов нет";
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
                                    p16 = Math.Round((Convert.ToDouble(this.fprTextBox4.Text) * arrConvert2[this.fprDropDownList2.SelectedIndex - 1]) * 3.6 / (this.math_16_cp() * (Convert.ToDouble(this.fprTextBox2.Text) - Convert.ToDouble(this.fprTextBox3.Text))), 2);

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
                                p16 = (Convert.ToDouble(this.fprTextBox1.Text) * arrConvert1[(this.fprDropDownList1.SelectedIndex - 1), 5]);
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
                                        p17 = Convert.ToDouble(this.lp1TextBox1.Text) * arrConvert3[this.lp1DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p19 = Convert.ToDouble(this.lp1TextBox2.Text) * arrConvert3[this.lp1DropDownList2.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p21 = Convert.ToDouble(this.lp1TextBox3.Text) * arrConvert3[this.lp1DropDownList3.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p23 = Convert.ToDouble(this.lp1TextBox4.Text) * arrConvert3[this.lp1DropDownList4.SelectedIndex - 1] / arrConvert3[2];
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

                                    if (!(p21 <= 16))
                                    {
                                        LabelError.Text = "На давление свыше 16 бар вариантов нет";
                                        return;
                                    }
                                    else if (!(p23 < p21))
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }
                                    else if (!((p17 + p19) <= (p21 - p23)))
                                    {
                                        lp1TextBox1.BackColor = Color.LightPink;
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
                                        this.lp1TextBox5.Text = p25.ToString();
                                    }
                                }
                            }
                            else if (eorRadioButtonList1.SelectedIndex == 1)
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
                                        p26 = Convert.ToDouble(this.lp2TextBox1.Text) * arrConvert3[this.lp2DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p28 = Convert.ToDouble(this.lp2TextBox2.Text) * arrConvert3[this.lp2DropDownList2.SelectedIndex - 1] / arrConvert3[2];
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

                                    if (!(p26 <= 16))
                                    {
                                        LabelError.Text = "На давление свыше 16 бар вариантов нет";
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
                                        p30 = Convert.ToDouble(this.lp3TextBox1.Text) * arrConvert3[this.lp3DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                    }
                                    catch (Exception)
                                    {
                                        LabelError.Text = "Неверно указано значение давления";
                                        return;
                                    }

                                    try
                                    {
                                        p32 = Convert.ToDouble(this.lp3TextBox2.Text) * arrConvert3[this.lp3DropDownList2.SelectedIndex - 1] / arrConvert3[2];
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

                                    if (!(p30 <= 16))
                                    {
                                        LabelError.Text = "На давление свыше 16 бар вариантов нет";
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
                                        p19 = Convert.ToDouble(this.lp4TextBox2.Text) * arrConvert3[this.lp4DropDownList2.SelectedIndex - 1] / arrConvert3[2];
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

                                    if (!(p19 <= 16))
                                    {
                                        LabelError.Text = "На давление свыше 16 бар вариантов нет";
                                        return;
                                    }
                                    else
                                    {
                                        g_dict.Add("p19", p19);
                                    }
                                }
                            }

                            try
                            {
                                double ptemp = Convert.ToDouble(this.calcrTextBox1.Text);
                            }
                            catch (Exception)
                            {
                                LabelError.Text = "Неверно указано значение давления";
                                return;
                            }

                            if (Convert.ToDouble(this.calcrTextBox1.Text) <= 0)
                            {

                                LabelError.Text = "Неверно указано значение давления";
                                return;
                            }
                            else if ((Convert.ToDouble(this.calcrTextBox1.Text) * arrConvert3[this.calcrDropDownList1.SelectedIndex - 1] / arrConvert3[2]) > 16)
                            {

                                LabelError.Text = "На давление свыше 16 бар вариантов нет";
                                return;
                            }



                            double p35 = 0;
                            try
                            {
                                p35 = Convert.ToDouble(this.calcrTextBox2.Text);
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
                            else if (p35 > 150)
                            {

                                LabelError.Text = "На температуру свыше 150°С вариантов нет";
                                return;
                            }


                            /*if (p35 < 7 || p35 > 150)
                            {
                                MessageBox.Show("Не задана температура для расчета регулятора на кавитацию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }*/

                            this.ws1ResultLabel.Text = "Рабочая среда - " + (ws1RadioButtonList1.SelectedIndex == 0 ? "вода" : ((ws1RadioButtonList1.SelectedIndex == 1 ? "этиленгликоль " : "пропиленгликоль ") + g_dict["p6"] + "%, " + g_dict["p7"] + " °С"));
                            //this.ws1ResultLabel.Text = "Рабочая среда - " + (this.ws1RadioButton1.Checked ? "вода" : "этиленгликоль " + g_dict["p6"] + "%, " + g_dict["p7"] + " °С");
                            this.maxt1ResultLabel.Text = "Максимальная температура - " + (ws1RadioButtonList1.SelectedIndex == 0 ? "150 °С" : "150 °С");
                            this.maxp1ResultLabel.Text = "Максимальное рабочее давление - 16 бар";

                            double t1_check = Convert.ToDouble(this.calcrTextBox2.Text);
                            Newtonsoft.Json.Linq.JObject max_check = dataFromFile.table9[dataFromFile.table9.Count - 1];
                            foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9)
                            {
                                if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max_check.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1_check))
                                {
                                    max_check = ob;
                                }
                            }
                           //double ps_check = Convert.ToDouble(max_check.GetValue("ps"));

                            if (((Convert.ToDouble(this.calcrTextBox1.Text) * arrConvert3[this.calcrDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - getPSbyT(t1_check)) <= 0)
                            {
                                LabelError.Text = "Указанная температура выше температуры парообразования. При указанной температуре в трубопроводе движется пар";
                                return;
                            }

                            mapInputParametersR(ref r_input_dict);

                            Dictionary<string, string[]> gtr = this.generatedTableR(g_dict);

                            /*GridView1.Columns.Clear();
                            GridView1.Rows.Clear();
                            GridView1.Refresh();*/

                            string[] titles = new string[] {
                                "Марка регулятора давления",
                                "Номинальный диаметр DN, мм",
                                "Пропускная способность Kvs, м3/ч",
                                "Фактические потери давления на полностью открытом клапане при заданном расходе ∆Рф,\n бар\n",
                                "Диапазон настройки,\n бар",
                                "Скорость в выходном сечении регулятора V, м/с",
                                "Шум, некачественное регулирование",
                                "Предельно допустимый перепад давлений на регуляторе ∆Pпред, бар",
                                "Кавитация"
                            };

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
                                        case "A": index = 0; break;
                                        case "B": index = 2; break;
                                        case "C": index = 1; break;
                                        case "D": index = 3; break;
                                        case "I": index = 4; break;
                                        case "F": index = 5; break;
                                        case "E": index = 6; break;
                                        case "G": index = 7; break;
                                        case "K": index = 8; break;
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
                        LabelError.Text = "Не выбран расход через регулятор давления";
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
        }
        else
        {
            LabelError.Text = "Не выбрано место установки регулятора";
            return;
        }

        Label52.Visible = true;
        maxp1ResultLabel.Visible = true;
        maxt1ResultLabel.Visible = true;
        ws1ResultLabel.Visible = true;
        GridView1.Enabled = true;
        GridView1.Visible = true;
        GridView1.Height = 250;
        this.Button2.Visible = true;
        this.Button2.Enabled = true;
        this.Button3.Visible = true;
        this.Button3.Enabled = true;

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
        try
        {
            this.readFile(0);
            r_input_dict[2] = objTextBox1.Text;
            //r_input_dict.Add(2, (this.textBox2.Text != "")? this.textBox2.Text : "-");

            r_input_dict[5] = "-";
            //r_input_dict.Add(5, (this.textBox5.Text != "") ? this.textBox5.Text : "-");

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }


            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("\\Content\\templates\\templateRDT.xlsx")))
            {
                LabelError.Text = "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("\\Content\\templates\\templateRDT.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            ws.Cells["K53"].Value = r_input_dict[0];

            ws.Cells["J2"].Value = r_input_dict[1];
            ws.Cells["C3"].Value = r_input_dict[2];
            ws.Cells["C4"].Value = r_input_dict[3];
            ws.Cells["C5"].Value = r_input_dict[4];
            ws.Cells["C6"].Value = r_input_dict[5];
            ws.Cells["C9"].Value = r_input_dict[6];

            ws.Cells["I12"].Value = r_input_dict[7];
            ws.Cells["K12"].Value = r_input_dict[8];

            ws.Cells["I13"].Value = r_input_dict[9];
            ws.Cells["K13"].Value = r_input_dict[10];

            ws.Cells["I14"].Value = r_input_dict[11];
            ws.Cells["K14"].Value = r_input_dict[12];

            ws.Cells["I15"].Value = r_input_dict[13];
            ws.Cells["K15"].Value = r_input_dict[14];

            ws.Cells["I16"].Value = r_input_dict[15];

            ws.Cells["D18"].Value = r_input_dict[16];
            ws.Cells["E18"].Value = r_input_dict[17];

            ws.Cells["D19"].Value = r_input_dict[18];
            ws.Cells["E19"].Value = r_input_dict[19];

            // пар
            ws.Cells["J18"].Value = r_input_dict[20];
            ws.Cells["K18"].Value = r_input_dict[21];

            ws.Cells["J19"].Value = r_input_dict[22];
            ws.Cells["K19"].Value = r_input_dict[23];

            ws.Cells["J20"].Value = r_input_dict[24];
            // пар

            ws.Cells["I22"].Value = r_input_dict[25];
            ws.Cells["K22"].Value = r_input_dict[26];

            ws.Cells["I23"].Value = r_input_dict[27];
            ws.Cells["K23"].Value = r_input_dict[28];

            ws.Cells["I25"].Value = r_input_dict[29];
            ws.Cells["K25"].Value = r_input_dict[30];

            ws.Cells["I27"].Value = r_input_dict[31];
            ws.Cells["K27"].Value = r_input_dict[32];

            ws.Cells["I28"].Value = r_input_dict[33];

            ws.Cells["I30"].Value = r_input_dict[34];

            ws.Cells["I31"].Value = r_input_dict[35];

            ws.Cells["I32"].Value = r_input_dict[36];
            ws.Cells["K32"].Value = r_input_dict[37];

            ws.Cells["I33"].Value = r_input_dict[38];
            ws.Cells["K33"].Value = r_input_dict[381];

            ws.Cells["E36"].Value = r_input_dict[39];

            ws.Cells["E37"].Value = r_input_dict[40];

            ws.Cells["A40"].Value = r_input_dict[41];
            ws.Cells["B40"].Value = r_input_dict[42];
            ws.Cells["C40"].Value = r_input_dict[43];
            ws.Cells["D40"].Value = r_input_dict[44];
            ws.Cells["F40"].Value = r_input_dict[45];
            ws.Cells["G40"].Value = r_input_dict[46];
            ws.Cells["H40"].Value = r_input_dict[47];
            ws.Cells["I40"].Value = r_input_dict[48];
            ws.Cells["K40"].Value = r_input_dict[49];

            getDimsR(ref r_input_dict);

            ws.Pictures.Add(HttpContext.Current.Server.MapPath("\\Content\\images\\" + ((r_input_dict[4] == this.eorRadioButtonList1.Items[0].Text) || (r_input_dict[4] == eorRadioButtonList1.Items[1].Text) ? "RDT-RDT-P.jpg" : "RDT-S-RDT-B.jpg")), "A44", "B53");



            ws.Cells["F44"].Value = r_input_dict[51];
            ws.Cells["F45"].Value = r_input_dict[52];
            ws.Cells["F46"].Value = r_input_dict[53];
            ws.Cells["F47"].Value = r_input_dict[54];


            string path = HttpContext.Current.Server.MapPath("\\Files\\RDT\\PDF\\" + DateTime.Now.ToString("dd-MM-yyyy"));
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

            string filePath = path + "\\" + fileName + ".pdf";

            ef.Save(filePath);

            WaitDownload(50);

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.TransmitFile(file.FullName);
                Response.End();
            }
        }

        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }
    

    protected void Button3_Click(object sender, EventArgs e)
    {
        try
        {
            this.readFile(0);
            r_input_dict[2] = objTextBox1.Text;
            //r_input_dict.Add(2, (this.textBox2.Text != "")? this.textBox2.Text : "-");

            r_input_dict[5] = "-";
            //r_input_dict.Add(5, (this.textBox5.Text != "") ? this.textBox5.Text : "-");

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }


            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("\\Content\\templates\\templateRDT.xlsx")))
            {
                LabelError.Text = "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("\\Content\\templates\\templateRDT.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            ws.Cells["K53"].Value = r_input_dict[0];

            ws.Cells["J2"].Value = r_input_dict[1];
            ws.Cells["C3"].Value = r_input_dict[2];
            ws.Cells["C4"].Value = r_input_dict[3];
            ws.Cells["C5"].Value = r_input_dict[4];
            ws.Cells["C6"].Value = r_input_dict[5];
            ws.Cells["C9"].Value = r_input_dict[6];

            ws.Cells["I12"].Value = r_input_dict[7];
            ws.Cells["K12"].Value = r_input_dict[8];

            ws.Cells["I13"].Value = r_input_dict[9];
            ws.Cells["K13"].Value = r_input_dict[10];

            ws.Cells["I14"].Value = r_input_dict[11];
            ws.Cells["K14"].Value = r_input_dict[12];

            ws.Cells["I15"].Value = r_input_dict[13];
            ws.Cells["K15"].Value = r_input_dict[14];

            ws.Cells["I16"].Value = r_input_dict[15];

            ws.Cells["D18"].Value = r_input_dict[16];
            ws.Cells["E18"].Value = r_input_dict[17];

            ws.Cells["D19"].Value = r_input_dict[18];
            ws.Cells["E19"].Value = r_input_dict[19];

            // пар
            ws.Cells["J18"].Value = r_input_dict[20];
            ws.Cells["K18"].Value = r_input_dict[21];

            ws.Cells["J19"].Value = r_input_dict[22];
            ws.Cells["K19"].Value = r_input_dict[23];

            ws.Cells["J20"].Value = r_input_dict[24];
            // пар

            ws.Cells["I22"].Value = r_input_dict[25];
            ws.Cells["K22"].Value = r_input_dict[26];

            ws.Cells["I23"].Value = r_input_dict[27];
            ws.Cells["K23"].Value = r_input_dict[28];

            ws.Cells["I25"].Value = r_input_dict[29];
            ws.Cells["K25"].Value = r_input_dict[30];

            ws.Cells["I27"].Value = r_input_dict[31];
            ws.Cells["K27"].Value = r_input_dict[32];

            ws.Cells["I28"].Value = r_input_dict[33];

            ws.Cells["I30"].Value = r_input_dict[34];

            ws.Cells["I31"].Value = r_input_dict[35];

            ws.Cells["I32"].Value = r_input_dict[36];
            ws.Cells["K32"].Value = r_input_dict[37];

            ws.Cells["I33"].Value = r_input_dict[38];
            ws.Cells["K33"].Value = r_input_dict[381];

            ws.Cells["E36"].Value = r_input_dict[39];

            ws.Cells["E37"].Value = r_input_dict[40];

            ws.Cells["A40"].Value = r_input_dict[41];
            ws.Cells["B40"].Value = r_input_dict[42];
            ws.Cells["C40"].Value = r_input_dict[43];
            ws.Cells["D40"].Value = r_input_dict[44];
            ws.Cells["F40"].Value = r_input_dict[45];
            ws.Cells["G40"].Value = r_input_dict[46];
            ws.Cells["H40"].Value = r_input_dict[47];
            ws.Cells["I40"].Value = r_input_dict[48];
            ws.Cells["K40"].Value = r_input_dict[49];

            getDimsR(ref r_input_dict);

            ws.Pictures.Add(HttpContext.Current.Server.MapPath("\\Content\\images\\" + ((r_input_dict[4] == this.eorRadioButtonList1.Items[0].Text) || (r_input_dict[4] == eorRadioButtonList1.Items[1].Text) ? "RDT-RDT-P.jpg" : "RDT-S-RDT-B.jpg")), "A44", "B53");


            ws.Cells["F44"].Value = r_input_dict[51];
            ws.Cells["F45"].Value = r_input_dict[52];
            ws.Cells["F46"].Value = r_input_dict[53];
            ws.Cells["F47"].Value = r_input_dict[54];


            string path = HttpContext.Current.Server.MapPath("\\Files\\RDT\\PDF\\" + DateTime.Now.ToString("dd-MM-yyyy"));
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
                Response.End();
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    protected void eorRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        changeImage(eorRadioButtonList1.SelectedIndex);
        switch (eorRadioButtonList1.SelectedIndex)
        {
            case 0:
                lp1ControlEnable(true);
                lp2ControlEnable(false);
                lp3ControlEnable(false);
                lp4ControlEnable(false);
                break;
            case 1:
                lp1ControlEnable(false);
                lp2ControlEnable(true);
                lp3ControlEnable(false);
                lp4ControlEnable(false);
                break;
            case 2:
                lp1ControlEnable(false);
                lp2ControlEnable(false);
                lp3ControlEnable(true);
                lp4ControlEnable(false);
                break;
            case 3:
                lp1ControlEnable(false);
                lp2ControlEnable(false);
                lp3ControlEnable(false);
                lp4ControlEnable(true);
                break;
        }
        
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
        }
    }


    protected void ws1RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        dropDownListEnable(calcrDropDownList1, true);
        textBoxEnabled(calcrTextBox2, true);
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

}