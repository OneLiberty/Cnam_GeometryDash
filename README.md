# Cnam_GeometryDash

## Gestion de projet

### Planification

Le projet à débuté le 26 mars 2025 et doit être livré le 17 mai 2025. Le planning est divisé en plusieurs étapes, chacune correspondant à une fonctionnalité ou un ensemble de fonctionnalités. Chaque étape est représentée par un milestone dans le diagramme de Gantt prévisionnel ci-dessous.

```mermaid 
%%{init: { 'gantt': {'leftPadding': 100} } }%%

gantt
    title Planning du développement de Geometry Dash
    section Définition <br>du besoin <br> et conception
    Modélisation Architecture (Diagramme de classes)        :t1, 2025-03-26, 1d
    Modélisation du Gameplay (Diagramme d'activités)        :t2, 2025-03-26, 1d
    Modélisation du Gameplay (Diagramme d'états)            :t3, 2025-03-26, 1d
    Définition des cas d'utilisation                        :t4, 2025-03-26, 1d
    Milestone 1 - Définition du besoin et conception        :crit, milestone,ML1, after t1 t2 t3 t4, 0d
    section Menu Principal
    Menu Principal                                          :t5, after ML1, 1d
    Selection de niveau                                     :t6, after t5, 3d
    Options - Editions des touches de jeu                   :t7, after t5, 4d
    Options - Editions des touches de l'éditeur             :t8, after t7, 1d
    Options - Gestion de l'audio                            :t9, after t5, 3d
    Milestone 2 - Menu principal                            :crit, milestone,ML2, after t5 t6 t7 t8 t9, 0d
    section Niveau
    Mode de jeu - Cube                                      :t10, after ML2, 5d
    Gestion du niveau, serialisation, deserialisation       :t11, after t10, 6d
    Gestion des obstacles                                   :t12, after t11, 2d
    Mode de jeu - Ship                                      :t13, after t12, 2d
    Mode de jeu - Wave                                      :t14, after t12, 2d
    Gestion des bonus                                       :t15, after t13 t14, 3d
    Milestone 3 - Niveau                                    :crit, milestone,ML3, after t10 t11 t12 t13 t14 t15, 0d
    section Statistiques
    Statitistiques des niveaux                              :t16, after ML3, 2d
    Statitistiques générales                                :t17, after ML3, 2d
    Milestone 4 - Statistiques                              :crit, milestone,ML4, after t16 t17, 0d
    section Editeur de niveaux
    Editeur - Creation de niveau                            :t18, after ML4, 4d
    Editeur - Placement/Gestion des objets                  :t19, after t18, 2d
    Editeur - Gérer les métadonnées du niveau               :t20, after t19, 1d
    Editeur - Chargement/Export des niveaux                 :t21, after t20, 2d
    Milestone 5 - Editeur de niveaux                        :crit, milestone,ML5, after t18 t19 t20 t21, 0d
    section Intelligence Artificielle
    IA - Cube                                               :t22, after ML5, 6d
    IA - Ship                                               :t23, after t22, 3d
    IA - Wave                                               :t24, after t22, 3d
    Milestone 6 - IA                                        :crit, milestone,ML6, after t22 t23 t24, 0d
    section Documention et livraison
    Rédaction du rapport                                    :t25, after ML6, 1d
    Livraison                                               :t26, after ML6, 1d
    Milestone 7 - Livraison                                 :crit, milestone, ML7, 2025-05-17, 0d
```

#### Outils de gestion de projet
Le projet est géré par le biais de la section Projects de Github. Ci dessous les liens vers le backlog et le tableau de gestion de projet.

- Backlog : [Github](https://github.com/users/OneLiberty/projects/3/views/4)
- Trello : [Github](https://github.com/users/OneLiberty/projects/3/views/5)


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
