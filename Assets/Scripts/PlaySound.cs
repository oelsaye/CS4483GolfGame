using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource src;
    public AudioClip click;
    public AudioClip background;
    public AudioClip start;

    public void Button_sound(){
        src.clip = click;
        src.Play();
    }

    public void PlayBackground(){
        src.clip = background;
        src.Play();
    }
    
    public void StopBackground(){
        src.clip = background;
        src.Stop();
    }
}
