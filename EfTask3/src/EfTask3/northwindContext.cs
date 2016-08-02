using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Internal;
using System.IO;
using EfTask3.Models;

namespace EfTask3
{
    public partial class northwindContext : DbContext
    {
        public static void Main()
        {
            using (var db = new northwindContext())
            {

                Console.OutputEncoding = Encoding.UTF8;

              
                // Select all customers whose name starts with letter “D”
                Console.WriteLine("1");
                var list1 = db.Customers.Where(c => c.CompanyName.StartsWith("D"));
                foreach (Customers c1 in list1)
                    Console.WriteLine(c1.CompanyName);

                // Convert names of all customers to Upper Case
                Console.WriteLine("2");
                var list2 = db.Customers.Select(c => c.CompanyName.ToUpper()).ToList();
                foreach (String c2 in list2)
                    Console.WriteLine(c2);
                //Select distinct country from Customers
                Console.WriteLine("3");
                var list3 = db.Customers.Select(c => c.Country).Distinct().ToList();
                foreach (String c3 in list3)
                    Console.WriteLine(c3);
                //Select Contact name from Customers Table from Lindon and title like “Sales”
                Console.WriteLine("4");
                var list4 = db.Customers.Where(c => c.City == "London" && c.ContactTitle.Contains("Sales")).Select(c => c.ContactName).ToList();
                foreach (String c4 in list4)
                    Console.WriteLine(c4);
                //Select all orders id where was bought “Tofu”
                Console.WriteLine("5");
                var list5 = db.OrderDetails
                    .Join(db.Orders, od => od.OrderId, o => o.OrderId, (od, o) => new { od, o })
                    .Join(db.Products, ood => ood.od.ProductId, p => p.ProductId, (p, ood) => new { p, ood })
                .Where(c => c.ood.ProductName == "Tofu").Select(c => c.p.o.OrderId);

                foreach (int c5 in list5)
                    Console.WriteLine(c5);
                //Select all product names that were shipped to Germany
                Console.WriteLine("6");
                var list6 = db.Orders
                    .Join(db.OrderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new { o, od })
                    .Join(db.Products, ood => ood.od.ProductId, p => p.ProductId, (ood, p) => new { ood, p })
                .Where(c => c.ood.o.ShipCountry == "Germany").Select(c => c.p.ProductName).Distinct();

                foreach (string c6 in list6)
                    Console.WriteLine(c6);
                //Select all customers that ordered “Ikura”
                Console.WriteLine("7");
                var list7 = db.Customers.Where(res =>
                db.Orders
                    .Join(db.OrderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new { o, od })
                    .Join(db.Products, ood => ood.od.ProductId, p => p.ProductId, (ood, p) => new { ood, p })
                .Where(c => c.p.ProductName == "Ikura").Select(c => c.ood.o.CustomerId).Distinct().Contains(res.CustomerId)).Select(r => new { r.CustomerId, r.CompanyName });

                foreach (var c7 in list7)
                    Console.WriteLine(c7.CustomerId + "," + c7.CompanyName);
                //Select all employees and any orders they might have

                var LeftJoin = from emp in db.Employees
                               join ord in db.Orders
                               on emp.EmployeeId equals ord.EmployeeId into JoinedEmpOrd
                               from ord in JoinedEmpOrd.DefaultIfEmpty()
                               select new
                               {
                                   EmployeeName = emp.LastName,
                                   OrderId = ord != null ? ord.OrderId : 0
                               };

                //Selects all employees, and all orders:

                var RightJoin = from ord in db.Orders
                                join emp in db.Employees
                               on ord.EmployeeId equals emp.EmployeeId into JoinedOrdEmp
                                from emp in JoinedOrdEmp.DefaultIfEmpty()
                                select new
                                {
                                    EmployeeName = emp != null ? emp.LastName : null,
                                    OrderId = ord.OrderId
                                };

                //Select all phones from Shippers and Suppliers
                Console.WriteLine("10");
                var list10 = db.Suppliers.Select(sp => sp.Phone).Union(db.Shippers.Select(s => s.Phone));
                foreach (string c10 in list10)
                    Console.WriteLine(c10);
                //Count all customers grouped by city
                Console.WriteLine("11");
                var list11 = db.Customers.GroupBy(c => c.City).Select(group => new { City = group.Key, Count = group.Count() });
                foreach (var c11 in list11)
                {
                    Console.WriteLine(c11.City + ": " + c11.Count);
                }

                //Select all customers that placed more than 10 orders with average Unit Price less than 17
                Console.WriteLine("12");

                var list12 = db.Orders.Where(o => o.OrderDetails.Select(od => float.Parse(od.UnitPrice, CultureInfo.InvariantCulture)).Average() > 17).
                    GroupBy(ord => ord.CustomerId).Where(ord => ord.Count() > 10).
                    Select(group => new { CustomerId = group.Key, Count = group.Count() }).
                    Join(db.Customers, c1 => c1.CustomerId, c2 => c2.CustomerId, (c1, c2) => new { c1, c2 }).
                    Select(res => new { CompanyName = res.c2.CompanyName, Count = res.c1.Count });


                foreach (var c12 in list12)
                {
                    Console.WriteLine(c12.CompanyName + ": " + c12.Count);
                }
                //Select all customers with phone that has format (’NNNN - NNNN’)
                Console.WriteLine("13");
                Regex phoneReg = new Regex(@"\d\d\d\d-\d\d\d\d$");
                var list13 = db.Customers.Where(c => phoneReg.IsMatch(c.Phone)).Select(res => new { Name = res.CompanyName, Phone = res.Phone });
                foreach (var c13 in list13)
                {
                    Console.WriteLine(c13.Name + ": " + c13.Phone);
                }
                //Select customer that ordered the greatest amount of goods(not price)
                Console.WriteLine("14");
                var c14 = db.Orders.Select(ord => new { CustomerId = ord.CustomerId, Quantity = ord.OrderDetails.Select(od => od.Quantity).Sum() }).Join(db.Customers, c1 => c1.CustomerId, c2 => c2.CustomerId, (c1, c2) => new { c1, c2 }).
                    GroupBy(ord => ord.c2.CompanyName).Select(group => new { CompanyName = group.Key, Quantity = group.Sum(w => w.c1.Quantity) }).OrderBy(w => w.Quantity).Last();

                Console.WriteLine(c14.CompanyName + ": " + c14.Quantity);
                Console.WriteLine("15");
                //Select only these customers that ordered the absolutely the same products as customer “FAMIA”
                Console.WriteLine("this query is executed for about 30 seconds.");
                var famia = db.Orders.Where(ord => ord.CustomerId == "FAMIA").SelectMany(ord => ord.OrderDetails.Select(od => od.ProductId)).Distinct();
                var list15 = db.Customers.Where(c => c.Orders.SelectMany(ord => ord.OrderDetails.Select(od => od.ProductId)).Distinct().SequenceEqual(famia)).Select(c => c.CustomerId).ToList();
                foreach (string c15 in list15)
                {
                    Console.WriteLine(c15);

                }
                Console.WriteLine("Outputs from tasks 8 and 9 are very long, enter 8 or 9 to view (q to quit)");
                string str;
                while ((str = Console.ReadLine()) != "q")
                {
                    if (str.StartsWith("8"))
                    {
                        Console.WriteLine("8");
                        foreach (var c8 in LeftJoin)
                            Console.WriteLine(c8.OrderId + ": " + c8.EmployeeName);
                    }
                    else if (str.StartsWith("9"))
                    {
                        Console.WriteLine("9");
                        foreach (var c9 in LeftJoin)
                            Console.WriteLine(c9.OrderId + ": " + c9.EmployeeName);
                    }
                }
            }
        }
        public static string dbloc = Directory.GetCurrentDirectory() + @"\northwind.db";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlite(@"Data Source= "+dbloc);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("sqlite_autoindex_Categories_1");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("CategoryID")
                    .HasColumnType("int");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasColumnType("nvarchar");

