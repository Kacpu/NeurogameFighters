namespace NeurogameFighters.Controllers
{
    class MainWindowController : ControllerBase
    {
        private readonly ControllersStore controllersStore;

        public ControllerBase CurrentController => controllersStore.CurrentController;

        public MainWindowController(ControllersStore controllersStore)
        {
            this.controllersStore = controllersStore;
            this.controllersStore.CurrentControllerChanged += OnCurrentControllerChanged;
        }

        private void OnCurrentControllerChanged()
        {
            OnPropertyChanged(nameof(CurrentController));
        }
    }
}
