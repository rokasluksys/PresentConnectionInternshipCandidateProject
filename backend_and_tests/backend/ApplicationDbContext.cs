using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace backend
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Package
            modelBuilder.Entity<Package>(entity =>
            {
                //Package has one "Sender" Contact, "Recipient" Contact has many "ReceivedPackages" Packages
                //sender can't be deleted if linked with package
                entity
                    .HasOne(p => p.Sender)
                    .WithMany(s => s.SentPackages)
                    .HasForeignKey(p => p.SenderID)
                    .OnDelete(DeleteBehavior.Restrict);

                //Package has one "Recipient" Contact, "Recipient" Contact has many "ReceivedPackages" Packages
                //recipient can't be deleted if linked with package
                entity
                    .HasOne(p => p.Recipient)
                    .WithMany(r => r.ReceivedPackages)
                    .HasForeignKey(p => p.RecipientID)
                    .OnDelete(DeleteBehavior.Restrict);

                //Package has many Statuses, a Status has one Package
                //status can't be deleted if linked with package
                entity
                    .HasMany(p => p.Statuses)
                    .WithOne(s => s.Package)
                    .HasForeignKey(s => s.PackageTrackingNumber)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //Status
            modelBuilder.Entity<Status>(entity =>
            {
                //Shaves off the ticks from the timestamp (2025-08-16T17:30:50.8384811Z => 2025-08-16T17:30:50Z)
                entity
                    .Property(s => s.Timestamp)
                    .HasConversion(
                        t => t.ToUniversalTime().Date.Add(t.TimeOfDay).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        t => DateTime.Parse(t, null, DateTimeStyles.AdjustToUniversal));
            });
        }
        public DbSet<Package> Package { get; set; } = default!;
        public DbSet<Contact> Contact {  get; set; } = default!;
    }
}
