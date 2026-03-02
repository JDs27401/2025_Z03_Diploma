using UnityEngine;

namespace C__Classes.Systems
{
    public class LootTracker : MonoBehaviour
    {
        public string itemName;
        
        private string myUniqueID;

        private void Start()
        {
            string buildingID = SceneTransport.ReturnSpawnID;
            string pos = Mathf.RoundToInt(transform.position.x) + "_" + Mathf.RoundToInt(transform.position.y);

            myUniqueID = $"{buildingID}_{itemName}_{pos}";

            if (LootManager.Instance != null && LootManager.Instance.IsAlreadyLooted(myUniqueID))
            {
                Destroy(gameObject);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("player"))
            {
                if(LootManager.Instance != null)
                    LootManager.Instance.MarkAsLooted(myUniqueID);
            }
        }
    }
}