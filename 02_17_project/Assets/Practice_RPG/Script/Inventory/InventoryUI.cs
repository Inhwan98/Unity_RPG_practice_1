using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance = null;

    Inventory inven;
    GameManager gmr;

    public GameObject inventoryPanel;
    bool activeInventory = false;

    public Slot[] slots;
    public Transform slotHolder;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        inven = Inventory.instance;
        gmr = GameManager.instance;
        slots = slotHolder.GetComponentsInChildren<Slot>();
        inven.onSlotCountChange += SlotChange;
        inventoryPanel.SetActive(activeInventory);
    }

    private void SlotChange(int val)
    {
        //SlotCnt만큼만 intractable을 true해준다.
        for(int i = 0; i<slots.Length; i++)
        {
            if(i<inven.SlotCnt)
                slots[i].GetComponent<Button>().interactable = true;
            else
                slots[i].GetComponent<Button>().interactable = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);

            if(activeInventory)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                inven.isUseInven = true;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                inven.isUseInven = false;

            }
        }
        if(gmr.IsGameOver)
        {
            Destroy(gameObject);
        }
    }

    //SlotCnt를 증가 시켜준다.
    public void AddSlot()
    {
        inven.SlotCnt++;
    }
}
