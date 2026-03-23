using System;
using CommandLine;

namespace Planclap.Client.App;

public record Options
{
    [Option('d', "dir", Required = false, HelpText = "Le chemin vers le dossier des fichier")]
    public string Dir { get; init; } = string.Empty;

    [Option('b', "bd", Required = false, HelpText = "Le string de connexion à la bd")]
    public string ConnectionString { get; init; } = string.Empty;

    [Option('t', "datetime", Required = false, HelpText = "Le moment à simuler au format iso (yyyy-MM-ddTHH:mm")]
    public string IsoDatetime { get; init; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
}
