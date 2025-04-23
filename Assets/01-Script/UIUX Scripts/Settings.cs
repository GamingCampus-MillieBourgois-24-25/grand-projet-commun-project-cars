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
    }

    public void ChangeVolumeMusic()
    {
        float volume = MusicSlider.value;
        Game.SetFloat("Music", Mathf.Log10(MusicSlider.value) * 20);
        //GameInstance.instance.SetMusicVolume(MusicSlider.value);
    }

    public void ChangeVolumeSFX()
    {
        float volume = EffectsSlider.value;
        Game.SetFloat("SFX", Mathf.Log10(EffectsSlider.value) * 20);
        //GameInstance.instance.SetMasterVolume(EffectsSlider.value);
    }
}