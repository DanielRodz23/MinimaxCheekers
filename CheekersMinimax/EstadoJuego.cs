using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheekersMinimax
{
    public class EstadoJuego
    {
        public int[,] Tablero { get; set; } // Matriz que representa el tablero
        public int Turno { get; set; } // 1 para jugador 1, -1 para jugador 2

        public EstadoJuego(int size)
        {
            Tablero = new int[size, size];
            Turno = 1; // El jugador 1 comienza
        }

        // Método para clonar el estado
        public EstadoJuego Clonar()
        {
            var nuevoEstado = new EstadoJuego(Tablero.GetLength(0))
            {
                Turno = this.Turno
            };
            Array.Copy(this.Tablero, nuevoEstado.Tablero, this.Tablero.Length);
            return nuevoEstado;
        }

        public List<EstadoJuego> GenerarSucesores()
        {
            var sucesores = new List<EstadoJuego>();
            for (int i = 0; i < Tablero.GetLength(0); i++)
            {
                for (int j = 0; j < Tablero.GetLength(1); j++)
                {
                    if (Tablero[i, j] == Turno)
                    {
                        // Calcula los movimientos válidos
                        var movimientos = CalcularMovimientosValidos(i, j);
                        foreach (var movimiento in movimientos)
                        {
                            var nuevoEstado = Clonar();
                            nuevoEstado.Tablero[movimiento.Item1, movimiento.Item2] = Turno;
                            nuevoEstado.Tablero[i, j] = 0; // Vacía la casilla original
                            nuevoEstado.Turno *= -1; // Cambia el turno
                            sucesores.Add(nuevoEstado);
                        }
                    }
                }
            }
            return sucesores;
        }

        private List<(int, int)> CalcularMovimientosValidos(int x, int y)
        {
            var movimientos = new List<(int, int)>();

            // Ejemplo: movimientos diagonales simples
            int[] dx = { -1, -1, 1, 1 };
            int[] dy = { -1, 1, -1, 1 };

            for (int i = 0; i < dx.Length; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];
                if (nx >= 0 && ny >= 0 && nx < Tablero.GetLength(0) && ny < Tablero.GetLength(1) && Tablero[nx, ny] == 0)
                {
                    movimientos.Add((nx, ny));
                }
            }

            return movimientos;
        }

        public int EvaluarHeuristica()
        {
            int puntaje = 0;
            for (int i = 0; i < Tablero.GetLength(0); i++)
            {
                for (int j = 0; j < Tablero.GetLength(1); j++)
                {
                    if (Tablero[i, j] == 1) puntaje += 10; // Piezas del jugador 1
                    else if (Tablero[i, j] == -1) puntaje -= 10; // Piezas del jugador 2
                }
            }
            return puntaje;
        }

        public int Minimax(int profundidad, bool esMaximizador)
        {
            if (profundidad == 0 || EsEstadoFinal())
                return EvaluarHeuristica();

            if (esMaximizador)
            {
                int maxEval = int.MinValue;
                foreach (var sucesor in GenerarSucesores())
                {
                    int eval = sucesor.Minimax(profundidad - 1, false);
                    maxEval = Math.Max(maxEval, eval);
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var sucesor in GenerarSucesores())
                {
                    int eval = sucesor.Minimax(profundidad - 1, true);
                    minEval = Math.Min(minEval, eval);
                }
                return minEval;
            }
        }

        private bool EsEstadoFinal()
        {
            return !this.Tablero.Cast<int>().Contains(1) || // No hay piezas del jugador 1
                   !this.Tablero.Cast<int>().Contains(-1) || // No hay piezas del jugador 2
                   !this.GenerarSucesores().Any(); // No hay movimientos posibles
        }



    }

}
