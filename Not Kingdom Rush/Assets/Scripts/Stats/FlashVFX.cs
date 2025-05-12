using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class SmoothFlashEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.3f; // Total time for flash (in/out)
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private AnimationCurve flashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Smooth fade in/out
    public SpriteRenderer sprite;
    private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");

    Health health;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.OnTakeDamage += Flash;
    }

    void OnDisable()
    {
        health.OnTakeDamage -= Flash;
    }

    // Call this to trigger the flash
    public void Flash()
    {
        StopAllCoroutines(); // Stop any existing flash
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float elapsedTime = 0f;

        // Initialize material properties

        sprite.material.SetColor(FlashColorID, flashColor);

        // Smooth flash in/out using AnimationCurve
        while (elapsedTime < flashDuration)
        {
            float flashProgress = elapsedTime / flashDuration;
            float flashAmount = flashCurve.Evaluate(flashProgress);

            sprite.material.SetFloat(FlashAmountID, flashAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset flash amount to 0 when done
        sprite.material.SetFloat(FlashAmountID, 0f);
    }
}