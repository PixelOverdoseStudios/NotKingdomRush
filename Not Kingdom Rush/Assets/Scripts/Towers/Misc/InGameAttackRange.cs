using UnityEngine;

public class InGameAttackRange : MonoBehaviour
{
    private Tower tower;
    private Vector3 startingSize;
    private float growthTimer;

    private void Awake()
    {
        tower = GetComponentInParent<Tower>();
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(0, 0, 1);
        growthTimer = 0;
    }

    private void Start()
    {
        startingSize = transform.localScale;
        transform.position += tower.GetRangeOffset();
    }

    private void Update()
    {
        UpdateScale();
    }

    public void UpdateScale()
    {
        growthTimer += Time.deltaTime;
        float percentageComplete = growthTimer / .25f;

        transform.localScale = Vector3.Lerp(startingSize, new Vector3(tower.GetAttackRange() * 2, tower.GetAttackRange() * 2, 1f), percentageComplete);
    }
}
