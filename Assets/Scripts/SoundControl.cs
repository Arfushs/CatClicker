using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundControl : MonoBehaviour
{
    [SerializeField] private GameObject OnIcon;
    [SerializeField] private GameObject OffIcon;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private String ExposedParam;
    private bool isOn = true;

    public void UpdateButton()
    {
        isOn = !isOn;
        if (isOn == true)
        {
            OnIcon.SetActive(true);
            OffIcon.SetActive(false);
            mixer.SetFloat(ExposedParam, 0f);
        }
        else
        {
            OnIcon.SetActive(false);
            OffIcon.SetActive(true);
            mixer.SetFloat(ExposedParam, -80f);
        }
            
    }
    
}
