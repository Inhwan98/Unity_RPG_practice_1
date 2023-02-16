using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //UI ���� ���̺귯��
using UnityEngine.SceneManagement; //�� �������� ���̺귯��

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameObject PlayerPrefab; // �÷��̾� ��ȯ�ϱ����� ����

    public GameObject playerObj;
    [SerializeField]
    private Player player;
    public Text ScoreText;
    public Transform startPoint;
    private bool isGameOver; //���ӿ��� ����

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
            //���ӿ��� ���¿��� RŰ�� ���� ���
            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(this.gameObject);
                SceneManager.LoadScene("1-World");
            }
        }
    }

    public void EndGame()
    {
        //���� ���¸� ���ӿ��� ���·� ��ȯ
        isGameOver = true;

        SceneManager.LoadScene("GameEnd");

        
        player.DestroyPlayer();
    }
}
