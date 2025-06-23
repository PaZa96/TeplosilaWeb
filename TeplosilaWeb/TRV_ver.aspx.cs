using GemBox.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeplosilaWeb.App_Code;

public partial class TRV_ver : System.Web.UI.Page
{
    private const int PressureBeforeValve2x = 25;
    private const int PressureBeforeValve3x = 16;
    private const int MaxT2x = 220;
    private const int MaxT3x = 150;
    private dynamic dataFromFile;

    public Dictionary<int, string> v_input_dict = new Dictionary<int, string>();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
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

    public double math_30_cp()
    {
        double cp = 0;
        double rr = 0;

        if (wsRadioButtonList1.SelectedIndex == 0)
        {
            MathUtils.Water(GetAvgT(), ref rr);
            cp = MathUtils.WaterCP(GetAvgT()); // 4.187;
        }
        else if (wsRadioButtonList1.SelectedIndex == 1)
        {
            MathUtils.Etgl(GetAvgT(), AppUtils.customConverterToDouble(this.wsTextBox1.Text), ref rr, ref cp);
        }
        else if (wsRadioButtonList1.SelectedIndex == 2)
        {
            MathUtils.Prgl(GetAvgT(), AppUtils.customConverterToDouble(this.wsTextBox1.Text), ref rr, ref cp);
        }
        return cp / 1000;
    }

    public double GetAvgT()
    {
        double avg_T = 0;

        if (this.wsRadioButtonList1.SelectedIndex == 0)
        {
            avg_T = AppUtils.customConverterToDouble(this.calcvTextBox2.Text);
        }
        else
        {
            avg_T = AppUtils.customConverterToDouble(this.wsTextBox2.Text);
        }

        return avg_T;
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

    private void getT1Steam()
    {
        if (lpvRadioButtonList1.SelectedIndex == 1)
        {
            double T1 = Math.Round(100 * Math.Pow((AppUtils.customConverterToDouble(lpv5TextBox1.Text) * MathUtils.getArrConvert3(lpv5DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2)) + 1, 0.25));
            lpv5TextBox3.Text = T1.ToString();
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
                v_in_dict.Add(9, wsRadioButtonList1.Items[wsRadioButtonList1.SelectedIndex].Text + " " + ((this.wsTextBox1.Enabled) ? (AppUtils.customConverterToDouble(this.wsTextBox1.Text) + " %, " + AppUtils.customConverterToDouble(this.wsTextBox2.Text) + " °С") : ""));
            }

            v_in_dict.Add(12, (this.lpvTextBox21.Enabled) ? this.lpvTextBox21.Text : "-");
            v_in_dict.Add(13, (this.lpvTextBox21.Enabled) ? this.lpvDropDownList21.Text : "-");

            v_in_dict.Add(16, (this.calcvTextBox1.Enabled) ? this.calcvTextBox1.Text : "-");
            v_in_dict.Add(17, (this.calcvTextBox1.Enabled) ? this.calcvDropDownList1.Text : "-");

            v_in_dict.Add(18, (this.calcvTextBox2.Text != "") ? this.calcvTextBox2.Text : "-");

            v_in_dict[34] = this.fvTextBox1.Text;
            v_in_dict[35] = this.fvDropDownList1.Text;

            if (tvRadioButtonList1.SelectedIndex == 1)
            {
                v_in_dict[40] = "220˚С";
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
        listResult.Add("D", new string[] { });
        listResult.Add("I", new string[] { });
        listResult.Add("I3", new string[] { });

        if (wsRadioButtonList1.SelectedIndex != 3)
        {
            listResult.Add("I1", new string[] { });
            listResult.Add("I2", new string[] { });
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

            if (wsRadioButtonList1.SelectedIndex == 3)
            {
                p1 = (AppUtils.customConverterToDouble(lpv5TextBox1.Text) * MathUtils.getArrConvert3(lpv5DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2));

                if (Math.Pow((p1 - 1), 2) - 4 * ((T1 + 273) / Math.Pow((Kv * 461 / Gkl), 2) - p1) > 0)
                {
                    p2 = 0.5 * ((p1 - 1) + Math.Sqrt(Math.Pow((p1 - 1), 2) - 4 * ((T1 + 273) / Math.Pow((Kv * 461 / Gkl), 2) - p1)));

                    if ((p1 - p2) > (0.5 * (p1 + 1)))
                    {
                        p2 = 0.5 * p1 - 0.5;
                    }
                }
                else
                {
                    p2 = 0.5 * p1 - 0.5;
                }

                T1 = AppUtils.customConverterToDouble(lpv5TextBox3.Text);
            }

            double col_B = Kv = AppUtils.customConverterToDouble(kvsDropDownList1.SelectedValue);
            listResult["B"] = new string[] { Kv.ToString() };

            int col_C = DN = Int32.Parse(dnDropDownList1.SelectedValue);

            double C = AppUtils.customConverterToDouble(dnDropDownList1.SelectedValue);

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
                    V = Gkl / g * Math.Pow((18.8 / C), 2);
                    listI.Add(Math.Round(V, 2).ToString());

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

                        double t1 = AppUtils.customConverterToDouble(this.calcvTextBox2.Text);
                        Newtonsoft.Json.Linq.JObject max = dataFromFile.table9v[dataFromFile.table9v.Count - 1];
                        foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9v)
                        {
                            if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1))
                            {
                                max = ob;
                            }
                        }
                        ps = MathUtils.getPSbyT(t1);

                        double F = Math.Round((dn * ((AppUtils.customConverterToDouble(this.calcvTextBox1.Text) * MathUtils.getArrConvert3(this.calcvDropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2)) - ps)), 2);
                        listF.Add(F.ToString());

                        string G_str = "Нет";
                        if (F < Pf)
                            G_str = "Угрожает опасность кавитации";

                        listG.Add(G_str);
                    }

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
                    V = (Gkl * (T1 + 273)) / Math.Pow((C / 18.8), 2) / (219 * (p2 + 1));
                    listI.Add(Math.Round(V, 2).ToString());

