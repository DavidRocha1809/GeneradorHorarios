using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GeneradorHorarios.Models;

namespace GeneradorHorarios.Data
{
    public class MateriaStorage
    {
        private readonly string rutaArchivo;

        public MateriaStorage()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string dataFolder = Path.Combine(basePath, "Data");

            // Crear carpeta si no existe
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            rutaArchivo = Path.Combine(dataFolder, "materias.json");

            // Crear archivo vacío si no existe
            if (!File.Exists(rutaArchivo))
                File.WriteAllText(rutaArchivo, "[]");
        }

        public List<Materia> CargarMaterias()
        {
            try
            {
                string json = File.ReadAllText(rutaArchivo);
                return JsonSerializer.Deserialize<List<Materia>>(json)
                    ?? new List<Materia>();
            }
            catch
            {
                return new List<Materia>();
            }
        }

        public void GuardarMaterias(List<Materia> lista)
        {
            string json = JsonSerializer.Serialize(lista, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(rutaArchivo, json);
        }
    }
}
