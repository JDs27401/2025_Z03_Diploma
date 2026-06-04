using System.Collections.Generic;
using UnityEngine;

namespace C__Classes.SaveSystem
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Save System/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemData> items = new List<ItemData>();

        private Dictionary<string, ItemData> itemsById;

        public ItemData GetItemById(string id)
        {
            EnsureCache();
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            itemsById.TryGetValue(id, out ItemData item);
            return item;
        }

        public T GetItemById<T>(string id) where T : ItemData
        {
            return GetItemById(id) as T;
        }

        private void EnsureCache()
        {
            if (itemsById != null)
            {
                return;
            }

            itemsById = new Dictionary<string, ItemData>();
            for (int i = 0; i < items.Count; i++)
            {
                ItemData item = items[i];
                if (item == null || string.IsNullOrWhiteSpace(item.id))
                {
                    continue;
                }

                if (itemsById.ContainsKey(item.id))
                {
                    Debug.LogWarning($"Duplicate item save id '{item.id}' in {name}. Loading will use the first item.");
                    continue;
                }

                itemsById.Add(item.id, item);
            }
        }

        private void OnValidate()
        {
            itemsById = null;
        }
    }
}
