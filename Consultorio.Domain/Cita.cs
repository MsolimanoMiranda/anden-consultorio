namespace Consultorio.Domain;

public sealed class ReglaDeNegocioException : Exception
{
    public ReglaDeNegocioException(string mensaje) : base(mensaje) { }
}

public sealed class Documento
{
    public Documento(string valor)
    {
        if (valor.Length is < 8 or > 12 || valor.Any(c => !char.IsDigit(c)))
            throw new ReglaDeNegocioException("El documento debe tener entre 8 y 12 dígitos");
        Valor = valor;
    }

    public string Valor { get; }
}

public sealed class Paciente
{
    public Paciente(Guid id, string nombre, Documento documento)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ReglaDeNegocioException("El paciente necesita un nombre");
        Id = id;
        Nombre = nombre.Trim();
        Documento = documento;
    }

    public Guid Id { get; }
    public string Nombre { get; }
    public Documento Documento { get; }
}

public sealed class Cita
{
    private Cita(Guid id, Guid medicoId, Guid pacienteId, DateTimeOffset inicio, DateTimeOffset fin)
    {
        Id = id;
        MedicoId = medicoId;
        PacienteId = pacienteId;
        Inicio = inicio;
        Fin = fin;
    }

    public Guid Id { get; }
    public Guid MedicoId { get; }
    public Guid PacienteId { get; }
    public DateTimeOffset Inicio { get; }
    public DateTimeOffset Fin { get; }

    public static Cita Agendar(Guid id, Guid medicoId, Guid pacienteId, DateTimeOffset inicio, DateTimeOffset fin, DateTimeOffset ahora)
    {
        if (fin <= inicio)
            throw new ReglaDeNegocioException("La cita termina antes de empezar");
        if (inicio < ahora)
            throw new ReglaDeNegocioException("No se agenda en el pasado");
        if (fin - inicio > TimeSpan.FromHours(2))
            throw new ReglaDeNegocioException("La cita no puede durar más de 2 horas");
        return new Cita(id, medicoId, pacienteId, inicio, fin);
    }

    public bool SeCruzaCon(Cita otra) =>
        MedicoId == otra.MedicoId && Inicio < otra.Fin && otra.Inicio < Fin;
}
