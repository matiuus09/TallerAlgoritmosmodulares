# Taller Algoritmos Modulares (C#)

## Integrantes

| Nombre | Código |
|--------|--------|
| _Integrante 1_ | _código_ |
| _Integrante 2_ | _código_ |
| _Integrante 3_ | _código_ |

## Casos desarrollados

1. **Sistema de control de turnos para una IPS** (caso 1)
2. **Sistema de control de parqueadero para conjunto residencial** (caso 4)
3. **Sistema de control de producción para una fábrica de empaques** (caso 5)

## Cómo ejecutarlo

Se necesita el SDK de .NET (8 o superior; el proyecto está en `net10.0`; si tienen otra versión, cambien `TargetFramework` en el `.csproj`).

```bash
cd TallerAlgoritmosModulares
dotnet run
```

## Organización del programa

| Archivo | Contenido |
|---------|-----------|
| `Program.cs` | `Main`: solo orquesta. Muestra el menú principal, lee la opción y llama al sistema elegido. Al terminar vuelve a mostrar el menú hasta que el usuario elige **0. Salir**. |
| `Utilidades.cs` | Funciones compartidas: menú genérico, lectura con validación (`LeerTexto`, `LeerEntero`, `LeerNombre`, `LeerHora`), búsqueda y conteo en vectores, formato de horas y duraciones. |
| `SistemaIPS.cs` | Caso 1 – turnos de la IPS. |
| `SistemaParqueadero.cs` | Caso 4 – parqueadero de visitantes. |
| `SistemaProduccion.cs` | Caso 5 – producción semanal de la fábrica. |

| `Modelos.cs` | Clases `Paciente`, `Vehiculo` y `Operario` (POO, puntos extra). |

La solución es **estructurada**: cada sistema es un módulo (`static class`) con **vectores paralelos**, una **matriz** y **funciones/procedimientos** estáticos. La posición `i` de todos los vectores corresponde al mismo registro.

### POO (puntos extra)

Se agregó un uso puntual de POO sin cambiar la forma de guardar los datos (siguen en vectores y matrices). En algunas funciones se crea un objeto a partir de una posición de los vectores para mostrar su detalle o calcular algo sobre ese registro:

| Clase | Métodos | Dónde se usa |
|---|---|---|
| `Paciente` | `EsPoblacionPreferencial()`, `MostrarDetalle()` | Buscar paciente por documento y mostrar el de mayor prioridad (`ObtenerPaciente(i)`). |
| `Vehiculo` | `SigueDentro()`, `CalcularPermanencia(horaActual)`, `MostrarDetalle()` | Calcular tiempo de permanencia y mostrar el de mayor permanencia (`ObtenerVehiculo(i)`). |
| `Operario` | `ProduccionSemanal()`, `PromedioDiario()`, `MejorDia()`, `MostrarResumen()` | Mostrar el mejor operario de la semana (`ObtenerOperario(i)` copia su fila de la matriz). |

Conceptos aplicados: clase, constructor (primario), datos privados del objeto (encapsulamiento), métodos y creación de objetos con `new`.

- **Funciones**: devuelven un valor (ej. `ContarPorTipo`, `CalcularPermanencia`, `CalcularProduccionSemanal`, `BuscarIndicePorDocumento`).
- **Procedimientos** (`void`): realizan una acción sin devolver valor (ej. `RegistrarPaciente`, `MostrarListadoGeneral`, `RegistrarSalida`).
- Cada sistema tiene su propio submenú con un ciclo `do-while` y la opción **0. Volver al menú principal**. Los datos se conservan mientras el programa esté abierto, así que se puede salir y volver a entrar a un sistema sin perder lo registrado.

---

## 1. Sistema de control de turnos para una IPS

### Análisis del problema

| | |
|---|---|
| **Entradas** | Documento (6–10 dígitos), nombre completo, edad (0–120), tipo de atención (1 Urgencias, 2 Consulta General, 3 Prioritaria), nivel de prioridad (1–5). |
| **Procesos** | Validar datos y cupos, guardar en vectores, buscar por documento (búsqueda secuencial), contar por tipo, buscar el mínimo de prioridad. |
| **Salidas** | Listado general, datos del paciente buscado, conteo por tipo, paciente con mayor prioridad, cupos disponibles. |

**Estructuras de datos:** vectores paralelos de 30 posiciones `documentos[]`, `nombres[]`, `edades[]`, `tipos[]`, `prioridades[]` y un contador `cantidad`.

