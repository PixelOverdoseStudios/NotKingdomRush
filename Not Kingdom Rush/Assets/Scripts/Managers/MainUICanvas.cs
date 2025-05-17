using TMPro;
using UnityEngine;

public class MainUICanvas : MonoBehaviour
{
    public static MainUICanvas instance;

    [SerializeField] private TextMeshProUGUI playersGoldText;
    [SerializeField] private TextMeshProUGUI castleHealthText;

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
        UpdateGoldUI();
    }

    public void UpdateGoldUI()
    {
        playersGoldText.text = GameManager.instance.GetGold().ToString();
        castleHealthText.text = GameManager.instance.GetCastleHealth().ToString();
    }
}
