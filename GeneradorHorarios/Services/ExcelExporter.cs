using GeneradorHorarios.Models;
using OfficeOpenXml;
using System;
using System.IO;
using System.Windows.Forms;

namespace GeneradorHorarios.Services
{
    public class ExcelExporter
    {
        public void ExportarProfesor(Profesor p)
        {
            // Seleccionar carpeta destino
            using var dialog = new FolderBrowserDialog()
            {
                Description = "Selecciona la carpeta donde deseas guardar el horario del profesor."
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string carpetaDestino = dialog.SelectedPath;

            // Ruta a la plantilla incluida en el proyecto
            string plantilla = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Plantillas",
                "PlantillaHorario.xlsx"
            );

            if (!File.Exists(plantilla))
            {
                System.Windows.MessageBox.Show(
                    "No se encontró la plantilla PlantillaHorario.xlsx",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
                return;
            }

            // Archivo final
            string destino = Path.Combine(
                carpetaDestino,
                $"{p.ApellidoPaterno}_{p.Nombre}_Horario.xlsx"
            );

            // EPPlus requiere definir el contexto de licencia
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(plantilla));
            var ws = package.Workbook.Worksheets["Hoja 1"];

            // --------------------------
            // 🔹 ENCABEZADO DEL PROFESOR
            // --------------------------

            ws.Cells["C7"].Value = p.ApellidoPaterno;
            ws.Cells["G7"].Value = p.ApellidoMaterno;
            ws.Cells["J7"].Value = p.Nombre;

            ws.Cells["C9"].Value = p.CURP;
            ws.Cells["G9"].Value = p.RFC;
            ws.Cells["L9"].Value = p.PeriodoEscolar;

            // --------------------------
            // 🔹 DATOS ACADÉMICOS / LABORALES
            // --------------------------

            ws.Cells["D10"].Value = p.GradoMaximo;             // Grado máximo estudios
            ws.Cells["I10"].Value = p.EgresadoDe;             // Egresado de
            ws.Cells["N10"].Value = p.CedulaProfesional;      // Cédula profesional

            ws.Cells["D11"].Value = p.Departamento;           // Departamento o academia
            ws.Cells["E15"].Value = p.HorasNombramiento;      // No. horas nombramiento
            ws.Cells["E18"].Value = p.HorasDescarga;          // Horas de descarga

            ws.Cells["G18"].Value = p.Plazas;                 // Plazas
            ws.Cells["P11"].Value = p.NumeroTarjeta;          // No. tarjeta

            ws.Cells["A18"].Value = p.FechaIngresoSEP;        // Fecha ingreso SEP
            ws.Cells["C18"].Value = p.FechaIngresoDGETI;      // Fecha ingreso DGETI

            // NOMBRAMIENTO (solo uno puede estar activo)
            ws.Cells["L13"].Value = p.Nombramiento == "Base" ? "X" : "";
            ws.Cells["N13"].Value = p.Nombramiento == "Interino" ? "X" : "";
            ws.Cells["N14"].Value = p.Nombramiento == "Otro" ? p.Nombramiento : "";

            // TURNO
            ws.Cells["P15"].Value = p.TurnoMatutino ? "X" : "";
            ws.Cells["Q15"].Value = p.TurnoVespertino ? "X" : "";

            // ------------------------------------------
            // 🔹 HORARIO ACADÉMICO (SE LLENARÁ MÁS ADELANTE)
            // ------------------------------------------
            // Más adelante rellenaremos bloques:
            // Lunes – Viernes desde fila 22 a 29

            // Guardar archivo final
            package.SaveAs(new FileInfo(destino));

            System.Windows.MessageBox.Show(
                $"Horario exportado correctamente en:\n{destino}",
                "Exportación completa",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );
        }
    }
}
