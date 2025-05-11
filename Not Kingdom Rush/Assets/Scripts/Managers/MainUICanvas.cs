using TMPro;
using UnityEngine;

public class MainUICanvas : MonoBehaviour
{
    public static MainUICanvas instance;

    [SerializeField] private TextMeshProUGUI playersGoldAmount;

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
        playersGoldAmount.text = GameManager.instance.GetGold().ToString();
    }
}
