using System.Web;
using System.Web.Services;

/// <summary>
/// Сводное описание для WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

// Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку.
// [System.Web.Script.Services.ScriptService]

public class RdtService : WebService {
    [WebMethod(EnableSession = true)]
    public object ReceivePrefill(object payload)
    {
        HttpContext.Current.Session["PrefillRDT"] = payload; return new { success = true };
    }
}