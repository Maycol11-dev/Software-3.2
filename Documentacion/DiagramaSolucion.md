flowchart LR
    subgraph Capa_Presentacion ["1. Capa de Presentación"]
        Cliente["App / Web Cliente\n(Pedir y consultar estado)"]
        Cocina["Panel Cocina / Admin\n(Gestionar pedidos y menú)"]
        Scalar["Scalar\n(Documentación y pruebas)"]
    end

    subgraph Capa_Negocio ["2. Capa de Lógica de Negocio (C# .NET 8)"]
        API["Minimal API (REST / JSON)\n• GET /pizzas\n• POST y GET /pedidos\n• PATCH /pedidos/{id}/estado"]
        Logica["Lógica de Negocio\n(Validación de stock, estados y cálculo total)"]
        Dapper["Dapper\n(Micro ORM)"]

        API --> Logica
        Logica <--> Dapper
    end

    subgraph Capa_Persistencia ["3. Capa de Persistencia"]
        DB[("MySQL Server\n(Tablas: Clientes, Pizzas, Pedidos)")]
    end

    Cliente -- "HTTP / JSON" --> API
    Cocina -- "HTTP / JSON" --> API
    Scalar -. "Pruebas" .-> API
    Dapper -- "Consultas SQL" --> DB
