// CASO 5: Control de producción semanal de una fábrica de empaques.
// Vectores: codigos[i], nombres[i], tieneProduccion[i].
// Matriz produccion[i, d]: filas = operarios (máx. 10), columnas = días (lunes a viernes).
static class SistemaProduccion
{
    const int MAX = 10, DIAS = 5, MAX_DIARIO = 100000;
    static readonly string[] NOMBRES_DIAS = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };
    static readonly string[] OPCIONES =
    {
        "Registrar operario", "Ingresar producción diaria de un operario", "Calcular producción semanal por operario",
        "Identificar mejor operario de la semana", "Mostrar promedio general de producción",
        "Detectar días con baja producción", "Ver matriz de producción"
    };

    static string[] codigos = new string[MAX], nombres = new string[MAX];
    static bool[] tieneProduccion = new bool[MAX];
    static int[,] produccion = new int[MAX, DIAS];
    static int cantidad = 0;

    public static void Ejecutar()
    {
        int opcion;
        while ((opcion = Utilidades.Menu($"FÁBRICA DE EMPAQUES - PRODUCCIÓN SEMANAL   (operarios: {cantidad}/{MAX})", OPCIONES)) != 0)
        {
            switch (opcion)
            {
                case 1: RegistrarOperario(); break;
                case 2: IngresarProduccion(); break;
                case 3: MostrarProduccionSemanal(); break;
                case 4: MostrarMejorOperario(); break;
                case 5: MostrarPromedioGeneral(); break;
                case 6: DetectarBajaProduccion(); break;
                case 7: MostrarMatriz(); break;
            }
            Utilidades.Pausar();
        }
    }

    static void RegistrarOperario()
    {
        Utilidades.MostrarTitulo("REGISTRAR OPERARIO");
        if (cantidad >= MAX) { Console.WriteLine($"Ya se registraron los {MAX} operarios permitidos."); return; }
        codigos[cantidad] = LeerCodigo("Código del operario: ", debeSerNuevo: true);
        nombres[cantidad] = Utilidades.LeerNombre("Nombre del operario: ");
        cantidad++;
        Console.WriteLine($"\nOperario {codigos[cantidad - 1]} - {nombres[cantidad - 1]} registrado correctamente.");
    }

    static void IngresarProduccion()
    {
        Utilidades.MostrarTitulo("INGRESAR PRODUCCIÓN DIARIA");
        if (!Utilidades.HayDatos(cantidad, "Aún no hay operarios registrados.")) return;

        Console.WriteLine("Operarios registrados:");
        for (int k = 0; k < cantidad; k++)
            Console.WriteLine($"  {codigos[k],-10} {nombres[k],-25} ({(tieneProduccion[k] ? "con" : "sin")} producción)");

        int i = Utilidades.BuscarIndice(codigos, cantidad, LeerCodigo("Código del operario: ", debeSerNuevo: false));
        if (i == -1) { Console.WriteLine("No existe un operario con ese código."); return; }
        if (tieneProduccion[i]) Console.WriteLine("Este operario ya tiene producción registrada; los valores se reemplazarán.");

        Console.WriteLine($"Producción de {nombres[i]} (unidades por día):");
        for (int d = 0; d < DIAS; d++)   // ningún valor puede ser negativo
            produccion[i, d] = Utilidades.LeerEntero($"  {NOMBRES_DIAS[d],-10}: ", 0, MAX_DIARIO,
                                                     $"la producción no puede ser negativa (entero entre 0 y {MAX_DIARIO}).");
        tieneProduccion[i] = true;
        Console.WriteLine($"\nProducción guardada. Total semanal: {Semanal(i)} unidades.");
    }

    static void MostrarProduccionSemanal()
    {
        Utilidades.MostrarTitulo("PRODUCCIÓN SEMANAL POR OPERARIO");
        if (!HayProduccion()) return;
        Console.WriteLine($"{"Código",-10}{"Nombre",-25}{"Total semana",-14}Promedio diario\n{new string('-', 64)}");
        for (int i = 0; i < cantidad; i++)
            if (tieneProduccion[i]) Console.WriteLine($"{codigos[i],-10}{nombres[i],-25}{Semanal(i),-14}{(double)Semanal(i) / DIAS:F2}");
        AvisarFaltantes();
    }

    static void MostrarMejorOperario()
    {
        Utilidades.MostrarTitulo("MEJOR OPERARIO DE LA SEMANA");
        if (!HayProduccion()) return;
        int mayor = -1;
        for (int i = 0; i < cantidad; i++) if (tieneProduccion[i] && Semanal(i) > mayor) mayor = Semanal(i);

        Console.WriteLine($"Mayor producción semanal: {mayor} unidades");
        for (int i = 0; i < cantidad; i++)   // se muestran todos por si hay empate
            if (tieneProduccion[i] && Semanal(i) == mayor) ObtenerOperario(i).MostrarResumen(NOMBRES_DIAS);
    }

    static void MostrarPromedioGeneral()
    {
        Utilidades.MostrarTitulo("PROMEDIO GENERAL DE PRODUCCIÓN");
        if (!HayProduccion()) return;
        Console.WriteLine($"Operarios con producción     : {ConProduccion()}");
        Console.WriteLine($"Producción total de la semana: {Total()} unidades");
        Console.WriteLine($"Promedio semanal por operario: {(double)Total() / ConProduccion():F2} unidades");
        Console.WriteLine($"Promedio diario por operario : {PromedioDiario():F2} unidades");
        AvisarFaltantes();
    }

    static void DetectarBajaProduccion()
    {
        Utilidades.MostrarTitulo("DÍAS CON BAJA PRODUCCIÓN");
        if (!HayProduccion()) return;
        Console.WriteLine($"Promedio diario por operario: {PromedioDiario():F2} unidades.");
        int meta = Utilidades.LeerEntero("Ingrese la meta mínima diaria por operario (0 = usar el promedio): ", 0, MAX_DIARIO);
        double umbral = meta == 0 ? PromedioDiario() : meta;

        // 1) Celdas de la matriz por debajo del umbral.
        Console.WriteLine($"\nRegistros por debajo de {umbral:F2} unidades:");
        int encontrados = 0;
        for (int i = 0; i < cantidad; i++)
            for (int d = 0; d < DIAS; d++)
                if (tieneProduccion[i] && produccion[i, d] < umbral)
                {
                    Console.WriteLine($"  {NOMBRES_DIAS[d],-10} {codigos[i],-10} {nombres[i],-25} {produccion[i, d]} unidades");
                    encontrados++;
                }
        if (encontrados == 0) Console.WriteLine("  Ningún operario estuvo por debajo de la meta. ¡Buen trabajo!");

        // 2) Total de cada día (columna) comparado con el promedio de los días.
        double promedioDias = (double)Total() / DIAS;
        Console.WriteLine($"\nTotal de la planta por día (promedio por día: {promedioDias:F2}):");
        for (int d = 0; d < DIAS; d++)
            Console.WriteLine($"  {NOMBRES_DIAS[d],-10}: {TotalDia(d)}{(TotalDia(d) < promedioDias ? "  <-- día de baja producción" : "")}");
    }

    static void MostrarMatriz()
    {
        Utilidades.MostrarTitulo("MATRIZ DE PRODUCCIÓN");
        if (!Utilidades.HayDatos(cantidad, "Aún no hay operarios registrados.")) return;
        Console.Write($"{"Código",-10}");
        for (int d = 0; d < DIAS; d++) Console.Write($"{NOMBRES_DIAS[d].Substring(0, 3),8}");
        Console.WriteLine($"{"Total",10}\n{new string('-', 60)}");
        for (int i = 0; i < cantidad; i++)
        {
            Console.Write($"{codigos[i],-10}");
            if (!tieneProduccion[i]) { Console.WriteLine("   (sin producción registrada)"); continue; }
            for (int d = 0; d < DIAS; d++) Console.Write($"{produccion[i, d],8}");
            Console.WriteLine($"{Semanal(i),10}");
        }
    }

    // ---- Funciones ----
    // Código: 2 a 10 letras/números en mayúsculas; al registrar no puede repetirse.
    static string LeerCodigo(string mensaje, bool debeSerNuevo) => Utilidades.LeerTexto(mensaje,
        c => Utilidades.EsAlfanumerico(c, 2, 10) && (!debeSerNuevo || Utilidades.BuscarIndice(codigos, cantidad, c.ToUpper()) == -1),
        debeSerNuevo ? "código de 2 a 10 letras o números que no esté registrado." : "código de 2 a 10 letras o números.").ToUpper();

    // Suma de la fila i (producción semanal del operario).
    static int Semanal(int i)
    {
        int suma = 0;
        for (int d = 0; d < DIAS; d++) suma += produccion[i, d];
        return suma;
    }

    // Suma de la columna d (producción de la planta ese día).
    static int TotalDia(int d)
    {
        int suma = 0;
        for (int i = 0; i < cantidad; i++) if (tieneProduccion[i]) suma += produccion[i, d];
        return suma;
    }

    static int Total()
    {
        int total = 0;
        for (int i = 0; i < cantidad; i++) if (tieneProduccion[i]) total += Semanal(i);
        return total;
    }

    static int ConProduccion()
    {
        int contador = 0;
        for (int i = 0; i < cantidad; i++) if (tieneProduccion[i]) contador++;
        return contador;
    }

    static double PromedioDiario() => (double)Total() / (ConProduccion() * DIAS);

    static bool HayProduccion() =>
        Utilidades.HayDatos(cantidad, "Aún no hay operarios registrados.") &&
        Utilidades.HayDatos(ConProduccion(), "Ningún operario tiene producción registrada todavía (use la opción 2).");

    static void AvisarFaltantes()
    {
        int faltantes = cantidad - ConProduccion();
        if (faltantes > 0) Console.WriteLine($"\nNota: {faltantes} operario(s) aún no tienen producción y no se incluyen en el cálculo.");
    }

    // POO: crea un Operario copiando su fila de la matriz a un vector.
    static Operario ObtenerOperario(int i)
    {
        int[] fila = new int[DIAS];
        for (int d = 0; d < DIAS; d++) fila[d] = produccion[i, d];
        return new Operario(codigos[i], nombres[i], fila);
    }
}
