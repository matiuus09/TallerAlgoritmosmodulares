// TALLER ALGORITMOS MODULARES - Casos 1 (IPS), 4 (Parqueadero) y 5 (Producción).
// Main solo orquesta: muestra el menú y ejecuta el sistema elegido hasta que el usuario sale.
class Program
{
    static readonly string[] SISTEMAS =
    {
        "Sistema de control de turnos (IPS)",
        "Sistema de control de parqueadero (conjunto residencial)",
        "Sistema de control de producción (fábrica de empaques)"
    };

    static void Main()
    {
        Utilidades.ConfigurarConsola();
        int opcion;
        while ((opcion = Utilidades.Menu("TALLER ALGORITMOS MODULARES - MENÚ PRINCIPAL", SISTEMAS, "Salir")) != 0)
        {
            EjecutarSistema(opcion);
        }
        Console.WriteLine("\nGracias por usar el programa. ¡Hasta pronto!");
    }

    static void EjecutarSistema(int opcion)
    {
        switch (opcion)
        {
            case 1: SistemaIPS.Ejecutar(); break;
            case 2: SistemaParqueadero.Ejecutar(); break;
            case 3: SistemaProduccion.Ejecutar(); break;
        }
    }
}
