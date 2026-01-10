using GeneradorHorarios.Data;
using GeneradorHorarios.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace GeneradorHorarios
{
    public partial class ProfesoresWindow : Window
    {
        private readonly ProfesorStorage storage = new ProfesorStorage();
        private List<Profesor> profesores = new();

        private Profesor? _profesorActual = null;
        private bool _cargandoProfesor = false;
        private bool _hayCambios = false;

        public ProfesoresWindow()
        {
            InitializeComponent();

            Debug.WriteLine("=== ProfesoresWindow INICIADO ===");

            profesores = storage.CargarProfesores();

            Debug.WriteLine($"Profesores cargados: {profesores.Count}");

            foreach (var p in profesores)
            {
                Debug.WriteLine($"- {p.Nombre} | CURP: {p.CURP}");
            }

            ListaProfesores.ItemsSource = profesores;
        }

        // ==============================
        // SELECCIÓN DE PROFESOR
        // ==============================
        private void ListaProfesores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(">>> SelectionChanged");

            if (_hayCambios)
            {
                Debug.WriteLine("⚠ Hay cambios sin guardar, se cancela selección");
                ListaProfesores.SelectionChanged -= ListaProfesores_SelectionChanged;
                ListaProfesores.SelectedItem = _profesorActual;
                ListaProfesores.SelectionChanged += ListaProfesores_SelectionChanged;
                return;
            }

            if (ListaProfesores.SelectedItem is not Profesor p)
            {
                Debug.WriteLine("❌ SelectedItem no es Profesor");
                return;
            }

            Debug.WriteLine($"✔ Profesor seleccionado: {p.Nombre}");
            Debug.WriteLine($"CURP guardado: {p.CURP}");
            Debug.WriteLine($"RFC guardado: {p.RFC}");

            _cargandoProfesor = true;
            _profesorActual = p;

            CargarProfesorEnPantalla(p);

            _cargandoProfesor = false;
            _hayCambios = false;
        }

        // ==============================
        // CARGAR DATOS EN UI
        // ==============================
        private void CargarProfesorEnPantalla(Profesor p)
        {
            Debug.WriteLine(">>> CargarProfesorEnPantalla");

            Debug.WriteLine($"ApellidoP: '{p.ApellidoPaterno}'");
            Debug.WriteLine($"ApellidoM: '{p.ApellidoMaterno}'");
            Debug.WriteLine($"Nombre: '{p.Nombre}'");
            Debug.WriteLine($"CURP: '{p.CURP}'");
            Debug.WriteLine($"RFC: '{p.RFC}'");

            TxtApellidoP.Text = p.ApellidoPaterno;
            TxtApellidoM.Text = p.ApellidoMaterno;
            TxtNombre.Text = p.Nombre;
            TxtCURP.Text = p.CURP;
            TxtRFC.Text = p.RFC;
            TxtPeriodo.Text = p.PeriodoEscolar;

            TxtGradoMax.Text = p.GradoMaximo;
            TxtEgresado.Text = p.EgresadoDe;
            TxtCedula.Text = p.CedulaProfesional;

            TxtIngresoSEP.Text = p.FechaIngresoSEP;
            TxtIngresoDGETI.Text = p.FechaIngresoDGETI;
            TxtDepartamento.Text = p.Departamento;              // ✅ corregido
            TxtNumeroTarjeta.Text = p.NumeroTarjeta;            // ✅ nuevo
            TxtCorreo.Text = p.CorreoElectronico;               // ✅ nuevo

            TxtHorasNombramiento.Text = p.HorasNombramiento.ToString();
            TxtHorasDescarga.Text = p.HorasDescarga.ToString();
            TxtPlazas.Text = p.Plazas;

            RdbBase.IsChecked = p.Nombramiento == "Base";
            RdbInterino.IsChecked = p.Nombramiento == "Interino";
            TxtOtroNombramiento.Text = p.Nombramiento != "Base" && p.Nombramiento != "Interino"
                ? p.Nombramiento
                : "";

            ChkMatutino.IsChecked = p.TurnoMatutino;
            ChkVespertino.IsChecked = p.TurnoVespertino;
        }

        // ==============================
        // DETECTAR CAMBIOS
        // ==============================
        private void Campo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_cargandoProfesor) return;
            _hayCambios = true;
            Debug.WriteLine("✏ Cambio detectado en TextBox");
        }

        private void Campo_Checked(object sender, RoutedEventArgs e)
        {
            if (_cargandoProfesor) return;
            _hayCambios = true;
            Debug.WriteLine("✏ Cambio detectado en CheckBox/Radio");
        }

        // ==============================
        // GUARDAR
        // ==============================
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(">>> Guardar_Click");

            if (_profesorActual == null)
            {
                Debug.WriteLine("❌ No hay profesor actual");
                return;
            }

            Debug.WriteLine($"Guardando profesor: {_profesorActual.Nombre}");
            Debug.WriteLine($"CURP antes: {_profesorActual.CURP}");

            ActualizarProfesorDesdePantalla(_profesorActual);

            Debug.WriteLine($"CURP después: {_profesorActual.CURP}");

            storage.GuardarProfesores(profesores);

            ListaProfesores.Items.Refresh();
            _hayCambios = false;

            MessageBox.Show("Profesor guardado correctamente");
        }

        // ==============================
        // ACTUALIZAR MODELO DESDE UI
        // ==============================
        private void ActualizarProfesorDesdePantalla(Profesor p)
        {
            Debug.WriteLine(">>> ActualizarProfesorDesdePantalla");

            p.ApellidoPaterno = TxtApellidoP.Text;
            p.ApellidoMaterno = TxtApellidoM.Text;
            p.Nombre = TxtNombre.Text;

            p.CURP = TxtCURP.Text;
            p.RFC = TxtRFC.Text;
            p.PeriodoEscolar = TxtPeriodo.Text;

            p.GradoMaximo = TxtGradoMax.Text;
            p.EgresadoDe = TxtEgresado.Text;
            p.CedulaProfesional = TxtCedula.Text;

            p.FechaIngresoSEP = TxtIngresoSEP.Text;
            p.FechaIngresoDGETI = TxtIngresoDGETI.Text;
            p.Departamento = TxtDepartamento.Text;               // ✅ corregido
            p.NumeroTarjeta = TxtNumeroTarjeta.Text;             // ✅ nuevo
            p.CorreoElectronico = TxtCorreo.Text;                // ✅ nuevo
            p.Plazas = TxtPlazas.Text;

            if (int.TryParse(TxtHorasNombramiento.Text, out int horasNombramiento))
            {
                p.HorasNombramiento = horasNombramiento;
            }
            else
            {
                p.HorasNombramiento = 0;
                Debug.WriteLine("⚠ HorasNombramiento inválidas, se asigna 0");
            }

            if (int.TryParse(TxtHorasDescarga.Text, out int horasDescarga))
            {
                p.HorasDescarga = horasDescarga;
            }
            else
            {
                p.HorasDescarga = 0;
                Debug.WriteLine("⚠ HorasDescarga inválidas, se asigna 0");
            }

            if (RdbBase.IsChecked == true) p.Nombramiento = "Base";
            else if (RdbInterino.IsChecked == true) p.Nombramiento = "Interino";
            else p.Nombramiento = TxtOtroNombramiento.Text;

            p.TurnoMatutino = ChkMatutino.IsChecked == true;
            p.TurnoVespertino = ChkVespertino.IsChecked == true;
        }

        // ==============================
        // AGREGAR NUEVO
        // ==============================
        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(">>> Agregar_Click");

            var nuevo = new Profesor
            {
                Nombre = "Nuevo Profesor"
            };

            profesores.Add(nuevo);
            storage.GuardarProfesores(profesores);

            ListaProfesores.Items.Refresh();
            ListaProfesores.SelectedItem = nuevo;
        }

        // ==============================
        // ELIMINAR
        // ==============================
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_profesorActual == null) return;

            profesores.Remove(_profesorActual);
            storage.GuardarProfesores(profesores);

            ListaProfesores.Items.Refresh();
            _profesorActual = null;

            LimpiarPantalla();
        }

        private void LimpiarPantalla()
        {
            Debug.WriteLine(">>> LimpiarPantalla");

            TxtApellidoP.Clear();
            TxtApellidoM.Clear();
            TxtNombre.Clear();
            TxtCURP.Clear();
            TxtRFC.Clear();
            TxtPeriodo.Clear();
        }
    }
}
