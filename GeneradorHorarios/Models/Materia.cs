namespace GeneradorHorarios.Models
{
    public class Materia
    {
        public string Nombre { get; set; } = "";
        public string Area { get; set; } = "";

        public Materia() { }

        public Materia(string nombre, string area)
        {
            Nombre = nombre;
            Area = area;
        }
    }
}
