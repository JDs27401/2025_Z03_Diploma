using DG.Tweening;
using C__Classes.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace C__Classes.Managers
{
    public class GameEndScreenManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup screenGroup;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private string mainMenuSceneName = MainMenuReturn.DefaultMainMenuSceneName;
        [SerializeField] private PlayerInput playerInput;

        private InputActionMap playerMap;

        private void Awake()
        {
            if (screenGroup != null)
            {
                screenGroup.alpha = 0f;
                screenGroup.interactable = false;
                screenGroup.blocksRaycasts = false;
            }

            if (playerInput == null)
            {
                playerInput = FindFirstObjectByType<PlayerInput>();
            }
        }

        public void Show()
        {
            if (playerInput != null)
            {
                playerMap = playerInput.actions.FindActionMap("Player", true);
                playerMap?.Disable();
            }

            if (screenGroup != null)
            {
                screenGroup.blocksRaycasts = true;
                screenGroup.interactable = true;
                screenGroup.DOFade(1f, fadeDuration).SetUpdate(true);
            }

            Time.timeScale = 0f;
        }

        public void GoToMainMenu()
        {
            DOTween.Kill(screenGroup);
            MainMenuReturn.LoadMainMenu(mainMenuSceneName);
        }
    }
}
