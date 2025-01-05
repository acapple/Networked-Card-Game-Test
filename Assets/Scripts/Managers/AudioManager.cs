using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //List of every audio queue we have in the game.
    internal enum AudioQueue
    {
        LENGTH
    }

    internal static AudioManager instance;

    [SerializeField]
    private int numberAudioPlayers = 5;

    [SerializeField]
    private Sound[] GameSounds;
    [SerializeField]
    private Track[] music;
    private AudioSource MusicSource;
    IEnumerator trackSwitching;

    private SoundList[] soundDirectory;

    private int currentSource = 0;
    private AudioSource[] sources;
    [SerializeField]
    private AudioCool[] AudioCooldown;
    float[] audioCooldownTimer;

    [Header("Volumes")]
    [SerializeField]
    [Range(0, 1)]
    float masterVolume = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float musicVolume = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float sfxVolume = 0.5f;




    private void Awake()
    {
        if (instance == null) instance = this;
        sources = new AudioSource[numberAudioPlayers];
        GameObject go;
        for (int i = 0; i < numberAudioPlayers; i++)
        {
            go = new GameObject();
            go.transform.parent = transform;
            go.AddComponent<AudioSource>();
            sources[i] = go.GetComponent<AudioSource>();
        }

        //Create directory
        soundDirectory = new SoundList[(int)AudioQueue.LENGTH];
        for (int i = 0; i < soundDirectory.Length; i++)
        {
            soundDirectory[i] = new SoundList();
        }
        for (int i = 0; i < GameSounds.Length; i++)
        {
            soundDirectory[(int)GameSounds[i].queue].AddSound(i);
        }

        go = new GameObject();
        go.transform.parent = transform;
        go.AddComponent<AudioSource>();
        MusicSource = go.GetComponent<AudioSource>();
        MusicSource.loop = true;
        MusicSource.clip = music[0].clip;
        MusicSource.pitch = music[0].pitch;
        MusicSource.volume = music[0].volume * musicVolume * masterVolume;
        MusicSource.outputAudioMixerGroup = music[0].mixer;
        MusicSource.Play();
        audioCooldownTimer = new float[(int)AudioQueue.LENGTH];
    }

    private void Update()
    {
        for (int i = 0; i < (int)audioCooldownTimer.Length; i++)
        {
            audioCooldownTimer[i] += Time.deltaTime;
        }
    }

    private bool doneCooldown(AudioQueue queue)
    {
        foreach (AudioCool cooldown in AudioCooldown)
        {
            if (cooldown.queue != queue) continue;
            if (cooldown.cooldown > audioCooldownTimer[(int)queue]) return false;
        }
        return true;
    }

    public void restartMusic(float volume)
    {

    }

    internal void TransitionTrack(string name)
    {
        if (trackSwitching == null)
        {
            trackSwitching = Transitions(name);
            StartCoroutine(trackSwitching);
        }

    }


    private IEnumerator Transitions(string name)
    {
        GameObject go = new GameObject();
        go.transform.parent = transform;
        go.AddComponent<AudioSource>();
        AudioSource newSource = go.GetComponent<AudioSource>();
        newSource.loop = true;
        newSource.volume = 0;
        int trackNum = 0;
        float oldVolume = MusicSource.volume;

        for (int i = 0; i < music.Length; i++)
        {
            if (name == music[i].trackName)
            {
                newSource.clip = music[i].clip;
                newSource.pitch = music[i].pitch;
                newSource.outputAudioMixerGroup = music[i].mixer;
                newSource.volume = 0;
                newSource.Play();
                trackNum = i;
                break;
            }
        }
        if (newSource.clip == null)
        {
            Destroy(go);
            Debug.LogWarning("A music track with the name " + name + " doesn't exist.");
            StopCoroutine(trackSwitching);
            Debug.LogError("Something went wrong. Aidan thought that the music transition couldn't get here.");
        }

        while (newSource.volume < music[trackNum].volume * musicVolume * masterVolume)
        {
            yield return null;
            float transitionTime = music[trackNum].transitionTimeIn;
            if (transitionTime == 0) transitionTime = 2f;
            MusicSource.volume -= Time.deltaTime * oldVolume / transitionTime;
            if (MusicSource.volume < 0)
            {
                MusicSource.volume = 0;
            }
            newSource.volume += Time.deltaTime * music[trackNum].volume * musicVolume * masterVolume / transitionTime;
            if (newSource.volume > music[trackNum].volume * musicVolume * masterVolume)
            {
                newSource.volume = music[trackNum].volume * musicVolume * masterVolume;
            }
        }
        go = MusicSource.gameObject;
        MusicSource = newSource;
        Destroy(go);
        trackSwitching = null;
    }


    internal void PlaySound(AudioQueue queue, float delay)
    {
        StartCoroutine(sfxDelay(queue, delay));
    }

    internal IEnumerator sfxDelay(AudioQueue queue, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySound(queue);
    }


    /// <summary>
    /// Attempts to play a sound based on the given Audio queue
    /// </summary>
    internal AudioSource PlaySound(AudioQueue queue)
    {
        if (!doneCooldown(queue)) return null;
        audioCooldownTimer[(int)queue] = 0;
        bool soundPlayed = false;
        for (int i = 0; i < numberAudioPlayers; i++)
        {
            int usedSource = (i + currentSource) % numberAudioPlayers;
            if (sources[usedSource].isPlaying)
            {
                continue;
            }
            currentSource = usedSource;
            soundPlayed = true;
            //PLay sound here
            Sound s = getSound(queue);
            if (s.clip == null)
            {
                Debug.LogWarning("Attempting to play sound " + queue + " but there are no sounds for it.");
                return null;
            }

            sources[usedSource].clip = s.clip;
            sources[usedSource].volume = s.volume * sfxVolume * masterVolume;
            sources[usedSource].pitch = s.pitch + Random.Range(s.pitchMinMax.x, s.pitchMinMax.y);
            sources[usedSource].outputAudioMixerGroup = s.mixer;

            sources[usedSource].Play();
            return sources[usedSource];
        }
        if (!soundPlayed)
        {
            Debug.LogWarning("Attemped to play a sound, but all AudioSources were in use.");
        }
        return null;
    }




    private Sound getSound(AudioQueue queue)
    {
        return getSound(queue, Random.Range(0, soundDirectory[(int)queue].sounds.Count));
    }

    private Sound getSound(AudioQueue queue, int soundChosen)
    {
        if (soundDirectory[(int)queue].sounds.Count == 0) return new Sound();
        return GameSounds[soundDirectory[(int)queue].sounds[soundChosen]];
    }



}





