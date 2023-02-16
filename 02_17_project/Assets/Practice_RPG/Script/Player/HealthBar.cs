using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public HitPoints hitPoints;

    [HideInInspector]
    public Player character;
    public Image meterImage;

    public Text hpText;

    float maxHitPoints;


    private void Start()
    {
        //���� maxHitPoints�� character��ũ��Ʈ�� ������ maxHitPoints���� �־��ش�.

        maxHitPoints = character.maxHitPoints;

    }
    private void Update()
    {
        if (character != null)
        {
            //value���� maxhitPoints�� ������.
            //fillAmount �ִ밪�� 1�̱� ������ �Ҽ������� ����� ��� �Ѵ�.
            meterImage.fillAmount = hitPoints.value / maxHitPoints; // value�� 10�̹Ƿ� 0.1


            //ǥ�õǴ� HP���� 100�� ���ؼ� �� �ڸ����� Ȯ���Ѵ�.
            hpText.text = "HP :" + (meterImage.fillAmount * 100);
        }
    }

}
