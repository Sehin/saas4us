using MvcWebRole1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            HttpContext.Response.Cookies["ID_PR"].Value = mp.ID_PR.ToString();
            TempData["ID_PR"] = mp.ID_PR;
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
    }
}
