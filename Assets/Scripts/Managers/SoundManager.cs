using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private GameObject soundPlayer = null;

    [SerializeField] private AudioClip[] tracks = null;
    [SerializeField] private AudioClip[] sounds = null;

    [SerializeField] private AudioSource currentMusic = null;

    private Dictionary<string, AudioClip> musicTracks = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        InitializeDictionaries();
    }

    // Start to fade between two tracks. No need to fade if the track doesn't change.
    public void PlayMusic(string trackName, float volume, float fadeOut, float fadeIn)
    {
        if (currentMusic.clip != musicTracks[trackName])
            StartCoroutine(FadeMusic(trackName, volume, fadeOut, fadeIn));
    }

    // Fade from one track to another.
    private IEnumerator FadeMusic(string trackName, float volume, float fadeOut, float fadeIn)
    {
        float startVolume = currentMusic.volume;

        float t = 0;
        // Fade old music out if there is any.
        if (currentMusic.clip != null)
        {
            while (t < fadeOut)
            {
                float currentVolume = Mathf.Lerp(startVolume, 0f, t / fadeOut);
                currentMusic.volume = currentVolume;

                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            currentMusic.volume = 0f;

            yield return new WaitForSeconds(0.5f);
        }

        currentMusic.clip = musicTracks[trackName];
        currentMusic.Play();

        t = 0;
        // Fade new music in.
        while (t < fadeIn)
        {
            float currentVolume = Mathf.Lerp(0f, volume, t / fadeIn);
            currentMusic.volume = currentVolume;

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        currentMusic.volume = volume;
    }

    // Player a sound with given settings.
    public void PlaySound(string clipName, float volume, float basePitch, float pitchVariance)
    {
        if (instance == null)
            return;

        AudioSource audio = Instantiate(soundPlayer, transform).GetComponent<AudioSource>();

        float pitch = Random.Range(basePitch - pitchVariance / 2f, basePitch + pitchVariance / 2f);
        audio.pitch = pitch;

        audio.volume = volume;
        AudioClip clip = soundEffects[clipName];

        audio.PlayOneShot(clip);
        Destroy(audio.gameObject, clip.length);
    }

    // Put all the sounds & music track on their own dictionaries
    // so we can find them with their names instead of index numbers.
    private void InitializeDictionaries()
    {
        for (int i = 0; i < tracks.Length; i++)
            musicTracks.Add(tracks[i].name, tracks[i]);

        for (int i = 0; i < sounds.Length; i++)
            soundEffects.Add(sounds[i].name, sounds[i]);
    }
}
