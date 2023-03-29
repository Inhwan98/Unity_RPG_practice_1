using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWave : MonoBehaviour
{
    public float speed;

    [HideInInspector]
    public int waveDamage;
    [HideInInspector]
    public float waveRate;
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>(); 
    }

    private void Start()
    {
        Destroy(gameObject, 1f);
    }

    private void Update()
    {
        rigid.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
    }
}
