using NeurogameFighters.Commands;
using System.Windows.Input;

namespace NeurogameFighters.Controllers
{
    class BlackScreenModeController : ControllerBase
    {
        public ICommand ChangeControllerToLearningModeCommand { get; }
        public int GenerationNumber { get; set; }

        public BlackScreenModeController(ControllersStore controllersStore, LearningModeController learningModeController)
        {
            ChangeControllerToLearningModeCommand =
                new ChangeControllerCommand<LearningModeController>(controllersStore, () => learningModeController);
        }

        public void ChangeGenerationNumber(int generationNumber)
        {
            GenerationNumber = generationNumber;
            OnPropertyChanged(nameof(GenerationNumber));
        }
    }
}
