using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public AudioClip clip;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        SoundManager.instance.SFXPlay("Hook", clip);
    }
}
