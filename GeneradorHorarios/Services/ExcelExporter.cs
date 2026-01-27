using GeneradorHorarios.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GeneradorHorarios.Services
{
    public class ExcelExporter
    {
        public void ExportarHorario(Profesor p, Dictionary<string, Dictionary<string, string>> datosHorario)
        {
            ExcelPackage.License.SetNonCommercialPersonal("David");

            using var dialog = new FolderBrowserDialog();
            dialog.Description = "Selecciona la carpeta para guardar el horario";

            if (dialog.ShowDialog() != DialogResult.OK) return;

            string rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", "Plantilla.xlsx");
            string nombreArchivo = $"Horario_{p.ApellidoPaterno}_{p.Nombre}.xlsx";
            string rutaSalida = Path.Combine(dialog.SelectedPath, nombreArchivo);

            if (!File.Exists(rutaPlantilla))
            {
                MessageBox.Show("No se encontró 'PlantillaHorario.xlsx' en la carpeta Plantillas.");
                return;
            }

            try
            {
                var template = new FileInfo(rutaPlantilla);
                var output = new FileInfo(rutaSalida);

                using (var package = new ExcelPackage(output, template))
                {
                    var ws = package.Workbook.Worksheets[0];

                    // --- ENCABEZADOS ---
                    ws.Cells[7, 3].Value = p.ApellidoPaterno?.ToUpper();
                    ws.Cells[7, 7].Value = p.ApellidoMaterno?.ToUpper();
                    ws.Cells[7, 10].Value = p.Nombre?.ToUpper();
                    ws.Cells[8, 3].Value = p.CURP?.ToUpper();
                    ws.Cells[8, 9].Value = p.RFC?.ToUpper();
                    ws.Cells[11, 4].Value = p.Departamento?.ToUpper();

                    // --- MATRIZ DE HORARIO (Filas 22 a 31) ---
                    var filasHoras = new Dictionary<string, int>
                    {
                        { "07:00 - 07:50", 22 },
                        { "07:50 - 08:40", 23 },
                        { "08:40 - 09:30", 24 }, 
                        
                        // Bifurcación
                        { "09:30 - 10:20", 25 },
                        { "09:50 - 10:40", 26 },

                        { "10:40 - 11:30", 27 },
                        { "11:30 - 12:20", 28 },
                        { "12:20 - 13:10", 29 },
                        { "13:10 - 14:00", 30 }, // Nueva Fila
                        { "14:00 - 14:50", 31 }  // Final
                    };

                    var colDias = new Dictionary<string, int>
                    {
                        { "Lunes", 3 }, { "Martes", 4 }, { "Miércoles", 5 }, { "Jueves", 6 }, { "Viernes", 7 }
                    };

                    foreach (var hora in datosHorario.Keys)
                    {
                        if (!filasHoras.ContainsKey(hora)) continue;
                        int fila = filasHoras[hora];

                        foreach (var dia in datosHorario[hora].Keys)
                        {
                            int col = colDias[dia];
                            string contenido = datosHorario[hora][dia];

                            if (!string.IsNullOrEmpty(contenido))
                            {
                                var celda = ws.Cells[fila, col];
                                celda.Value = contenido;
                                celda.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                celda.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                celda.Style.WrapText = true;
                                celda.Style.Font.Size = 9;
                                celda.Style.Font.Bold = true;
                            }
                        }
                    }

                    // --- PIE DE PÁGINA (Hora Entrada / Salida) ---
                    // Bajamos a 34 y 35 para estar seguros
                    int filaEntrada = 34;
                    int filaSalida = 35;

                    string strEntrada = $"{p.HoraEntrada:00}:00:00";
                    string strSalida = $"{p.HoraSalida:00}:00:00";

                    if (p.AsisteLunes) { ws.Cells[filaEntrada, 3].Value = strEntrada; ws.Cells[filaSalida, 3].Value = strSalida; }
                    if (p.AsisteMartes) { ws.Cells[filaEntrada, 4].Value = strEntrada; ws.Cells[filaSalida, 4].Value = strSalida; }
                    if (p.AsisteMiercoles) { ws.Cells[filaEntrada, 5].Value = strEntrada; ws.Cells[filaSalida, 5].Value = strSalida; }
                    if (p.AsisteJueves) { ws.Cells[filaEntrada, 6].Value = strEntrada; ws.Cells[filaSalida, 6].Value = strSalida; }
                    if (p.AsisteViernes) { ws.Cells[filaEntrada, 7].Value = strEntrada; ws.Cells[filaSalida, 7].Value = strSalida; }

                    package.Save();
                }

                MessageBox.Show($"¡Horario generado correctamente!\nGuardado en: {rutaSalida}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}");
            }
        }
    }
}