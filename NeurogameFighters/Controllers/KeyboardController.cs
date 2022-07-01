using NeurogameFighters.Models;
using System.Windows.Input;

namespace NeurogameFighters.Controllers
{
    class KeyboardController
    {
        private readonly Fighter jet;
        private readonly Key left;
        private readonly Key right;
        private readonly Key up;
        private readonly Key down;
        private readonly Key shoot;

        public KeyboardController(Fighter jet, Key left, Key right, Key up, Key down, Key shoot)
        {
            this.jet = jet;
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
            this.shoot = shoot;
        }

        public void OnPreviewKeyDown(KeyEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Key);

            if (e.IsRepeat)
            {
                // Ignore key repeats...let the timer handle that
                e.Handled = true;
                return;
            }

            if (e.Key == left)
            {
                jet.TurningLeft = true;
            }
            if (e.Key == right)
            {
                jet.TurningRight = true;
            }
            if (e.Key == up)
            {
                jet.MovingForward = true;
            }
            if (e.Key == down)
            {
                jet.MovingBack = true;
            }
            if (e.Key == shoot)
            {
                jet.Shooting = true;
            }

            e.Handled = true;
        }

        public void OnPreviewKeyUp(KeyEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Key);

            if (e.IsRepeat)
            {
                // Ignore key repeats...let the timer handle that
                e.Handled = true;
                return;
            }

            if (e.Key == left)
            {
                jet.TurningLeft = false;
            }
            if (e.Key == right)
            {
                jet.TurningRight = false;
            }
            if (e.Key == up)
            {
                jet.MovingForward = false;
            }
            if (e.Key == down)
            {
                jet.MovingBack = false;
            }
            if (e.Key == shoot)
            {
                jet.Shooting = false;
            }

            e.Handled = true;
        }
    }
}
