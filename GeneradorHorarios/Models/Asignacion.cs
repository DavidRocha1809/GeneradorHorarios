namespace GeneradorHorarios.Models
{
    public class Asignacion
    {
        public string NombreMateria { get; set; }
        public string Grupo { get; set; }

        // La cantidad de horas que se deben impartir a este grupo
        public int HorasSemana { get; set; }

        // Texto para mostrar en la lista visual
        public string TextoVista => $"{NombreMateria} - {Grupo} ({HorasSemana} hrs)";
    }
}