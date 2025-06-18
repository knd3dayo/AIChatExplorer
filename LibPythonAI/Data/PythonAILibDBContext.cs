using System.IO;
using LibPythonAI.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibPythonAI.Data {
    public class PythonAILibDBContext : DbContext {

        public PythonAILibDBContext() : base() { }

        public DbSet<ContentFolderEntity> ContentFolders { get; set; }

        public DbSet<ContentFolderRootEntity> ContentFolderRoots { get; set; }

        public DbSet<ContentItemEntity> ContentItems { get; set; }


        // MainStatisticsEntity
        public DbSet<MainStatisticsEntity> MainStatistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

            // var logFilePath = "logfile.txt";
            // logStream = new StreamWriter(logFilePath, append: true);
            // logStream.AutoFlush = true; // 自動的にバッファをフラッシュするように設定
           
            string dbPath = PythonAILibManager.Instance.ConfigParams.GetMainDBPath();
            // optionsBuilder.EnableSensitiveDataLogging().LogTo(logStream.WriteLine, LogLevel.Information).UseSqlite($"Data Source={dbPath}");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        public override void Dispose() {
            base.Dispose();
        }


        public static async Task Init() {
            await Task.Run(() => {
                using var context = new PythonAILibDBContext();
                context.Database.EnsureCreated();

                context.SaveChanges();

            });
        }
    }
}
