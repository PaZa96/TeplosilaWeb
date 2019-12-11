using System;
using System.Collections.Generic;
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
}