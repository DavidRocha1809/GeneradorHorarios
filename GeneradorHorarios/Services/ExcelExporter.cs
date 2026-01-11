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
            ExcelPackage.License.SetNonCommercialPersonal("David");

            using var package = new ExcelPackage(new FileInfo(plantilla));
            var ws = package.Workbook.Worksheets["Hoja 1"];

            // --------------------------
            // 🔹 ENCABEZADO DEL PROFESOR
            // --------------------------

            ws.Cells["C7"].Value = p.ApellidoPaterno;
            ws.Cells["G7"].Value = p.ApellidoMaterno;
            ws.Cells["J7"].Value = p.Nombre;
            ws.Cells["O7"].Value = $"Correo Eléctronico: {p.CorreoElectronico}"; // nuevo: correo electrónico

            ws.Cells["A8"].Value = $"CURP: {p.CURP}";
            ws.Cells["H8"].Value = $"RFC: {p.RFC}";
            ws.Cells["L8"].Value = $"PERIODO ESCOLAR: {p.PeriodoEscolar}";

            // --------------------------
            // 🔹 DATOS ACADÉMICOS / LABORALES
            // --------------------------

            ws.Cells["D10"].Value = p.GradoMaximo;             // Grado máximo estudios
            ws.Cells["I10"].Value = p.EgresadoDe;             // Egresado de
            ws.Cells["N10"].Value = p.CedulaProfesional;      // Cédula profesional

            ws.Cells["D11"].Value = p.Departamento;           // Departamento o academia
            ws.Cells["E14"].Value = p.HorasNombramiento;      // No. horas nombramiento
            ws.Cells["E17"].Value = p.HorasDescarga;          // Horas de descarga

            ws.Cells["G14"].Value = p.Plazas;                 // Plazas
            ws.Cells["P11"].Value = p.NumeroTarjeta;          // No. tarjeta

            ws.Cells["A14"].Value = p.FechaIngresoSEP;        // Fecha ingreso SEP
            ws.Cells["C14"].Value = p.FechaIngresoDGETI;      // Fecha ingreso DGETI

            // NOMBRAMIENTO (solo uno puede estar activo)
            ws.Cells["L13"].Value = p.Nombramiento == "Base" ? "1. Base X" : "";
            ws.Cells["N13"].Value = p.Nombramiento == "Interino" ? "2. Base X" : "";
            ws.Cells["N14"].Value = (p.Nombramiento != "Base" && p.Nombramiento != "Interino") ? p.Nombramiento : "";

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
