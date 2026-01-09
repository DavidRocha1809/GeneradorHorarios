using GeneradorHorarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneradorHorarios.Services
{
    public class HorarioGenerator
    {
        // Bloques de tiempo del turno matutino
        public static readonly string[] HorasBloques =
        {
            "07:00 - 08:00",
            "08:00 - 09:00",
            "09:00 - 10:00",
            "10:00 - 11:00",
            "11:00 - 12:00",
            "12:00 - 13:00",
            "13:00 - 14:00",
            "14:00 - 15:00"
        };

        /// <summary>
        /// Genera un horario inicial vacío basado en el número de bloques.
        /// Preparado para llenarse después manualmente o automáticamente.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> CrearHorarioBase()
        {
            var dias = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };

            var horario = new Dictionary<string, Dictionary<string, string>>();

            foreach (var hora in HorasBloques)
            {
                horario[hora] = new Dictionary<string, string>();

                foreach (var dia in dias)
                {
                    horario[hora][dia] = ""; // vacío por ahora
                }
            }

            return horario;
        }

        /// <summary>
        /// Genera un horario automáticamente para un profesor.
        /// NOTA: Esto es una versión simple. Luego lo reemplazaremos por el algoritmo completo.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> GenerarHorarioProfesor(Profesor profesor)
        {
            var horario = CrearHorarioBase();

            // Lista de materias (solo nombres)
            var materias = profesor.MateriasQueImparte
                                   .Select(m => m.Nombre)
                                   .ToList();

            if (materias.Count == 0 || profesor.HorasDisponibles <= 0)
                return horario;

            int horasRestantes = profesor.HorasDisponibles;
            int indexMateria = 0;

            // Recorremos cada bloque horario y cada día
            foreach (var bloque in HorasBloques)
            {
                if (horasRestantes <= 0)
                    break;

                foreach (var dia in horario[bloque].Keys.ToList())
                {
                    if (horasRestantes <= 0)
                        break;

                    // Asignar materia automáticamente (simple)
                    horario[bloque][dia] = materias[indexMateria];

                    horasRestantes--;
                    indexMateria = (indexMateria + 1) % materias.Count;
                }
            }

            return horario;
        }
    }
}
