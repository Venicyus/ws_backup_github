using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Process;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backup {
    public class Worker : BackgroundService {
        private readonly ILogger<Worker> _logger;
        private static readonly int _countdias = 7;

        public Worker(ILogger<Worker> logger) {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    //-> Backup GitHub
                    GitHub gitHub = new GitHub();
                    await gitHub.InitBackup();
                    Console.WriteLine("Backup done successfully. Congratulations!");

                } catch (Exception ex) {
                    _logger.LogInformation($"The process failed: {ex.Message}");
                }

                _logger.LogInformation($"sleeping service {_countdias}");
                await Task.Delay((1000 * 60 * 24 * _countdias), stoppingToken);
            }
        }
    }
}
