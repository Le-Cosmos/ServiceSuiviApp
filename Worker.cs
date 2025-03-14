using System.Diagnostics;

namespace serviceE;

// La classe Worker hérite de BackgroundService, ce qui permet de la faire fonctionner en tâche de fond dans une application.
public class Worker : BackgroundService
{   
    // Déclaration du chemin du fichier de log.
    private string logFilePath = "logfile.log";
    
    // Déclaration d'un logger pour enregistrer des informations de log dans le service.
    private readonly ILogger<Worker> _logger;

    // Variable pour contrôler le délai avant d'écrire un log.
    private int writeLogTimer = 0;

    // Liste qui stocke les informations sur les processus en cours, incluant leur ID, nom, heure de début, et l'heure de fermeture.
    private List<ProcessList> processL = Process.GetProcesses()
            .Select(q => new ProcessList(q.Id, q.ProcessName, DateTimeOffset.Now, null))
            .ToList();

    // Constructeur pour initialiser le logger.
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    // Méthode exécutée en tâche de fond. Elle est exécutée jusqu'à ce que le service soit annulé.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Boucle principale, s'exécutant jusqu'à l'annulation du service.
        while (!stoppingToken.IsCancellationRequested)
        {
            // Vérifie si le niveau de log "Information" est activé et, si oui, enregistre une entrée de log.
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            // Récupère la liste actuelle des processus en cours.
            var p = Process.GetProcesses()
                .Select(q => new ProcessList(q.Id, q.ProcessName, DateTimeOffset.Now, null))
                .ToList();

            // Compare les processus précédemment suivis et les marque comme fermés si un processus n'existe plus.
            foreach(var proc in processL)
            {
                if(!p.Contains(proc) && proc.CloseAt == null)
                {
                    proc.CloseAt = DateTimeOffset.Now;
                }
            }

            // Ajoute les nouveaux processus à la liste si ce n'est pas déjà fait.
            foreach(var proc in p)
            {
                if (!processL.Contains(proc))
                {
                    processL.Add(proc);
                }
            }

            // Vérifie si le délai pour écrire dans le fichier log a été dépassé (10 secondes) et, si oui, écrit les logs.
            if (writeLogTimer > 10000)
            {
                _logger.LogInformation($"ecriture des log");

                // Tentative d'écriture dans le fichier de log.
                using (StreamWriter writer = new StreamWriter(logFilePath))
                try
                {
                    // Crée une chaîne de caractères contenant les informations des processus suivis.
                    var str = "";
                    for(int i = 0; i < processL.Count; i++)
                    {
                        str += ($"{processL[i].ProcessName}; PID = {processL[i].Id}; openAt:{processL[i].StartTime}; closeAt:{processL[i].CloseAt}\n");
                    }

                    // Écrit les informations des processus dans le fichier log.
                    writer.WriteLine(str);
                }
                catch (Exception ex)
                {
                    // Si une erreur survient lors de l'écriture, log l'erreur.
                    _logger.LogInformation($"Erreur lors de l'écriture dans le fichier log : {ex.Message}");
                }

                // Réinitialise le timer de délai.
                writeLogTimer = 0;
            }

            // Augmente le timer à chaque itération de la boucle.
            writeLogTimer += 1000;

            // Attend 1 seconde avant de réexécuter la boucle.
            await Task.Delay(1000, stoppingToken);
        }
    }
}