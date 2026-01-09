using GeneradorHorarios.Data;
using GeneradorHorarios.Models;
using System.Collections.Generic;
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
                TxtNombre.Text = m.Nombre;
                TxtArea.Text = m.Area;
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (ListaMaterias.SelectedItem is Materia m)
            {
                m.Nombre = TxtNombre.Text;
                m.Area = TxtArea.Text;

                storage.GuardarMaterias(materias);
                ListaMaterias.Items.Refresh();
            }
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                var nueva = new Materia(TxtNombre.Text, TxtArea.Text);

                materias.Add(nueva);
                storage.GuardarMaterias(materias);

                ListaMaterias.Items.Refresh();

                TxtNombre.Clear();
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

                TxtNombre.Clear();
                TxtArea.Clear();
            }
        }
    }
}
