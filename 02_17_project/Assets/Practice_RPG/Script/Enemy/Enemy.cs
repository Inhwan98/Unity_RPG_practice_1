using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //Nav 관련 네임스페이스 가져오기

public class Enemy : MonoBehaviour
{
    //체력
    public int maxHealth;
    public int curHealth;

    public int damage;
    public EnemyState enemyState;
    
    public GameObject heartPrefab;
    public GameObject CoinPrefab;
    public AudioClip[] m_damage;
    public Transform effectPos;
    public Transform target;
    public GameObject[] SpwanEffectPrefab;
    public BoxCollider boxCollider;

    public float sensorRadious;
    public float sensorRange;

    public bool isAttack;
    public bool isChase;

    Animator ani;
    Rigidbody rigid;
    NavMeshAgent nav;
    AudioSource audioSrc;

    
    float damageDelay;
    int Effectnum;


    bool isTakeDamage;
    bool isDie;

    Vector3 KnockBack;

    public enum EnemyState
    {
        Spider,
        fastSpider
    }

    private void Awake()
    {
        ani = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        audioSrc = GetComponent<AudioSource>();
    }

    private void Start()
    {
        KnockBack = new Vector3(0, 0, -0.2f);
        target = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        //NavMesh : NavAgent가 경로를 그리기 위한 바탕(Mesh)
        //SetDestination() = 도착할 목표 위치 지정 함수
          if(nav.enabled)
          {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase; //isChase가 false면 iStopped은 true...
          }
    }


    private void FixedUpdate()
    {
        FreezeRotation();
        
        if (!isDie)
        {
            StartCoroutine(Targetting());
        }
        else
            StopCoroutine(Targetting());

    }

    void ChaseStart()
    {
        isChase = true;
        ani.SetBool("isWalk", true);
        nav.enabled = true;
    }

    void ChaseEnd()
    {
        isChase = false;
        ani.SetBool("isWalk", false);
        nav.enabled = false;
    }

    void FreezeRotation()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        {
            Gizmos.color = Color.red;
            Debug.DrawLine(transform.position, transform.position + transform.forward * sensorRange);
            Gizmos.DrawWireSphere(transform.position + transform.forward * sensorRange, sensorRadious);
        }
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        transform.LookAt(target);
        ani.SetBool("isAttack", true);

        switch (enemyState)
        {
            case EnemyState.Spider:

                yield return new WaitForSeconds(0.05f);
                boxCollider.enabled = true;

                yield return new WaitForSeconds(0.5f);
                boxCollider.enabled = false;

                yield return new WaitForSeconds(1f);

                break;

            case EnemyState.fastSpider:

                yield return new WaitForSeconds(0.05f);
                boxCollider.enabled = true;

                yield return new WaitForSeconds(0.25f);
                boxCollider.enabled = false;

                yield return new WaitForSeconds(0.5f);

                break;
        }
        isChase = true;
        isAttack = false;
        ani.SetBool("isAttack", false);
    }

    IEnumerator Targetting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        switch (enemyState)
        {
            case EnemyState.Spider:
                targetRadius = 0.6f;
                targetRange = 0.2f;
                break;
            case EnemyState.fastSpider:
                targetRadius = 0.6f;
                targetRange = 0.3f;
                break;
        }

        RaycastHit[] rayHits =
           Physics.SphereCastAll(transform.position,
                                 sensorRadious,
                                 transform.forward,
                                 sensorRange,
                                 LayerMask.GetMask("Player"));

        RaycastHit[] attackHits =
            Physics.SphereCastAll(transform.position,
                                  targetRadius,
                                  transform.forward,
                                  targetRange,
                                  LayerMask.GetMask("Player"));
        //Physics.SphereCastAll:
        //위치, (구체)반지름, 방향, 구체범위, 구체의 들어온 레이어판별

       

        if (rayHits.Length > 0 && !isAttack)
        {
            ChaseStart();
            if (attackHits.Length > 0 && !isAttack && !isTakeDamage)
            {
                StartCoroutine(Attack());
            }
        }

        yield return null;
    }

    IEnumerator OnDamage(Vector3 KnockBack, int Effectnum)
    {
        if (curHealth > 0 && !isDie)
        {
            GameObject TakeEffect = Instantiate(SpwanEffectPrefab[Effectnum], effectPos.position, effectPos.rotation);
            isTakeDamage = true;

            ani.SetBool("isTakeDamage", isTakeDamage);
            
            rigid.AddForce(KnockBack);
            
            isTakeDamage = false;
            yield return new WaitForSeconds(damageDelay);
            Destroy(TakeEffect);
            ani.SetBool("isTakeDamage", isTakeDamage);
        }
        else
        {
            ani.SetTrigger("Die");
            ChaseEnd();
            audioSrc.Stop();
            gameObject.layer = 8;
            isDie = true;
            float randomDrop = Random.Range(0, 3);
            Debug.Log(randomDrop);
            GameObject dropItem;
            //1/randomDrop 확률로 피회복 아이템 드롭
            if (randomDrop == 2)
            {
                dropItem = Instantiate(heartPrefab, transform.position, transform.rotation);
            }
            else
            {
                dropItem = Instantiate(CoinPrefab, transform.position, transform.rotation);
            }
            Rigidbody itemRigid = dropItem.GetComponent<Rigidbody>();
            Vector3 itemVec = new Vector3(Random.Range(-1, 1), Random.Range(2, 3), Random.Range(-1, 1));

            //랜덤한 방향으로 나온후 회전
            itemRigid.AddForce(itemVec, ForceMode.Impulse);
            itemRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

            Destroy(gameObject, 4f);
        }
    }

    public void RandomAudio(AudioClip[] args)
    {
        int random = Random.Range(0, args.Length);

        SoundManager.instance.SFXPlay(args[random].name, args[random]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GSword")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
           
            damageDelay = weapon.rate;
            Effectnum = 1;

            SoundManager.instance.SFXPlay("TakeSword", m_damage[0]);
            StartCoroutine(OnDamage(KnockBack, Effectnum));

            Debug.Log("SwordAttack :" + curHealth);
        }
        else if (other.tag == "GSwave")
        {
            SoundManager.instance.SFXPlay("TakeWave", m_damage[1]);
            GameObject DestoryWave = other.GetComponent<SwordWave>().gameObject;
            SwordWave wave = other.GetComponent<SwordWave>();
            curHealth -= wave.waveDamage;
            
            damageDelay = wave.waveRate;
            Destroy(DestoryWave);
            Effectnum = 0;
            StartCoroutine(OnDamage(KnockBack, Effectnum));

            Debug.Log("WaveAttack :" + curHealth);
        }
    }
}



