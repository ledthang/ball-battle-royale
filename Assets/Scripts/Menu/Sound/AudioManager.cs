using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    [SerializeField] Slider backgroundVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] AudioMixer masterMixer;
    [SerializeField] AudioClip buttonAudio;
    [SerializeField] AudioClip powerupPickupAudio;
    [SerializeField] AudioClip collisonAudio;
    [SerializeField] AudioClip gameOverAudio;
    [SerializeField] PowerupAudio[] powerupAudios;

    [SerializeField] AudioMixerGroup sfxMixerGroup;
    [SerializeField] public AudioMixerSnapshot muteSnapshot;
    [SerializeField] public AudioMixerSnapshot normalSnapshot;

    [System.Serializable]
    class PowerupAudio
    {
        public PowerupType powerupType;
        public AudioClip audioClip;
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        if (!PlayerPrefs.HasKey("masterVol"))
        {
            PlayerPrefs.SetFloat("masterVol", -25);
            PlayerPrefs.SetFloat("backgroundVol", -25);
            PlayerPrefs.SetFloat("sfxVol", -25);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void ChangeBackgroundVolume(float vol)
    {
        masterMixer.SetFloat("backgroundVol", vol);
        Save();
    }

    public void ChangeSfxVolume(float vol)
    {
        masterMixer.SetFloat("sfxVol", vol);
        Save();
    }

    private void Load()
    {
        backgroundVolumeSlider.value = PlayerPrefs.GetFloat("backgroundVol");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVol");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("backgroundVol", backgroundVolumeSlider.value);
        PlayerPrefs.SetFloat("sfxVol", sfxVolumeSlider.value);
    }

    public void PlayButtonClicked()
    {
        float vol = 1;
        PlayClipAtPoint(buttonAudio, this.transform.position, vol, sfxMixerGroup);
    }

    public void PlayPowerupPickupSfx()
    {
        float vol = 1;
        PlayClipAtPoint(powerupPickupAudio, this.transform.position, vol, sfxMixerGroup);
    }

    public void PlayPowerupSfx(PowerupType powerup)
    {
        float vol = 1;
        PlayClipAtPoint(GetPowerupSfxAudio(powerup), this.transform.position, vol, sfxMixerGroup);
    }

    AudioClip GetPowerupSfxAudio(PowerupType powerup)
    {
        foreach (PowerupAudio powerupAudio in powerupAudios)
        {
            if (powerupAudio.powerupType == powerup)
            {
                return powerupAudio.audioClip;
            }
        }
        return null;
    }

    public void PlayCollsionWithEnemySfx(Vector3 pos)
    {
        float vol = 1;
        PlayClipAtPoint(collisonAudio, pos, vol, sfxMixerGroup);
    }
    public void PlayCollsionWithPlayerSfx(Vector3 pos)
    {
        float vol = 1;
        PlayClipAtPoint(collisonAudio, pos, vol, sfxMixerGroup);
    }

    public void PlayGameOver()
    {
        float vol = 1;
        PlayClipAtPoint(gameOverAudio, this.transform.position, vol, sfxMixerGroup);
    }

    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1.0f, AudioMixerGroup group = null)
    {
        if (clip == null) return;
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        if (group != null)
            audioSource.outputAudioMixerGroup = group;
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();
        Object.Destroy(gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
}
