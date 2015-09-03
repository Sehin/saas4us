using MvcWebRole1.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MvcWebRole1.Controllers
{
    public class ClientController : Controller
    {
        //
        // GET: /Client/

        public ActionResult Index()
        {

            return View();
        }
        public void updateSocClients_FB() // обновляем клиентов из Facebook
        {
            DatabaseContext db = new DatabaseContext();
            List<SocAccount> socAccs = db.SocAccounts.Where(q => q.SOCNET_TYPE == 1).ToList();
            List<MvcWebRole1.Models.Group> groups = new List<MvcWebRole1.Models.Group>();
            // подгрузили соцакки для Facebook

            foreach (SocAccount sa in socAccs)
            {
                groups.AddRange(db.Groups.Where(q => q.ID_AC == sa.ID_AC).ToList());  //выберем все группы для отдельного соцакка
                foreach (MvcWebRole1.Models.Group group in groups)
                {
                    List<string> loadedIds = FBWorker.getGroupSubscribersIds(group.ID_GROUP, sa.TOKEN); //получим айди юзеров для отдельной группы

                    #region Отбираем новых пользователей
                    List<Client> allClients = db.Clients.ToList();
                    List<string> allClientIds = new List<string>();
                    foreach (Client c in allClients)
                    {
                        allClientIds.Add(c.ID_FB);   //получим всех пользователей из базы
                    }

                    List<String> newIds = new List<String>();
                    for (int i = 0; i < loadedIds.Count; i++)
                    {
                        for (int q = 0; q < allClientIds.Count; q++)
                        {
                            if (loadedIds[i] != allClientIds[q])   //выберем новых пользователей
                                newIds.Add(loadedIds[i]);
                            break;
                        }
                    }

                    //отсеим клиентов которые вышли и запишем дату выхода
                    List<int> leaveClientIds = new List<int>();

                    for (int i = 0; i < allClientIds.Count; i++)
                    {
                        bool isLeave = true;
                        for (int q = 0; q < loadedIds.Count; q++)
                        {
                            if (allClientIds[i] == loadedIds[q])
                                isLeave = false;
                        }
                        if (isLeave)
                        {
                            String clId = allClientIds[i];


                            if (!(clId.Equals("-1")))
                            {
                                Client client = db.Clients.Where(c => c.ID_FB == clId).Single();
                                DateTime tl = new DateTime(0001, 01, 01);
                                if (client.DATE_LEAVE.Equals(tl))
                                {
                                    client.DATE_LEAVE = DateTime.Now;
                                }
                            }
                        }
                    }
                    #endregion

                    #region загрузим данные ползователей с новыми айдишниками
                    for(int i = 0; i < newIds.Count; i++)
                    {

                        Client new_client = FBWorker.getClientById_FB(newIds[i], sa);
                        db.Clients.Add(new_client);
                    }
                    #endregion
                }
                groups.Clear();
            }
            db.SaveChanges();
        }

        // Метод, который обновляет клиентов из соц сети VK
        public void updateSocClients()
        {
            DatabaseContext db = new DatabaseContext();
            List<SocAccount> socAccs = db.SocAccounts.Where(s => s.SOCNET_TYPE == 0).ToList();
            List<MvcWebRole1.Models.Group> groups = new List<MvcWebRole1.Models.Group>();

            foreach (SocAccount sa in socAccs)  // с каждого соцАккаунта берем все группы и подгружаем их в общий лист
            {
                groups.AddRange(db.Groups.Where(g => g.ID_AC == sa.ID_AC).ToList());
            }

            foreach (MvcWebRole1.Models.Group group in groups)
            {
                List<String> ids = VKWorker.getGroupSubscribersIds(group.ID_GROUP);
                SocAccount sa = db.SocAccounts.Where(g => g.ID_AC == group.ID_AC).Single();

                #region Отсеиваем только новых клиентов
                List<Client> clients = db.Clients.ToList(); // Получение списка всех клиентов
                List<String> clientIds = new List<String>();
                foreach (Client client in clients)
                {
                    clientIds.Add(client.ID_VK); 
                }

                List<String> idsToRemove = new List<String>();
                for (int i = 0; i < ids.Count; i++)
                {
                    for (int q = 0; q < clientIds.Count; q++)
                    {
                        if (ids[i] == clientIds[q])  
                            idsToRemove.Add(ids[i]);
                    }
                }

                // Если человек вышел из группы, необходимо записать это в его DATE_LEAVE
                // todo НЕ ОТЛАЖИВАЛОСЬ (debug need)
                List<int> leaveClientIds = new List<int>();

                for (int i = 0; i < clientIds.Count; i++)
                {
                    bool isLeave = true;
                    for (int q = 0; q < ids.Count; q++)
                    {
                        if (clientIds[i] == ids[q])
                            isLeave = false;
                    }
                    if (isLeave)
                    {
                        String clId = clientIds[i];
                        Client client = db.Clients.Where(c => c.ID_VK == clId).Single();
                        DateTime tl = new DateTime(0001, 01, 01);
                        if (client.DATE_LEAVE.Equals(tl))
                        {
                            client.DATE_LEAVE = DateTime.Now;
                        }
                    }
                }

                // Удаление уже присутствующих id
                foreach (String id in idsToRemove)
                {
                    ids.Remove(id);
                }
                #endregion


                // В случае если ids.count > i, то необходимо разбить его на отдельные list, чтобы можно было проводить запрос к api
                if (ids.Count > 0)
                {
                    List<List<String>> ids_ = splitList(ids, 400);

                    foreach (List<String> list in ids_)
                    {
                        // Преобразуем list в строку вида "id, id, id" для дальнейшего запроса к api
                        String sIds = listToString(list);
                        // Обращаясь к api получаем все все данные, формируем лист Client
                        List<Client> clientList = VKWorker.getClientsByIds(sIds, sa);
                        // Добавляем в базу всех клиентов, профит
                        foreach (Client cl in clientList)
                        {
                            db.Clients.Add(cl);
                        }

                    }
                }
                db.SaveChanges();

            }
        }
        // Метод, который анализирует лайки и комментарии VK
        public void updateVkActions()
        {
            int contentCount = 20; // КОЛИЧЕСТВО ПОСТОВ, КОТОРЫЕ БУДУТ БРАТЬСЯ ПОСЛЕДНИМИ ИЗ ГРУПП ДЛЯ ДОБАВЛЕНИЯ ИНФЫ О l/c/r
            DatabaseContext db = new DatabaseContext();

            updateContentInGroups();


            // Получаем Like/Comment/Repost за последние N CIG.
            List<SocAccount> socAccs = db.SocAccounts.Where(s => s.SOCNET_TYPE == 0).ToList();
            List<MvcWebRole1.Models.Group> groups = new List<MvcWebRole1.Models.Group>();

            foreach (SocAccount sa in socAccs)  // с каждого соцАккаунта берем все группы и подгружаем их в общий лист
            {
                groups.AddRange(db.Groups.Where(g => g.ID_AC == sa.ID_AC).ToList());
            }
            // Получаем N последних CIG 
            foreach (var group in groups)
            {
                List<ContentInGroup> lastNCigs = new List<ContentInGroup>();
                List<ContentInGroup> cigs = db.ContentsInGroups.Where(c => c.ID_GROUP == group.ID).ToList();
                for (int i = 0; i < contentCount; i++)
                {
                    ContentInGroup latestCig = new ContentInGroup(0, 0, 0, new DateTime(1, 1, 1));
                    foreach (ContentInGroup cig in cigs)
                    {
                        if (CompareWithoutSeconds(cig.POST_TIME, latestCig.POST_TIME) == 1)
                        {
                            latestCig = cig;
                        }
                    }
                    lastNCigs.Add(latestCig);
                    cigs.Remove(latestCig);
                }
                foreach (ContentInGroup cig in lastNCigs)
                {
                    #region likes
                    //todo Для каждого CIG запилить бы отдельный Thread.. к вопросу о быстродействии :)
                    List<String> likesForPost = VKWorker.getLikeIdsFromPost(group.ID_GROUP, cig.ID_POST);
                    foreach (String clientVkId in likesForPost)
                    {
                        int isHaveThisClient = db.Clients.Where(c => c.ID_VK == clientVkId).Count();
                        if (isHaveThisClient == 0) // Нет такого клиента
                        {
                            SocAccount sa = db.SocAccounts.Where(s => s.ID_AC == group.ID_AC).Single();
                            Client client = addNewClientByVKId(clientVkId, sa); // Добавили такого клиента в БД
                            ClientLike clL = new ClientLike(cig.ID_CIG, client.ID_CL);
                            db.ClientLikes.Add(clL);
                        }
                        else // Есть такой клиент
                        {
                            Client client = db.Clients.Where(c => c.ID_VK == clientVkId).Single();
                            int a = db.ClientLikes.Where(c => c.ID_CL == client.ID_CL && c.ID_CIG == cig.ID_CIG).Count();
                            if (a == 0)
                            {
                                ClientLike clL = new ClientLike(cig.ID_CIG, client.ID_CL);
                                db.ClientLikes.Add(clL);
                            }
                        }
                    }

                    #endregion

                    #region repost
                    List<String> repostForPost = VKWorker.getRepostIdsFromPost(group.ID_GROUP, cig.ID_POST);
                    foreach (String clientVkId in repostForPost)   // Для каждого id ищем клиента в БД
                    {
                        if (Int64.Parse(clientVkId) < 0)
                            continue;
                        int isHaveThisClient = db.Clients.Where(c => c.ID_VK == clientVkId).Count();
                        if (isHaveThisClient == 0) // Нет такого клиента
                        {
                            SocAccount sa = db.SocAccounts.Where(s => s.ID_AC == group.ID_AC).Single();
                            Client client = addNewClientByVKId(clientVkId, sa); // Добавили такого клиента в БД
                            ClientRepost clR = new ClientRepost(cig.ID_CIG, client.ID_CL);
                            db.ClientReposts.Add(clR);
                        }
                        else
                        {
                            Client client = db.Clients.Where(c => c.ID_VK == clientVkId).Single();  // Exception если нет
                            int a = db.ClientReposts.Where(c => c.ID_CL == client.ID_CL && c.ID_CIG == cig.ID_CIG).Count();
                            if (a == 0)
                            {
                                ClientRepost clR = new ClientRepost(cig.ID_CIG, client.ID_CL);
                                db.ClientReposts.Add(clR);
                            }
                        }
                    }
                    #endregion

                    #region comments
                    List<Tuple<String, int>> commentIdsForPost = VKWorker.getCommentIdsFromPost(group.ID_GROUP, cig.ID_POST);

                    foreach (Tuple<String, int> tpl in commentIdsForPost)
                    {
                        if (Int64.Parse(tpl.Item1) < 0)
                            continue;
                        int isHaveThisClient = db.Clients.Where(c => c.ID_VK == tpl.Item1).Count();
                        if (isHaveThisClient == 0) // Нет такого клиента
                        {
                            SocAccount sa = db.SocAccounts.Where(s => s.ID_AC == group.ID_AC).Single();
                            Client client = addNewClientByVKId(tpl.Item1, sa); // Добавили такого клиента в БД
                            ClientComment clC = new ClientComment(cig.ID_CIG, client.ID_CL, tpl.Item2);
                            db.ClientComments.Add(clC);
                        }
                        else
                        {
                            Client client = db.Clients.Where(c => c.ID_VK == tpl.Item1).Single();  // Exception если нет
                            int a = db.ClientReposts.Where(c => c.ID_CL == client.ID_CL && c.ID_CIG == cig.ID_CIG).Count();
                            if (a == 0)
                            {
                                ClientComment clC = new ClientComment(cig.ID_CIG, client.ID_CL, tpl.Item2);
                                db.ClientComments.Add(clC);
                            }
                        }
                    }

                    db.SaveChanges();
                    #endregion
                }
            }




        }

        public Client addNewClientByVKId(String idVk, SocAccount sa)
        {
            DatabaseContext db = new DatabaseContext();
            if (Int64.Parse(idVk) < 0)
                return null;
            try
            {
                Client cl = db.Clients.Where(c => c.ID_VK == idVk).Single(); // Если такой клиент уже найден - false
                return null;
            }
            catch (Exception e)  // Создаем нового
            {
                Client client = VKWorker.getClientById(idVk, sa);
                db.Clients.Add(client);
                db.SaveChanges();
                return client;
            }
        }

        public void testIt()
        {
            //VKWorker.getPostIdsFromGroup(30022666);
            //       VKWorker.getLikeIdsFromPost(30022666, 115376);
            //VKWorker.getCommentIdsFromPost(87953130, 53);
            updateSocClients_FB();
           
        }
        private List<List<String>> splitList(List<String> list, int size)
        {
            if (list.Count < size)  // Если размер листа меньше минимального, возвращаем его
            {
                List<List<String>> rList = new List<List<String>>();
                rList.Add(list);
                return rList;
            }
            else
            {
                List<List<String>> rList = new List<List<String>>();
                int i = 0;
                while (i < list.Count)
                {
                    List<String> smallList;
                    try
                    {
                        smallList = list.GetRange(i, size);
                    }
                    catch (Exception e)
                    {
                        smallList = list.GetRange(i, list.Count - i);
                    }
                    rList.Add(smallList);
                    i += size;
                }
                return rList;
            }
        }
        private String listToString(List<String> list)
        {
            String rList = "";
            for (int i = 0; i < list.Count; i++)
            {
                if (i + 1 != list.Count)
                {
                    rList += list[i] + ",";
                }
                else
                    rList += list[i];
            }
            return rList;
        }
        private void updateContentInGroups()
        {

            DatabaseContext db = new DatabaseContext();
            List<SocAccount> socAccs = db.SocAccounts.Where(s => s.SOCNET_TYPE == 0).ToList();
            List<MvcWebRole1.Models.Group> groups = new List<MvcWebRole1.Models.Group>();


            foreach (SocAccount sa in socAccs)  // с каждого соцАккаунта берем все группы и подгружаем их в общий лист
            {
                groups.AddRange(db.Groups.Where(g => g.ID_AC == sa.ID_AC).ToList());
                // Создаем мастер-контент с type=-1
                try
                {
                    db.Contents.Where(c => c.CONTENT_TYPE == -1).Single();
                }
                catch (Exception e)
                {
                    Content content = new Content(sa.ID_USER, "", "", -1);
                    db.Contents.Add(content);
                }
            }
            db.SaveChanges();

            foreach (var group in groups)
            {
                int idMasterContent = db.Contents.Where(c => c.CONTENT_TYPE == -1).Single().ID_CO;
                // Для каждой группы получаем список CIG
                List<ContentInGroup> cigsFromVk = VKWorker.getCIGfromGroup(group.ID_GROUP, group.ID, idMasterContent);

                //List<int> postIds = VKWorker.getPostIdsFromGroup(group.ID_GROUP);


                // Анализируем все посты в группе, если пост в группе не дублируется записью в ContentInGroup
                // То добавляем запись в ContentInGroups с ContentID = -1

                List<ContentInGroup> cigsFromDb = db.ContentsInGroups.Where(g => g.ID_GROUP == group.ID).ToList();
                List<int> cigIds = new List<int>();



                foreach (ContentInGroup cigVk in cigsFromVk)
                {
                    bool isHave = false;    // Этого поста нет в нашей БД
                    foreach (ContentInGroup cigDb in cigsFromDb)
                    {
                        if ((cigVk.ID_GROUP == cigDb.ID_GROUP) && (cigVk.ID_POST == cigDb.ID_POST))  // Если находим, то указываем что есть
                        {
                            isHave = true;
                            break;
                        }
                    }
                    if (!isHave) // Если поста не было - то добавляем новую запись в CIG
                    {
                        db.ContentsInGroups.Add(cigVk);
                    }
                }
                db.SaveChanges();
            }
        }
        private int CompareWithoutSeconds(DateTime d1, DateTime d2)
        {
            if (d1.Minute == d2.Minute && d1.Hour == d2.Hour && d1.Date == d2.Date)
            {
                return 0;
            }
            return d1.CompareTo(d2);
        }
    }

    public static class FBWorker
    {
        public static List<string> getGroupSubscribersIds(string _groupId, string _accessToken)
        {
            int offset = 0;
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String result = wc.DownloadString("https://graph.facebook.com/v2.4/" + _groupId + "/members?fields=id&offset=" + offset + "&limit=100&access_token=" + _accessToken);
            JObject obj = JObject.Parse(result);
            JToken jtoken = obj["data"].First;

            List<string> IDs = new List<string>();
            do
            {
                IDs.Add((string)jtoken["id"]);
                jtoken = jtoken.Next;
            }
            while (jtoken != null);

            return IDs;
        }

        public static Client getClientById_FB (String id, SocAccount sa)
        {
            Client loaded_client = new Client();
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String result = wc.DownloadString("https://graph.facebook.com/v2.4/" + id +"?access_token="+sa.TOKEN+"&fields=id,name,birthday,gender");
            
            JObject obj = JObject.Parse(result);
            JToken jtoken = obj["base"].First;


                String name = jtoken["name"].ToString();

                #region Парсим дату рождения
                DateTime birthday = new DateTime(0001, 01, 01);
                try
                {
                    String bday = jtoken["bdate"].ToString();

                    String regex = @"(\d*)\.(\d*)\.(\d*)";
                    Match m = Regex.Match(bday, regex);
                    if (m.Success)
                    {
                        birthday = new DateTime(Int16.Parse(m.Groups[3].ToString()), Int16.Parse(m.Groups[2].ToString()), Int16.Parse(m.Groups[1].ToString()));
                    }
                    else
                    {
                        regex = @"(\d*)\.(\d*)";
                        m = Regex.Match(bday, regex);
                        if (m.Success)
                        {
                            birthday = new DateTime(9999, Int16.Parse(m.Groups[2].ToString()), Int16.Parse(m.Groups[1].ToString()));
                        }
                    }

                }
                catch (Exception e)
                {
                    birthday = new DateTime(0001, 01, 01);
                }
                #endregion
                String fbId = jtoken["id"].ToString();
                DateTime time_come = DateTime.Now;
                DateTime time_leave = new DateTime(0001, 01, 01);
                int sex;
                int temp_sex = (int)jtoken["gender"];
                if (temp_sex.Equals("male"))
                    sex = 2;
                else sex = 1;
                loaded_client = new Client(sa.ID_USER, name, birthday, 0, "-1", fbId, time_come, "", time_leave, sex);


            return loaded_client;
        }


        }

    }

    public static class VKWorker
    {
        public static List<String> getGroupSubscribersIds(String groupId)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String answer = wc.DownloadString("https://api.vk.com/method/groups.getMembers?group_id=" + groupId);
            JObject obj = JObject.Parse(answer);
            JToken jtoken = obj["response"]["users"].First;

            List<String> ids = new List<String>();
            do
            {
                ids.Add(jtoken.ToString());
                jtoken = jtoken.Next;
            }
            while (jtoken != null);

            return ids;
        }
        public static List<Client> getClientsByIds(String ids, SocAccount sa)
        {
            List<Client> clients = new List<Client>();
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String answer = wc.DownloadString("https://api.vk.com/method/users.get?fields=bdate,sex&access_token=" + sa.TOKEN + "&user_ids=" + ids);
            JObject obj = JObject.Parse(answer);
            JToken jtoken = obj["response"].First;
            do
            {
                String name = jtoken["first_name"].ToString() + " " + jtoken["last_name"];

                #region bday parse
                DateTime birthday = new DateTime(0001, 01, 01);
                try
                {
                    String bday = jtoken["bdate"].ToString();

                    String regex = @"(\d*)\.(\d*)\.(\d*)";
                    Match m = Regex.Match(bday, regex);
                    if (m.Success)
                    {
                        birthday = new DateTime(Int16.Parse(m.Groups[3].ToString()), Int16.Parse(m.Groups[2].ToString()), Int16.Parse(m.Groups[1].ToString()));
                    }
                    else
                    {
                        regex = @"(\d*)\.(\d*)";
                        m = Regex.Match(bday, regex);
                        if (m.Success)
                        {
                            birthday = new DateTime(9999, Int16.Parse(m.Groups[2].ToString()), Int16.Parse(m.Groups[1].ToString()));
                        }
                    }

                }
                catch (Exception e)
                {
                    birthday = new DateTime(0001, 01, 01);
                }
                #endregion
                String vkId = jtoken["uid"].ToString();
                DateTime time_come = DateTime.Now;
                DateTime time_leave = new DateTime(0001, 01, 01);
                int sex = (int)jtoken["sex"];
                Client client = new Client(sa.ID_USER, name, birthday, 0, vkId, "-1", time_come, "", time_leave, sex);
                clients.Add(client);
                jtoken = jtoken.Next;
            }
            while (jtoken != null);
            return clients;
        }
        public static Client getClientById(String id, SocAccount sa)
        {
            if (Int64.Parse(id) < 0)
                return null;
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String answer = wc.DownloadString("https://api.vk.com/method/users.get?fields=bdate,sex&access_token=" + sa.TOKEN + "&user_ids=" + id);
            JObject obj = JObject.Parse(answer);
            JToken jtoken = obj["response"].First;
            String name = jtoken["first_name"].ToString() + " " + jtoken["last_name"];
            #region bday parse
            DateTime birthday = new DateTime(0001, 01, 01);
            try
            {
                String bday = jtoken["bdate"].ToString();

                String regex = @"(\d*)\.(\d*)\.(\d*)";
                Match m = Regex.Match(bday, regex);
                if (m.Success)
                {
                    birthday = new DateTime(Int16.Parse(m.Groups[3].ToString()), Int16.Parse(m.Groups[2].ToString()), Int16.Parse(m.Groups[1].ToString()));
                }
                else
                {
                    regex = @"(\d*)\.(\d*)";
                    m = Regex.Match(bday, regex);
                    if (m.Success)
                    {
                        birthday = new DateTime(9999, Int16.Parse(m.Groups[2].ToString()), Int16.Parse(m.Groups[1].ToString()));
                    }
                }

            }
            catch (Exception e)
            {
                birthday = new DateTime(0001, 01, 01);
            }
            #endregion
            String vkId = jtoken["uid"].ToString();
            DateTime time_come = DateTime.Now;
            DateTime time_leave = new DateTime(0001, 01, 01);
            int sex = (int)jtoken["sex"];
            Client client = new Client(sa.ID_USER, name, birthday, 0, vkId, "-1", time_come, "", time_leave, sex);
            jtoken = jtoken.Next;
            return client;
        }
        public static List<int> getPostIdsFromGroup(int groupId)
        {
            List<int> ids = new List<int>();
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String answer = wc.DownloadString("https://api.vk.com/method/wall.get?count=100&owner_id=-" + groupId);
            JObject obj = JObject.Parse(answer);
            JToken jtoken = obj["response"].First;
            int count = int.Parse(jtoken.ToString());
            int counter = 0;
            int counterM = 1;
            jtoken = jtoken.Next;
            do
            {
                ids.Add(int.Parse(jtoken["id"].ToString()));
                jtoken = jtoken.Next;
                counter++;
                if (counter == 100)
                {
                    answer = wc.DownloadString("https://api.vk.com/method/wall.get?count=100&owner_id=-" + groupId + "&offset=" + counterM * 100);
                    obj = JObject.Parse(answer);
                    jtoken = obj["response"].First;
                    jtoken = jtoken.Next;
                    counterM += 1;
                    counter = 0;
                }
            } while (jtoken != null);
            return ids;
        }
        public static List<ContentInGroup> getCIGfromGroup(String groupId, int groupIdBD, int idMasterContent)
        {
            List<ContentInGroup> cigs = new List<ContentInGroup>();
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String answer = wc.DownloadString("https://api.vk.com/method/wall.get?count=100&owner_id=-" + groupId);
            JObject obj = JObject.Parse(answer);
            JToken jtoken = obj["response"].First;
            int count = int.Parse(jtoken.ToString());
            int counter = 0;
            int counterM = 1;
            jtoken = jtoken.Next;
            do
            {
                DateTime pDate = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(int.Parse(jtoken["date"].ToString()));
                ContentInGroup cig = new ContentInGroup(groupIdBD, idMasterContent, int.Parse(jtoken["id"].ToString()), pDate);
                cigs.Add(cig);
                jtoken = jtoken.Next;
                counter++;
                if (counter == 100)
                {
                    answer = wc.DownloadString("https://api.vk.com/method/wall.get?count=100&owner_id=-" + groupId + "&offset=" + counterM * 100);
                    obj = JObject.Parse(answer);
                    jtoken = obj["response"].First;
                    jtoken = jtoken.Next;
                    counterM += 1;
                    counter = 0;
                }
            } while (jtoken != null);
            return cigs;
        }
        public static List<String> getLikeIdsFromPost(String groupId, int postId)
        {
            List<String> ids = new List<String>();
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String answer = wc.DownloadString("https://api.vk.com/method/likes.getList?count=1000&type=post&owner_id=-" + groupId + "&item_id=" + postId);
            JObject obj = JObject.Parse(answer);
            JToken jtoken = obj["response"]["users"].First;
            int counter = 0;
            int counterM = 1;
            do
            {
                try
                {
                    ids.Add(jtoken.ToString());
                }
                catch (Exception e)
                {
                    break;
                }
                counter++;
                if (counter == 1000)
                {
                    answer = wc.DownloadString("https://api.vk.com/method/likes.getList?count=1000&type=post&owner_id=-" + groupId + "&item_id=" + postId + "&offset=" + counterM * 1000);
                    obj = JObject.Parse(answer);
                    jtoken = obj["response"]["users"].First;
                    counterM += 1;
                    counter = 0;
                }
                else
                    jtoken = jtoken.Next;

            } while (jtoken != null);

            return ids;
        }
        public static List<String> getRepostIdsFromPost(String groupId, int postId)
        {
            List<String> ids = new List<String>();
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String answer = wc.DownloadString("https://api.vk.com/method/likes.getList?count=1000&filter=copies&type=post&owner_id=-" + groupId + "&item_id=" + postId);
            JObject obj = JObject.Parse(answer);
            JToken jtoken = obj["response"]["users"].First;
            int counter = 0;
            int counterM = 1;
            do
            {
                try
                {
                    ids.Add(jtoken.ToString());
                }
                catch (Exception e)
                {
                    break;
                }
                counter++;
                if (counter == 1000)
                {
                    answer = wc.DownloadString("https://api.vk.com/method/likes.getList?count=1000&filter=copies&type=post&owner_id=-" + groupId + "&item_id=" + postId + "&offset=" + counterM * 1000);
                    obj = JObject.Parse(answer);
                    jtoken = obj["response"]["users"].First;
                    counterM += 1;
                    counter = 0;
                }
                else
                    jtoken = jtoken.Next;

            } while (jtoken != null);

            return ids;
        }
        public static List<Tuple<String, int>> getCommentIdsFromPost(String groupId, int postId)
        {
            List<Tuple<String, int>> ids = new List<Tuple<String, int>>();
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            String answer = wc.DownloadString("https://api.vk.com/method/wall.getComments?count=100&owner_id=-" + groupId + "&post_id=" + postId);
            JObject obj = JObject.Parse(answer);
            JToken jtoken = obj["response"].First.Next;
            int counter = 0;
            int counterM = 1;
            do
            {
                try
                {
                    Tuple<String, int> tpl = new Tuple<String, int>(jtoken["from_id"].ToString(), int.Parse(jtoken["cid"].ToString()));
                    ids.Add(tpl);
                }
                catch (Exception e)
                {
                    break;
                }
                counter++;
                if (counter == 100)
                {
                    answer = wc.DownloadString("https://api.vk.com/method/likes.getList?count=100&type=post&owner_id=-" + groupId + "&item_id=" + postId + "&offset=" + counterM * 100);
                    obj = JObject.Parse(answer);
                    jtoken = obj["response"].First.Next;
                    counterM += 1;
                    counter = 0;
                }
                else
                    jtoken = jtoken.Next;

            } while (jtoken != null);

            return ids;
        }

    }
