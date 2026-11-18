
USE PizzeriaDB;

INSERT INTO Rol (nombre) VALUES
('Cliente'),
('Administrador');

INSERT INTO Pizza (nombre, descripcion, precio, imagen_url) VALUES
('Muzzarella', 'La clásica: muzzarella fundida y salsa de tomate.', 9000, '/images/pizzas/muzzarella.jpg'),
('Napolitana', 'Tomate en rebanadas, ajo y queso gratinado.', 11000, '/images/pizzas/napolitana.png'),
('Fugazzeta', 'Mucha cebolla dulce y muzzarella por debajo.', 8800, '/images/pizzas/fugazzeta.jpg'),
('Cuatro Quesos', 'Mozzarella, roquefort, parmesano y fontina.', 12000, '/images/pizzas/cuatro-quesos.png'),
('Rúcula y Jamón', 'Jamón crudo, rúcula fresca y bocconcini.', 14000, '/images/pizzas/rucula-y-jamon.jpg'),
('Prosciutto', 'Jamón cocido sobre base de muzzarella.', 13000, '/images/pizzas/prosciutto.jpg');

-- Usuario Administrador por defecto (admin@pizzabrava.com / admin123)
INSERT INTO Usuario (correo, contrasena, rol_id) VALUES
('admin@pizzabrava.com', 'PBKDF2$100000$HQYxA9RDdgPdv7GSRJloYA==$uYBs1s/oSWadNfYb+2kYw37u15TqFgw0/UAUap4iI80=', 2);

INSERT INTO Cliente (usuario_id, nombre, telefono, direccion) VALUES
(LAST_INSERT_ID(), 'Administrador Pizza Brava', '1122334455', 'Sucursal Central');
