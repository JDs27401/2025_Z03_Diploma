using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace C__Classes
{
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance;

        public string Seed {get; private set;}
        
        [SerializeField] private TMP_InputField inputField;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(this);
        }

        public void OnSubmitSeed()
        {
            Seed = inputField.text;
            SceneManager.LoadScene("JD_Gym");
            // print(_hash);
        } 
    }
}