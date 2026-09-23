using PizzeriaMVC.Models;

namespace PizzeriaMVC.Data;

public static class MenuMock
{
    public static readonly List<PizzaMenuItem> Pizzas = new()
    {
        new() { IdPizza = 1, Nombre = "Muzzarella", Descripcion = "La clásica: muzzarella fundida y salsa de tomate.", Precio = 4500, Emoji = "🍕" },
        new() { IdPizza = 2, Nombre = "Napolitana", Descripcion = "Rebanadas de tomate, ajo y queso gratinado.", Precio = 5500, Emoji = "🍅" },
        new() { IdPizza = 3, Nombre = "Fugazzeta", Descripcion = "Mucha cebolla dulce y muzzarella por debajo.", Precio = 5200, Emoji = "🧅" },
        new() { IdPizza = 4, Nombre = "Cuatro Quesos", Descripcion = "Mozzarella, roquefort, parmesano y fontina.", Precio = 6500, Emoji = "🧀" },
        new() { IdPizza = 5, Nombre = "Rúcula y Jamón", Descripcion = "Jamón crudo, rúcula fresca y bocconcini.", Precio = 7000, Emoji = "🥗" },
        new() { IdPizza = 6, Nombre = "Prosciutto", Descripcion = "Jamón cocido sobre base de muzzarella.", Precio = 5900, Emoji = "🍖" }
    };
}