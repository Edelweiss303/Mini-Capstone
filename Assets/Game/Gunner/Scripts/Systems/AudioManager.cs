using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager>
{
    public List<Sound> Sounds;

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);

        foreach (Sound sound in Sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.looping;
        }
    }

    public void PlaySound(string name)
    {
        Sound selectedSound = Sounds.Find(s => s.name == name);

        if (selectedSound == null)
        {
            Debug.LogWarning("Sound: " + name + "not found.");
            return;
        }

        selectedSound.source.Play();

    }
}
