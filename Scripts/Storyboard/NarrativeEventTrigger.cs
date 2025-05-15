using UnityEngine;

public class NarrativeEventTrigger : MonoBehaviour
{
    [SerializeField]
    private NarrativeEvent _event;

    [SerializeField]
    private TriggerConditionType _triggerCondition;

    [Tooltip("Si activé, la narration restera affichée tant que le joueur est dans la zone")]
    [SerializeField]
    private bool _keepNarrationWhileInTrigger = true;

    private bool _playerInTrigger = false;
    private NarrativeSystem _narrativeSystem;

    private void Start()
    {
        // Récupération du système de narration au démarrage
        _narrativeSystem = FindObjectOfType<NarrativeSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_narrativeSystem != null && collision.gameObject.CompareTag("Player"))
        {
            _playerInTrigger = true;

            // Vérifier si on peut déclencher l'événement
            if (CanTrigger(_narrativeSystem))
            {
                // Démarrer la narration avec le flag indiquant que le joueur est dans le trigger
                _narrativeSystem.StartNarration(_event, _keepNarrationWhileInTrigger);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerInTrigger = false;

            // Informer le système de narration que le joueur est sorti de la zone
            if (_narrativeSystem != null && _keepNarrationWhileInTrigger)
            {
                _narrativeSystem.PlayerExitedTrigger();
            }
        }
    }

    private bool CanTrigger(NarrativeSystem narrativeSystem)
    {
        return _triggerCondition switch
        {
            TriggerConditionType.Always => true,
            TriggerConditionType.Once => !narrativeSystem.IsEventComplete(_event),
            //TriggerConditionType.KeyItem => Inventory.Instance.HasItem(_triggerKey),
            //TriggerConditionType.Flag => Game.Instance.Flags[_triggerKey] == false,
            _ => false,
        };
    }
}

public enum TriggerConditionType
{
    /// <summary>
    /// Always trigger the event
    /// </summary>
    Always,

    /// <summary>
    /// Must have a key item in the inventory
    /// </summary>
    KeyItem,

    /// <summary>
    /// Event can only be triggered once
    /// </summary>
    Once
}