**Validación de cupos:** el día tiene 30 cupos, repartidos por tipo de atención: Urgencias 10, Consulta General 12, Prioritaria 8. Antes de registrar se verifica que quede cupo total y cupo en el tipo elegido; si no hay, el paciente no se registra.

**Prioridad:** se usa la escala de triage, donde **1 es la prioridad más alta** (Crítico) y **5 la más baja** (No urgente). Si dos pacientes tienen la misma prioridad, se atiende primero al que llegó antes (menor turno).

**Otras validaciones:** documento solo numérico y sin repetir, nombre solo con letras, edad y prioridad dentro del rango.

### Prueba de escritorio

Datos de entrada:

| Turno | Documento | Nombre | Edad | Tipo | Prioridad |
|---|---|---|---|---|---|
| 1 | 1020304050 | Ana Gomez | 34 | 1 (Urgencias) | 3 |
| 2 | 52123456 | Luis Perez | 70 | 2 (Consulta General) | 2 |
| 3 | 1098765432 | Maria Ruiz | 8 | 3 (Prioritaria) | 2 |

Validaciones probadas en el registro:

| Dato ingresado | Resultado |
|---|---|
| Documento `12ab` | Error: el documento debe tener de 6 a 10 dígitos y no estar registrado hoy |
| Edad `150` | Error: ingrese un número entero entre 0 y 120 |
| Documento `1020304050` (otra vez) | Error: el documento debe tener de 6 a 10 dígitos y no estar registrado hoy |
| Nombre `Luis 9` | Error: solo letras y espacios |

Seguimiento de `ObtenerIndiceMayorPrioridad()`:

| i | prioridades[i] | prioridades[indiceMayor] | ¿prioridades[i] < mayor? | indiceMayor |
|---|---|---|---|---|
| inicio | – | – | – | 0 (Ana, 3) |
| 1 | 2 | 3 | Sí | 1 (Luis, 2) |
| 2 | 2 | 2 | No (empate, se queda el que llegó primero) | 1 |

**Resultado:** paciente con mayor prioridad = **Luis Perez (turno 2, prioridad 2)**.

Seguimiento de `ContarPorTipo` y `CuposDisponibles`:

| Tipo | Pacientes | Cupos máximos | Cupos libres |
|---|---|---|---|
| Urgencias | 1 | 10 | 9 |
| Consulta General | 1 | 12 | 11 |
| Prioritaria | 1 | 8 | 7 |
| **Total del día** | **3** | **30** | **27** |

Búsqueda del documento `52123456`: i=0 (`1020304050` ≠), i=1 (`52123456` =) → se muestra el turno #2, Luis Perez.

---

## 2. Sistema de control de parqueadero (conjunto residencial)

### Análisis del problema

| | |
|---|---|
| **Entradas** | Placa (3–7 letras/números), torre (1–10), apartamento (101–2504), hora de ingreso `HH:MM`, tipo de vehículo (1 Carro, 2 Moto, 3 Bicicleta), hora de salida `HH:MM`. |
| **Procesos** | Convertir horas a minutos, validar que la placa no esté dentro, registrar salida, restar salida − ingreso, contar por tipo, buscar el máximo de permanencia. |
| **Salidas** | Confirmación de ingreso/salida, tiempo de permanencia, ingresos por tipo, vehículo con mayor permanencia, listado del día. |

**Estructuras de datos:** vectores paralelos de 40 posiciones `placas[]`, `torres[]`, `apartamentos[]`, `horasIngreso[]`, `horasSalida[]`, `tipos[]` y el contador `cantidad`.

Las horas se guardan en **minutos desde las 00:00** (ej. `08:15` → 8·60 + 15 = 495), así la permanencia es una simple resta. Mientras el vehículo está dentro, `horasSalida[i] = -1`.

**Validaciones:**
- Una misma placa **no puede ingresar dos veces sin haber salido** (`BuscarIngresoActivo`).
- La hora de salida no puede ser anterior a la de ingreso.
- Si una placa vuelve a entrar el mismo día, su nueva entrada debe ser después de su última salida.
- Máximo 40 ingresos por día; formato de hora `HH:MM` válido (00:00 a 23:59).
- La placa se pasa a mayúsculas y se le quitan guiones y espacios (`xyz-45d` → `XYZ45D`).

### Prueba de escritorio

