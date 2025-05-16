# Cnam_GeometryDash

## Gestion de projet

### Planification

Diagramme de Gantt

Backlog [Github](https://github.com/users/OneLiberty/projects/3/views/4)

Trello [Github](https://github.com/users/OneLiberty/projects/3/views/5)

### Gestion de code source

#### Commits

Les commits suivent une covention de nommage pour faciliter la compréhension de l'historique du projet. Voici les conventions utilisées :

- ADD : Ajout de fonctionnalité
- CHG : Modification dans le comportement d'une méthode/classe
- FIX : Correction d'un bug
- REM : Suppression de fonctionnalité, méthode/classe ou de fichier
- DOC : Ajout de documentation
- REF : Refactoring de code

#### Branches

L'ajout de fonctionnalité est fait par le biais de branches. Chaque fonctionnalité est doit être idéalement développée dans une branche lui étant propre.
Les branches sont nommées de la manière suivante :

- INITIALE-NOM_DE_LA_FONCTIONNALITE

Les branches sont ensuite mergées dans la branche de developpement (dev) une fois la fonctionnalité terminée et testée. Le merge est fait par le biais d'une pull request. La pull request peut être reviewée par d'autres membres de l'équipe avant d'être mergée dans la branche. Il est recommandé de détailler le plus possible la pull request afin de faciliter la compréhension par les autres membres.

#### Review de code

Les pull requests sont reviewées par d'autres membres de l'équipe, a défaut copilot. Chaque membre peut commenter et suggérer des modifications. Il est important de prendre en compte les retours des autres membres et de les intégrer dans la branche.

#### Gestion des bugs

Les bugs sont gérés par le biais d'issues sur Github. Chaque bug est décrit de manière précise afin de faciliter la compréhension du problème, comment le reproduire et de proposer une solution. Les bugs sont ensuite assignés à un membre de l'équipe qui est responsable de la résolution du bug. L'issue peut être cloturée par un commit de type FIX en précisant le numéro de l'issue dans le message de commit, il est aussi possible de cloturer l'issue manuellement en ajoutant un commentaire précisant les commits qui ont permis de résoudre le bug.

## Conception

### Use-case diagram

```mermaid
flowchart LR
    title[<u>Diagramme de cas d'utilisation - GeomeTry</u>]

    %% Actor
    Joueur([Joueur])

    %% Main menu use cases
    subgraph MenuPrincipal["Menu principal"]
        Jouer(["Jouer"])
        ChoisirNiveau(["Choisir un niveau"])
        CreerNiveau(["Créer un niveau"])
        EditerNiveau(["Editer un niveau"])
        ModifierOptions(["Modifier les options"])
        ConsulterStats(["Consulter les stats"])
        ConsulterStatsNiveau(["Consulter les stats d'un niveau"])
        ConsulterStatsGlobales(["Consulter les stats globales"])
        QuitterJeu(["Quitter le jeu"])

        subgraph Options
            ModifierControles(["Modifier les contrôles"])
            ModifierControlesJeu(["Modifier les contrôles de jeu"])
            ModifierControlesEditeur(["Modifier les contrôles de l'éditeur"])
            ModifierVolume(["Modifier le volume"])
        end
    end

    %% Gameplay use cases
    subgraph Partie
        MettrePause(["Mettre en pause"])
        Interagir(["Intéragir"])

        subgraph Gameplay
            SauterCube(["Sauter avec le cube"])
            VolerShip(["Voler avec le ship"])
            MonterWave(["Monter avec la wave"])
        end

        subgraph Pause
            ReprendrePartie(["Reprendre la partie"])
            QuitterPartie(["Quitter la partie"])
            RecommencerPartie(["Recommencer la partie"])
        end
    end

    %% Relationships
    Joueur --> Jouer
    Jouer -.-> ChoisirNiveau
    Joueur --> CreerNiveau
    CreerNiveau <-. "<<extends>>" .-> EditerNiveau
    Joueur --> ModifierOptions
    Joueur --> ConsulterStats
    Joueur --> QuitterJeu

    ModifierOptions <-. "<<extends>>" .-> ModifierControles
    ModifierControles <-. "<<extends>>" .-> ModifierControlesJeu
    ModifierControles <-. "<<extends>>" .-> ModifierControlesEditeur
    ModifierOptions <-. "<<extends>>" .-> ModifierVolume

    ConsulterStats <-. "<<extends>>" .-> ConsulterStatsNiveau
    ConsulterStats <-. "<<extends>>" .-> ConsulterStatsGlobales

    Jouer <-. "<<extends>>" .-> MettrePause
    Jouer <-. "<<extends>>" .-> Interagir

    Interagir <-. "<<extends>>" .-> SauterCube
    Interagir <-. "<<extends>>" .-> VolerShip
    Interagir <-. "<<extends>>" .-> MonterWave

    MettrePause <-. "<<extends>>" .-> ReprendrePartie
    MettrePause <-. "<<extends>>" .-> QuitterPartie
    MettrePause <-. "<<extends>>" .-> RecommencerPartie
```

### Activity diagram

### State diagram

### Class diagram

diagrammes (usecase, activity, state, class)

## Réalisation

blabla enjeux clé, difficulté

## Livraison

Lien vers la release
Explication sur le jeu, les contrôles...

```

```
