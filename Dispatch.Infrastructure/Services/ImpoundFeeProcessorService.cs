using Dispatch.Domain.Entities;
using Dispatch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Infrastructure.Services
{
    public class ImpoundFeeProcessorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ImpoundFeeProcessorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var jobsToProcess = await db.JobRequests
                        .Where(j => j.IsImpoundTriggered && !j.IsFeeCalculated)
                        .ToListAsync();

                    foreach (var job in jobsToProcess)
                    {
                        // Simulate fee calculation logic
                        var fee = new ImpoundFeeRecord
                        {
                            Id = Guid.NewGuid(),
                            JobRequestId = job.Id,
                            TotalFee = 150.00m, // Placeholder logic
                            CalculatedOn = DateTime.UtcNow
                        };

                        db.ImpoundFees.Add(fee);

                        // Simulate first notice creation
                        var letter = new NotificationLetter
                        {
                            Id = Guid.NewGuid(),
                            JobRequestId = job.Id,
                            LetterType = "1st",
                            GeneratedOn = DateTime.UtcNow,
                            FilePath = $"letters/{job.Id}_1st.pdf" // Placeholder
                        };

                        db.NotificationLetters.Add(letter);

                        job.IsFeeCalculated = true;
                        job.FeeCalculationDate = DateTime.UtcNow;
                    }

                    await db.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check every 1 min
            }
        }
    }
}
