using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
        arrConvert3 = new double[4] { 1000, 1, 100, 10 };


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
    }

    protected void aaRadioButton2_CheckedChanged(object sender, EventArgs e)
    {
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
    }

    protected void aaRadioButton3_CheckedChanged(object sender, EventArgs e)
    {
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
    }

    private void changeImage(int index)
    {
        switch (index)
        {
            case 0:
                vPictureBox.ImageUrl = @"\Content\images\TRV-2.png";
                break;
            case 1:
                vPictureBox.ImageUrl = @"\Content\images\TRV-3.png";
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
    }

    

    protected void aa1RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetPressureLoss(aa1RadioButtonList1);
        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
    }

    protected void aa2RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetPressureLoss(aa2RadioButtonList1);
        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
        }
    }

    protected void aa3RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetPressureLoss(aa3RadioButtonList1);
        if (tvRadioButtonList1.SelectedIndex >= 0 && fvRadioButton2.Checked)
        {
            EnableTemperatureTable();
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
                result = (customConverterToDouble(tb.Text) * arr[jj - 1] / arr[3]);
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



    protected void tdRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void tdRadioButtonList2_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void tdRadioButtonList3_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void tdRadioButtonList4_SelectedIndexChanged(object sender, EventArgs e)
    {

    }


    private void ValidationAlert(TextBox tb, string str, int ms)
    {
        tb.BackColor = Color.LightPink;
        //ToolTip t = new ToolTip();
        //t.Show(str, tb, ms);
    }

    private void ValidationAlertC(Control c, string str, int ms)
    {
        //c.BackColor = Color.LightPink;
        //ToolTip t = new ToolTip();
        //t.Show(str, c, ms);
        //MessageBox.Show(str, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private bool firstMoreSecondDouble(string s1, string s2)
    {
        if (!String.IsNullOrWhiteSpace(s1) && !String.IsNullOrWhiteSpace(s2) && !String.IsNullOrEmpty(s1) && !String.IsNullOrEmpty(s2))
        {
            if (Convert.ToDouble(s1) > Convert.ToDouble(s2))
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
                    avg_T = Convert.ToDouble(this.ws2TextBox2.Text);
                }
            }
            else
            {
                if (this.tvRadioButtonList1.SelectedIndex == 0 || this.aaRadioButton1.Checked)
                {
                    avg_T = 0.5 * (double.Parse(this.fvTextBox2.Text) + double.Parse(this.fvTextBox3.Text));
                }
                else if (this.aaRadioButton2.Checked)
                {
                    avg_T = 0.5 * (double.Parse(this.fvTextBox6.Text) + double.Parse(this.fvTextBox7.Text));
                }
                else if (this.aaRadioButton3.Checked)
                {
                    avg_T = 0.5 * (double.Parse(this.fvTextBox8.Text) + double.Parse(this.fvTextBox9.Text));
                }
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

    protected void vButton_Click(object sender, EventArgs e)
    {
        ResetColorToAllControls();
        dataGridView2.Columns.Clear();
        dataGridView2.Rows.Clear();
        dataGridView2.Refresh();

        readFile(0);
        Dictionary<string, double> g_dict = new Dictionary<string, double>();
        v_input_dict.Clear();

        if (spvRadioButtonList1.SelectedIndex != -1)
        {
            if (spvRadioButtonList1.SelectedIndex == 0) g_dict.Add("vmax", 5); else g_dict.Add("vmax", 3);

            if (rpvRadioButtonList1.SelectedIndex != -1)
            {
                if (rpvRadioButtonList1.SelectedIndex == 0) g_dict.Add("vKv", 1.0); else g_dict.Add("vKv", 1.2);

                if (checkRadioButtons(aaGroupBox))
                {
                    if ((aaRadioButton1.Checked && checkRadioButtons(aa1Panel))
                        || (aaRadioButton2.Checked && checkRadioButtons(aa2Panel))
                        || (aaRadioButton3.Checked && checkRadioButtons(aa3Panel)))
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
                                        p14 = Convert.ToDouble(ws2TextBox1.Text);
                                    }
                                    catch (Exception)
                                    {
                                        ValidationAlertC(ws2TextBox1, "Не указано значение концентрации " + (ws2RadioButtonList1.SelectedIndex == 1 ? "этиленгликоля" : "пропиленгликоля"), 5000);
                                        return;
                                    }

                                    if (p14 < 5 || p14 > 65)
                                    {
                                        ValidationAlertC(ws2TextBox1, "Неверно указано значение концентрации " + (ws2RadioButtonList1.SelectedIndex == 1 ? "этиленгликоля" : "пропиленгликоля"), 5000);
                                        return;
                                    }
                                    else
                                    {
                                        g_dict.Add("p14", p14);
                                    }


                                    try
                                    {
                                        p15 = Convert.ToDouble(ws2TextBox2.Text);
                                    }
                                    catch (Exception)
                                    {
                                        ValidationAlertC(ws2TextBox2, "Не указано значение температуры " + (ws2RadioButtonList1.SelectedIndex == 1 ? "этиленгликоля" : "пропиленгликоля"), 5000);
                                        return;
                                    }

                                    if (p15 < 0 || p15 > 150)
                                    {
                                        ValidationAlertC(ws2TextBox2, "Неверно указано значение температуры " + (ws2RadioButtonList1.SelectedIndex == 1 ? "этиленгликоля" : "пропиленгликоля"), 5000);
                                        return;
                                    }
                                    else
                                    {
                                        g_dict.Add("p15", p15);
                                    }
                                }

                                if (fvRadioButton1.Checked || fvRadioButton2.Checked)
                                {

                                    Double p30 = 0;
                                    if (fvRadioButton2.Checked)
                                    {
                                        Double dt = 0;
                                        if (aaRadioButton1.Checked)
                                        {
                                            if (!(firstMoreSecondDouble(fvTextBox2.Text, fvTextBox3.Text) &&
                                                firstMoreSecondDouble(fvTextBox4.Text, fvTextBox5.Text)))
                                            {
                                                fvTextBox3.BackColor = Color.LightPink;
                                                fvTextBox4.BackColor = Color.LightPink;
                                                fvTextBox5.BackColor = Color.LightPink;

                                                ValidationAlertC(fvTextBox2, "Неверно заданы температуры для вычисления расхода", 5000);
                                                return;
                                            }
                                            else
                                            {
                                                dt = (Convert.ToDouble(fvTextBox2.Text) - Convert.ToDouble(fvTextBox3.Text)) > (Convert.ToDouble(fvTextBox4.Text) - Convert.ToDouble(fvTextBox5.Text)) ?
                                                    (Convert.ToDouble(fvTextBox4.Text) - Convert.ToDouble(fvTextBox5.Text)) :
                                                    (Convert.ToDouble(fvTextBox2.Text) - Convert.ToDouble(fvTextBox3.Text));
                                            }
                                        }
                                        else if ((aaRadioButton2.Checked && tvRadioButtonList1.SelectedIndex == 0)
                                            || (aaRadioButton2.Checked && tvRadioButtonList1.SelectedIndex == 1 && aa2RadioButtonList1.SelectedIndex == 0)
                                            || (aaRadioButton3.Checked && tvRadioButtonList1.SelectedIndex == 0)
                                            || (aaRadioButton3.Checked && tvRadioButtonList1.SelectedIndex == 1 && aa3RadioButtonList1.SelectedIndex == 0))
                                        {
                                            if (!firstMoreSecondDouble(fvTextBox2.Text, fvTextBox3.Text))
                                            {
                                                fvTextBox3.BackColor = Color.LightPink;

                                                ValidationAlertC(fvTextBox2, "Неверно заданы температуры для вычисления расхода", 5000);
                                                return;
                                            }
                                            else
                                            {
                                                dt = (Convert.ToDouble(fvTextBox2.Text) - Convert.ToDouble(fvTextBox3.Text));
                                            }
                                        }
                                        else if (aaRadioButton2.Checked && tvRadioButtonList1.SelectedIndex == 1 && aa2RadioButtonList1.SelectedIndex == 1)
                                        {
                                            if (!firstMoreSecondDouble(fvTextBox6.Text, fvTextBox7.Text))
                                            {
                                                fvTextBox7.BackColor = Color.LightPink;

                                                ValidationAlertC(fvTextBox6, "Неверно заданы температуры для вычисления расхода", 5000);
                                                return;
                                            }
                                            else
                                            {
                                                dt = (Convert.ToDouble(fvTextBox6.Text) - Convert.ToDouble(fvTextBox7.Text));
                                            }
                                        }
                                        else if (aaRadioButton3.Checked && tvRadioButtonList1.SelectedIndex == 1 && aa3RadioButtonList1.SelectedIndex == 1)
                                        {
                                            if (!firstMoreSecondDouble(fvTextBox8.Text, fvTextBox9.Text))
                                            {
                                                fvTextBox9.BackColor = Color.LightPink;

                                                ValidationAlertC(fvTextBox8, "Неверно заданы температуры для вычисления расхода", 5000);
                                                return;
                                            }
                                            else
                                            {
                                                dt = (Convert.ToDouble(fvTextBox8.Text) - Convert.ToDouble(fvTextBox9.Text));
                                            }
                                        }


                                        if (fvTextBox2.Enabled && Convert.ToDouble(fvTextBox2.Text) > g_dict["vTMax"])
                                        {
                                            ValidationAlertC(fvTextBox2, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                            return;
                                        }
                                        else if (fvTextBox3.Enabled && Convert.ToDouble(fvTextBox3.Text) > g_dict["vTMax"])
                                        {
                                            ValidationAlertC(fvTextBox3, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                            return;
                                        }
                                        else if (fvTextBox4.Enabled && Convert.ToDouble(fvTextBox4.Text) > g_dict["vTMax"])
                                        {
                                            ValidationAlertC(fvTextBox4, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                            return;
                                        }
                                        else if (fvTextBox5.Enabled && Convert.ToDouble(fvTextBox5.Text) > g_dict["vTMax"])
                                        {
                                            ValidationAlertC(fvTextBox5, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                            return;
                                        }
                                        else if (fvTextBox6.Enabled && Convert.ToDouble(fvTextBox6.Text) > g_dict["vTMax"])
                                        {
                                            ValidationAlertC(fvTextBox6, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                            return;
                                        }
                                        else if (fvTextBox7.Enabled && Convert.ToDouble(fvTextBox7.Text) > g_dict["vTMax"])
                                        {
                                            ValidationAlertC(fvTextBox7, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                            return;
                                        }
                                        else if (fvTextBox8.Enabled && Convert.ToDouble(fvTextBox8.Text) > g_dict["vTMax"])
                                        {
                                            ValidationAlertC(fvTextBox8, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                            return;
                                        }
                                        else if (fvTextBox9.Enabled && Convert.ToDouble(fvTextBox9.Text) > g_dict["vTMax"])
                                        {
                                            ValidationAlertC(fvTextBox9, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                            return;
                                        }

                                        if (!String.IsNullOrWhiteSpace(fvTextBox10.Text))
                                        {
                                            p30 = Math.Round(((Convert.ToDouble(fvTextBox10.Text) * arrConvert2[fvDropDownList2.SelectedIndex - 1] * 3.6) / (math_30_cp() * dt)), 2);
                                            fvTextBox11.Text = p30.ToString();
                                        }
                                        else
                                        {
                                            ValidationAlertC(fvTextBox10, "Не задана тепловая мощность", 5000);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrWhiteSpace(fvTextBox1.Text))
                                        {
                                            p30 = (Convert.ToDouble(fvTextBox1.Text) * arrConvert1[(fvDropDownList1.SelectedIndex - 1), 5]);
                                        }
                                        else
                                        {
                                            ValidationAlertC(fvTextBox1, "Не задан расход через клапан", 5000);
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
                                                    p35 = Convert.ToDouble(calcvTextBox2.Text);
                                                }
                                                catch (Exception) { }

                                                if (p35 <= 0)
                                                {

                                                    ValidationAlertC(calcvTextBox2, "Неверно указано значение температуры", 5000);
                                                    return;
                                                }
                                                else if (p35 > g_dict["vTMax"])
                                                {

                                                    ValidationAlertC(calcvTextBox2, "На температуру свыше " + g_dict["vTMax"].ToString() + "°С вариантов нет", 5000);
                                                    return;
                                                }

                                                double p61 = 0;
                                                try
                                                {
                                                    if ((aa1Panel.Enabled && aa1RadioButton1.Checked)
                                                        || (aa2Panel.Enabled && aa2RadioButton1.Checked)
                                                        || (aa3Panel.Enabled && aa3RadioButton1.Checked))
                                                        p61 = Convert.ToDouble(lpvTextBox2.Text) * arrConvert3[lpvDropDownList2.SelectedIndex - 1] / arrConvert3[2];

                                                    else if ((aa1Panel.Enabled && aa1RadioButton2.Checked)
                                                        || (aa2Panel.Enabled && aa2RadioButton2.Checked)
                                                        || (aa3Panel.Enabled && aa3RadioButton2.Checked))
                                                        p61 = Convert.ToDouble(lpvTextBox21.Text) * arrConvert3[lpvDropDownList21.SelectedIndex - 1] / arrConvert3[2];
                                                }
                                                catch (Exception) { }

                                                if (p61 > 16)
                                                {
                                                    if ((aa1Panel.Enabled && aa1RadioButton1.Checked)
                                                        || (aa2Panel.Enabled && aa2RadioButton1.Checked)
                                                        || (aa3Panel.Enabled && aa3RadioButton1.Checked))
                                                        ValidationAlertC(lpvTextBox2, "На давление свыше 16 бар вариантов нет", 5000);

                                                    else if ((aa1Panel.Enabled && aa1RadioButton2.Checked)
                                                        || (aa2Panel.Enabled && aa2RadioButton2.Checked)
                                                        || (aa3Panel.Enabled && aa3RadioButton2.Checked))
                                                        ValidationAlertC(lpvTextBox21, "На давление свыше 16 бар вариантов нет", 5000);

                                                    return;
                                                }
                                                else g_dict.Add("p61", p61);

                                                double p62 = 0;
                                                try
                                                {
                                                    if (lpvTextBox1.Enabled)
                                                        p62 = Convert.ToDouble(lpvTextBox1.Text) * arrConvert3[lpvComboBox1.SelectedIndex - 1] / arrConvert3[2];
                                                }
                                                catch (Exception) { }

                                                if (p62 > 16)
                                                {
                                                    ValidationAlertC(lpvTextBox1, "На давление свыше 16 бар вариантов нет", 5000);

                                                    return;
                                                }
                                                else g_dict.Add("p62", p62);

                                                double p63 = 0;
                                                try
                                                {
                                                    if (calcvTextBox1.Enabled)
                                                        p63 = Convert.ToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1] / arrConvert3[2];
                                                }
                                                catch (Exception) { }

                                                if (p63 > 16)
                                                {
                                                    ValidationAlertC(calcvTextBox1, "На давление свыше 16 бар вариантов нет", 5000);

                                                    return;
                                                }
                                                else g_dict.Add("p63", p63);


                                                if (!(p63 > p62))
                                                {
                                                    ValidationAlertC(lpvTextBox1, "Неверно указано значение давления", 5000);

                                                    return;
                                                }

                                                /*if (p35 < 7 || p35 > 150)
                                                {
                                                    MessageBox.Show("Не задана температура для расчета клапана на кавитацию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                                                ws2ResultLabel.Text = "Рабочая среда - " + (ws2RadioButtonList1.SelectedIndex == 0 ? "вода" : ((ws2RadioButtonList1.SelectedIndex == 1 ? "этиленгликоль " : "пропиленгликоль ") + g_dict["p14"] + "%, " + g_dict["p15"] + " °С"));
                                                maxt2ResultLabel.Text = "Максимальная температура - " + g_dict["vTMax"].ToString() + " °С";
                                                maxp2ResultLabel.Text = "Максимальное рабочее давление - 16 бар";


                                                double t1_check = Convert.ToDouble(calcvTextBox2.Text);
                                                Newtonsoft.Json.Linq.JObject max_check = dataFromFile.table9v[dataFromFile.table9v.Count - 1];
                                                foreach (Newtonsoft.Json.Linq.JObject ob in dataFromFile.table9v)
                                                {
                                                    if ((Convert.ToDouble(ob.GetValue("t1")) <= Convert.ToDouble(max_check.GetValue("t1"))) && (Convert.ToDouble(ob.GetValue("t1")) >= t1_check))
                                                    {
                                                        max_check = ob;
                                                    }
                                                }
                                                double ps_check = Convert.ToDouble(max_check.GetValue("ps"));

                                                if (((Convert.ToDouble(calcvTextBox1.Text) * arrConvert3[calcvDropDownList1.SelectedIndex - 1] / arrConvert3[2]) - ps_check) <= 0)
                                                {
                                                    ValidationAlertC(calcvTextBox2, "Указанная температура выше температуры парообразования. При указанной температуре в трубопроводе движется пар", 5000);
                                                    return;
                                                }

                                                mapInputParametersV(ref v_input_dict);

                                                Dictionary<string, string[]> gtr = generatedTableV(g_dict);

                                                if (gtr is null) return;

                                                string[] titles = new string[] {
                                            "Марка регулирующего клапана",
                                            "Номинальный диаметр DN, мм",
                                            "Пропускная пособность Kvs, м3/ч",
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
                                                DataGridViewColumn column;
                                                /*column = new DataGridViewCheckBoxColumn();
                                                column.DataPropertyName = "Column0";
                                                column.Name = "Column0";
                                                column.HeaderText = "";
                                                dataGridView2.Columns.Add(column);*/

                                                for (int i = 0; i < titles.Count(); i++)
                                                {
                                                    column = new DataGridViewTextBoxColumn();
                                                    column.DataPropertyName = "Column" + i.ToString();
                                                    column.Name = "Column" + i.ToString();
                                                    column.HeaderText = titles[i];
                                                    if (i > 10) column.Visible = false;
                                                    dataGridView2.Columns.Add(column);
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
                                                    dataGridView2.Rows.Add();
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
                                                                if (dataGridView2.Rows.Count > 1)
                                                                {
                                                                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[index - 1].Value = dataGridView2.Rows[dataGridView2.Rows.Count - 2].Cells[index - 1].Value;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[index - 1].Value = tmp;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (dataGridView2.Rows.Count > 1)
                                                            {
                                                                dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[index - 1].Value = dataGridView2.Rows[dataGridView2.Rows.Count - 2].Cells[index - 1].Value;
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                ValidationAlertC(calcvTextBox1, "Неверно указано значение давления", 5000);
                                                return;
                                            }

                                        }
                                        else
                                        {
                                            ValidationAlertC((lpvDropDownList2.Enabled) ? lpvTextBox2 : lpvTextBox21, "Неверно указано значение давления", 5000);
                                            return;
                                        }

                                    }
                                    else
                                    {
                                        ValidationAlertC(lpvTextBox1, "Неверно указано значение давления", 5000);
                                        return;
                                    }
                                }
                                else
                                {
                                    ValidationAlertC(fvGroupBox, "Не задан расход через клапан", 5000);
                                    return;
                                }
                            }
                            else
                            {
                                ValidationAlertC(ws2RadioButtonList1.SelectedIndex < 0, "Не выбрана рабочая среда", 5000);
                                return;
                            }

                        }
                        else
                        {
                            ValidationAlertC(tvGroupBox, "Не выбран тип регулирующего клапана", 5000);
                            return;
                        }
                    }
                    else
                    {
                        if (aaRadioButton1.Checked)
                        {
                            ValidationAlertC(aa1Panel, "Не выбрана схема присоединения регулирующего клапана", 5000);
                        }
                        else if (aaRadioButton2.Checked)
                        {
                            ValidationAlertC(aa2Panel, "Не выбрана схема присоединения регулирующего клапана", 5000);
                        }
                        else if (aaRadioButton3.Checked)
                        {
                            ValidationAlertC(aa3Panel, "Не выбрана схема присоединения регулирующего клапана", 5000);
                        }

                        return;
                    }
                }
                else
                {
                    ValidationAlertC(aaGroupBox, "Не выбрана область применения (система) регулирующего клапана", 5000);
                    return;
                }
            }
            else
            {
                ValidationAlertC(rpvGroupBox, "Не указано наличие регулятора перепада давления", 5000);
                return;
            }

        }
        else
        {
            ValidationAlertC(spvGroupBox, "Не выбрано место установки регулирующего клапана", 5000);
            return;
        }
        if (tabControl1.SelectedIndex == 0)
        {
            dataGridView1.Visible = true;
            dataGridView1.Height = 250;
            dataGridView1.ClearSelection();
            button1.Visible = true;
            button1.Enabled = false;
        }
        else if (tabControl1.SelectedIndex == 1)
        {
            dataGridView2.Visible = true;
            dataGridView2.Height = 250;
            dataGridView2.ClearSelection();
            button2.Visible = true;
            button2.Enabled = false;
        }
    }

    protected void spvRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}