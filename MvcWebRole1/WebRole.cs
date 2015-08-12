using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace MvcWebRole1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // Сведения о работе с изменениями конфигурации
            // см. в разделе MSDN по адресу http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