| Paso | Acción | Datos | Resultado |
|---|---|---|---|
| 1 | Ingreso | ABC123, Torre 3, Apto 502, 08:15, Carro | Registrado. horasIngreso = 495, horasSalida = −1 |
| 2 | Ingreso | xyz-45d, Torre 5, Apto 1203, 09:00, Moto | Registrado como **XYZ45D**. horasIngreso = 540 |
| 3 | Ingreso | ABC123 | **Rechazado**: ya está dentro y no ha salido |
| 4 | Salida | ABC123, 07:00 | Error: use el formato HH:MM y una hora desde las 08:15 (no puede ser antes del ingreso) |
| 5 | Salida | ABC123, 11:45 | horasSalida = 705 → 705 − 495 = 210 min = **3h 30m** |
| 6 | Ingreso | BICI01, Torre 2, Apto 101, 10:30, Bicicleta | Registrado. horasIngreso = 630 |
| 7 | Salida | XYZ45D, 10:10 | 610 − 540 = 70 min = **1h 10m** |
| 8 | Permanencia | BICI01, hora actual 12:00 | Sigue dentro: 720 − 630 = 90 min = **1h 30m** |

Seguimiento de `ObtenerIndiceMayorPermanencia()`:

| i | Placa | horasSalida | Permanencia | indiceMayor |
|---|---|---|---|---|
| 0 | ABC123 | 705 | 210 | 0 (primer vehículo con salida) |
| 1 | XYZ45D | 610 | 70 | 0 (70 < 210) |
| 2 | BICI01 | −1 | – (sigue dentro, se omite) | 0 |

**Resultado:** mayor permanencia = **ABC123, 3h 30m**.

Ingresos por tipo: Carro 1, Moto 1, Bicicleta 1, Total 3.

---

## 3. Sistema de control de producción (fábrica de empaques)

### Análisis del problema

| | |
|---|---|
| **Entradas** | Código de operario (2–10 letras/números, único), nombre, producción de lunes a viernes (enteros ≥ 0), meta mínima diaria (opcional). |
| **Procesos** | Llenar la matriz, sumar filas (producción semanal), sumar columnas (total por día), sumar toda la matriz, buscar el máximo, comparar contra el umbral. |
| **Salidas** | Producción semanal por operario, mejor operario, promedio general, días/registros de baja producción, matriz completa. |

**Estructuras de datos:**
- Vectores `codigos[10]`, `nombres[10]`, `tieneProduccion[10]`.
- **Matriz** `produccion[10, 5]`: filas = operarios, columnas = días (lunes a viernes).

**Validaciones:** ningún valor de producción puede ser negativo (se vuelve a pedir el dato), el valor debe ser entero, el código no se puede repetir, máximo 10 operarios. Los cálculos solo incluyen a los operarios que ya tienen la producción ingresada.

**Baja producción:** el usuario indica una meta mínima diaria (o `0` para usar el promedio diario por operario). Se listan todas las celdas de la matriz por debajo de esa meta y, además, se marcan los días en que el total de la planta quedó por debajo del promedio de los días.

### Prueba de escritorio

Matriz de producción ingresada (en OP01 se intentó ingresar `-5` el martes y el sistema lo rechazó: *"la producción no puede ser negativa"*; luego se ingresó 100):

| Código | Nombre | Lun | Mar | Mié | Jue | Vie | **Semana (suma fila)** |
|---|---|---|---|---|---|---|---|
| OP01 | Carlos Diaz | 120 | 100 | 130 | 110 | 125 | **585** |
| OP02 | Laura Mena | 90 | 95 | 80 | 100 | 85 | **450** |
| OP03 | Pedro Rios | 150 | 140 | 60 | 135 | 145 | **630** |
| | **Total día (suma columna)** | 360 | 335 | 270 | 345 | 355 | **1665** |

Seguimiento de `ObtenerMayorProduccionSemanal()`:

| i | Semana | mayor |
|---|---|---|
| inicio | – | −1 |
| 0 | 585 | 585 |
| 1 | 450 | 585 |
| 2 | 630 | 630 |

**Mejor operario:** OP03 – Pedro Rios, 630 unidades.

Promedios:
- Promedio semanal por operario = 1665 / 3 = **555,00**
- Promedio diario por operario = 1665 / (3 × 5) = **111,00**

Baja producción con meta = 0 (umbral = promedio 111):

| Día | Operario | Unidades |
|---|---|---|
| Martes | OP01 | 100 |
| Jueves | OP01 | 110 |
| Lunes a Viernes | OP02 | 90, 95, 80, 100, 85 |
| Miércoles | OP03 | 60 |

Totales por día contra el promedio de los días (1665 / 5 = 333): solo el **miércoles (270)** queda por debajo → día de baja producción.
