using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Player player;
    public Image itemIcon;

    public Item item;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public void UpdateSlotUI(Item _item)
    {
        item = _item;
        itemIcon.sprite = item.itemImage;
        itemIcon.gameObject.SetActive(true);
    }

    //슬롯버튼에 onclick함수로 추가
    //인벤토리에서 눌렀을때 타입확인하고 진행
    public void useSlot()
    {
        switch (item.itemType)
        {
            case Item.ItemType.Coin:
                player.AmountCoin += item.quantity;
                itemIcon.sprite = null;
                itemIcon.gameObject.SetActive(false);
                Debug.Log("player : coin : " + player.AmountCoin);
                item = null;
                break;

            default:
                Debug.Log("Empty");
                break;
        }
        //안해주면 다음에 아이템을 못주움
        player.ItemCnt -= 1;
    }
}