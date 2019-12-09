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


    protected void tvRadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        changeImage(tvRadioButtonList1.SelectedIndex);
    }
}