using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private static Player s_Instance = null;

    //캐릭터 정보
    public HitPoints hitPoints;
    public float maxHitPoints; //최대 체력
    public float curHitPoints; //시작 체력

    //보유동전
    private int amountCoin;
    public int AmountCoin
    {
        get => amountCoin;
        set { amountCoin = value; }
    }
    public float Movespeed;
    public float jumpPower;

    //오디오
    public AudioClip pickItem;
    public AudioClip[] AttackClip;
    public AudioClip[] JumpClip;
    public AudioClip TakeDamageClip;
    public AudioClip walkClip;

    //아이템
    public InventoryUI InvenUI;
    [SerializeField]
    private Inventory inven;
    private int itemCnt;
    public int ItemCnt
    {
        get => itemCnt;
        set { itemCnt = value; }
    }

    public Transform Swordrot;
    public Transform wavePos;
    public Transform lookwavePos;
    public GameObject swordwave;
    [SerializeField]
    private SwordWave waveScript;

    //체력바
    public HealthBar healthBarPrefab;
    [HideInInspector]
    public HealthBar healthBar;

    private Weapon weapon;
    private GameObject Aim;
    private Rigidbody rigid;
    private Animator ani;
    private AudioSource audioSrc;

    float SlideTime;
    float runtime;
    float firecount;
    float AttackRate; //wepon.rate 저장
    float AttackDelay;

    //Input
    float xInput;
    float zInput;

    bool rInput;
    bool jInput;
    bool fInput;
    bool cInput;
    bool sInput;
    //----------
    bool isSwing;
    bool isJump;
    bool isDead;
    bool isTakeDamage;
    bool isSlide;
    bool isAimSet;
    bool isMoving;

    //Camera
    float yMouse;
    float xMouse;
    private float limitMinX = -25;
    private float limitMaxX = 30;
    private float eulerAngleX;
    private float eulerAngleY;

    Vector3 vec;

    // Start is called before the first frame update
    private void Awake()
    {
        //스테이지 넘어가도 케릭터유지 싱글톤
        if (s_Instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        s_Instance = this;
        DontDestroyOnLoad(this.gameObject);

        inven = GetComponent<Inventory>();
        InvenUI = GameObject.Find("Canvas").GetComponent<InventoryUI>();

        Aim = GameObject.FindWithTag("Aim");
        Aim.SetActive(false);

        GameObject weaponObject = GameObject.FindWithTag("GSword");
        weapon = weaponObject.GetComponent<Weapon>();

        rigid = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();

        hitPoints.value = curHitPoints;
        healthBar = Instantiate(healthBarPrefab);
        healthBar.character = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        DontDestroyOnLoad(healthBar.gameObject);
    }

    private void Start()
    {
        ItemCnt = 0;
        SlideTime = 0.8f; //슬라이드 시간
        AttackDelay = 1.0f; // 마지막 콤보 어택후 1초 대기
        runtime = 0.0f;
        firecount = 0;
        AmountCoin = 0;
        isDead = false;
        isTakeDamage = false;
        AttackRate = weapon.rate;
    }


    // Update is called once per frame
    void Update()
    {
        if(!inven.isUseInven)
        {
            runtime += Time.deltaTime;

            GetInput();
            Move();
            Turn();

            AimSwap();
            StartCoroutine(Slide());
            StartCoroutine(Jump());
            StartCoroutine(Attack(0.6f));
            StartCoroutine(ChangeAttack(0.6f));

            RotateTo(xMouse, yMouse);
        }
      }



    public void GetInput()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
        rInput = Input.GetButton("Run");
        jInput = Input.GetButtonDown("Jump");
        yMouse = Input.GetAxis("Mouse Y");
        xMouse = Input.GetAxis("Mouse X");
        fInput = Input.GetButton("Fire1");
        cInput = Input.GetButtonDown("Fire2");
        sInput = Input.GetButtonDown("Slide");
    }

    public void Move()
    {
        vec = new Vector3(xInput, 0f, zInput).normalized;

        if (isSwing || isDead || isTakeDamage)
        {
            vec = Vector3.zero;
        }

        ani.SetBool("isWalk", vec != Vector3.zero);
        ani.SetBool("isRun", rInput);
        ani.SetBool("isWalkR", vec.x > 0);
        ani.SetBool("isWalkL", vec.x < 0);
        ani.SetBool("isWalkB", vec.z < 0);

        transform.Translate(vec * Movespeed * (rInput ? 2.5f : 1.0f) *
                            (vec.z < 0 ? 0.5f : 1.0f) * Time.deltaTime);

        if (vec != Vector3.zero)
            isMoving = true;
        else
            isMoving = false;

        if (isMoving && !isJump && !isSwing)
        {
            if (!audioSrc.isPlaying)
                audioSrc.Play();
            if (rInput)
            {
                audioSrc.pitch = 2;
            }
            else
                audioSrc.pitch = 1.3f;
        }
        else
            audioSrc.Stop();
    }

    public void Turn()
    {
        transform.LookAt(transform.position + vec);
    }

    IEnumerator Jump()
    {
        if (jInput && !isSwing && !isJump && !isDead)
        {
            ani.SetBool("IsJump", true);
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            ani.SetTrigger("doJump");
            isJump = true;
            SoundManager.instance.RandomAudio(JumpClip);
            yield return null;
        }
    }

    IEnumerator Slide()
    {
        if (rInput && sInput && !isDead && !isSlide && !isJump && runtime > SlideTime)
        {
            isSlide = true;
            ani.SetTrigger("doSlide");
            yield return new WaitForSeconds(0.5f);

            isSlide = false;
            runtime = 0; //
        }
    }
    void AimSwap()
    {
        if (cInput && !isAimSet)
        {
            isAimSet = true;
            Aim.SetActive(true);
        }
        else if (cInput && isAimSet)
        {
            isAimSet = false;
            Aim.SetActive(false);
        }
    }
    IEnumerator Attack(float time)
    {
        if (!isAimSet && fInput && runtime > weapon.rate && !isSwing && !isDead)
        {
            float AttackDelay = 1.0f;
            firecount += 1;

            if (firecount >= 7) firecount = 1;
            if (firecount == 6) weapon.rate = AttackDelay;
            if (firecount == 1) weapon.rate = AttackRate;
            if (runtime > 1.6f) firecount = 1;

            isSwing = true;
            ani.SetTrigger("Attack" + firecount);
            weapon.Use();

            if (firecount < 5)
            {
                SoundManager.instance.SFXPlay(AttackClip[0].name, AttackClip[0]);
            }
            else if (firecount == 5)
            {
                SoundManager.instance.SFXPlay(AttackClip[1].name, AttackClip[1]);
            }
            runtime = 0.0f;
            yield return new WaitForSeconds(time);
            isSwing = false;
        }
    }

    IEnumerator ChangeAttack(float time)
    {
        if (isAimSet && fInput && runtime > weapon.rate && !isSwing && !isDead)
        {
            firecount += 1;

            if (firecount >= 4)
                firecount = 1;

            if (firecount == 3)
                weapon.rate = AttackDelay;

            if (runtime > 1.6f)
                firecount = 1;

            if (firecount == 1)
                weapon.rate = AttackRate;

            isSwing = true;
            ani.SetTrigger("Attack" + firecount);

            StartCoroutine(Createwave());

            //WeaPon의 연속공격인 Use함수 사용.
            weapon.Use();

            SoundManager.instance.SFXPlay(AttackClip[2].name, AttackClip[2]);


            runtime = 0.0f;
            yield return new WaitForSeconds(time);
            isSwing = false;

        }
    }

    IEnumerator Createwave()
    {
        yield return new WaitForSeconds(0.3f);
        GameObject wave = Instantiate(swordwave, wavePos.position, wavePos.rotation);

        wave.transform.LookAt(lookwavePos.position);

        waveScript = wave.GetComponent<SwordWave>();
        waveScript.waveDamage = weapon.damage - 5;
        waveScript.waveRate = weapon.rate;
    }



    public void RotateTo(float mouseX, float mouseY)
    {
        eulerAngleY += mouseX;
        //같은 개념으로 카메라가 위 / 아래를 보려면 카메라 오브젝트의 x축이 회전
        eulerAngleX -= mouseY;

        // x축 회전 값의 경우 아래, 위를 볼 수 있는 제한 각도가 설정되어 있다.
        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        //실제 오브젝트의 쿼터니온 회전에 적용
        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            ani.SetBool("IsJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyAttack")
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            Debug.Log("2");

            if (!isTakeDamage && !isSlide)
            {
                curHitPoints -= enemy.damage;
                Debug.Log("Player Hp : " + curHitPoints);
                StartCoroutine(OnDamage());
                hitPoints.value = curHitPoints;
            }
        }

        if (other.tag == "BossAttack")
        {
            BossEnemy boss = other.GetComponentInParent<BossEnemy>();
            Debug.Log("2");

            if (!isTakeDamage && !isSlide)
            {
                curHitPoints -= boss.damage;
                Debug.Log("Player Hp : " + curHitPoints);
                StartCoroutine(OnDamage());
                hitPoints.value = curHitPoints;
            }
        }
        else if (other.tag == "BossFire")
        {
            Fireball boss = other.GetComponentInParent<Fireball>();
            Debug.Log("BossFire");
            if (!isTakeDamage && !isSlide)
            {
                curHitPoints -= boss.fireDamage;
                Debug.Log("Player Hp : " + curHitPoints);
                StartCoroutine(OnDamage());
                hitPoints.value = curHitPoints;
            }
        }

        //Item과 충돌
        if (other.tag == "Item")
        {
            //Item 클래스의 hitObject 인스턴스화 과정.
            //상대 오브젝트가 Consumable클래스의 item을 호출한다.
            Item hitObject = other.gameObject.GetComponentInParent<Consum>().item;
            Debug.Log("item");

            //스크립트를 성공적으로 불러왔을 때
            if (hitObject != null)
            {
                /*print("Hit: " + hitObject.objectName);*/
                //shouldDisapper값 초기화
                bool shouldDisapper = false;

                //switch문을 사용하여 Item 스크립트에 itemType enum 데이터 참조
                switch (hitObject.itemType)
                {
                    //충돌한 오브젝트의 enum 데이터가 HEALTH일때는 함수를 사용하여 판단.
                    case Item.ItemType.HEALTH:
                        shouldDisapper = AdjustHitPoints(hitObject.quantity);
                        break;

                    case Item.ItemType.Coin:
                        if(ItemCnt < inven.SlotCnt)
                        {
                            InvenUI.slots[ItemCnt++].UpdateSlotUI(hitObject);
                            shouldDisapper = true;
                        }
                        break;
                }

                //shouldDisapper가 true일때 태그된 게임오브젝트 비활성화
                if (shouldDisapper)
                {
                    Destroy(other.gameObject);
                    SoundManager.instance.SFXPlay("PickItem", pickItem);
                }
            }
        }
    }
    //현재 체력이 최대체력보다 낮을때만 아이템 줍기 가능
    public bool AdjustHitPoints(int amount)
    {
        if (hitPoints.value < maxHitPoints)
        {
            curHitPoints += amount;
            hitPoints.value = curHitPoints;
            print("adjusthitPoints hitpoints by:" + amount + ". new value: " + hitPoints.value);
            return true;
        }
        return false;
    }

    IEnumerator OnDamage()
    {
        if (!isSwing)
        {
            ani.SetTrigger("isTakeDamage");
            isTakeDamage = true;
            SoundManager.instance.SFXPlay(TakeDamageClip.name, TakeDamageClip);
            yield return new WaitForSeconds(0.5f);
            isTakeDamage = false;
        }
        else if (isSwing)
        {
            isTakeDamage = true;
            yield return new WaitForSeconds(0.5f);
            isTakeDamage = false;
        }

        if (curHitPoints <= 0)
        {
            //씬에 존재하는 GameManager 타입의 오브젝트 찾아서 가져오기
            //메모리 다소 잡아먹나?
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.EndGame();

            
            yield return new WaitForSeconds(2.0f);
        }
    }

    public void DestroyPlayer()
    {
        ani.SetTrigger("doDead");
        isDead = true;
        Destroy(this.gameObject);
        gameObject.layer = 10;
    }
}
