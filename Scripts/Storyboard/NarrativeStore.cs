using System.Collections.Generic;

public class NarrativeStore
{
    public static NarrativeStore Instance { get; set; }

    private readonly Dictionary<string, NarrativeEvent> _completedEvents = new();

    public void CompleteEvent(NarrativeEvent narrativeEvent)
    {
        _completedEvents[narrativeEvent.name] = narrativeEvent;
    }

    public bool IsEventCompleted(NarrativeEvent narrativeEvent)
    {
        return _completedEvents.ContainsKey(narrativeEvent.name);
    }
}
