using PizzeriaMVC.Models;

namespace PizzeriaMVC.Data;

public static class MenuMock
{
    public static readonly List<PizzaMenuItem> Pizzas = new()
    {
        new() { IdPizza = 1, Nombre = "Muzzarella", Descripcion = "La clásica: muzzarella fundida y salsa de tomate.", Precio = 4500, ImagenUrl = "/images/pizzas/muzzarella.jpg" },
        new() { IdPizza = 2, Nombre = "Napolitana", Descripcion = "Tomate en rebanadas, ajo y queso gratinado.", Precio = 5500, ImagenUrl = "/images/pizzas/napolitana.png" },
        new() { IdPizza = 3, Nombre = "Fugazzeta", Descripcion = "Mucha cebolla dulce y muzzarella por debajo.", Precio = 5200, ImagenUrl = "/images/pizzas/fugazzeta.jpg" },
        new() { IdPizza = 4, Nombre = "Cuatro Quesos", Descripcion = "Mozzarella, roquefort, parmesano y fontina.", Precio = 6500, ImagenUrl = "/images/pizzas/cuatro-quesos.png" },
        new() { IdPizza = 5, Nombre = "Rúcula y Jamón", Descripcion = "Jamón crudo, rúcula fresca y bocconcini.", Precio = 7000, ImagenUrl = "/images/pizzas/rucula-y-jamon.jpg" },
        new() { IdPizza = 6, Nombre = "Prosciutto", Descripcion = "Jamón cocido sobre base de muzzarella.", Precio = 5900, ImagenUrl = $"/images/pizzas/prosciutto.jpg" }
    };
}
