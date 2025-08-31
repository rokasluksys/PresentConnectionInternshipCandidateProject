using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Services;
using backend.Middleware;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("PackagesDb"));

            builder.Services.AddScoped<IRequestLogger, RequestLogger>();

            var app = builder.Build();
            app.UseCors();

            app.UseRequestLogging();

            // data seeding
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var senders = new List<Contact>
                {
                    new Contact { Name = "Amazon", Address = "Berlin, DE", Phone = "+49 123456" },
                    new Contact { Name = "eBay", Address = "Hamburg, DE", Phone = "+49 654321" },
                    new Contact { Name = "AliExpress", Address = "Shenzhen, CN", Phone = "+86 123456" }
                };

                var recipients = new List<Contact>
                {
                    new Contact { Name = "Paulius Paulauskas", Address = "Vilnius, LT", Phone = "+370 654321" },
                    new Contact { Name = "Jonas Jonaitis", Address = "Kaunas, LT", Phone = "+370 123456" },
                    new Contact { Name = "Mary Smith", Address = "London, UK", Phone = "+44 987654" },
                    new Contact { Name = "John Doe", Address = "New York, USA", Phone = "+1 555111" }
                };

                var packages = new List<Package>
                {
                    new Package
                    {
                        Name = "iPhone 16 Pro Max",
                        Sender = senders[0],
                        Recipient = recipients[0],
                        Statuses = new List<Status>
                        {
                            new Status { StatusName = Status.StatusTypes.Created, Timestamp = new DateTime(2025, 8, 15, 10, 30, 0, DateTimeKind.Utc) }
                        }
                    },
                    new Package
                    {
                        Name = "Samsung Galaxy S24 Ultra",
                        Sender = senders[1],
                        Recipient = recipients[1],
                        Statuses = new List<Status>
                        {
                            new Status { StatusName = Status.StatusTypes.Created, Timestamp = new DateTime(2025, 8, 12, 9, 15, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Sent, Timestamp = new DateTime(2025, 8, 13, 14, 45, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Accepted, Timestamp = new DateTime(2025, 8, 14, 11, 0, 0, DateTimeKind.Utc) }
                        }
                    },
                    new Package
                    {
                        Name = "Apple Watch Series 9",
                        Sender = senders[2],
                        Recipient = recipients[2],
                        Statuses = new List<Status>
                        {
                            new Status { StatusName = Status.StatusTypes.Created, Timestamp = new DateTime(2025, 8, 10, 8, 0, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Sent, Timestamp = new DateTime(2025, 8, 11, 10, 30, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Returned, Timestamp = new DateTime(2025, 8, 12, 16, 20, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Sent, Timestamp = new DateTime(2025, 8, 13, 9, 0, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Accepted, Timestamp = new DateTime(2025, 8, 14, 15, 45, 0, DateTimeKind.Utc) }
                        }
                    },
                    new Package
                    {
                        Name = "MacBook Pro 16\" M3",
                        Sender = senders[0],
                        Recipient = recipients[3],
                        Statuses = new List<Status>
                        {
                            new Status { StatusName = Status.StatusTypes.Created, Timestamp = new DateTime(2025, 8, 15, 12, 0, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Canceled, Timestamp = new DateTime(2025, 8, 15, 18, 30, 0, DateTimeKind.Utc) }
                        }
                    },
                    new Package
                    {
                        Name = "Dell XPS 15 2025",
                        Sender = senders[1],
                        Recipient = recipients[0],
                        Statuses = new List<Status>
                        {
                            new Status { StatusName = Status.StatusTypes.Created, Timestamp = new DateTime(2025, 8, 11, 7, 45, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Sent, Timestamp = new DateTime(2025, 8, 12, 13, 0, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Returned, Timestamp = new DateTime(2025, 8, 13, 9, 30, 0, DateTimeKind.Utc) },
                            new Status { StatusName = Status.StatusTypes.Canceled, Timestamp = new DateTime(2025, 8, 14, 11, 15, 0, DateTimeKind.Utc) }
                        }
                    }
                };

                db.Package.AddRange(packages);
                db.SaveChanges();
            }

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}