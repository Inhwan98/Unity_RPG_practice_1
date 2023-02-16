using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public EnemyHit enemyhitPoints;

    [HideInInspector]
    public BossEnemy bossObject;

    public Image meterImage;

    float maxHitPoints;

    private void Start()
    {
        //���� maxHitPoints�� character��ũ��Ʈ�� ������ maxHitPoints���� �־��ش�.
        maxHitPoints = bossObject.maxHealth;
    }
    private void Update()
    {
        if (bossObject != null)
        {
            //value���� maxhitPoints�� ������.
            //fillAmount �ִ밪�� 1�̱� ������ �Ҽ������� ����� ��� �Ѵ�.
            meterImage.fillAmount = enemyhitPoints.EnemyHitvalue / maxHitPoints; // value�� 10�̹Ƿ� 0.1
        }
    }

}
