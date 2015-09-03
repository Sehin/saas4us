using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MvcWebRole1.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<ContentData> ContentDatas { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<SocAccount> SocAccounts { get; set; }
        public DbSet<ContentInGroup> ContentsInGroups { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagsInContent> TagsInContents { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientLike> ClientLikes { get; set; }
        public DbSet<ClientRepost> ClientReposts { get; set; }
        public DbSet<ClientComment> ClientComments { get; set; }

        public DbSet<MarkProgram> MarkPrograms { get; set; }
        public DbSet<T1Trigger> T1Trigger{ get; set; }
        public DbSet<T3Trigger> T3Trigger { get; set; }
        public DbSet<ClientInMP> ClientInMps { get; set; }
        public DbSet<Arrows> Arrows { get; set; }
        public DbSet<T1Arrow> T1Arrow { get; set; }
        public DbSet<T2Arrow> T2Arrow { get; set; }
        public DbSet<Action> Actions { get; set; }
        public DbSet<T1Action> T1Actions { get; set; }
        public DbSet<T2Action> T2Actions { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobCollection> JobCollections { get; set; }



        public DbSet<Subscriber> Subscribers { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}