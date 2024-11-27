using TMPro;
using UnityEngine;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(string text, Vector2 position, float duration)
    {
        _text.text = text;
        _rectTransform.anchoredPosition = position;

        // Yükseklik animasyonu
        _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + 150f, duration);

        // Opaklık animasyonu
        _text.DOFade(0, duration).OnComplete(() =>
        {
            Destroy(gameObject); // Animasyon bitince objeyi yok et
        });
    }
}