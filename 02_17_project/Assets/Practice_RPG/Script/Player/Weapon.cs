using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        GSword
    }

    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;


    public void Use()
    {
        if(type == Type.GSword)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
        //공격의 단계를 코루틴으로 구분
        //1
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true; //컴포넌트 활성화
        trailEffect.enabled = true;

        //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        //3
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

}
