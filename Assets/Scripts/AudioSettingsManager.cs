using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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

    private int currentSongIndex = 0;

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);

        UpdateSongDisplay();
        
        leftArrow.onClick.AddListener(PreviousSong);
        rightArrow.onClick.AddListener(NextSong);
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
}