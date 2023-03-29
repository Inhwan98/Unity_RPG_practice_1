using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance = null;

    public delegate void OnSlotCountChange(int val); //�븮�� ����
    public OnSlotCountChange onSlotCountChange; //�븮�� �ν��Ͻ�ȭ

    private bool IsUseInven;
    public bool isUseInven
    {
        get => IsUseInven;
        set
        {
            IsUseInven = value;
        }
    }

    private int slotCnt; //slot ������ ���� ����
    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            onSlotCountChange.Invoke(slotCnt);
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        SlotCnt = 4;
    }
}
