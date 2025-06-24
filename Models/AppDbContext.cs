using Microsoft.EntityFrameworkCore;

namespace SpecShare.Models {
    public class AppDbContext : DbContext {
        public AppDbContext (
            DbContextOptions<AppDbContext> options
        ) : base (options) {}
        
        public DbSet<Studentt> StudentsData {get; set;}
        public DbSet<DepartmentModel> DepartmentsData {get; set; }
        public DbSet<SubjectModel> SubjectsData {get; set; }
        public DbSet<MaterialMoodel> Materials { get; set; }
    }
}