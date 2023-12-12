using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SysAdminApp
{
    /* For Migrations
     EntityFrameworkCore\Add-Migration <migrationName>
    Update-Database
     */
    public class Context : DbContext
    {
        public DbSet<HelperInfo> HelperInfo { get; set; }
        public DbSet<HelperType> HelperType { get; set; }


        public Context()
        {
            //comment this for migrate
            //this.Database.EnsureCreated();
            this.Database.Migrate();
        }
        protected readonly IConfiguration Configuration;

        public Context(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //comment this for migrate
            CreateDocumentsFolder();
            string documentPath = GetDocumentFolder();
            optionsBuilder.UseSqlite(@$"Data Source={documentPath + "\\Data"}");

            //uncomment this for migrate
            //optionsBuilder.UseSqlite(@$"Data Source=ironXsolutionDB.sqlite3");
        }
        public static string GetDocumentFolder()
        {
            string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" +
                            "IronXSolutionSysAdmin";

            return documentPath;
        }
        public static void CreateDocumentsFolder()
        {
            string documentPath = GetDocumentFolder();

            if (!Directory.Exists(documentPath))
                Directory.CreateDirectory(documentPath);
        }
    }
}
