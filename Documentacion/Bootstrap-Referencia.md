# Referencia Bootstrap — Pizzeria MVC

Bootstrap 5 se incluye desde `wwwroot/lib/bootstrap/`:
- `bootstrap.min.css` (estilos) — linkeado en `_Layout.cshtml`.
- `bootstrap.bundle.min.js` (comportamiento: menú hamburguesa, alertas, modales) — al final de `_Layout.cshtml`.

No abrimos esos archivos: los usamos escribiendo **clases** en el HTML. Bootstrap ya "sabe" cómo pintarlas.

---

## 1. Cómo se lee una clase

Bootstrap combina tres tipos:

```
componente-base    modificador       utilidad
    btn        +   btn-pizza / btn-sm +   w-100
    card       +   card-pizza        +   h-100
```

- **Componentes** (`btn`, `card`, `navbar`, `form-control`): dan la estructura base.
- **Modificadores** (`btn-pizza`, `btn-lg`, `btn-outline-danger`): cambian apariencia.
- **Utilities** (`d-flex`, `mt-3`, `text-muted`): ajustes puntuales.

Varios pueden convivir en el mismo elemento:

```html
<button class="btn btn-pizza btn-lg w-100 mt-3">Confirmar</button>
```

> En nuestro proyecto hay además clases propias (`btn-pizza`, `card-pizza`, `seccion-datos`). Regla para distinguirlas: con prefijo propio (`pizza-`, `cart-`, `step-`, `titulo-`) son nuestras; sin prefijo, de Bootstrap.

---

## 2. La grilla (container / row / col)

Organiza el contenido en columnas responsivas.

```html
<div class="container">        <!-- centra y limita el ancho -->
  <div class="row">            <!-- crea una fila con columnas internas -->
    <div class="col-lg-8">...</div>   <!-- ocupa 8/12 en pantallas grandes -->
    <div class="col-lg-4">...</div>   <!-- ocupa 4/12 y se apila abajo en chicas -->
  </div>
</div>
```

**Breakpoints (anchos de pantalla):**

| Prefijo | Pantalla | Se aplica desde |
|---|---|---|
| `col-` | celular | siempre (móvil) |
| `col-sm-` | tablets chicas | ≥ 576px |
| `col-md-` | tablets | ≥ 768px |
| `col-lg-` | escritorio | ≥ 992px |
| `col-xl-` | escritorio grande | ≥ 1200px |

`col-6` = la mitad del ancho (6 de 12), `col-12` = ancho completo. En el menú usamos `col-md-6` para que cada pizza ocupe media fila desde tablet, y `col-lg-8`/`col-lg-4` para menú + carrito.

---

## 3. Flexbox (d-flex y compañía)

`d-flex` hace que los hijos se acomoden en una fila flexible.

```html
<div class="d-flex justify-content-between align-items-center mt-auto">
    <span>Precio</span>
    <button>Agregar</button>
</div>
```

| Utilidad | Efecto |
|---|---|
| `d-flex` | activa flexbox en el elemento |
| `justify-content-start/center/end/between` | alinea en horizontal (between = extremos con espacio en el medio) |
| `align-items-center/start/end` | alinea en vertical |
| `flex-grow-1` | el hijo ocupa todo el espacio libre (navbar links) |
| `gap-2`, `gap-3` | espacio entre elementos hijos |
| `m-*` + `auto` → `ms-auto`, `mx-auto` | margen automático (empuja el elemento) |

---

## 4. Márgenes y rellenos (m / p)

Formato: `{m|p}{lado}{tamaño}`

- `m` = margin, `p` = padding.
- Lados: `t` (arriba/top), `b` (abajo/bottom), `s` (izquierda/start), `e` (derecha/end), `x` (horizontal), `y` (vertical), sin letra = todo.
- Tamaños: `0` (nada), `1` (poco), `2`, `3`, `4`, `5` (mucho). También `auto`.

Ejemplos que usamos:

```html
<h2 class="mb-4">          <!-- margen inferior -->
<div class="mt-3">          <!-- margen superior -->
<button class="mt-3">       <!-- separa el botón del contenido -->
<a class="me-2">            <!-- margen derecho -->
```

---

## 5. Texto (text-*, fw-*, fs-*)

