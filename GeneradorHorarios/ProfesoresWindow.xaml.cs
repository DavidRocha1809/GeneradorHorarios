using GeneradorHorarios.Data;
using GeneradorHorarios.Models;
using System.Collections.Generic;
using System.Windows;

namespace GeneradorHorarios
{
    public partial class ProfesoresWindow : Window
    {
        // Lista temporal para ir guardando lo que agregas en la pantallita
        private List<Asignacion> asignacionesActuales = new List<Asignacion>();

        public ProfesoresWindow()
        {
            InitializeComponent();
            CargarMaterias();
        }

        private void CargarMaterias()
        {
            var materiaStorage = new MateriaStorage();
            // Esto carga las materias del JSON para que salgan en el ComboBox
            cmbMaterias.ItemsSource = materiaStorage.CargarMaterias();
        }

        // Botón pequeño "Agregar a la lista"
        private void BtnAgregarAsignacion_Click(object sender, RoutedEventArgs e)
        {
            var materiaSeleccionada = cmbMaterias.SelectedItem as Materia;
            string nombreGrupo = txtGrupoAsignado.Text.Trim();

            if (materiaSeleccionada == null || string.IsNullOrEmpty(nombreGrupo))
            {
                MessageBox.Show("Por favor selecciona una materia y escribe un grupo.");
                return;
            }

            // Creamos la relación
            var nuevaAsignacion = new Asignacion
            {
                NombreMateria = materiaSeleccionada.Nombre,
                Grupo = nombreGrupo
            };

            // Agregamos a la lista temporal
            asignacionesActuales.Add(nuevaAsignacion);

            // Refrescamos la lista visual (truco de poner null primero para que se actualice)
            lstAsignaciones.ItemsSource = null;
            lstAsignaciones.ItemsSource = asignacionesActuales;

            // Limpiamos el cuadrito de texto del grupo
            txtGrupoAsignado.Text = "";
        }

        // Botón Principal "GUARDAR PROFESOR"
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            // Crear el objeto profesor con los datos de los TextBox
            var nuevoProfesor = new Profesor();
            nuevoProfesor.Nombre = txtNombre.Text;
            nuevoProfesor.ApellidoPaterno = txtAppPaterno.Text; // Asegúrate que tus textbox se llamen así
            nuevoProfesor.ApellidoMaterno = txtAppMaterno.Text;

            // ... (Asigna aquí el resto de tus campos: RFC, Correo, etc.) ...

            // IMPORTANTE: Guardamos la lista de materias+grupos que creamos
            nuevoProfesor.CargaAcademica = asignacionesActuales;

            // Guardar en JSON
            var storage = new ProfesorStorage();
            var listaProfesores = storage.CargarProfesores();
            listaProfesores.Add(nuevoProfesor);
            storage.GuardarProfesores(listaProfesores); // Asegúrate de tener este método en tu Storage

            MessageBox.Show("Profesor y su carga académica guardados correctamente.");
            this.Close();
        }
    }
}