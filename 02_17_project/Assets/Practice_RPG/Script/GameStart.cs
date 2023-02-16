using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public void PreeStartButton()
    {
        SceneManager.LoadScene("1-World");
    }
}
