using GemBox.Spreadsheet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeplosilaWeb.App_Code;

public partial class RDT_ver : System.Web.UI.Page
{
    private const int PressureBeforeValve2x = 25;
    private const int PressureBeforeValve3x = 16;
    private const int MaxT2x = 220;
    private const int MaxT3x = 150;
    private const string JsonKeyName = "JSON_RDT_ver";
    private dynamic dataFromFile;

    public Dictionary<int, string> r_input_dict = new Dictionary<int, string>();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                Logger.InitLogger(); //инициализация - требуется один раз в начале
                AppUtils.readFile(@"Content/data/dataRDT_ver.json", JsonKeyName); //читаем json файл с данными
            }
            else
            {
                if (Session[JsonKeyName] == null)
                {
                    LabelError.Text = "Сессия завершена. Пожалуйста, перезагрузите страницу.";
                    return;
                }
                dataFromFile = Session[JsonKeyName];
                rdtSave.Visible = false;
                resultPanel.Visible = false;
            }
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
        }
    }

    //вспомогательные функции работы с данными

    private void mapInputParametersR(ref Dictionary<int, string> r_in_dict)
    {
        try
        {
            r_in_dict.Add(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            r_in_dict.Add(1, DateTime.Now.ToShortDateString().ToString());
            r_in_dict.Add(2, "-"); // Объект добавляется в диалоговом окне при сохранении

            IEnumerable<RadioButton> ie_rb = null;

            string eorName = "";
            if (eorRadioButton1.Checked)
            {
                eorName = eorRadioButton1.Text;
            }
            else if (eorRadioButton2.Checked)
            {
                eorName = eorRadioButton2.Text;
            }
            else if (eorRadioButton3.Checked)
            {
                eorName = eorRadioButton3.Text;
            }
            else if (eorRadioButton4.Checked)
            {
                eorName = eorRadioButton4.Text;
            }

            r_in_dict.Add(4, eorName);

            r_in_dict.Add(5, "Marka"); // Марка добавляется в диалоговом окне при сохранении

            r_in_dict.Add(6, wsRadioButtonList1.Items[wsRadioButtonList1.SelectedIndex].Text + " " + ((this.wsTextBox1.Enabled) ? (this.wsTextBox1.Text + " %, " + this.wsTextBox2.Text + " °С") : ""));

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

            r_in_dict.Add(25, (this.lp3TextBox1.Enabled) ? this.lp3TextBox1.Text : "-");
            r_in_dict.Add(26, (this.lp3TextBox1.Enabled) ? this.lp3DropDownList1.Text : "-");

            r_in_dict.Add(27, (this.lp3TextBox2.Enabled) ? this.lp3TextBox2.Text : "-");
            r_in_dict.Add(28, (this.lp3TextBox2.Enabled) ? this.lp3DropDownList2.Text : "-");

            r_in_dict.Add(29, (this.lp4TextBox2.Enabled) ? this.lp4TextBox2.Text : "-");
            r_in_dict.Add(30, (this.lp4TextBox2.Enabled) ? this.lp4DropDownList2.Text : "-");

            r_in_dict.Add(31, (this.calcrTextBox1.Enabled) ? this.calcrTextBox1.Text : "-");
            r_in_dict.Add(32, (this.calcrDropDownList1.Enabled) ? this.calcrDropDownList1.Text : "-");

            r_in_dict.Add(33, (this.calcrTextBox2.Text != "") ? this.calcrTextBox2.Text : "-");

            r_in_dict[38] = this.fprTextBox1.Text;
            r_in_dict[381] = this.fprDropDownList1.Text;

            r_in_dict.Add(39, getMaxTemp() + "˚С");

            r_in_dict.Add(40, pnRadioButtonList1.SelectedValue + " бар");

            Session["r_input_dict"] = r_in_dict;
        }
        catch (Exception e)
        {
            Logger.Log.Error(e);
        }
    }

    private Dictionary<string, string[]> generatedTableR(Dictionary<string, double> g_dict)
    {
        try
        {
            /*BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB*/
            double Gpg = 0, g = 0, p2 = 0, V = 0, T1 = 0;
            string Kv = kvsDropDownList1.SelectedValue;
            int DN = Int32.Parse(dnDropDownList1.SelectedValue);

            Dictionary<string, string[]> listResult = new Dictionary<string, string[]>();
            listResult.Add("A", new string[] { });
            listResult.Add("B", new string[] { });
            listResult.Add("F", new string[] { });
            listResult.Add("E", new string[] { });
            listResult.Add("D", new string[] { });
            listResult.Add("G", new string[] { });
            listResult.Add("K", new string[] { });

            Gpg = g_dict["p16"];

            if (wsRadioButtonList1.SelectedIndex == 0)
            {
                MathUtils.Water(GetAvgT(), ref g);
            }
            else if (wsRadioButtonList1.SelectedIndex == 1)
            {
                double p6 = AppUtils.customConverterToDouble(this.wsTextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                MathUtils.Etgl(p7, p6, ref g, ref cp);
            }
            else if (wsRadioButtonList1.SelectedIndex == 2)
            {
                double p6 = AppUtils.customConverterToDouble(this.wsTextBox1.Text);
                double p7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                MathUtils.Prgl(p7, p6, ref g, ref cp);
            }

            
            listResult["B"] = new string[] { Kv};
            int col_C = DN;

            double C = DN;

            string csr = (csrRadioButtonList1.SelectedValue).Split(' ')[0];

            int PN = Int32.Parse(pnRadioButtonList1.SelectedValue);

            double Pf = 1;

            if (eorRadioButton1.Checked)
            {
                listResult["A"] = new string[] { eorRadioButtonList1.SelectedValue + "-" + csr + "-" + DN + "-" + Kv + (PN == 25 ? "-25" : "") };
            }
            else if (eorRadioButton2.Checked)
            {
                listResult["A"] = new string[] { eorRadioButtonList2.SelectedValue + "-" + csr + "-" + DN + "-" + Kv + (PN == 25 ? "-25" : "") };
            }
            else if (eorRadioButton3.Checked)
            {
                listResult["A"] = new string[] { eorRadioButtonList3.SelectedValue + "-" + csr + "-" + DN + "-" + Kv + (PN == 25 ? "-25" : "") };
            }
            else
            {
                listResult["A"] = new string[] { eorRadioButtonList4.SelectedValue + "-" + csr + "-" + DN + "-" + Kv + (PN == 25 ? "-25" : "") };
            }


            V = Gpg / g * Math.Pow((18.8 / C), 2);

            /*FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF*/
            /*GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG*/
            List<string> listD = new List<string>(),
                listF = new List<string>(),
                listG = new List<string>(),
                listE = new List<string>(),
                listK = new List<string>();

            listF.AddRange(listResult["F"]);
            listE.AddRange(listResult["E"]);

            listD.AddRange(listResult["D"]);
            listG.AddRange(listResult["G"]);
            listK.AddRange(listResult["K"]);

            for (int i = 0; i < listResult["A"].Count(); i++)
            {
                if (wsRadioButtonList1.SelectedIndex != 3)
                {
                    /*DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD*/
                    Pf = (Math.Pow(Gpg, 2) * 0.1) / (Math.Pow(double.Parse(listResult["B"].GetValue(i).ToString()), 2) * g);
                    Pf = Math.Round(Pf / 100, 2); /*Перевод с кПа в бар*/
                    //listResult["D"] = new string[] { Pf.ToString() };

                    listD.Add(Pf.ToString());
                }

                listF.Add(Math.Round(V, 2).ToString());

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

                    double t1 = AppUtils.customConverterToDouble(this.calcrTextBox2.Text);
                    Newtonsoft.Json.Linq.JObject max = dataFromFile.table9[dataFromFile.table9.Count - 1];
                    foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9)
                    {
                        if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1))
                        {
                            max = ob;
                        }
                    }
                    ps = MathUtils.getPSbyT(t1);

                    double G = Math.Round((dn * ((AppUtils.customConverterToDouble(this.calcrTextBox1.Text) * MathUtils.getArrConvert3(this.calcrDropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2)) - ps)), 2);
                    listG.Add(G.ToString());

                    string K = "Нет";
                    if (G < Pf) //double.Parse(listResult["D"].GetValue(i).ToString()) )//Pf)
                        K = "Угрожает опасность кавитации";
                    if (eorRadioButton1.Checked && (G < g_dict["p25"]))
                        K = "Угрожает опасность кавитации";
                    if (eorRadioButton2.Checked && (G < (g_dict["p26"] - g_dict["p28"])))
                        K = "Угрожает опасность кавитации";
                    if (eorRadioButton3.Checked && (G < (g_dict["p30"] - g_dict["p32"])))
                        K = "Угрожает опасность кавитации";
                    if (eorRadioButton4.Checked && (G < g_dict["p19"]))
                        K = "Угрожает опасность кавитации";
                    listK.Add(K);
                }
            }

            listResult["D"] = listD.ToArray();
            listResult["G"] = listG.ToArray();
            listResult["K"] = listK.ToArray();
            listResult["F"] = listF.ToArray();
            listResult["E"] = listE.ToArray();

            return listResult;
        }
        catch (Exception e)
        {
            Logger.Log.Error(e);
            return null;
        }
    }

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

    private int getMaxTemp()
    {
        int maxTemp = 0;

        if (eorRadioButtonList1.SelectedIndex == 0 || eorRadioButtonList2.SelectedIndex == 0 || eorRadioButtonList3.SelectedIndex == 0 || eorRadioButtonList4.SelectedIndex == 0)
        {
            maxTemp = 150;
        }
        else
        {
            maxTemp = 220;
        }
        return maxTemp;
    }

    private string getWorkEnv()
    {
        string workEnv = "";

        if (wsRadioButtonList1.SelectedIndex == 1 || wsRadioButtonList1.SelectedIndex == 2)
        {
            workEnv = wsRadioButtonList1.SelectedValue + " " + AppUtils.ConvertPointToComma(wsTextBox1.Text) + "%, " + AppUtils.ConvertPointToComma(wsTextBox2.Text) + " °С";
        }
        else
        {
            workEnv = wsRadioButtonList1.SelectedValue;
        }

        return workEnv;
    }

    public double GetAvgT()
    {
        double avg_T = 0;

        if (this.wsRadioButtonList1.SelectedIndex == 0)
        {
            avg_T = AppUtils.customConverterToDouble(this.calcrTextBox2.Text);
        }
        else
        {
            avg_T = AppUtils.customConverterToDouble(this.wsTextBox2.Text);
        }

        return avg_T;
    }

    //вспомогательные функции

    public void lp1ControlEnable(bool flag)
    {
        lpPane1.Visible = flag;
        AppUtils.dropDownListEnable(lp1DropDownList2, flag);
        AppUtils.DisableTextBox(lp1TextBox2);
        AppUtils.dropDownListEnable(lp1DropDownList3, flag);
        AppUtils.DisableTextBox(lp1TextBox3);
        AppUtils.dropDownListEnable(lp1DropDownList4, flag);
        AppUtils.DisableTextBox(lp1TextBox4);
        AppUtils.DisableTextBox(lp1TextBox5);
    }

    public void lp2ControlEnable(bool flag)
    {
        lpPane2.Visible = flag;
        AppUtils.dropDownListEnable(lp2DropDownList1, flag);
        AppUtils.DisableTextBox(lp2TextBox1);
        AppUtils.dropDownListEnable(lp2DropDownList2, flag);
        AppUtils.DisableTextBox(lp2TextBox2);
    }

    public void lp3ControlEnable(bool flag)
    {
        lpPane3.Visible = flag;
        AppUtils.dropDownListEnable(lp3DropDownList1, flag);
        AppUtils.DisableTextBox(lp3TextBox1);
        AppUtils.dropDownListEnable(lp3DropDownList2, flag);
        AppUtils.DisableTextBox(lp3TextBox2);
    }

    public void lp4ControlEnable(bool flag)
    {
        lpPane4.Visible = flag;
        AppUtils.dropDownListEnable(lp4DropDownList2, flag);
        AppUtils.DisableTextBox(lp4TextBox2);
    }

    private void SetRadioButtonGroupState(RadioButton selectedRadio, RadioButtonList selectedList)
    {
        // Все радио-кнопки
        var allRadioButtons = new[] { eorRadioButton1, eorRadioButton2, eorRadioButton3, eorRadioButton4 };

        // Все списки
        var allRadioLists = new[] { eorRadioButtonList1, eorRadioButtonList2, eorRadioButtonList3, eorRadioButtonList4 };

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
        EnableDNPanel();
        lp1ControlEnable(true);
        lp2ControlEnable(false);
        lp3ControlEnable(false);
        lp4ControlEnable(false);
    }

    protected void eorRadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        SetRadioButtonGroupState(eorRadioButton2, eorRadioButtonList2);
        pnRadioButtonList1.Enabled = true;
        EnableDNPanel();
        lp1ControlEnable(false);
        lp2ControlEnable(true);
        lp3ControlEnable(false);
        lp4ControlEnable(false);
    }

    protected void eorRadioButton3_CheckedChanged(object sender, EventArgs e)
    {
        SetRadioButtonGroupState(eorRadioButton3, eorRadioButtonList3);
        eorRadioButtonList3.SelectedIndex = 0;
        pnRadioButtonList1.Enabled = true;
        EnableDNPanel();
        lp1ControlEnable(false);
        lp2ControlEnable(false);
        lp3ControlEnable(true);
        lp4ControlEnable(false);
    }

    protected void eorRadioButton4_CheckedChanged(object sender, EventArgs e)
    {
        SetRadioButtonGroupState(eorRadioButton4, eorRadioButtonList4);
        eorRadioButtonList4.SelectedIndex = 0;
        pnRadioButtonList1.Enabled = true;
        EnableDNPanel();
        lp1ControlEnable(false);
        lp2ControlEnable(false);
        lp3ControlEnable(false);
        lp4ControlEnable(true);
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

        aaPanel3.Visible = true;
        AppUtils.dropDownListEnable(calcrDropDownList1, true);
        calcrTextBox2.Enabled = true;
        aaPanel5.Visible = true;
        AppUtils.dropDownListEnable(fprDropDownList1, true);
    }

    protected void lp1DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lp1DropDownList2, lp1TextBox2))
        {
            MathUtils.convertArr3((sender as DropDownList), ref lp1TextBox2);
        }
        AppUtils.SaveKeyToSession(lp1DropDownList2.ID, lp1DropDownList2.SelectedIndex);
    }

    protected void lp1DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lp1DropDownList3, lp1TextBox3))
        {
            MathUtils.convertArr3((sender as DropDownList), ref lp1TextBox3);
        }
        AppUtils.SaveKeyToSession(lp1DropDownList3.ID, lp1DropDownList3.SelectedIndex);
    }

    protected void lp1DropDownList4_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lp1DropDownList4, lp1TextBox4))
        {
            MathUtils.convertArr3((sender as DropDownList), ref lp1TextBox4);
        }
        AppUtils.SaveKeyToSession(lp1DropDownList4.ID, lp1DropDownList4.SelectedIndex);
    }

    protected void lp2DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lp2DropDownList1, lp2TextBox1))
        {
            MathUtils.convertArr3((sender as DropDownList), ref lp2TextBox1);
        }
        AppUtils.SaveKeyToSession(lp2DropDownList1.ID, lp2DropDownList1.SelectedIndex);
    }

    protected void lp2DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lp2DropDownList2, lp2TextBox2))
        {
            MathUtils.convertArr3((sender as DropDownList), ref lp2TextBox2);
        }
        AppUtils.SaveKeyToSession(lp2DropDownList2.ID, lp2DropDownList2.SelectedIndex);
    }

    protected void lp3DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lp3DropDownList1, lp3TextBox1))
        {
            MathUtils.convertArr3((sender as DropDownList), ref lp3TextBox1);
        }
        AppUtils.SaveKeyToSession(lp3DropDownList1.ID, lp3DropDownList1.SelectedIndex);
    }

    protected void lp3DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lp3DropDownList2, lp3TextBox2))
        {
            MathUtils.convertArr3((sender as DropDownList), ref lp3TextBox2);
        }
        AppUtils.SaveKeyToSession(lp3DropDownList2.ID, lp3DropDownList2.SelectedIndex);
    }

    protected void lp4DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lp4DropDownList2, lp4TextBox2))
        {
            MathUtils.convertArr3((sender as DropDownList), ref lp4TextBox2);
        }
        AppUtils.SaveKeyToSession(lp4DropDownList2.ID, lp4DropDownList2.SelectedIndex);
    }

    protected void calcrDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(calcrDropDownList1, calcrTextBox1))
        {
            MathUtils.convertArr3((sender as DropDownList), ref calcrTextBox1);
        }
        AppUtils.SaveKeyToSession(calcrDropDownList1.ID, calcrDropDownList1.SelectedIndex);
    }

    protected void fprDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(fprDropDownList1, fprTextBox1))
        {
            MathUtils.convertArrDouble((sender as DropDownList), ref fprTextBox1);
        }
        AppUtils.SaveKeyToSession(fprDropDownList1.ID, fprDropDownList1.SelectedIndex);
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        bool BlockGridSelect = Convert.ToBoolean(Session["BlockGridSelect"] ?? false);
        if (BlockGridSelect) 
        {
            return; 
        }
        else {
            Label53.Visible = true;
            objTextBox1.Enabled = true;
            objTextBox1.Visible = true;
            rdtSave.Visible = true;
            resultPanel.Visible = true;
        }
        
    }

    protected void rdtCalc_Click(object sender, EventArgs e)
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

            Dictionary<string, double> g_dict = new Dictionary<string, double>();
            double g = 0;
            r_input_dict.Clear();

            g_dict.Add("vmax", 3);

            if (this.wsRadioButtonList1.SelectedIndex == 1 || wsRadioButtonList1.SelectedIndex == 2)
            {
                Double p6 = -1;
                Double p7 = -1;

                p6 = AppUtils.customConverterToDouble(this.wsTextBox1.Text);
                g_dict.Add("p6", p6);
                p7 = AppUtils.customConverterToDouble(this.wsTextBox2.Text);
                g_dict.Add("p7", p7);
            }

            if (wsRadioButtonList1.SelectedIndex == 0)
            {
                MathUtils.Water(GetAvgT(), ref g);
            }
            else if (wsRadioButtonList1.SelectedIndex == 1)
            {
                double pl6 = AppUtils.customConverterToDouble(this.wsTextBox1.Text);
                double pl7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                MathUtils.Etgl(pl7, pl6, ref g, ref cp);
            }
            else if (wsRadioButtonList1.SelectedIndex == 2)
            {
                double pl6 = AppUtils.customConverterToDouble(this.wsTextBox1.Text);
                double pl7 = Math.Round(GetAvgT() / 10) * 10;
                double cp = 0;
                MathUtils.Prgl(pl7, pl6, ref g, ref cp);
            }

            double checkVal = AppUtils.customConverterToDouble(this.fprTextBox1.Text);

            Double p16, p25 = 0;

            if (fprDropDownList1.SelectedIndex > 4)
            {
                p16 = (AppUtils.customConverterToDouble(this.fprTextBox1.Text) * MathUtils.getArrConvert1((this.fprDropDownList1.SelectedIndex - 1), 5));
            }
            else
            {
                p16 = (AppUtils.customConverterToDouble(this.fprTextBox1.Text) * MathUtils.getArrConvert1((this.fprDropDownList1.SelectedIndex - 1), 5) * (g / 1000));
            }

            g_dict.Add("p16", p16);

            if (eorRadioButton1.Checked)
            {
                double p19, p21, p23;

                p19 = AppUtils.customConverterToDouble(this.lp1TextBox2.Text) * MathUtils.getArrConvert3(this.lp1DropDownList2.SelectedIndex - 1) / MathUtils.getArrConvert3(2);
                p21 = AppUtils.customConverterToDouble(this.lp1TextBox3.Text) * MathUtils.getArrConvert3(this.lp1DropDownList3.SelectedIndex - 1) / MathUtils.getArrConvert3(2);
                p23 = AppUtils.customConverterToDouble(this.lp1TextBox4.Text) * MathUtils.getArrConvert3(this.lp1DropDownList4.SelectedIndex - 1) / MathUtils.getArrConvert3(2);

                g_dict.Add("p19", p19);
                g_dict.Add("p21", p21);
                g_dict.Add("p23", p23);

                p25 = Math.Round(p21 - p23 - p19, 2);
                g_dict.Add("p25", p25);
                lp1TextBox5.Text = AppUtils.ConvertCommaToPoint(p25.ToString());
            }
            else if (eorRadioButton2.Checked)
            {
                double p26, p28;
                p26 = AppUtils.customConverterToDouble(this.lp2TextBox1.Text) * MathUtils.getArrConvert3(this.lp2DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2);
                p28 = AppUtils.customConverterToDouble(this.lp2TextBox2.Text) * MathUtils.getArrConvert3(this.lp2DropDownList2.SelectedIndex - 1) / MathUtils.getArrConvert3(2);

                g_dict.Add("p26", p26);
                g_dict.Add("p28", p28);
            }
            else if (eorRadioButton3.Checked)
            {
                double p30, p32;
                p30 = AppUtils.customConverterToDouble(this.lp3TextBox1.Text) * MathUtils.getArrConvert3(this.lp3DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2);
                p32 = AppUtils.customConverterToDouble(this.lp3TextBox2.Text) * MathUtils.getArrConvert3(this.lp3DropDownList2.SelectedIndex - 1) / MathUtils.getArrConvert3(2);

                g_dict.Add("p30", p30);
                g_dict.Add("p32", p32);
            }
            else if (eorRadioButton4.Checked)
            {
                double p19;
                p19 = AppUtils.customConverterToDouble(this.lp4TextBox2.Text) * MathUtils.getArrConvert3(this.lp4DropDownList2.SelectedIndex - 1) / MathUtils.getArrConvert3(2);

                g_dict.Add("p19", p19);
            }

            double ptemp = AppUtils.customConverterToDouble(this.calcrTextBox1.Text);
            double p35 = 0;
            p35 = AppUtils.customConverterToDouble(this.calcrTextBox2.Text);

            g_dict.Add("p35", p35);

            ws1ResultLabel.Text = "Рабочая среда - " + getWorkEnv();
            this.maxt1ResultLabel.Text = "Максимальная температура - " + getMaxTemp() + " °С";
            maxp1ResultLabel.Text = "Максимальное рабочее давление - " + pnRadioButtonList1.SelectedValue + " бар";
            labelOptyV.Text = "Оптимальная скорость в выходном сечении регулятора: 2 - 3 м / с для ИТП; 2 - 5 м / с для ЦТП.";

            mapInputParametersR(ref r_input_dict);

            Dictionary<string, string[]> gtr = this.generatedTableR(g_dict);

            if (gtr is null) return;

            string[] titles;

            if (wsRadioButtonList1.SelectedIndex != 3)
            {
                titles = dataFromFile.ResultTableRdtVer.ColumnName.ToObject<string[]>();
            }
            else
            {
                titles = dataFromFile.ResultTableRdtSteamVer.ColumnName.ToObject<string[]>();
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

            double ppy = AppUtils.customConverterToDouble(gtr["D"].FirstOrDefault());

            for (int i = 0; i < maxCount; i++)
            {
                dr = dt.NewRow();

                dt.Rows.Add(dr);
                GridView1.DataSource = dt;
                GridView1.DataBind();

                for (int j = 0; j < gtr.Count(); j++)
                {
                    int index = -1;
                    if (wsRadioButtonList1.SelectedIndex != 3)
                    {
                        index = (dataFromFile.ResultTableRdtVer.ColumnIndex[gtr.ElementAt(j).Key] != null ? dataFromFile.ResultTableRdtVer.ColumnIndex[gtr.ElementAt(j).Key] : 6); //7 столбцов, последний скрыт
                    }
                    else
                    {
                        index = (dataFromFile.ResultTableRdtSteamVer.ColumnIndex[gtr.ElementAt(j).Key] != null ? dataFromFile.ResultTableRdtSteamVer.ColumnIndex[gtr.ElementAt(j).Key] : 4);
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

            LabelError.Text = "";
            Label52.Visible = true;
            maxp1ResultLabel.Visible = true;
            maxt1ResultLabel.Visible = true;
            ws1ResultLabel.Visible = true;

            bool mtmFlag = false;
            string mtmErrorMsg = "";
            if (eorRadioButton1.Checked)
            {
                mtmFlag = ppy > AppUtils.customConverterToDouble(lp1TextBox5.Text);
                mtmErrorMsg = "Фактические потери давления на полностью открытом регуляторе при заданном расходе ∆Рф больше максимального располагаемого перепада давления на регуляторе ΔPрд(max), указанный регулятор не подойдет";

            } 
            else if (eorRadioButton2.Checked) 
            {
                mtmFlag = ppy > (MathUtils.convertArrToBar(lp2DropDownList1, lp2TextBox1) - MathUtils.convertArrToBar(lp2DropDownList2, lp2TextBox2));
                mtmErrorMsg = "Фактические потери давления на полностью открытом регуляторе при заданном расходе ∆Рф больше максимального располагаемого перепада давления на регуляторе ΔPрд(max) = P'1 - Р(треб), указанный регулятор не подойдет";
            }
            else if (eorRadioButton3.Checked)
            {
                mtmFlag = ppy > (MathUtils.convertArrToBar(lp3DropDownList1, lp3TextBox1) - MathUtils.convertArrToBar(lp3DropDownList2, lp3TextBox2));
                mtmErrorMsg = "Фактические потери давления на полностью открытом регуляторе при заданном расходе ∆Рф больше максимального располагаемого перепада давления на регуляторе ΔPрд(max) = Р(треб) - Р'2, указанный регулятор не подойдет";
            }
            else if (eorRadioButton4.Checked)
            {
                mtmFlag = ppy > MathUtils.convertArrToBar(lp4DropDownList2, lp4TextBox2);
                mtmErrorMsg = "Фактические потери давления на полностью открытом регуляторе при заданном расходе ∆Рф больше максимального располагаемого перепада давления на регуляторе ΔPрд(max) = ΔPру, указанный регулятор не подойдет";
            }

            if(mtmFlag)
            {
                mtmLabelError.Text = mtmErrorMsg;
                mtmLabelError.Enabled = true;
                rdtSave.Visible = false;
                rdtSave.Enabled = false;
                Session["BlockGridSelect"] = true;
            } 
            else
            {
                if (GridView1.Rows.Count > 0)
                {
                    GridView1.SelectedIndex = 0;
                    GridView1_SelectedIndexChanged(GridView1, EventArgs.Empty);
                }
                rdtSave.Visible = true;
                rdtSave.Enabled = true;
                mtmLabelError.Text = "";
                mtmLabelError.Enabled = false;
                Session["BlockGridSelect"] = false;
            }

            labelOptyV.Visible = true;
            GridView1.Enabled = true;
            GridView1.Visible = true;
            GridView1.Height = 250;
            
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
        }
    }

    protected void rdtSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (eorRadioButton1.Checked)
            {
                GenerateOtherExel();
            } 
            else if (eorRadioButton2.Checked)
            {
                GeneratePosleExel();
            }
            else if (eorRadioButton3.Checked)
            {
                GenerateDoExel();
            }
            else if (eorRadioButton4.Checked)
            {
                GeneratePerepyskaExel();
            }    
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);
        }
    }

    //Валидаторы элементор управления
    protected void eorCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!eorRadioButton1.Checked && !eorRadioButton2.Checked && !eorRadioButton3.Checked && !eorRadioButton4.Checked)
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
        if (eorRadioButton3.Checked && eorRadioButtonList3.SelectedIndex == -1)
        {
            eorCustomValidator1.ErrorMessage = "Выберите необходимое значение";
            args.IsValid = false;
            return;
        }
        if (eorRadioButton4.Checked && eorRadioButtonList4.SelectedIndex == -1)
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

    protected void CustomValidator8_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (wsCustomValidator1.IsValid)
        {
            if (lp1DropDownList2.Enabled)
            {
                if (lp1TextBox2.Enabled == false || AppUtils.checkTextBoxEmpty(lp1TextBox2))
                {
                    CustomValidator8.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(lp1TextBox2.Text) <= 0)
                {
                    CustomValidator8.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }

                double csrMin = dataFromFile.ControllerSettingRange.SettingsParam[csrRadioButtonList1.SelectedValue.Split(' ')[0]].min;
                double csrMax = dataFromFile.ControllerSettingRange.SettingsParam[csrRadioButtonList1.SelectedValue.Split(' ')[0]].max;

                if (!(csrMin <= MathUtils.convertArrToBar(lp1DropDownList2, lp1TextBox2) && MathUtils.convertArrToBar(lp1DropDownList2, lp1TextBox2) <= csrMax))
                {
                    CustomValidator8.ErrorMessage = "Давление настройки не попадает в выбранный диапазон настройки. Указанный регулятор не подойдет";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            CustomValidator8.ErrorMessage = "";
            return;
        }
    }

    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator8.IsValid)
        {
            if (lp1DropDownList3.Enabled)
            {
                if (lp1TextBox3.Enabled == false || AppUtils.checkTextBoxEmpty(lp1TextBox3))
                {
                    CustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(lp1TextBox3.Text) <= 0)
                {
                    CustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
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
                if (lp1TextBox4.Enabled == false || AppUtils.checkTextBoxEmpty(lp1TextBox4))
                {
                    CustomValidator2.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(lp1TextBox4.Text) <= 0)
                {
                    CustomValidator2.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (MathUtils.convertArrToBar(lp1DropDownList4, lp1TextBox4) >= MathUtils.convertArrToBar(lp1DropDownList3, lp1TextBox3))
                {
                    CustomValidator2.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }

                if (MathUtils.convertArrToBar(lp1DropDownList2, lp1TextBox2) >= (MathUtils.convertArrToBar(lp1DropDownList3, lp1TextBox3) - MathUtils.convertArrToBar(lp1DropDownList4, lp1TextBox4)))
                {
                    CustomValidator8.ErrorMessage = "Потери давления на регулируемом участке превышают располагаемый перепад давлений на вводе";
                    CustomValidator8.IsValid = false;
                    return;
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
            if (lp2TextBox1.Enabled == false || AppUtils.checkTextBoxEmpty(lp2TextBox1))
            {
                CustomValidator3.ErrorMessage = "Необходимо заполнить поле";
                args.IsValid = false;
                return;
            }
            if (AppUtils.customConverterToDouble(lp2TextBox1.Text) <= 0)
            {
                CustomValidator3.ErrorMessage = "Неверно указано значение давления";
                args.IsValid = false;
                return;
            }
            if (MathUtils.convertArrToBar(lp2DropDownList1, lp2TextBox1) > AppUtils.customConverterToDouble(pnRadioButtonList1.SelectedValue))
            {
                CustomValidator3.ErrorMessage = "Давление перед регулятором больше номинального давления регулятора. Указанный регулятор не подойдет";
                args.IsValid = false;
                return;
            }
        }
    }

    protected void CustomValidator4_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator3.IsValid)
        {
            if (lp2DropDownList2.Enabled)
            {
                if (lp2TextBox2.Enabled == false || AppUtils.checkTextBoxEmpty(lp2TextBox2))
                {
                    CustomValidator4.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(lp2TextBox2.Text) <= 0)
                {
                    CustomValidator4.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (MathUtils.convertArrToBar(lp2DropDownList2, lp2TextBox2) >= MathUtils.convertArrToBar(lp2DropDownList1, lp2TextBox1))
                {
                    CustomValidator4.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }

                double csrMin = dataFromFile.ControllerSettingRange.SettingsParam[csrRadioButtonList1.SelectedValue.Split(' ')[0]].min;
                double csrMax = dataFromFile.ControllerSettingRange.SettingsParam[csrRadioButtonList1.SelectedValue.Split(' ')[0]].max;

                if (!(csrMin <= MathUtils.convertArrToBar(lp2DropDownList2, lp2TextBox2) && MathUtils.convertArrToBar(lp2DropDownList2, lp2TextBox2) <= csrMax))
                {
                    CustomValidator4.ErrorMessage = "Давление настройки не попадает в выбранный диапазон настройки. Указанный регулятор не подойдет";
                    args.IsValid = false;
                    return;
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
            if (lp3TextBox1.Enabled == false || AppUtils.checkTextBoxEmpty(lp3TextBox1))
            {
                CustomValidator5.ErrorMessage = "Необходимо заполнить поле";
                args.IsValid = false;
                return;
            }
            if (AppUtils.customConverterToDouble(lp3TextBox1.Text) <= 0)
            {
                CustomValidator5.ErrorMessage = "Неверно указано значение давления";
                args.IsValid = false;
                return;
            }

            if (MathUtils.convertArrToBar(lp3DropDownList1, lp3TextBox1) > AppUtils.customConverterToDouble(pnRadioButtonList1.SelectedValue))
            {
                CustomValidator5.ErrorMessage = "Давление перед регулятором больше номинального давления регулятора. Указанный регулятор не подойдет";
                args.IsValid = false;
                return;
            }
            double csrMin = dataFromFile.ControllerSettingRange.SettingsParam[csrRadioButtonList1.SelectedValue.Split(' ')[0]].min;
            double csrMax = dataFromFile.ControllerSettingRange.SettingsParam[csrRadioButtonList1.SelectedValue.Split(' ')[0]].max;

            if (!(csrMin <= MathUtils.convertArrToBar(lp3DropDownList1, lp3TextBox1) && MathUtils.convertArrToBar(lp3DropDownList1, lp3TextBox1) <= csrMax))
            {
                CustomValidator5.ErrorMessage = "Давление настройки не попадает в выбранный диапазон настройки. Указанный регулятор не подойдет";
                args.IsValid = false;
                return;
            }
        }
    }

    protected void CustomValidator6_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator5.IsValid)
        {
            if (lp3DropDownList2.Enabled)
            {
                if (lp3TextBox2.Enabled == false || AppUtils.checkTextBoxEmpty(lp3TextBox2))
                {
                    CustomValidator6.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(lp3TextBox2.Text) <= 0)
                {
                    CustomValidator6.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (MathUtils.convertArrToBar(lp3DropDownList2, lp3TextBox2) >= MathUtils.convertArrToBar(lp3DropDownList1, lp3TextBox1))
                {
                    CustomValidator6.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
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
            if (lp4TextBox2.Enabled == false || AppUtils.checkTextBoxEmpty(lp4TextBox2))
            {
                CustomValidator7.ErrorMessage = "Необходимо заполнить поле";
                args.IsValid = false;
                return;
            }
            if (AppUtils.customConverterToDouble(lp4TextBox2.Text) <= 0)
            {
                CustomValidator7.ErrorMessage = "Неверно указано значение давления";
                args.IsValid = false;
                return;
            }

            double csrMin = dataFromFile.ControllerSettingRange.SettingsParam[csrRadioButtonList1.SelectedValue.Split(' ')[0]].min;
            double csrMax = dataFromFile.ControllerSettingRange.SettingsParam[csrRadioButtonList1.SelectedValue.Split(' ')[0]].max;

            if (!(csrMin <= MathUtils.convertArrToBar(lp4DropDownList2, lp4TextBox2) && MathUtils.convertArrToBar(lp4DropDownList2, lp4TextBox2) <= csrMax))
            {
                CustomValidator7.ErrorMessage = "Давление настройки не попадает в выбранный диапазон настройки. Указанный регулятор не подойдет";
                args.IsValid = false;
                return;
            }
        }

        return;
    }

    protected void calcrCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CustomValidator2.IsValid && CustomValidator4.IsValid && CustomValidator6.IsValid && CustomValidator7.IsValid)
        {
            if (calcrDropDownList1.Enabled)
            {
                if (calcrTextBox1.Enabled == false || AppUtils.checkTextBoxEmpty(calcrTextBox1))
                {
                    calcrCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(calcrTextBox1.Text) <= 0)
                {
                    calcrCustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (MathUtils.convertArrToBar(calcrDropDownList1, calcrTextBox1) > AppUtils.customConverterToDouble(pnRadioButtonList1.SelectedValue))
                {
                    calcrCustomValidator1.ErrorMessage = "Давление перед регулятором больше номинального давления регулятора. Указанный регулятор не подойдет";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            calcrCustomValidator1.ErrorMessage = "";
            return;
        }
    }

    protected void calcrCustomValidator2_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (calcrCustomValidator1.IsValid)
        {
            if (calcrTextBox2.Enabled != false)
            {
                if (calcrTextBox2.Enabled == false || AppUtils.checkTextBoxEmpty(calcrTextBox2))
                {
                    calcrCustomValidator2.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(calcrTextBox2.Text) <= 0)
                {
                    calcrCustomValidator2.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return;
                }

                if ((eorRadioButtonList1.SelectedIndex == 0 || eorRadioButtonList2.SelectedIndex == 0 || eorRadioButtonList3.SelectedIndex == 0 || eorRadioButtonList4.SelectedIndex == 0) 
                    && AppUtils.customConverterToDouble(this.calcrTextBox2.Text) > MaxT3x)
                {
                    calcrCustomValidator2.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного регулятора. Указанный регулятор не подойдет";
                    args.IsValid = false;
                    return;
                }

                if ((eorRadioButtonList1.SelectedIndex == 1 || eorRadioButtonList2.SelectedIndex == 1) && AppUtils.customConverterToDouble(this.calcrTextBox2.Text) > MaxT2x)
                {
                    calcrCustomValidator2.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного регулятора. Указанный регулятор не подойдет";
                    args.IsValid = false;
                    return;
                }

                if (((AppUtils.customConverterToDouble(this.calcrTextBox1.Text) * MathUtils.getArrConvert3(this.calcrDropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2)) - MathUtils.getPSbyT(AppUtils.customConverterToDouble(this.calcrTextBox2.Text))) <= 0)
                {
                    calcrCustomValidator2.ErrorMessage = "Указанная температура выше температуры парообразования. При указанной температуре в трубопроводе движется пар";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            calcrCustomValidator2.ErrorMessage = "";
            return;
        }
    }

    protected void fprCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (calcrCustomValidator2.IsValid)
        {
            if (fprDropDownList1.Enabled)
            {
                if (fprTextBox1.Enabled == false || AppUtils.checkTextBoxEmpty(fprTextBox1))
                {
                    fprCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(fprTextBox1.Text) <= 0)
                {
                    fprCustomValidator1.ErrorMessage = "Неверно указано значение расхода";
                    args.IsValid = false;
                    return;
                }
            }
        }
        else
        {
            args.IsValid = false;
            fprCustomValidator1.ErrorMessage = "";
            return;
        }
    }

    //Вывод в файл
    public void GenerateOtherExel()
    {
        try
        {
            r_input_dict = (Dictionary<int, string>)Session["r_input_dict"];

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = AppUtils.ConvertCommaToPoint(r_input_dict[41]);
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT,RDT-H_ver.xlsx");
            if (!File.Exists(templatePath))
            {
                LabelError.Text += "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(templatePath);
            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            AppUtils.SetCellValue(ws, "J2", 1, r_input_dict);
            AppUtils.SetCellValue(ws, "C3", 2, r_input_dict);
            AppUtils.SetCellValue(ws, "C4", 4, r_input_dict);
            AppUtils.SetCellValue(ws, "C5", 5, r_input_dict);
            AppUtils.SetCellValue(ws, "C8", 6, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I11", 9, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K11", 10, r_input_dict);
            AppUtils.SetCellValue(ws, "I12", 11, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K12", 12, r_input_dict);
            AppUtils.SetCellValue(ws, "I13", 13, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K13", 14, r_input_dict);
            AppUtils.SetCellValue(ws, "I14", 15, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I16", 31, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K16", 32, r_input_dict);
            AppUtils.SetCellValue(ws, "I17", 33, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I19", 38, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K19", 381, r_input_dict);

            AppUtils.SetCellValue(ws, "E22", 39, r_input_dict);
            AppUtils.SetCellValue(ws, "E23", 40, r_input_dict);

            AppUtils.SetCellValue(ws, "A26", 41, r_input_dict);
            AppUtils.SetCellValue(ws, "C26", 42, r_input_dict, true);
            AppUtils.SetCellValue(ws, "E26", 43, r_input_dict, true);
            AppUtils.SetCellValue(ws, "G26", 44, r_input_dict);
            AppUtils.SetCellValue(ws, "I26", 45, r_input_dict, true);
            AppUtils.SetCellValue(ws, "J26", 46, r_input_dict);


            string saveDirectory = HttpContext.Current.Server.MapPath($"~/Files/RDT_ver/PDF/{DateTime.Now:dd-MM-yyyy}");
            AppUtils.EnsureDirectoryExists(saveDirectory);

            string uniqueFileName = AppUtils.GenerateUniqueFileName(saveDirectory, fileName);
            string fullPath = Path.Combine(saveDirectory, uniqueFileName + ".pdf");

            ef.Save(fullPath);
            AppUtils.WaitDownload(50);

            AppUtils.ServeFile(Response, fullPath);
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

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = AppUtils.ConvertCommaToPoint(r_input_dict[41]);
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-P,RDT-PH_ver.xlsx");
            if (!File.Exists(templatePath))
            {
                LabelError.Text += "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(templatePath);
            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            AppUtils.SetCellValue(ws, "J2", 1, r_input_dict);
            AppUtils.SetCellValue(ws, "C3", 2, r_input_dict);
            AppUtils.SetCellValue(ws, "C4", 4, r_input_dict);
            AppUtils.SetCellValue(ws, "C5", 5, r_input_dict);
            AppUtils.SetCellValue(ws, "C8", 6, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I10", 16, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K10", 17, r_input_dict);
            AppUtils.SetCellValue(ws, "I11", 18, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K11", 19, r_input_dict);

            AppUtils.SetCellValue(ws, "I13", 31, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K13", 32, r_input_dict);
            AppUtils.SetCellValue(ws, "I14", 33, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I16", 38, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K16", 381, r_input_dict); // не входит в список — без true

            AppUtils.SetCellValue(ws, "E19", 39, r_input_dict);
            AppUtils.SetCellValue(ws, "E20", 40, r_input_dict);

            AppUtils.SetCellValue(ws, "A23", 41, r_input_dict);
            AppUtils.SetCellValue(ws, "C23", 42, r_input_dict, true);
            AppUtils.SetCellValue(ws, "E23", 43, r_input_dict, true);
            AppUtils.SetCellValue(ws, "G23", 44, r_input_dict);
            AppUtils.SetCellValue(ws, "I23", 45, r_input_dict, true);
            AppUtils.SetCellValue(ws, "J23", 46, r_input_dict);

            string saveDirectory = HttpContext.Current.Server.MapPath($"~/Files/RDT_ver/PDF/{DateTime.Now:dd-MM-yyyy}");
            AppUtils.EnsureDirectoryExists(saveDirectory);

            string uniqueFileName = AppUtils.GenerateUniqueFileName(saveDirectory, fileName);
            string fullPath = Path.Combine(saveDirectory, uniqueFileName + ".pdf");

            ef.Save(fullPath);
            AppUtils.WaitDownload(50);

            AppUtils.ServeFile(Response, fullPath);
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

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = AppUtils.ConvertCommaToPoint(r_input_dict[41]);
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-S_ver.xlsx");
            if (!File.Exists(templatePath))
            {
                LabelError.Text += "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(templatePath);
            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            AppUtils.SetCellValue(ws, "J2", 1, r_input_dict);
            AppUtils.SetCellValue(ws, "C3", 2, r_input_dict);
            AppUtils.SetCellValue(ws, "C4", 4, r_input_dict);
            AppUtils.SetCellValue(ws, "C5", 5, r_input_dict);
            AppUtils.SetCellValue(ws, "C8", 6, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I10", 25, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K10", 26, r_input_dict);
            AppUtils.SetCellValue(ws, "I11", 27, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K11", 28, r_input_dict);

            AppUtils.SetCellValue(ws, "I13", 31, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K13", 32, r_input_dict);
            AppUtils.SetCellValue(ws, "I14", 33, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I16", 38, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K16", 381, r_input_dict); // не входит в список — без true

            AppUtils.SetCellValue(ws, "E19", 39, r_input_dict);
            AppUtils.SetCellValue(ws, "E20", 40, r_input_dict);

            AppUtils.SetCellValue(ws, "A23", 41, r_input_dict);
            AppUtils.SetCellValue(ws, "C23", 42, r_input_dict, true);
            AppUtils.SetCellValue(ws, "E23", 43, r_input_dict, true);
            AppUtils.SetCellValue(ws, "G23", 44, r_input_dict);
            AppUtils.SetCellValue(ws, "I23", 45, r_input_dict, true);
            AppUtils.SetCellValue(ws, "J23", 46, r_input_dict);

            string saveDirectory = HttpContext.Current.Server.MapPath($"~/Files/RDT_ver/PDF/{DateTime.Now:dd-MM-yyyy}");
            AppUtils.EnsureDirectoryExists(saveDirectory);

            string uniqueFileName = AppUtils.GenerateUniqueFileName(saveDirectory, fileName);
            string fullPath = Path.Combine(saveDirectory, uniqueFileName + ".pdf");

            ef.Save(fullPath);
            AppUtils.WaitDownload(50);

            AppUtils.ServeFile(Response, fullPath);
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

            if (!String.IsNullOrWhiteSpace(objTextBox1.Text))
            {
                r_input_dict[2] = objTextBox1.Text;
            }
            else
            {
                r_input_dict[2] = "-";
            }

            int pos = 41;

            for (int r = 1; r < GridView1.SelectedRow.Cells.Count; r++)
            {
                r_input_dict[pos] = GridView1.SelectedRow.Cells[r].Text;
                pos++;

            }

            r_input_dict[5] = r_input_dict[41];
            string fileName = AppUtils.ConvertCommaToPoint(r_input_dict[41]);
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/templates/templateRDT-B_ver.xlsx");
            if (!File.Exists(templatePath))
            {
                LabelError.Text += "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(templatePath);
            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.2 / 2.54;
            ws.PrintOptions.BottomMargin = 0.2 / 2.54;
            ws.PrintOptions.LeftMargin = 1 / 2.54;
            ws.PrintOptions.RightMargin = 1 / 2.54;

            AppUtils.SetCellValue(ws, "J2", 1, r_input_dict);
            AppUtils.SetCellValue(ws, "C3", 2, r_input_dict);
            AppUtils.SetCellValue(ws, "C4", 4, r_input_dict);
            AppUtils.SetCellValue(ws, "C5", 5, r_input_dict);
            AppUtils.SetCellValue(ws, "C8", 6, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I10", 29, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K10", 30, r_input_dict);

            AppUtils.SetCellValue(ws, "I12", 31, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K12", 32, r_input_dict);
            AppUtils.SetCellValue(ws, "I13", 33, r_input_dict, true);

            AppUtils.SetCellValue(ws, "I15", 38, r_input_dict, true);
            AppUtils.SetCellValue(ws, "K15", 381, r_input_dict);

            AppUtils.SetCellValue(ws, "E18", 39, r_input_dict);
            AppUtils.SetCellValue(ws, "E19", 40, r_input_dict);

            AppUtils.SetCellValue(ws, "A22", 41, r_input_dict);
            AppUtils.SetCellValue(ws, "C22", 42, r_input_dict, true);
            AppUtils.SetCellValue(ws, "E22", 43, r_input_dict, true);
            AppUtils.SetCellValue(ws, "G22", 44, r_input_dict);
            AppUtils.SetCellValue(ws, "I22", 45, r_input_dict, true);
            AppUtils.SetCellValue(ws, "J22", 46, r_input_dict);


            string saveDirectory = HttpContext.Current.Server.MapPath($"~/Files/RDT_ver/PDF/{DateTime.Now:dd-MM-yyyy}");
            AppUtils.EnsureDirectoryExists(saveDirectory);

            string uniqueFileName = AppUtils.GenerateUniqueFileName(saveDirectory, fileName);
            string fullPath = Path.Combine(saveDirectory, uniqueFileName + ".pdf");

            ef.Save(fullPath);
            AppUtils.WaitDownload(50);

            AppUtils.ServeFile(Response, fullPath);
        }
        catch (Exception er)
        {
            Logger.Log.Error(er);

        }
    }

}