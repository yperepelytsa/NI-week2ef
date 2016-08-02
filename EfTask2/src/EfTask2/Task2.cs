using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfDemo
{
    public class PagesContext : DbContext
    {
        public static void Main()
        {

            //add Pages {UrlName:"example", Title:"example", Description:"example", Content:"example"}
            //update Pages 1 {UrlName:"newexample"}
            //delete Pages 1
            //list all
            using (var db = new PagesContext())
            {
                db.Database.Migrate();
                db.SaveChanges();
                Console.WriteLine("You can now work with db.");
                Console.WriteLine("Available commands: add, update, delete, list all, q to quit");
                Console.WriteLine("add Pages { UrlName: \"example\", Title: \"example\", Description: \"example\", Content: \"example\"}");
                Console.WriteLine("update Pages 1 { UrlName: \"newexample\"}");
                Console.WriteLine("delete Pages 1");
                Console.WriteLine("Enter commands:");
                string str;
                while ((str = Console.ReadLine()) != "q")
                {
                    try
                    {
                        if (str.StartsWith("add"))
                        {
                            var parts = str.Split(' ');
                            string model = parts[1];
                            string json = str.Substring(parts[1].Length + 5);
                            addModel(db, model, json);
                        }
                        else if (str.StartsWith("update"))
                        {
                            var parts = str.Split(' ');
                            string model = parts[1];
                            int id = Convert.ToInt32(parts[2]);
                            string json = str.Substring(parts[1].Length + parts[2].Length + 9);
                            updateModel(db, model, id, json);
                        }
                        else if (str.StartsWith("delete"))
                        {
                            var parts = str.Split(' ');
                            string model = parts[1];
                            int id = Convert.ToInt32(parts[2]);
                            deleteModel(db, model, id);
                        }
                        else if (str.StartsWith("list"))
                        {
                            Console.WriteLine("Pages:");
                            foreach (var i in db.Pages)
                            {

                                Console.WriteLine(i.ToString() + " AddedDate: " + db.Entry(i).Property("AddedDate").CurrentValue.ToString());
                            }
                            Console.WriteLine();
                            Console.WriteLine("NavLinks:");
                            foreach (var i in db.NavLinks)
                            {
                                Console.WriteLine(i.ToString());
                            }
                            Console.WriteLine();
                            Console.WriteLine("RelatedPages:");
                            foreach (var i in db.RelatedPages)
                            {
                                Console.WriteLine(i.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input");
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Wrong input");
                    }
                }
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), "pages.db"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Page>();
            modelBuilder.Entity<Page>().Property<DateTime>("AddedDate").ValueGeneratedOnAdd().HasDefaultValueSql("datetime('now')");
            modelBuilder.Entity<NavLink>();
            modelBuilder.Entity<RelatedPages>();

        }
        public DbSet<Page> Pages { get; set; }
        public DbSet<NavLink> NavLinks { get; set; }
        public DbSet<RelatedPages> RelatedPages { get; set; }

        public static void addModel(PagesContext db, string Model, string data)
        {
            if (Model.Equals("Pages"))
            {
                Page obj = null;
                try
                {
                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Page>(data);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Wrong input");
                    return;
                }
                db.Pages.Add(obj);
                db.SaveChanges();
                Console.WriteLine("Page added, id: " + obj.PageId);
            }
            else if (Model.Equals("NavLinks"))
            {
                NavLink obj = null;
                try
                {
                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<NavLink>(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Wrong input");
                    return;
                }
                db.NavLinks.Add(obj);
                db.SaveChanges();
                Console.WriteLine("NavLink added, id: " + obj.NavLinkId);
            }
            else if (Model.Equals("RelatedPages"))
            {
               RelatedPages obj = null;
                try
                {
                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RelatedPages>(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Wrong input");
                    return;
                }
                if (obj.Page1Id == 0 || obj.Page2Id == 0)
                {
                    Console.WriteLine("Wrong data in json, try again");
                }
                else
                {
                    db.RelatedPages.Add(obj);
                    db.SaveChanges();
                    Console.WriteLine("RelatedPages added, id: " + obj.RelatedPagesId);

                }
            }

        }
        public static void updateModel(PagesContext db, string Model, int id, string data)
        {
            if (Model.Equals("Pages"))
            {
                Page change = db.Pages.Where(b => b.PageId == id).ToList().First();
                if (change == null)
                {
                    Console.WriteLine("Object not found");
                }
                else
                {
                    Page obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Page>(data);
                    if (obj.UrlName != null) change.UrlName = obj.UrlName;
                    if (obj.Title != null) change.Title = obj.Title;
                    if (obj.Description != null) change.Description = obj.Description;
                    if (obj.Content != null) change.Content = obj.Content;
                    Console.WriteLine("Page updated");
                }
            }
            else if (Model.Equals("NavLinks"))
            {
                NavLink change = db.NavLinks.Where(b => b.NavLinkId == id).ToList().First();
                if (change == null)
                {
                    Console.WriteLine("Object not found");
                }
                else
                {
                    NavLink obj = Newtonsoft.Json.JsonConvert.DeserializeObject<NavLink>(data);
                    if (obj.ParentLinkId != 0) change.ParentLinkId = obj.ParentLinkId;
                    if (obj.PageId != 0) change.PageId = obj.PageId;
                    if (obj.Title != null) change.Title = obj.Title;
                    Console.WriteLine("NavLink updated");
                }
            }
            else if (Model.Equals("RelatedPages"))
            {
                RelatedPages change = db.RelatedPages.Where(b => b.RelatedPagesId == id).ToList().First();
                if (change == null)
                {
                    Console.WriteLine("Object not found");
                }
                else
                {
                    RelatedPages obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RelatedPages>(data);
                    if (obj.Page1Id != 0) change.Page1Id = obj.Page1Id;
                    if (obj.Page2Id != 0) change.Page2Id = obj.Page2Id;
                    Console.WriteLine("RelatedPages updated");
                }

            }
            db.SaveChanges();
        }
        public static void deleteModel(PagesContext db, string Model, int id)
        {
            if (Model.Equals("Pages"))
            {
                Page toRemove = db.Pages.Where(b => b.PageId == id).ToList().First();
                if (toRemove == null)
                {
                    Console.WriteLine("Object not found");
                }
                else
                {
                    Console.WriteLine("Page removed");
                }
                db.Pages.Remove(toRemove);
            }
            else if (Model.Equals("NavLinks"))
            {
                NavLink toRemove = db.NavLinks.Where(b => b.NavLinkId == id).ToList().First();
                if (toRemove == null)
                {
                    Console.WriteLine("Object not found");
                }
                else
                {
                    Console.WriteLine("NavLink removed");
                }
                db.NavLinks.Remove(toRemove);
            }
            else if (Model.Equals("RelatedPages"))
            {
                RelatedPages toRemove = db.RelatedPages.Where(b => b.RelatedPagesId == id).ToList().First();
                if (toRemove == null)
                {
                    Console.WriteLine("Object not found");
                }
                else
                {
                    Console.WriteLine("RelatedPages removed");
                }
                db.RelatedPages.Remove(toRemove);
            }
            db.SaveChanges();
        }
    }

    public class Page
    {
        public Page() { }
        public int PageId { get; set; }
        public string UrlName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        [InverseProperty("Page")]
        public List<NavLink> NavLinks { get; set; }
        [InverseProperty("ParentLink")]
        public List<NavLink> NavLinksParent { get; set; }
        public override String ToString()
        {
            return "Page Id: " + PageId + " UrlName: " + UrlName + " Title: " + Title + " Descr: " + Description + " Content: " + Content;
        }
    }

    public class NavLink
    {
        public NavLink() { }
        public int NavLinkId { get; set; }
        public string Title { get; set; }

        public int PageId { get; set; }
        [ForeignKey("PageId")]
        public Page Page { get; set; }

       public int ParentLinkId { get; set; }
        [ForeignKey("ParentLinkId")]
        public Page ParentLink { get; set; }

        public string Position { get; set; }
        public override String ToString()
        {
            return "Link Id: " + NavLinkId + " Title: " + Title +" PageId: "+ PageId + " ParentLinkId: " + ParentLinkId;
        }
    }

    public class RelatedPages
    {
        public RelatedPages() { }
        //was added because id is needed for update/delete statement as it was mentioned in the task
        //delete <modelName> <id>
        public int RelatedPagesId { get; set; }
        public int Page1Id { get; set; }
        public Page Page1 { get; set; }
        public int Page2Id { get; set; }
        public Page Page2 { get; set; }
        public override string ToString()
        {
            return "RelatedPages Id: " + RelatedPagesId + " Page1Id: " + Page1Id + " Page2Id: " + Page2Id;
        }
    }

}





