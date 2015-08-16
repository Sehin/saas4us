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

                List<Client> clients = new List<Client>();

                if (t1.CL_AGE_SIGN!=-1) // Возраст не указан
                {
                    if (t1.CL_SEX != -1)  // Пол не важен
                    {
                        if (t1.CL_TYPE != -1)   // Тип не важен
                        {
                            clients = db.Clients.ToList();
                        }
                        else        // Тип важен
                        {
                            clients = db.Clients.Where(c=>c.TYPE==t1.CL_TYPE).ToList();
                        }
                    }
                    else // Пол важен
                    {
                        if (t1.CL_TYPE != -1)   // Тип не важен, пол важен
                        {
                            clients = db.Clients.Where(c=>c.SEX==t1.CL_SEX).ToList();
                        }
                        else        // Тип важен, пол важен
                        {
                            clients = db.Clients.Where(c => c.TYPE == t1.CL_TYPE && c.SEX==t1.CL_SEX).ToList();
                        }
                    }
                }
                else        // Возраст указан
                {
                    if (t1.CL_SEX != -1)  // Пол не важен
                    {
                        if (t1.CL_TYPE != -1)   // Тип не важен, возраст важен
                        {
                            clients = db.Clients.ToList();
                        }
                        else        // Тип важен
                        {
                            clients = db.Clients.Where(c => c.TYPE == t1.CL_TYPE).ToList();
                        }
                    }
                    else // Пол важен
                    {
                        if (t1.CL_TYPE != -1)   // Тип не важен, пол важен
                        {
                            clients = db.Clients.Where(c => c.SEX == t1.CL_SEX).ToList();
                        }
                        else        // Тип важен, пол важен
                        {
                            clients = db.Clients.Where(c => c.TYPE == t1.CL_TYPE && c.SEX == t1.CL_SEX).ToList();
                        }
                    }
                }

                
            }
#endregion
        }
        public String getT1WhereStatement(TriggerT1 t1)
        {
            String whereStatement = "";
            DatabaseContext db = new DatabaseContext();
           // IQueryable<Client> cli = from m in db.Clients where m.
            if(t1.CL_AGE_SIGN!=-1)
            {
                switch(t1.CL_AGE_SIGN)
                {
                    case(0):    // Если знак >

                        break;
                }
                DateTime now = DateTime.Now.AddYears(t1.CL_AGE*-1);

                whereStatement += "m.BIRTHDAY < CONVERT(DATETIME2, '1980-09-02') and q.BIRTHDAY != CONVERT(DATETIME2, '0001-01-01')";
            }

            return whereStatement;
        }
    }
}
