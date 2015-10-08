using MvcWebRole1.Models;
using NLog;
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

            #region Выделение ids
            foreach (T1Trigger t1 in T1Triggers)
            {
                int userId = db.MarkPrograms.Where(m => m.ID_PR == t1.ID_PR).Select(m => m.ID_USER).Single();
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
                    if (t1.CL_AGE_SIGN == 0)
                        idAge = db.Clients.Where(c => c.BIRTHDAY < now && c.BIRTHDAY != new DateTime(1, 1, 1) && c.ID_USER == userId).Select(c => c.ID_CL).ToList();
                    if (t1.CL_AGE_SIGN == 1)
                        idAge = db.Clients.Where(c => c.BIRTHDAY > now && c.BIRTHDAY.Year != 9999 && c.ID_USER == userId).Select(c => c.ID_CL).ToList();
                    if (t1.CL_AGE_SIGN == 2)
                        idAge = db.Clients.Where(c => c.BIRTHDAY.Year == now.Year && c.ID_USER == userId).Select(c => c.ID_CL).ToList();
                }

                if (t1.CL_SEX != -1)  // Пол указан
                {
                    sex = true;
                    idSex = db.Clients.Where(c => c.SEX == t1.CL_SEX && c.ID_USER == userId).Select(c => c.ID_CL).ToList();
                }

                if (t1.CL_TYPE != 1) // Тип указан
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

                if (age)
                {
                    List<int> ageTempIds = new List<int>();
                    foreach (int id in idAge)
                    {
                        if (ids.Contains(id))
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
                List<int> idsToRemove = new List<int>();
                foreach (int id in ids)
                {
                    if(db.ClientInMps.Where(c => c.ID_CL == id && c.ID_MP == t1.ID_PR).Count()>0)
                    {
                        idsToRemove.Add(id);
                    }
                }
                foreach(int id in idsToRemove)
                {
                    ids.Remove(id);
                }
                // Оставить ids которых еще не было в данной MP
                
            #endregion

                List<int> firstArrowsIds = getFirstArrowsIds(t1.ID_PR);
                int firstArrowId = firstArrowsIds[0];
                int arrowsType = db.Arrows.Where(a => a.ID_ARROW == firstArrowId).Select(a => a.TYPE).Single();

                if (firstArrowsIds.Count > 1)
                {
                    ActionWorker.doSplitterArrowStep(firstArrowId, ids);
                }
                else
                {
                    ActionWorker.doNextArrowStep(firstArrowId,ids);
                }


                /*switch (arrowsType)
                {
                    // Если тип стрелки - 1 (random)
                    case 1:
                        List<List<int>> splitteredIds = getSplittedIds(ids, firstArrowsIds);
                        int i = 0;
                        List<int> actionIds = new List<int>();
                        foreach (int arrowId in firstArrowsIds)
                        {
                            int actionId = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_TO).Single();
                            actionIds.Add(actionId);
                            foreach (int ID_CL in splitteredIds[i])
                            {
                                int ID_ACTION = db.Arrows.Where(a=>a.ID_ARROW==arrowId).Select(a=>a.ID_TO).Single();
                                ClientInMP cimp = new ClientInMP(t1.ID_PR, ID_CL, ID_ACTION);   // Привязываем клиентов к MP и к конкретному Action
                                if(db.ClientInMps.Where(c => c.ID_CL == cimp.ID_CL && c.ID_MP == cimp.ID_MP).Count()==0)    // Если клиента нет в этой MP
                                    db.ClientInMps.Add(cimp);
                            }
                            i++;
                            // Т.к. в данном типе стрелок нет временного параметра - то можно создать один Job для всех последующих Action
                        }
                        if (ids.Count > 0)
                        {
                            db.SaveChanges();
                            JobWorker.createNewJob(actionIds);
                        }
                        JobWorker.createNewJob(actionIds);

                        break;
                }*/
            }   
            #endregion
        }
        [HttpPost]
        public void checkT3Triggers(String token, String ID_VK, String ID_FB, String MAIL="", String MOBILE_NUMBER="")
        {
            DatabaseContext db = new DatabaseContext();

            // Определение триггера по токену
            T3Trigger tr;
            if(db.T3Trigger.Where(t=>t.TOKEN==token).Count()>0)
            {
                tr=db.T3Trigger.Where(t=>t.TOKEN==token).Single();
            }
            else
            {
                return;
            }
            // Проверка нет ли уже клиента с параметрами, которые были переданы
            int count = db.Clients.Where(c => c.ID_VK == ID_VK || c.ID_FB == ID_FB).Count();
            Client client = new Client();
            if (count == 0)  // Если нет
            {
                int ID_USER = db.MarkPrograms.Where(m => m.ID_PR == tr.ID_PR).Select(m => m.ID_USER).Single();
                client = ClientWorker.addNewClient(ID_VK, ID_FB, MAIL, MOBILE_NUMBER,ID_USER);
                db.Clients.Add(client);
                db.SaveChanges();
            }
            else
            {
                client = ClientWorker.findClientByParams(ID_VK: ID_VK, ID_FB: ID_FB);
                if (MAIL != null)
                    client.MAIL = MAIL;
                if (MOBILE_NUMBER != null)
                    client.MOBILE_NUMBER = MOBILE_NUMBER;
            }


            List<int> firstArrowsIds = getFirstArrowsIds(tr.ID_PR);
            int firstArrowId = firstArrowsIds[0];
            int arrowsType = db.Arrows.Where(a => a.ID_ARROW == firstArrowId).Select(a => a.TYPE).Single();
            List<int> ids = new List<int>();
            ids.Add(client.ID_CL);
            if (firstArrowsIds.Count > 1)
            {
                ActionWorker.doSplitterArrowStep(firstArrowId, ids);
            }
            else
            {
                ActionWorker.doNextArrowStep(firstArrowId, ids);
            }
        }
        public void testIt()
        {
           /* List<int> ids = new List<int>();
            for (int i = 0; i < 16; i++)
            {
                Random q = new Random(i * DateTime.Now.Second);
                ids.Add(q.Next(10000));
            }
            DatabaseContext db = new DatabaseContext();
            T1Trigger t1 = db.T1Trigger.Where(t => t.ID_TT1 == 2).Single();
            List<int> firstArrowsIds = getFirstArrowsIds(t1.ID_PR);
            List<List<int>> qwerty = getSplittedIds(ids, firstArrowsIds);*/
            
        }
        public List<int> getFirstArrowsIds(int ID_PR)
        {
            DatabaseContext db = new DatabaseContext();
            return db.Arrows.Where(a => a.ID_PR == ID_PR && a.ID_FROM == 1).Select(a => a.ID_ARROW).ToList();

        }
        
        public List<List<int>> getSplittedIds(List<int> ids, List<int> arrowIds)
        {
            DatabaseContext db = new DatabaseContext();
            List<double> chances = new List<double>();

            List<int> firstAIds = new List<int>();

            foreach (int id in arrowIds)
            {
                chances.Add(db.T1Arrow.Where(a => a.ID_ARROW == id).Select(a => a.CHANCE).Single());
            }
            // Получили массив с процентами (в сумме должно быть 100 :) )
            int clientCount = ids.Count;
            List<int> countForChance = new List<int>();
            foreach (float chance in chances)
            {
                countForChance.Add((int)(clientCount * chance));
            }

            int q = 0;
            List<List<int>> splittedList = new List<List<int>>();
            foreach (int count in countForChance)
            {
                List<int> splittedPart = new List<int>();
                for (int i = q; i < q + count; i++)
                {
                    splittedPart.Add(ids[i]);
                }
                q += count;
                splittedList.Add(splittedPart);
            }
            int totalIdsCount = 0;
            foreach (int count in countForChance)
            {
                totalIdsCount += count;
            }
            if (clientCount != totalIdsCount)
            {
                splittedList[0].Add(ids.ElementAt(ids.Count - 1));
            }
            return splittedList;
        }
       /* public List<List<int>> getSplittedIds_new(List<int> ids, List<int> arrowIds)
        {
            List<List<int>> splittedIds = new List<List<int>>();
            DatabaseContext db = new DatabaseContext();
            List<double> chances = new List<double>();

            List<int> firstAIds = new List<int>();

            foreach (int id in arrowIds)
            {
                chances.Add(db.T1Arrow.Where(a => a.ID_ARROW == id).Select(a => a.CHANCE).Single());
            }
            // Получили массив с процентами (в сумме должно быть 100 :) )

            List<int> randomChances = getRandomChances(ids.Count);

            for (int i = 0; i < ids.Count;i++)
            {
                
            }

                return null;
        }


        public List<int> getRandomChances(int count)
        {
            List<int> chances = new List<int>();
            for (int i = 0; i < count; i++)
            {
                Random q = new Random(DateTime.Now.Millisecond);
                int chance = q.Next(0, 100);

                if(i>0)
                {
                    if (chance != chances[i - 1])
                        chances.Add(chance);
                    else
                    {
                        i--;
                        continue;
                    }
                }
                if (i == 0)
                    chances.Add(chance);
            }
            return chances;

        }
        public int associateChanceWithSplitterChances(int chance, List<double> chances)
        {
            
            for (int i=0;i<chances.Count;i++)
            {
                int sum = 0;
                sum += chances[i];
                if (chance<chances[i])
                {
                    return i;
                }
                else
                {

                }
            }
        }*/
    }
public static class TriggerWorker
{
    public static String getNewT3Token()
    {
        DatabaseContext db = new DatabaseContext();
        String token = "";

        for(int i=0;i<50;i++)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            int c = r.Next(48,91);
            if (token.Length > 0)
            {
                if (token[token.Length - 1] == Convert.ToChar(c))
                {
                    i--; continue;
                }
                else
                    token += Convert.ToChar(c);
            }
            else
                token += Convert.ToChar(c);

        }
        
        if (db.T3Trigger.Where(t => t.TOKEN == token).Count() > 0)
            return getNewT3Token();
        else
            return token;
    }
}
}
