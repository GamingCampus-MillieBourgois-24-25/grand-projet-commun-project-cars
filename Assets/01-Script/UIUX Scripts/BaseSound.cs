using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BaseSound : MonoBehaviour
{
    [SerializeField] private AudioMixer Game;

    // Start is called before the first frame update
    void Start()
    {
        Game.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 0.5f)) * 20);
        Game.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", 0.5f)) * 20);
    }
}
