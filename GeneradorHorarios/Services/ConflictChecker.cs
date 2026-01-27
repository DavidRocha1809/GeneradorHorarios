using GeneradorHorarios.Models;
using System.Collections.Generic;
using System.Linq;

namespace GeneradorHorarios.Services
{
    public class ConflictChecker
    {
        private List<Profesor> _todosLosProfesores;
        private Profesor _profesorActual;

        public ConflictChecker(List<Profesor> todosLosProfesores, Profesor profesorActual)
        {
            _todosLosProfesores = todosLosProfesores;
            _profesorActual = profesorActual;
        }

        // Verifica si el grupo 'targetGrupo' ya está ocupado por OTRO profesor en ese 'dia' y 'hora'
        public bool EstaElGrupoOcupado(string dia, string hora, string targetGrupo)
        {
            // Recorremos a todos los profesores EXCEPTO el que estamos generando ahorita
            foreach (var profe in _todosLosProfesores.Where(p => p.RFC != _profesorActual.RFC))
            {
                if (profe.HorarioFinalOcupado == null) continue;

                string clave = $"{dia}|{hora}";

                // Si este profesor tiene clase a esa hora
                if (profe.HorarioFinalOcupado.ContainsKey(clave))
                {
                    string contenido = profe.HorarioFinalOcupado[clave];
                    // El contenido es "Materia (Grupo)". Buscamos si contiene el grupo que queremos.
                    // Ej: Si contenido es "Matemáticas (4H)" y buscamos "4H", hay conflicto.
                    if (contenido.Contains($"({targetGrupo})"))
                    {
                        return true; // ¡Conflicto encontrado! El grupo está ocupado.
                    }
                }
            }
            return false; // El grupo está libre a esa hora
        }
    }
}