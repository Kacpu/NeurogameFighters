using System;

namespace NeurogameFighters.Controllers
{
    class ControllersStore
    {
        public event Action CurrentControllerChanged;

        private ControllerBase _currentController;

        public ControllerBase CurrentController
        {
            get => _currentController;
            set
            {
                _currentController = value;
                OnCurrentControllerChanged();
            }
        }

        private void OnCurrentControllerChanged()
        {
            CurrentControllerChanged?.Invoke();
        }
    }
}
