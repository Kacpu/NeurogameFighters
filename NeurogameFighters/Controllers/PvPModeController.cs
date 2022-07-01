using NeurogameFighters.Commands;
using NeurogameFighters.Models;
using NeurogameFighters.Views;
using System.Windows.Input;

namespace NeurogameFighters.Controllers
{
    class PvPModeController : ControllerBase
    {
        private readonly ControllersStore controllersStore;
        public ICommand ChangeControllerToModeSelectionCommand { get; }
        public ICommand OnPreviewKeyDownCommand { get; }
        public ICommand OnPreviewKeyUpCommand { get; }

        private readonly Game game;
        private bool timeLimitedMode = false;
        private readonly int shootIntervalTime = 1000; //ms
        private readonly int fighterLife = 10;
        public Fighter Player1 { get; }
        public Fighter Player2 { get; }
        public JetView Player1View { get; }
        public JetView Player2View { get; }

        private readonly KeyboardController keyboardController1;
        private readonly KeyboardController keyboardController2;

        public PvPModeController(ControllersStore controllersStore)
        {
            this.controllersStore = controllersStore;
            ChangeControllerToModeSelectionCommand = new RelayCommand<object>(OnReturnButton);

            Player1 = new Fighter(1, 0, 30, 270);
            Player2 = new Fighter(2, 180, 1190, 270);
            game = new Game(Player1, Player2, timeLimitedMode ,fighterLife, shootIntervalTime);

            Player1View = new JetView(Player1.Id);
            Player2View = new JetView(Player2.Id);

            keyboardController1 = new KeyboardController(Player1, Key.A, Key.D, Key.W, Key.S, Key.Space);
            keyboardController2 = new KeyboardController(Player2, Key.Left, Key.Right, Key.Up, Key.Down, Key.K);
            OnPreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(OnPreviewKeyDown);
            OnPreviewKeyUpCommand = new RelayCommand<KeyEventArgs>(OnPreviewKeyUp);

            Player1.PropertyChanged += OnJetPropertyChanged;
            Player2.PropertyChanged += OnJetPropertyChanged;

            game.GameOver += OnGameOver;
            game.StartNewGame();
        }

        private void OnJetPropertyChanged()
        {
            OnPropertyChanged(nameof(Player1));
            OnPropertyChanged(nameof(Player2));
        }

        private void OnPreviewKeyDown(KeyEventArgs e)
        {
            keyboardController1.OnPreviewKeyDown(e);
            keyboardController2.OnPreviewKeyDown(e);
        }

        private void OnPreviewKeyUp(KeyEventArgs e)
        {
            keyboardController1.OnPreviewKeyUp(e);
            keyboardController2.OnPreviewKeyUp(e);
        }

        private void OnGameOver()
        {
            int winnerID = Player1.Life > Player2.Life ? Player1.Id : Player2.Id;
            controllersStore.CurrentController =
                new GameOverController(controllersStore, () => new PvPModeController(controllersStore), winnerID);
        }

        private void OnReturnButton(object t)
        {
            game.EndGame();
            controllersStore.CurrentController = new GameModeSelectionController(controllersStore);
        }
    }
}
