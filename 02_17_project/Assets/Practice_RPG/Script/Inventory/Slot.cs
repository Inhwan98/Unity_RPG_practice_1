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

    //���Թ�ư�� onclick�Լ��� �߰�
    //�κ��丮���� �������� Ÿ��Ȯ���ϰ� ����
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
        //�����ָ� ������ �������� ���ֿ�
        player.ItemCnt -= 1;
    }
}