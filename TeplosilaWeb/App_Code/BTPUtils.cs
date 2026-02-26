using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;

/// <summary>
/// Сводное описание для BPTUtils
/// </summary>
public static class BTPUtils
{
    public static void SetPressureUnit(DropDownList ddl, string unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
            return;

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "mpa", "МПа" },
            { "bar", "бар" },
            { "mwc", "м. в. ст." }
        };

        if (!map.TryGetValue(unit, out string displayValue))
            return;

        var item = ddl.Items.FindByText(displayValue);
        if (item != null)
            ddl.SelectedValue = displayValue;
    }

    public static void SetFlowPipeUnit(DropDownList ddl, string unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
            return;

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "m3_per_h", "м³/ч" },
            { "l_per_s", "л/с" },
            { "kg_per_s", "кг/с" },
            { "kg_per_h", "кг/ч" },
            { "t_per_h", "т/ч" }
        };

        if (!map.TryGetValue(unit, out string displayValue))
            return;

        var item = ddl.Items.FindByText(displayValue);
        if (item != null)
            ddl.SelectedValue = displayValue;
    }

    public static void SetPowerPipeUnit(DropDownList ddl, string unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
            return;

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "mw", "МВт" }, 
            { "kw", "кВт" }, 
            { "w", "Вт" }, 
            { "gcal_per_h", "Гкал/ч" }
        };

        if (!map.TryGetValue(unit, out string displayValue))
            return;

        var item = ddl.Items.FindByText(displayValue);
        if (item != null)
            ddl.SelectedValue = displayValue;
    }

    public static string GetSHA256HashString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}