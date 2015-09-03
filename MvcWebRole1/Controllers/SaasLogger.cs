using MvcWebRole1.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Controllers
{
    public static class SaasLogger
    {
        public static Logger log = LogManager.GetCurrentClassLogger();
        public static void WriteTriggerInfo(object trigger)
        {
            if(trigger.GetType().Name.Equals("T3Trigger"))
            {
                DatabaseContext db = new DatabaseContext();
                T3Trigger tr = (T3Trigger)trigger;
                MarkProgram pr = db.MarkPrograms.Where(t=>t.ID_PR==tr.ID_PR).Single();
                log.Info("T3Trigger of MP(id:{0},name:{1}) was execute.", tr.ID_PR, pr.name);
            }
        }
    }
}