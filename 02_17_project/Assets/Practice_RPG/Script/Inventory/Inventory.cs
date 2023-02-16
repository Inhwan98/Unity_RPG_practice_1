using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance = null;

    public delegate void OnSlotCountChange(int val); //대리자 정의
    public OnSlotCountChange onSlotCountChange; //대리자 인스턴스화

    private int slotCnt; //slot 사이즈 정할 변수
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
