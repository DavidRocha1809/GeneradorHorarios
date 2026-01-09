using GeneradorHorarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneradorHorarios.Services
{
    public class GeneradorAutomatico
    {
        public List<string> Dias { get; } = new()
        {
            "Lunes", "Martes", "Miércoles", "Jueves", "Viernes"
        };

        public List<string> Horas { get; } = new()
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

        // ================================================================
        // Crear un horario vacío (8 filas x 5 columnas)
        // ================================================================
        public BloqueHorarioAsignado[,] CrearHorarioVacio()
        {
            var matriz = new BloqueHorarioAsignado[Horas.Count, Dias.Count];

            for (int h = 0; h < Horas.Count; h++)
            {
                for (int d = 0; d < Dias.Count; d++)
                {
                    matriz[h, d] = new BloqueHorarioAsignado(Horas[h], Dias[d]);
                }
            }

            return matriz;
        }

        // ================================================================
        // Validar si un bloque está disponible
        // ================================================================
        public bool EstaDisponible(BloqueHorarioAsignado[,] matriz, int hora, int dia)
        {
            return matriz[hora, dia].Materia == null;
        }

        // ================================================================
        // Asignar una materia a un bloque específico
        // ================================================================
        public void Asignar(BloqueHorarioAsignado[,] matriz, int h, int d,
                            Materia materia, Profesor profesor, Grupo grupo, string salon)
        {
            matriz[h, d].Materia = materia;
            matriz[h, d].Profesor = profesor;
            matriz[h, d].Grupo = grupo;
            matriz[h, d].Salon = salon;
        }

        // ================================================================
        // Generación automática simple (puede mejorar luego)
        // ================================================================
        public void GenerarAutomatico(
            BloqueHorarioAsignado[,] matriz,
            Profesor profesor,
            List<MateriaProfesorAsignada> asignaciones)
        {
            var rnd = new Random();

            foreach (var item in asignaciones)
            {
                int horasPendientes = item.Horas;

                while (horasPendientes > 0)
                {
                    int h = rnd.Next(Horas.Count);
                    int d = rnd.Next(Dias.Count);

                    if (!EstaDisponible(matriz, h, d))
                        continue;

                    Asignar(
                        matriz,
                        h, d,
                        item.Materia,
                        profesor,
                        item.Grupo,
                        item.Salon
                    );

                    horasPendientes--;
                }
            }
        }
    }

    // ===================================================================
    // Modelo pequeño para manejar asignación del profesor
    // ===================================================================
    public class MateriaProfesorAsignada
    {
        public Materia Materia { get; set; }
        public Grupo Grupo { get; set; }
        public int Horas { get; set; }
        public string Salon { get; set; }
    }
}
