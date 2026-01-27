using GeneradorHorarios.Data;
using GeneradorHorarios.Models;
using System;
using System.Collections.Generic;
using System.Linq; // Necesario para .Any()
using System.Windows;
using System.Windows.Controls; // Necesario para SelectionChanged

namespace GeneradorHorarios
{
    public partial class MateriasWindow : Window
    {
        public MateriasWindow()
        {
            InitializeComponent();
            CargarMaterias();
        }

        private void CargarMaterias()
        {
            try
            {
                var storage = new MateriaStorage();
                var materias = storage.CargarMaterias();
                lstMaterias.ItemsSource = null;
                lstMaterias.ItemsSource = materias;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar materias: " + ex.Message);
            }
        }

        // --- 1. BOTÓN AGREGAR (Con validación de duplicados) ---
        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            string nombreIngresado = txtNombreMateria.Text.Trim();

            if (string.IsNullOrEmpty(nombreIngresado))
            {
                MessageBox.Show("Por favor escribe el nombre de la materia.");
                return;
            }

            var storage = new MateriaStorage();
            var lista = storage.CargarMaterias();

            // Validación: ¿Ya existe?
            bool existe = lista.Any(m => m.Nombre.Equals(nombreIngresado, StringComparison.OrdinalIgnoreCase));

            if (existe)
            {
                MessageBox.Show($"La materia '{nombreIngresado}' ya existe.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Guardar nueva
            lista.Add(new Materia { Nombre = nombreIngresado });
            storage.GuardarMaterias(lista);

            MessageBox.Show("Materia guardada.");
            txtNombreMateria.Text = ""; // Limpiar
            CargarMaterias(); // Recargar lista
        }

        // --- 2. BOTÓN ELIMINAR (Que faltaba antes) ---
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var materiaSeleccionada = lstMaterias.SelectedItem as Materia;

            if (materiaSeleccionada == null)
            {
                MessageBox.Show("Selecciona una materia de la lista para eliminar.");
                return;
            }

            var confirm = MessageBox.Show($"¿Estás seguro de eliminar '{materiaSeleccionada.Nombre}'?",
                                          "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                var storage = new MateriaStorage();
                var lista = storage.CargarMaterias();

                // Buscamos y eliminamos
                var itemAEliminar = lista.FirstOrDefault(m => m.Nombre == materiaSeleccionada.Nombre);
                if (itemAEliminar != null)
                {
                    lista.Remove(itemAEliminar);
                    storage.GuardarMaterias(lista);

                    txtNombreMateria.Text = "";
                    CargarMaterias();
                }
            }
        }

        // --- 3. EVENTO DE SELECCIÓN (Para llenar el textbox al dar click en la lista) ---
        private void ListaMaterias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMaterias.SelectedItem is Materia materia)
            {
                txtNombreMateria.Text = materia.Nombre;
            }
        }
    }
}