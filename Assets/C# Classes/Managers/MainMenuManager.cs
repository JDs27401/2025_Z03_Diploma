using C__Classes.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // WYMAGANE: Dodanie przestrzeni nazw DOTween

namespace C__Classes.Managers
{
    public class MainMenuManager : SingletonNonPersistant<MainMenuManager>
    {
        public string Seed { get; private set; }
        
        [Header("UI Panels")]
        [SerializeField] private CanvasGroup mainMenuPanel;
        [SerializeField] private CanvasGroup seedPanel;
        [SerializeField] private CanvasGroup howToPlayPanel;
        [SerializeField] private CanvasGroup creditsPanel;

        [Header("Animations")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("UI Elements")]
        [SerializeField] private TMP_InputField inputField;

        private CanvasGroup _currentPanel;
        private bool _isStartingGame = false;

        private void Start()
        {
            InitPanel(mainMenuPanel, true);
            InitPanel(seedPanel, false);
            InitPanel(howToPlayPanel, false);
            InitPanel(creditsPanel, false);
            
            _currentPanel = mainMenuPanel;
        }

        private void InitPanel(CanvasGroup panel, bool startActive)
        {
            panel.alpha = startActive ? 1f : 0f;
            panel.gameObject.SetActive(startActive);
            panel.blocksRaycasts = startActive;
            panel.interactable = startActive;
        }

        private void SwitchPanel(CanvasGroup targetPanel)
        {
            if (_currentPanel == targetPanel) return;

            if (_currentPanel != null)
            {
                CanvasGroup oldPanel = _currentPanel;
                oldPanel.blocksRaycasts = false;
                oldPanel.interactable = false;

                oldPanel.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    oldPanel.gameObject.SetActive(false);
                });
            }

            if (targetPanel != null)
            {
                targetPanel.gameObject.SetActive(true);
                targetPanel.alpha = 0f;

                targetPanel.DOFade(1f, fadeDuration).OnComplete(() =>
                {
                    targetPanel.blocksRaycasts = true;
                    targetPanel.interactable = true;
                });
            }

            _currentPanel = targetPanel;
        }
        
        public void OnStartClicked()
        {
            SwitchPanel(seedPanel);
        }

        public void OnHowToPlayClicked()
        {
            _isStartingGame = false;
            SwitchPanel(howToPlayPanel);
        }

        public void OnCreditsClicked()
        {
            SwitchPanel(creditsPanel);
        }

        public void OnExitClicked()
        {
            Debug.Log("Wychodzenie z gry...");
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        public void OnSubmitSeed()
        {
            Seed = inputField.text;
            _isStartingGame = true;
            SwitchPanel(howToPlayPanel);
        }

        public void SetSeed(string seed)
        {
            Seed = seed;
        }
        
        public void OnBackFromSeedClicked() 
        {
            SwitchPanel(mainMenuPanel);
        }

        public void OnCloseHowToPlayClicked()
        {
            if (_isStartingGame)
            {
                _currentPanel.blocksRaycasts = false;
                _currentPanel.interactable = false;
                _currentPanel.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    SceneManager.LoadScene("MERGED_SCENE");
                });
            }
            else
            {
                SwitchPanel(mainMenuPanel);
            }
        }

        public void OnCloseCreditsClicked()
        {
            SwitchPanel(mainMenuPanel);
        }
    }
}
