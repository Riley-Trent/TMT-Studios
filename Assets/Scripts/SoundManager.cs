using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum SoundType
{
    JUMP,
    LAND,
    FOOTSTEP,
    HURT,
    CARDSHUFFLE,
    CARDDEAL
}
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;

    
    void Awake(){
        if(instance == null){

            instance = this;
            
            //DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }

/*#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] name = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, name.Length);
        for(int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = name[i];
        }
    }
#endif*/

}

/*[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}*/