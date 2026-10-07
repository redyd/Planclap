# Planclap Client

Application graphique (.NET 10, Avalonia) pour acheter des places de cinéma. Elle est prévue pour des écrans tactiles installés à l'entrée de la salle : on choisit une séance du jour, ses sièges, le type de ticket, puis on paie.

Le planning qu'elle affiche est produit par [Planclap Admin](../Planclap-admin/README.md).

## Prérequis

- Le SDK .NET 10 (`dotnet --list-sdks` pour vérifier).
- Un environnement graphique (l'application ouvre une fenêtre).
- Une connexion internet pour afficher les affiches des films.

Toutes les commandes se lancent depuis le dossier `Planclap-client`.

## Démarrage rapide

```bash
dotnet run --project Planclap.Client.App -- --dir=Planclap.Client.App/resources --datetime=2026-10-08T12:00
```

Le dossier `Planclap.Client.App/resources` contient déjà un planning et ses films pour la semaine du 5 octobre 2026. Cette commande simule le jeudi 8 octobre à midi, elle fonctionne donc quelle que soit la date du jour.

> Le `--` sépare les arguments de `dotnet run` de ceux de l'application. Gardez-le.

## Arguments

| Argument | Rôle |
|---|---|
| `--dir`, `-d` | Dossier contenant les fichiers de planning et de films. |
| `--bd`, `-b` | Chaîne de connexion à une base MySQL, à la place de `--dir`. |
| `--datetime`, `-t` | Moment à simuler, au format `yyyy-MM-ddTHH:mm`. Par défaut : maintenant. |

Il faut donner `--dir` ou `--bd`. Si les deux sont présents, `--dir` l'emporte.

### Avec la date du jour

```bash
dotnet run --project Planclap.Client.App -- --dir=Planclap.Client.App/resources
```

Ne fonctionne que s'il existe des fichiers pour la semaine en cours (voir ci-dessous).

### Avec une base de données

```bash
dotnet run --project Planclap.Client.App -- --bd="Server=<hote>;Port=<port>;Database=<base>;User ID=<utilisateur>;Password=<mot de passe>"
```

### Dates en français

Les dates s'affichent dans la langue du système. Sur une machine en anglais, sous Linux ou macOS :

```bash
LC_ALL=fr_BE.UTF-8 dotnet run --project Planclap.Client.App -- --dir=Planclap.Client.App/resources --datetime=2026-10-08T12:00
```

## Les fichiers

L'application cherche, dans le dossier donné à `--dir`, une paire de fichiers portant la date du **lundi de la semaine en cours** (ou de la semaine simulée avec `--datetime`) :

| Fichier | Rôle |
|---|---|
| `<lundi>.csv` | Le planning de la semaine et les réservations. Mis à jour à chaque achat. |
| `<lundi>.json` | Les films correspondants. |

Seules les séances du jour sont proposées. Exemple : avec `--datetime=2026-10-08T12:00`, l'application lit `2026-10-05.csv` et `2026-10-05.json`, et affiche les séances du 8 octobre.

Paires fournies dans `Planclap.Client.App/resources` :

| Semaine | Commande |
|---|---|
| 5 au 11 octobre 2026 | `--datetime=2026-10-08T12:00` |
| 10 au 16 novembre 2025 | `--datetime=2025-11-11T12:00` |

### Format du CSV

```csv
date,startTime,endTime,slug,reservations
2026-10-05,12:00,13:34,kung-fu-panda-4,0-0=N|0-1=C|1-2=S
```

La colonne `reservations` est facultative : un planning tout juste généré par l'admin n'en a pas. Chaque réservation s'écrit `rangée-siège=type`, séparées par `|`, avec comme type `N` (normal), `C` (enfant) ou `S` (senior). La salle compte 10 rangées de 8 sièges, numérotés à partir de 0.

### Format du JSON

```json
{
  "movies": [
    {
      "slug": "vaiana-2",
      "title": "Vaiana 2",
      "duration": 100,
      "poster": "https://theposterdb.com/api/assets/515709/view",
      "description": "Vaiana part pour une nouvelle aventure épique à travers les océans.",
      "cinechecks": ["AL"],
      "seances": 5
    }
  ]
}
```

Le `slug` fait le lien entre un film du JSON et ses séances dans le CSV. La description ne dépasse pas 200 caractères. `cinechecks` contient un âge (`AL`, `6`, `9`, `12`, `14`, `16`, `18`) et, au besoin, des mentions parmi `Violence`, `Peur`, `Sexe`, `Paroles grossieres`, `Discrimination`, `Drogues, alcool et fumer`.

## Enchaîner avec l'admin

Le client lit directement ce que l'admin produit.

1. Dans `Planclap-admin`, lancez l'admin et générez le planning (choix 4) :

   ```bash
   ./gradlew run -q --args="--dir=src/main/resources"
   ```

2. Dans `Planclap-client`, ouvrez le client sur ce même dossier, à une date de la semaine planifiée :

   ```bash
   dotnet run --project Planclap.Client.App -- --dir=../Planclap-admin/app/src/main/resources --datetime=2026-10-12T12:00
   ```

L'admin planifie toujours la semaine suivante : adaptez la date de `--datetime` au nom du fichier `.csv` qu'il vient de créer.

## Construire et tester

```bash
dotnet build
dotnet test
```

Deux particularités de `dotnet test` :

- Un test compare une date écrite en français. Sur une machine en anglais, lancez `LC_ALL=fr_BE.UTF-8 dotnet test`.
- Après les tests, une étape génère un rapport de couverture avec un outil Windows. Sous Linux et macOS elle se termine par une erreur `MSB3073` ; les résultats des tests, affichés juste au-dessus, restent valables.

## Organisation du code

| Projet | Contenu |
|---|---|
| `Planclap.Client.App` | Point d'entrée, lecture des arguments, assemblage des dépendances. |
| `Planclap.Client.Domains` | Le métier : films, séances, salle, réservations, services. |
| `Planclap.Client.Infrastructures` | Lecture et écriture : CSV, JSON, SQL. |
| `Planclap.Client.Presentations` | ViewModels et navigation. |
| `Planclap.Client.Views` | Fenêtre et pages Avalonia. |
| `*.Tests` | Tests unitaires de chaque projet, et tests d'architecture. |

## Dépannage

L'application écrit ses messages dans la console et dans `log.txt`.

| Message | Cause |
|---|---|
| `Pas de planning pour aujourd'hui` | Pas de fichier `.csv` ou `.json` pour le lundi de la semaine visée, ou aucune séance ce jour-là. Vérifiez `--datetime`. |
| `Les arguments passes sont invalides` | Le dossier de `--dir` n'existe pas, la base de `--bd` est injoignable, ou aucun des deux n'est donné. |
| `La source de donnees contient des elements invalides` | Le JSON est vide ou mal formé, ou le CSV n'a pas ses colonnes `date`, `startTime`, `endTime`, `slug`. |
| `La salle de cinema est trop petite pour les reservations` | Le CSV contient une réservation hors de la salle (10 rangées de 8 sièges). |
| `Errors in command-line args detected` | Un argument est mal écrit ; vérifiez la présence du `--` après le nom du projet. |
