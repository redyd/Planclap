DROP TABLE IF EXISTS movie;
DROP TABLE IF EXISTS movie_cinecheck;
DROP TABLE IF EXISTS movie_to_plan;
DROP TABLE IF EXISTS movie_show;

CREATE TABLE movie
(
    movie_id    INTEGER PRIMARY KEY AUTOINCREMENT,
    slug        varchar(300) UNIQUE,
    title       varchar(300),
    description varchar(300),
    poster      varchar(256),
    duration    int
);

CREATE TABLE movie_cinecheck
(
    movie_id INTEGER REFERENCES movie (movie_id),
    label    VARCHAR(20),
    PRIMARY KEY (`movie_id`, `label`)
);

CREATE TABLE movie_to_plan
(
    movie_id   INTEGER REFERENCES movie (movie_id),
    week       INTEGER,
    show_count INTEGER,
    PRIMARY KEY (`movie_id`, `week`)
);

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
        'https://theposterdb.com/api/assets/465175/view', 96);
-- Insertion des cinechecks
INSERT INTO movie_cinecheck (movie_id, label)
VALUES (1, 'AL'),
       (2, '14'),
       (2, 'VIOLENCE'),
       (3, '6'),
       (4, '9'),
       (4, 'RUDE'),
       (5, 'AL');

-- Insertion du nombre de séances par semaine (semaine du 17/11/2025)
INSERT INTO movie_to_plan (movie_id, week, show_count)
VALUES (1, 1763942400, 5),
       (2, 1763942400, 3),
       (3, 1763942400, 5),
       (4, 1763942400, 4),
       (5, 1763942400, 4);
