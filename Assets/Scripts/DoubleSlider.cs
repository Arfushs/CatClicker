using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening; // DoTween kütüphanesi

public class DoubleSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float decreaseAmount = .2f;
    [SerializeField] private float decreaseSecond = .2f;
    [SerializeField] private float increaseAmount = 1f;
    [SerializeField] private float lowDecreaseAmount = 0.5f;
    [SerializeField] private TextMeshProUGUI totalCountText;
    private Slider slider;
    [SerializeField] private RectTransform handle;

    private Tween handleTween; // Tween referansı
    private Vector3 defaultScale; // Handle'ın varsayılan scale değeri

    private void Awake()
    {
        slider = GetComponent<Slider>();
        defaultScale = handle.localScale; // Varsayılan scale değerini kaydet
    }

    private void Start()
    {
        StartCoroutine(DecreaseCoroutine());
    }

    private void OnEnable()
    {
        Cat.OnCatClicked += OnCatClicked;
    }

    private void Update()
    {
        if (slider.value >= slider.maxValue - .5f)
        {
            GameManager.Instance.SetIsPowerOn(true);
            textMesh.color = new Color32(255, 135, 2, 255);
            totalCountText.color = new Color32(255, 135, 2, 255);

            // Handle büyüyüp küçülme animasyonu başlat
            if (handleTween == null || !handleTween.IsActive() || !handleTween.IsPlaying())
            {
                StartHandleAnimation();
            }
        }
        else
        {
            GameManager.Instance.SetIsPowerOn(false);
            textMesh.color = Color.white;
            totalCountText.color = Color.white;

            // Handle animasyonunu durdur
            StopHandleAnimation();
        }
    }

    private IEnumerator DecreaseCoroutine()
    {
        while (true)
        {
            if (slider.value > 0)
            {
                if (slider.value >= slider.maxValue - 0.5f)
                {
                    slider.value -= lowDecreaseAmount;
                }
                else
                {
                    slider.value -= decreaseAmount;
                }
            }
            yield return new WaitForSeconds(decreaseSecond);
        }
    }

    private void OnCatClicked()
    {
        slider.value += increaseAmount;
    }

    private void OnDisable()
    {
        Cat.OnCatClicked -= OnCatClicked;
    }

    private void StartHandleAnimation()
    {
        // Handle büyüyüp küçülme animasyonu, varsayılan scale değerine göre ayarlanıyor
        handleTween = handle.DOScale(defaultScale * 1.2f, 0.5f) // Varsayılan scale'in %120'sine büyüt
            .SetLoops(-1, LoopType.Yoyo) // Sürekli büyü-küçül
            .SetEase(Ease.InOutSine); // Yumuşak geçiş
    }

    private void StopHandleAnimation()
    {
        if (handleTween != null && handleTween.IsActive())
        {
            handleTween.Kill(); // Animasyonu durdur
            handle.localScale = defaultScale; // Handle'ı varsayılan scale boyutuna döndür
        }
    }
}
