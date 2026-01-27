using GeneradorHorarios.Data;
using GeneradorHorarios.Models;
using GeneradorHorarios.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GeneradorHorarios
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CargarProfesores();
        }

        // Método para refrescar la lista
        private void CargarProfesores()
        {
            var storage = new ProfesorStorage();
            List<Profesor> lista = storage.CargarProfesores();
            lstProfesores.ItemsSource = null;
            lstProfesores.ItemsSource = lista;
        }

        // Abrir Ventana de Materias
        private void BtnAbrirMaterias_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new MateriasWindow();
            ventana.ShowDialog();
        }

        // Abrir Ventana de Profesores
        private void BtnAbrirProfesores_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new ProfesoresWindow();
            ventana.ShowDialog();
            CargarProfesores(); // Al cerrar, recargamos la lista
        }

        // ---------------------------------------------------------
        // BOTÓN 1: GENERAR WORD (CORREGIDO)
        // ---------------------------------------------------------
        private void BtnGenerarWord_Click(object sender, RoutedEventArgs e)
        {
            var profesorSeleccionado = lstProfesores.SelectedItem as Profesor;

            if (profesorSeleccionado == null)
            {
                MessageBox.Show("Por favor, selecciona un profesor de la lista primero.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 1. NECESITAMOS CARGAR LA LISTA COMPLETA AUNQUE SEA WORD
            // (El generador la pide obligatoriamente para checar conflictos)
            var storage = new ProfesorStorage();
            var todosLosProfesores = storage.CargarProfesores();

            // Buscamos la referencia exacta
            var profesorEnLista = todosLosProfesores.FirstOrDefault(p => p.RFC == profesorSeleccionado.RFC);
            if (profesorEnLista == null) profesorEnLista = profesorSeleccionado;

            // 2. Generar Lógica (Pasando los DOS argumentos)
            var generator = new HorarioGenerator();
            // --- AQUÍ ESTABA EL ERROR: Faltaba 'todosLosProfesores' ---
            var datosHorario = generator.GenerarHorarioProfesor(profesorEnLista, todosLosProfesores);

            // 3. Exportar a Word
            var exporter = new WordExporter();
            exporter.ExportarHorario(profesorEnLista, datosHorario);
        }

        // ---------------------------------------------------------
        // BOTÓN 2: GENERAR EXCEL
        // ---------------------------------------------------------
        private void BtnGenerarExcel_Click(object sender, RoutedEventArgs e)
        {
            var profesorSeleccionado = lstProfesores.SelectedItem as Profesor;

            if (profesorSeleccionado == null)
            {
                MessageBox.Show("Por favor, selecciona un profesor de la lista primero.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 1. Cargar lista para conflictos
            var storage = new ProfesorStorage();
            var todosLosProfesores = storage.CargarProfesores();

            var profesorEnLista = todosLosProfesores.FirstOrDefault(p => p.RFC == profesorSeleccionado.RFC);
            if (profesorEnLista == null) profesorEnLista = profesorSeleccionado;

            // 2. Generar con detección de conflictos
            var generator = new HorarioGenerator();
            var matrizParaExcel = generator.GenerarHorarioProfesor(profesorEnLista, todosLosProfesores);

            // 3. GUARDAR EL RESULTADO EN JSON (Para que el próximo sepa que está ocupado)
            storage.GuardarProfesores(todosLosProfesores);

            // 4. Exportar a Excel
            var exporter = new ExcelExporter();
            exporter.ExportarHorario(profesorEnLista, matrizParaExcel);

            // Refrescar y avisar
            CargarProfesores();
            MessageBox.Show($"Horario de {profesorEnLista.Nombre} generado y espacios reservados.");
        }

        private void BtnLiberarHoras_Click(object sender, RoutedEventArgs e)
        {
            var profesorSeleccionado = lstProfesores.SelectedItem as Profesor;

            if (profesorSeleccionado == null)
            {
                MessageBox.Show("Selecciona un profesor para liberarle el horario.");
                return;
            }

            if (MessageBox.Show($"¿Estás seguro de borrar el horario generado de {profesorSeleccionado.Nombre}?\n\nEsto liberará los grupos y horas que tenía ocupados.",
                                "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // 1. Cargar la lista real
                var storage = new ProfesorStorage();
                var todos = storage.CargarProfesores();
                var profeReal = todos.FirstOrDefault(p => p.RFC == profesorSeleccionado.RFC);

                if (profeReal != null)
                {
                    // 2. LIMPIAR SU MEMORIA DE HORARIO
                    profeReal.HorarioFinalOcupado.Clear();

                    // 3. GUARDAR EL CAMBIO
                    storage.GuardarProfesores(todos);

                    // 4. Actualizar la vista
                    CargarProfesores();
                    MessageBox.Show("Horario eliminado. Las horas de este profesor han quedado libres para otros.");
                }
            }
        }
        }
}