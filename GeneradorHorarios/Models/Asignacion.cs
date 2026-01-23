namespace GeneradorHorarios.Models
{
    public class Asignacion
    {
        public string NombreMateria { get; set; }
        public string Grupo { get; set; }

        // Esta propiedad combina los dos textos para mostrarlo en la lista (Ej: "Matemáticas - 6IM8")
        public string TextoVista => $"{NombreMateria} - {Grupo}";
    }
}