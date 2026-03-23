DROP TABLE IF EXISTS movie;
DROP TABLE IF EXISTS movie_cinecheck;
DROP TABLE IF EXISTS reservation_seat;
DROP TABLE IF EXISTS reservation;
DROP TABLE IF EXISTS movie_show;

CREATE TABLE movie
(
    movie_id    INTEGER PRIMARY KEY AUTOINCREMENT,
    slug        varchar(300) UNIQUE,
    title       varchar(300),
    description varchar(300),
    poster      varchar(256),
    duration    INTEGER
);

CREATE TABLE movie_cinecheck
(
    movie_id INTEGER REFERENCES movie (movie_id),
    label    VARCHAR(20),
    PRIMARY KEY (`movie_id`, `label`)
);

CREATE TABLE movie_show
(
    movie_id   INTEGER REFERENCES movie (movie_id),
    show_start INTEGER,
    PRIMARY KEY (`movie_id`, `show_start`)
);

CREATE TABLE reservation
(
    reservation_id INTEGER PRIMARY KEY AUTOINCREMENT,
    movie_id       INTEGER,
    show_start     INTEGER,
    FOREIGN KEY (movie_id, show_start) REFERENCES movie_show (movie_id, show_start)
);

CREATE TABLE reservation_seat
(
    reservation_id INTEGER REFERENCES reservation (reservation_id),
    seat_id        VARCHAR(5),
    customer_age   INTEGER,
    PRIMARY KEY (`reservation_id`, `seat_id`)
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

-- Insertion des films plannifiés
INSERT INTO movie_show (movie_id, show_start)
VALUES (1, 1762858800), -- 2025-11-11T12:00
       (2, 1762864800), -- 2025-11-11T13:40
       (1, 1762875060), -- 2025-11-11T16:31
       (3, 1762881900), -- 2025-11-11T18:25
       (5, 1762887840), -- 2025-11-11T20:04

       -- the next day
       (2, 1762945200), -- 2025-11-12T12:00
       (1, 1762955160), -- 2025-11-12T14:46
       (1, 1762962000), -- 2025-11-12T16:40
       (5, 1762968300), -- 2025-11-12T18:25
       (3, 1762974360);
-- 2025-11-12T20:06

-- Réservation 1 : Vaiana 2 - Séance du 11/11 à 12h00
-- Famille avec 2 adultes et 2 enfants
-- + 2ème famille ajouté
INSERT INTO reservation (movie_id, show_start)
VALUES (1, 1762858800);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (1, '5-8', 35),
       (1, '5-9', 32),
       (1, '5-10', 8),
       (1, '5-11', 6),
       (1, '4-5', 34), -- Mère
       (1, '4-6', 7);
-- Enfant


-- Réservation 2 : Dune 2 - Séance du 11/11 à 13h40
-- Couple d'adultes, places centrales
-- + couple senior + groupe SF
INSERT INTO reservation (movie_id, show_start)
VALUES (2, 1762864800);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (2, '7-15', 28),
       (2, '7-16', 26),
       (2, '8-10', 68),
       (2, '8-11', 66),
       (2, '6-18', 25),
       (2, '6-19', 27),
       (2, '6-20', 26);


-- Réservation 3 : Vaiana 2 - Séance du 11/11 à 16h31
-- Grand groupe familial (3 générations)
-- + jeune couple avec bébé
INSERT INTO reservation (movie_id, show_start)
VALUES (1, 1762875060);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (3, '6-5', 65),
       (3, '6-6', 63),
       (3, '6-7', 38),
       (3, '6-8', 36),
       (3, '6-9', 10),
       (3, '6-10', 7),
       (3, '8-5', 28),
       (3, '8-6', 27),
       (3, '8-7', 2);


-- Réservation 4 : Kung Fu Panda 4 - Séance du 11/11 à 18h25
-- Anniversaire enfant avec amis
-- + famille monoparentale
INSERT INTO reservation (movie_id, show_start)
VALUES (3, 1762881900);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (4, '4-12', 9),
       (4, '4-13', 8),
       (4, '4-14', 9),
       (4, '4-15', 8),
       (4, '4-16', 9),
       (4, '5-12', 42),
       (4, '5-13', 40),
       (4, '5-5', 41),
       (4, '5-6', 11),
       (4, '5-7', 9);


-- Réservation 5 : Dune 2 - Séance du 12/11 à 12h00
-- Solo - amateur de cinéma
-- + Cinéphile solo (2e)
INSERT INTO reservation (movie_id, show_start)
VALUES (2, 1762945200);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (5, '7-17', 31),
       (5, '8-18', 29);


-- Réservation 6 : Vaiana 2 - Séance du 12/11 à 14h46
-- Petite famille
-- + grands-parents avec petits-enfants
INSERT INTO reservation (movie_id, show_start)
VALUES (1, 1762955160);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (6, '5-6', 29),
       (6, '5-7', 30),
       (6, '5-8', 5),
       (6, '6-15', 72),
       (6, '6-16', 74),
       (6, '6-17', 6),
       (6, '6-18', 4);


-- Réservation 7 : Vaiana 2 - Séance du 12/11 à 16h40
-- Groupe d'amis adolescents
-- + groupe centre aéré
INSERT INTO reservation (movie_id, show_start)
VALUES (1, 1762962000);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (7, '3-10', 15),
       (7, '3-11', 16),
       (7, '3-12', 15),
       (7, '3-13', 16),
       (7, '3-14', 15),
       (7, '2-8', 8),
       (7, '2-9', 9),
       (7, '2-10', 7),
       (7, '2-11', 8),
       (7, '2-12', 9),
       (7, '2-13', 8),
       (7, '3-8', 24),
       (7, '3-9', 26);


-- Réservation 8 : Vice-Versa 2 - Séance du 12/11 à 18h25
-- Couple avec adolescent
-- + famille recomposée
INSERT INTO reservation (movie_id, show_start)
VALUES (5, 1762968300);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (8, '6-12', 45),
       (8, '6-13', 43),
       (8, '6-14', 14),
       (8, '7-15', 38),
       (8, '7-16', 36),
       (8, '7-17', 13),
       (8, '7-18', 11);


-- Réservation 9 : Kung Fu Panda 4 - Séance du 12/11 à 20h06
-- Groupe de jeunes adultes (soirée entre amis)
-- + couple sans enfants
INSERT INTO reservation (movie_id, show_start)
VALUES (3, 1762974360);

INSERT INTO reservation_seat (reservation_id, seat_id, customer_age)
VALUES (9, '7-8', 22),
       (9, '7-9', 23),
       (9, '7-10', 21),
       (9, '7-11', 22),
       (9, '8-12', 27),
       (9, '8-13', 28);
