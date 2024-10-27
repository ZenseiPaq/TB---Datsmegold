using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private AudioSource[] musicSources;
    [SerializeField] private AudioSource[] sfxSources;

    [SerializeField] private TextMeshProUGUI songTitleText;
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;

    [SerializeField] private int specificSongIndexForStartScene = 0;
    private int currentSongIndex = 0;

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Start")
        {
            currentSongIndex = specificSongIndexForStartScene;
            UpdateSongDisplay();
        }
        else
        {
            currentSongIndex = Random.Range(0, musicSources.Length);
            UpdateSongDisplay();
        }

        leftArrow.onClick.AddListener(PreviousSong);
        rightArrow.onClick.AddListener(NextSong);
    }

    private void UpdateSongDisplay()
    {
        songTitleText.text = musicSources[currentSongIndex].clip.name;
        PlayCurrentSong();
    }

    private void PlayCurrentSong()
    {
        foreach (var musicSource in musicSources)
        {
            musicSource.Stop();
        }

        musicSources[currentSongIndex].Play();
        StartCoroutine(WaitForSongEnd());
    }

    private IEnumerator WaitForSongEnd()
    {
        while (musicSources[currentSongIndex].isPlaying)
        {
            yield return null;
        }

        PlayNextRandomSong();
    }

    public void NextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % musicSources.Length;
        UpdateSongDisplay();
    }

    public void PreviousSong()
    {
        currentSongIndex = (currentSongIndex - 1 + musicSources.Length) % musicSources.Length;
        UpdateSongDisplay();
    }

    private void PlayNextRandomSong()
    {
        int newSongIndex;
        do
        {
            newSongIndex = Random.Range(0, musicSources.Length);
        }
        while (newSongIndex == currentSongIndex);

        currentSongIndex = newSongIndex;
        UpdateSongDisplay();
    }

    private void SetMusicVolume(float volume)
    {
        foreach (var musicSource in musicSources)
        {
            musicSource.volume = volume;
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void SetSFXVolume(float volume)
    {
        foreach (AudioSource source in sfxSources)
        {
            source.volume = volume;
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}