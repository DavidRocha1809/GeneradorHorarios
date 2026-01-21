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
            var storage = new ProfesorStorage();
            var profesores = storage.CargarProfesores();

            if (profesores.Count == 0)
            {
                MessageBox.Show("No hay profesores registrados.");
                return;
            }

            var profesor = profesores[0]; // (Aquí luego podrás poner un selector)

            // 1. Generar datos
            var generator = new HorarioGenerator();
            var datosHorario = generator.GenerarHorarioProfesor(profesor);

            // 2. Exportar a Word
            var wordExporter = new WordExporter();
            wordExporter.ExportarHorario(profesor, datosHorario);
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
