// CASO 4: Control de parqueadero de visitantes para un conjunto residencial.
// Vectores paralelos: la posición i de cada vector es el mismo ingreso.
// Las horas se guardan en minutos desde las 00:00 (08:30 = 510); si no ha salido, salida = SIN_SALIDA.
static class SistemaParqueadero
{
    const int MAX = 40, SIN_SALIDA = -1;
    static readonly string[] TIPOS = { "Carro", "Moto", "Bicicleta" };
    static readonly string[] OPCIONES =
    {
        "Registrar ingreso", "Consultar / registrar salida de vehículo", "Calcular tiempo de permanencia de un vehículo",
        "Mostrar cantidad de ingresos por tipo", "Identificar el vehículo con mayor permanencia", "Listado de ingresos del día"
    };

    static string[] placas = new string[MAX];
    static int[] torres = new int[MAX], apartamentos = new int[MAX], ingresos = new int[MAX], salidas = new int[MAX], tipos = new int[MAX];
    static int cantidad = 0;

    public static void Ejecutar()
    {
        int opcion;
        while ((opcion = Utilidades.Menu($"PARQUEADERO DE VISITANTES   (ingresos: {cantidad}/{MAX}, " +
                                         $"dentro: {Utilidades.Contar(salidas, cantidad, SIN_SALIDA)})", OPCIONES)) != 0)
        {
            switch (opcion)
            {
                case 1: RegistrarIngreso(); break;
                case 2: ConsultarSalida(); break;
                case 3: CalcularTiempoPermanencia(); break;
                case 4: MostrarPorTipo(); break;
                case 5: MostrarMayorPermanencia(); break;
                case 6: MostrarListado(); break;
            }
            Utilidades.Pausar();
        }
    }

    static void RegistrarIngreso()
    {
        Utilidades.MostrarTitulo("REGISTRAR INGRESO");
        if (cantidad >= MAX) { Console.WriteLine($"No se pueden registrar más ingresos: el máximo diario es {MAX}."); return; }

        string placa = LeerPlaca("Placa: ");
        if (BuscarActivo(placa) != -1)   // una placa no puede entrar dos veces sin haber salido
        {
            Console.WriteLine($"La placa {placa} ya está dentro del conjunto y no ha registrado salida. Ingreso rechazado.");
            return;
        }

        placas[cantidad] = placa;
        torres[cantidad] = Utilidades.LeerEntero("Torre visitada (1-10): ", 1, 10);
        apartamentos[cantidad] = Utilidades.LeerEntero("Apartamento (101-2504): ", 101, 2504);
        ingresos[cantidad] = Utilidades.LeerHora("Hora de ingreso (HH:MM): ", UltimaSalida(placa));
        Utilidades.MostrarOpciones("Tipos de vehículo:", TIPOS);
        tipos[cantidad] = Utilidades.LeerEntero("Tipo de vehículo (1-3): ", 1, 3) - 1;
        salidas[cantidad] = SIN_SALIDA;
        cantidad++;
        int i = cantidad - 1;
        Console.WriteLine($"\nIngreso registrado: {placa} ({TIPOS[tipos[i]]}) a las {Utilidades.FormatearHora(ingresos[i])} " +
                          $"- Torre {torres[i]}, Apto {apartamentos[i]}.");
    }

    static void ConsultarSalida()
    {
        Utilidades.MostrarTitulo("CONSULTAR / REGISTRAR SALIDA");
        if (!HayIngresos()) return;
        string placa = LeerPlaca("Placa del vehículo: ");
        int i = BuscarActivo(placa);

        if (i != -1)
        {
            Console.WriteLine($"El vehículo {placa} entró a las {Utilidades.FormatearHora(ingresos[i])}.");
            salidas[i] = Utilidades.LeerHora("Hora de salida (HH:MM): ", ingresos[i]);
            Console.WriteLine($"\nSalida registrada. Tiempo de permanencia: {Utilidades.FormatearDuracion(Permanencia(i))}.");
        }
        else if ((i = BuscarUltimo(placa)) != -1)
        {
            Console.WriteLine($"La placa {placa} ya salió a las {Utilidades.FormatearHora(salidas[i])} " +
                              $"(permanencia: {Utilidades.FormatearDuracion(Permanencia(i))}).");
        }
        else Console.WriteLine($"La placa {placa} no tiene ingresos registrados hoy.");
    }

