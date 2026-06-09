using System.Collections.Generic;
using C__Classes.SaveSystem;
using C__Classes.Singletons;
using UnityEngine;

namespace C__Classes.Systems
{
    public class LootManager : SingletonPersistant<LootManager>
    {
        private HashSet<string> collectedItems = new HashSet<string>();
        private Dictionary<string, ContainerSaveData> containersById = new Dictionary<string, ContainerSaveData>();
    
        public void MarkAsLooted(string itemID)
        {
            collectedItems.Add(itemID);
        }

        public bool IsAlreadyLooted(string itemID)
        {
            return collectedItems.Contains(itemID);
        }

        public List<string> CaptureLootedIds()
        {
            return new List<string>(collectedItems);
        }

        public void RestoreLootedIds(IEnumerable<string> lootedIds)
        {
            collectedItems.Clear();
            if (lootedIds == null)
            {
                return;
            }

            foreach (string lootedId in lootedIds)
            {
                if (!string.IsNullOrWhiteSpace(lootedId))
                {
                    collectedItems.Add(lootedId);
                }
            }
        }

        public void SaveContainerState(ContainerSaveData containerSaveData)
        {
            if (containerSaveData == null || string.IsNullOrWhiteSpace(containerSaveData.containerId))
            {
                return;
            }

            containersById[containerSaveData.containerId] = containerSaveData;
        }

        public bool TryGetContainerState(string containerId, out ContainerSaveData containerSaveData)
        {
            containerSaveData = null;
            if (string.IsNullOrWhiteSpace(containerId))
            {
                return false;
            }

            return containersById.TryGetValue(containerId, out containerSaveData);
        }

        public List<ContainerSaveData> CaptureContainerStates()
        {
            return new List<ContainerSaveData>(containersById.Values);
        }

        public void RestoreContainerStates(IEnumerable<ContainerSaveData> containerStates)
        {
            containersById.Clear();
            if (containerStates == null)
            {
                return;
            }

            foreach (ContainerSaveData containerSaveData in containerStates)
            {
                SaveContainerState(containerSaveData);
            }
        }
    }
}
