using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private long _autoCatValue;
    [SerializeField] private long _catPowerValue;
    [SerializeField] private long _buyValue;
    
    private AudioSource _audioSource;
    private string _playerPrefKey;
    
    private int _buyCount = 0;
    private Button _button; 
    
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _countText;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerPrefKey = gameObject.name;
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _buyCount = PlayerPrefs.GetInt(_playerPrefKey, 0);
        _countText.text = _buyCount.ToString();
    }

    public void ButtonClicked()
    {
        if(_buyValue > GameManager.Instance.GetCurrentCats())
            return;
        
        _audioSource.Play();
        GameManager.Instance.RemoveCats(_buyValue);
        _buyCount++;
        _countText.text = _buyCount.ToString();
        PlayerPrefs.SetInt(_playerPrefKey, _buyCount);
        GameManager.Instance.AddAutoCats(_autoCatValue);
        GameManager.Instance.AddCatPower(_catPowerValue);
        
    }

    public void ResetItem()
    {
        PlayerPrefs.SetInt(_playerPrefKey, 0);
        _buyCount = 0;
    }

    private void Update()
    {
        if (_buyCount < 1)
        {
            if (long.Parse(PlayerPrefs.GetString("MaxCats", "0")) < _buyValue)
            {
                _button.interactable = false;
                _iconImage.color = Color.black;
            }
            else
            {
                if(GameManager.Instance.GetCurrentCats() >= _buyValue)
                    _button.interactable = true;
                else
                    _button.interactable = false;
                _iconImage.color = Color.white;
            }
        }
        else 
        {
            if (GameManager.Instance.GetCurrentCats() >= _buyValue)
                _button.interactable = true;
            else
            {
                _button.interactable = false;
            }
        }
    }
}