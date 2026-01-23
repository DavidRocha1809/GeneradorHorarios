using GeneradorHorarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneradorHorarios.Services
{
    public class HorarioGenerator
    {
        public Dictionary<string, Dictionary<string, string>> GenerarHorarioProfesor(Profesor profesor)
        {
            var horarioGenerado = new Dictionary<string, Dictionary<string, string>>();
            var random = new Random();

            // --- CAMBIO AQUI ---
            // Convertimos la CargaAcademica (Materia + Grupo) en la lista de textos a repartir
            var materias = new List<string>();

            if (profesor.CargaAcademica != null && profesor.CargaAcademica.Count > 0)
            {
                // Si tiene asignaciones nuevas (Materia + Grupo)
                foreach (var asignacion in profesor.CargaAcademica)
                {
                    // Esto generará texto como: "Matemáticas I (6IM8)"
                    materias.Add($"{asignacion.NombreMateria} ({asignacion.Grupo})");
                }
            }
            else
            {
                // Si es un profesor antiguo que no tiene grupos asignados, usamos solo el nombre
                foreach (var m in profesor.MateriasQueImparte)
                {
                    materias.Add(m.Nombre);
                }
            }
            // -------------------

            // Definir horas y días (Igual que antes)
            var horas = new List<string>
            {
                "07:00 - 08:00", "08:00 - 09:00", "09:00 - 10:00",
                "10:00 - 11:00", "11:00 - 12:00", "12:00 - 13:00",
                "13:00 - 14:00", "14:00 - 15:00"
            };

            var dias = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };

            // Inicializar diccionario vacío
            foreach (var hora in horas)
            {
                horarioGenerado[hora] = new Dictionary<string, string>();
                foreach (var dia in dias)
                {
                    horarioGenerado[hora][dia] = "";
                }
            }

            // Lógica simple de asignación aleatoria (Tu lógica existente)
            foreach (var materia in materias)
            {
                int horasAsignadas = 0;
                int horasObjetivo = 5; // Ejemplo: 5 horas por materia a la semana

                while (horasAsignadas < horasObjetivo)
                {
                    var hora = horas[random.Next(horas.Count)];
                    var dia = dias[random.Next(dias.Count)];

                    if (string.IsNullOrEmpty(horarioGenerado[hora][dia]))
                    {
                        horarioGenerado[hora][dia] = materia;
                        horasAsignadas++;
                    }
                }
            }

            return horarioGenerado;
        }
    }
}