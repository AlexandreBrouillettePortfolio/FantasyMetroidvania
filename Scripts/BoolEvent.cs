using UnityEngine.Events;

/// <summary>
/// A UnityEvent that takes a bool parameter. Used by event that has a on or off state, like a crouch event.
/// </summary>
[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }
