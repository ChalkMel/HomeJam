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
        // Загружаем сохранённые настройки
        LoadSettings();

        // Настраиваем слайдеры
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        // Конвертируем значение слайдера (0-1) в децибелы (-80 до 0)
        float dbVolume = Mathf.Log10(volume) * 20;

        if (volume <= 0.001f) // Если почти 0, делаем тише
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
        // Загружаем сохранённые значения или используем значения по умолчанию
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        // Применяем настройки
        SetMusicVolume(savedMusicVolume);
        SetSFXVolume(savedSFXVolume);

        // Устанавливаем значения слайдеров
        if (musicSlider != null)
            musicSlider.value = savedMusicVolume;

        if (sfxSlider != null)
            sfxSlider.value = savedSFXVolume;
    }
}