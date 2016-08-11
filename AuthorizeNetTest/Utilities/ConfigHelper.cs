using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AuthorizeNetTest.Utilities
{
    public static class ConfigHelper
    {
        public static bool UseApiSandbox = ConfigurationManager.AppSettings["UseApiSandbox"] != null &&
                                           ConfigurationManager.AppSettings["UseApiSandbox"].ToUpper() == "TRUE";

        public static string ApiLoginId = UseApiSandbox
                                        ? ConfigurationManager.AppSettings["SandboxApiLoginID"]
                                        : ConfigurationManager.AppSettings["ApiLoginID"];
        
        public static string ApiTransactionKey = UseApiSandbox
                                                ? ConfigurationManager.AppSettings["SandboxApiTransactionKey"]
                                                : ConfigurationManager.AppSettings["ApiTransactionKey"];
    }
}