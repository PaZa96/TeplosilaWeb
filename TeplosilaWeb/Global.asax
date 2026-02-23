<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {

        new ScriptResourceDefinition
        {
            Path = "~/Scripts/jquery-3.4.1.min.js",
            DebugPath = "~/Scripts/jquery-3.4.1.min.js",
            CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.4.1.min.js",
            CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.4.1.js"
        };
    }

    void Application_End(object sender, EventArgs e)
    {
        //  Код, выполняемый при завершении работы приложения

    }

    void Application_Error(object sender, EventArgs e)
    {
        // Код, выполняемый при возникновении необрабатываемой ошибки

    }

    void Session_Start(object sender, EventArgs e)
    {
        // Код, выполняемый при запуске нового сеанса

    }

    void Session_End(object sender, EventArgs e)
    {
        // Код, выполняемый при запуске приложения. 
        // Примечание: Событие Session_End вызывается только в том случае, если для режима sessionstate
        // задано значение InProc в файле Web.config. Если для режима сеанса задано значение StateServer 
        // или SQLServer, событие не порождается.

    }
    protected void Application_EndRequest(object sender, EventArgs e)
    {
        var cookies = Response.Headers.GetValues("Set-Cookie");
        if (cookies == null) return;

        Response.Headers.Remove("Set-Cookie");

        foreach (var cookie in cookies)
        {
            var updated = cookie;

            // Принудительно ставим SameSite=None
            if (updated.Contains("SameSite"))
            {
                updated = System.Text.RegularExpressions.Regex.Replace(
                    updated,
                    @"SameSite=\w+",
                    "SameSite=None",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }
            else
            {
                updated += "; SameSite=None";
            }

            // Принудительно ставим Secure
            if (!updated.Contains("Secure"))
            {
                updated += "; Secure";
            }

            Response.Headers.Add("Set-Cookie", updated);
        }
    }


</script>
