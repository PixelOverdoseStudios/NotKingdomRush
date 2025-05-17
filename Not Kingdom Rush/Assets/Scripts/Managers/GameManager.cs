using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int gold;
    [SerializeField] private int startingCastleHealth;
    private int castleHealth;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        castleHealth = startingCastleHealth;
    }

    #region Gold Methods
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
    #endregion

    public int GetCastleHealth() => castleHealth;

    public void CastleTakeDamage(int amount)
    {
        if(castleHealth > 0)
        {
            castleHealth -= amount;
            if(castleHealth < 0)
            {
                castleHealth = 0;
            }
        }
    }
}
