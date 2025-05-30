-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 30-05-2025 a las 19:46:38
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `basedatos`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `idiomas`
--

CREATE TABLE `idiomas` (
  `id` varchar(50) NOT NULL,
  `ESP` text DEFAULT NULL,
  `EN` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `idiomas`
--

INSERT INTO `idiomas` (`id`, `ESP`, `EN`) VALUES
('main_menu_button_exit', 'Salir', 'Exit'),
('main_menu_button_start', 'Empezar', 'Start');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `idiomos`
--

CREATE TABLE `idiomos` (
  `id` varchar(50) NOT NULL,
  `ESP` text DEFAULT NULL,
  `EN` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `idiomos`
--

INSERT INTO `idiomos` (`id`, `ESP`, `EN`) VALUES
('main_manu_button_exit', 'Salir', 'Exit'),
('main_menu_button_start', 'Empezar', 'Start');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `logros`
--

CREATE TABLE `logros` (
  `id` int(11) NOT NULL,
  `descripcion` varchar(255) NOT NULL,
  `puntos_requeridos` int(11) NOT NULL,
  `completado` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `logros`
--

INSERT INTO `logros` (`id`, `descripcion`, `puntos_requeridos`, `completado`) VALUES
(1, 'Consigue 100 puntos', 100, 1),
(2, 'Consigue 200 puntos', 200, 0),
(3, 'Consigue 300 puntos', 300, 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `logros_completados`
--

CREATE TABLE `logros_completados` (
  `usuario_email` varchar(255) DEFAULT NULL,
  `logro_id` int(11) DEFAULT NULL,
  `completado` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `logros_completados`
--

INSERT INTO `logros_completados` (`usuario_email`, `logro_id`, `completado`) VALUES
('tusilinix@gmail.com', 1, 1),
('prueba@gmail.com', 1, 1),
('prueba@gmail.com', 2, 1),
('prueba@gmail.com', 3, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `puntos_jugadores`
--

CREATE TABLE `puntos_jugadores` (
  `email` varchar(255) NOT NULL,
  `max_points` int(11) NOT NULL,
  `username` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `puntos_jugadores`
--

INSERT INTO `puntos_jugadores` (`email`, `max_points`, `username`) VALUES
('miguel4@gmail.com', 17, NULL),
('miguel@gmail.com', 0, NULL),
('panoramix@gmail.com', 0, NULL),
('patato@gmail.com', 0, NULL),
('prueba', 11, NULL),
('prueba@gmail.com', 0, NULL),
('tusilini1@gmail.com', 20, NULL),
('tusilinix', 103, NULL),
('tusilinix@gmail.com', 14, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `id` int(11) NOT NULL,
  `email` varchar(100) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `last_login` datetime DEFAULT NULL,
  `volume` float DEFAULT 0.5
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`id`, `email`, `username`, `password`, `last_login`, `volume`) VALUES
(1, 'apilanezmiguel@gmail.com', 'apilanezmiguel', 'MiguelSQL', NULL, 0.5),
(5, 'miguel2@gmail.com', 'miguel2', '1234', NULL, 0.5),
(12, 'miguel3@gmail.com', 'miguel3', 'SQL', NULL, 0.5),
(13, 'miguel4@gmail.com', 'miguel4', 'Miguel4', NULL, 0.5),
(14, 'tusilini1@gmail.com', 'tusilini1', 'tusilini1', NULL, 0.5),
(19, 'tusilinix@gmail.com', 'tusilinix', 'tusilini10', '2025-05-27 17:58:11', 0),
(20, 'prueba@gmail.com', 'prueba', 'prueba1', NULL, 0.5);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario_settings`
--

CREATE TABLE `usuario_settings` (
  `id` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `volume` float DEFAULT 0.5
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario_settings`
--

INSERT INTO `usuario_settings` (`id`, `email`, `volume`) VALUES
(1, 'tusilinix@gmail.com', 0.0498093);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `idiomos`
--
ALTER TABLE `idiomos`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `logros`
--
ALTER TABLE `logros`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `logros_completados`
--
ALTER TABLE `logros_completados`
  ADD KEY `logro_id` (`logro_id`),
  ADD KEY `usuario_email` (`usuario_email`);

--
-- Indices de la tabla `puntos_jugadores`
--
ALTER TABLE `puntos_jugadores`
  ADD PRIMARY KEY (`email`),
  ADD UNIQUE KEY `email` (`email`) USING BTREE,
  ADD UNIQUE KEY `username` (`username`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- Indices de la tabla `usuario_settings`
--
ALTER TABLE `usuario_settings`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `logros`
--
ALTER TABLE `logros`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT de la tabla `usuario_settings`
--
ALTER TABLE `usuario_settings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=480;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `logros_completados`
--
ALTER TABLE `logros_completados`
  ADD CONSTRAINT `logros_completados_ibfk_1` FOREIGN KEY (`logro_id`) REFERENCES `logros` (`id`),
  ADD CONSTRAINT `logros_completados_ibfk_2` FOREIGN KEY (`usuario_email`) REFERENCES `usuarios` (`email`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
