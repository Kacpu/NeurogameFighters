using System.Windows;
using System.Windows.Controls;

namespace NeurogameFighters.Views
{
    /// <summary>
    /// Interaction logic for PlayerVsAiModeView.xaml
    /// </summary>
    public partial class PlayerVsAiModeView : UserControl
    {
        public PlayerVsAiModeView()
        {
            InitializeComponent();
        }

        private void PlayerVsAiModeView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }
    }
}
