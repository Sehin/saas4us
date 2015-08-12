using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.Scheduler;
using Microsoft.WindowsAzure.Management.Scheduler.Models;
using Microsoft.WindowsAzure.Scheduler;
using Microsoft.WindowsAzure.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