/// <summary>
/// This class contains a list of integers that refer to where in the GameSounds Array the sound is
/// </summary>
internal class SoundList
{
    internal List<int> sounds;

    internal SoundList()
    {
        sounds = new List<int>();
    }

    internal void AddSound(int s)
    {
        sounds.Add(s);
    }
}



/// <summary>
/// This contains a single sound, audioQueue and all.
/// </summary>
[System.Serializable]
internal struct Sound
{
    [SerializeField]
    internal AudioManager.AudioQueue queue;
    [SerializeField]
    internal AudioClip clip;
    [SerializeField]
    [Range(0, 1)]
    internal float volume;
    [SerializeField]
    [Range(0.1f, 3)]
    internal float pitch;
    [SerializeField]
    internal Vector2 pitchMinMax;
    [SerializeField]
    internal AudioMixerGroup mixer;
}

[System.Serializable]
internal struct AudioCool
{
    [SerializeField]
    internal AudioManager.AudioQueue queue;
    [SerializeField]
    internal float cooldown;
}

[System.Serializable]
internal struct Track
{
    [SerializeField]
    internal string trackName;
    [SerializeField]
    internal AudioClip clip;
    [SerializeField]
    [Range(0, 1)]
    internal float volume;
    [SerializeField]
    [Range(0.1f, 3)]
    internal float pitch;
    [SerializeField]
    internal AudioMixerGroup mixer;
    [SerializeField]
    internal float transitionTimeIn;
}