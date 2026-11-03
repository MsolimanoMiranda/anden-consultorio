using Consultorio.Domain;

namespace Consultorio.Application;

public interface ICitaRepositorio
{
    void Guardar(Cita cita);
    IReadOnlyList<Cita> DelMedico(Guid medicoId);
}

public interface IPacienteRepositorio
{
    void Guardar(Paciente paciente);
    Paciente? Obtener(Guid id);
}

public sealed class Agenda
{
    private readonly ICitaRepositorio _citas;
    private readonly IPacienteRepositorio _pacientes;
    private readonly Func<DateTimeOffset> _ahora;

    public Agenda(ICitaRepositorio citas, IPacienteRepositorio pacientes, Func<DateTimeOffset>? ahora = null)
    {
        _citas = citas;
        _pacientes = pacientes;
        _ahora = ahora ?? (() => DateTimeOffset.UtcNow);
    }

    public Paciente RegistrarPaciente(Guid id, string nombre, string documento)
    {
        var paciente = new Paciente(id, nombre, new Documento(documento));
        _pacientes.Guardar(paciente);
        return paciente;
    }

    public Cita Agendar(Guid id, Guid medicoId, Guid pacienteId, DateTimeOffset inicio, DateTimeOffset fin)
    {
        if (_pacientes.Obtener(pacienteId) is null)
            throw new ReglaDeNegocioException("El paciente no existe");
        var cita = Cita.Agendar(id, medicoId, pacienteId, inicio, fin, _ahora());
        if (_citas.DelMedico(medicoId).Any(existente => existente.SeCruzaCon(cita)))
            throw new ReglaDeNegocioException("El médico ya tiene una cita en ese horario");
        _citas.Guardar(cita);
        return cita;
    }
}
