using C__Classes.SceneManagement;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup deathScreenGroup;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private string mainMenuSceneName = MainMenuReturn.DefaultMainMenuSceneName;
    [SerializeField] private PlayerInput playerInput;

    private InputActionMap _playerMap;

    private void Start()
    {
        if (deathScreenGroup != null)
        {
            deathScreenGroup.alpha = 0f;
            deathScreenGroup.interactable = false;
            deathScreenGroup.blocksRaycasts = false;
        }

        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>();
        }

        _playerMap = playerInput != null ? playerInput.actions.FindActionMap("Player", true) : null;
    }

    public void ShowDeathScreen()
    {
        if (deathScreenGroup != null)
        {
            deathScreenGroup.blocksRaycasts = true;
            deathScreenGroup.interactable = true;
            deathScreenGroup.DOFade(1f, fadeDuration).SetUpdate(true);
        }

        _playerMap?.Disable();
        Time.timeScale = 0f;
    }

    public void GoToMainMenu()
    {
        DOTween.Kill(deathScreenGroup);
        MainMenuReturn.LoadMainMenu(mainMenuSceneName);
    }
}
