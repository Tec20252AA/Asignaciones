using System;
using System.Collections.Generic;

namespace Asignaciones
{
	class Program
	{
		// Paso 1: Definir la matriz de costos
		static int[,] costMatrix = {
			{ 9, 2, 7, 8 },
			{ 6, 4, 3, 7 },
			{ 5, 8, 1, 8 },
			{ 7, 6, 9, 4 }
		};

		static int n = 4; // Número de tareas/trabajadores
		static int bestCost = int.MaxValue; // Mejor costo encontrado
		static int[] bestAssignment = new int[n]; // Mejor asignación encontrada

	// Listas para almacenar ramas exploradas y podadas junto con su suma de costos
	static List<(string rama, int suma)> ramasExploradas = new List<(string, int)>();
	static List<(string rama, int suma)> ramasPodadas = new List<(string, int)>();

		static void Main(string[] args)
		{
			// Paso 2: Iniciar la exploración del árbol de asignaciones
			bool[] assigned = new bool[n]; // Para marcar trabajadores asignados
			int[] currentAssignment = new int[n]; // Asignación actual
			BranchAndBound(0, 0, assigned, currentAssignment, "", 0);

			// Mostrar la mejor solución encontrada
			Console.WriteLine("\nMejor costo total: " + bestCost);
			Console.WriteLine("Mejor asignación:");
			for (int i = 0; i < n; i++)
				Console.WriteLine($"Tarea {i} -> Trabajador {bestAssignment[i]}");

			// Mostrar el resumen gráfico de la poda
			MostrarPodaGrafica();
		}

		// Paso 3, 4 y 5: Algoritmo Branch and Bound
	static void BranchAndBound(int task, int currentCost, bool[] assigned, int[] currentAssignment, string rama, int suma)
		{
			// Si todas las tareas han sido asignadas
			if (task == n)
			{
				// Si el costo actual es mejor que el mejor encontrado
				if (currentCost < bestCost)
				{
					bestCost = currentCost;
					Array.Copy(currentAssignment, bestAssignment, n);
					Console.WriteLine($"\nNueva mejor solución encontrada con costo {bestCost}");
				}
				ramasExploradas.Add((rama + " ✔", suma)); // Marcar rama como explorada exitosa con suma
				return;
			}

			// Paso 3: Calcular cota inferior
			int lowerBound = currentCost + CalculateLowerBound(task, assigned);
			Console.WriteLine($"Explorando tarea {task}, costo actual {currentCost}, cota inferior {lowerBound}, mejor costo {bestCost}");

			// Paso 5: Podar ramas
			if (lowerBound >= bestCost)
			{
				Console.WriteLine($"PODA: Se poda la rama en tarea {task} porque la cota {lowerBound} >= mejor costo {bestCost}");
				ramasPodadas.Add((rama + " X", suma)); // Marcar rama como podada con suma
				return;
			}

			// Paso 2: Explorar asignaciones posibles para la tarea actual
			for (int worker = 0; worker < n; worker++)
			{
				if (!assigned[worker]) // Si el trabajador no ha sido asignado
				{
					assigned[worker] = true; // Asignar trabajador
					currentAssignment[task] = worker; // Registrar asignación
					Console.WriteLine($"Asignando tarea {task} al trabajador {worker}");
					BranchAndBound(task + 1, currentCost + costMatrix[task, worker], assigned, currentAssignment, rama + $"[{task}->{worker}] ", suma + costMatrix[task, worker]); // Recursión
					assigned[worker] = false; // Desasignar para explorar otras opciones
				}
			}
		}

		// Calcula la cota inferior sumando el menor costo posible para cada tarea restante
		static int CalculateLowerBound(int task, bool[] assigned)
		{
			int bound = 0;
			Console.Write($"Cálculo de cota inferior para tareas restantes: ");
			for (int t = task; t < n; t++)
			{
				int minCost = int.MaxValue;
				int minWorker = -1;
				for (int w = 0; w < n; w++)
				{
					if (!assigned[w] && costMatrix[t, w] < minCost)
					{
						minCost = costMatrix[t, w];
						minWorker = w;
					}
				}
				bound += minCost;
				Console.Write($"Tarea {t} -> Trabajador {minWorker} (costo {minCost}) ");
			}
			Console.WriteLine($"| Suma mínima: {bound}");
			return bound;
		}

		// Método para mostrar gráficamente la poda del árbol
		static void MostrarPodaGrafica()
		{
			Console.WriteLine("\nResumen gráfico de ramas exploradas y podadas:");
			Console.WriteLine("✔ = Rama exitosa, X = Rama podada");
			Console.WriteLine("------------------------------------");
			foreach (var (rama, suma) in ramasExploradas)
				Console.WriteLine(rama + $" Suma: {suma}");
			foreach (var (rama, suma) in ramasPodadas)
				Console.WriteLine(rama + $" Suma: {suma}");
		}
	}
}
