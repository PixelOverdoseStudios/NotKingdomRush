using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int gold;
    [SerializeField] private int startingCastleHealth;
    private int castleHealth;
    private bool isPaused;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
        }

        castleHealth = startingCastleHealth;
    }

    private void Update()
    {
        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainUICanvas.instance.TogglePauseMenu();
        }
    }

    #region Gold Methods
    public int GetGold() => gold;

    public void AddGold(int goldToAdd)
    {
        gold += goldToAdd;
        MainUICanvas.instance.UpdatePlayerHUD();
    }

    public void SubtractGold(int goldToTake)
    {
        gold -= goldToTake;
        MainUICanvas.instance.UpdatePlayerHUD();
    }
    #endregion

    #region Castle Health Methods
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
    #endregion

    public void PauseGame() => isPaused = true;
    public void UnpauseGame() => isPaused = false;
}
