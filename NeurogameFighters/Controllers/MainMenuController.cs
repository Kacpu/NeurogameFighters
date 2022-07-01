using NeurogameFighters.Commands;
using System.Windows.Input;

namespace NeurogameFighters.Controllers
{
    class MainMenuController : ControllerBase
    {
        public ICommand ChangeControllerToGameModeSelectionCommand { get; }
        public ICommand ExitCommand { get; }

        public MainMenuController(ControllersStore controllersStore)
        {
            ChangeControllerToGameModeSelectionCommand =
                new ChangeControllerCommand<GameModeSelectionController>(controllersStore, () => new GameModeSelectionController(controllersStore));

            ExitCommand = new RelayCommand<object>(OnExit);
        }

        private void OnExit(object t)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
