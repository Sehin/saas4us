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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}