                entity.Property(e => e.Description).HasColumnType("nvarchar");

                entity.Property(e => e.Picture).HasColumnType("blob");
            });

           
            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("sqlite_autoindex_Customers_1");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CustomerID")
                    .HasColumnType("char");

                entity.Property(e => e.Address).HasColumnType("nvarchar");

                entity.Property(e => e.City).HasColumnType("nvarchar");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnType("nvarchar");

                entity.Property(e => e.ContactName).HasColumnType("nvarchar");

                entity.Property(e => e.ContactTitle).HasColumnType("nvarchar");

                entity.Property(e => e.Country).HasColumnType("nvarchar");

                entity.Property(e => e.Fax).HasColumnType("nvarchar");

                entity.Property(e => e.Phone).HasColumnType("nvarchar");

                entity.Property(e => e.PostalCode).HasColumnType("nvarchar");

                entity.Property(e => e.Region).HasColumnType("nvarchar");
            });
            modelBuilder.Entity<EmployeeTerritories>().
                  HasKey(e => new { e.EmployeeId, e.TerritoryId });
                     
            modelBuilder.Entity<EmployeeTerritories>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.TerritoryId })
                    .HasName("sqlite_autoindex_EmployeeTerritories_1");
               
                entity.Property(e => e.EmployeeId)
                    .HasColumnName("EmployeeID")
                    .HasColumnType("int");

                entity.Property(e => e.TerritoryId)
                    .HasColumnName("TerritoryID")
                    .HasColumnType("nvarchar");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeTerritories)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Territory)
                    .WithMany(p => p.EmployeeTerritories)
                    .HasForeignKey(d => d.TerritoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.EmployeeId)
                    .HasName("sqlite_autoindex_Employees_1");

                entity.Property(e => e.EmployeeId)
                    .HasColumnName("EmployeeID")
                    .HasColumnType("int");

                entity.Property(e => e.Address).HasColumnType("nvarchar");

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.City).HasColumnType("nvarchar");

                entity.Property(e => e.Country).HasColumnType("nvarchar");

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasColumnType("bit")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Extension).HasColumnType("nvarchar");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("nvarchar");

                entity.Property(e => e.HireDate).HasColumnType("datetime");

                entity.Property(e => e.HomePhone).HasColumnType("nvarchar");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("nvarchar");

                entity.Property(e => e.Notes).HasColumnType("nvarchar");

                entity.Property(e => e.Photo).HasColumnType("blob");

                entity.Property(e => e.PhotoPath).HasColumnType("nvarchar");

                entity.Property(e => e.PostalCode).HasColumnType("nvarchar");

                entity.Property(e => e.Region).HasColumnType("nvarchar");

                entity.Property(e => e.ReportsTo).HasColumnType("int");

                entity.Property(e => e.Title).HasColumnType("nvarchar");

                entity.Property(e => e.TitleOfCourtesy).HasColumnType("nvarchar");

                entity.HasOne(d => d.ReportsToNavigation)
                    .WithMany(p => p.InverseReportsToNavigation)
                    .HasForeignKey(d => d.ReportsTo);
            });

            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("sqlite_autoindex_Order Details_1");

                entity.ToTable("Order Details");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasColumnType("int");

                entity.Property(e => e.ProductId)
                    .HasColumnName("ProductID")
                    .HasColumnType("int");

                entity.Property(e => e.Discount)
                    .IsRequired()
                    .HasColumnType("SINGLE")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Quantity)
                    .HasColumnType("smallint")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.UnitPrice)
                    .IsRequired()
                    .HasColumnType("decimal")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("sqlite_autoindex_Orders_1");

                entity.Property(e => e.OrderId)
                    .HasColumnName("OrderID")
                    .HasColumnType("int");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CustomerID")
                    .HasColumnType("char");

                entity.Property(e => e.EmployeeId)
                    .HasColumnName("EmployeeID")
                    .HasColumnType("int");

                entity.Property(e => e.Freight)
                    .HasColumnType("decimal")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.RequiredDate).HasColumnType("datetime");

                entity.Property(e => e.ShipAddress).HasColumnType("nvarchar");

                entity.Property(e => e.ShipCity).HasColumnType("nvarchar");

                entity.Property(e => e.ShipCountry).HasColumnType("nvarchar");

                entity.Property(e => e.ShipName).HasColumnType("nvarchar");

                entity.Property(e => e.ShipPostalCode).HasColumnType("nvarchar");

                entity.Property(e => e.ShipRegion).HasColumnType("nvarchar");

                entity.Property(e => e.ShipVia).HasColumnType("int");

                entity.Property(e => e.ShippedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.EmployeeId);

                entity.HasOne(d => d.ShipViaNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipVia);
            });

            modelBuilder.Entity<ProductCategoryMap>(entity =>
            {
                entity.HasKey(e => new { e.CategoryId, e.ProductId })
                    .HasName("sqlite_autoindex_Product_Category_Map_1");

                entity.ToTable("Product_Category_Map");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("CategoryID")
                    .HasColumnType("int");

                entity.Property(e => e.ProductId)
                    .HasColumnName("ProductID")
                    .HasColumnType("int");

                entity.HasOne(d => d.Categories)
                    .WithMany(p => p.ProductCategoryMap)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Products)
                    .WithMany(p => p.ProductCategoryMap)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("sqlite_autoindex_Products_1");

                entity.Property(e => e.ProductId)
                    .HasColumnName("ProductID")
                    .HasColumnType("int");

                entity.Property(e => e.AttributeXml)
                    .HasColumnName("AttributeXML")
                    .HasColumnType("varchar");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("CategoryID")
                    .HasColumnType("int");

                entity.Property(e => e.CreatedBy).HasColumnType("nvarchar");

                entity.Property(e => e.CreatedOn)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasColumnType("bit")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Discontinued)
                    .IsRequired()
                    .HasColumnType("bit")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ModifiedBy).HasColumnType("nvarchar");

                entity.Property(e => e.ModifiedOn)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ProductGuid)
                    .HasColumnName("ProductGUID")
                    .HasColumnType("uniqueidentifier");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnType("nvarchar");

                entity.Property(e => e.QuantityPerUnit).HasColumnType("nvarchar");

                entity.Property(e => e.ReorderLevel)
                    .HasColumnType("smallint")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("SupplierID")
                    .HasColumnType("int");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.UnitsInStock)
                    .HasColumnType("smallint")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.UnitsOnOrder)
                    .HasColumnType("smallint")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId);

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierId);
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.Property(e => e.RegionId)
                    .HasColumnName("RegionID")
                    .HasColumnType("int");

                entity.Property(e => e.RegionDescription)
                    .IsRequired()
                    .HasColumnType("char");
            });

            modelBuilder.Entity<Shippers>(entity =>
            {
                entity.HasKey(e => e.ShipperId)
                    .HasName("sqlite_autoindex_Shippers_1");

                entity.Property(e => e.ShipperId)
                    .HasColumnName("ShipperID")
                    .HasColumnType("int");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnType("nvarchar");

                entity.Property(e => e.Phone).HasColumnType("nvarchar");
            });

            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.HasKey(e => e.SupplierId)
                    .HasName("sqlite_autoindex_Suppliers_1");

                entity.Property(e => e.SupplierId)
                    .HasColumnName("SupplierID")
                    .HasColumnType("int");

                entity.Property(e => e.Address).HasColumnType("nvarchar");

                entity.Property(e => e.City).HasColumnType("nvarchar");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnType("nvarchar");

                entity.Property(e => e.ContactName).HasColumnType("nvarchar");

                entity.Property(e => e.ContactTitle).HasColumnType("nvarchar");

                entity.Property(e => e.Country).HasColumnType("nvarchar");

                entity.Property(e => e.Fax).HasColumnType("nvarchar");

                entity.Property(e => e.HomePage).HasColumnType("nvarchar");

                entity.Property(e => e.Phone).HasColumnType("nvarchar");

                entity.Property(e => e.PostalCode).HasColumnType("nvarchar");

                entity.Property(e => e.Region).HasColumnType("nvarchar");
            });

            modelBuilder.Entity<Territories>(entity =>
            {
                entity.HasKey(e => e.TerritoryId)
                    .HasName("sqlite_autoindex_Territories_1");

                entity.Property(e => e.TerritoryId)
                    .HasColumnName("TerritoryID")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.RegionId)
                    .HasColumnName("RegionID")
                    .HasColumnType("int");

                entity.Property(e => e.TerritoryDescription)
                    .IsRequired()
                    .HasColumnType("char");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.Territories)
                    .HasForeignKey(d => d.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TextEntry>(entity =>
            {
                entity.HasKey(e => e.ContentId)
                    .HasName("sqlite_autoindex_TextEntry_1");

                entity.Property(e => e.ContentId)
                    .HasColumnName("contentID")
                    .HasColumnType("int");

                entity.Property(e => e.CallOut)
                    .HasColumnName("callOut")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.ContentGuid)
                    .IsRequired()
                    .HasColumnName("contentGUID")
                    .HasColumnType("uniqueidentifier");

                entity.Property(e => e.ContentName)
                    .IsRequired()
                    .HasColumnName("contentName")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("createdBy")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("createdOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DateExpires)
                    .HasColumnName("dateExpires")
                    .HasColumnType("datetime");

                entity.Property(e => e.ExternalLink)
                    .HasColumnName("externalLink")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.IconPath)
                    .HasColumnName("iconPath")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.LastEditedBy)
                    .HasColumnName("lastEditedBy")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.ListOrder)
                    .HasColumnName("listOrder")
                    .HasColumnType("int")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modifiedBy")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modifiedOn")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("nvarchar");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("nvarchar");
            });
        }

        public virtual DbSet<Categories> Categories { get; set; }
 
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<EmployeeTerritories> EmployeeTerritories { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<ProductCategoryMap> ProductCategoryMap { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<Shippers> Shippers { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<Territories> Territories { get; set; }
        public virtual DbSet<TextEntry> TextEntry { get; set; }
    }
}