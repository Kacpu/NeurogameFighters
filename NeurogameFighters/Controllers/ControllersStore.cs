using System;

namespace NeurogameFighters.Controllers
{
    class ControllersStore
    {
        public event Action CurrentControllerChanged;

        private ControllerBase currentController;

        public ControllerBase CurrentController
        {
            get => currentController;
            set
            {
                currentController = value;
                OnCurrentControllerChanged();
            }
        }

        private void OnCurrentControllerChanged()
        {
            CurrentControllerChanged?.Invoke();
        }
    }
}
