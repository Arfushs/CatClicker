using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _resetHeightValue = 128f;
    [SerializeField] private float _startHeight = 64f; 
    private SpriteRenderer _spriteRenderer;
    private float _currentHeight;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentHeight = _startHeight;
    }

    void Update()
    {
        _currentHeight += Time.deltaTime * _speed;
        if(_currentHeight > _resetHeightValue) _currentHeight = _startHeight;
        
        _spriteRenderer.size = new Vector2(_spriteRenderer.size.x, _currentHeight);
    }
}
