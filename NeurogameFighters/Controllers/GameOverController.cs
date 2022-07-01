using NeurogameFighters.Commands;
using System;
using System.Windows.Input;

namespace NeurogameFighters.Controllers
{
    class GameOverController : ControllerBase
    {
        public ICommand ChangeControllerToGameCommand { get; }
        public ICommand ChangeControllerToMainMenuCommand { get; }
        public string WinnerName { get; set; }

        public GameOverController(ControllersStore controllersStore, Func<ControllerBase> changeController, int winnerID)
        {
            ChangeControllerToGameCommand =
                new ChangeControllerCommand<ControllerBase>(controllersStore, changeController);
            ChangeControllerToMainMenuCommand =
                new ChangeControllerCommand<MainMenuController>(controllersStore, () => new MainMenuController(controllersStore));

            if (winnerID == 1)
            {
                WinnerName = "pierwszy";
            }
            else if (winnerID == 2)
            {
                WinnerName = "drugi";
            }
        }
    }
}