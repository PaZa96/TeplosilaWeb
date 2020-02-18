using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Сводное описание для Logger
/// </summary>
/// 

namespace TeplosilaWeb.App_Code
{
    public static class Logger
    {
        private static ILog log = LogManager.GetLogger("LOGGER");


        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }


    }
}

