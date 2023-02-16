using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHelathBar : MonoBehaviour
{

    public GameObject m_goPrefab;

    public EnemyHit enemyHits;

    [HideInInspector]
    public Enemy enemyObject;

    // Hp바가 적용될 몬스터의 위치가 담긴 리스트
    List<Transform> m_objectList = new List<Transform>();
    // Hp바 객체의 리스트바
    List<GameObject> m_hpBarList = new List<GameObject>();

    float M_maxhitPoints;

    public Image meterImage;

    public Text hpText;

    Camera m_cam = null;


    // Start is called before the first frame update
    void Start()
    {
        M_maxhitPoints = enemyObject.maxHealth;

        m_cam = Camera.main;
        GameObject[] t_objects = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < t_objects.Length; i++)
        {
            m_objectList.Add(t_objects[i].transform);
            GameObject t_hpbar = Instantiate(m_goPrefab, t_objects[i].transform.position, Quaternion.identity, transform);
            m_hpBarList.Add(t_hpbar);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyObject != null)
        {
            //value값을 maxhitPoints로 나눈다.
            //fillAmount 최대값이 1이기 때문에 소수점으로 만들어 줘야 한다.
            meterImage.fillAmount = enemyHits.EnemyHitvalue / M_maxhitPoints; // value가 10이므로 0.1
            
            //표시되는 HP값은 100을 곱해서 세 자리수로 확인한다.
            hpText.text = "HP :" + (meterImage.fillAmount * 100);
        }

        for (int i = 0; i < m_objectList.Count; i++)
        {
            m_hpBarList[i].transform.position = m_cam.WorldToScreenPoint(m_objectList[i].position + Vector3.up);
        }
    }
}
