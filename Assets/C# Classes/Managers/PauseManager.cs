using C__Classes.SceneManagement;
using C__Classes.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace C__Classes.Managers
{
    public class PauseManager : SingletonPersistant<PauseManager>
    {
        public bool IsPaused { get; private set; } = false;

        [SerializeField] private GameObject pausePanel;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private string mainMenuSceneName = MainMenuReturn.DefaultMainMenuSceneName;

        private InputActionMap _playerMap;
        private InputActionMap _uiMap;
        private InputAction _pauseAction;

        private float previousTimeScale = 1f;

        private void Start()
        {
            if (playerInput == null)
            {
                playerInput = FindFirstObjectByType<PlayerInput>();
            }

            if (playerInput == null || playerInput.actions == null)
            {
                Debug.LogWarning("[PauseManager] PlayerInput or InputActionAsset is missing.");
                return;
            }

            _playerMap = playerInput.actions.FindActionMap("Player", true);
            _uiMap = playerInput.actions.FindActionMap("UI", true);
            _pauseAction = _uiMap.FindAction("Pause", true);

            _uiMap.Enable();
            _pauseAction.performed += TogglePause;
        }

        public void TogglePause(InputAction.CallbackContext context)
        {
            if (IsPaused) ResumeGame();
            else PauseGame();
        }

        public void PauseGame()
        {
            ApplyPauseState(true);
        }

        public void ResumeGame()
        {
            ApplyPauseState(false);
        }

        public void ReturnToMainMenu()
        {
            ApplyPauseState(false);
            MainMenuReturn.LoadMainMenu(mainMenuSceneName);
        }

        private void ApplyPauseState(bool paused)
        {
            IsPaused = paused;

            if (paused)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                if (pausePanel != null) pausePanel.SetActive(true);

                _playerMap?.Disable();
            }
            else
            {
                Time.timeScale = previousTimeScale != 0f ? previousTimeScale : 1f;
                if (pausePanel != null) pausePanel.SetActive(false);

                _playerMap?.Enable();
            }
        }

        private void OnDisable()
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }
        }

        private void OnDestroy()
        {
            if (_pauseAction != null)
            {
                _pauseAction.performed -= TogglePause;
            }
        }
    }
}
