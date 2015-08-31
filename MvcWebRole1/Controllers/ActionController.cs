using MvcWebRole1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            execute(ID_ACTION);
        }

        public void execute(int ID_ACTION)
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
    }

    public static class ActionWorker
    {
        public static void doNextArrowStep(int ID_ARROW)
        {
            DatabaseContext db = new DatabaseContext();
            int arrowType = db.Arrows.Where(a => a.ID_ARROW == ID_ARROW).Select(a => a.TYPE).Single();
            switch (arrowType)
            {
                // Если тип стрелки - 1 (random)
                case 1:
                    List<List<int>> splitteredIds = getSplittedIds(ids, firstArrowsIds);
                    int i = 1;
                    foreach (int arrowId in firstArrowsIds)
                    {
                        int actionId = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_TO).Single();
                        foreach (int ID_CL in splitteredIds[i])
                        {
                            int ID_ACTION = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_TO).Single();
                            ClientInMP cimp = new ClientInMP(t1.ID_PR, ID_CL, ID_ACTION);   // Привязываем клиентов к MP и к конкретному Action
                        }
                        i++;
                        // Т.к. в данном типе стрелок нет временного параметра - то можно создать один Job для всех последующих Action
                    }
                    break;
            }
        }
        public static void doSplitterArrowStep(int ID_ARROW)
        {
            DatabaseContext db = new DatabaseContext();
            Arrows arrow = db.Arrows.Where(a=>a.ID_ARROW==ID_ARROW).Single();

            List<int> arrowsIds = db.Arrows.Where(a => a.ID_ARROW == arrow.ID_ARROW).Select(a => a.ID_ARROW).ToList();

            List<int> ids = db.ClientInMp.Where(a => a.ID_ACTION == arrow.ID_FROM).Select(a => a.ID_CL).ToList();

            List<List<int>> splitteredIds = getSplittedIds(ids, arrowsIds);
            int i = 1;
            foreach (int arrowId in arrowsIds)
            {
                int actionId = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_TO).Single();
                foreach (int ID_CL in splitteredIds[i])
                {
                    int ID_ACTION_FROM = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_FROM).Single();
                    int ID_ACTION_TO = db.Arrows.Where(a => a.ID_ARROW == arrowId).Select(a => a.ID_TO).Single();

                    ClientInMP cimp = db.ClientInMp.Where(c => c.ID_CL == ID_CL && c.ID_ACTION == ID_ACTION_FROM).Single();
                    cimp.ID_ACTION = ID_ACTION_TO; // Меняем ID_ACTION на следующий
                }
                i++;
                // Т.к. в данном типе стрелок нет временного параметра - то можно создать один Job для всех последующих Action
            }
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
