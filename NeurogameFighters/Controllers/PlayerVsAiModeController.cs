using NeurogameFighters.Commands;
using NeurogameFighters.Models;
using NeurogameFighters.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using NeurogameFighters.Controllers;

namespace NeurogameFighters.Controllers
{
    class PlayerVsAiModeController : ControllerBase
    {
        private readonly ControllersStore controllersStore;
        public ICommand ChangeControllerToModeSelectionCommand { get; }
        public ICommand OnPreviewKeyDownCommand { get; }
        public ICommand OnPreviewKeyUpCommand { get; }

        private readonly Game game;
        private readonly bool timeLimitedMode = false;
        private readonly int shootIntervalTime = 1000; //ms
        private readonly int fighterLife = 10;
        public Fighter Player1 { get; }
        public Fighter Player2 { get; }
        public JetView Player1View { get; }
        public JetView Player2View { get; }
        private readonly int networkInputSize = 5;
        private readonly int networkOutputSize = 5;

        private readonly KeyboardController keyboardController;
        private AIController AIController;

        public PlayerVsAiModeController(ControllersStore controllersStore)
        {
            this.controllersStore = controllersStore;
            ChangeControllerToModeSelectionCommand = new RelayCommand<object>(OnReturnButton);

            Player1 = new Fighter(1, 0, 30, 270);
            Player2 = new Fighter(2, 180, 1190, 270);
            game = new Game(Player1, Player2, timeLimitedMode, fighterLife, shootIntervalTime);

            Player1View = new JetView(Player1.Id);
            Player2View = new JetView(Player2.Id);

            keyboardController = new KeyboardController(Player1, Key.A, Key.D, Key.W, Key.S, Key.Space);
            OnPreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(OnPreviewKeyDown);
            OnPreviewKeyUpCommand = new RelayCommand<KeyEventArgs>(OnPreviewKeyUp);

            ReadNeuralNetworkFromFile(Player2, Player1);

            Player1.PropertyChanged += OnJetPropertyChanged;
            Player2.PropertyChanged += OnJetPropertyChanged;

            game.GameOver += OnGameOver;
            game.StartNewGame();
            AIController.StartController();
        }

        private void OnJetPropertyChanged()
        {
            OnPropertyChanged(nameof(Player1));
            OnPropertyChanged(nameof(Player2));
        }

        private void OnPreviewKeyDown(KeyEventArgs e)
        {
            keyboardController.OnPreviewKeyDown(e);
        }

        private void OnPreviewKeyUp(KeyEventArgs e)
        {
            keyboardController.OnPreviewKeyUp(e);
        }

        private void OnGameOver()
        {
            int winnerID = Player1.Life > Player2.Life ? Player1.Id : Player2.Id;
            controllersStore.CurrentController =
                new GameOverController(controllersStore, () => new PlayerVsAiModeController(controllersStore), winnerID);
        }

        private void OnReturnButton(object t)
        {
            game.EndGame();
            controllersStore.CurrentController = new GameModeSelectionController(controllersStore);
        }

        private void ReadNeuralNetworkFromFile(Fighter AIPLayer, Fighter opponent)
        {
            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Neurogame");
            string fighterPath = Path.Combine(directoryPath, "bestFighters.csv");

            AIPLayer.SetNeuralNetwork(networkInputSize, networkOutputSize);

            if (File.Exists(fighterPath) && AIPLayer.NeuralNetwork.ReadDNAFromFile(fighterPath))
            {
                AIController = new NeuralNetworkController(AIPLayer, opponent);
            }
            else
            {
                AIController = new DeterministicController(AIPLayer, opponent);
            }
        }
    }
}
