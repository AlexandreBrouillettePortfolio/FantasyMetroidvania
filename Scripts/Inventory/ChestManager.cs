using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

public enum PowerType
{
    None,
    Earth,
    Ice,
    Fire,
    Wind
}

public class ChestManager : MonoBehaviour
{
    public List<Item> itemList = new List<Item>();
    public PowerType powerUnlocked;
    // Start is called before the first frame update
    private bool isOpen = false;
    private InventoryManager inventoryManager;
    public bool playerInsideZone = false;
    public GameObject interactionUI;
    private Animator _animator;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        _animator = GetComponent<Animator>();

        if (inventoryManager == null)
        {
            Debug.LogWarning("ChestManager: No InventoryManager found in the scene.");
        }
    }

    private void Update()
    {
        if (!isOpen && playerInsideZone)
        {
            if (Input.GetButton("Interact"))
            {
                if (_animator != null)
                {
                    //Chest open loot glory
                    _animator.SetTrigger("Open");
                }
                else
                {
                    Debug.LogWarning("No animator on Chest");
                }

                UnlockChestPower();

                //Give item
                for (int i = 0; i < itemList.Count; i++)
                {
                    inventoryManager.AddItemInventory(itemList[i]);
                    InventoryEvents.RaiseItemAdded(itemList[i]);
                }
                isOpen = true;
                interactionUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isOpen)
            {
                playerInsideZone = true;
                interactionUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isOpen)
            {
                playerInsideZone = false;
                interactionUI.SetActive(false);
            }
        }
    }

    private void UnlockChestPower()
    {
        if (inventoryManager == null) return;

        switch (powerUnlocked)
        {
            case PowerType.Earth:
                inventoryManager.earthPower = true;
                InventoryEvents.RaisePowerStoneUnlocked(4);
                Debug.Log("Unlocked Earth Power!");
                break;
            case PowerType.Fire:
                inventoryManager.firePower = true;
                InventoryEvents.RaisePowerStoneUnlocked(1); 
                Debug.Log("Unlocked Fire Power!");
                break;
            case PowerType.Ice:
                inventoryManager.icePower = true;
                InventoryEvents.RaisePowerStoneUnlocked(3);
                Debug.Log("Unlocked Ice Power!");
                break;
            case PowerType.Wind:
                inventoryManager.windPower = true;
                InventoryEvents.RaisePowerStoneUnlocked(2);
                Debug.Log("Unlocked Wind Power!");
                break;
            case PowerType.None:
            default:
                // If none is set or recognized, do nothing
                Debug.Log("No power to unlock.");
                break;
        }
    }
}
