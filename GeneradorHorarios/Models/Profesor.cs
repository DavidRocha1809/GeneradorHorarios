using System.Collections.Generic;

namespace GeneradorHorarios.Models
{
    public class Profesor
    {
        public string ApellidoPaterno { get; set; } = "";
        public string ApellidoMaterno { get; set; } = "";
        public string Nombre { get; set; } = "";

        public string CURP { get; set; } = "";
        public string RFC { get; set; } = "";
        public string PeriodoEscolar { get; set; } = "";

        public string GradoMaximo { get; set; } = "";
        public string EgresadoDe { get; set; } = "";
        public string CedulaProfesional { get; set; } = "";

        public string Departamento { get; set; } = "";

        public string Nombramiento { get; set; } = ""; // Base / Interino / Otro
        public int HorasNombramiento { get; set; }
        public int HorasDescarga { get; set; }
        public string Plazas { get; set; } = "";

        public string FechaIngresoSEP { get; set; } = "";
        public string FechaIngresoDGETI { get; set; } = "";
        public string NumeroTarjeta { get; set; } = "";

        public bool TurnoMatutino { get; set; }
        public bool TurnoVespertino { get; set; }

        // Relación con materias
        public List<Materia> MateriasQueImparte { get; set; } = new();

        // Horas disponibles para el generador de horarios
        public int HorasDisponibles { get; set; }
    }
}
