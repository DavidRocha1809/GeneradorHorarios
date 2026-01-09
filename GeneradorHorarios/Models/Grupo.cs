using System.Collections.Generic;

namespace GeneradorHorarios.Models
{
    public class Grupo
    {
        public string Nombre { get; set; } = "";
        public Dictionary<Materia, int> Materias { get; set; } = new();

        public Grupo(string nombre)
        {
            Nombre = nombre;
        }
    }
}
