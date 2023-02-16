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
        //변수 maxHitPoints에 character스크립트의 변수인 maxHitPoints값을 넣어준다.

        maxHitPoints = character.maxHitPoints;

    }
    private void Update()
    {
        if (character != null)
        {
            //value값을 maxhitPoints로 나눈다.
            //fillAmount 최대값이 1이기 때문에 소수점으로 만들어 줘야 한다.
            meterImage.fillAmount = hitPoints.value / maxHitPoints; // value가 10이므로 0.1


            //표시되는 HP값은 100을 곱해서 세 자리수로 확인한다.
            hpText.text = "HP :" + (meterImage.fillAmount * 100);
        }
    }

}
