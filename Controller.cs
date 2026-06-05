using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;

namespace Mario_Unbound
{
    public class Controller
    {
        public class PS4Controller
        {
            private RawGameController _controller;

            public bool IsConnected => _controller != null;

            // Arrays für die Rohdaten
            private bool[] _buttons;
            private GameControllerSwitchPosition[] _switches;
            private double[] _axes;

            public PS4Controller()
            {
                // Abonnieren der Windows-Events für angeschlossene/getrennte Controller
                RawGameController.RawGameControllerAdded += OnControllerAdded;
                RawGameController.RawGameControllerRemoved += OnControllerRemoved;

                // Prüfen, ob schon beim Start ein Controller da ist
                _controller = RawGameController.RawGameControllers.FirstOrDefault();
                if (_controller != null)
                {
                    InitializeArrays();
                }
            }

            private void OnControllerAdded(object sender, RawGameController e)
            {
                if (_controller == null)
                {
                    _controller = e;
                    InitializeArrays();
                }
            }

            private void OnControllerRemoved(object sender, RawGameController e)
            {
                if (_controller == e)
                {
                    _controller = null;
                }
            }

            private void InitializeArrays()
            {
                // Die Arrays müssen genau die Größe haben, die der Controller Hardware-seitig meldet
                _buttons = new bool[_controller.ButtonCount];
                _switches = new GameControllerSwitchPosition[_controller.SwitchCount];
                _axes = new double[_controller.AxisCount];
            }

            // Liest die aktuellen Werte aus und gibt sie zurück
            public (bool[] Buttons, double[] Axes, GameControllerSwitchPosition[] Switches)? GetState()
            {
                if (_controller == null) return null;

                // Füllt die Arrays mit den aktuellen Daten des Controllers
                _controller.GetCurrentReading(_buttons, _switches, _axes);

                return (_buttons, _axes, _switches);
            }
        }

    }
}
