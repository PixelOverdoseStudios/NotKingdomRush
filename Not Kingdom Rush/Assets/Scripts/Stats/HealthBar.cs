using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health; // Reference to the Health script
    [SerializeField] private GameObject healthGrouping; //parent used to dissable health bar
    [SerializeField] private Transform fillTransform; // The "fill" part of the bar (child object)
    
    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = fillTransform.localScale;
        
    }

    void Start()
    {
        HideHealthBar();   
    }

    void OnEnable()
    {   
        ResetHealthBar();

        health.OnHealthReset += ResetHealthBar;
        health.OnFullHPRegen += HideHealthBar;
        health.OnHealthChanged += UpdateHealthBar;
        health.OnDeath += HideHealthBar;
    }

    void OnDisable()
    {   
        health.OnHealthReset -= ResetHealthBar;
        health.OnFullHPRegen -= HideHealthBar;
        health.OnHealthChanged -= UpdateHealthBar;
        health.OnDeath -= HideHealthBar;
    }

    private void UpdateHealthBar(float healthPercent)
    {   
        healthGrouping.SetActive(true);
        // Scale the fill bar horizontally (X-axis)
        fillTransform.localScale = new Vector3(initialScale.x * healthPercent, initialScale.y, initialScale.z);
    }

    //For enemy when they died
    public void ResetHealthBar()
    {
        healthGrouping.SetActive(false); // Disable on death

        //Reset health bar as well
        fillTransform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
    }

    //For troops when regen to full health
    private void HideHealthBar()
    {
        healthGrouping.SetActive(false);
    }
}