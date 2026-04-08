using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    
    public ItemData[] inputs = new ItemData[4]; 

    public ItemData resultItem;
    public int resultAmount = 1;
}