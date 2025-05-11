using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int costToClick;
    [SerializeField] private TextMeshProUGUI costText;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (costToClick > GameManager.instance.GetGold())
            button.interactable = false;
        else
            button.interactable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        costText.gameObject.SetActive(true);
        costText.text = "$" + costToClick.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        costText.gameObject.SetActive(false);
    }
}