    static void CalcularTiempoPermanencia()
    {
        Utilidades.MostrarTitulo("TIEMPO DE PERMANENCIA");
        if (!HayIngresos()) return;
        int i = BuscarUltimo(LeerPlaca("Placa del vehículo: "));
        if (i == -1) { Console.WriteLine("Esa placa no tiene ingresos registrados hoy."); return; }

        Vehiculo vehiculo = ObtenerVehiculo(i);
        vehiculo.MostrarDetalle();
        if (vehiculo.SigueDentro())   // si sigue dentro, se calcula hasta la hora actual
        {
            int horaActual = Utilidades.LeerHora("Hora actual (HH:MM): ", ingresos[i]);
            Console.WriteLine($"Lleva {Utilidades.FormatearDuracion(vehiculo.CalcularPermanencia(horaActual))} en el parqueadero.");
        }
    }

    static void MostrarPorTipo()
    {
        Utilidades.MostrarTitulo("INGRESOS POR TIPO DE VEHÍCULO");
        for (int t = 0; t < TIPOS.Length; t++) Console.WriteLine($"  {TIPOS[t],-10}: {Utilidades.Contar(tipos, cantidad, t)} ingreso(s)");
        Console.WriteLine($"  {"TOTAL",-10}: {cantidad} ingreso(s)");
    }

    static void MostrarMayorPermanencia()
    {
        Utilidades.MostrarTitulo("VEHÍCULO CON MAYOR PERMANENCIA");
        if (!HayIngresos()) return;
        int i = IndiceMayorPermanencia();
        if (i == -1) Console.WriteLine("Ningún vehículo ha registrado salida todavía, no se puede calcular.");
        else ObtenerVehiculo(i).MostrarDetalle();
    }

    static void MostrarListado()
    {
        Utilidades.MostrarTitulo("LISTADO DE INGRESOS DEL DÍA");
        if (!HayIngresos()) return;
        Console.WriteLine($"{"#",-4}{"Placa",-9}{"Tipo",-11}{"Torre",-7}{"Apto",-7}{"Ingreso",-9}{"Salida",-9}Permanencia\n{new string('-', 68)}");
        for (int i = 0; i < cantidad; i++)
        {
            bool dentro = salidas[i] == SIN_SALIDA;
            Console.WriteLine($"{i + 1,-4}{placas[i],-9}{TIPOS[tipos[i]],-11}{torres[i],-7}{apartamentos[i],-7}" +
                              $"{Utilidades.FormatearHora(ingresos[i]),-9}{(dentro ? "--:--" : Utilidades.FormatearHora(salidas[i])),-9}" +
                              $"{(dentro ? "Dentro" : Utilidades.FormatearDuracion(Permanencia(i)))}");
        }
    }

    // ---- Funciones ----
    // Placa: 3 a 7 letras/números; se pasa a mayúsculas y sin guiones ni espacios (xyz-45d -> XYZ45D).
    static string Normalizar(string placa) => placa.ToUpper().Replace(" ", "").Replace("-", "");
    static string LeerPlaca(string mensaje) => Normalizar(Utilidades.LeerTexto(mensaje,
        p => Utilidades.EsAlfanumerico(Normalizar(p), 3, 7), "la placa debe tener entre 3 y 7 letras o números (ej. ABC123)."));

    static int Permanencia(int i) => salidas[i] - ingresos[i];
    static bool HayIngresos() => Utilidades.HayDatos(cantidad, "Aún no hay ingresos registrados.");

    // Ingreso de la placa que aún no ha salido, o -1.
    static int BuscarActivo(string placa)
    {
        for (int i = 0; i < cantidad; i++) if (placas[i] == placa && salidas[i] == SIN_SALIDA) return i;
        return -1;
    }

    // Último ingreso (el más reciente) de la placa, o -1.
    static int BuscarUltimo(string placa)
    {
        for (int i = cantidad - 1; i >= 0; i--) if (placas[i] == placa) return i;
        return -1;
    }

    // Si la placa ya entró y salió hoy, su nuevo ingreso no puede ser antes de esa salida.
    static int UltimaSalida(string placa)
    {
        int i = BuscarUltimo(placa);
        return i == -1 || salidas[i] == SIN_SALIDA ? 0 : salidas[i];
    }

    // Ingreso (con salida) de mayor permanencia, o -1 si ninguno ha salido.
    static int IndiceMayorPermanencia()
    {
        int mayor = -1;
        for (int i = 0; i < cantidad; i++)
            if (salidas[i] != SIN_SALIDA && (mayor == -1 || Permanencia(i) > Permanencia(mayor))) mayor = i;
        return mayor;
    }

    static Vehiculo ObtenerVehiculo(int i) =>
        new Vehiculo(placas[i], TIPOS[tipos[i]], torres[i], apartamentos[i], ingresos[i], salidas[i]);
}
