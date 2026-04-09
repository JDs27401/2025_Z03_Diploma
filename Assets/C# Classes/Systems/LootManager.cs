using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance;
    
    private HashSet<string> collectedItems = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarkAsLooted(string itemID)
    {
        collectedItems.Add(itemID);
    }

    public bool IsAlreadyLooted(string itemID)
    {
        return collectedItems.Contains(itemID);
    }
}