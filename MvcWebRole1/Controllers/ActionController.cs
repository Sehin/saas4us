using MvcWebRole1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MvcWebRole1.Controllers
{
    public class ActionController : Controller
    {
        //
        // GET: /Action/

        public ActionResult Index()
        {
            return View();
        }

        public void testId(int ID_ACTION)
        {
            //execute(ID_ACTION);
        }

        public void executeById(int ID_ACTION)
        {
            DatabaseContext db = new DatabaseContext();
            // В первую очередь необходимо определить тип данного Action
            int type = db.Actions.Where(a => a.ID_ACTION == ID_ACTION).Select(a => a.type).Single();

            switch (type)   // Выбираем функцию для обработки Action
            {
                case 0:
                    executeT0(ID_ACTION);
                    break;
            }
        }
        public void execute(String id)
        {
            List<String> ids = id.Split(',').ToList();
            foreach (String id_ in ids)
            {
                executeById(int.Parse(id_));
            }
        }
        public void executeT0(int ID_ACTION)
        {
            // T0 - empty
            // Просто перекинуть всех клиентов дальше
            DatabaseContext db = new DatabaseContext();
            List<Arrows> arrows = db.Arrows.Where(a => a.ID_FROM == ID_ACTION).ToList();
            if (arrows.Count>1)
            {
                ActionWorker.doSplitterArrowStep(arrows[0].ID_ARROW);
            }
            else
            {
                ActionWorker.doNextArrowStep(arrows[0].ID_ARROW);
            }
        }       
        public void executeT1(int ID_ACTION)
        {
            DatabaseContext db = new DatabaseContext();
            int ID_PR = db.Actions.Where(a => a.ID_ACTION == ID_ACTION).Select(a => a.ID_PR).Single();
            int ID_USER = db.MarkPrograms.Where(mp => mp.ID_PR == ID_PR).Select(mp => mp.ID_USER).Single();
            int Type = db.T1Actions.Where(a => a.ID_ACTION == ID_ACTION).Select(a => a.TYPE).Single();

            List<int> ids = db.ClientInMps.Where(c => c.ID_ACTION == ID_ACTION).Select(c => c.ID_CL).ToList();

            foreach (int id in ids)
            {
                Client client = db.Clients.Where(c => c.ID_CL == id).Single();
                client.TYPE = Type;
            }
            db.SaveChanges();

            List<Arrows> arrows = db.Arrows.Where(a => a.ID_FROM == ID_ACTION).ToList();
            if (arrows.Count > 1)
            {
                ActionWorker.doSplitterArrowStep(arrows[0].ID_ARROW);
            }
            else
            {
                ActionWorker.doNextArrowStep(arrows[0].ID_ARROW);
            }

        }
        public void executeT2(int ID_ACTION)    // mail
        {
            DatabaseContext db = new DatabaseContext();
            int ID_PR = db.Actions.Where(a => a.ID_ACTION == ID_ACTION).Select(a => a.ID_PR).Single();
            int ID_USER = db.MarkPrograms.Where(mp => mp.ID_PR == ID_PR).Select(mp => mp.ID_USER).Single();
            User user = db.Users.Where(u => u.Id == ID_USER).Single();
            String email = user.Email_sender;
            String pass = user.Email_sender_pass;

            String mailProvider = (email.Split('@').ToArray())[1];

            #region Определение smtp сервера
            Tuple<String, int> smtp = new Tuple<string,int>("",0);
            String login = email;
            if (mailProvider.Equals("gmail.com"))
            { smtp = new Tuple<string, int>("smtp.gmail.com", 25);}
            else if (mailProvider.Equals("rambler.ru"))
            { smtp = new Tuple<string, int>("mail.rambler.ru", 25);}
            else if (mailProvider.Equals("yandex.ru"))
            { smtp = new Tuple<string, int>("mail.yandex.ru", 25); login = (email.Split('@').ToArray())[0]; }
            else if (mailProvider.Equals("mail.ru"))
            { smtp = new Tuple<string, int>("smtp.mail.ru", 25); }
            #endregion
            SmtpClient Smtp = new SmtpClient(smtp.Item1, smtp.Item2);
            Smtp.Credentials = new NetworkCredential(login, pass);
            Smtp.EnableSsl = true;

            //Формирование письма
            T2Action t2action = db.T2Actions.Where(a => a.ID_ACTION == ID_ACTION).Single();
            Content content = db.Contents.Where(c => c.ID_CO == t2action.ID_CO).Single();
            MailMessage message = new MailMessage();
            message.From = new MailAddress(email);
            message.Subject = content.CONTENT_TITLE;
            message.Body = content.CONTENT_TEXT;
            String adresses = "";
            List<int> clientIds = new List<int>();
            foreach (ClientInMP cimp in db.ClientInMps.Where(c=>c.ID_ACTION==ID_ACTION))
            {
                clientIds.Add(cimp.ID_CL);
            }
            foreach(int id in clientIds)
            {
                String mail = db.Clients.Where(c => c.ID_CL == id).Select(c => c.MAIL).Single();
                adresses += mail + ",";
            }
            adresses = adresses.Remove(adresses.Count()-1);
            message.To.Add(adresses);
            Smtp.Send(message);//отправка


            List<Arrows> arrows = db.Arrows.Where(a => a.ID_FROM == ID_ACTION).ToList();
            if (arrows.Count > 1)
            {
                ActionWorker.doSplitterArrowStep(arrows[0].ID_ARROW);
            }
            else
            {
                ActionWorker.doNextArrowStep(arrows[0].ID_ARROW);
            }
        }
        
    }

    public static class ActionWorker
    {
        public static void doNextArrowStep(int ID_ARROW, List<int> ids = null)
        {
            DatabaseContext db = new DatabaseContext();
            int arrowType = db.Arrows.Where(a => a.ID_ARROW == ID_ARROW).Select(a => a.TYPE).Single();
            int ID_ACTION_FROM = db.Arrows.Where(a => a.ID_ARROW == ID_ARROW).Select(a => a.ID_FROM).Single();
            int ID_ACTION_TO = db.Arrows.Where(a => a.ID_ARROW == ID_ARROW).Select(a => a.ID_TO).Single();
            int ID_MP = db.Arrows.Where(a=> a.ID_ARROW == ID_ARROW).Select(a=>a.ID_PR).Single();
            if (ids == null)
            {
                ids = db.ClientInMps.Where(c => c.ID_ACTION == ID_ACTION_FROM).Select(c => c.ID_CL).ToList();
            }

            switch (arrowType)
            {
                // Если тип стрелки - 0 (empty)
                case 0:
                    foreach (int id in ids)
                    {
                        if (db.ClientInMps.Where(c => c.ID_CL == id && c.ID_ACTION == ID_ACTION_FROM).Count() == 0)
                        {
                            ClientInMP cimp = new ClientInMP(ID_MP, id, ID_ACTION_TO);
                            db.ClientInMps.Add(cimp);
                        }
                        else
                        {
                            ClientInMP cimp = db.ClientInMps.Where(c => c.ID_CL == id && c.ID_ACTION == ID_ACTION_FROM).Single();
                            cimp.ID_ACTION = ID_ACTION_TO;
                        }
                    }
                    db.SaveChanges();
                    JobWorker.createNewJob(ID_ACTION_TO,0);
                    break;
                case 2:
                    foreach (int id in ids)
                    {
                        if (db.ClientInMps.Where(c => c.ID_CL == id && c.ID_ACTION == ID_ACTION_FROM).Count() == 0)
                        {
                            ClientInMP cimp = new ClientInMP(ID_MP, id, ID_ACTION_TO);
                            db.ClientInMps.Add(cimp);
                        }
                        else
                        {
                            ClientInMP cimp = db.ClientInMps.Where(c => c.ID_CL == id && c.ID_ACTION == ID_ACTION_FROM).Single();
                            cimp.ID_ACTION = ID_ACTION_TO;
                        }
                    }
                    db.SaveChanges();
                    int hours = db.T2Arrow.Where(a=>a.ID_ARROW==ID_ARROW).Select(a=>a.HOURS).Single();
                    JobWorker.createNewJob(ID_ACTION_TO,hours);
                    break;
                 
            }
        }
        public static void doSplitterArrowStep(int ID_ARROW, List<int> ids = null)
        {
            DatabaseContext db = new DatabaseContext();
            Arrows arrow = db.Arrows.Where(a=>a.ID_ARROW==ID_ARROW).Single();

            List<int> arrowsIds = db.Arrows.Where(a => a.ID_FROM == arrow.ID_FROM).Select(a => a.ID_ARROW).ToList();
            if (ids == null)
            {
                ids = db.ClientInMps.Where(a => a.ID_ACTION == arrow.ID_FROM).Select(a => a.ID_CL).ToList();
            }
            List<List<int>> splitteredIds = getSplittedIds(ids, arrowsIds);
            int i = 0;
            List<int> actionIds = new List<int>();
            foreach (int arrowId in arrowsIds)
            {
                int actionId = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_TO).Single();
                foreach (int ID_CL in splitteredIds[i])
                {
                    int ID_ACTION_FROM = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_FROM).Single();
                    int ID_ACTION_TO = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_TO).Single();
                    ClientInMP cimp = db.ClientInMps.Where(c => c.ID_CL == ID_CL && c.ID_ACTION == ID_ACTION_FROM).Single();
                    cimp.ID_ACTION = ID_ACTION_TO; // Меняем ID_ACTION на следующий
                }
                i++;
                actionIds.Add(actionId);

            }
            db.SaveChanges();
            // Т.к. в данном типе стрелок нет временного параметра - то можно создать один Job для всех последующих Action
            JobWorker.createNewJob(actionIds);

        }
        public static List<List<int>> getSplittedIds(List<int> ids, List<int> arrowIds)
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
    }
}
