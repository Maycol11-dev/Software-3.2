
# Diagrama Entidad-Relación - PizzeriaDB

```mermaid
erDiagram
    Rol ||--o{ Usuario : tiene
    Usuario ||--o| Cliente : pertenece
    Cliente ||--o{ Pedido : realiza
    Pedido ||--|{ PedidoPizza : contiene
    Pizza ||--o{ PedidoPizza : incluida_en

    Rol {
        INT id PK
        VARCHAR(50) nombre UK
    }

    Usuario {
        INT id PK
        VARCHAR(150) correo UK
        VARCHAR(255) contrasena
        INT rol_id FK
        TIMESTAMP fecha_registro
    }

    Cliente {
        INT id PK
        INT usuario_id FK, UK
        VARCHAR(100) nombre
        VARCHAR(20) telefono
        VARCHAR(150) direccion
    }

    Pizza {
        INT id PK
        VARCHAR(100) nombre
        VARCHAR(255) descripcion
        DECIMAL(8,2) precio
        VARCHAR(255) imagen_url
    }

    Pedido {
        INT id PK
        INT cliente_id FK
        ENUM estado
        TIMESTAMP fecha_creacion
        DECIMAL(10,2) total
    }

    PedidoPizza {
        INT pedido_id PK, FK
        INT pizza_id PK, FK
        INT cantidad
        DECIMAL(8,2) precio_unitario
        DECIMAL(8,2) subtotal
    }
```
