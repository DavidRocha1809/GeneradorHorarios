using System.Collections.Generic;

namespace GeneradorHorarios.Models
{
    public class HorarioGrupo
    {
        public Grupo Grupo { get; set; }
        public List<BloqueHorario> Bloques { get; set; } = new();

        public HorarioGrupo(Grupo grupo)
        {
            Grupo = grupo;
        }
    }
}
