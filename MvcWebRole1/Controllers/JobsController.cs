using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.Scheduler;
using Microsoft.WindowsAzure.Management.Scheduler.Models;
using Microsoft.WindowsAzure.Scheduler;
using Microsoft.WindowsAzure.Scheduler.Models;
using MvcWebRole1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MvcWebRole1.Controllers
{
    public class JobsController : Controller
    {
        //
        // GET: /Jobs/

        public ActionResult Index()
        {
            return View();
        }

        /* Метод который отвечает за первоначальную инициализацию JobCollection
         * и базовых Job:
         * 1. UpdateSocClients
         * 2. UpdateVkAction
         * ...
         */
        public void Init(HttpPostedFileBase fileBase,String subscriptionName,String url)
        {
           var credentials = CertificateCloudCredentialsFactory.FromPublishSettingsFile(fileBase, subscriptionName);
           var cloudServiceClient = new CloudServiceManagementClient(credentials);

           try
           {
               var result = cloudServiceClient.CloudServices.Create("sch-serv", new CloudServiceCreateParameters()
               {
                   Description = "sch-serv",
                   GeoRegion = "west europe",
                   Label = "sch-serv"
               });

               Console.WriteLine(result.Status);
               Console.WriteLine(result.HttpStatusCode);
           }
            catch(Exception e){}

           var schedulerServiceClient = new SchedulerManagementClient(credentials);
           try
           {
               // Создаем новый jobCollection
              
               var result_ = schedulerServiceClient.JobCollections.Create("sch-serv", "backgroundActions", new JobCollectionCreateParameters()
               {
                   Label = "backgroundActions",
                   IntrinsicSettings = new JobCollectionIntrinsicSettings()
                   {
                       Plan = JobCollectionPlan.Standard,
                       Quota = new JobCollectionQuota()
                       {
                           MaxJobCount = 50,
                           MaxJobOccurrence = 50,
                           MaxRecurrence = new JobCollectionMaxRecurrence()
                           {
                               Frequency = JobCollectionRecurrenceFrequency.Minute,
                               Interval = 1
                           }
                       }
                   }
               });
           }
            catch (Exception e)
           {
               Console.WriteLine("Some error :(" + e.Message + ";" + e.InnerException.Message);
           }

            // Update soc clients

           var schedulerClient = new SchedulerClient("sch-serv", "backgroundActions", credentials);
           var _result = schedulerClient.Jobs.Create(new JobCreateParameters()
           {
               Action = new JobAction()
               {
                   Type = JobActionType.Http,
                   Request = new JobHttpRequest()
                   {
                       Method = "GET",
                       Uri = new Uri(url + "/Client/updateSocClients")
                   }
               },
               StartTime = DateTime.UtcNow,
               Recurrence = new JobRecurrence()
               {
                   Frequency = JobRecurrenceFrequency.Minute,
                   Interval = 30
               }
           });
           // Update vk actions

           var _result1 = schedulerClient.Jobs.Create(new JobCreateParameters()
           {
               Action = new JobAction()
               {
                   Type = JobActionType.Http,
                   Request = new JobHttpRequest()
                   {
                       Method = "GET",
                       Uri = new Uri(url + "/Client/updateVkActions")
                   }
               },
               StartTime = DateTime.UtcNow,
               Recurrence = new JobRecurrence()
               {
                   Frequency = JobRecurrenceFrequency.Minute,
                   Interval = 30
               }
           }); 




        }
        public void testIt()
        {
            JobWorker.createNewJob(3, 1);
        }
    }

    public static class CertificateCloudCredentialsFactory
    {
        public static CertificateCloudCredentials FromPublishSettingsFile(HttpPostedFileBase file, string subscriptionName)
        {
            
            var profile = XDocument.Load(file.InputStream);
            var subscriptionId = profile.Descendants("Subscription")
                .First(element => element.Attribute("Name").Value == subscriptionName)
                .Attribute("Id").Value;
            var certificate = new X509Certificate2(
                Convert.FromBase64String(profile.Descendants("PublishProfile").Descendants("Subscription").Single().Attribute("ManagementCertificate").Value));
            return new CertificateCloudCredentials(subscriptionId, certificate);
        }
        public static CertificateCloudCredentials FromPublishSettingsFileInStorage(String url)
        {
            var profile = XDocument.Load(url);
            var subscriptionId = profile.Descendants("Subscription")
    .First()
    .Attribute("Id").Value;
            var certificate = new X509Certificate2(
                Convert.FromBase64String(profile.Descendants("PublishProfile").Descendants("Subscription").Single().Attribute("ManagementCertificate").Value));
            return new CertificateCloudCredentials(subscriptionId, certificate);
        }
    }
    public static class JobWorker
    {
        public static void createNewJob(int actionId, int hours)
        {
            /*  1. Проверить наличие свободных JC. В случае если их нет - создать новый
             *  2. В выбранный JC поместить данный Job.
             */
            DatabaseContext db = new DatabaseContext();
            #region jobCleaner
            var jcs = db.JobCollections;
            foreach (JobCollection jc in jcs)
            {
                var _schedulerClient = new SchedulerClient("sch-service", "S4U_jc_" + jc.ID_JC, CertificateCloudCredentialsFactory.FromPublishSettingsFileInStorage("https://storagesaas4.blob.core.windows.net/credentials/credentials.publishsettings"));
                var jobs = db.Jobs.Where(j => j.ID_JC == jc.ID_JC);
                foreach (MvcWebRole1.Models.Job job in jobs)
                {
                    var result = _schedulerClient.Jobs.Get(job.ID);
                    if(result.Job.State!=JobState.Enabled)
                    {
                        db.Jobs.Remove(job);
                    }
                }
            }
            db.SaveChanges();

            #endregion
            #region checkJCs
            JobCollection jobCollection = new JobCollection();    // JC который будем использовать
            bool isOldJC = false; // Используем старый JC
            foreach (JobCollection jc in jcs)
            {
                int jobCountInJc = db.Jobs.Where(j => j.ID_JC == jc.ID_JC).Count();
                if (jobCountInJc < 5)
                {
                    jobCollection = jc;
                    isOldJC = true;
                    break;
                }
            }
            if (!isOldJC)
            {
                // Создаем новый JC
                jobCollection = createNewJC();
            }
            #endregion

            
            var schedulerClient = new SchedulerClient("sch-service", "S4U_jc_" + jobCollection.ID_JC, CertificateCloudCredentialsFactory.FromPublishSettingsFileInStorage("https://storagesaas4.blob.core.windows.net/credentials/credentials.publishsettings"));
            var _result = new JobCreateResponse();
            if (hours == 0)
            {
                _result = schedulerClient.Jobs.Create(new JobCreateParameters()
                {
                    Action = new JobAction()
                    {
                        Type = JobActionType.Http,
                        Request = new JobHttpRequest()
                        {
                            Method = "GET",
                            Uri = new Uri(ConfigurationManager.AppSettings["saasUrl"] + "Action/executeAction?id=" + actionId)
                        }
                    },
                    StartTime = DateTime.UtcNow
                });
            }
            else
            {
                _result = schedulerClient.Jobs.Create(new JobCreateParameters()
                {
                    Action = new JobAction()
                    {
                        Type = JobActionType.Http,
                        Request = new JobHttpRequest()
                        {
                            Method = "GET",
                            Uri = new Uri(ConfigurationManager.AppSettings["saasUrl"] + "Action/executeAction?id=" + actionId)
                        }
                    },
                    StartTime = DateTime.UtcNow.AddHours(hours)
                });
            }
            MvcWebRole1.Models.Job new_job = new Models.Job(_result.Job.Id, jobCollection.ID_JC);
            db.Jobs.Add(new_job);
            db.SaveChanges();
        }
        public static void createNewJob(List<int> actionIds)
        {
            /*  1. Проверить наличие свободных JC. В случае если их нет - создать новый
            *  2. В выбранный JC поместить данный Job.
            */
            DatabaseContext db = new DatabaseContext();
            #region jobCleaner
            var jcs = db.JobCollections;
            foreach (JobCollection jc in jcs)
            {
                var _schedulerClient = new SchedulerClient("sch-service", "S4U_jc_" + jc.ID_JC, CertificateCloudCredentialsFactory.FromPublishSettingsFileInStorage("https://storagesaas4.blob.core.windows.net/credentials/credentials.publishsettings"));
                var jobs = db.Jobs.Where(j => j.ID_JC == jc.ID_JC);
                foreach (MvcWebRole1.Models.Job job in jobs)
                {
                    var result = _schedulerClient.Jobs.Get(job.ID);
                    if (result.Job.State != JobState.Enabled)
                    {
                        db.Jobs.Remove(job);
                        _schedulerClient.Jobs.Delete(job.ID);
                    }
                }
            }
            db.SaveChanges();

            #endregion
            #region checkJCs
            JobCollection jobCollection = new JobCollection();    // JC который будем использовать
            bool isOldJC = false; // Используем старый JC
            foreach (JobCollection jc in jcs)
            {
                int jobCountInJc = db.Jobs.Where(j => j.ID_JC == jc.ID_JC).Count();
                if (jobCountInJc < 5)
                {
                    jobCollection = jc;
                    isOldJC = true;
                    break;
                }
            }
            if (!isOldJC)
            {
                // Создаем новый JC
                jobCollection = createNewJC();
            }
            #endregion

            #region Создаем строку actionsString в которой будут хранится id's последующих действий

            String actionsString = "";
            for (int i = 0; i < actionIds.Count; i++)
            {
                if (i + 1 != actionIds.Count)
                {
                    actionsString += actionIds[i] + ",";
                }
                else
                    actionsString += actionIds[i];
            }
            #endregion

            var schedulerClient = new SchedulerClient("sch-service", "S4U_jc_" + jobCollection.ID_JC, CertificateCloudCredentialsFactory.FromPublishSettingsFileInStorage("https://storagesaas4.blob.core.windows.net/credentials/credentials.publishsettings"));
            var _result = new JobCreateResponse();
                _result = schedulerClient.Jobs.Create(new JobCreateParameters()
                {
                    Action = new JobAction()
                    {
                        Type = JobActionType.Http,
                        Request = new JobHttpRequest()
                        {
                            Method = "GET",
                            Uri = new Uri(ConfigurationManager.AppSettings["saasUrl"] + "Action/executeAction?id=" + actionsString)
                        }
                    },
                    StartTime = DateTime.UtcNow
                });
                MvcWebRole1.Models.Job job_ = new Models.Job(_result.Job.Id, jobCollection.ID_JC);
                db.Jobs.Add(job_);
                db.SaveChanges();
        }
        public static JobCollection createNewJC()
        {
            DatabaseContext db = new DatabaseContext();
            var credentials = CertificateCloudCredentialsFactory.FromPublishSettingsFileInStorage("https://storagesaas4.blob.core.windows.net/credentials/credentials.publishsettings");
            var cloudServiceClient = new CloudServiceManagementClient(credentials);
            // 1. Создаем новый CloudService, который отвечает за работу Scheduler.
            try
            {
                var result = cloudServiceClient.CloudServices.Create("sch-service", new CloudServiceCreateParameters()
                {
                    Description = "sch-service",
                    GeoRegion = "West Europe",
                    Label = "sch-service"
                });

                Console.WriteLine(result.Status);
                Console.WriteLine(result.HttpStatusCode);
            }
            catch (Exception e) { } // На случай если уже был создан такой сервис

            // 2. Создаем новый JobCollection
            JobCollection jc = new JobCollection("");
            db.JobCollections.Add(jc);
            db.SaveChanges();
            var schedulerServiceClient = new SchedulerManagementClient(credentials);
            try
            {
                var result_ = schedulerServiceClient.JobCollections.Create("sch-service", "S4U_jc_"+jc.ID_JC, new JobCollectionCreateParameters()
                {
                    Label = "S4U_jc_" + jc.ID,
                    IntrinsicSettings = new JobCollectionIntrinsicSettings()
                    {
                        Plan = JobCollectionPlan.Free,
                        Quota = new JobCollectionQuota()
                        {
                            MaxJobCount = 5,
                            MaxJobOccurrence = 5,
                            MaxRecurrence = new JobCollectionMaxRecurrence()
                            {
                                Frequency = JobCollectionRecurrenceFrequency.Hour,
                                Interval = 1
                            }
                        }
                    }
                });
                jc.ID = result_.Id;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Some error :(" + e.Message + ";" + e.InnerException.Message);
                return null;
            }
            return jc;
        }
    }
}
