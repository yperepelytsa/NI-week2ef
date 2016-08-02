using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;

namespace EfTask1
{

    public class BloggingContext : DbContext
    {

           public static void Main()
           {
               using (var db = new BloggingContext())
               {
                //using created migration FirstMig
                   db.Database.Migrate();
                   var blog1 = new Blog {  Url = "http://sample1.com" };
                   var blog2 = new Blog {  Url = "http://sample2.com" };
                   var blog3 = new Blog {  Url = "http://sample3.com" };
                   db.Blogs.Add(blog1);
                   db.Blogs.Add(blog2);
                   db.Blogs.Add(blog3);
                   var post11 = new Post {  Title = "post11", Content = "content11", BlogId = 1 };
                   var post12 = new Post {  Title = "post12", Content = "content12", BlogId = 1 };
                   var post21 = new Post {  Title = "post21", Content = "content21", BlogId = 2 };
                   var post31 = new Post {  Title = "post31", Content = "content31", BlogId = 3 };
                   db.Posts.Add(post11);
                   db.Posts.Add(post12);
                   db.Posts.Add(post21);
                   db.Posts.Add(post31);
                   db.SaveChanges();

                   var blogList= db.Blogs.ToList();
                   var postList = db.Posts.ToList();
                   Console.WriteLine("Blogs:");
                   foreach (Blog b in blogList)
                   {
                       Console.WriteLine("BlogId: " + b.BlogId + ", Url:" + b.Url);
                   }
                   Console.WriteLine("Posts:");
                   foreach (Post p in postList)
                   {
                       Console.WriteLine("BlogId: " + p.BlogId + ", PostId:" + p.PostId + ", Title:" + p.Title + ", Content:" + p.Content );
                   }
               }

           }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), "blogging.db"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Blog>();
            modelBuilder.Entity<Post>();
        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }

    }


}
