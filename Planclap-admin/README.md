# Planclap Admin

Application console (Java 21, Gradle) pour préparer la programmation d'un cinéma : on encode les films à projeter la semaine suivante, puis l'application génère le planning des séances.

Le planning généré est ensuite lu par [Planclap Client](../Planclap-client/README.md), la borne d'achat de tickets.

## Prérequis

- Un JDK 21 (`java -version` pour vérifier).
- Rien d'autre : Gradle est fourni par le wrapper (`./gradlew`), qui se télécharge tout seul au premier lancement.

Toutes les commandes se lancent depuis le dossier `Planclap-admin`. Sous Windows, remplacez `./gradlew` par `gradlew.bat`.

## Démarrage rapide

```bash
./gradlew run -q --args="--dir=src/main/resources"
```

Le dossier `app/src/main/resources` contient déjà un fichier de films prêt à l'emploi. Le menu s'affiche :

```
1. Films a planifier
2. Ajouter un film a planifier
3. Rechercher un film
4. Generer le planning
5. Quitter
```

> Le chemin de `--dir` est relatif au dossier `app/`, pas à `Planclap-admin/`. Un chemin absolu fonctionne aussi.

## Le menu

| Choix | Ce que ça fait |
|---|---|
| 1 | Affiche les films à planifier pour lundi prochain, avec la durée et le nombre de séances de chacun, et le total d'heures. |
| 2 | Ajoute un film : titre, description, URL de l'affiche, durée en minutes, nombre de séances, âge minimum, puis les CineChecks. |
| 3 | Recherche un film par son nom ; la recherche tolère jusqu'à 3 fautes de frappe. |
| 4 | Génère le planning de la semaine et l'enregistre. |
| 5 | Quitte. |

## Les fichiers

L'application travaille toujours pour **le lundi qui suit la date du jour**. Dans le dossier donné à `--dir` :

| Fichier | Rôle |
|---|---|
| `<lundi prochain>.json` | Les films à planifier. Lu au démarrage, réécrit à chaque ajout. Créé vide s'il n'existe pas. |
| `<lundi prochain>.csv` | Le planning. Écrit par le choix 4, écrase le précédent. |

Exemple : lancée entre le 5 et le 11 octobre 2026, l'application utilise `2026-10-12.json` et produit `2026-10-12.csv`.

Pour réutiliser le jeu de films fourni une autre semaine, copiez-le sous le nom du lundi voulu :

```bash
cp app/src/main/resources/2026-10-12.json app/src/main/resources/2026-10-19.json
```

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

### Règles de validité

| Champ | Règle |
|---|---|
| `title` | Non vide. Deux films ne peuvent pas avoir le même titre. |
| `description` | 1 à 200 caractères. |
| `duration` | 1 à 240 minutes. |
| `seances` | 1 à 9. |
| `poster` | Une URL non vide. |
| `cinechecks` | Exactement un âge parmi `AL`, `6`, `9`, `12`, `14`, `16`, `18`, plus zéro ou plusieurs mentions parmi `Violence`, `Peur`, `Sexe`, `Paroles grossieres`, `Discrimination`, `Drogues, alcool et fumer`. |
| Total | La somme `duration × seances` de tous les films doit rester sous 77 heures. |

Le champ `slug` est recalculé à partir du titre ; sa valeur dans le fichier est ignorée à la lecture.

> **Attention :** si un seul film du fichier est invalide, l'application affiche une liste vide (« Aucune seance pour le moment ») sans message d'erreur.

### Générer le planning

La génération demande **au moins 70 heures** de films (et donc moins de 77). Le jeu fourni en contient 70 h 54 : la génération fonctionne tout de suite, et il reste de la place pour ajouter un film d'environ 6 heures au total (par exemple 3 séances de 1 h 40).

Le planning place les séances de 12 h à 23 h, du lundi au dimanche :

```csv
date,startTime,endTime,slug
2026-10-12,12:00,13:39,un-p-tit-truc-en-plus
2026-10-12,13:45,15:25,vaiana-2
```

## Utiliser une base de données

À la place de `--dir`, l'application peut travailler sur une base MySQL :

```bash
./gradlew run -q --args="--db=jdbc:mysql://<hote>:<port>/<base> --user=<utilisateur> --pwd=<mot de passe>"
```

Les trois arguments sont obligatoires ensemble. Les tables se créent avec le script `infrastructures/src/main/java/org/helmo/planclap_admin/infrastructures/scripts/init.sql`.

Si les arguments manquent ou si la connexion échoue, l'application affiche `=== argument requis db manquant ou incorrect ===` et s'arrête.

## Enchaîner avec le client

Une fois le planning généré (choix 4), le client peut le lire directement dans le même dossier. Depuis `Planclap-client` :

```bash
dotnet run --project Planclap.Client.App -- --dir=../Planclap-admin/app/src/main/resources --datetime=2026-10-12T12:00
```

`--datetime` doit tomber dans la semaine du planning généré.

## Tests et qualité

```bash
./gradlew test     # tests unitaires
./gradlew check    # tests + couverture JaCoCo + analyse PMD
```

Les rapports sont dans `app/build/reports/`.

## Organisation du code

| Module | Contenu |
|---|---|
| `app` | Point d'entrée (`Program`), lecture des arguments, assemblage des dépendances. |
| `domains` | Le métier : films, CineChecks, planning, génération, services. Ne dépend d'aucun autre module. |
| `infrastructures` | Lecture et écriture : JSON, CSV, SQL. |
| `presentations` | Presenters, validation des saisies, menu de commandes. |
| `views` | Affichage et saisie dans la console. |

## Dépannage

| Symptôme | Cause |
|---|---|
| `=== argument requis db manquant ou incorrect ===` | `--dir` pointe vers un dossier inexistant (rappel : relatif à `app/`), ou les arguments de base de données sont incomplets ou refusés. |
| La liste des films est vide alors que le JSON est rempli | Un film du fichier ne respecte pas les règles de validité, ou le fichier ne porte pas la date du lundi prochain. |
| « Generer le planning » échoue | Le total est sous 70 heures. |
| Une barre `<====> 95% EXECUTING` recouvre le menu | Gradle est lancé en console riche ; `org.gradle.console=plain` dans `gradle.properties` l'évite (déjà en place). |
