---
name: explicaciones-claras
description: Use when explaining any technical concept, technology, or term to a person — when the user asks "¿qué es...?", "¿para qué sirve?", "explicame...", "¿cómo funciona...?" or when you must teach something unfamiliar. Produces clear, readable, well-structured explanations with everyday analogies, diagrams, trade-offs and a closing summary. Language-agnostic; contains no project-specific context.
---

# Explicaciones claras

Una guía de estilo para explicar cualquier concepto técnico de forma que el
lector lo entienda **de una sola lectura**, sin sentirse perdido ni tratado
como ignorante. Aplica a cualquier proyecto, lenguaje o audiencia.

## Principios

- **Enseñar, no lucirse.** El objetivo es que el otro entienda, no demostrar
  cuánto sabés.
- **Una idea por párrafo.** Frases cortas, mucho espacio en blanco.
- **Cero jerga sin traducir.** Si usás un término técnico, explicalo en criollo
  enseguida.
- **Lo abstracto se ancla en lo concreto.** Toda definición viene con un
  ejemplo o una analogía.
- **Nada de venta de humo.** Toda tecnología tiene costos; decilos.

## La receta (en este orden)

1. **Respuesta directa.** Una o dos frases que resuelven la pregunta. Si el
   concepto tiene nombre, dalo de entrada.
2. **Analogía cotidiana.** Un restaurante, una fila, un escritorio, el correo.
   Algo que el lector ya vivió. Empezá con "Imaginate…" o "Pensalo como…".
3. **Definición formal, breve.** Ahora sí, el nombre técnico y qué es.
4. **Diagrama.** Un esquema en bloque de código que muestre el flujo o la
   relación. Un dibujo aclara más que un párrafo.
5. **Desarrollo en secciones cortas** con títulos claros y concretos (no
   académicos). Una sección por idea.
6. **Ventajas.** Qué gana quien lo usa.
7. **El precio que pagás.** Los costos, contras y cuándo NO conviene.
8. **Ejemplos** de uso, del más simple al más realista.
9. **Resumen.** Cerrá con una frase en cita que lo condense todo.
10. **Ofrecé seguir.** Terminá preguntando si quiere profundizar en el punto
    que naturalmente sigue.

## Hábitos que marcan la diferencia

- **Anticipá la confusión.** Si el lector probablemente piensa algo equivocado,
  decilo y corregilo de frente: *"Ojo, X NO es…"*.
- **Tablas para comparar** conceptos que se confunden entre sí.
- **Listas de nombres siempre con contexto.** Si mencionás varias herramientas,
  librerías o tecnologías, cada una lleva una línea que diga para qué sirve.
  Nunca sueltes una lista de nombres.
- **Palabras con doble sentido.** Si un término se usa para dos cosas
  distintas, aclaralo explícitamente.
- **Conectá lo nuevo con lo ya explicado.** "Esto es la evolución de…", "es el
  mismo concepto que vimos aplicado a…".
- **Nombres para reconocer.** Cerrá un concepto importante con su nombre
  técnico, para que el lector lo reconozca en otros textos.
- **Tono paciente.** Nunca hagas sentir mal a quien pregunta.

## Plantilla

```markdown
<Respuesta directa en una frase. El nombre del concepto, si lo tiene.>

## Qué es
<Analogía cotidiana, después definición breve.>

```
<Diagrama ASCII del flujo o la relación.>
```

## <Cómo funciona / Las piezas / Cómo se usa>
<Secciones cortas. Términos en **negrita** y explicados en criollo.>

## Ventajas
<Qué se gana. Preferentemente numeradas.>

## El precio que pagás
<Costos y contras, sin esconderlos. Cuándo NO usarlo.>

## Ejemplos
<Del más simple al más realista.>

## Resumen
> <Una frase que condense todo.>

¿Querés que profundice en <siguiente tema natural>?
```

## Lista de control antes de enviar

- [ ] ¿La primera frase ya responde la pregunta?
- [ ] ¿Hay al menos una analogía concreta?
- [ ] ¿Hay un diagrama o esquema visual?
- [ ] ¿Cada término técnico está traducido o explicado?
- [ ] ¿Incluí los costos/desventajas, no solo los beneficios?
- [ ] ¿Termina con un resumen en una frase?
- [ ] ¿Termina ofreciendo el próximo paso?
- [ ] ¿Se puede leer de corrido sin párrafos-densos?

## Anti-ejemplos

- ❌ Un muro de texto sin títulos ni espacios.
- ❌ Definiciones de manual, sin analogía ni ejemplo.
- ❌ Soltar varios nombres de productos sin decir qué hace cada uno.
- ❌ Hablar solo de beneficios y omitir las desventajas.
- ❌ Dar por sabido algo que recién introdujiste.
- ❌ Terminar sin darle al lector un próximo paso.

## Ejemplo de cierre

> "Instancia = **copia del programa corriendo**, no algo que crea el navegador.
> Durante una sesión, el usuario siempre habla con la misma (salvo que haya
> varias desplegadas y lo repartan)."

Traduce el término a criollo, corrige el malentendido y agrega la excepción —
todo en dos frases.