using UnityEngine;

public class ArcherShootArrow : MonoBehaviour
{
    private TowerArcher towerArcher;

    private void Awake()
    {
        towerArcher = GetComponentInParent<TowerArcher>();
    }

    public void CallFireArrow()
    {
        towerArcher.FireArrow();
    }
}
