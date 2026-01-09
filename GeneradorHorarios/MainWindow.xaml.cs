using System.Windows;
using GeneradorHorarios.Data;
using GeneradorHorarios.Services;
using GeneradorHorarios.Models;

namespace GeneradorHorarios
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // BOTÓN: Crear nuevo horario (y exportar Excel)
        private void CrearHorario_Click(object sender, RoutedEventArgs e)
        {
            // Cargar profesores guardados
            var storage = new ProfesorStorage();
            var profesores = storage.CargarProfesores();

            if (profesores.Count == 0)
            {
                MessageBox.Show(
                    "No hay profesores registrados.\nPrimero agrega profesores.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // 👉 Por ahora tomamos el primero
            // (después se podrá elegir desde una ventana)
            var profesor = profesores[0];

            // Generar horario
            var generator = new HorarioGenerator();
            var horario = generator.GenerarHorarioProfesor(profesor);

            // Exportar a Excel usando plantilla
            var exporter = new ExcelExporter();
            exporter.ExportarProfesor(profesor);

            MessageBox.Show(
                $"Horario generado y exportado correctamente para:\n{profesor.Nombre}",
                "Proceso completado",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        // BOTÓN: Abrir archivo (futuro)
        private void AbrirArchivo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funcionalidad próximamente disponible.");
        }

        // BOTÓN: Configuración (scroll)
        private void Config_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Desplázate hacia abajo para ver las opciones de configuración.");
        }

        // BOTÓN: Administrar materias
        private void BtnMaterias_Click(object sender, RoutedEventArgs e)
        {
            var win = new MateriasWindow();
            win.Owner = this;
            win.ShowDialog();
        }

        // BOTÓN: Administrar profesores
        private void BtnProfesores_Click(object sender, RoutedEventArgs e)
        {
            var win = new ProfesoresWindow();
            win.Owner = this;
            win.ShowDialog();
        }
    }
}
