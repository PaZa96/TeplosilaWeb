<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        log4net.Config.XmlConfigurator.Configure();
        TeplosilaWeb.App_Code.Logger.InitLogger();

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

</script>
