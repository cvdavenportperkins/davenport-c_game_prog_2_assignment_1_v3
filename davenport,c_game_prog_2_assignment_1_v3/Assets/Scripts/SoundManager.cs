using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager SoundMan;
    public AudioSource AS;
    public AudioClip BoundarySFX, DecaySFX, PickUpSFX, ComboSFX, ScoreSFX, SpawnSFX, DespawnSFX;

    void Awake()
    {
        if (SoundMan == null)
        {
            SoundMan = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        AS.PlayOneShot(clip);
    }
}
