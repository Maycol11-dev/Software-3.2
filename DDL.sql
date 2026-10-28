
CREATE DATABASE IF NOT EXISTS PizzeriaDB;
USE PizzeriaDB;

CREATE TABLE Rol (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    correo VARCHAR(150) NOT NULL UNIQUE,
    contrasena VARCHAR(255) NOT NULL,
    rol_id INT NOT NULL,
    fecha_registro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_usuario_rol FOREIGN KEY (rol_id) REFERENCES Rol (id)
);

CREATE TABLE Cliente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL UNIQUE,
    nombre VARCHAR(100) NOT NULL,
    telefono VARCHAR(20) NULL,
    direccion VARCHAR(150) NOT NULL,
    CONSTRAINT fk_cliente_usuario FOREIGN KEY (usuario_id) REFERENCES Usuario (id)
);

CREATE TABLE Pizza (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(255) NULL,
    precio DECIMAL(8, 2) NOT NULL,
    imagen_url VARCHAR(255) NULL
);

CREATE TABLE Pedido (
    id INT AUTO_INCREMENT PRIMARY KEY,
    cliente_id INT NOT NULL,
    estado ENUM('EsperaDeConfirmacion', 'EnPreparacion', 'EnViaje', 'Entregado')
        NOT NULL DEFAULT 'EsperaDeConfirmacion',
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    total DECIMAL(10, 2) NOT NULL DEFAULT 0,
    CONSTRAINT fk_pedido_cliente FOREIGN KEY (cliente_id) REFERENCES Cliente (id)
);

CREATE TABLE PedidoPizza (
    pedido_id INT NOT NULL,
    pizza_id INT NOT NULL,
    cantidad INT NOT NULL DEFAULT 1,
    precio_unitario DECIMAL(8, 2) NOT NULL DEFAULT 0,
    subtotal DECIMAL(8, 2) NOT NULL DEFAULT 0,
    PRIMARY KEY (pedido_id, pizza_id),
    CONSTRAINT fk_pizza_pedido FOREIGN KEY (pedido_id) REFERENCES Pedido (id) ON DELETE CASCADE,
    CONSTRAINT fk_pizza_producto FOREIGN KEY (pizza_id) REFERENCES Pizza (id)
);
