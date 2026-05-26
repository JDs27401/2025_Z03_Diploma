using System.Collections.Generic;
using UnityEngine;
using C__Classes.Singletons;

namespace C__Classes.Managers
{
    public class ItemDiscoveryManager : SingletonNonPersistant<ItemDiscoveryManager>
    {
        private HashSet<string> discoveredItemIDs = new HashSet<string>();

        public void DiscoverItem(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                discoveredItemIDs.Add(itemId);
            }
        }

        public bool IsItemDiscovered(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            return discoveredItemIDs.Contains(itemId);
        }
    }
}