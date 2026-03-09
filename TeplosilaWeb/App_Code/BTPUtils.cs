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
    public static void SetUnit(DropDownList ddl, string unit, Dictionary<string, string> map)
    {
        if (string.IsNullOrWhiteSpace(unit) || map == null)
            return;

        if (!map.TryGetValue(unit, out string displayValue))
            return;

        var item = ddl.Items.FindByText(displayValue);
        if (item != null)
            ddl.SelectedValue = displayValue;
    }

    public static void SetPressureUnit(DropDownList ddl, string unit)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "mpa", "МПа" },
        { "bar", "бар" },
        { "mwc", "м. в. ст." }
    };

        SetUnit(ddl, unit, map);
    }

    public static void SetFlowPipeUnit(DropDownList ddl, string unit)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "m3_per_h", "м³/ч" },
        { "l_per_s", "л/с" },
        { "kg_per_s", "кг/с" },
        { "kg_per_h", "кг/ч" },
        { "t_per_h", "т/ч" }
    };

        SetUnit(ddl, unit, map);
    }

    public static void SetPowerPipeUnit(DropDownList ddl, string unit)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "mw", "МВт" },
        { "kw", "кВт" },
        { "w", "Вт" },
        { "gcal_per_h", "Гкал/ч" }
    };

        SetUnit(ddl, unit, map);
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

    public static string CheckJsonAttr(dynamic b, string key)
    {
        return b.ContainsKey(key) ? b[key]?.ToString() ?? "" : "";
    }

    public static string DetectMethodSetting(dynamic b)
    {
        if (b.ContainsKey("method_setting_regulator_before"))
            return "method_setting_regulator_before";

        if (b.ContainsKey("method_setting_regulator_after"))
            return "method_setting_regulator_after";

        return "method_setting_regulator_differential";
    }

    public static string CreateResultObjectRTD(string blockType, string methodSettingRegulator, GridView GridView1)
    {

        GridViewRow row = GridView1.SelectedRow;

        var ResultList = new Dictionary<string, object>();

        switch (blockType)
        {
            case "TBV_TBVU":
                switch (methodSettingRegulator)
                {
                    case "method_setting_regulator_after":
                        ResultList["mark_regulator_after"] = row.Cells[1].Text.Trim();
                        ResultList["dn_regulator_after"] = AppUtils.customConverterToDouble(row.Cells[2].Text.Trim());
                        ResultList["kvs_regulator_after"] = AppUtils.customConverterToDouble(row.Cells[3].Text.Trim());
                        ResultList["loss_pressure_regulator_after"] = AppUtils.customConverterToDouble(row.Cells[4].Text.Trim());
                        ResultList["setting_range_regulator_after"] = row.Cells[5].Text.Trim();
                        break;
                    case "method_setting_regulator_before":
                        ResultList["mark_regulator_before"] = row.Cells[1].Text.Trim();
                        ResultList["dn_regulator_before"] = AppUtils.customConverterToDouble(row.Cells[2].Text.Trim());
                        ResultList["kvs_regulator_before"] = AppUtils.customConverterToDouble(row.Cells[3].Text.Trim());
                        ResultList["loss_pressure_regulator_before"] = AppUtils.customConverterToDouble(row.Cells[4].Text.Trim());
                        ResultList["setting_range_regulator_before"] = row.Cells[5].Text.Trim();
                        break;
                    case "method_setting_regulator_differential":
                        ResultList["mark_regulator_differential"] = row.Cells[1].Text.Trim();
                        ResultList["dn_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[2].Text.Trim());
                        ResultList["kvs_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[3].Text.Trim());
                        ResultList["loss_pressure_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[4].Text.Trim());
                        ResultList["setting_range_regulator_differential"] = row.Cells[5].Text.Trim();
                        break;
                    default:
                        return "";
                }
                break;
            case "TBGV":
                ResultList["mark_regulator_differential"] = row.Cells[1].Text.Trim();
                ResultList["dn_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[2].Text.Trim());
                ResultList["kvs_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[3].Text.Trim());
                ResultList["loss_pressure_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[4].Text.Trim());
                ResultList["setting_range_regulator_differential"] = row.Cells[5].Text.Trim();
                break;
            case "TBO":
                ResultList["mark_regulator_differential"] = row.Cells[1].Text.Trim();
                ResultList["dn_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[2].Text.Trim());
                ResultList["kvs_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[3].Text.Trim());
                ResultList["loss_pressure_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[4].Text.Trim());
                ResultList["setting_range_regulator_differential"] = row.Cells[5].Text.Trim();
                break;
            case "TBR":
                ResultList["mark_regulator"] = row.Cells[1].Text.Trim();
                ResultList["dn_regulator"] = AppUtils.customConverterToDouble(row.Cells[2].Text.Trim());
                ResultList["kvs_regulator"] = AppUtils.customConverterToDouble(row.Cells[3].Text.Trim());
                ResultList["loss_pressure_regulator"] = AppUtils.customConverterToDouble(row.Cells[4].Text.Trim());
                ResultList["setting_range_regulator"] = row.Cells[5].Text.Trim();
                break;
            case "TBSV":
                ResultList["mark_regulator_differential"] = row.Cells[1].Text.Trim();
                ResultList["dn_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[2].Text.Trim());
                ResultList["kvs_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[3].Text.Trim());
                ResultList["loss_pressure_regulator_differential"] = AppUtils.customConverterToDouble(row.Cells[4].Text.Trim());
                ResultList["setting_range_regulator_differential"] = row.Cells[5].Text.Trim();
                break;

            default:
                return "";
        }

        string result = Newtonsoft.Json.JsonConvert.SerializeObject(ResultList);

        return result;
    }
}