DROP TABLE IF EXISTS movie;
DROP TABLE IF EXISTS movie_cinecheck;
DROP TABLE IF EXISTS movie_to_plan;
DROP TABLE IF EXISTS movie_show;

# TABLE FOR EVERY MOVIE
CREATE TABLE movie
(
    movie_id    INTEGER PRIMARY KEY AUTO_INCREMENT,
    slug        varchar(300) UNIQUE,
    title       varchar(300),
    description varchar(300),
    poster      varchar(256),
    duration    int
);

# TABLE FOR EVERY CINECHECK PER MOVIE
CREATE TABLE movie_cinecheck
(
    movie_id INTEGER REFERENCES movie (movie_id),
    label    VARCHAR(20),
    PRIMARY KEY (`movie_id`, `label`)
);

# TABLE FOR EVERY MOVIE TO PLAN
CREATE TABLE movie_to_plan
(
    movie_id   INTEGER REFERENCES movie (movie_id),
    week       INTEGER,
    show_count INTEGER,
    PRIMARY KEY (`movie_id`, `week`)
);

# TABLE FOR THE PLANNING
CREATE TABLE movie_show
(
    movie_id   INTEGER REFERENCES movie (movie_id),
    show_start INTEGER,
    PRIMARY KEY (`movie_id`, `show_start`)
);

-- SCRIPT D'INSERTION
INSERT INTO movie (slug, title, description, poster, duration)
VALUES ('vaiana-2', 'Vaiana 2',
        'Vaiana part pour une nouvelle aventure épique à travers les océans de l''Océanie avec ses compagnons. Un voyage musical qui célèbre la famille et le courage.',
        'https://theposterdb.com/api/assets/515709/view', 100),
       ('dune-deuxieme-partie', 'Dune : Deuxième Partie',
        'Paul Atreides s''unit à Chani et aux Fremen pour mener la révolte contre ceux qui ont détruit sa famille. Face à un choix entre l''amour et le destin de l''univers.',
        'https://theposterdb.com/api/assets/478769/view', 166),
       ('kung-fu-panda-4', 'Kung Fu Panda 4',
        'Po doit former un nouveau guerrier du Dragon tout en affrontant une sorcière capable de ressusciter ses anciens ennemis. Action et humour pour toute la famille.',
        'https://theposterdb.com/api/assets/486505/view', 94),
       ('un-p-tit-truc-en-plus', 'Un p''tit truc en plus',
        'Pour échapper à la justice, deux braqueurs se font passer pour des éducateurs spécialisés. Une comédie touchante sur la différence et l''humanité.',
        'https://theposterdb.com/api/assets/519673/view', 99),
       ('vice-versa-2', 'Vice-Versa 2',
        'Riley entre dans l''adolescence et de nouvelles émotions débarquent au quartier général : Anxiété, Envie, Ennui et Embarras. Joie et ses amis doivent s''adapter.',
        'https://theposterdb.com/api/assets/465175/view', 96),
       ('godzilla-x-kong-le-nouvel-empire', 'Godzilla x Kong : Le Nouvel Empire',
        'Kong et Godzilla doivent unir leurs forces face à une menace colossale cachée dans les profondeurs de la Terre. Action spectaculaire et créatures titanesques.',
        'https://theposterdb.com/api/assets/468363/view', 115),
       ('le-comte-de-monte-cristo', 'Le Comte de Monte-Cristo',
        'Adaptation époustouflante du classique d''Alexandre Dumas. L''histoire de vengeance d''Edmond Dantès, trahi et emprisonné, qui devient le mystérieux Comte.',
        'https://theposterdb.com/api/assets/539099/view', 178),
       ('wicked', 'Wicked',
        'L''histoire inédite des sorcières du Pays d''Oz. L''amitié improbable entre Elphaba et Glinda avant qu''elles ne deviennent la Méchante Sorcière et Glinda la Bonne.',
        'https://theposterdb.com/api/assets/540735/view', 160),
       ('la-passion-de-dodin-bouffant', 'La Passion de Dodin Bouffant',
        'L''histoire d''amour entre un gastronome légendaire et sa cuisinière. Un hymne à la cuisine française et aux plaisirs de la table. Palme d''Or 2023.',
        'https://theposterdb.com/api/assets/468365/view', 135);

-- Insertion des cinechecks
INSERT INTO movie_cinecheck (movie_id, label)
VALUES (1, 'AL'),
       (2, '14'),
       (2, 'VIOLENCE'),
       (3, '6'),
       (4, '9'),
       (4, 'RUDE'),
       (5, 'AL'),
       (6, '9'),
       (6, 'VIOLENCE'),
       (6, 'FEAR'),
       (7, '12'),
       (7, 'VIOLENCE'),
       (8, '6'),
       (8, 'FEAR'),
       (9, '16'),
       (9, 'SEX');

-- Insertion du nombre de séances par semaine (semaine du 17/11/2025)
INSERT INTO movie_to_plan (movie_id, week, show_count)
VALUES (1, 1764547200, 5),
       (2, 1764547200, 3),
       (3, 1764547200, 5),
       (4, 1764547200, 4),
       (5, 1764547200, 3),
       (6, 1764547200, 3),
       (7, 1764547200, 4);