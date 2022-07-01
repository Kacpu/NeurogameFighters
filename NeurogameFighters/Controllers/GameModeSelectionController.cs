using NeurogameFighters.Commands;
using System.Windows.Input;

namespace NeurogameFighters.Controllers
{
    class GameModeSelectionController : ControllerBase
    {
        public ICommand ChangeControllerToMainMenuCommand { get; }
        public ICommand ChangeControllerToGameCommand { get; }
        public ICommand ChangeControllerToPlayerVsAiModeCommand { get; }
        public ICommand ChangeControllerToLearningModeCommand { get; }

        public GameModeSelectionController(ControllersStore controllersStore)
        {
            ChangeControllerToMainMenuCommand =
                new ChangeControllerCommand<MainMenuController>(controllersStore, () => new MainMenuController(controllersStore));

            ChangeControllerToGameCommand =
                new ChangeControllerCommand<PvPModeController>(controllersStore, () => new PvPModeController(controllersStore));

            ChangeControllerToPlayerVsAiModeCommand =
                new ChangeControllerCommand<PlayerVsAiModeController>(controllersStore, () => new PlayerVsAiModeController(controllersStore));

            ChangeControllerToLearningModeCommand =
                new ChangeControllerCommand<LearningModeController>(controllersStore, () => new LearningModeController(controllersStore));
        }
    }
}
