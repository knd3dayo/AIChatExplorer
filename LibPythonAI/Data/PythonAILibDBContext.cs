using Microsoft.EntityFrameworkCore;
using PythonAILib.Common;

namespace LibPythonAI.Data {
    public class PythonAILibDBContext : DbContext {

        public DbSet<ContentFolderEntity> ContentFolders { get; set; }

        public DbSet<ContentItemEntity> ContentItems { get; set; }

        //TagItemEntity
        public DbSet<TagItemEntity> TagItems { get; set; }

        // SearchRuleEntity
        public DbSet<SearchRuleEntity> SearchRules { get; set; }

        public DbSet<RAGSourceItemEntity> RAGSourceItems { get; set; }

        public DbSet<VectorDBItemEntity> VectorDBItems { get; set; }

        public DbSet<PromptItemEntity> PromptItems { get; set; }

        public DbSet<AutoProcessRuleEntity> AutoProcessRules { get; set; }

        // MainStatisticsEntity
        public DbSet<MainStatisticsEntity> MainStatistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            string dbPath = PythonAILibManager.Instance.ConfigParams.GetMainDBPath();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        public static void Init() {
            using var context = new PythonAILibDBContext();
            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}
