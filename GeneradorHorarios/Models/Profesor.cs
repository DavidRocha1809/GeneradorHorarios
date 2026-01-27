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

        // --- DISPONIBILIDAD ---
        public bool AsisteLunes { get; set; } = true;
        public bool AsisteMartes { get; set; } = true;
        public bool AsisteMiercoles { get; set; } = true;
        public bool AsisteJueves { get; set; } = true;
        public bool AsisteViernes { get; set; } = true;

        // Horario de permanencia (0-24)
        public int HoraEntrada { get; set; } = 7;
        public int HoraSalida { get; set; } = 15;

        // Turnos (informativo)
        public bool TurnoMatutino { get; set; }
        public bool TurnoVespertino { get; set; }

        // Listas
        public List<Materia> MateriasQueImparte { get; set; } = new List<Materia>();
        public List<Asignacion> CargaAcademica { get; set; } = new List<Asignacion>();

        public Dictionary<string, string> HorarioFinalOcupado { get; set; } = new Dictionary<string, string>();
    }
}