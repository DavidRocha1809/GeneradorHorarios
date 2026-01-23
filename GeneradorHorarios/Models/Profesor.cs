using System.Collections.Generic;

namespace GeneradorHorarios.Models
{
    public class Profesor
    {
        // Datos Personales
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string CorreoElectronico { get; set; }

        // Datos Laborales
        public string GradoMaximo { get; set; }
        public string Departamento { get; set; }
        public string CedulaProfesional { get; set; }
        public int HorasNombramiento { get; set; }
        public int HorasDescarga { get; set; }
        public string Plazas { get; set; }
        public string FechaIngresoSEP { get; set; }

        // Turnos
        public bool TurnoMatutino { get; set; }
        public bool TurnoVespertino { get; set; }

        // Listas
        public List<Materia> MateriasQueImparte { get; set; } = new List<Materia>();

        // --- NUEVA LISTA IMPORTANTE ---
        // Aquí guardaremos "Materia + Grupo"
        public List<Asignacion> CargaAcademica { get; set; } = new List<Asignacion>();
    }
}