using MvcWebRole1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;

namespace MvcWebRole1.Controllers
{
    public class MPTestController : Controller
    {
        //
        // GET: /MPTest/

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult addMP(String MPName, int ID_USER)
        {
            DatabaseContext db = new DatabaseContext();
            MarkProgram mp = new MarkProgram(MPName, ID_USER);
            db.MarkPrograms.Add(mp);
            db.SaveChanges();
            mpGlVar.ID_PR = mp.ID_PR;
            mpGlVar.PR_NAME = MPName;
            MvcWebRole1.Models.Action action = new Models.Action(-1, mpGlVar.ID_PR);
            db.Actions.Add(action);
            db.SaveChanges();
            mpGlVar.ID_ACTION = action.ID_ACTION;
            return PartialView();
        }
        public PartialViewResult SelectTriggerType(int triggerType)
        {
            switch (triggerType)
            {
                case 1:
                    return PartialView("addTT1View");
                    break;
                case 2:
                    return PartialView("addTT2View");
                    break;
                case 3:
                    return PartialView("addTT3View");
                    break;
            }
            return null;
        }
        public PartialViewResult addTT1View()
        {
            return PartialView("addTT1View");
        }
        public ActionResult addTT1(int ageSign, int age, int type, int sex)
        {
            DatabaseContext db = new DatabaseContext();
            T1Trigger t1tr = new T1Trigger(mpGlVar.ID_PR, type, age, sex, ageSign);
            db.T1Trigger.Add(t1tr);
            db.SaveChanges();
            mpGlVar.TR_TYPE = 1;
            mpGlVar.ID_TR = t1tr.ID_TT1;
            return RedirectToAction("trigger");
        }
        public PartialViewResult trigger(int trId)
        {
            DatabaseContext db = new DatabaseContext();
            mpGlVar.ID_PR = 1;         // id MP
            mpGlVar.TR_TYPE = 1;   // Тип триггера
            mpGlVar.ID_ACTION = 1;    // id мастер-действия
            return PartialView(db);
        }
    }
    public static class mpGlVar
    {
        public static int ID_PR;        // id MP
        public static String PR_NAME;   // Имя MP
        public static int TR_TYPE;      // Тип триггера
        public static int ID_ACTION;    // id мастер-действия
        public static int ID_TR;        // Id триггера
    }
}
