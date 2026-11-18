sequenceDiagram
    autonumber
    actor Cliente
    participant API as API REST (.NET 8)
    participant BD as Base de Datos
    Cliente->>+API: POST /pedidos (Items, Datos, Comentario)
    API->>+BD: Consultar pizzas en catálogo
    BD-->>-API: Pizzas válidas y precios (OK)
    API->>+BD: INSERT Pedido (Estado: Espera de confirmación)
    BD-->>-API: Pedido guardado (ID generado)
    API-->>-Cliente: 201 Created (JSON con detalle y Estado: Espera de confirmación)
