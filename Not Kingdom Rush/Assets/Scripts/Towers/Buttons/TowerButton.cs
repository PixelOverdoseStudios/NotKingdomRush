using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TowerButtonType buttonType;
    [SerializeField] private int goldValue;
    [SerializeField] private TextMeshProUGUI costText;
    private ButtonCollection buttonCollection;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonCollection = GetComponentInParent<ButtonCollection>();
    }

    private void OnEnable()
    {
        CheckButtonStatus();
    }

    public void CostOfBuilding()
    {
        GameManager.instance.SubtractGold(goldValue);
        CheckButtonStatus();
    }

    public void SellBuilding(GameObject _buildingPlot)
    {
        GameManager.instance.AddGold(goldValue);
        Instantiate(_buildingPlot, GetComponentInParent<Tower>().gameObject.transform.position, Quaternion.identity);
        Destroy(GetComponentInParent<Tower>().gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonCollection.IsFullGrown)
        {
            costText.gameObject.SetActive(true);
            costText.text = "$" + goldValue.ToString();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        costText.gameObject.SetActive(false);
    }

    public void CheckButtonStatus()
    {
        if (buttonType == TowerButtonType.Buy)
        {
            if (goldValue > GameManager.instance.GetGold())
                button.interactable = false;
            else
                button.interactable = true;
        }
        else if (buttonType == TowerButtonType.Sell)
            button.interactable = true;
    }
}

public enum TowerButtonType
{
    Buy,
    Sell
}
