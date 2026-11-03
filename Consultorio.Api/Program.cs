using Consultorio.Application;
using Consultorio.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Directory.CreateDirectory("data");
var conexion = builder.Configuration.GetConnectionString("Consultorio") ?? "Data Source=data/consultorio.db";
builder.Services.AddDbContext<ConsultorioDb>(opciones => opciones.UseSqlite(conexion));
builder.Services.AddScoped<ICitaRepositorio, CitaEfRepositorio>();
builder.Services.AddScoped<IPacienteRepositorio, PacienteEfRepositorio>();
builder.Services.AddScoped<Agenda>();
builder.Services.AddScoped<Consultas>();

var app = builder.Build();
using (var alcance = app.Services.CreateScope())
{
    alcance.ServiceProvider.GetRequiredService<ConsultorioDb>().Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/salud", () => new { servicio = "consultorio" });
app.Run();
