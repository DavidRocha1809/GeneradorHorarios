using System;

namespace GeneradorHorarios.Models
{
    public class BloqueHorarioAsignado
    {
        public string Hora { get; set; }     // Ejemplo: "07:00 - 08:00"
        public string Dia { get; set; }      // Lunes, Martes…

        public Materia Materia { get; set; }
        public Profesor Profesor { get; set; }
        public Grupo Grupo { get; set; }
        public string Salon { get; set; }

        public BloqueHorarioAsignado(string hora, string dia)
        {
            Hora = hora;
            Dia = dia;
        }

        public override string ToString()
        {
            if (Materia == null)
                return "";

            return $"{Materia.Nombre}\n{Grupo?.Nombre}\n{Profesor?.Nombre}";
        }
    }
}
