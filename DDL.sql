CREATE DATABASE IF NOT EXISTS PizzeriaDB;
USE PizzeriaDB;

CREATE TABLE Pizzeria (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    direccion VARCHAR(150) NOT NULL,
    telefono VARCHAR(20) NULL,
    horario_atencion VARCHAR(100) NULL
);

CREATE TABLE Cliente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    telefono VARCHAR(20) NULL,
    direccion VARCHAR(150) NOT NULL
);

CREATE TABLE Cocina (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    disponible TINYINT(1) NOT NULL DEFAULT 1,
    pedidos_en_preparacion INT NOT NULL DEFAULT 0
);

CREATE TABLE Pizza (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(255) NULL,
    precio DECIMAL(8, 2) NOT NULL
);

CREATE TABLE Pedido (
    id INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id INT NOT NULL,
    estado ENUM('EsperaDeConfirmacion', 'EnPreparacion', 'EnViaje', 'Entregado')
        NOT NULL DEFAULT 'EsperaDeConfirmacion',
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_pedido_cliente FOREIGN KEY (cliente_id) REFERENCES Cliente (id)
);

CREATE TABLE PedidoPizza (
    pedido_id INT NOT NULL,
    pizza_id INT NOT NULL,
    cantidad INT NOT NULL DEFAULT 1,
    PRIMARY KEY (pedido_id, pizza_id),
    CONSTRAINT fk_pizza_pedido FOREIGN KEY (pedido_id) REFERENCES Pedido (id) ON DELETE CASCADE,
    CONSTRAINT fk_pizza_producto FOREIGN KEY (pizza_id) REFERENCES Pizza (id)
);