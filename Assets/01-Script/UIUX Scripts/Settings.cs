using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer Game;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider EffectsSlider;
    [SerializeField] private Button[] Controls;
    [SerializeField] private Button ControlActive;

    private void Start()
    {
        for (int i = 0; i < Controls.Length; i++)
        {
            Controls[i].interactable = false;
        }
        ColorBlock cb = ControlActive.colors;
        cb.normalColor = Color.white;
        ControlActive.colors = cb;
        MusicSlider.value = PlayerPrefs.GetFloat("Music", 0.5f);
        Game.SetFloat("Music", Mathf.Log10(MusicSlider.value) * 20);
        EffectsSlider.value = PlayerPrefs.GetFloat("SFX", 0.5f);
        Game.SetFloat("SFX", Mathf.Log10(EffectsSlider.value) * 20);
    }

    public void ChangeVolumeMusic()
    {
        float volume = MusicSlider.value;
        Game.SetFloat("Music", Mathf.Log10(MusicSlider.value) * 20);
        PlayerPrefs.SetFloat("Music", MusicSlider.value);
        PlayerPrefs.Save();
    }

    public void ChangeVolumeSFX()
    {
        float volume = EffectsSlider.value;
        Game.SetFloat("SFX", Mathf.Log10(EffectsSlider.value) * 20);
        PlayerPrefs.SetFloat("SFX", EffectsSlider.value);
        PlayerPrefs.Save();
    }
}