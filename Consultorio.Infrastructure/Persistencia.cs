using Consultorio.Application;
using Consultorio.Domain;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Infrastructure;

public sealed class ConsultorioDb(DbContextOptions<ConsultorioDb> options) : DbContext(options)
{
    public DbSet<PacienteFila> Pacientes => Set<PacienteFila>();
    public DbSet<CitaFila> Citas => Set<CitaFila>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<PacienteFila>().ToTable("Pacientes").HasKey(p => p.Id);
        model.Entity<CitaFila>().ToTable("Citas").HasKey(c => c.Id);
    }
}

public sealed class PacienteFila
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Documento { get; set; } = "";
}

public sealed class CitaFila
{
    public Guid Id { get; set; }
    public Guid MedicoId { get; set; }
    public Guid PacienteId { get; set; }
    public DateTimeOffset Inicio { get; set; }
    public DateTimeOffset Fin { get; set; }
}

public sealed class PacienteEfRepositorio(ConsultorioDb db) : IPacienteRepositorio
{
    public void Guardar(Paciente paciente)
    {
        db.Pacientes.Add(new PacienteFila
        {
            Id = paciente.Id,
            Nombre = paciente.Nombre,
            Documento = paciente.Documento.Valor,
        });
        db.SaveChanges();
    }

    public Paciente? Obtener(Guid id)
    {
        var fila = db.Pacientes.Find(id);
        return fila is null ? null : new Paciente(fila.Id, fila.Nombre, new Documento(fila.Documento));
    }
}

public sealed class CitaEfRepositorio(ConsultorioDb db) : ICitaRepositorio
{
    public void Guardar(Cita cita)
    {
        db.Citas.Add(new CitaFila
        {
            Id = cita.Id,
            MedicoId = cita.MedicoId,
            PacienteId = cita.PacienteId,
            Inicio = cita.Inicio,
            Fin = cita.Fin,
        });
        db.SaveChanges();
    }

    public IReadOnlyList<Cita> DelMedico(Guid medicoId) =>
        db.Citas.Where(c => c.MedicoId == medicoId)
            .AsEnumerable()
            .Select(c => Cita.Agendar(c.Id, c.MedicoId, c.PacienteId, c.Inicio, c.Fin, c.Inicio.AddMinutes(-1)))
            .ToList();
}

public sealed record CitaDelDia(string Paciente, string Inicio);

public sealed class Consultas(ConsultorioDb db)
{
    public IReadOnlyList<CitaDelDia> DelDia(string dia)
    {
        var conexion = db.Database.GetDbConnection();
        return conexion.Query<CitaDelDia>(
            """
            select p.Nombre as Paciente, c.Inicio as Inicio
            from Citas c
            join Pacientes p on p.Id = c.PacienteId
            where c.Inicio like @Dia || '%'
            """,
            new { Dia = dia }).ToList();
    }
}
