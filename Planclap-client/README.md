# Planclap.Client

## Description

`Planclap.Client` est une application graphique pour acheter des places de cinéma.  Elle est à déployer sur des écrans tactiles installés devant les accès à la salle de réunion.

## Développer le projet

### Prérequis

Vous devez avoir le SDK .NET 10 installé sur votre machine. Pour récupérer le projet, il est préférable d'avoir un client `git` installé. Enfin, nous vous recommandons d'utiliser un EDI comme Rider ou Visual Studio 2022.

Vous devez définir un répertoire dans lequel se trouveront des paires de fichiers `(yyyy-mm-dd.csv; yyyy-mm-dd.json)`. Le fichier CSV contient le planning des séances de la semaine `yyyy-mm-dd.csv`, tandis que le fichier JSON contient les films associés aux séances.


### Construction

```bash
# Clonez le dépôt
> git clone https://git.helmo.be/students/info/q240078/planclap-client.git

> cd planclap-client # Accédez au répertoire du projet
> dotnet restore # Installez les dépendances 
> dotnet build # Construisez le projet
> dotnet test # Exécutez les tests unitaires
```

## Utilisation
Pour démarrer l'application, une commande est à disposition.
Celle ci comporte deux arguments (dont un obligatoire).
* `--dir` (requis): dossier d'emplacement des fichiers
* `--datetime`: heure pour laquelle démarrer l'application (sous forme de local datetime `yyyy-MM-ddTHH:mm`)
* 
### Commande simple
```
dotnet run --project "Planclap.Client.App" --dir=Planclap.Client.App\resources
```
### Commande complète
```
dotnet run --project "Planclap.Client.App" --dir=Planclap.Client.App\resources --datetime=2025-11-11T12:00
```
