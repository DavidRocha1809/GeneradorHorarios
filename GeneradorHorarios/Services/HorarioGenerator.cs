using GeneradorHorarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneradorHorarios.Services
{
    public class ItemMateria
    {
        public string Nombre { get; set; }
        public string GrupoSolo { get; set; }
        public int Horas { get; set; }
    }

    public class HorarioGenerator
    {
        public Dictionary<string, Dictionary<string, string>> GenerarHorarioProfesor(Profesor profesorActual, List<Profesor> todosLosProfesores)
        {
            var horarioMatriz = new Dictionary<string, Dictionary<string, string>>();
            var horarioPlanoParaGuardar = new Dictionary<string, string>();

            var random = new Random();
            var checker = new ConflictChecker(todosLosProfesores, profesorActual);

            // 1. GRID (10 Bloques - Fila 22 a 31)
            var horasPosibles = new List<string>
            {
                "07:00 - 07:50", "07:50 - 08:40", "08:40 - 09:30",
                "09:30 - 10:20", // Bifurcación A
                "09:50 - 10:40", // Bifurcación B
                "10:40 - 11:30", "11:30 - 12:20", "12:20 - 13:10",
                "13:10 - 14:00", "14:00 - 14:50"
            };

            var diasSemana = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes" };

            // Inicializar
            foreach (var h in horasPosibles)
            {
                horarioMatriz[h] = new Dictionary<string, string>();
                foreach (var d in diasSemana) horarioMatriz[h][d] = "";
            }

            // 2. DISPONIBILIDAD
            var diasDisponibles = new List<string>();
            if (profesorActual.AsisteLunes) diasDisponibles.Add("Lunes");
            if (profesorActual.AsisteMartes) diasDisponibles.Add("Martes");
            if (profesorActual.AsisteMiercoles) diasDisponibles.Add("Miércoles");
            if (profesorActual.AsisteJueves) diasDisponibles.Add("Jueves");
            if (profesorActual.AsisteViernes) diasDisponibles.Add("Viernes");
            if (diasDisponibles.Count == 0) diasDisponibles.AddRange(diasSemana);

            // 3. PREPARAR MATERIAS
            var listaPorAsignar = new List<ItemMateria>();
            if (profesorActual.CargaAcademica != null)
            {
                foreach (var a in profesorActual.CargaAcademica)
                {
                    int h = a.HorasSemana > 0 ? a.HorasSemana : 5;
                    listaPorAsignar.Add(new ItemMateria
                    {
                        Nombre = $"{a.NombreMateria} ({a.Grupo})",
                        GrupoSolo = a.Grupo.Trim(),
                        Horas = h
                    });
                }
            }

            // 4. ALGORITMO PRINCIPAL
            foreach (var item in listaPorAsignar)
            {
                int horasRestantes = item.Horas;

                // CONTROL DE CARGA DIARIA (Para materias de 10 horas)
                // Llevamos la cuenta de cuántas horas de ESTA materia van en cada día
                var horasAsignadasPorDia = new Dictionary<string, int>();
                foreach (var d in diasDisponibles) horasAsignadasPorDia[d] = 0;

                // Definimos el "Tope Ideal" por día. 
                // Si la materia es de 10 horas y va 5 días, el tope ideal es 2 horas/día.
                // Si es de 5 horas, tope ideal es 2.
                // Le damos un margen de +1 o +2 para permitir bloques grandes de taller.
                int topeDiario = (int)Math.Ceiling((double)item.Horas / diasDisponibles.Count) + 1;
                if (topeDiario < 2) topeDiario = 2; // Mínimo dejamos poner bloques de 2

                // Pre-cálculo de índices válidos (Filtrando Recesos)
                var indicesValidosParaGrupo = new List<int>();
                string horaProhibidaReceso = "";
                if (!string.IsNullOrEmpty(item.GrupoSolo))
                {
                    char primerDigito = item.GrupoSolo[0];
                    if (primerDigito == '2') horaProhibidaReceso = "09:30 - 10:20";
                    else if (primerDigito == '4' || primerDigito == '6') horaProhibidaReceso = "09:50 - 10:40";
                }

                for (int i = 0; i < horasPosibles.Count; i++)
                {
                    string horaStr = horasPosibles[i];
                    int horaInicio = int.Parse(horaStr.Substring(0, 2));
                    if (horaInicio < profesorActual.HoraEntrada || horaInicio >= profesorActual.HoraSalida) continue;
                    if (horaStr == horaProhibidaReceso) continue;
                    indicesValidosParaGrupo.Add(i);
                }

                if (indicesValidosParaGrupo.Count == 0) continue;

                // -------------------------------------------------------------
                // FASE 1: BLOQUES DE 2 HORAS (DISTRIBUIDOS)
                // -------------------------------------------------------------
                while (horasRestantes >= 2)
                {
                    bool bloqueAsignado = false;
                    int intentos = 0;

                    while (intentos < 100 && !bloqueAsignado)
                    {
                        // ESTRATEGIA DE SELECCIÓN DE DÍA:
                        // 1. Buscamos días que NO hayan superado el tope diario.
                        // 2. De esos, preferimos los que tengan MENOS carga actual (para equilibrar).

                        var diasCandidatos = diasDisponibles
                            .Where(d => horasAsignadasPorDia[d] + 2 <= topeDiario) // Que quepa un bloque más
                            .OrderBy(d => horasAsignadasPorDia[d]) // Preferir los vacíos
                            .ToList();

                        // Si ya todos están llenos al tope, relajamos la regla y agarramos cualquiera disponible
                        if (diasCandidatos.Count == 0)
                            diasCandidatos = diasDisponibles.ToList();

                        // Elegir uno al azar de los candidatos (para mantener variedad)
                        var dia = diasCandidatos[random.Next(diasCandidatos.Count)];

                        // Buscar hueco consecutivo
                        if (indicesValidosParaGrupo.Count < 2) break;
                        int posLista = random.Next(indicesValidosParaGrupo.Count - 1);
                        int idx1 = indicesValidosParaGrupo[posLista];
                        int idx2 = indicesValidosParaGrupo[posLista + 1];

                        string h1 = horasPosibles[idx1];
                        string h2 = horasPosibles[idx2];

                        if (EsHoraLibre(horarioMatriz, checker, dia, h1, item.GrupoSolo) &&
                            EsHoraLibre(horarioMatriz, checker, dia, h2, item.GrupoSolo))
                        {
                            Asignar(horarioMatriz, horarioPlanoParaGuardar, dia, h1, item.Nombre);
                            Asignar(horarioMatriz, horarioPlanoParaGuardar, dia, h2, item.Nombre);

                            horasAsignadasPorDia[dia] += 2; // Actualizamos contador
                            horasRestantes -= 2;
                            bloqueAsignado = true;
                        }
                        intentos++;
                    }
                    if (!bloqueAsignado) break;
                }

                // -------------------------------------------------------------
                // FASE 2: HORAS SUELTAS (DISTRIBUIDAS)
                // -------------------------------------------------------------
                int intentosSueltos = 0;
                while (horasRestantes > 0 && intentosSueltos < 300)
                {
                    // Misma estrategia: buscar días menos cargados
                    var diasCandidatos = diasDisponibles
                            .OrderBy(d => horasAsignadasPorDia[d])
                            .ToList();

                    var dia = diasCandidatos[random.Next(diasCandidatos.Count > 1 ? 2 : 1)]; // Elegir entre los 2 menos cargados

                    int idx = indicesValidosParaGrupo[random.Next(indicesValidosParaGrupo.Count)];
                    string hora = horasPosibles[idx];

                    if (EsHoraLibre(horarioMatriz, checker, dia, hora, item.GrupoSolo))
                    {
                        Asignar(horarioMatriz, horarioPlanoParaGuardar, dia, hora, item.Nombre);
                        horasAsignadasPorDia[dia] += 1;
                        horasRestantes--;
                    }
                    intentosSueltos++;
                }
            }

            profesorActual.HorarioFinalOcupado = horarioPlanoParaGuardar;
            return horarioMatriz;
        }

        private void Asignar(Dictionary<string, Dictionary<string, string>> matriz, Dictionary<string, string> plano, string dia, string hora, string valor)
        {
            matriz[hora][dia] = valor;
            plano[$"{dia}|{hora}"] = valor;
        }

        private bool EsHoraLibre(Dictionary<string, Dictionary<string, string>> matriz, ConflictChecker checker, string dia, string hora, string grupo)
        {
            if (!string.IsNullOrEmpty(matriz[hora][dia])) return false;
            if (checker.EstaElGrupoOcupado(dia, hora, grupo)) return false;

            // Chequeo Bifurcación (9:30 vs 9:50)
            if (hora == "09:30 - 10:20" && !string.IsNullOrEmpty(matriz["09:50 - 10:40"][dia])) return false;
            if (hora == "09:50 - 10:40" && !string.IsNullOrEmpty(matriz["09:30 - 10:20"][dia])) return false;

            return true;
        }
    }
}