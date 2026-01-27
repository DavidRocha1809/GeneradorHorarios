using GeneradorHorarios.Data;
using GeneradorHorarios.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GeneradorHorarios
{
    public partial class ProfesoresWindow : Window
    {
        private List<Profesor> listaProfesoresCache = new List<Profesor>();
        private Profesor profesorSeleccionado = null;
        private List<Asignacion> asignacionesActuales = new List<Asignacion>();
        private List<Materia> listaCompletaMaterias = new List<Materia>();

        public ProfesoresWindow()
        {
            InitializeComponent();
            CargarDatosIniciales();
        }

        private void CargarDatosIniciales()
        {
            var materiaStorage = new MateriaStorage();
            listaCompletaMaterias = materiaStorage.CargarMaterias();
            cmbMaterias.ItemsSource = listaCompletaMaterias;

            CargarListaProfesores();
        }

        private void CargarListaProfesores()
        {
            var storage = new ProfesorStorage();
            listaProfesoresCache = storage.CargarProfesores();
            lstProfesoresExistentes.ItemsSource = null;
            lstProfesoresExistentes.ItemsSource = listaProfesoresCache;
        }

        // --- FILTRO DE MATERIAS ---
        private void TxtBuscarMateria_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = txtBuscarMateria.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(filtro))
            {
                cmbMaterias.ItemsSource = listaCompletaMaterias;
            }
            else
            {
                var filtradas = listaCompletaMaterias.Where(m => m.Nombre.ToLower().Contains(filtro)).ToList();
                cmbMaterias.ItemsSource = filtradas;
                if (filtradas.Count > 0) cmbMaterias.IsDropDownOpen = true;
            }
        }

        // --- AGREGAR ASIGNACIÓN CON HORAS ---
        private void BtnAgregarAsignacion_Click(object sender, RoutedEventArgs e)
        {
            var materiaObj = cmbMaterias.SelectedItem as Materia;
            string grupo = txtGrupoAsignado.Text.Trim();
            string horasTexto = txtHorasSemana.Text.Trim();

            if (materiaObj == null || string.IsNullOrEmpty(grupo))
            {
                MessageBox.Show("Selecciona materia y escribe el grupo.");
                return;
            }

            if (!int.TryParse(horasTexto, out int horasSemana) || horasSemana <= 0)
            {
                MessageBox.Show("Ingresa un número de horas válido (ej: 3, 5).");
                return;
            }

            if (asignacionesActuales.Any(a => a.NombreMateria == materiaObj.Nombre && a.Grupo.Equals(grupo, System.StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Esa asignación ya existe.");
                return;
            }

            asignacionesActuales.Add(new Asignacion
            {
                NombreMateria = materiaObj.Nombre,
                Grupo = grupo,
                HorasSemana = horasSemana
            });

            lstAsignaciones.ItemsSource = null;
            lstAsignaciones.ItemsSource = asignacionesActuales;

            txtGrupoAsignado.Clear();
            txtBuscarMateria.Clear();
        }

        private void BtnEliminarAsignacion_Click(object sender, RoutedEventArgs e)
        {
            if (lstAsignaciones.SelectedItem is Asignacion seleccionada)
            {
                asignacionesActuales.Remove(seleccionada);
                lstAsignaciones.ItemsSource = null;
                lstAsignaciones.ItemsSource = asignacionesActuales;
            }
        }

        // --- SELECCIONAR PROFESOR (EDITAR) ---
        private void LstProfesores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstProfesoresExistentes.SelectedItem is Profesor p)
            {
                profesorSeleccionado = p;
                lblTituloFormulario.Text = "Editando a: " + p.Nombre;
                btnGuardar.Content = "ACTUALIZAR DATOS";
                btnGuardar.Background = System.Windows.Media.Brushes.ForestGreen;

                // Cargar Textos
                txtNombre.Text = p.Nombre;
                txtAppPaterno.Text = p.ApellidoPaterno;
                txtAppMaterno.Text = p.ApellidoMaterno;
                txtRFC.Text = p.RFC;
                txtDepartamento.Text = p.Departamento;

                // Cargar Disponibilidad
                chkLun.IsChecked = p.AsisteLunes;
                chkMar.IsChecked = p.AsisteMartes;
                chkMie.IsChecked = p.AsisteMiercoles;
                chkJue.IsChecked = p.AsisteJueves;
                chkVie.IsChecked = p.AsisteViernes;

                txtHoraEntrada.Text = p.HoraEntrada.ToString();
                txtHoraSalida.Text = p.HoraSalida.ToString();

                chkMatutino.IsChecked = p.TurnoMatutino;
                chkVespertino.IsChecked = p.TurnoVespertino;

                // Cargar Materias
                asignacionesActuales = p.CargaAcademica != null ? new List<Asignacion>(p.CargaAcademica) : new List<Asignacion>();
                lstAsignaciones.ItemsSource = null;
                lstAsignaciones.ItemsSource = asignacionesActuales;
            }
        }

        // --- BOTÓN NUEVO ---
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            profesorSeleccionado = null;
            lstProfesoresExistentes.SelectedItem = null;
            lblTituloFormulario.Text = "Nuevo Profesor";
            btnGuardar.Content = "GUARDAR NUEVO";
            btnGuardar.Background = System.Windows.Media.Brushes.DarkBlue;

            txtNombre.Clear(); txtAppPaterno.Clear(); txtAppMaterno.Clear();
            txtRFC.Clear(); txtDepartamento.Clear(); txtBuscarMateria.Clear();

            // Defaults disponibilidad
            chkLun.IsChecked = true; chkMar.IsChecked = true; chkMie.IsChecked = true;
            chkJue.IsChecked = true; chkVie.IsChecked = true;
            txtHoraEntrada.Text = "7"; txtHoraSalida.Text = "15";
            chkMatutino.IsChecked = false; chkVespertino.IsChecked = false;

            asignacionesActuales.Clear();
            lstAsignaciones.ItemsSource = null;
        }

        // --- GUARDAR EN JSON ---
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtAppPaterno.Text))
            {
                MessageBox.Show("Nombre y Apellido son obligatorios.");
                return;
            }

            int.TryParse(txtHoraEntrada.Text, out int hEntrada);
            int.TryParse(txtHoraSalida.Text, out int hSalida);

            var storage = new ProfesorStorage();

            if (profesorSeleccionado == null)
            {
                // NUEVO
                var nuevo = new Profesor();
                LlenarObjeto(nuevo, hEntrada, hSalida);
                listaProfesoresCache.Add(nuevo);
                MessageBox.Show("Profesor agregado.");
            }
            else
            {
                // ACTUALIZAR
                LlenarObjeto(profesorSeleccionado, hEntrada, hSalida);
                MessageBox.Show("Datos actualizados.");
            }

            storage.GuardarProfesores(listaProfesoresCache);
            lstProfesoresExistentes.Items.Refresh();
            LimpiarFormulario();
        }

        private void LlenarObjeto(Profesor p, int hEntrada, int hSalida)
        {
            p.Nombre = txtNombre.Text;
            p.ApellidoPaterno = txtAppPaterno.Text;
            p.ApellidoMaterno = txtAppMaterno.Text;
            p.RFC = txtRFC.Text;
            p.Departamento = txtDepartamento.Text;

            p.AsisteLunes = chkLun.IsChecked == true;
            p.AsisteMartes = chkMar.IsChecked == true;
            p.AsisteMiercoles = chkMie.IsChecked == true;
            p.AsisteJueves = chkJue.IsChecked == true;
            p.AsisteViernes = chkVie.IsChecked == true;

            p.HoraEntrada = hEntrada;
            p.HoraSalida = hSalida;
            p.TurnoMatutino = chkMatutino.IsChecked == true;
            p.TurnoVespertino = chkVespertino.IsChecked == true;

            p.CargaAcademica = new List<Asignacion>(asignacionesActuales);
        }
    }
}