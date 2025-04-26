using System.IO;
using Microsoft.EntityFrameworkCore;
using PythonAILib.Common;

namespace LibPythonAI.Data {
    public class PythonAILibDBContext : DbContext {

        public PythonAILibDBContext() : base() { }

        public DbSet<ContentFolderEntity> ContentFolders { get; set; }

        public DbSet<ContentFolderRootEntity> ContentFolderRoots { get; set; }

        public DbSet<ContentItemEntity> ContentItems { get; set; }

        //TagItemEntity
        public DbSet<TagItemEntity> TagItems { get; set; }

        // SearchRuleEntity
        public DbSet<SearchRuleEntity> SearchRules { get; set; }

        public DbSet<RAGSourceItemEntity> RAGSourceItems { get; set; }

        public DbSet<PromptItemEntity> PromptItems { get; set; }

        public DbSet<AutoProcessRuleEntity> AutoProcessRules { get; set; }

        public DbSet<AutoProcessItemEntity> AutoProcessItems { get; set; }

        // MainStatisticsEntity
        public DbSet<MainStatisticsEntity> MainStatistics { get; set; }

        private StreamWriter? logStream;

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
            logStream?.Close();
            logStream?.Dispose();
        }


        public static void Init() {
            using var context = new PythonAILibDBContext();
            context.Database.EnsureCreated();
            
            context.SaveChanges();

        }
    }
}
