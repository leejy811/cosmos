using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]       
public class Sound
{
    public string name;
    public AudioClip clip;
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] List<Sound> sfx = null;
    [SerializeField] Sound[] bgm = null;

    [SerializeField] AudioSource bgmPlayer = null;  
    [SerializeField] AudioSource[] sfxPlayer = null;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public void PlayBGM(string bgmName)
    {
        foreach (Sound s in bgm) 
        {
            if (bgmName == s.name)     // play BGM if such name exists
            {
                bgmPlayer.clip = s.clip;
                bgmPlayer.Play();           
                return;
            }
        }
    }
    public void StopBGM()
    {
        bgmPlayer.Stop();
    }
    public void PlaySFX(string sfxName)
    {
        int idx = sfx.FindIndex(x => x.name == sfxName);
        if (idx < 0)
            return;
        for (int i = 0; i < sfxPlayer.Length; i++)
        {
            if (!sfxPlayer[i].isPlaying)     //check if available audiosrc left
            {
                sfxPlayer[i].clip = sfx[idx].clip;
                sfxPlayer[i].Play();
                return;
            }
        }
        //Debug.Log("남아있는 오디오소스가 없습니다");
        return;
    }
}