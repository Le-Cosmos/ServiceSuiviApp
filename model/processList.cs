// Déclaration de la classe ProcessList, qui sert à encapsuler les informations d'un processus.
public class ProcessList
{
    // Propriétés de la classe : 
    // - Id : Identifiant du processus (PID)
    // - ProcessName : Nom du processus (nom de l'application ou du programme)
    // - StartTime : Heure à laquelle le processus a démarré
    // - CloseAt : Heure à laquelle le processus a été fermé, nullable car ce n'est pas toujours défini.
    public int Id { get; set; }
    public string ProcessName { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? CloseAt { get; set; }

    // Constructeur qui permet d'initialiser les propriétés de la classe à partir des paramètres.
    public ProcessList(int id, string processName, DateTimeOffset startTime, DateTimeOffset? closeAt)
    {
        Id = id;
        ProcessName = processName;
        StartTime = startTime;
        CloseAt = closeAt;
    }

    // Redéfinition de la méthode Equals, utilisée pour comparer des objets ProcessList.
    // Deux objets ProcessList sont considérés égaux si leur ID et leur nom de processus sont identiques.
    public override bool Equals(object? obj)
    {
        return obj is ProcessList list &&
               Id == list.Id && ProcessName == list.ProcessName;
    }

    // Redéfinition de la méthode GetHashCode pour que les objets ProcessList aient des hash codes cohérents.
    // Cela est nécessaire lorsque la classe est utilisée dans des structures de données comme des dictionnaires ou des ensembles (sets).
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ProcessName);
    }
}