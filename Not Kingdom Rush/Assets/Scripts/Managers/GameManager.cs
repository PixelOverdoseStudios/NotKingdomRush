using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int gold;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
        }
    }

    public int GetGold() => gold;

    public void AddGold(int goldToAdd)
    {
        gold += goldToAdd;
        MainUICanvas.instance.UpdateGoldUI();
    }

    public void SubtractGold(int goldToTake)
    {
        gold -= goldToTake;
        MainUICanvas.instance.UpdateGoldUI();
    }
}
