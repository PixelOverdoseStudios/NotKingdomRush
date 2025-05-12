using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LifeManager : MonoBehaviour
{   
    public static LifeManager Instance;


    [Header("Settings")]
    [SerializeField] private int maxLives = 20;
    [SerializeField] private bool canGameOver = true;

    [Header("Events")]
    public Action onLifeLost;
    public Action onGameOver;

    [Header("UI Settings")]
    public TextMeshProUGUI numberOfLifeText;

    private int currentLives;

    private void Awake()
    {   
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }

        InitializeLives();
    }

    private void InitializeLives()
    {
        currentLives = maxLives;

        numberOfLifeText.text = currentLives.ToString();
    }

    public void LoseLife(int amount = 1)
    {   
        
        currentLives -= amount;
        currentLives = Mathf.Max(0, currentLives);

        // Update visuals
        numberOfLifeText.text = currentLives.ToString();

        onLifeLost?.Invoke();

        if (currentLives <= 0 && canGameOver)
        {
            onGameOver.Invoke();
        }
    }

    public void GainLife(int amount = 1)
    {
        currentLives += amount;
        currentLives = Mathf.Min(currentLives, maxLives);

        // Update visuals
        numberOfLifeText.text = currentLives.ToString();
    }

    // For testing - call these from Unity Events or other scripts
    [ContextMenu("Test Lose Life")]
    private void TestLoseLife() => LoseLife();

    [ContextMenu("Test Gain Life")]
    private void TestGainLife() => GainLife();
}