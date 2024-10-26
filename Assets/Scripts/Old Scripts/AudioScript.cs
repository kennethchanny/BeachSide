using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioScript : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip[] audioClipArray;
    public AudioClip[] audioClipArray2;
    public AudioClip[] audioClipArray3;
    public AudioClip[] audioClipArray4;




    //public int AudioChoice;
    AudioClip lastClip;

    //Lines to add into other scripts: 
        //private AudioScript Audio_ref;
        //Audio_ref = gameObject.GetComponent<AudioScript>();
        //Audio_ref.playAudio();


    void Start()
    {

        //load all audio
    }

    //Play random audio
    public void playAudio()
    {
        if(audioClipArray.Length > 0)
            audioSource.PlayOneShot(RandomClip());
    }

    //prevent same audio from playing twice
    AudioClip RandomClip()
    {
        int attempts = 3;
        AudioClip newClip = audioClipArray[Random.Range(0, audioClipArray.Length)];

        while (newClip == lastClip && attempts > 0)
        {
            newClip = audioClipArray[Random.Range(0, audioClipArray.Length)];
            attempts--;
        }

        lastClip = newClip;
        return newClip;
    }

    //2nd Slot 
    public void playAudio2()
    {
        if(audioClipArray.Length > 0)
        audioSource.PlayOneShot(RandomClip2());
    }

    AudioClip RandomClip2()
    {
        int attempts = 5;
        AudioClip newClip = audioClipArray2[Random.Range(0, audioClipArray2.Length)];

        while (newClip == lastClip && attempts > 0)
        {
            newClip = audioClipArray2[Random.Range(0, audioClipArray2.Length)];
            attempts--;
        }

        lastClip = newClip;
        return newClip;
    }



    public void playAudio3()
    {
        if (audioClipArray.Length > 0)
            audioSource.PlayOneShot(RandomClip3());
    }

    AudioClip RandomClip3()
    {
        int attempts = 10;
        AudioClip newClip = audioClipArray3[Random.Range(0, audioClipArray3.Length)];

        while (newClip == lastClip && attempts > 0)
        {
            newClip = audioClipArray3[Random.Range(0, audioClipArray3.Length)];
            attempts--;
        }

        lastClip = newClip;
        return newClip;
    }

    public void playAudio4()
    {
        if (audioClipArray.Length > 0)
            audioSource.PlayOneShot(RandomClip4());
    }

    AudioClip RandomClip4()
    {
        int attempts = 10;
        AudioClip newClip = audioClipArray4[Random.Range(0, audioClipArray4.Length)];

        while (newClip == lastClip && attempts > 0)
        {
            newClip = audioClipArray4[Random.Range(0, audioClipArray4.Length)];
            attempts--;
        }

        lastClip = newClip;
        return newClip;
    }


    /*
    public void playOneAudio()
    {
        audioSource.PlayOneShot(SpecificClip(AudioChoice));
    }
    */

    /*
    AudioClip SpecificClip(int AudioChoice)
    {
        AudioClip newClip = audioClipArray[AudioChoice];
        return newClip;
    }
    */
}
