using UnityEngine;
using System.Collections;

public class FireballRain : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public int fireballCount = 4;
    public int damage = 30;
    public float splashRadius = 2f;
    public float delayBetweenFireballs = 0.2f;
    public float fallHeight = 10f;

    [Header("UI")]
    public GameObject targetCursorPrefab;

    [Header("Targeting")]
    public LayerMask enemyLayer;
    public float castRadius = 5f;
    public float castWidthMultiplyer = 1.5f;

    private GameObject currentCursor;
    private bool isTargeting = false;
    private Camera mainCamera;

    //Gizmo purpose
    private Vector2 lastTargetPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {   
        if (!isTargeting) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        lastTargetPosition = mouseWorldPos;
        currentCursor.transform.position = mouseWorldPos;

        if (Input.GetMouseButtonDown(0  )) // Right mouse click
        {
            CastFireballRain(mouseWorldPos);
            EndTargeting();
        }

        if (Input.GetMouseButtonDown(1))
        {
            EndTargeting();
        }
    }

    // Call this from your UI button
    public void StartTargeting()
    {
        if (isTargeting) return;
        
        isTargeting = true;
        currentCursor = Instantiate(targetCursorPrefab);
    }

    private void EndTargeting()
    {
        if (!isTargeting) return;
        
        isTargeting = false;
        if (currentCursor != null)
        {
            Destroy(currentCursor);
        }
    }


    public void CastFireballRain(Vector2 centerPosition)
    {
        StartCoroutine(FireballRainCoroutine(centerPosition));
    }

    private IEnumerator FireballRainCoroutine(Vector2 center)
    {
        for (int i = 0; i < fireballCount; i++)
        {
            // Random position within radius
            Vector2 randomOffset = Random.insideUnitCircle * castRadius;
            randomOffset.x *= castWidthMultiplyer;
            Vector2 spawnPosition = center + randomOffset + Vector2.up * fallHeight;
            Vector2 targetPosition = center + randomOffset;

            // Create fireball
            GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);
            Fireball fb = fireball.GetComponent<Fireball>();
            fb.Initialize(targetPosition, damage, splashRadius, enemyLayer);

            yield return new WaitForSeconds(delayBetweenFireballs);
        }
    }

    [Header("Gizmo Settings")]
    public bool alwaysShowGizmos = false;
    public Color castRadiusColor = new Color(1, 0.5f, 0, 0.3f);
    public Color splashRadiusColor = new Color(1, 0, 0, 0.2f);
    public Color fallHeightColor = new Color(0, 1, 1, 0.5f);

    private void OnDrawGizmos()
    {   
        if (!alwaysShowGizmos) return;
        if (isTargeting)
        {   
            // Draw the main cast radius
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f); // Orange with transparency
            Gizmos.DrawSphere(lastTargetPosition, castRadius);
        }
        
        // Always show the fall height in Scene view
        if (Application.isPlaying && isTargeting)
        {
            Gizmos.color = new Color(0, 1, 1, 0.5f); // Cyan with transparency
            Gizmos.DrawLine(lastTargetPosition, lastTargetPosition + Vector2.up * fallHeight);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (isTargeting)
        {   
            // Draw the main cast radius
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f); // Orange with transparency
            Gizmos.DrawSphere(lastTargetPosition, castRadius);
        }

        // Only show fall height while playing and targeting
        if (Application.isPlaying && isTargeting)
        {
            Gizmos.color = new Color(0, 1, 1, 0.5f); // Cyan with transparency
            Gizmos.DrawLine(lastTargetPosition, lastTargetPosition + Vector2.up * fallHeight);
        }
    }

}