                    if (V > 40 && lpvRadioButtonList1.SelectedIndex == 1)
                        listI3.Add("возможен шум");
                    else if (V > 60 && lpvRadioButtonList1.SelectedIndex == 0)
                        listI3.Add("возможен шум");
                    else
                    {
                        listI3.Add("нет");
                    }

                    listResult["D"] = new string[] { p2.ToString() };
                    listResult["I"] = listI.ToArray();
                    listResult["I3"] = listI3.ToArray();
                }
            }

            for (int i = 0; i < listResult["C"].Count(); i++)
            {
                if (wsRadioButtonList1.SelectedIndex == 3)
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
                v_input_dict[pos] = GridView2.SelectedRow.Cells[i].Text;
                pos++;
            }

            v_input_dict[8] = v_input_dict[42];
            string fileName = AppUtils.ConvertCommaToPoint(v_input_dict[42]);

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/templates/templateTRVSteam_ver.xlsx");
            if (!File.Exists(templatePath))
            {
                LabelError.Text += "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(templatePath);
            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.1 / 2.54;
            ws.PrintOptions.BottomMargin = 0.1 / 2.54;
            ws.PrintOptions.LeftMargin = 0.6 / 2.54;
            ws.PrintOptions.RightMargin = 0.6 / 2.54;

            SetCellValue(ws, "J2", 1);
            SetCellValue(ws, "B3", 2);
            SetCellValue(ws, "C4", 6);
            SetCellValue(ws, "I4", 8);
            SetCellValue(ws, "C6", 9);
            SetCellValue(ws, "I7", 66, true); // ConvertPointToComma
            SetCellValue(ws, "K7", 67);
            SetCellValue(ws, "I8", 71, true); // ConvertPointToComma
            SetCellValue(ws, "I9", 34, true); // ConvertPointToComma
            SetCellValue(ws, "K9", 35);
            SetCellValue(ws, "I11", 34);
            SetCellValue(ws, "K11", 35);
            SetCellValue(ws, "E11", 40);
            SetCellValue(ws, "E12", 41);
            SetCellValue(ws, "A15", 42);
            SetCellValue(ws, "D15", 43, true); // ConvertPointToComma
            SetCellValue(ws, "G15", 44, true); // ConvertPointToComma
            SetCellValue(ws, "I15", 45);

            string saveDirectory = HttpContext.Current.Server.MapPath($"~/Files/TRV/PDF/{DateTime.Now:dd-MM-yyyy}");
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

    public void GenerateOtherExel()
    {
        try
        {
            v_input_dict = (Dictionary<int, string>)Session["v_input_dict"];

            v_input_dict[2] = (!String.IsNullOrWhiteSpace(objTextBox1.Text)) ? objTextBox1.Text : "-";

            int pos = 42;

            for (int i = 1; i < GridView2.SelectedRow.Cells.Count; i++)
            {
                v_input_dict[pos] = GridView2.SelectedRow.Cells[i].Text;
                pos++;
            }

            v_input_dict[8] = v_input_dict[42];
            string fileName = AppUtils.ConvertCommaToPoint(v_input_dict[42]);

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            string templatePath = HttpContext.Current.Server.MapPath("~/Content/templates/templateTRV_ver.xlsx");
            if (!File.Exists(templatePath))
            {
                LabelError.Text += "Не найден файл шаблона";
                return;
            }

            ExcelFile ef = ExcelFile.Load(templatePath);

            ExcelWorksheet ws = ef.Worksheets[0];

            ws.PrintOptions.TopMargin = 0.1 / 2.54;
            ws.PrintOptions.BottomMargin = 0.1 / 2.54;
            ws.PrintOptions.LeftMargin = 0.6 / 2.54;
            ws.PrintOptions.RightMargin = 0.6 / 2.54;

            SetCellValue(ws, "J2", 1);
            SetCellValue(ws, "B3", 2);
            SetCellValue(ws, "C4", 6);
            SetCellValue(ws, "C5", 8);
            SetCellValue(ws, "C7", 9);
            SetCellValue(ws, "J8", 12, true); // ConvertPointToComma
            SetCellValue(ws, "K8", 13);
            SetCellValue(ws, "J9", 16, true); // ConvertPointToComma
            SetCellValue(ws, "K9", 17);
            SetCellValue(ws, "J10", 18, true);
            SetCellValue(ws, "I11", 34, true);
            SetCellValue(ws, "K11", 35);
            SetCellValue(ws, "E13", 40);
            SetCellValue(ws, "E14", 41);
            SetCellValue(ws, "A17", 42);
            SetCellValue(ws, "C17", 43, true);
            SetCellValue(ws, "E17", 44, true);
            SetCellValue(ws, "F17", 45);
            SetCellValue(ws, "H17", 46, true);
            SetCellValue(ws, "I17", 47);
            SetCellValue(ws, "J17", 48, true);
            SetCellValue(ws, "K17", 49);

            string saveDirectory = HttpContext.Current.Server.MapPath($"~/Files/TRV/PDF/{DateTime.Now:dd-MM-yyyy}");
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

    //вспомогательные функции

    public void SetCellValue(ExcelWorksheet worksheet, string cellName, int dictKey, bool convertPointToComma = false)
    {
        if (v_input_dict.TryGetValue(dictKey, out string value))
        {
            worksheet.Cells[cellName].Value = convertPointToComma ? AppUtils.ConvertPointToComma(value) : value;
        }
        else
        {
            worksheet.Cells[cellName].Value = "";
        }
    }

    public void EnablePanel1()
    {
        if ((tvRadioButton1.Checked == true && tvRadioButtonList1.SelectedIndex != -1 && pnRadioButtonList1.SelectedIndex != -1) ||
            (tvRadioButton2.Checked == true && tvRadioButtonList2.SelectedIndex != -1 && pnRadioButtonList1.SelectedIndex != -1))
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

    private void ManageWSRBL()
    {
        if (tvRadioButton1.Checked && tvRadioButtonList1.SelectedIndex == 1)
        {
            wsRadioButtonList1.Items[3].Enabled = true;
        }
        else
        {
            wsRadioButtonList1.Items[3].Enabled = false;
        }
    }

    //обработчики элементов
    protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        tvRadioButton2.Checked = false;
        tvRadioButtonList1.Enabled = true;
        AppUtils.DisableRadioButtonList(tvRadioButtonList2);

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
        AppUtils.DisableRadioButtonList(tvRadioButtonList1);

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
            AppUtils.DisableRadioButtonList(lpvRadioButtonList1);

            fvDropDownList1.Enabled = true;
            lpvDropDownList21.Enabled = true;
            calcvDropDownList1.Enabled = true;
            AppUtils.DisableDropDownList(lpv5DropDownList1);
            AppUtils.DisableTextBox(lpv5TextBox1);
            AppUtils.DisableTextBox(lpv5TextBox3);
            calcvTextBox2.Enabled = true;

            fvDropDownList1.Items[1].Enabled = true;
            fvDropDownList1.Items[2].Enabled = true;
            fvDropDownList1.Items[3].Enabled = true;
            fvDropDownList1.Items[4].Enabled = true;
            fvDropDownList1.Items[5].Enabled = true;

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
            AppUtils.DisableDropDownList(lpvDropDownList21);
            AppUtils.DisableDropDownList(calcvDropDownList1);
            AppUtils.DisableTextBox(lpvTextBox21);
            AppUtils.DisableTextBox(calcvTextBox1);
            AppUtils.DisableTextBox(calcvTextBox2);
            AppUtils.DisableTextBox(calcvTextBox2);

            if (fvDropDownList1.SelectedIndex < 5)
            {
                AppUtils.DisableTextBox(fvTextBox1);
            }

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

    protected void lpvRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lpvRadioButtonList1.SelectedIndex == 1)
        {
            AppUtils.DisableTextBox(lpv5TextBox3);
        }
        else
        {
            lpv5TextBox3.Enabled = true;
        }
    }

    protected void lpvDropDownList21_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lpvDropDownList21, lpvTextBox21))
        {
            MathUtils.convertArr((sender as DropDownList), ref lpvTextBox21);
        }
        AppUtils.SavePrevSelectedIndexDDL(lpvDropDownList21.ID, lpvDropDownList21.SelectedIndex);
    }

    protected void calcvDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(calcvDropDownList1, calcvTextBox1))
        {
            MathUtils.convertArr((sender as DropDownList), ref calcvTextBox1);
        }
        AppUtils.SavePrevSelectedIndexDDL(calcvDropDownList1.ID, calcvDropDownList1.SelectedIndex);
    }

    protected void lpv5DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(lpv5DropDownList1, lpv5TextBox1))
        {
            MathUtils.convertArr((sender as DropDownList), ref lpv5TextBox1);
        }
        AppUtils.SavePrevSelectedIndexDDL(lpv5DropDownList1.ID, lpv5DropDownList1.SelectedIndex);
    }

    protected void fvDropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AppUtils.SetEnableTextBox(fvDropDownList1, fvTextBox1))
        {
            MathUtils.convertArrDouble((sender as DropDownList), ref fvTextBox1);
        }
        AppUtils.SavePrevSelectedIndexDDL(fvDropDownList1.ID, fvDropDownList1.SelectedIndex);
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

    protected void lpvCustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (wsCustomValidator1.IsValid)
        {
            if (lpvDropDownList21.Enabled)
            {
                if (lpvTextBox21.Enabled == false || AppUtils.checkTextBoxEmpty(lpvTextBox21))
                {
                    lpvCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(lpvTextBox21.Text) <= 0)
                {
                    lpvCustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
            }
            if (calcvTextBox1.Enabled == true && !AppUtils.checkTextBoxEmpty(calcvTextBox1) && MathUtils.convertArrToBar(lpvDropDownList21, lpvTextBox21) >= MathUtils.convertArrToBar(calcvDropDownList1, calcvTextBox1))
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
                if (calcvTextBox1.Enabled == false || AppUtils.checkTextBoxEmpty(calcvTextBox1))
                {
                    calcvCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(calcvTextBox1.Text) <= 0)
                {
                    calcvCustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }

                if (MathUtils.convertArrToBar(calcvDropDownList1, calcvTextBox1) > AppUtils.customConverterToDouble(pnRadioButtonList1.SelectedValue))
                {
                    calcvCustomValidator1.ErrorMessage = "Давление перед клапаном больше номинального давления клапана. Указанный клапан не подойдет";
                    args.IsValid = false;
                    return;
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
                if (AppUtils.checkTextBoxEmpty(calcvTextBox2))
                {
                    calcvCustomValidator2.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(calcvTextBox2.Text) <= 0)
                {
                    calcvCustomValidator2.ErrorMessage = "Неверно указано значение температуры";
                    args.IsValid = false;
                    return;
                }
                if (tvRadioButtonList1.SelectedIndex == 1 && AppUtils.customConverterToDouble(calcvTextBox2.Text) > MaxT2x)
                {
                    calcvCustomValidator2.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного клапана. Указанный клапан не подойдет";
                    args.IsValid = false;
                    return;
                }

                if ((tvRadioButtonList1.SelectedIndex == 0 || tvRadioButtonList2.SelectedIndex == 0) && AppUtils.customConverterToDouble(calcvTextBox2.Text) > MaxT3x)
                {
                    calcvCustomValidator2.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного клапана. Указанный клапан не подойдет";
                    args.IsValid = false;
                    return;
                }

                if (((AppUtils.customConverterToDouble(calcvTextBox1.Text) * MathUtils.getArrConvert3(calcvDropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2)) - MathUtils.getPSbyT(AppUtils.customConverterToDouble(calcvTextBox2.Text))) <= 0)
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
        if (wsCustomValidator1.IsValid)
        {
            if (lpv5DropDownList1.Enabled)
            {
                if (lpv5TextBox1.Enabled == false || AppUtils.checkTextBoxEmpty(lpv5TextBox1))
                {
                    CustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(lpv5TextBox1.Text) <= 0)
                {
                    CustomValidator1.ErrorMessage = "Неверно указано значение давления";
                    args.IsValid = false;
                    return;
                }
                if (MathUtils.convertArrToBar(lpv5DropDownList1, lpv5TextBox1) > AppUtils.customConverterToDouble(pnRadioButtonList1.SelectedValue))
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
                    if (lpv5TextBox3.Enabled == false || AppUtils.checkTextBoxEmpty(lpv5TextBox3))
                    {
                        CustomValidator3.ErrorMessage = "Необходимо заполнить поле";
                        args.IsValid = false;
                        return;
                    }
                    if (AppUtils.customConverterToDouble(lpv5TextBox3.Text) <= 0)
                    {
                        CustomValidator3.ErrorMessage = "Неверно указано значение температуры";
                        args.IsValid = false;
                        return;
                    }

                    if (AppUtils.customConverterToDouble(lpv5TextBox3.Text) > MaxT2x)
                    {
                        CustomValidator3.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного клапана. Указанный клапан не подойдет";
                        args.IsValid = false;
                        return;
                    }

                    if (AppUtils.customConverterToDouble(lpv5TextBox3.Text) < (100 * Math.Pow((AppUtils.customConverterToDouble(lpv5TextBox1.Text) * MathUtils.getArrConvert3(lpv5DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2)) + 1, 0.25)))
                    {
                        CustomValidator3.ErrorMessage = "Указанная температура ниже температуры парообразования. При указанной температуре в трубопроводе движется жидкость";
                        args.IsValid = false;
                        return;
                    }
                }
            }
            else
            {
                if (!AppUtils.checkTextBoxEmpty(lpv5TextBox3))
                {
                    if (AppUtils.customConverterToDouble(lpv5TextBox3.Text) > MaxT2x)
                    {
                        CustomValidator3.ErrorMessage = "Температура теплоносителя больше максимальной температуры рабочей среды для указанного клапана. Указанный клапан не подойдет";
                        CustomValidator3.IsValid = false;
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
                if (fvTextBox1.Enabled == false || AppUtils.checkTextBoxEmpty(fvTextBox1))
                {
                    fvCustomValidator1.ErrorMessage = "Необходимо заполнить поле";
                    args.IsValid = false;
                    return;
                }
                if (AppUtils.customConverterToDouble(fvTextBox1.Text) <= 0)
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
        getT1Steam(); //расчет температуры
        Page.Validate();

        if (!Page.IsValid) { return; }
        try
        {
            resultPanel.Visible = true;
            AppUtils.DisableTextBox(objTextBox1);
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
                Double p14 = AppUtils.customConverterToDouble(wsTextBox1.Text);
                Double p15 = AppUtils.customConverterToDouble(wsTextBox2.Text);

                g_dict.Add("p14", p14);
                g_dict.Add("p15", p15);
            }

            double checkValue;

            if (this.lpvTextBox21.Enabled)
            {
                checkValue = AppUtils.customConverterToDouble(this.lpvTextBox21.Text);
            }

            if (this.calcvTextBox1.Enabled)
            {
                checkValue = AppUtils.customConverterToDouble(this.calcvTextBox1.Text);
            }

            if (this.wsRadioButtonList1.SelectedIndex == 0)
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

            Double p30 = 0;

            double checkVal = AppUtils.customConverterToDouble(this.fvTextBox1.Text);

            if (!String.IsNullOrWhiteSpace(fvTextBox1.Text))
            {
                if (fvDropDownList1.SelectedIndex > 4)
                {
                    p30 = (AppUtils.customConverterToDouble(fvTextBox1.Text) * MathUtils.getArrConvert1((fvDropDownList1.SelectedIndex - 1), 5));
                }
                else
                {
                    p30 = (AppUtils.customConverterToDouble(fvTextBox1.Text) * MathUtils.getArrConvert1((fvDropDownList1.SelectedIndex - 1), 5) * (g / 1000));
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
                g_dict.Add("p56", AppUtils.customConverterToDouble(this.lpv5TextBox1.Text) * MathUtils.getArrConvert3(this.lpv5DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2));
                g_dict.Add("p58", 0);

                if (lpvRadioButtonList1.SelectedIndex == 0)
                {
                    g_dict.Add("p61", AppUtils.customConverterToDouble(this.lpv5TextBox3.Text));
                }

                double p35 = 0;
                try
                {
                    double p1 = (AppUtils.customConverterToDouble(lpv5TextBox1.Text) * MathUtils.getArrConvert3(lpv5DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2));

                    if (lpvRadioButtonList1.SelectedIndex == 0)
                    {
                        p35 = AppUtils.customConverterToDouble(lpv5TextBox3.Text);
                    }
                    else
                    {
                        if (lpvRadioButtonList1.SelectedIndex == 0)
                        {
                            p35 = AppUtils.customConverterToDouble(lpv5TextBox3.Text);
                        }
                        else
                        {
                            p35 = Math.Round(100 * Math.Pow((AppUtils.customConverterToDouble(lpv5TextBox1.Text) * MathUtils.getArrConvert3(lpv5DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2)) + 1, 0.25));
                        }
                    }
                }
                catch (Exception) { }

                g_dict.Add("p35", p35);
            }
            else
            {
                g_dict.Add("p35", AppUtils.customConverterToDouble(calcvTextBox2.Text));
                g_dict.Add("p61", AppUtils.customConverterToDouble(lpvTextBox21.Text) * MathUtils.getArrConvert3(lpvDropDownList21.SelectedIndex - 1));
                g_dict.Add("p62", 0);
                g_dict.Add("p63", AppUtils.customConverterToDouble(calcvTextBox1.Text) * MathUtils.getArrConvert3(calcvDropDownList1.SelectedIndex - 1));
            }

            if (lpvRadioButtonList1.SelectedIndex == 1)
            {
                lpv5TextBox3.Text = (Math.Round(100 * Math.Pow((AppUtils.customConverterToDouble(lpv5TextBox1.Text) * MathUtils.getArrConvert3(lpv5DropDownList1.SelectedIndex - 1) / MathUtils.getArrConvert3(2)) + 1, 0.25))).ToString();
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

            if (tvRadioButtonList1.SelectedIndex == 1)
            {
                this.maxt2ResultLabel.Text = "Максимальная температура - 220 °С";
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
                titles = dataFromFile.ResultTableTrvVer.ColumnName.ToObject<string[]>();
            }
            else
            {
                titles = dataFromFile.ResultTableTrvSteamVer.ColumnName.ToObject<string[]>();
            }

            DataTable dt = new DataTable();
            DataRow dr;

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
                        index = (dataFromFile.ResultTableTrvVer.ColumnIndex[gtr.ElementAt(j).Key] != null ? dataFromFile.ResultTableTrvVer.ColumnIndex[gtr.ElementAt(j).Key] : 8); //8 столбцов, последний скрыт
                    }
                    else
                    {
                        index = (dataFromFile.ResultTableTrvSteamVer.ColumnIndex[gtr.ElementAt(j).Key] != null ? dataFromFile.ResultTableTrvSteamVer.ColumnIndex[gtr.ElementAt(j).Key] : 4);
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