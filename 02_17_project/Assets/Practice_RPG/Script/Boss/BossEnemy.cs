using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossEnemy : MonoBehaviour
{
    private static BossEnemy b_Instance = null;

    public EnemyHit enemyHit;
    public int maxHealth;
    public int curHealth;
    public int damage;
    public PhaseState BossState;

    //audio
    public AudioSource audioSrc;
    public AudioClip step;
    public AudioClip[] Takedamage;

    public GameObject fireballPrefab;
    public BoxCollider AttackCollider;
    public Transform AttackWave;
    public BossHealthBar bossHealthBarPrefab;
    BossHealthBar bossHealthBar;

    private Transform target;
    private Transform firetarget;

    private Fireball firewave;

    public Transform effectPos;
    public GameObject[] SpwanEffectPrefab;
    private int Effectnum;

    public float sensorRadious;
    public float sensorRange;

    public bool isAttack;
    public bool isChase;

    float delayTime;
    float firetime;
    float damageDelay;
    bool isTakeDamage;
    //[HideInInspector]
    public bool isDie = false;
    
    Animator ani;
    Rigidbody rigid;
    NavMeshAgent nav;
    public enum PhaseState
    {
        phase1,
        phase2,
        LastPhase
    }

    private void Awake()
    {
        if (b_Instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        b_Instance = this;
        DontDestroyOnLoad(this.gameObject);
        

        ani = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        audioSrc = GetComponent<AudioSource>();
        enemyHit.EnemyHitvalue = curHealth;

        CreateBar();
        DontDestroyOnLoad(bossHealthBar.gameObject);
    }
    void Start()
    {
        target = FindObjectOfType<Player>().transform;
        firetarget = transform.Find("WavePos");
        
        firetime = Random.Range(1.5f, 3.0f);
        delayTime = 0f;
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
    // Update is called once per frame
    void Update()
    {
        //NavMesh : NavAgent가 경로를 그리기 위한 바탕(Mesh)
        //SetDestination() = 도착할 목표 위치 지정 함수
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase; //isChase가 false면 iStopped은 true...
        }

        delayTime += Time.deltaTime;
    }

    void CreateBar()
    {
        bossHealthBar = Instantiate(bossHealthBarPrefab);
        bossHealthBar.bossObject = this;
        bossHealthBar.gameObject.SetActive(false);
     }

    void ChaseStart()
    {
        if (!audioSrc.isPlaying)
            audioSrc.Play();
        isChase = true;
        ani.SetBool("isRun", true);
        nav.enabled = true;
    }

    void ChaseEnd()
    {
        audioSrc.Stop();
        isChase = false;
        ani.SetBool("isRun", false);
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

    void Fireball()
    {
        if(delayTime > firetime)
        {
            GameObject fire = Instantiate(fireballPrefab, AttackWave.position, AttackWave.rotation);
            fire.transform.LookAt(firetarget);

            firewave = fire.GetComponent<Fireball>();
            firewave.fireDamage = this.damage * 1.3f;

            firetime = Random.Range(2, 4);
            delayTime = 0.0f;
        }
    }


    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        transform.LookAt(target);
        ani.SetBool("isAttack", true);

        switch (BossState)
        {
            case PhaseState.phase1:

                yield return new WaitForSeconds(0.05f);
                AttackCollider.enabled = true;

                yield return new WaitForSeconds(0.5f);
                AttackCollider.enabled = false;

                yield return new WaitForSeconds(1f);

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

        switch (BossState)
        {
            case PhaseState.phase1:
                targetRadius = 0.5f;
                targetRange = 0.3f;
                break;
            case PhaseState.phase2:
                targetRadius = 0.5f;
                targetRange = 0.3f;
                break;
            case PhaseState.LastPhase:
                targetRadius = 0.5f;
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
            Debug.Log("chase");
            ChaseStart();
            Fireball();
            bossHealthBar.gameObject.SetActive(true);

            if (attackHits.Length > 0 && !isAttack && !isTakeDamage)
            {
                StartCoroutine(Attack());
            }
        }
        else
            ChaseEnd();

        yield return null;
    }

    IEnumerator OnDamage(int Effectnum)
    {
        if (curHealth > 0 && !isDie)
        {
            GameObject TakeEffect = Instantiate(SpwanEffectPrefab[Effectnum], effectPos.position, effectPos.rotation);
            isTakeDamage = true;

            ani.SetBool("isTakeDamage", true);

            yield return new WaitForSeconds(damageDelay);

            isTakeDamage = false;

            Destroy(TakeEffect);
            ani.SetBool("isTakeDamage", false);
        }
        else
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.EndGame();
        }
    }

    public void DestoryBoss()
    {
        ani.SetTrigger("Die");
        ChaseEnd();
        gameObject.layer = 8;
        isDie = true;
        Destroy(this.gameObject);
        Destroy(bossHealthBar.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "GSword")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            damageDelay = weapon.rate;
            enemyHit.EnemyHitvalue = curHealth;
            Effectnum = 1;
            SoundManager.instance.SFXPlay("Take", Takedamage[0]);
            StartCoroutine(OnDamage(Effectnum));

            

            Debug.Log("SwordAttack :" + curHealth);
        }
        else if (other.tag == "GSwave")
        {
            GameObject DestoryWave = other.GetComponent<SwordWave>().gameObject;
            SwordWave wave = other.GetComponent<SwordWave>();
            curHealth -= wave.waveDamage;
            enemyHit.EnemyHitvalue = curHealth;
            damageDelay = wave.waveRate;
            Effectnum = 0;
            SoundManager.instance.SFXPlay("Take", Takedamage[1]);

            Destroy(DestoryWave);
            StartCoroutine(OnDamage(Effectnum));

            Debug.Log("WaveAttack :" + curHealth);
            
        }
    }

}


