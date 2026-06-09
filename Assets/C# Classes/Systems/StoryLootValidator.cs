using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C__Classes.Systems
{
    // Ta klasa pomoze ogarnac, czy sie wylosowal unikat potrzebny do skonczenia gry. Jesli nie to go dogeneruje
    public class StoryLootValidator : MonoBehaviour
    {
        public static StoryLootValidator Instance { get; private set; }
        
        [Header("Nazwy scen interiorów")]
        public List<string> pharmacySceneNames = new List<string> { "SmallBuildingInside" };
        public List<string> hardwareSceneNames = new List<string> { "MediumBuildingInside", "BigBuildingInside" };
        public List<string> gunShopSceneNames = new List<string> { "GasStationInside" };
        public List<string> generalStoreSceneNames = new List<string> { "GreenBarnInside", "RedBarnInside" };
        
        [Header("ID unikatów (pole 'Id' z ItemData)")]
        public string pharmacyUnique = "keypad";
        public string hardwareUnique = "electronics";
        public string gunShopUnique = "antenna";
        public string generalStoreUnique = "katana";
        
        private HashSet<string> allPharmacyDoors = new HashSet<string>();
        private HashSet<string> allHardwareDoors = new HashSet<string>();
        private HashSet<string> allGunShopDoors = new HashSet<string>();
        private HashSet<string> allGeneralStoreDoors = new HashSet<string>();
        
        private HashSet<string> visitedPharmacyDoors = new HashSet<string>();
        private HashSet<string> visitedHardwareDoors = new HashSet<string>();
        private HashSet<string> visitedGunShopDoors = new HashSet<string>();
        private HashSet<string> visitedGeneralStoreDoors = new HashSet<string>();
        
        [HideInInspector] public bool forcePharmacyUnique = false;
        [HideInInspector] public bool forceHardwareUnique = false;
        [HideInInspector] public bool forceGunShopUnique = false;
        [HideInInspector] public bool forceGeneralStoreUnique = false;
        
        [HideInInspector] public bool hasPharmacyUniqueSpawned = false;
        [HideInInspector] public bool hasHardwareUniqueSpawned = false;
        [HideInInspector] public bool hasGunShopUniqueSpawned = false;
        [HideInInspector] public bool hasGeneralStoreUniqueSpawned = false;

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            SceneManagement.SceneManagement[] allDoors = FindObjectsOfType<SceneManagement.SceneManagement>();

            foreach (var door in allDoors)
            {
                if (pharmacySceneNames.Contains(door.sceneToLoad)) allPharmacyDoors.Add(door.myUniqueID);
                else if (hardwareSceneNames.Contains(door.sceneToLoad)) allHardwareDoors.Add(door.myUniqueID);
                else if (gunShopSceneNames.Contains(door.sceneToLoad)) allGunShopDoors.Add(door.myUniqueID);
                else if (generalStoreSceneNames.Contains(door.sceneToLoad)) allGeneralStoreDoors.Add(door.myUniqueID);
                
            }
        }
        
        public void RegisterDoorEntry(string sceneToLoad, string doorID)
        {
            if (pharmacySceneNames.Contains(sceneToLoad))
            {
                visitedPharmacyDoors.Add(doorID);
                if (visitedPharmacyDoors.Count >= allPharmacyDoors.Count && !hasPharmacyUniqueSpawned) 
                    forcePharmacyUnique = true;
            }
            else if (hardwareSceneNames.Contains(sceneToLoad))
            {
                visitedHardwareDoors.Add(doorID);
                if (visitedHardwareDoors.Count >= allHardwareDoors.Count && !hasHardwareUniqueSpawned) 
                    forceHardwareUnique = true;
            }
            else if (gunShopSceneNames.Contains(sceneToLoad))
            {
                visitedGunShopDoors.Add(doorID);
                if (visitedGunShopDoors.Count >= allGunShopDoors.Count && !hasGunShopUniqueSpawned) 
                    forceGunShopUnique = true;
            }
            else if (generalStoreSceneNames.Contains(sceneToLoad))
            {
                visitedGeneralStoreDoors.Add(doorID);
                if (visitedGeneralStoreDoors.Count >= allGeneralStoreDoors.Count && !hasGeneralStoreUniqueSpawned) 
                    forceGeneralStoreUnique = true;
            }
        }
        
        public void MarkUniqueAsSpawned(string itemID)
        {
            if (string.IsNullOrEmpty(itemID)) return;
            
            if (itemID.Equals(pharmacyUnique, System.StringComparison.OrdinalIgnoreCase)) { hasPharmacyUniqueSpawned = true; forcePharmacyUnique = false; }
            else if (itemID.Equals(hardwareUnique, System.StringComparison.OrdinalIgnoreCase)) { hasHardwareUniqueSpawned = true; forceHardwareUnique = false; }
            else if (itemID.Equals(gunShopUnique, System.StringComparison.OrdinalIgnoreCase)) { hasGunShopUniqueSpawned = true; forceGunShopUnique = false; }
            else if (itemID.Equals(generalStoreUnique, System.StringComparison.OrdinalIgnoreCase)) { hasGeneralStoreUniqueSpawned = true; forceGeneralStoreUnique = false; }
        }
        
    }
}