| Clase | Efecto |
|---|---|
| `text-center` | centra el texto |
| `text-start` / `text-end` | izquierda / derecha |
| `text-muted` | gris apagado (secundario) |
| `text-dark` / `text-white` | color del texto |
| `fw-bold` / `fw-semibold` | grosor (negrita / seminegrita) |
| `fs-3`, `fs-5` | tamaño (1 más grande que 5) |
| `lead` | párrafo destacado grande |

---

## 6. Botones (btn)

```html
<a class="btn btn-pizza" ...>Ver menú</a>
<button type="submit" class="btn btn-pizza-verde btn-lg">Confirmar</button>
```

| Clase | Rol |
|---|---|
| `btn` | **obligatoria**; da la forma base del botón |
| `btn-pizza` / `btn-pizza-verde` | nuestras (colores rojo/verde) |
| `btn-outline-danger` | contorno rojo (botón ✕ del carrito) |
| `btn-sm` | chico |
| `btn-lg` | grande (hero y confirmación) |
| `w-100` | ancho completo (se combina con `btn`) |

---

## 7. Navbar y el menú hamburguesa (collapse)

Este es el caso que más confunde. En `_Layout.cshtml`:

```html
<button class="navbar-toggler" type="button"
        data-bs-toggle="collapse"
        data-bs-target=".navbar-collapse"
        aria-expanded="false">
    <span class="navbar-toggler-icon"></span>
</button>

<div class="navbar-collapse collapse d-sm-inline-flex justify-content-end">
    <ul class="navbar-nav"> ... </ul>
</div>
```

Cómo funciona:

- `navbar-expand-sm`: en pantallas **≥ 768px** los links se muestran siempre en una fila.
- En pantallas **más chicas**, la `collapse` **oculta** el menú y aparece el botón `navbar-toggler` (☰).
- El `data-bs-toggle="collapse"` + `data-bs-target=".navbar-collapse"` es el **pegamento en JS**: al tocar el botón, Bootstrap busca el elemento con clase CSS `.navbar-collapse` y le marca/desmarca `collapse` para mostrarlo/ocultarlo. Así **no escribimos nada de JavaScript**.
- `collapse` y `collapse show`: la primera esconde, `show` lo fuerza visible.

> Datos importantes:
> - El `target` y el elemento a mostrar **deben coincidir** (selectores de clase o `#id`).
> - `aria-*` es accesibilidad (lectores de pantalla) — no cambia lo visual.

Con `collapse` esto funciona con cualquier elemento, no solo navbar:

```html
<button data-bs-toggle="collapse" data-bs-target="#detalle">Más info</button>
<div id="detalle" class="collapse">Contenido ocultable</div>
```

---

## 8. Formularios

```html
<div class="mb-3">
    <label class="form-label" for="nombre">Nombre</label>
    <input class="form-control" name="nombre" id="nombre" required />
</div>
```

- `form-control`: estilo base del input (bordes redondeados, padding).
- `form-label`: estilo del label (separación, fuente).
- `mb-3`: espacio entre cada campo.
- `required`: validación nativa del navegador (marca rojo si está vacío).

---

## 9. Alertas

```html
<div class="alert alert-success">Pedido recibido.</div>
```

`alert` + `alert-success` (verde) / `alert-warning` (amarillo) / `alert-danger` (rojo). Se cierran con JS, pero también pueden dejarse fijas.

---

## 10. Card

```html
<div class="card card-pizza p-3 h-100">...</div>
```

- `card` (Bootstrap): estructura base.
- `card-pizza` (nuestra): sombra y hover flotante.
- `p-3`: relleno interno.
- `h-100`: todas las cards de una fila quedan de la misma altura.

---

## 11. Cómo probar un cambio sin romper

1. **Encontrá la clase** en el `.cshtml` (Ctrl+F).
2. **Sabé el alcance**: si está en `_Layout`, afecta a todas las páginas; si en una vista, solo a esa.
3. **Añadí la clase en el navegador** (F12 → Elements) y probá antes de guardar:
   - agregá `text-center` para ver alineación,
   - cambiá `col-lg-8` por `col-lg-6` para ver la grilla,
   - meté/quita `mt-3` para el espaciado.
4. **Probar responsivo**: achicá la ventana para ver el menú hamburguesa.

---

## 12. Documentación oficial (referencia completa)

- Componentes: https://getbootstrap.com/docs/5.3/components/
- Utilities:   https://getbootstrap.com/docs/5.3/utilities/
- Grilla:      https://getbootstrap.com/docs/5.3/layout/grid/