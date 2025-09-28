using Microsoft.EntityFrameworkCore;


namespace smartAttendents.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Classes> Classes { get; set; }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        public DbSet<FaceMatchImage> FaceMatchImages { get; set; }
        public DbSet<FaceRecognitionEvents> FaceRecognitionEvents { get; set; }
        public DbSet<Instructors> Instructors { get; set; }
        public DbSet<MoodleSyncLog> MoodleSyncLog { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<UserModel> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // هنا بتضيف أي إعدادات Fluent API إذا احتجت

            modelBuilder.Entity<UserModel>(e =>
            {
                // Username فريد
                e.HasIndex(u => u.Username).IsUnique();

                // منع أي Role خارج student / instructor
                e.HasCheckConstraint(
                    "CK_Users_Role",
                    "[Role] IN ( 'admin' , 'student','instructor')"
                );
            });

            // كل Class مربوط بـ Instructor (FK: Classes.InstructorID -> Instructors.InstructorID)
            modelBuilder.Entity<Classes>()
                .HasOne<Instructors>()
                .WithMany() // ما في Navigation عند المعلّم
                .HasForeignKey(c => c.InstructorID)
                .OnDelete(DeleteBehavior.Restrict);

            // كل Enrollment مربوط بـ Class
            modelBuilder.Entity<ClassEnrollment>()
                .HasOne<Classes>()
                .WithMany() // ما في Navigation
                .HasForeignKey(e => e.ClassID)
                .OnDelete(DeleteBehavior.Cascade);

            // وكل Enrollment مربوط بـ Student
            modelBuilder.Entity<ClassEnrollment>()
                .HasOne<Students>()
                .WithMany() // ما في Navigation
                .HasForeignKey(e => e.StudentID)
                .OnDelete(DeleteBehavior.Cascade);

            // ربط اختياري بـ Students / Instructors
            modelBuilder.Entity<UserModel>()
                .HasOne<Students>().WithMany()
                .HasForeignKey(u => u.StudentID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserModel>()
                .HasOne<Instructors>().WithMany()
                .HasForeignKey(u => u.InstructorID)
                .OnDelete(DeleteBehavior.SetNull);

            // فهارس فريدة مع فلتر (كل طالب/مدرّس له يوزر واحد)
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.StudentID).IsUnique().HasFilter("[StudentID] IS NOT NULL");

            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.InstructorID).IsUnique().HasFilter("[InstructorID] IS NOT NULL");

            // Students: بريد أو رقم جامعي فريد (اختياري مع فلتر)
            modelBuilder.Entity<Students>()
                .HasIndex(s => s.UniversityID).IsUnique().HasFilter("[UniversityID] IS NOT NULL");
            modelBuilder.Entity<Students>()
                .HasIndex(s => s.Email).IsUnique().HasFilter("[Email] IS NOT NULL");

            // Instructors: بريد فريد (اختياري)
            modelBuilder.Entity<Instructors>()
                .HasIndex(i => i.Email).IsUnique().HasFilter("[Email] IS NOT NULL");


        }

    }
}
