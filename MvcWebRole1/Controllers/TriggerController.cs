using MvcWebRole1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebRole1.Controllers
{
    public class TriggerController : Controller
    {
        //
        // GET: /Trigger/

        public ActionResult Index()
        {
            return View();
        }

        public void checkT1Triggers()
        {
            DatabaseContext db = new DatabaseContext();
#region T1
            List<TriggerT1> T1Triggers = db.TriggersT1.ToList();
            foreach(TriggerT1 t1 in T1Triggers)
            {
                Trigger trigger = db.Triggers.Where(t => t.ID_TR == t1.ID_TR).Single();
                MarkProgram mp = db.MarkPrograms.Where(m => m.ID_PR == trigger.ID_PR).Single();
                int userId = mp.ID_USER;

                List<Client> clients = db.Clients.Where(c => c.ID_USER == userId).ToList();

                
            }
#endregion
        }

    }
}
