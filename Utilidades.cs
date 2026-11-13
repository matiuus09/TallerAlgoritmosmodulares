// Funciones y procedimientos de lectura, validación y búsqueda que comparten los tres sistemas.
static class Utilidades
{
    public static void ConfigurarConsola() => Console.OutputEncoding = System.Text.Encoding.UTF8;

    public static void MostrarTitulo(string titulo)
    {
        string linea = new string('=', titulo.Length + 4);
        Console.WriteLine($"\n{linea}\n  {titulo}\n{linea}");
    }

    // Muestra un menú numerado (con 0 para salir) y devuelve la opción elegida.
    public static int Menu(string titulo, string[] opciones, string salir = "Volver al menú principal")
    {
        MostrarTitulo(titulo);
        MostrarOpciones(null, opciones);
        Console.WriteLine($"  0. {salir}");
        return LeerEntero("Seleccione una opción: ", 0, opciones.Length);
    }

    public static void MostrarOpciones(string? titulo, string[] opciones)
    {
        if (titulo != null) Console.WriteLine(titulo);
        for (int i = 0; i < opciones.Length; i++) Console.WriteLine($"  {i + 1}. {opciones[i]}");
    }

    public static void Pausar()
    {
        Console.Write("\nPresione Enter para continuar...");
        LeerLinea();
    }

    // Lee una línea; si se acaba la entrada (Ctrl+Z) cierra el programa.
    public static string LeerLinea()
    {
        string? linea = Console.ReadLine();
        if (linea == null) Environment.Exit(0);
        return linea.Trim();
    }

    // Lee un texto y lo repite hasta que cumpla la condición 'esValido'.
    public static string LeerTexto(string mensaje, Func<string, bool> esValido, string error)
    {
        while (true)
        {
            Console.Write(mensaje);
            string texto = LeerLinea();
            if (esValido(texto)) return texto;
            Console.WriteLine("  Error: " + error);
        }
    }

    public static int LeerEntero(string mensaje, int min, int max, string? error = null) =>
        int.Parse(LeerTexto(mensaje, t => int.TryParse(t, out int v) && v >= min && v <= max,
                            error ?? $"ingrese un número entero entre {min} y {max}."));

    // Nombre: solo letras y espacios, mínimo 3 letras.
    public static string LeerNombre(string mensaje) =>
        string.Join(" ", LeerTexto(mensaje, t => t.All(c => char.IsLetter(c) || c == ' ') && t.Count(char.IsLetter) >= 3,
                                   "el nombre solo puede tener letras y espacios (mínimo 3 letras).")
                         .Split(' ', StringSplitOptions.RemoveEmptyEntries));

    // Lee una hora HH:MM (24 h) que no sea anterior a 'minimo'. Devuelve minutos desde las 00:00.
    public static int LeerHora(string mensaje, int minimo = 0) =>
        ConvertirHora(LeerTexto(mensaje, t => ConvertirHora(t) >= minimo,
            minimo == 0 ? "use el formato HH:MM de 24 horas (ej. 07:45)."
                        : $"use el formato HH:MM y una hora desde las {FormatearHora(minimo)}."));

    // "08:30" -> 510 minutos. Devuelve -1 si el formato no es válido.
    static int ConvertirHora(string texto)
    {
        string[] p = texto.Split(':');
        bool valida = p.Length == 2 && int.TryParse(p[0], out int h) && int.TryParse(p[1], out int m)
                      && h >= 0 && h <= 23 && m >= 0 && m <= 59;
        return valida ? int.Parse(p[0]) * 60 + int.Parse(p[1]) : -1;
    }

    public static string FormatearHora(int minutos) => $"{minutos / 60:D2}:{minutos % 60:D2}";
    public static string FormatearDuracion(int minutos) => $"{minutos / 60}h {minutos % 60:D2}m";

    public static bool EsAlfanumerico(string t, int min, int max) =>
        t.Length >= min && t.Length <= max && t.All(char.IsLetterOrDigit);

    // Posición de 'valor' en las primeras 'cantidad' posiciones del vector, o -1.
    public static int BuscarIndice(string[] vector, int cantidad, string valor)
    {
        for (int i = 0; i < cantidad; i++) if (vector[i] == valor) return i;
        return -1;
    }

    // Cuántas veces aparece 'valor' en las primeras 'cantidad' posiciones del vector.
    public static int Contar(int[] vector, int cantidad, int valor)
    {
        int contador = 0;
        for (int i = 0; i < cantidad; i++) if (vector[i] == valor) contador++;
        return contador;
    }

    public static bool HayDatos(int cantidad, string mensaje)
    {
        if (cantidad == 0) Console.WriteLine(mensaje);
        return cantidad > 0;
    }
}
