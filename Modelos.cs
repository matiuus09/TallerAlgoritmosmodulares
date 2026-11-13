// POO (puntos extra): clases pequeñas que se crean a partir de una posición de los vectores
// para mostrar el detalle de un registro o hacer un cálculo sobre él.
// Los parámetros del constructor quedan como datos privados de cada objeto (encapsulamiento).

class Paciente(int turno, string documento, string nombre, int edad, string tipoAtencion, int prioridad, string nivel)
{
    public bool EsPoblacionPreferencial() => edad < 18 || edad >= 60;

    public void MostrarDetalle() => Console.WriteLine(
        $"Turno #{turno}\n  Documento : {documento}\n  Nombre    : {nombre}\n" +
        $"  Edad      : {edad} años{(EsPoblacionPreferencial() ? " (población preferencial)" : "")}\n" +
        $"  Atención  : {tipoAtencion}\n  Prioridad : {prioridad} ({nivel})");
}

class Vehiculo(string placa, string tipo, int torre, int apartamento, int horaIngreso, int horaSalida)
{
    public bool SigueDentro() => horaSalida < 0;

    // Minutos hasta la salida, o hasta 'horaActual' si sigue dentro.
    public int CalcularPermanencia(int horaActual) => (SigueDentro() ? horaActual : horaSalida) - horaIngreso;

    public void MostrarDetalle()
    {
        Console.WriteLine($"Placa       : {placa} ({tipo})\nVisitó      : Torre {torre}, Apto {apartamento}");
        Console.WriteLine($"Ingreso     : {Utilidades.FormatearHora(horaIngreso)}");
        Console.WriteLine(SigueDentro() ? "Salida      : (sigue dentro)"
            : $"Salida      : {Utilidades.FormatearHora(horaSalida)}\n" +
              $"Permanencia : {Utilidades.FormatearDuracion(CalcularPermanencia(horaSalida))}");
    }
}

class Operario(string codigo, string nombre, int[] produccionDiaria)
{
    public int ProduccionSemanal()
    {
        int suma = 0;
        foreach (int unidades in produccionDiaria) suma += unidades;
        return suma;
    }

    public double PromedioDiario() => (double)ProduccionSemanal() / produccionDiaria.Length;

    public int MejorDia()
    {
        int mejor = 0;
        for (int d = 1; d < produccionDiaria.Length; d++) if (produccionDiaria[d] > produccionDiaria[mejor]) mejor = d;
        return mejor;
    }

    public void MostrarResumen(string[] dias)
    {
        int d = MejorDia();
        Console.WriteLine($"  -> {codigo} - {nombre}\n     Total semana   : {ProduccionSemanal()} unidades\n" +
                          $"     Promedio diario: {PromedioDiario():F2} unidades\n" +
                          $"     Mejor día      : {dias[d]} ({produccionDiaria[d]} unidades)");
    }
}
