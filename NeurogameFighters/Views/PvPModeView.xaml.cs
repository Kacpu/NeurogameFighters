using System.Windows;
using System.Windows.Controls;

namespace NeurogameFighters.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class PvPModeView : UserControl
    {
        public PvPModeView()
        {
            InitializeComponent();
        }

        private void GameView_Loaded(object sender, RoutedEventArgs e)
        {
            //Loaded="GameView_Loaded"/.xaml
            this.Focus();
        }



        /*
        private void GameView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //IsVisibleChanged="GameView_IsVisibleChanged"/.xaml
            if (this.Visibility == Visibility.Visible)
            {
                this.Focus();
            }
        }
        */
    }
}
