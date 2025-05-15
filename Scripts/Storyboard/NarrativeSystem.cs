using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class NarrativeSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject _dialogPanel;

    [SerializeField]
    private GameObject _titlePanel;

    [SerializeField]
    private TextMeshProUGUI _whoIsTalking;

    [SerializeField]
    private TextMeshProUGUI _whatIsBeingSaid;

    [SerializeField]
    private TextMeshProUGUI _whatItemIsBeingGiven;

    private NarrativeEvent _currentEvent;
    private int _currentEventStep = 0;
    private bool _waitForPlayerExit = false;
    private bool _isShowingSteps = false;
    private Coroutine _currentCoroutine = null;

    private void Awake()
    {
        NarrativeStore.Instance ??= new NarrativeStore();
    }

    public void StartNarration(NarrativeEvent narrativeEvent, bool waitForPlayerExit = false)
    {
        // Si on est déjà en train de montrer cet événement, ne rien faire
        if (_currentEvent == narrativeEvent && _isShowingSteps)
        {
            return;
        }

        if (narrativeEvent.Steps.Count == 0)
        {
            return;
        }

        // Arrêter une éventuelle coroutine en cours
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        _currentEvent = narrativeEvent;
        _currentEventStep = 0;
        _waitForPlayerExit = waitForPlayerExit;
        _isShowingSteps = true;

        // Démarrer la coroutine pour afficher les étapes
        _currentCoroutine = StartCoroutine(ShowSteps());
    }

    public void PlayerExitedTrigger()
    {
        // Le joueur est sorti de la zone de trigger
        _waitForPlayerExit = false;
    }

    private IEnumerator ShowSteps()
    {
        while (_currentEventStep < _currentEvent.Steps.Count)
        {
            var step = _currentEvent.Steps[_currentEventStep];
            PerformStep(step);

            if (!step.IsImmediate)
            {
                if (_waitForPlayerExit)
                {
                    // Attendre que le joueur sorte de la zone
                    yield return new WaitUntil(() => !_waitForPlayerExit);
                }

                // Après que le joueur soit sorti (ou si on n'attend pas), attendre la durée indiquée
                yield return new WaitForSecondsOrEscape(step.DisplayLength);
            }

            _currentEventStep++;
        }

        NarrativeStore.Instance.CompleteEvent(_currentEvent);
        _currentEvent = null;
        _isShowingSteps = false;
        _currentCoroutine = null;

        _dialogPanel.SetActive(false);
    }

    private void PerformStep(NarrativeEventStep step)
    {
        if (step is DialogEventStep dialogStep)
        {
            var name = "You";
            if (!dialogStep.FromPlayer)
            {
                name = dialogStep.NpcName;
                if (string.IsNullOrEmpty(name))
                    name = _currentEvent.DefaultNpcName;
            }

            _whoIsTalking.text = name;
            _whatIsBeingSaid.text = dialogStep.Text;
            _whatItemIsBeingGiven.text = "";

            _titlePanel.SetActive(true);
            _dialogPanel.SetActive(true);
        }
        else if (step is GiveItemToPlayerEventStep giveItemStep)
        {
            _titlePanel.SetActive(false);

            _whatIsBeingSaid.text = "";
            _whatItemIsBeingGiven.text = $@"You have obtained <color=""red"">{giveItemStep.Item.name}</color>";
            _titlePanel.SetActive(false);
            _dialogPanel.SetActive(true);
        }
        else if (step is SetFlagEventStep setFlagStep)
        {
            // Ne pas modifier le dialogue actuel
        }
    }

    public bool IsEventComplete(NarrativeEvent @event)
    {
        return NarrativeStore.Instance.IsEventCompleted(@event);
    }
}