using GeneradorHorarios.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms; // Para FolderBrowserDialog y MessageBox
using Xceed.Document.NET;
using Xceed.Words.NET; // Paquete DocX

namespace GeneradorHorarios.Services
{
    public class WordExporter
    {
        public void ExportarHorario(Profesor p, Dictionary<string, Dictionary<string, string>> datosHorario)
        {
            using var dialog = new FolderBrowserDialog() { Description = "Selecciona la carpeta de destino" };
            if (dialog.ShowDialog() != DialogResult.OK) return;

            // Ruta de la plantilla (Asegúrate que exista en tu carpeta bin/Debug/net10.0.../Plantillas)
            string rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", "PlantillaHorario.docx");
            string rutaDestino = Path.Combine(dialog.SelectedPath, $"Horario_{p.Nombre}_{p.ApellidoPaterno}.docx");

            if (!File.Exists(rutaPlantilla))
            {
                MessageBox.Show("Error: No se encontró el archivo PlantillaHorario.docx");
                return;
            }

            try
            {
                using (var doc = DocX.Load(rutaPlantilla))
                {
                    // 1. Reemplazar Nombre
                    string nombreCompleto = $"{p.Nombre} {p.ApellidoPaterno} {p.ApellidoMaterno}";
                    doc.ReplaceText("{{NOMBRE}}", nombreCompleto);

                    // 2. Llenar Tabla (Asumiendo que es la primera tabla [0])
                    if (doc.Tables.Count > 0 && datosHorario != null)
                    {
                        var tabla = doc.Tables[0];

                        // Mapeo: Hora -> Fila
                        var mapHorasFilas = new Dictionary<string, int>
                        {
                            { "07:00 - 08:00", 1 }, // Fila 1 (La 0 es encabezado)
                            { "08:00 - 09:00", 2 },
                            { "09:00 - 10:00", 3 },
                            { "10:00 - 11:00", 4 },
                            { "11:00 - 12:00", 5 },
                            { "12:00 - 13:00", 6 },
                            { "13:00 - 14:00", 7 },
                            { "14:00 - 15:00", 8 }
                        };

                        // Mapeo: Día -> Columna
                        var mapDiasColumnas = new Dictionary<string, int>
                        {
                            { "Lunes", 1 },
                            { "Martes", 2 },
                            { "Miércoles", 3 },
                            { "Jueves", 4 },
                            { "Viernes", 5 }
                        };

                        foreach (var hora in datosHorario.Keys)
                        {
                            if (!mapHorasFilas.ContainsKey(hora)) continue;
                            int fila = mapHorasFilas[hora];

                            foreach (var dia in datosHorario[hora].Keys)
                            {
                                if (!mapDiasColumnas.ContainsKey(dia)) continue;
                                int col = mapDiasColumnas[dia];

                                string contenido = datosHorario[hora][dia];

                                if (!string.IsNullOrEmpty(contenido))
                                {
                                    // Escribir en la celda
                                    tabla.Rows[fila].Cells[col].Paragraphs[0].Append(contenido);
                                    tabla.Rows[fila].Cells[col].Paragraphs[0].Alignment = Alignment.center;
                                }
                            }
                        }
                    }
                    doc.SaveAs(rutaDestino);
                }
                MessageBox.Show("Horario generado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}