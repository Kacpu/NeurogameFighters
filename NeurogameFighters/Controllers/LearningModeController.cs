using NeurogameFighters.Commands;
using NeurogameFighters.Models;
using NeurogameFighters.Views;
using System.Windows.Input;

namespace NeurogameFighters.Controllers
{
    class LearningModeController : ControllerBase
    {
        private readonly ControllersStore controllersStore;
        public ICommand ChangeControllerToModeSelectionCommand { get; }
        public ICommand ChangeControllerToBlackScreenModeCommand { get; }
        private BlackScreenModeController blackScreenModeController;

        public BattleCamp BattleCamp { get; }
        public Fighter Player1 { get; set; }
        public Fighter Player2 { get; set; }
        public JetView Player1View { get; }
        public JetView Player2View { get; }
        private readonly int populationSize = 100;
        private readonly int networkInputSize = 5;
        private readonly int networkOutputSize = 5;
        private readonly int fighterLife = 3;
        private readonly int shootIntervalTime = 1000; //ms

        public LearningModeController(ControllersStore controllersStore)
        {
            this.controllersStore = controllersStore;
            ChangeControllerToModeSelectionCommand = new RelayCommand<object>(OnReturnButton);
            blackScreenModeController = new BlackScreenModeController(controllersStore, this);
            ChangeControllerToBlackScreenModeCommand =
                new ChangeControllerCommand<BlackScreenModeController>(controllersStore, () => blackScreenModeController);

            BattleCamp = new BattleCamp(populationSize, networkInputSize, networkOutputSize, fighterLife, shootIntervalTime);
            BattleCamp.PropertyChanged += OnBattleCampPropertyChanged;

            Player1 = BattleCamp.Population1.Fighters[BattleCamp.FightingPair];
            //Player2 = BattleCamp.Population2.Fighters[BattleCamp.FightingPair];
            Player2 = BattleCamp.DeterministicFighters[BattleCamp.FightingPair];

            Player1View = new JetView(Player1.Id);
            Player2View = new JetView(Player2.Id);

            Player1.PropertyChanged += OnJetPropertyChanged;
            Player2.PropertyChanged += OnJetPropertyChanged;

            BattleCamp.StartTraining();
        }

        private void OnJetPropertyChanged()
        {
            OnPropertyChanged(nameof(Player1));
            OnPropertyChanged(nameof(Player2));
        }

        private void OnBattleCampPropertyChanged()
        {
            Player1 = BattleCamp.Population1.Fighters[BattleCamp.FightingPair];
            //Player2 = BattleCamp.Population2.Fighters[BattleCamp.FightingPair];
            Player2 = BattleCamp.DeterministicFighters[BattleCamp.FightingPair];
            Player1.PropertyChanged += OnJetPropertyChanged;
            Player2.PropertyChanged += OnJetPropertyChanged;
            OnJetPropertyChanged();

            OnPropertyChanged(nameof(BattleCamp));
            blackScreenModeController.ChangeGenerationNumber(BattleCamp.GenerationNumber);
        }

        private void OnReturnButton(object t)
        {
            BattleCamp.EndTraining();
            controllersStore.CurrentController = new GameModeSelectionController(controllersStore);
        }
    }
}
