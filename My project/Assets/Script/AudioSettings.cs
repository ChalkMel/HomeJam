using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings instance;

    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadSettings();

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        float dbVolume = Mathf.Log10(volume) * 20;

        if (volume <= 0.001f) 
            dbVolume = -80f;

        audioMixer.SetFloat("MusicVolume", dbVolume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        float dbVolume = Mathf.Log10(volume) * 20;

        if (volume <= 0.001f)
            dbVolume = -80f;

        audioMixer.SetFloat("SFXVolume", dbVolume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    void LoadSettings()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume",1f);

        SetMusicVolume(savedMusicVolume);
        SetSFXVolume(savedSFXVolume);

        if (musicSlider != null)
            musicSlider.value = savedMusicVolume;

        if (sfxSlider != null)
            sfxSlider.value = savedSFXVolume;
    }
}