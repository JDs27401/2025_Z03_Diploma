using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // Wymagane namespace dla DOTween

public class DeathScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup deathScreenGroup;
    
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        if (deathScreenGroup != null)
        {
            // Resetujemy stan początkowy ekranu
            deathScreenGroup.alpha = 0f;
            deathScreenGroup.interactable = false;
            deathScreenGroup.blocksRaycasts = false;
        }
    }

    public void ShowDeathScreen()
    {
        if (deathScreenGroup != null)
        {
            // Włączamy możliwość kliknięcia w przyciski na ekranie śmierci
            deathScreenGroup.blocksRaycasts = true;
            deathScreenGroup.interactable = true;

            // Animacja DOTween
            // .SetUpdate(true) sprawia, że animacja zignoruje Time.timeScale = 0
            // Jest to kluczowe, jeśli w momencie śmierci zatrzymujesz czas gry.
            deathScreenGroup.DOFade(1f, fadeDuration).SetUpdate(true);
        }
    }

    public void GoToMainMenu()
    {
        // Przywracamy normalny upływ czasu przed załadowaniem menu
        if (Time.timeScale < 1f)
        {
            Time.timeScale = 1f;
        }
        
        // Dobrą praktyką przy zmianie sceny jest ubicie wszystkich aktywnych tweenów,
        // aby uniknąć błędów rzucanych przez obiekty, które zostały zniszczone.
        DOTween.Kill(deathScreenGroup); 
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
}