using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeplosilaWeb.App_Code;

public partial class DownloadFile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string file = HttpUtility.UrlDecode(Request.QueryString["path"]);
            string baseFolder = "";

            // 1. Проверка параметра
            if (string.IsNullOrWhiteSpace(file))
            {
                Logger.Log.Info("Отсутствует обязательный параметр 'path'");
                return;
            }

            Logger.Log.Info(file);

            // 2. Защита от попыток выйти из директории (path traversal)
            if (file.Contains("..") || file.Contains("\\"))
            {
                Logger.Log.Info("Недопустимое имя файла. " + file);
                return;
            }

            if (file.Contains("RDT"))
            {
                baseFolder = "~/Files/RDT/PDF/";
            }
            else if (file.Contains("TRV"))
            {
                baseFolder = "~/Files/TRV/PDF/";
            }
            else
            {
                Logger.Log.Info("Неизвестный тип файла. " + file);
                return;
            }

            // 3. Формируем путь
            string fullPath = Server.MapPath(baseFolder + file);

            // 4. Проверяем существование файла
            if (!File.Exists(fullPath))
            {
                Logger.Log.Info("Файл" + file + " не найден.");
                return;
            }

            int pos = file.IndexOf('_');

            string fileName = Regex.Replace(file, @"^.*?(RDT|TRV)", "$1");

            // 5. Отдаём файл
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.TransmitFile(fullPath);
            Response.Flush();
            Response.End();
        }

        catch (Exception ex)
        {
            Logger.Log.Error(ex);
        }
    }

}