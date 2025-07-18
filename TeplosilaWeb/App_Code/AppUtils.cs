using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

/// <summary>
/// функции используемые во всех классах
/// </summary>
public static class AppUtils
{

    public static void EnsureDirectoryExists(string path)
    {
        var dir = new DirectoryInfo(path);
        if (!dir.Exists)
            dir.Create();
    }

    public static string GenerateUniqueFileName(string directory, string baseName)
    {
        int counter = 1;
        string fileName = baseName;

        while (File.Exists(Path.Combine(directory, fileName + ".pdf")))
        {
            fileName = $"{baseName}_{counter++}";
        }

        return fileName;
    }

    public static void ServeFile(HttpResponse Response, string fullPath)
    {
        var fileInfo = new FileInfo(fullPath);
        if (fileInfo.Exists)
        {
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", $"attachment; filename={fileInfo.Name}");
            Response.TransmitFile(fileInfo.FullName);
            Response.Flush();
        }
    }

    public static void WaitDownload(int second)
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
    public static bool SetEnableTextBox(DropDownList dropDownList, TextBox textBox)
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
    public static void DisableTextBox(TextBox textBox)
    {
        textBox.Enabled = false;
        textBox.Text = "";
    }

    public static void textBoxEnabled(TextBox textBox, bool flag)
    {
        textBox.Enabled = flag;
        if (flag == false)
        {
            textBox.Text = String.Empty;
        }

    
    }

    public static void DisableDropDownList(DropDownList dropDownList)
    {
        dropDownList.Enabled = false;
        dropDownList.ClearSelection();
    }

    public static void dropDownListEnable(DropDownList dropDownList, bool flag)
    {
        dropDownList.Enabled = flag;
        if (flag != true)
        {
            dropDownList.ClearSelection();
        };

    }

    public static void DisableRadioButtonList(RadioButtonList radioButtonList)
    {
        radioButtonList.Enabled = false;
        radioButtonList.ClearSelection();
    }

    public static bool checkTextBoxEmpty(TextBox tb)
    {
        return tb.Text == "";
    }

    public static string ConvertPointToComma(string tb)
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

    public static string ConvertCommaToPoint(string tb)
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

    public static double customConverterToDouble(string tb)
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

    public static void RemoveCssClass(HtmlGenericControl controlInstance, string css)
    {
        var strCssClass = controlInstance.Attributes["class"];
        controlInstance.Attributes["class"] = string.Join(" ", strCssClass.Split(' ').Where(x => x != css).ToArray());
    }

    public static void AddCssClass(HtmlGenericControl controlInstance, string css)
    {
        var strCssClass = controlInstance.Attributes["class"];
        controlInstance.Attributes["class"] += (" " + css);
    }

    public static bool firstMoreSecondDouble(string s1, string s2)
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

    public static void SaveKeyToSession(string id, int key)
    {
        HttpContext.Current.Session[id] = key;
    }
}