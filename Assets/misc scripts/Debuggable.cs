using UnityEngine;

public class Debuggable : MonoBehaviour
{
    private long frameCounter = 0;
    // This function prints values of variables specified by their names (string[]) for the inheriting class
    protected void PrintVariables(params string[] variablePaths)
    {
        print("VVV FRAME " + frameCounter++ + " VVV");
        foreach (var path in variablePaths)
        {
            object value = this;
            System.Type currentType = value.GetType();
            string[] names = path.Split('.');

            bool found = true;

            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                var field = currentType.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    value = field.GetValue(value);
                    currentType = value != null ? value.GetType() : null;
                }
                else
                {
                    var property = currentType?.GetProperty(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (property != null)
                    {
                        value = property.GetValue(value, null);
                        currentType = value != null ? value.GetType() : null;
                    }
                    else
                    {
                        Debug.LogWarning($"Variable or property '{name}' not found in path '{path}' (at {currentType?.Name ?? "null"})");
                        found = false;
                        break;
                    }
                }
                // If at any point the value is null, we can't traverse further
                if (value == null)
                {
                    Debug.LogWarning($"Null encountered while traversing '{path}' at '{name}'");
                    found = false;
                    break;
                }
            }

            if (found)
            {
                Debug.Log($"{path} = {value}");
            }
        }
    }
}
