/**********************************************************************************

// File Name :         AudioManager.cs
// Author :            Marissa Moser
// Creation Date :     October 16, 2023
//
// Brief Description : 

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance;

    /// <summary>
    /// Start function assigns components to the sound class objects
    /// </summary>
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixer;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    /// <summary>
    /// Function called from anywhere to play sound 
    /// </summary>
    /// <param name="name"></param>
    public void Play(string name)
    {
        Sound s = System.Array.Find(sounds, sounds => sounds.name == name);
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = System.Array.Find(sounds, sounds => sounds.name == name);
        s.source.Stop();
    }
}