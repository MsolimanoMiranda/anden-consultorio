using Consultorio.Application;
using Consultorio.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Tests;

public class PersistenciaTests
{
    [Fact]
    public void EntityFrameworkEscribeYDapperLeeElDia()
    {
        var opciones = new DbContextOptionsBuilder<ConsultorioDb>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        using var db = new ConsultorioDb(opciones);
        db.Database.OpenConnection();
        db.Database.EnsureCreated();

        var agenda = new Agenda(new CitaEfRepositorio(db), new PacienteEfRepositorio(db), () => new DateTimeOffset(2026, 11, 1, 8, 0, 0, TimeSpan.Zero));
        var paciente = agenda.RegistrarPaciente(Guid.NewGuid(), "Nora", "87654321");
        agenda.Agendar(Guid.NewGuid(), Guid.NewGuid(), paciente.Id,
            new DateTimeOffset(2026, 11, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 11, 2, 10, 30, 0, TimeSpan.Zero));

        var lecturas = new Consultas(db).DelDia("2026-11-02");
        Assert.Equal("Nora", Assert.Single(lecturas).Paciente);
    }
}
