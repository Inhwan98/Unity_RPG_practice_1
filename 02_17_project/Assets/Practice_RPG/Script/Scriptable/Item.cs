using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        HEALTH,
        Coin
    }

    public Sprite itemImage;
    public int quantity; //������ ����
    public bool stackable; // ����...true�� �ѹ��� ó���� �� ����... (�������� ������)

    public ItemType itemType;
}
