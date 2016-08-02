using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using EfDemo;

namespace EfTask2.Migrations
{
    [DbContext(typeof(PagesContext))]
    [Migration("20160802153729_FirstMig")]
    partial class FirstMig
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("EfDemo.NavLink", b =>
                {
                    b.Property<int>("NavLinkId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PageId");

                    b.Property<int>("ParentLinkId");

                    b.Property<string>("Position");

                    b.Property<string>("Title");

                    b.HasKey("NavLinkId");

                    b.HasIndex("PageId");

                    b.HasIndex("ParentLinkId");

                    b.ToTable("NavLinks");
                });

            modelBuilder.Entity("EfDemo.Page", b =>
                {
                    b.Property<int>("PageId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("datetime('now')");

                    b.Property<string>("Content");

                    b.Property<string>("Description");

                    b.Property<string>("Title");

                    b.Property<string>("UrlName");

                    b.HasKey("PageId");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("EfDemo.RelatedPages", b =>
                {
                    b.Property<int>("RelatedPagesId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Page1Id");

                    b.Property<int>("Page2Id");

                    b.HasKey("RelatedPagesId");

                    b.HasIndex("Page1Id");

                    b.HasIndex("Page2Id");

                    b.ToTable("RelatedPages");
                });

            modelBuilder.Entity("EfDemo.NavLink", b =>
                {
                    b.HasOne("EfDemo.Page", "Page")
                        .WithMany("NavLinks")
                        .HasForeignKey("PageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EfDemo.Page", "ParentLink")
                        .WithMany("NavLinksParent")
                        .HasForeignKey("ParentLinkId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("EfDemo.RelatedPages", b =>
                {
                    b.HasOne("EfDemo.Page", "Page1")
                        .WithMany()
                        .HasForeignKey("Page1Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EfDemo.Page", "Page2")
                        .WithMany()
                        .HasForeignKey("Page2Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
