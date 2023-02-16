using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioSource bgSound;
    public AudioClip[] bglist;

    public static SoundManager instance = null;

    private void Awake()
    {
        //싱글톤 처리
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //씬에따라 sound 변경
        for (int i = 0; i<bglist.Length; i++)
        {
            if (arg0.name == bglist[i].name)
                BgSoundPlay(bglist[i]);
        }
    }

    public void BGSoundVolume(float val)
    {
        //믹서의 볼륨은 로그 스케일을 사용한다
        mixer.SetFloat("BGSoundVolume", Mathf.Log10(val) * 20);
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audiosource = go.AddComponent<AudioSource>();
        audiosource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audiosource.clip = clip;
        audiosource.Play();

        Destroy(go, clip.length);
    }
    public void BgSoundPlay(AudioClip clip)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.1f;
        bgSound.Play();
    }

    public void RandomAudio(AudioClip[] args)
    {
        int random = Random.Range(0, args.Length);

        SoundManager.instance.SFXPlay(args[random].name, args[random]);
    }
}
