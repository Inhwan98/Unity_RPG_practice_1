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
        //변수 maxHitPoints에 character스크립트의 변수인 maxHitPoints값을 넣어준다.
        maxHitPoints = bossObject.maxHealth;
    }
    private void Update()
    {
        if (bossObject != null)
        {
            //value값을 maxhitPoints로 나눈다.
            //fillAmount 최대값이 1이기 때문에 소수점으로 만들어 줘야 한다.
            meterImage.fillAmount = enemyhitPoints.EnemyHitvalue / maxHitPoints; // value가 10이므로 0.1
        }
    }

}
