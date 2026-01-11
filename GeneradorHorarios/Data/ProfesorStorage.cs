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
            // Directorio fijo: %APPDATA%\GeneradorHorarios
            string basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GeneradorHorarios");

            // Crear carpeta si no existe
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            // Ruta completa al archivo JSON
            rutaArchivo = Path.Combine(basePath, "profesores.json");

            // Crear archivo vacío si no existe
            if (!File.Exists(rutaArchivo))
            {
                File.WriteAllText(rutaArchivo, "[]");
            }
        }

        public List<Profesor> CargarProfesores()
        {
            try
            {
                string json = File.ReadAllText(rutaArchivo);
                return JsonSerializer.Deserialize<List<Profesor>>(json) ?? new List<Profesor>();
            }
            catch
            {
                // Si hay algún problema de lectura, devolvemos una lista vacía
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
