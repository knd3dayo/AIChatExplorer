using Microsoft.EntityFrameworkCore;
using PythonAILib.Common;

namespace LibPythonAI.Data {
    public class PythonAILibDBContext : DbContext {

        public DbSet<ContentFolderEntity> Folders { get; set; }

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
            using (var context = new PythonAILibDBContext()) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Create ContentFolder
                ContentFolderEntity rootFolder = new() {
                    FolderName = "Root",
                    IsRootFolder = true
                };
                // Children
                ContentFolderEntity childFolder = new() {
                    FolderName = "Child",
                    ParentId = rootFolder.Id
                };
                context.Folders.Add(rootFolder);
                context.Folders.Add(childFolder);

                context.SaveChanges();

                // Querying child folder using LINQ
                ContentFolderEntity? child = context.Folders.Where(f => f.FolderName == "Child").FirstOrDefault();
                if (child != null) {
                    Console.WriteLine($"Child folder found: {child.FolderName}");
                    // Get root folder by child.Parent using LINQ
                    ContentFolderEntity? root = child.Parent;
                    if (root != null) {
                        Console.WriteLine($"Root folder found: {root.FolderName}");
                    } else {
                        Console.WriteLine("Root folder not found");
                    }


                } else {
                    Console.WriteLine("Child folder not found");
                }


            }
        }
    }
}
