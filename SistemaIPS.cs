// CASO 1: Control de turnos para una IPS.
// Vectores paralelos: la posición i de cada vector es el mismo paciente.
// Cupos del día: 30 (Urgencias 10, Consulta General 12, Prioritaria 8). Prioridad: 1 = más alta, 5 = más baja.
static class SistemaIPS
{
    const int MAX = 30;
    static readonly string[] TIPOS = { "Urgencias", "Consulta General", "Prioritaria" };
    static readonly int[] CUPOS = { 10, 12, 8 };
    static readonly string[] NIVELES = { "Crítico", "Emergencia", "Urgente", "Menos urgente", "No urgente" };
    static readonly string[] OPCIONES =
    {
        "Registrar paciente", "Mostrar listado general", "Buscar paciente por documento",
        "Mostrar cuántos pacientes hay por tipo de atención", "Identificar el paciente con mayor prioridad",
        "Ver cupos disponibles"
    };

    static string[] documentos = new string[MAX], nombres = new string[MAX];
    static int[] edades = new int[MAX], tipos = new int[MAX], prioridades = new int[MAX];
    static int cantidad = 0;

    public static void Ejecutar()
    {
        int opcion;
        while ((opcion = Utilidades.Menu($"IPS - CONTROL DE TURNOS   (registrados: {cantidad}/{MAX})", OPCIONES)) != 0)
        {
            switch (opcion)
            {
                case 1: RegistrarPaciente(); break;
                case 2: MostrarListado(); break;
                case 3: BuscarPaciente(); break;
                case 4: MostrarPorTipo(); break;
                case 5: MostrarMayorPrioridad(); break;
                case 6: MostrarCupos(); break;
            }
            Utilidades.Pausar();
        }
    }

    static void RegistrarPaciente()
    {
        Utilidades.MostrarTitulo("REGISTRAR PACIENTE");
        if (cantidad >= MAX) { Console.WriteLine($"No hay cupos: ya se registraron los {MAX} pacientes del día."); return; }

        string documento = Utilidades.LeerTexto("Número de documento: ",
            d => EsDocumento(d) && Utilidades.BuscarIndice(documentos, cantidad, d) == -1,
            "el documento debe tener de 6 a 10 dígitos y no estar registrado hoy.");

        Console.WriteLine("Tipos de atención:");
        for (int t = 0; t < TIPOS.Length; t++) Console.WriteLine($"  {t + 1}. {TIPOS[t]}  (cupos libres: {CuposLibres(t)})");
        int tipo = Utilidades.LeerEntero("Tipo de atención (1-3): ", 1, 3) - 1;
        if (CuposLibres(tipo) == 0)
        {
            Console.WriteLine($"No hay cupos para {TIPOS[tipo]} (máximo {CUPOS[tipo]}). El paciente no fue registrado.");
            return;
        }

        documentos[cantidad] = documento;
        tipos[cantidad] = tipo;
        nombres[cantidad] = Utilidades.LeerNombre("Nombre completo: ");
        edades[cantidad] = Utilidades.LeerEntero("Edad (0-120): ", 0, 120);
        Utilidades.MostrarOpciones("Niveles de prioridad:", NIVELES);
        prioridades[cantidad] = Utilidades.LeerEntero("Nivel de prioridad (1-5): ", 1, 5);
        cantidad++;
        Console.WriteLine($"\nPaciente registrado con el turno #{cantidad}. Cupos restantes en {TIPOS[tipo]}: {CuposLibres(tipo)}.");
    }

    static void MostrarListado()
    {
        Utilidades.MostrarTitulo("LISTADO GENERAL DE PACIENTES");
        if (!HayPacientes()) return;
        Console.WriteLine($"{"Turno",-6}{"Documento",-12}{"Nombre",-26}{"Edad",-6}{"Atención",-18}Prioridad\n{new string('-', 80)}");
        for (int i = 0; i < cantidad; i++)
        {
            Console.WriteLine($"{i + 1,-6}{documentos[i],-12}{nombres[i],-26}{edades[i],-6}{TIPOS[tipos[i]],-18}" +
                              $"{prioridades[i]} - {NIVELES[prioridades[i] - 1]}");
        }
        Console.WriteLine($"\nTotal de pacientes: {cantidad}");
    }

    static void BuscarPaciente()
    {
        Utilidades.MostrarTitulo("BUSCAR PACIENTE");
        if (!HayPacientes()) return;
        string documento = Utilidades.LeerTexto("Documento a buscar: ", EsDocumento, "el documento debe tener de 6 a 10 dígitos.");
        int i = Utilidades.BuscarIndice(documentos, cantidad, documento);
        if (i == -1) Console.WriteLine("No se encontró ningún paciente con ese documento.");
        else ObtenerPaciente(i).MostrarDetalle();
    }

    static void MostrarPorTipo()
    {
        Utilidades.MostrarTitulo("PACIENTES POR TIPO DE ATENCIÓN");
        for (int t = 0; t < TIPOS.Length; t++) Console.WriteLine($"  {TIPOS[t],-18}: {Utilidades.Contar(tipos, cantidad, t)} paciente(s)");
        Console.WriteLine($"  {"TOTAL",-18}: {cantidad} paciente(s)");
    }

    static void MostrarMayorPrioridad()
    {
        Utilidades.MostrarTitulo("PACIENTE CON MAYOR PRIORIDAD");
        if (!HayPacientes()) return;
        Console.WriteLine("El siguiente paciente debe ser atendido primero:\n");
        ObtenerPaciente(IndiceMayorPrioridad()).MostrarDetalle();
    }

    static void MostrarCupos()
    {
        Utilidades.MostrarTitulo("CUPOS DISPONIBLES");
        for (int t = 0; t < TIPOS.Length; t++) Console.WriteLine($"  {TIPOS[t],-18}: {CuposLibres(t)} de {CUPOS[t]}");
        Console.WriteLine($"  {"TOTAL DEL DÍA",-18}: {MAX - cantidad} de {MAX}");
    }

    // ---- Funciones ----
    static bool EsDocumento(string d) => d.Length >= 6 && d.Length <= 10 && d.All(char.IsDigit);
    static int CuposLibres(int tipo) => CUPOS[tipo] - Utilidades.Contar(tipos, cantidad, tipo);
    static bool HayPacientes() => Utilidades.HayDatos(cantidad, "Aún no hay pacientes registrados.");

    // Número de prioridad más bajo = más urgente. En empate gana quien llegó primero.
    static int IndiceMayorPrioridad()
    {
        int mayor = 0;
        for (int i = 1; i < cantidad; i++) if (prioridades[i] < prioridades[mayor]) mayor = i;
        return mayor;
    }

    static Paciente ObtenerPaciente(int i) =>
        new Paciente(i + 1, documentos[i], nombres[i], edades[i], TIPOS[tipos[i]], prioridades[i], NIVELES[prioridades[i] - 1]);
}
