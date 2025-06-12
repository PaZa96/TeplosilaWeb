using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            resultPanel.Visible = false;
            trvSave.Visible = false;
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

    private double getPSbyT(double t)
    {
        return Math.Pow(t / 103, 1 / 0.242) - 0.892;
    }

    private double getTbyPS(double ps)
    {
        return 103 * Math.Pow(ps + 0.892, 0.242);
    }

    public double math_30_cp()
    {
        double cp = 0;
        double rr = 0;

        if (wsRadioButtonList1.SelectedIndex == 0)
        {
            Water(GetAvgT(), ref rr);
            cp = WaterCP(GetAvgT()); // 4.187;
        }
        else if (wsRadioButtonList1.SelectedIndex == 1)
        {
            Etgl(GetAvgT(), customConverterToDouble(this.wsTextBox1.Text), ref rr, ref cp);
        }
        else if (wsRadioButtonList1.SelectedIndex == 2)
        {
            Prgl(GetAvgT(), customConverterToDouble(this.wsTextBox1.Text), ref rr, ref cp);
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

    public double GetAvgT()
    {
        double avg_T = 0;
        
        if (this.wsRadioButtonList1.SelectedIndex == 0)
        {
            avg_T = customConverterToDouble(this.calcvTextBox2.Text);
        }
        else
        {
            avg_T = customConverterToDouble(this.wsTextBox2.Text);
        }
       
        return avg_T;
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

    private void mapInputParametersV(ref Dictionary<int, string> v_in_dict)
    {
        try
        {
            v_in_dict.Add(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            v_in_dict.Add(1, DateTime.Now.ToShortDateString().ToString());
            v_in_dict.Add(2, "-"); // Объект добавляется в диалоговом окне при сохранении

            v_in_dict.Add(3, "-"); //было место установки
            v_in_dict.Add(4, "-");
            v_in_dict.Add(5, "-");
            v_in_dict.Add(6, tvRadioButton1.Checked ? tvRadioButton1.Text : tvRadioButton2.Text);
            v_in_dict.Add(7, tvRadioButton1.Checked ? tvRadioButtonList1.SelectedValue : tvRadioButtonList2.SelectedValue);
            v_in_dict.Add(8, "Marka"); // Марка добавляется в диалоговом окне при сохранении

            if (wsRadioButtonList1.SelectedIndex == 3)
            {
                v_in_dict.Add(9, wsRadioButtonList1.SelectedValue + " " + lpvRadioButtonList1.SelectedValue.ToLower());
            }
            else
            {
                v_in_dict.Add(9, wsRadioButtonList1.Items[wsRadioButtonList1.SelectedIndex].Text + " " + ((this.wsTextBox1.Enabled) ? (customConverterToDouble(this.wsTextBox1.Text) + " %, " + customConverterToDouble(this.wsTextBox2.Text) + " °С") : ""));

            }


            v_in_dict.Add(12, (this.lpvTextBox21.Enabled) ? this.lpvTextBox21.Text : "-");
            v_in_dict.Add(13, (this.lpvTextBox21.Enabled) ? this.lpvDropDownList21.Text : "-");

            v_in_dict.Add(16, (this.calcvTextBox1.Enabled) ? this.calcvTextBox1.Text : "-");
            v_in_dict.Add(17, (this.calcvTextBox1.Enabled) ? this.calcvDropDownList1.Text : "-");

            v_in_dict.Add(18, (this.calcvTextBox2.Text != "") ? this.calcvTextBox2.Text : "-");

            v_in_dict[34] = this.fvTextBox1.Text;
            v_in_dict[35] = this.fvDropDownList1.Text;


            if (tvRadioButtonList1.SelectedIndex == 0)
            {
                if (wsRadioButtonList1.SelectedIndex != 3)
                {
                    v_in_dict.Add(40, (customConverterToDouble(v_in_dict[18]) > 150 ? "220 ˚С" : "150 ˚С"));
                }
                else
                {
                    v_in_dict[40] = "220˚С";
                }

            }
            else
            {
                v_in_dict[40] = "150 ˚С";
            }

            v_in_dict.Add(41, pnRadioButtonList1.SelectedValue + " бар");

            //пар

            v_in_dict.Add(66, (this.lpv5TextBox1.Enabled) ? this.lpv5TextBox1.Text : "-");
            v_in_dict.Add(67, (this.lpv5TextBox1.Enabled) ? this.lpv5DropDownList1.Text : "-");


            v_in_dict.Add(71, this.lpv5TextBox3.Text);

            Session["v_input_dict"] = v_in_dict;
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
        }
    }

    private Dictionary<string, string[]> generatedTableV(Dictionary<string, double> g_dict)
    {

        LabelError.Text = "";
        /*BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB*/
        double Kv = 0, Gkl = 0, dPkl = 0, dPto = 0, g = 0, p1 = 0, p2 = 0, V = 0, T1 = 0;
        int DN = 0;
 


        Dictionary<string, string[]> listResult = new Dictionary<string, string[]>();
        listResult.Add("A", new string[] { });
        listResult.Add("C", new string[] { });
        listResult.Add("B", new string[] { });

        listResult.Add("I", new string[] { });
        listResult.Add("I3", new string[] { });

        if (wsRadioButtonList1.SelectedIndex != 3)
        {
            listResult.Add("I1", new string[] { });
            listResult.Add("I2", new string[] { });
            listResult.Add("D", new string[] { });
            listResult.Add("F", new string[] { });
            listResult.Add("G", new string[] { });
        }

        Gkl = g_dict["p30"];

        try
        {

            if (wsRadioButtonList1.SelectedIndex != 3)
            { 
                dPto = g_dict["p61"]; 
            }

            if (this.wsRadioButtonList1.SelectedIndex == 0)
            {
                Water(GetAvgT(), ref g);
            }
            else if (wsRadioButtonList1.SelectedIndex == 1)
            {
                double p6 = customConverterToDouble(this.wsTextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Etgl(p7, p6, ref g, ref cp);
            }
            else if (wsRadioButtonList1.SelectedIndex == 2)
            {
                double p6 = customConverterToDouble(this.wsTextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Prgl(p7, p6, ref g, ref cp);
            }

            if (wsRadioButtonList1.SelectedIndex == 3)
            {
                p1 = (customConverterToDouble(lpv5TextBox1.Text) * arrConvert3[lpv5DropDownList1.SelectedIndex - 1] / arrConvert3[2]);
                p2 = 0.6 * p1 - 0.4;

                if (lpvRadioButtonList1.SelectedIndex == 0)
                {
                    T1 = customConverterToDouble(lpv5TextBox3.Text);
                }
                else
                {
                    T1 = Math.Round(100 * Math.Pow((customConverterToDouble(lpv5TextBox1.Text) * arrConvert3[lpv5DropDownList1.SelectedIndex - 1] / arrConvert3[2]) + 1, 0.25));
                }
            }

            double col_B = Kv = customConverterToDouble(kvsDropDownList1.SelectedValue);
            listResult["B"] = new string[] { Kv.ToString() };

            int col_C = DN = Int32.Parse(dnDropDownList1.SelectedValue);

            double C = customConverterToDouble(dnDropDownList1.SelectedValue);


            listResult["C"] = new string[] { dnDropDownList1.SelectedValue };

            int PN = Int32.Parse(pnRadioButtonList1.SelectedValue);

            double Pf = 1;

            if (tvRadioButton1.Checked)
            {
                listResult["A"] = new string[] { tvRadioButtonList1.SelectedValue + "-" + DN + "-" + Kv + (PN == 25 ? "-25" : "") };
            }

            if (tvRadioButton2.Checked)
            {
                listResult["A"] = new string[] { tvRadioButtonList2.SelectedValue + "-" + DN + "-" + Kv + (PN == 25 ? "-25" : "") };
            }


            bool exit_t = false;

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

            listI3.AddRange(listResult["I3"]);

            if (wsRadioButtonList1.SelectedIndex != 3)
            {
                listI1.AddRange(listResult["I1"]);
                listI2.AddRange(listResult["I2"]);
                listF.AddRange(listResult["F"]);
                listG.AddRange(listResult["G"]);
                listD.AddRange(listResult["D"]);
            }

            Gkl = g_dict["p30"];

            for (int i = 0; i < listResult["C"].Count(); i++)
            {

                Pf = (Math.Pow(Gkl, 2) * 0.1) / (Math.Pow(double.Parse(listResult["B"].GetValue(i).ToString()), 2) * g);
                double dPf = Pf / 100;
                Pf = Math.Round(dPf, 2); /*Перевод с кПа в бар*/

                listD.Add(Pf.ToString());
                /*/DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD*/

                C = Convert.ToDouble(listResult["C"][i]);

                if (wsRadioButtonList1.SelectedIndex != 3)
                {
                    V = Gkl / g * Math.Pow((18.8 / C), 2);
                }
                else
                {
                    V = (Gkl * (T1 + 273)) / Math.Pow((C / 18.8), 2) / (219 * (p2 + 1));
                }

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
                else if (Ia >= 0.4 && Ia < 0.5)
                {
                    listI2.Add("удовлетво-\nрительное");
                }
                else
                {
                    listI2.Add("плохое");
                }

                if (wsRadioButtonList1.SelectedIndex != 3)
                {

                    if (V > 3 && V <= 5)
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
                }
                else
                {
                    if (V > 40 && lpvRadioButtonList1.SelectedIndex == 1)
                        listI3.Add("возможен шум");
                    else if (V > 60 && lpvRadioButtonList1.SelectedIndex == 0)
                        listI3.Add("возможен шум");
                    else
                    {
                        listI3.Add("нет");
                    }
                }

                if (wsRadioButtonList1.SelectedIndex != 3)
                {

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
                        ps = getPSbyT(t1);

                        double F = Math.Round((dn * ((customConverterToDouble(this.calcvTextBox1.Text) * arrConvert3[this.calcvDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - ps)), 2);
                        listF.Add(F.ToString());

                        string G_str = "Нет";
                        if (F < Pf)
                            G_str = "Угрожает опасность кавитации";

                        listG.Add(G_str);
                    }
                }
            }

            if (wsRadioButtonList1.SelectedIndex != 3)
            {
                listResult["I"] = listI.ToArray();
                listResult["I1"] = listI1.ToArray();
                listResult["I2"] = listI2.ToArray();
                listResult["I3"] = listI3.ToArray();
                listResult["F"] = listF.ToArray();
                listResult["G"] = listG.ToArray();
                listResult["D"] = listD.ToArray();
            }
            else
            {
                listResult["I"] = listI.ToArray();
                listResult["I3"] = listI3.ToArray();
            }


            string tmpMarkPriv = "";
            string tmpPriv = "";

            string paramDN = "";
            string paramKv = "";

            for (int i = 0; i < listResult["C"].Count(); i++)
            {

                tmpMarkPriv = tmpPriv = "";
                paramDN = listResult["C"].ElementAt(i);
                paramKv = listResult["B"].ElementAt(i);
                
                if (wsRadioButtonList1.SelectedIndex != 3)
                {

                    string I = listResult["I"][0];
                    if (I != "-")
                    {
                        if (customConverterToDouble(I) > 10)
                        {
                            listResult["A"].SetValue("-", 0);
                            listResult["D"].SetValue("-", 0);
                            listResult["I"].SetValue("вариантов нет", 0);
                            listResult["I1"].SetValue("-", 0);
                            listResult["I2"].SetValue("-", 0);
                            listResult["I3"].SetValue("-", 0);
                            listResult["F"].SetValue("-", 0);
                            listResult["G"].SetValue("-", 0);
                        }
                    }
                    
                }
                else
                {


                    int indexNo = listI3.IndexOf("нет");
                    List<string> listA = new List<string>();

                    listA.AddRange(listResult["A"]);

                    if (indexNo != -1)
                    {

                        if (listA.Count - 1 > indexNo + 1)
                        {
                            listA.RemoveRange(indexNo + 1, listA.Count - indexNo - 1);
                            listI.RemoveRange(indexNo + 1, listI.Count - indexNo - 1);
                            listI3.RemoveRange(indexNo + 1, listI3.Count - indexNo - 1);

                            listResult["A"] = listA.ToArray();
                            listResult["I"] = listI.ToArray();
                            listResult["I3"] = listI3.ToArray();
                        }
                    }

                }
            }


        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
        }

        return listResult;
    }

    public void GenerateSteamExel()
    {
        try
        {
            v_input_dict = (Dictionary<int, string>)Session["v_input_dict"];

            v_input_dict[2] = (!String.IsNullOrWhiteSpace(objTextBox1.Text)) ? objTextBox1.Text : "-";


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


            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Content/templates/templateTRVSteam_ver.xlsx")))
            {
                LabelError.Text += "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("~/Content/templates/templateTRVSteam_ver.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.1 / 2.54;
            ws.PrintOptions.BottomMargin = 0.1 / 2.54;
            ws.PrintOptions.LeftMargin = 0.6 / 2.54;
            ws.PrintOptions.RightMargin = 0.6 / 2.54;



            ws.Cells["J2"].Value = v_input_dict[1];
            ws.Cells["B3"].Value = v_input_dict[2];
            ws.Cells["C4"].Value = v_input_dict[6];
            ws.Cells["I4"].Value = v_input_dict[8];
            ws.Cells["C6"].Value = v_input_dict[9];
            ws.Cells["I7"].Value = v_input_dict[66];
            ws.Cells["K7"].Value = v_input_dict[67];
            ws.Cells["I8"].Value = v_input_dict[71];
            ws.Cells["I9"].Value = v_input_dict[34];
            ws.Cells["K9"].Value = v_input_dict[35];
            ws.Cells["I11"].Value = v_input_dict[34];
            ws.Cells["K11"].Value = v_input_dict[35];
            ws.Cells["E11"].Value = v_input_dict[40];
            ws.Cells["E12"].Value = v_input_dict[41];
            ws.Cells["A15"].Value = v_input_dict[42];
            ws.Cells["E15"].Value = v_input_dict[43];
            ws.Cells["I15"].Value = v_input_dict[44];


            string path = HttpContext.Current.Server.MapPath("~/Files/TRV/PDF/" + DateTime.Now.ToString("dd-MM-yyyy"));
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
            v_input_dict = (Dictionary<int, string>) Session["v_input_dict"];

            v_input_dict[2] = (!String.IsNullOrWhiteSpace(objTextBox1.Text)) ? objTextBox1.Text : "-";


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


            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Content/templates/templateTRV_ver.xlsx")))
            {
                LabelError.Text += "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(HttpContext.Current.Server.MapPath("~/Content/templates/templateTRV_ver.xlsx"));

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.1 / 2.54;
            ws.PrintOptions.BottomMargin = 0.1 / 2.54;
            ws.PrintOptions.LeftMargin = 0.6 / 2.54;
            ws.PrintOptions.RightMargin = 0.6 / 2.54;

            ws.Cells["J2"].Value = v_input_dict[1];
            ws.Cells["B3"].Value = v_input_dict[2];
            ws.Cells["C4"].Value = v_input_dict[6];
            ws.Cells["C5"].Value = v_input_dict[8];
            ws.Cells["C7"].Value = v_input_dict[9];
            ws.Cells["J8"].Value = v_input_dict[12];
            ws.Cells["K8"].Value = v_input_dict[13];
            ws.Cells["J9"].Value = v_input_dict[16];
            ws.Cells["K9"].Value = v_input_dict[17];
            ws.Cells["J10"].Value = v_input_dict[18];
            ws.Cells["I11"].Value = v_input_dict[34];
            ws.Cells["K11"].Value = v_input_dict[35];
            ws.Cells["E13"].Value = v_input_dict[40];
            ws.Cells["E14"].Value = v_input_dict[41];
            ws.Cells["A17"].Value = v_input_dict[42];
            ws.Cells["C17"].Value = v_input_dict[43];
            ws.Cells["E17"].Value = v_input_dict[44];
            ws.Cells["F17"].Value = v_input_dict[45];
            ws.Cells["H17"].Value = v_input_dict[46];
            ws.Cells["I17"].Value = v_input_dict[47];
            ws.Cells["J17"].Value = v_input_dict[48];
            ws.Cells["K17"].Value = v_input_dict[49];



            string path = HttpContext.Current.Server.MapPath("~/Files/TRV/PDF/" + DateTime.Now.ToString("dd-MM-yyyy"));
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
            }

        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

    //вспомогательные функции 

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
            
            
            setDNDataset();
            dnDropDownList1.Enabled = true;
            DisableDropDownList(kvsDropDownList1);
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

            fvDropDownList1.Items[1].Enabled = false;
            fvDropDownList1.Items[2].Enabled = false;
            fvDropDownList1.Items[3].Enabled = false;
            fvDropDownList1.Items[4].Enabled = false;
            fvDropDownList1.Items[5].Enabled = false;

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

    protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
    {
        Label53.Visible = true;
        objTextBox1.Enabled = true;
        objTextBox1.Visible = true;
        trvSave.Visible = true;
        resultPanel.Visible = true;
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
                    return;
                }
            }
            if (wsTextBox1.Enabled || wsTextBox2.Enabled)
            {
                if (customConverterToDouble(wsTextBox1.Text) < 5 || customConverterToDouble(wsTextBox1.Text) > 65)
                {
                    wsCustomValidator1.ErrorMessage = "Неверно указано значение концентрации";
                    args.IsValid = false;
                    return;
                }

                if (customConverterToDouble(wsTextBox2.Text) < 0 || customConverterToDouble(wsTextBox2.Text) > 150)
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



    protected void trvCalc_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) { return; }
        try
        {
            resultPanel.Visible = true;
            DisableTextBox(objTextBox1);
            objTextBox1.Enabled = false;
            GridView2.Columns.Clear();
            GridView2.DataSource = null;
            GridView2.DataBind();
            GridView2.SelectedIndex = -1;
            readFile();
            Dictionary<string, double> g_dict = new Dictionary<string, double>();
            v_input_dict.Clear();
            LabelError.Text = "";
            double g = 0;


            g_dict.Add("vmax", 3);

            if (tvRadioButtonList1.SelectedIndex == 0) g_dict.Add("vTMax", 220);
            else g_dict.Add("vTMax", 150);

            if (wsRadioButtonList1.SelectedIndex == 1 || wsRadioButtonList1.SelectedIndex == 2)
            {
                Double p14 = customConverterToDouble(wsTextBox1.Text);
                Double p15 = customConverterToDouble(wsTextBox2.Text);

                g_dict.Add("p14", p14);
                g_dict.Add("p15", p15);

            }

            double checkValue;

                    
            if (this.lpvTextBox21.Enabled)
            {
                checkValue = customConverterToDouble(this.lpvTextBox21.Text);
            }
                    
            if (this.calcvTextBox1.Enabled)
            {
                checkValue = customConverterToDouble(this.calcvTextBox1.Text);

            }
                     
            if (this.wsRadioButtonList1.SelectedIndex == 0)
            {
                Water(GetAvgT(), ref g);
            }
            else if (wsRadioButtonList1.SelectedIndex == 1)
            {
                double p6 = customConverterToDouble(this.wsTextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Etgl(p7, p6, ref g, ref cp);
            }
            else if (wsRadioButtonList1.SelectedIndex == 2)
            {
                double p6 = customConverterToDouble(this.wsTextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                Prgl(p7, p6, ref g, ref cp);
            }

            Double p30 = 0;
                        
                        
            double checkVal= customConverterToDouble(this.fvTextBox1.Text);
                        

            if (!String.IsNullOrWhiteSpace(fvTextBox1.Text))
            {
                if (fvDropDownList1.SelectedIndex > 4)
                {
                    p30 = (customConverterToDouble(fvTextBox1.Text) * arrConvert1[(fvDropDownList1.SelectedIndex - 1), 5]);

                }
                else
                {
                    p30 = (customConverterToDouble(fvTextBox1.Text) * arrConvert1[(fvDropDownList1.SelectedIndex - 1), 5] * (g / 1000));
                }

            }


            g_dict.Add("p30", p30);


            string checkTextBox = "";

            if (lpv5DropDownList1.Enabled)
            {
                checkTextBox = lpv5TextBox1.Text;
            }
            else
            {
                checkTextBox = lpvTextBox21.Text;
            }



            if (wsRadioButtonList1.SelectedIndex == 3)
            {

                double p56, p58 = 0;
                p56 = customConverterToDouble(this.lpv5TextBox1.Text) * arrConvert3[this.lpv5DropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                         
                g_dict.Add("p56", p56);
                g_dict.Add("p58", p58);

                if (lpvRadioButtonList1.SelectedIndex == 0)
                {
                    g_dict.Add("p61", customConverterToDouble(this.lpv5TextBox3.Text));
                }

                double p35 = 0;
                try
                {
                    double p1 = (customConverterToDouble(lpv5TextBox1.Text) * arrConvert3[lpv5DropDownList1.SelectedIndex - 1] / arrConvert3[2]);
                    double p2 = 0;

                    if (lpvRadioButtonList1.SelectedIndex == 0)
                    {
                        p35 = customConverterToDouble(lpv5TextBox3.Text);
                    }
                    else
                    {
                        if (lpvRadioButtonList1.SelectedIndex == 0)
                        {
                            p35 = customConverterToDouble(lpv5TextBox3.Text);
                        }
                        else
                        {
                            p35 = Math.Round(100 * Math.Pow((customConverterToDouble(lpv5TextBox1.Text) * arrConvert3[lpv5DropDownList1.SelectedIndex - 1] / arrConvert3[2]) + 1, 0.25));
                        }
                    }

                }
                catch (Exception) { }

                g_dict.Add("p35", p35);

            }
            else
            {

                double p35 = customConverterToDouble(calcvTextBox2.Text);

                g_dict.Add("p35", p35);

                double p61 = customConverterToDouble(lpvTextBox21.Text) * arrConvert3[lpvDropDownList21.SelectedIndex - 1];
                g_dict.Add("p61", p61);

                double p62 = 0;
                g_dict.Add("p62", p62);

                double p63 = customConverterToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1];
                g_dict.Add("p63", p63);

            }

            if (lpvRadioButtonList1.SelectedIndex == 1)
            {
                lpv5TextBox3.Text = (Math.Round(100 * Math.Pow((customConverterToDouble(lpv5TextBox1.Text) * arrConvert3[lpv5DropDownList1.SelectedIndex - 1] / arrConvert3[2]) + 1, 0.25))).ToString();
            }

            ws2ResultLabel.Visible = true;
            maxp2ResultLabel.Visible = true;
            maxt2ResultLabel.Visible = true;

            labelOptyV.Text = "Оптимальная скорость в выходном сечении клапана: 2-3 м/с для ИТП; 2-5 м/с для ЦТП.";

                                    

            if (wsRadioButtonList1.SelectedIndex == 0)
            {
                ws2ResultLabel.Text = "Рабочая среда - " + wsRadioButtonList1.SelectedValue;
            }
            else if (wsRadioButtonList1.SelectedIndex == 1 || wsRadioButtonList1.SelectedIndex == 2)
            {
                ws2ResultLabel.Text = "Рабочая среда - " + wsRadioButtonList1.SelectedValue + " " + g_dict["p14"] + "%, " + g_dict["p15"] + " °С";
            }

            else
            {
                ws2ResultLabel.Text = "Рабочая среда - " + wsRadioButtonList1.SelectedValue + " " + lpvRadioButtonList1.SelectedValue.ToLower();
                labelOptyV.Text = "Оптимальная скорость в выходном сечении клапана: 40 м/с для насыщенного пара; 60 м/с для перегретого пара.";
            }


            if (tvRadioButtonList1.SelectedIndex == 0)
            {
                if (wsRadioButtonList1.SelectedIndex != 3)
                {
                    this.maxt2ResultLabel.Text = "Максимальная температура - " + ((double.Parse(g_dict["p35"].ToString()) <= 150) ? "150" : "220") + " °С";
                }
                else
                {
                    this.maxt2ResultLabel.Text = "Максимальная температура - 220 °С";
                }
            }
            else
            {
                this.maxt2ResultLabel.Text = "Максимальная температура - 150 °С";
            }

            maxp2ResultLabel.Text = "Максимальное рабочее давление - " + pnRadioButtonList1.SelectedValue + " бар";

            mapInputParametersV(ref v_input_dict);

            Dictionary<string, string[]> gtr = generatedTableV(g_dict);

            if (gtr is null) return;

            string[] titles;

            if (wsRadioButtonList1.SelectedIndex != 3)
            {
                titles = new string[] {
                "Марка регулирующего клапана",
                "Фактические потери давления на полностью открытом клапане при заданном расходе ∆Рф,\nбар\n",
                "Внешний авторитет клапана",
                "Качество регулирования",
                "Скорость в выходном сечении клапана V,\nм/с",
                "Шум, колебательный режим",
                "Предельно допустимый перепад давлений ∆Pпред,\nбар",
                "Кавитация",
                "temp"
                };
            }
            else
            {
                titles = new string[] {
                    "Марка регулирующего клапана",
                    "Скорость в выходном сечении клапана V,\nм/с",
                    "Шум, колебательный режим",
                    "temp"
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
                GridView2.DataSource = dt;
                GridView2.DataBind();

                for (int j = 0; j < gtr.Count(); j++)
                {
                    int index = -1;
                    if (wsRadioButtonList1.SelectedIndex != 3)
                    {
                        switch (gtr.ElementAt(j).Key)
                        {
                            case "A":
                                index = 0;
                                break;
                            case "D":
                                index = 1;
                                break;

                            case "I1":
                                index = 2;
                                break;
                            case "I2":
                                index = 3;
                                break;
                            case "I":
                                index = 4;
                                break;

                            case "I3":
                                index = 5;
                                break;

                            case "F":
                                index = 6;
                                break;
                            case "G":
                                index = 7;
                                break;
                            default :
                                index = 8;
                                break;
                        }
                    }
                    else
                    {
                        switch (gtr.ElementAt(j).Key)
                        {
                            case "A":
                                index = 0;
                                break;
                            case "I":
                                index = 1;
                                break;

                            case "I3":
                                index = 2;
                                break;
                            default:
                                index = 3;
                                break;
                        }
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

            if (GridView2.Rows.Count > 0)
            {
                GridView2.SelectedIndex = 0;
                GridView2_SelectedIndexChanged(GridView2, EventArgs.Empty);
            }


            Label52.Visible = true;
            LabelError.Text = "";
            GridView2.Enabled = true;
            labelOptyV.Visible = true;
            this.GridView2.Visible = true;
            this.GridView2.Height = 250;
            this.trvSave.Visible = true;
            this.trvSave.Enabled = true;

            if (wsRadioButtonList1.SelectedIndex == 3)
            {
                GridView2.CssClass = "table table-result steam-ver";
            }
            else
            {
                GridView2.CssClass = "table table-result trv-ver";
            }

           
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
        }
    }

    protected void trvSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (lpv5TextBox1.Enabled)
            {
                GenerateSteamExel();
            }
            else
            {
                GenerateOtherExel();
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }

    }
}