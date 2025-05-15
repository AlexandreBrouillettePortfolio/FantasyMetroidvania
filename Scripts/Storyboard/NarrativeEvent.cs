using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a narrative event. A narrative event is a collection of steps that are executed in order.
/// Steps can be a dialog from the player, NPC. It can be an animation trigger or a new item given to the player.
/// </summary>
[CreateAssetMenu(menuName = "Game/Narrative Event")]
public class NarrativeEvent : ScriptableObject
{
    [Tooltip("The name of the NPC that is speaking to the player. This is the default value in case the name of the NPC is not set in a step.")]
    public string DefaultNpcName;

    [SerializeReference]
    public List<NarrativeEventStep> Steps;
}

/// <summary>
/// Represents the base step in a narrative event.
/// </summary>
[Serializable]
public abstract class NarrativeEventStep
{
    public virtual string Name => GetType().Name;

    /// <summary>
    /// Gets or sets whether the step should be executed immediately
    /// without waiting for 5 seconds.
    /// </summary>
    public virtual bool IsImmediate => false;

    public int DisplayLength = 5;
}

/// <summary>
/// Represents a dialog step in a narrative event. A dialog contains a text and a speaker.
/// </summary>
[Serializable]
public class DialogEventStep : NarrativeEventStep
{
    public override string Name
    {
        get
        {
            var from = FromPlayer ? "Player" : (string.IsNullOrEmpty(NpcName) ? "Default NPC Name" : NpcName);
            var text = Text.Length > 20 ? $"{Text[..20]}..." : Text;
            return $"{from} says {text}";
        }
    }

    /// <summary>
    /// The text that the source will say.
    /// </summary>
    public string Text;

    /// <summary>
    /// Gets or sets whether the dialog is from the player or from an NPC.
    /// </summary>
    public bool FromPlayer;

    /// <summary>
    /// If the dialog is from an NPC, this is the name of the NPC.
    /// </summary>
    public string NpcName;
}

[Serializable]
public class GiveItemToPlayerEventStep : NarrativeEventStep
{
    public override string Name => $"Give {Item.name} to player";

    public InventoryItem Item;
}

[Serializable]
public class SetFlagEventStep : NarrativeEventStep
{
    public override string Name => $"Set {FlagName} to {Value}";

    override public bool IsImmediate => true;

    public string FlagName;

    public bool Value = true;
}
