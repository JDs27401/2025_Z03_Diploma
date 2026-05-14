using UnityEngine;
using UnityEngine.UI;

namespace C__Classes.Managers
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button exitButton;

        void Start()
        {
            if (panel != null) panel.SetActive(false);
            if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
            if (loadButton != null) loadButton.onClick.AddListener(OnLoadClicked);
            if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
        }

        public void Show()
        {
            if (panel != null) panel.SetActive(true);
        }

        public void Hide()
        {
            if (panel != null) panel.SetActive(false);
        }

        public void OnResumeClicked()
        {
            PauseManager.Instance?.ResumeGame();
        }

        public void OnSettingsClicked()
        {
            // TODO: implement settings
        }

        public void OnSaveClicked()
        {
            // TODO: implement save
        }

        public void OnLoadClicked()
        {
            // TODO: implement load
        }

        public void OnExitClicked()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}

