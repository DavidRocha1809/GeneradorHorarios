using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GeneradorHorarios.Models;

namespace GeneradorHorarios.Data
{
    public class ProfesorStorage
    {
        private readonly string rutaArchivo;

        public ProfesorStorage()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string dataFolder = Path.Combine(basePath, "Data");

            // Crear carpeta si no existe
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            rutaArchivo = Path.Combine(dataFolder, "profesores.json");

            // Crear archivo vacío si no existe
            if (!File.Exists(rutaArchivo))
                File.WriteAllText(rutaArchivo, "[]");
        }

        public List<Profesor> CargarProfesores()
        {
            try
            {
                string json = File.ReadAllText(rutaArchivo);

                return JsonSerializer.Deserialize<List<Profesor>>(json)
                    ?? new List<Profesor>();
            }
            catch
            {
                return new List<Profesor>();
            }
        }

        public void GuardarProfesores(List<Profesor> lista)
        {
            string json = JsonSerializer.Serialize(lista, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(rutaArchivo, json);
        }
    }
}
