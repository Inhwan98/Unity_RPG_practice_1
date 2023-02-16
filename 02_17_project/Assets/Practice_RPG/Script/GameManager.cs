using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UI 관련 라이브러리
using UnityEngine.SceneManagement; //씬 관리관련 라이브러리

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameObject PlayerPrefab; // 플레이어 소환하기위한 변수

    public GameObject playerObj;
    [SerializeField]
    private Player player;
    public Text ScoreText;
    public Transform startPoint;
    private bool isGameOver; //게임오버 상태

    // Start is called before the first frame update
    private void Awake()
    {
        #region Singleton

        if (instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        #endregion

        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            ScoreText.text = "Score : " + player.AmountCoin;
        }
        else
        {
            //게임오버 상태에서 R키를 누른 경우
            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(this.gameObject);
                SceneManager.LoadScene("1-World");
            }
        }
    }

    public void EndGame()
    {
        //현재 상태를 게임오버 상태로 전환
        isGameOver = true;

        SceneManager.LoadScene("GameEnd");

        
        player.DestroyPlayer();
    }
}
