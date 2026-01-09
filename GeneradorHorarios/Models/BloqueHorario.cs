namespace GeneradorHorarios.Models
{
    public class BloqueHorario
    {
        public string Dia { get; set; } = ""; // Lunes, Martes...
        public string Hora { get; set; } = ""; // Ej: 07:00-07:50
        public Materia? Materia { get; set; }
        public Profesor? Profesor { get; set; }

        public BloqueHorario(string dia, string hora)
        {
            Dia = dia;
            Hora = hora;
        }
    }
}
