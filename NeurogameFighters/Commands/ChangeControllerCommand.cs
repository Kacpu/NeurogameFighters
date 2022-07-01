using NeurogameFighters.Controllers;
using System;

namespace NeurogameFighters.Commands
{
    class ChangeControllerCommand<ViewController> : CommandBase
        where ViewController : ControllerBase
    {
        private readonly ControllersStore controllersStore;
        private readonly Func<ViewController> changeController;

        public ChangeControllerCommand(ControllersStore controllersStore, Func<ViewController> changeController)
        {
            this.controllersStore = controllersStore;
            this.changeController = changeController;
        }

        public override void Execute(object parameter)
        {
            controllersStore.CurrentController = changeController();
        }
    }
}
