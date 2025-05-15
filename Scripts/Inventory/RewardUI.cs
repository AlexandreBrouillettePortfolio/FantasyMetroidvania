using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;           // <-- and this (for TMP_Text)

public class RewardUI : MonoBehaviour
{
    [Header("Prefabs / References")]
    [Tooltip("UI prefab that contains an Image + TMP_Text. " +
                "It will be instantiated for every reward.")]
    [SerializeField] private GameObject rewardEntryPrefab;

    [Tooltip("Parent that will hold the spawned reward entries. " +
             "Usually an empty RectTransform anchored top-right.")]
    [SerializeField] private RectTransform rewardContainer;

    [Tooltip("How long a single reward stays fully visible (seconds).")]
    [SerializeField] private float showTime = 2.0f;

    [Tooltip("Fade-out duration (seconds).")]
    [SerializeField] private float fadeTime = 0.5f;

    [SerializeField]  private List<Sprite> powerStoneIcon;

    // Internal queue so rewards appear one after another
    private readonly Queue<IEnumerator> rewardQueue = new Queue<IEnumerator>();
    private bool isShowingReward = false;

    // ------------------------------------------------------------------

    private void OnEnable()
    {
        InventoryEvents.OnItemAdded += HandleItemAdded;
        InventoryEvents.OnPowerStoneUnlocked += HandleStoneUnlocked;
    }

    private void OnDisable()
    {
        InventoryEvents.OnItemAdded -= HandleItemAdded;
        InventoryEvents.OnPowerStoneUnlocked -= HandleStoneUnlocked;
    }

    // ------------------------------------------------------------------
    // EVENT HANDLERS
    // ------------------------------------------------------------------

    private void HandleItemAdded(Item item)
    {
        // Build the text and icon for the notification
        string msg = $"+{item.itemCount}  {item.itemID}";
        Sprite icon = item.itemIcon;
        EnqueueReward(msg, icon);
    }

    private void HandleStoneUnlocked(int idx)
    {
        // idx: 1=Fire, 2=Wind, 3=Ice, 4=Earth (adjust as you wish)
        string stoneName = idx switch
        {
            1 => "Fire Stone Unlocked!",
            2 => "Wind Stone Unlocked!",
            3 => "Ice Stone Unlocked!",
            4 => "Earth Stone Unlocked!",
            _ => "Unknown Power Unlocked!"
        };
        // You could store stone icons in an array; here we pass null
        EnqueueReward(stoneName, powerStoneIcon[idx-1]);
    }

    // ------------------------------------------------------------------
    // REWARD QUEUE
    // ------------------------------------------------------------------

    private void EnqueueReward(string text, Sprite icon)
    {
        rewardQueue.Enqueue(ShowRewardCoroutine(text, icon));

        if (!isShowingReward)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowingReward = true;

        while (rewardQueue.Count > 0)
        {
            yield return StartCoroutine(rewardQueue.Dequeue());
        }

        isShowingReward = false;
    }

    // ------------------------------------------------------------------
    // The actual pop-up coroutine
    // ------------------------------------------------------------------

    private IEnumerator ShowRewardCoroutine(string text, Sprite icon)
    {
        // 1) Instantiate UI entry
        GameObject entry = Instantiate(rewardEntryPrefab, rewardContainer);
        entry.transform.SetAsFirstSibling(); // stack newest on top

        Image img = entry.transform.Find("Icon").GetComponent<Image>();
        TMP_Text lbl = entry.transform.Find("Label").GetComponent<TMP_Text>();

        if (img != null) img.sprite = icon;
        if (lbl != null) lbl.text = text;

        // Ensure it’s fully visible
        CanvasGroup cg = entry.GetComponent<CanvasGroup>();
        if (cg == null) cg = entry.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        // 2) Wait visible time
        yield return new WaitForSeconds(showTime);

        // 3) Fade out
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            cg.alpha = 1f - t / fadeTime;
            yield return null;
        }

        // 4) Destroy UI entry
        Destroy(entry);
    }
}
