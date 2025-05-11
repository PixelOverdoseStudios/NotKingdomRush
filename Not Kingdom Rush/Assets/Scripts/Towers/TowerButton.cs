using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private int costToClick;

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
}
