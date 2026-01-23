using GeneradorHorarios.Data;
using GeneradorHorarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace GeneradorHorarios
{
    public partial class MateriasWindow : Window
    {
        MateriaStorage storage = new MateriaStorage();
        List<Materia> materias;

        public MateriasWindow()
        {
            InitializeComponent();

            materias = storage.CargarMaterias();
            ListaMaterias.ItemsSource = materias;
        }

        private void ListaMaterias_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ListaMaterias.SelectedItem is Materia m)
            {
                txtNombreMateria.Text = m.Nombre;
                TxtArea.Text = m.Area;
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtener el nombre del TextBox (Asegúrate que tu TextBox se llame txtNombreMateria)
            string nombreIngresado = txtNombreMateria.Text.Trim();

            // Validación básica: Campo vacío
            if (string.IsNullOrEmpty(nombreIngresado))
            {
                MessageBox.Show("Por favor escribe el nombre de la materia.");
                return;
            }

            // ---------------------------------------------------------
            // 2. VALIDACIÓN DE DUPLICADOS
            // ---------------------------------------------------------
            var storage = new MateriaStorage();
            var materiasExistentes = storage.CargarMaterias();

            // Verificamos si alguna materia en la lista tiene el MISMO nombre
            // StringComparison.OrdinalIgnoreCase -> Ignora mayúsculas/minúsculas (Matemáticas == matemáticas)
            bool existe = materiasExistentes.Any(m =>
                m.Nombre.Equals(nombreIngresado, StringComparison.OrdinalIgnoreCase));

            if (existe)
            {
                MessageBox.Show($"La materia '{nombreIngresado}' ya existe en el sistema.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Se detiene aquí y NO guarda
            }
            // ---------------------------------------------------------

            // 3. Si no existe, procedemos a guardar
            var nuevaMateria = new Materia
            {
                Nombre = nombreIngresado
                // Si tienes otros campos como ID o Semestre, asígnalos aquí
            };

            materiasExistentes.Add(nuevaMateria);
            storage.GuardarMaterias(materiasExistentes);

            MessageBox.Show("Materia guardada correctamente.");

            // Limpiar y recargar
            txtNombreMateria.Text = "";
            CargarMaterias();
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNombreMateria.Text))
            {
                var nueva = new Materia(txtNombreMateria.Text, TxtArea.Text);

                materias.Add(nueva);
                storage.GuardarMaterias(materias);

                ListaMaterias.Items.Refresh();

                txtNombreMateria.Clear();
                TxtArea.Clear();
            }
            else
            {
                MessageBox.Show("Debes escribir el nombre de la materia.");
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (ListaMaterias.SelectedItem is Materia m)
            {
                materias.Remove(m);
                storage.GuardarMaterias(materias);
                ListaMaterias.Items.Refresh();

                txtNombreMateria.Clear();
                TxtArea.Clear();
            }
        }
    }
}
