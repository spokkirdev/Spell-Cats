using System.Collections;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;

    [Header("Playlist")]
    [SerializeField] private AudioClip song1;
    [SerializeField] private AudioClip song2;
    [SerializeField] private AudioClip song3;
    [SerializeField] private AudioClip song4;
    [SerializeField] private AudioClip song5;

    [SerializeField] private float crossfadeDuration = 3f;

    private AudioClip[] playlist;
    private AudioSource currentSource;
    private AudioSource nextSource;
    private int currentSongIndex = 0;


    private void Start()
    {
        playlist = new AudioClip[]
        {
            song1,
            song2,
            song3,
            song4,
            song5
        };

        currentSource = sourceA;
        nextSource = sourceB;

        StartCoroutine(PlaySongsWithCrossfade());
    }

    private IEnumerator PlaySongsWithCrossfade()
    {
        currentSource.clip = playlist[currentSongIndex];
        currentSource.volume = 1f;
        currentSource.Play();

        while (true)
        {
            // Wait until it's time to crossfade
            yield return new WaitForSeconds(currentSource.clip.length - crossfadeDuration);

            currentSongIndex = (currentSongIndex + 1) % playlist.Length;

            nextSource.clip = playlist[currentSongIndex];
            nextSource.volume = 0f;
            nextSource.Play();

            float timer = 0f;

            while (timer < crossfadeDuration)
            {
                timer += Time.deltaTime;
                float t = timer / crossfadeDuration;

                currentSource.volume = 1f - t;
                nextSource.volume = t;

                yield return null;
            }

            currentSource.Stop();
            currentSource.volume = 1f;

            // Swap the sources
            AudioSource temp = currentSource;
            currentSource = nextSource;
            nextSource = temp;
        }
    }

    private void PauseMusic()
    {
        currentSource.Pause();
        nextSource.Pause();
    }

    private void ResumeMusic()
    {
        currentSource.UnPause();
        nextSource.UnPause();
    }

    public void ToggleMuteMusic()
    {
        if (currentSource.volume > 0f)
        {
            currentSource.volume = 0f;
            nextSource.volume = 0f;
        }
        else
        {
            currentSource.volume = 1f;
            nextSource.volume = 1f;
        }
    }
}