using C__Classes.Managers;
using C__Classes.SaveSystem;
using C__Classes.Systems;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace C__Classes.SceneManagement
{
    public static class MainMenuReturn
    {
        public const string DefaultMainMenuSceneName = "MainMenu";

        public static void LoadMainMenu(string mainMenuSceneName = DefaultMainMenuSceneName)
        {
            Time.timeScale = 1f;
            DOTween.KillAll();

            SceneTransport.TargetSpawnID = null;
            SceneTransport.ReturnSpawnID = null;

            DestroyIfExists(PauseManager.Instance);
            DestroyIfExists(LootManager.Instance);
            DestroyIfExists(MainMenuManager.Instance);
            DestroyIfExists(SaveGameManager.Instance);

            SceneManager.LoadScene(string.IsNullOrWhiteSpace(mainMenuSceneName) ? DefaultMainMenuSceneName : mainMenuSceneName, LoadSceneMode.Single);
        }

        private static void DestroyIfExists(Object target)
        {
            if (target == null)
            {
                return;
            }

            GameObject gameObject = target is Component component ? component.gameObject : target as GameObject;
            if (gameObject != null)
            {
                Object.Destroy(gameObject);
            }
        }
    }
}
