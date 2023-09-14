using Library.Functions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Library.Models
{
    /* For Migrations
     EntityFrameworkCore\Add-Migration <migrationName>
    Update-Database
     */
    public class IronContext : DbContext
    {
        public DbSet<Admin> Admin { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Waste> Waste { get; set; }
        public DbSet<ComputerGame> Updates { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<CheckUpdate> CheckUpdates { get; set; }
        public DbSet<LaunchersInfo> LaunchersInfos { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Log> Logs { get; set; }
        private DbSet<Config> Config { get; set; }
        private DbSet<HelperInfo> HelperInfo { get; set; }

        public IronContext()
        {
            //comment this for migrate
            //this.Database.EnsureCreated();
            try
            {
                this.Database.Migrate();
            }
            catch
            {
                //this.Database.EnsureCreated();
            }
            CheckOrCreateConfig();
        }
        protected readonly IConfiguration Configuration;

        public IronContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //comment this for migrate
            var data = JsonDocument.Parse(BaseFunctions.GetStringFileFromResources("DBConfig.json"));
            BaseFunctions.CreateDocumentsFolder();
            string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" +
                            data.RootElement.GetProperty("DocumentsFolder").ToString();
            optionsBuilder.UseSqlite(@$"Data Source={documentPath + "\\" + data.RootElement.GetProperty("DBName")}");

            //uncomment this for migrate
            //optionsBuilder.UseSqlite(@$"Data Source=ironXsolutionDB.sqlite3");
        }
        private void CheckOrCreateConfig()
        {
            Config config;
            if (Config.Count()==0)
            {
                Task<Config> task = new Task<Config>(() => BaseFunctions.SetDefaultConfig());
                task.Start();
                config = task.Result;
                Config.Add(config);
                this.SaveChanges();
            }
        }
        public Config GetConfig() 
        {
            return Config.FirstOrDefault();
        }

        public void SetNewConfigPassword(string password)
        {
            GetConfig().SuperUserPassword = password;
            this.SaveChanges();
        }

        public void SetNewConfigShift(int day, int night)
        {
            GetConfig().DayWork = day;
            GetConfig().NightWork = night;

            this.SaveChanges();
        }
    }
}
