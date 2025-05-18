using TMPro;
using UnityEngine;

public class MainUICanvas : MonoBehaviour
{
    public static MainUICanvas instance;

    [SerializeField] private TextMeshProUGUI playersGoldText;
    [SerializeField] private TextMeshProUGUI castleHealthText;

    [Header("Menu References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMainPanel;
    [SerializeField] private GameObject pauseSettingsPanel;

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
        UpdatePlayerHUD();
    }

    public void UpdatePlayerHUD()
    {
        playersGoldText.text = GameManager.instance.GetGold().ToString();
        castleHealthText.text = GameManager.instance.GetCastleHealth().ToString();
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            GameManager.instance.UnpauseGame();
        }
        else
        {
            pauseMenu.SetActive(true);
            pauseMainPanel.SetActive(true);
            pauseSettingsPanel.SetActive(false);
            GameManager.instance.PauseGame();
        }
    }

    public void ContinueButtonPressed()
    {
        TogglePauseMenu();
    }

    public void SettingsButtonPressed()
    {
        pauseMainPanel.SetActive(false);
        pauseSettingsPanel.SetActive(true);
    }

    public void BackButtonPressed()
    {
        pauseMainPanel.SetActive(true);
        pauseSettingsPanel.SetActive(false);
    }
}
