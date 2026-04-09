using System.Collections.Generic;
using C__Classes.Singletons;
using UnityEngine;

namespace C__Classes.Systems
{
    public class LootManager : SingletonPersistant<LootManager>
    {
        private HashSet<string> collectedItems = new HashSet<string>();
    
        public void MarkAsLooted(string itemID)
        {
            collectedItems.Add(itemID);
        }

        public bool IsAlreadyLooted(string itemID)
        {
            return collectedItems.Contains(itemID);
        }
    }
}