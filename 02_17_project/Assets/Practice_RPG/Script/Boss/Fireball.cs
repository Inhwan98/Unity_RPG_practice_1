using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public float fireDamage;
    private Rigidbody fireRigid;
    // Start is called before the first frame update
    void Start()
    {
        fireRigid = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        fireRigid.velocity = this.transform.forward * speed * Time.deltaTime;

        Destroy(this.gameObject, 3f);
    }
}
