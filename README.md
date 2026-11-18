# Pizzería - Software 3.2

<p align="center">
  <img src="https://et12.edu.ar/imgs/et12.gif">
</p>


Sistema de gestión de pizzería desarrollado para la materia **Software 3.2** de la E.T N°12.

## Arquitectura

El proyecto está dividido en dos componentes principales:

- **PizzeriaBackend**: API REST en .NET 8 (Clean Architecture: Models, Data, Business, API, Tests). Autenticación JWT y hash PBKDF2. Los estados del pedido los gestiona el Administrador directamente desde la API.
- **PizzeriaMVC**: Aplicación ASP.NET Core MVC como cliente web (menú, carrito, autenticación, seguimiento de pedidos).

## Base de datos

MySQL. Scripts: `DDL.sql` y `INSERTS.sql`. Credenciales por defecto `adminPizzeria/1234` (configuradas en `PizzeriaBackend/src/API/appsettings.json`).

## Cómo ejecutar

### 1. Base de datos
Ejecutar `DDL.sql` y luego `INSERTS.sql` en MySQL.

### 2. Backend (API)
```bash
cd PizzeriaBackend/src/API
dotnet run
```
La API queda disponible en `http://localhost:5184`. Swagger en `/swagger` y Scalar en `/scalar`.

### 3. MVC (Frontend web)
```bash
cd PizzeriaMVC
dotnet run
```
Disponible en `http://localhost:5032`.

## Tests

```bash
cd PizzeriaBackend/src
dotnet test Pizzeria.sln
```

## Documentación

En la carpeta `Documentacion/` se encuentran explicaciones, diagramas (`*.canvas`) y material didáctico sobre sistemas distribuidos.
