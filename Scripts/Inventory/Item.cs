using UnityEngine;

/// <summary>
/// Possible categories of items (optional).
/// </summary>
public enum ItemType
{
    Ring,
    Necklace,
    Boot,
    Consumable
}

/// <summary>
/// Abstract base class for all item ScriptableObjects.
/// No [CreateAssetMenu] here, because we only instantiate child classes.
/// </summary>
public abstract class Item : ScriptableObject
{
    public string itemID;
    public int itemCount;
    public Sprite itemIcon;
}

/// <summary>
/// Optional extension method to clone ScriptableObjects at runtime.
/// </summary>
public static class ScriptableObjectExtension
{
    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        T instance = Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }
}