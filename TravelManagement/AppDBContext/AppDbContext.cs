using Microsoft.EntityFrameworkCore;
using TravelManagement.Models;

namespace TravelManagement.AppDBContext
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ExternalEmployee> ExternalEmployees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VehicleExpence> vehicleExpences { get; set; }
        public DbSet<Salary> salaries { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<OvertimeLog> overtimeLogs { get; set; }
        public DbSet<VehicleMaintenanceShedule> vehicleMaintenanceShedules { get; set; }
        public DbSet<TravelAgent> TravelAgents { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<BookingPaymentAllocation> BookingPaymentAllocations { get; set; }
        public DbSet<EmailInquiry> EmailInquiries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Customers>()
            //    .HasKey(c => c.CustomersId);

            modelBuilder.Entity<Booking>()
            .Property(b => b.Amount)
            .HasPrecision(18, 2);

            modelBuilder.Entity<Salary>()
            .Property(s => s.BaseSalay)
            .HasPrecision(18, 2);  // Precision of 18 and scale of 2 (e.g., 9999999999999999.99)

            modelBuilder.Entity<Salary>()
                .Property(s => s.Deduction)
                .HasPrecision(18, 2);  // Precision of 18 and scale of 2

            modelBuilder.Entity<Salary>()
                .Property(s => s.Overtimepay)
                .HasPrecision(18, 2);  // Precision of 18 and scale of 2

            modelBuilder.Entity<Salary>()
                .Property(s => s.NetSalaey)
                .HasPrecision(18, 2);  // Precision of 18 and scale of 2

            modelBuilder.Entity<User>()
                .Property(s => s.Salary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VehicleExpence>()
                .Property(a => a.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BookingPaymentAllocation>()
                .Property(a => a.AllocatedAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BookingPaymentAllocation>()
                .Property(a => a.PaidAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payments>()
                .Property(a => a.AmountPaid)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

            modelBuilder.Entity<OvertimeLog>()
                   .Property(o => o.hours)
                   .HasPrecision(5, 2); // e.g. max 999.99 hours

            modelBuilder.Entity<VehicleMaintenanceShedule>()
                .Property(v => v.cost)
                .HasPrecision(10, 2);
            
            modelBuilder.Entity<TravelAgent>()
               .Property(t => t.CommissionRate)
               .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);

        }
    }
}
