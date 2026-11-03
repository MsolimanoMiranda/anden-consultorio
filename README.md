# Consultorio Norte

Citas con Clean Architecture, DDD y las dos formas de leer datos que pedía el stack: Entity Framework escribe el agregado y Dapper lee el día.

## Reglas

- La cita no empieza en el pasado, no termina antes de empezar y no dura más de dos horas.
- El documento del paciente tiene entre 8 y 12 dígitos.
- Un médico no tiene dos citas que se crucen.

## Correr

```bash
cd projects/dotnet-consultorio
dotnet test
dotnet run --project Consultorio.Api
```

Swagger queda en `/swagger`. El API escucha el puerto que indique `launchSettings.json`.
