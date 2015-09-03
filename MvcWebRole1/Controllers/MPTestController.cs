using MvcWebRole1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult addMP(String MPName)
        {
            DatabaseContext db = new DatabaseContext();
            MarkProgram mp = new MarkProgram(MPName);
            db.MarkPrograms.Add(mp);
            db.SaveChanges();
            return View(mp.ID_PR);
        }
    }
}
