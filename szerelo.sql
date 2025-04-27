DROP DATABASE IF EXISTS szerelo;
CREATE DATABASE szerelo
DEFAULT CHARACTER SET utf8 COLLATE utf8_hungarian_ci;
USE szerelo;
-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Ápr 27. 11:05
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `szerelo`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `dolgozo`
--

CREATE TABLE `dolgozo` (
  `Dolgozo_id` int(11) NOT NULL,
  `Nev` varchar(255) DEFAULT NULL,
  `Beosztas` varchar(255) DEFAULT NULL,
  `Lakcim` varchar(255) DEFAULT NULL,
  `Telefonszam` varchar(20) DEFAULT NULL,
  `Email_cim` varchar(255) DEFAULT NULL,
  `Szemelyazonosito_igazolvany_szam` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `dolgozo`
--

INSERT INTO `dolgozo` (`Dolgozo_id`, `Nev`, `Beosztas`, `Lakcim`, `Telefonszam`, `Email_cim`, `Szemelyazonosito_igazolvany_szam`) VALUES
(1, 'Kiss Gergő', 'Autószerelő', 'Pécs, Munkás utca 10.', '+36301112233', 'gergo.kiss@szerelo.hu', '987654BA'),
(2, 'Fekete Virág', 'Recepciós', 'Szeged, Rózsa utca 2.', '+36204445566', 'virag.fekete@szerelo.hu', '321098DC'),
(3, 'Zöld Ádám', 'Diagnoszta', 'Budapest, Akácfa utca 15.', '+36707778899', 'adam.zold@szerelo.hu', '654321FE'),
(4, 'Barna Péter', 'Autóvillamossági szerelő', 'Debrecen, Liget utca 8.', '+36302223344', 'peter.barna@szerelo.hu', '112233GH'),
(5, 'Fehér Anna', 'Gumiszervizes', 'Győr, Vasút utca 3.', '+36205556677', 'anna.feher@szerelo.hu', '445566IJ'),
(6, 'Kék László', 'Karosszéria lakatos', 'Székesfehérvár, Fő utca 20.', '+36306667788', 'laszlo.kek@szerelo.hu', '778899KL'),
(7, 'Piros Mária', 'Alkatrész értékesítő', 'Veszprém, Kossuth utca 1.', '+36209990011', 'maria.piros@szerelo.hu', '001122MN'),
(8, 'Szürke Gábor', 'Autószerelő', 'Miskolc, Ipari Park 1.', '+36301239876', 'gabor.szurke@szerelo.hu', '334455OP'),
(9, 'Lila Katalin', 'Ügyfélszolgálat', 'Eger, Dobó tér 5.', '+36204567890', 'katalin.lila@szerelo.hu', '667788QR'),
(10, 'Narancs Zoltán', 'Diagnoszta', 'Sopron, Várkerület 10.', '+36707890123', 'zoltan.narancs@szerelo.hu', '990011ST'),
(11, 'Ezüst Tamás', 'Autószerelő', 'Zalaegerszeg, Ady Endre utca 30.', '+36303334455', 'tamas.ezust@szerelo.hu', '223344UV'),
(12, 'Arany Judit', 'Recepciós', 'Szolnok, Mártírok útja 12.', '+36206667788', 'judit.arany@szerelo.hu', '556677WX'),
(13, 'Bronz Ferenc', 'Gumiszervizes', 'Kaposvár, Fő utca 50.', '+36709990022', 'ferenc.bronz@szerelo.hu', '889900YZ'),
(14, 'Réz Andrea', 'Alkatrész értékesítő', 'Békéscsaba, Andrássy út 10.', '+36301122334', 'andrea.rez@szerelo.hu', '112233AB'),
(15, 'Acél Gábor', 'Autóvillamossági szerelő', 'Nyíregyháza, Széchenyi utca 25.', '+36204455667', 'gabor.acel@szerelo.hu', '445566CD');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `elvegzi`
--

CREATE TABLE `elvegzi` (
  `Javitas_id` int(11) NOT NULL,
  `Dolgozo_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `elvegzi`
--

INSERT INTO `elvegzi` (`Javitas_id`, `Dolgozo_id`) VALUES
(1, 1),
(2, 3),
(3, 1),
(4, 4),
(5, 1),
(6, 1),
(7, 6),
(8, 5),
(9, 8),
(10, 4),
(11, 8),
(12, 1),
(13, 8),
(14, 1),
(15, 8),
(16, 1),
(17, 8),
(18, 4),
(19, 8),
(20, 4);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `foglal`
--

CREATE TABLE `foglal` (
  `Idopont_id` int(11) NOT NULL,
  `Ugyfel_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `foglal`
--

INSERT INTO `foglal` (`Idopont_id`, `Ugyfel_id`) VALUES
(1, 1),
(2, 2),
(3, 3),
(5, 4),
(8, 6),
(10, 8),
(12, 10),
(14, 11),
(16, 13),
(18, 15),
(20, 17),
(22, 19),
(24, 20);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `idopont`
--

CREATE TABLE `idopont` (
  `Idopont_id` int(11) NOT NULL,
  `Datum` datetime DEFAULT NULL,
  `Nap` varchar(20) DEFAULT NULL,
  `Statusz` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `idopont`
--

INSERT INTO `idopont` (`Idopont_id`, `Datum`, `Nap`, `Statusz`) VALUES
(1, '2025-05-05 08:00:00', 'Hétfő', 'Foglalt'),
(2, '2025-05-05 10:00:00', 'Hétfő', 'Foglalt'),
(3, '2025-05-06 09:00:00', 'Kedd', 'Foglalt'),
(5, '2025-05-08 14:00:00', 'Csütörtök', 'Foglalt'),
(8, '2025-05-12 11:00:00', 'Hétfő', 'Foglalt'),
(10, '2025-05-14 13:00:00', 'Szerda', 'Foglalt'),
(12, '2025-05-16 11:00:00', 'Péntek', 'Foglalt'),
(14, '2025-05-20 14:00:00', 'Kedd', 'Foglalt'),
(16, '2025-05-22 10:00:00', 'Csütörtök', 'Foglalt'),
(18, '2025-05-26 09:00:00', 'Hétfő', 'Foglalt'),
(20, '2025-05-28 14:00:00', 'Szerda', 'Foglalt'),
(22, '2025-05-30 08:00:00', 'Péntek', 'Foglalt'),
(24, '2025-06-03 13:00:00', 'Kedd', 'Foglalt');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `jarmuvek`
--

CREATE TABLE `jarmuvek` (
  `Jarmu_id` int(11) NOT NULL,
  `Rendszam` varchar(20) DEFAULT NULL,
  `Alvazszam` varchar(20) DEFAULT NULL,
  `Marka` varchar(255) DEFAULT NULL,
  `Modell` varchar(255) DEFAULT NULL,
  `Gyartasi_ev` year(4) DEFAULT NULL,
  `Km_ora_allas` int(20) DEFAULT NULL,
  `Ugyfel_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `jarmuvek`
--

INSERT INTO `jarmuvek` (`Jarmu_id`, `Rendszam`, `Alvazszam`, `Marka`, `Modell`, `Gyartasi_ev`, `Km_ora_allas`, `Ugyfel_id`) VALUES
(1, 'PQR-456', 'VF100000000000001', 'Renault', 'Clio', '2019', 65000, 1),
(2, 'STU-789', 'WBA00000000000002', 'BMW', '320d', '2017', 150000, 1),
(3, 'VWX-123', 'TSM00000000000003', 'Suzuki', 'Vitara', '2021', 25000, 2),
(4, 'YZA-456', 'JFL00000000000004', 'Ford', 'Focus', '2018', 90000, 3),
(5, 'ABC-789', 'WOL00000000000005', 'Opel', 'Astra', '2016', 180000, 4),
(6, 'DEF-123', 'ZFA00000000000006', 'Fiat', '500', '2020', 40000, 5),
(7, 'GHI-456', 'WVW00000000000007', 'Volkswagen', 'Passat', '2015', 220000, 6),
(8, 'JKL-789', 'TMB00000000000008', 'Skoda', 'Octavia', '2019', 110000, 7),
(9, 'MNO-123', 'WDB00000000000009', 'Mercedes-Benz', 'C-osztály', '2020', 75000, 8),
(10, 'PQR-789', 'SAJ00000000000010', 'Toyota', 'Corolla', '2022', 18000, 9),
(11, 'STU-123', 'VVW00000000000011', 'Seat', 'Leon', '2017', 130000, 10),
(12, 'VWX-456', 'ZFA00000000000012', 'Fiat', 'Punto', '2014', 250000, 11),
(13, 'YZA-789', 'JMZ00000000000013', 'Mazda', '3', '2019', 88000, 12),
(14, 'AAB-111', 'VF100000000000014', 'Renault', 'Megane', '2018', 95000, 13),
(15, 'BBC-222', 'WBA00000000000015', 'BMW', 'X5', '2016', 190000, 14),
(16, 'CCD-333', 'TSM00000000000016', 'Suzuki', 'Swift', '2022', 12000, 15),
(17, 'DDE-444', 'JFL00000000000017', 'Ford', 'Kuga', '2020', 55000, 16),
(18, 'EEF-555', 'WOL00000000000018', 'Opel', 'Corsa', '2019', 70000, 17),
(19, 'FFG-666', 'ZFA00000000000019', 'Fiat', 'Tipo', '2017', 120000, 18),
(20, 'GGH-777', 'WVW00000000000020', 'Volkswagen', 'Golf Variant', '2018', 105000, 19),
(21, 'HHI-888', 'TMB00000000000021', 'Skoda', 'Superb', '2021', 30000, 20);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `javitasok`
--

CREATE TABLE `javitasok` (
  `Javitas_id` int(11) NOT NULL,
  `Megnevezes` varchar(255) DEFAULT NULL,
  `Leiras` text DEFAULT NULL,
  `Koltseg` int(10) DEFAULT NULL,
  `Datum` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `javitasok`
--

INSERT INTO `javitasok` (`Javitas_id`, `Megnevezes`, `Leiras`, `Koltseg`, `Datum`) VALUES
(1, 'Éves szerviz', 'Olaj-, levegő- és pollenszűrő csere, fékfolyadék ellenőrzés', 35000, '2025-04-20 10:00:00'),
(2, 'Vezérlés csere', 'Vezérműszíj és feszítők cseréje', 80000, '2025-04-21 14:00:00'),
(3, 'Klímarendszer töltés', 'Klímagáz feltöltése és rendszer ellenőrzése', 20000, '2025-04-22 09:30:00'),
(4, 'Fékbetét és féktárcsa csere (első)', 'Első fékbetétek és féktárcsák cseréje', 60000, '2025-04-23 11:00:00'),
(5, 'Akkumulátor csere', 'Új akkumulátor beszerelése', 40000, '2025-04-24 13:00:00'),
(6, 'Futómű beállítás', 'Futómű geometria ellenőrzése és beállítása', 25000, '2025-04-25 10:00:00'),
(7, 'Kipufogó javítás', 'Lyukas kipufogó szakasz hegesztése', 15000, '2025-04-25 14:00:00'),
(8, 'Gumicsere (téli/nyári)', 'Gumiabroncsok cseréje és centrírozás', 12000, '2025-04-26 09:00:00'),
(9, 'Olajszivárgás javítás', 'Motorolaj szivárgás forrásának felderítése és javítása', 50000, '2025-04-28 11:30:00'),
(10, 'Generátor csere', 'Hibás generátor cseréje', 70000, '2025-04-29 15:00:00'),
(11, 'Indítómotor javítás', 'Indítómotor hibájának elhárítása', 45000, '2025-04-30 08:00:00'),
(12, 'Kuplung csere', 'Kuplungszerkezet és tárcsa cseréje', 120000, '2025-05-02 10:00:00'),
(13, 'Lengéscsillapító csere', 'Első és hátsó lengéscsillapítók cseréje', 90000, '2025-05-05 13:00:00'),
(14, 'Fékfolyadék csere', 'Teljes fékfolyadék rendszer cseréje és légtelenítés', 18000, '2025-05-06 09:00:00'),
(15, 'Gyújtógyertya csere', 'Összes gyújtógyertya cseréje', 10000, '2025-05-07 11:00:00'),
(16, 'Pollenszűrő csere', 'Utastér levegőszűrő cseréje', 8000, '2025-05-07 11:30:00'),
(17, 'Váltóolaj csere', 'Manuális vagy automata váltó olaj cseréje', 30000, '2025-05-08 14:00:00'),
(18, 'Fényszóró beállítás', 'Fényszórók magasságának és irányának beállítása', 5000, '2025-05-09 09:00:00'),
(19, 'Kerékcsapágy csere', 'Hibás kerékcsapágy cseréje', 40000, '2025-05-12 10:00:00'),
(20, 'Önindító javítás/csere', 'Önindító problémák elhárítása', 55000, '2025-05-13 14:00:00');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `ugyfelek`
--

CREATE TABLE `ugyfelek` (
  `Ugyfel_id` int(11) NOT NULL,
  `Nev` varchar(255) DEFAULT NULL,
  `Email_cim` varchar(255) DEFAULT NULL,
  `Telefonszam` varchar(20) DEFAULT NULL,
  `Lakcim` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `ugyfelek`
--

INSERT INTO `ugyfelek` (`Ugyfel_id`, `Nev`, `Email_cim`, `Telefonszam`, `Lakcim`) VALUES
(1, 'Varga Zoltán', 'z.varga@email.com', '+36205551122', 'Debrecen, Foglaltság út 25.'),
(2, 'Kovács Nóra', 'nora.kovacs@freemail.hu', '+36309993344', 'Győr, Béke tér 1.'),
(3, 'Németh Tamás', 'tamas.nemeth@gmail.com', '+36701239876', 'Miskolc, Fő tér 10.'),
(4, 'Szabó Erika', 'erika.szabo@citromail.hu', '+36308889900', 'Pécs, Baross utca 7.'),
(5, 'Horváth István', 'istvan.horvath@indamail.hu', '+36201110099', 'Szeged, Aradi vértanúk tere 5.'),
(6, 'Tóth Katalin', 'katalin.toth@email.com', '+36304441122', 'Budapest, Alkotmány utca 3.'),
(7, 'Nagy Sándor', 'sandor.nagy@freemail.hu', '+36207778899', 'Szombathely, Petőfi Sándor utca 12.'),
(8, 'Molnár Ágnes', 'agnes.molnar@gmail.com', '+36702345678', 'Veszprém, Ady Endre utca 5.'),
(9, 'Fekete Dávid', 'david.fekete@citromail.hu', '+36305678901', 'Eger, Széchenyi utca 15.'),
(10, 'Bíró Eszter', 'eszter.biro@indamail.hu', '+36208901234', 'Sopron, Erzsébet utca 2.'),
(11, 'Papp Gergely', 'gergely.papp@email.com', '+36301234567', 'Szolnok, Kossuth tér 8.'),
(12, 'Takács Zsófia', 'zsofia.takacs@freemail.hu', '+36204567890', 'Kaposvár, Fő utca 18.'),
(13, 'Kovács László', 'laszlo.kovacs@gmail.com', '+36707890123', 'Székesfehérvár, Liget sor 3.'),
(14, 'Nagy Andrea', 'andrea.nagy@citromail.hu', '+36301112233', 'Zalaegerszeg, Dózsa György utca 7.'),
(15, 'Tóth Gábor', 'gabor.toth@indamail.hu', '+36204445566', 'Békéscsaba, Kazinczy utca 1.'),
(16, 'Varga Mária', 'maria.varga@email.com', '+36707778899', 'Nyíregyháza, Rákóczi út 15.'),
(17, 'Kiss Péter', 'peter.kiss@freemail.hu', '+36302223344', 'Sopron, Fő tér 2.'),
(18, 'Horváth Anna', 'anna.horvath@gmail.com', '+36205556677', 'Eger, Széchenyi tér 10.'),
(19, 'Szabó Zoltán', 'zoltan.szabo@citromail.hu', '+36708889900', 'Veszprém, Bajcsy-Zsilinszky utca 8.'),
(20, 'Molnár Dávid', 'david.molnar@indamail.hu', '+36301110099', 'Miskolc, Szentpáli utca 4.');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `vegez`
--

CREATE TABLE `vegez` (
  `Jarmu_id` int(11) NOT NULL,
  `Javitas_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `vegez`
--

INSERT INTO `vegez` (`Jarmu_id`, `Javitas_id`) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5),
(6, 6),
(7, 7),
(8, 8),
(9, 9),
(10, 10),
(11, 11),
(12, 12),
(13, 13),
(14, 14),
(15, 15),
(16, 16),
(17, 17),
(18, 18),
(19, 19),
(20, 20);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `dolgozo`
--
ALTER TABLE `dolgozo`
  ADD PRIMARY KEY (`Dolgozo_id`);

--
-- A tábla indexei `elvegzi`
--
ALTER TABLE `elvegzi`
  ADD PRIMARY KEY (`Javitas_id`,`Dolgozo_id`),
  ADD KEY `Dolgozo_id` (`Dolgozo_id`);

--
-- A tábla indexei `foglal`
--
ALTER TABLE `foglal`
  ADD PRIMARY KEY (`Idopont_id`,`Ugyfel_id`),
  ADD KEY `Ugyfel_id` (`Ugyfel_id`);

--
-- A tábla indexei `idopont`
--
ALTER TABLE `idopont`
  ADD PRIMARY KEY (`Idopont_id`);

--
-- A tábla indexei `jarmuvek`
--
ALTER TABLE `jarmuvek`
  ADD PRIMARY KEY (`Jarmu_id`),
  ADD KEY `Ugyfel_id` (`Ugyfel_id`);

--
-- A tábla indexei `javitasok`
--
ALTER TABLE `javitasok`
  ADD PRIMARY KEY (`Javitas_id`);

--
-- A tábla indexei `ugyfelek`
--
ALTER TABLE `ugyfelek`
  ADD PRIMARY KEY (`Ugyfel_id`);

--
-- A tábla indexei `vegez`
--
ALTER TABLE `vegez`
  ADD PRIMARY KEY (`Jarmu_id`,`Javitas_id`),
  ADD KEY `Javitas_id` (`Javitas_id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `dolgozo`
--
ALTER TABLE `dolgozo`
  MODIFY `Dolgozo_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT a táblához `idopont`
--
ALTER TABLE `idopont`
  MODIFY `Idopont_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT a táblához `jarmuvek`
--
ALTER TABLE `jarmuvek`
  MODIFY `Jarmu_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT a táblához `javitasok`
--
ALTER TABLE `javitasok`
  MODIFY `Javitas_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT a táblához `ugyfelek`
--
ALTER TABLE `ugyfelek`
  MODIFY `Ugyfel_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `elvegzi`
--
ALTER TABLE `elvegzi`
  ADD CONSTRAINT `elvegzi_ibfk_1` FOREIGN KEY (`Javitas_id`) REFERENCES `javitasok` (`Javitas_id`),
  ADD CONSTRAINT `elvegzi_ibfk_2` FOREIGN KEY (`Dolgozo_id`) REFERENCES `dolgozo` (`Dolgozo_id`);

--
-- Megkötések a táblához `foglal`
--
ALTER TABLE `foglal`
  ADD CONSTRAINT `foglal_ibfk_1` FOREIGN KEY (`Idopont_id`) REFERENCES `idopont` (`Idopont_id`),
  ADD CONSTRAINT `foglal_ibfk_2` FOREIGN KEY (`Ugyfel_id`) REFERENCES `ugyfelek` (`Ugyfel_id`);

--
-- Megkötések a táblához `jarmuvek`
--
ALTER TABLE `jarmuvek`
  ADD CONSTRAINT `jarmuvek_ibfk_1` FOREIGN KEY (`Ugyfel_id`) REFERENCES `ugyfelek` (`Ugyfel_id`);

--
-- Megkötések a táblához `vegez`
--
ALTER TABLE `vegez`
  ADD CONSTRAINT `vegez_ibfk_1` FOREIGN KEY (`Jarmu_id`) REFERENCES `jarmuvek` (`Jarmu_id`),
  ADD CONSTRAINT `vegez_ibfk_2` FOREIGN KEY (`Javitas_id`) REFERENCES `javitasok` (`Javitas_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
