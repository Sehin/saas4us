using MvcWebRole1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcWebRole1.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult addSubscribe(String email)
        {
            DatabaseContext db = new DatabaseContext();
            Subscriber sub = new Subscriber(email);
            db.Subscribers.Add(sub);
            db.SaveChanges();
            return View();
        }

    }
}
