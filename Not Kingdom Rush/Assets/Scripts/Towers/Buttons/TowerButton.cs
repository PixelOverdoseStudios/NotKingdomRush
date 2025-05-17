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
    private Tower tower;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonCollection = GetComponentInParent<ButtonCollection>();
        if (GetComponentInParent<Tower>() != null)
            tower = GetComponentInParent<Tower>();
    }

    private void OnEnable()
    {
        RefreshButtonInfo();
    }

    public void CostOfBuilding()
    {
        GameManager.instance.SubtractGold(goldValue);
        RefreshButtonInfo();
    }

    public void UpgradeTower()
    {
        tower.UpgradeTower();
        RefreshButtonInfo();
    }

    public void SellBuilding(GameObject _buildingPlot)
    {
        GameManager.instance.AddGold(goldValue);
        Instantiate(_buildingPlot, GetComponentInParent<Tower>().gameObject.transform.position, Quaternion.identity);
        Destroy(GetComponentInParent<Tower>().gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RefreshButtonInfo();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        costText.gameObject.SetActive(false);
    }

    //== INTERACTABLE and TEXT ==//
    // Checks if the button should be interactable based on the button type 
    // Also updates the text on the button

    public void RefreshButtonInfo()
    {
        switch(buttonType)
        {
            case TowerButtonType.Buy:
                if (goldValue > GameManager.instance.GetGold())
                    button.interactable = false;
                else
                    button.interactable = true;
                break;

            case TowerButtonType.Upgrade:
                if(tower.GetTowerLevel() < 2)
                {
                    if (goldValue > GameManager.instance.GetGold())
                        button.interactable = false;
                    else
                        button.interactable = true;
                }
                else
                    button.interactable = false;
                break;

            case TowerButtonType.Sell:
                button.interactable = true;
                break;
        }

        if (buttonCollection.IsFullGrown)
        {
            costText.gameObject.SetActive(true);

            switch (buttonType)
            {
                case TowerButtonType.Buy:
                    costText.text = "-" + goldValue.ToString();
                    break;
                case TowerButtonType.Upgrade:
                    if (tower.GetTowerLevel() < 2)
                    {
                        costText.text = "-" + tower.GetUpgradeCost().ToString();
                    }
                    else
                    {
                        costText.text = "MAX";
                    }
                    break;
                case TowerButtonType.Sell:
                    costText.text = "+" + goldValue.ToString();
                    break;
            }
        }
    }
}

public enum TowerButtonType
{
    Buy,
    Upgrade,
    Sell
}
