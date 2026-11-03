using Consultorio.Application;
using Consultorio.Domain;

namespace Consultorio.Tests;

public class AgendaTests
{
    [Fact]
    public void NoPermiteDosCitasDelMismoMedicoEnElMismoHorario()
    {
        var agenda = new Agenda(new CitasMemoria(), new PacientesMemoria(), () => new DateTimeOffset(2026, 11, 1, 8, 0, 0, TimeSpan.Zero));
        var paciente = agenda.RegistrarPaciente(Guid.NewGuid(), "Ana", "12345678");
        var medico = Guid.NewGuid();
        agenda.Agendar(Guid.NewGuid(), medico, paciente.Id,
            new DateTimeOffset(2026, 11, 2, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 11, 2, 9, 30, 0, TimeSpan.Zero));

        var error = Assert.Throws<ReglaDeNegocioException>(() => agenda.Agendar(
            Guid.NewGuid(), medico, paciente.Id,
            new DateTimeOffset(2026, 11, 2, 9, 15, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 11, 2, 9, 45, 0, TimeSpan.Zero)));
        Assert.Contains("horario", error.Message);
    }
}

file sealed class CitasMemoria : ICitaRepositorio
{
    private readonly List<Cita> _citas = [];
    public void Guardar(Cita cita) => _citas.Add(cita);
    public IReadOnlyList<Cita> DelMedico(Guid medicoId) => _citas.Where(c => c.MedicoId == medicoId).ToList();
}

file sealed class PacientesMemoria : IPacienteRepositorio
{
    private readonly Dictionary<Guid, Paciente> _pacientes = [];
    public void Guardar(Paciente paciente) => _pacientes[paciente.Id] = paciente;
    public Paciente? Obtener(Guid id) => _pacientes.GetValueOrDefault(id);
}
