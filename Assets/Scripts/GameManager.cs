using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI _autoClickText;
    [SerializeField] private TextMeshProUGUI _clickdPowerText;
    [SerializeField] private TextMeshProUGUI _currentClickText;
    [SerializeField] private FloatingText _floatingTextPrefab;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TextMeshProUGUI _currentEvolutionText;
    [SerializeField] private TextMeshProUGUI _nextEvolutionText;
    [SerializeField] private GameObject _itemContainer;
    [SerializeField] private GameObject _nextEvolveButton;
    [SerializeField] private Animator _fadeoutAnimator;
    
    private long _clickedPower = 1;
    private long _clickedCountBySec = 0;
    private long _clickedCount = 0;

    private bool _isPowerOn = false;
    private bool _isAutoClickRunning = true;
    private double _evolveMultiplier = 1.0;  // Ondalık olarak güncellendi
    private double _nextEvolveMultiplier = 1.0; // Ondalık olarak güncellendi

    private void Awake()
    {
        Instance = this;
        if (Instance != this)
            Destroy(this);

        if (!PlayerPrefs.HasKey("AutoCat"))
        {
            PlayerPrefs.SetString("AutoCat", "0");
        }

        if (!PlayerPrefs.HasKey("CatPower"))
        {
            PlayerPrefs.SetString("CatPower", "1");
        }

        if (!PlayerPrefs.HasKey("EvolveMultiplier"))
        {
            PlayerPrefs.SetFloat("EvolveMultiplier", 1.0f); // PlayerPrefs float destekliyor
        }
    }

    void Start()
    {
        LoadStats();
        UpdateTexts();
        StartAutoClick();
    }

    private void OnEnable()
    {
        Cat.OnCatClicked += CatClicked;
    }

    private void OnDisable()
    {
        Cat.OnCatClicked -= CatClicked;
    }

    public void CatClicked()
    {
        if (_isPowerOn)
            _clickedCount += (long)(2 * _clickedPower * _evolveMultiplier);
        else
            _clickedCount += (long)(1 * _clickedPower * _evolveMultiplier);
        UpdateTexts();
        SaveClicks();

        Vector2 clickPosition = GetClickPositionInCanvas();

        long textNumber = _isPowerOn ? (long)(2 * _clickedPower * _evolveMultiplier) : (long)(_clickedPower * _evolveMultiplier);
        ShowFloatingText(clickPosition, "+" + textNumber);
        CheckNextEvolution();
    }

    private Vector2 GetClickPositionInCanvas()
    {
        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );

        return localPoint;
    }

    private void ShowFloatingText(Vector2 position, string text)
    {
        var floatingText = Instantiate(_floatingTextPrefab, _canvas.transform);
        var rectTransform = floatingText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        floatingText.GetComponent<FloatingText>().Initialize(text, position, 1.5f);
    }

    private void UpdateTexts()
    {
        _autoClickText.text = "Auto Cats: " + _clickedCountBySec.ToString("N0");
        _currentClickText.text = _clickedCount.ToString("N0");
        _clickdPowerText.text = "Cat Power: " + _clickedPower.ToString("N0");
        _currentEvolutionText.text = $"EVOLVE MULTIPLIER: {_evolveMultiplier:F2}x"; // Ondalık format
    }

    public void SetIsPowerOn(bool isPowerOn) => _isPowerOn = isPowerOn;

    private IEnumerator AutoClickCoroutine()
    {
        while (_isAutoClickRunning)
        {
            _clickedCount += (long)(1 * _clickedCountBySec * _evolveMultiplier);
            UpdateTexts();
            SaveClicks();
            CheckNextEvolution();
            yield return new WaitForSeconds(1f);
        }
    }

    public void StartAutoClick()
    {
        _isAutoClickRunning = true;
        StartCoroutine(AutoClickCoroutine());
    }

    public void StopAutoClick()
    {
        _isAutoClickRunning = false;
    }

    public void AddAutoCats(long amount)
    {
        _clickedCountBySec += amount;
        PlayerPrefs.SetString("AutoCat", _clickedCountBySec.ToString());
        UpdateTexts();
    }

    public void AddCatPower(long amount)
    {
        _clickedPower += amount;
        PlayerPrefs.SetString("CatPower", _clickedPower.ToString());
        UpdateTexts();
    }

    public void RemoveCats(long amount)
    {
        _clickedCount -= amount;
        SaveClicks();
        UpdateTexts();
    }

    public long GetCurrentCats() => _clickedCount;

    private void SaveClicks()
    {
        PlayerPrefs.SetString("CurrentCats", _clickedCount.ToString());
        if (_clickedCount > long.Parse(PlayerPrefs.GetString("MaxCats", "0")))
        {
            PlayerPrefs.SetString("MaxCats", _clickedCount.ToString());
        }
        PlayerPrefs.SetFloat("EvolveMultiplier", (float)_evolveMultiplier); // PlayerPrefs float ile kaydet
    }

    private void LoadStats()
    {
        _clickedPower = long.Parse(PlayerPrefs.GetString("CatPower", "1"));
        _clickedCount = long.Parse(PlayerPrefs.GetString("CurrentCats", "0"));
        _clickedCountBySec = long.Parse(PlayerPrefs.GetString("AutoCat", "0"));
        _evolveMultiplier = PlayerPrefs.GetFloat("EvolveMultiplier", 1.0f); // Float olarak yükle
    }

    private void CheckNextEvolution()
    {
        long totalPower = _clickedPower + _clickedCountBySec;

        // Dinamik growthRate ve scalingFactor ayarları
        float baseThreshold = 1000f;
        float growthRate = totalPower > 100_000_000 ? 0.0003f : 
            totalPower > 50_000_000 ? 0.0005f : 
            totalPower > 10_000_000 ? 0.001f : 0.005f;
        float scalingFactor = totalPower > 100_000_000 ? 0.25f : 
            totalPower > 50_000_000 ? 0.2f : 
            totalPower > 10_000_000 ? 0.15f : 0.1f;

        // Gelişmiş dinamik eşik değeri
        float dynamicThreshold = baseThreshold + (growthRate * Mathf.Sqrt(totalPower));

        // _nextEvolveMultiplier hesaplama
        _nextEvolveMultiplier = (totalPower / dynamicThreshold) * scalingFactor;

        // Arayüzde göster
        _nextEvolutionText.text = $"NEXT EVOLVE: {_nextEvolveMultiplier:F2}x";

        // Evolve butonunu kontrol et
        if (_nextEvolveMultiplier >= _evolveMultiplier / 4)
            _nextEvolveButton.SetActive(true);
        else
            _nextEvolveButton.SetActive(false);
    }



    public void NextEvolve()
    {
        
        _fadeoutAnimator.Play("fadeOut");
        _evolveMultiplier += _nextEvolveMultiplier;
        _clickedPower = 1;
        _clickedCount = 0;
        _clickedCountBySec = 0;

        _nextEvolveMultiplier = 1.0;
        UpdateTexts();
            
        PlayerPrefs.SetFloat("EvolveMultiplier", (float)_evolveMultiplier);
        PlayerPrefs.SetString("CatPower", "1");
        PlayerPrefs.SetString("AutoCat", "0");
        PlayerPrefs.SetString("CurrentCats", "0");
        PlayerPrefs.SetString("MaxCats","0");

        Debug.Log($"EVOLVE: Yeni multiplier {_evolveMultiplier:F2}x");

        foreach (Item item in _itemContainer.GetComponentsInChildren<Item>())
        {
            item.ResetItem();
        }
        _nextEvolveButton.SetActive(false);
    }
}
