using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Cat : MonoBehaviour
{
    public static event Action OnCatClicked;

    [SerializeField] private float _punchScaleAmount = .2f;
    [SerializeField] private float _punchDuration = .5f;
    [SerializeField] private int _vibrato = 10;
    [SerializeField] private float _elasticity = 1f;
    private AudioSource _punchSound;

    private Image _image;
    private bool _isTweening = false;

    private void Awake()
    {
        _punchSound = GetComponent<AudioSource>();
        _image = GetComponent<Image>();
    }

    private float _lastClickTime = 0f;
    private float _minClickInterval = 0.1f; // 0.1 saniye = saniyede maksimum 10 tıklama

    public void CatClicked()
    {
        _punchSound.Play();
        float currentTime = Time.time;
        float clickInterval = currentTime - _lastClickTime;

        // Eğer tıklama hızı çok yüksekse engelle
        if (clickInterval < _minClickInterval)
        {
            Debug.LogWarning("Makro tıklama algılandı!");
            return; // Tıklama işleme alınmaz
        }

        _lastClickTime = currentTime;

        // Normal tıklama işlemleri
        if (!_isTweening)
        {
            _isTweening = true;
            _image.rectTransform
                .DOPunchScale(Vector3.one * _punchScaleAmount, _punchDuration, _vibrato, _elasticity)
                .OnComplete(() => _isTweening = false);
        }

        OnCatClicked?.Invoke();
    }

}
