-- phpMyAdmin SQL Dump
-- version 5.0.4
-- https://www.phpmyadmin.net/
--
-- Hôte : localhost
-- Généré le : mar. 09 mars 2021 à 18:12
-- Version du serveur :  10.4.17-MariaDB
-- Version de PHP : 7.3.27

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `Users`
--

-- --------------------------------------------------------

--
-- Structure de la table `Authentification`
--

CREATE TABLE `Authentification` (
  `ID` int(40) NOT NULL,
  `Username` text NOT NULL,
  `Password` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `Authentification`
--

INSERT INTO `Authentification` (`ID`, `Username`, `Password`) VALUES
(25909, '606b52a8ed3b0be73b1b94dfbcac32a3103f77e8', '8cb2237d0679ca88db6464eac60da96345513964'),
(25910, '66c1ea1d03ad39c3f503ab4eb78974ebfac16617', '8cb2237d0679ca88db6464eac60da96345513964');

-- --------------------------------------------------------

--
-- Structure de la table `Inventory`
--

CREATE TABLE `Inventory` (
  `Username` text NOT NULL,
  `health` int(11) NOT NULL DEFAULT 3,
  `defense` int(11) NOT NULL DEFAULT 2,
  `speed` int(11) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `Inventory`
--

INSERT INTO `Inventory` (`Username`, `health`, `defense`, `speed`) VALUES
('606b52a8ed3b0be73b1b94dfbcac32a3103f77e8', 3, 2, 1),
('66c1ea1d03ad39c3f503ab4eb78974ebfac16617', 3, 120, 1);

--
-- Index pour les tables déchargées
--

--
-- Index pour la table `Authentification`
--
ALTER TABLE `Authentification`
  ADD UNIQUE KEY `ID` (`ID`),
  ADD UNIQUE KEY `Username` (`Username`) USING HASH,
  ADD UNIQUE KEY `Username_2` (`Username`) USING HASH,
  ADD UNIQUE KEY `Username_3` (`Username`) USING HASH;

--
-- Index pour la table `Inventory`
--
ALTER TABLE `Inventory`
  ADD UNIQUE KEY `Username` (`Username`) USING HASH;

--
-- AUTO_INCREMENT pour les tables déchargées
--

--
-- AUTO_INCREMENT pour la table `Authentification`
--
ALTER TABLE `Authentification`
  MODIFY `ID` int(40) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25911;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
