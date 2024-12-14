using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CheekersMinimax
{

    public partial class MainWindow : Window
    {
        private EstadoJuego estadoActual;

        public MainWindow()
        {
            InitializeComponent();
            InicializarTablero(8); // Por ejemplo, un tablero de 8x8
            ColocarPiezasIniciales();
        }

        private (int, int)? seleccionInicial = null;

        private void Casilla_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            var (fila, columna) = ((int, int))boton.Tag;

            if (seleccionInicial == null) // Seleccionar una pieza
            {
                if (estadoActual.Tablero[fila, columna] == estadoActual.Turno)
                {
                    seleccionInicial = (fila, columna);
                    boton.BorderBrush = Brushes.Yellow; // Resaltar selección
                    boton.BorderThickness = new Thickness(3);
                }
            }
            else // Mover la pieza seleccionada
            {
                var (filaInicio, columnaInicio) = seleccionInicial.Value;

                if (EsMovimientoValido(filaInicio, columnaInicio, fila, columna))
                {
                    MoverPieza(filaInicio, columnaInicio, fila, columna);
                    seleccionInicial = null;
                    ActualizarTablero();

                    // Turno de la IA
                    TurnoIA();
                }
                else
                {
                    seleccionInicial = null; // Reiniciar selección
                    ActualizarTablero();
                }
            }
        }

        private void ActualizarTablero()
        {
            foreach (var elemento in TableroGrid.Children)
            {
                if (elemento is Button boton)
                {
                    var (fila, columna) = ((int, int))boton.Tag;
                    int valor = estadoActual.Tablero[fila, columna];

                    boton.Content = valor switch
                    {
                        1 => "⚪", // Pieza del jugador 1
                        -1 => "⚫", // Pieza del jugador 2
                        _ => null, // Casilla vacía
                    };

                    boton.BorderBrush = Brushes.Transparent; // Resetear bordes
                }
            }
        }

        private void MoverPieza(int filaInicio, int columnaInicio, int filaDestino, int columnaDestino)
        {
            estadoActual.Tablero[filaDestino, columnaDestino] = estadoActual.Tablero[filaInicio, columnaInicio];
            estadoActual.Tablero[filaInicio, columnaInicio] = 0;
            estadoActual.Turno *= -1; // Cambiar turno
        }

        private bool EsMovimientoValido(int filaInicio, int columnaInicio, int filaDestino, int columnaDestino)
        {
            // Validar que el movimiento sea dentro del tablero y la casilla destino esté vacía
            if (filaDestino >= 0 && filaDestino < estadoActual.Tablero.GetLength(0) &&
                columnaDestino >= 0 && columnaDestino < estadoActual.Tablero.GetLength(1) &&
                estadoActual.Tablero[filaDestino, columnaDestino] == 0)
            {
                // Validar movimiento diagonal simple
                int dx = Math.Abs(filaDestino - filaInicio);
                int dy = Math.Abs(columnaDestino - columnaInicio);
                return dx == 1 && dy == 1;
            }
            return false;
        }

        private async void TurnoIA()
        {
            TableroGrid.IsEnabled = false;
            await Task.Delay(500); // Añadir un breve retraso para simular el tiempo de pensamiento de la IA
            var mejorMovimiento = ObtenerMejorMovimiento(estadoActual, 3); // Profundidad de 3
            if (mejorMovimiento != null)
            {
                estadoActual = mejorMovimiento;
                ActualizarTablero();
            }
            TableroGrid.IsEnabled = true;
        }


        private EstadoJuego ObtenerMejorMovimiento(EstadoJuego estado, int profundidad)
        {
            int mejorValor = int.MinValue;
            EstadoJuego mejorEstado = null;

            foreach (var sucesor in estado.GenerarSucesores())
            {
                // Usamos el método Minimax de la clase EstadoJuego
                int valor = sucesor.Minimax(profundidad - 1, false);
                if (valor > mejorValor)
                {
                    mejorValor = valor;
                    mejorEstado = sucesor;
                }
            }
            return mejorEstado;
        }


        private bool EsEstadoFinal()
        {
            return !estadoActual.Tablero.Cast<int>().Contains(1) ||
                   !estadoActual.Tablero.Cast<int>().Contains(-1) ||
                   !estadoActual.GenerarSucesores().Any();
        }



        private void InicializarTablero(int size)
        {
            estadoActual = new EstadoJuego(size);

            for (int i = 0; i < size; i++)
            {
                TableroGrid.RowDefinitions.Add(new RowDefinition());
                TableroGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    var casilla = new Button
                    {
                        Background = (row + col) % 2 == 0 ? Brushes.White : Brushes.Black,
                        Tag = (row, col), // Guardar posición en la casilla
                    };

                    casilla.Click += Casilla_Click; // Manejar clics del usuario

                    Grid.SetRow(casilla, row);
                    Grid.SetColumn(casilla, col);
                    TableroGrid.Children.Add(casilla);
                }
            }
        }
        private void ColocarPiezasIniciales()
        {
            for (int i = 0; i < estadoActual.Tablero.GetLength(0); i++)
            {
                for (int j = 0; j < estadoActual.Tablero.GetLength(1); j++)
                {
                    if ((i + j) % 2 != 0)
                    {
                        if (i < 3) estadoActual.Tablero[i, j] = -1; // Piezas del jugador 2
                        else if (i >= estadoActual.Tablero.GetLength(0) - 3) estadoActual.Tablero[i, j] = 1; // Piezas del jugador 1
                    }
                }
            }
            ActualizarTablero();
        }




    }

}