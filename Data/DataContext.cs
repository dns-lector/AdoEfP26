using AdoEfP26.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AdoEfP26.Data
{
    public class DataContext : DbContext
    {
        // Контекст даних - відображення всієї БД
        // Таблиці БД відповідають колекціям (DbSet) сутностей
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.UserRole> UserRoles { get; set; }
        public DbSet<Entities.UserAccess> UserAccesses { get; set; }

        public DataContext() : base() { }

        // Налаштування контексту - переозначення двох методів: OnConfiguring, OnModelCreating

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // у даному методі налаштовується підключення до БД

            // дістаємось конфігурації 
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // зазначаємо рядок підключення і тип драйвера (SqlServer)
            optionsBuilder.UseSqlServer(
                config.GetConnectionString("LocalDb")
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // у даному методі налаштовуються зв'язки між сутностями
            modelBuilder.Entity<Entities.UserAccess>()
                .HasIndex(ua => ua.Login)
                .IsUnique();

            modelBuilder.Entity<Entities.UserAccess>()
                .HasOne(ua => ua.User)           // Налаштування зв'язків через 
                .WithMany(u => u.UserAccesses)   // навігаційні властивості
                .HasForeignKey(ua => ua.UserId)  // | Ці інструкції не обов'язкові
                .HasPrincipalKey(u => u.Id);     // | якщо додержуватись іменування

            // modelBuilder.Entity<Entities.User>()   // Аналогічна інструкція,
            //     .HasMany(u => u.UserAccesses)      // тільки починається з 
            //     .WithOne(ua => ua.User);           // Entity<Entities.User>()

            modelBuilder.Entity<Entities.UserAccess>()
                .HasOne(ua => ua.UserRole)          // За замовчанням FK - UserRoleId
                .WithMany(ur => ur.UserAccesses)    // Оскільки назва інша, то
                .HasForeignKey(ua => ua.RoleId);    // інструкція HasForeignKey обов'язкова

            // а також сідування - початкові дані які завжди будуть у БД
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = "SelfRegistered",
                    Description = "Самостійно зареєстрований користувач",
                    CanCreate = false,
                    CanRead = false,
                    CanUpdate = false,
                    CanDelete = false
                },
                new UserRole
                {
                    Id = "Employee",
                    Description = "Співробітник компанії",
                    CanCreate = true,
                    CanRead = true,
                    CanUpdate = false,
                    CanDelete = false
                },
                new UserRole
                {
                    Id = "Moderator",
                    Description = "Редактор контенту",
                    CanCreate = false,
                    CanRead = true,
                    CanUpdate = true,
                    CanDelete = true
                },
                new UserRole
                {
                    Id = "Administrator",
                    Description = "Системний адміністратор",
                    CanCreate = true,
                    CanRead = true,
                    CanUpdate = true,
                    CanDelete = true
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                    Name = "Палійчук Яків",
                    Email = "jakiv@ukr.net",
                    Birthdate = new DateTime(1998, 3, 15),
                    RegisteredAt = new DateTime(2025, 3, 10)
                },
                new User
                {
                    Id = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                    Name = "Сторож Чеслава",
                    Email = "storozh@ukr.net",
                    Birthdate = new DateTime(1999, 5, 11),
                    RegisteredAt = new DateTime(2025, 3, 15)
                },
                new User
                {
                    Id = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    Name = "Дністрянський Збоїслав",
                    Email = "dnistr@ukr.net",
                    Birthdate = new DateTime(1989, 7, 10),
                    RegisteredAt = new DateTime(2024, 8, 5)
                },
                new User
                {
                    Id = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    Name = "Гординська Діна",
                    Email = "dina@ukr.net",
                    Birthdate = new DateTime(2005, 2, 15),
                    RegisteredAt = new DateTime(2024, 12, 20)
                },
                new User
                {
                    Id = Guid.Parse("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                    Name = "Ромашко Жадан",
                    Email = "romashko@ukr.net",
                    Birthdate = new DateTime(2005, 2, 15),
                    RegisteredAt = new DateTime(2024, 12, 20)
                },
                new User
                {
                    Id = Guid.Parse("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                    Name = "Ерстенюк Вікторія",
                    Email = "erstenuk@ukr.net",
                    Birthdate = new DateTime(2001, 12, 21),
                    RegisteredAt = new DateTime(2025, 1, 21)
                },
                new User
                {
                    Id = Guid.Parse("a0f7b463-6eef-4a70-8444-789bbea23369"),
                    Name = "Бондарко Юрій",
                    Email = "bondarko@ukr.net",
                    Birthdate = new DateTime(1999, 10, 21),
                    RegisteredAt = new DateTime(2025, 2, 2)
                }
            );

            modelBuilder.Entity<UserAccess>().HasData(
                new UserAccess
                {
                    Id = Guid.Parse("e29b6a1a-5bc7-4f42-9fa4-db25de342b42"),
                    UserId = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                    Login = "jakiv",
                    Salt = "Salt1",
                    Dk = "Salt1123",
                    RoleId = "SelfRegistered"
                },
                new UserAccess
                {
                    Id = Guid.Parse("fb4ad18c-d916-4708-be71-a9bbcf1eb806"),
                    UserId = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                    Login = "storozh",
                    Salt = "Salt2",
                    Dk = "Salt2123",
                    RoleId = "Employee"
                },
                new UserAccess
                {
                    Id = Guid.Parse("b31355b7-aa02-4b10-afda-eb9ec8294e78"),
                    UserId = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    Login = "dnistr",
                    Salt = "Salt3",
                    Dk = "Salt3123",
                    RoleId = "SelfRegistered"
                },
                new UserAccess
                {
                    Id = Guid.Parse("92cd36b8-ea5a-4cbb-a232-268d942c97fd"),
                    UserId = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    Login = "dina",
                    Salt = "Salt4",
                    Dk = "Salt4123",
                    RoleId = "Employee"
                },
                new UserAccess
                {
                    Id = Guid.Parse("7a38a3aa-de9f-4519-bb48-eeb86c1efcdf"),
                    UserId = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    Login = "dina@ukr.net",
                    Salt = "Salt5",
                    Dk = "Salt5123",
                    RoleId = "Moderator"
                },
                new UserAccess
                {
                    Id = Guid.Parse("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                    UserId = Guid.Parse("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                    Login = "romashko",
                    Salt = "Salt6",
                    Dk = "Salt6123",
                    RoleId = "SelfRegistered"
                },
                new UserAccess
                {
                    Id = Guid.Parse("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                    UserId = Guid.Parse("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                    Login = "erstenuk",
                    Salt = "Salt7",
                    Dk = "Salt7123",
                    RoleId = "Employee"
                },
                new UserAccess
                {
                    Id = Guid.Parse("97191468-a02f-4a78-927b-9ea660e9ea36"),
                    UserId = Guid.Parse("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                    Login = "erstenuk@ukr.net",
                    Salt = "Salt8",
                    Dk = "Salt8123",
                    RoleId = "Administrator"
                },
                new UserAccess
                {
                    Id = Guid.Parse("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                    UserId = Guid.Parse("a0f7b463-6eef-4a70-8444-789bbea23369"),
                    Login = "bondarko",
                    Salt = "Salt9",
                    Dk = "Salt9123",
                    RoleId = "SelfRegistered"
                }
            );
        }

    }
}
