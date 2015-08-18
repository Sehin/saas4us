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
            List<T1Trigger> T1Triggers = db.T1Trigger.ToList();


            foreach (T1Trigger t1 in T1Triggers)
            {
                 Trigger trigger = db.Triggers.Where(t=>t.ID_TR==t1.ID_TR).Single();
                MarkProgram mp = db.MarkPrograms.Where(m => m.ID_PR == trigger.ID_PR).Single();
                int userId = mp.ID_USER;
                bool age = false;
                bool sex = false;
                bool type = false;

                List<int> idAge = null;
                List<int> idSex = null;
                List<int> idType = null;

                if (t1.CL_AGE_SIGN != -1) // Возраст указан
                {
                    age = true;
                    DateTime now = DateTime.Now.AddYears(t1.CL_AGE * -1);
                    if (t1.CL_AGE_SIGN==0)
                        idAge = db.Clients.Where(c => c.BIRTHDAY < now && c.BIRTHDAY!=new DateTime(1,1,1) && c.ID_USER==userId).Select(c => c.ID_CL).ToList();
                    if (t1.CL_AGE_SIGN == 1)
                        idAge = db.Clients.Where(c => c.BIRTHDAY > now && c.BIRTHDAY.Year != 9999 && c.ID_USER == userId).Select(c => c.ID_CL).ToList();
                    if (t1.CL_AGE_SIGN == 2)
                        idAge = db.Clients.Where(c => c.BIRTHDAY.Year == now.Year && c.ID_USER == userId).Select(c => c.ID_CL).ToList();
                }

                if (t1.CL_SEX!=-1)  // Пол указан
                {
                    sex = true;
                    idSex = db.Clients.Where(c => c.SEX == t1.CL_SEX && c.ID_USER == userId).Select(c => c.ID_CL).ToList();
                }

                if (t1.CL_TYPE!=1) // Тип указан
                {
                    type = true;
                    idType = db.Clients.Where(c => c.TYPE == t1.CL_TYPE && c.ID_USER == userId).Select(c => c.ID_CL).ToList();
                }

                List<int> ids = new List<int>();


                if (age)
                {
                    ids = idAge;
                    age = false;
                }
                else if (sex)
                {
                    ids = idSex;
                    sex = false;
                }
                else if (type)
                {
                    ids = idType;
                    type = false;
                }

                if(age)
                {
                    List<int> ageTempIds = new List<int>();
                    foreach(int id in idAge)
                    {
                        if(ids.Contains(id))
                        {
                            ageTempIds.Add(id);
                        }
                    }
                    ids = ageTempIds;
                }
                
                if (sex)
                {
                    List<int> sexTempIds = new List<int>();

                    foreach (int id in idSex)
                    {
                        if (ids.Contains(id))
                        {
                            sexTempIds.Add(id);
                        }
                    }
                    ids = sexTempIds;
                }

                if (type)
                {
                    List<int> typeTempIds = new List<int>();

                    foreach (int id in idType)
                    {
                        if (ids.Contains(id))
                        {
                            typeTempIds.Add(id);
                        }
                    }
                    ids = typeTempIds;
                }

                //ClientInMP cim = new ClientInMP()

            }
            #endregion
        }
        public int getFirstActionId(MarkProgram mp)
        {
            DatabaseContext db = new DatabaseContext();
            List<int> actionids = db.Actions.Where(a => a.ID_PR == mp.ID_PR).Select(a => a.ID_ACTION).ToList();
            foreach (int id in actionids)
            {

            }
        }
    
    }
}
