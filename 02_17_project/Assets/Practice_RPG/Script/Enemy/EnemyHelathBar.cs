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

    // Hp�ٰ� ����� ������ ��ġ�� ��� ����Ʈ
    List<Transform> m_objectList = new List<Transform>();
    // Hp�� ��ü�� ����Ʈ��
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
            //value���� maxhitPoints�� ������.
            //fillAmount �ִ밪�� 1�̱� ������ �Ҽ������� ����� ��� �Ѵ�.
            meterImage.fillAmount = enemyHits.EnemyHitvalue / M_maxhitPoints; // value�� 10�̹Ƿ� 0.1
            
            //ǥ�õǴ� HP���� 100�� ���ؼ� �� �ڸ����� Ȯ���Ѵ�.
            hpText.text = "HP :" + (meterImage.fillAmount * 100);
        }

        for (int i = 0; i < m_objectList.Count; i++)
        {
            m_hpBarList[i].transform.position = m_cam.WorldToScreenPoint(m_objectList[i].position + Vector3.up);
        }
    }
}
