using C__Classes.Singletons;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace C__Classes.Managers
{
    public class MainMenuManager : SingletonPersistant<MainMenuManager>
    {
        public string Seed {get; private set;}
        
        [SerializeField] private TMP_InputField inputField;

        public void OnSubmitSeed()
        {
            Seed = inputField.text;
            SceneManager.LoadScene("JD_Gym");
            // print(_hash);
        } 
    }
}