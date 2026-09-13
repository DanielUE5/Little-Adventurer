using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_music : MonoBehaviour
{
    public AudioSource introSorce, loopSorce;

    // Start is called before the first frame update
    void Start()
    {
        introSorce.Play();
        loopSorce.PlayScheduled(AudioSettings.dspTime + introSorce.clip.length);
    }
}
