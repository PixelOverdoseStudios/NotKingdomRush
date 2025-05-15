using UnityEngine;

public class ButtonCollection : MonoBehaviour
{
    [SerializeField] private GameObject objectToTurnOff;
    public bool IsFullGrown { get; private set; }

    public void TurnOffObject()
    {
        if(objectToTurnOff.activeInHierarchy) objectToTurnOff.SetActive(false);
    }

    public void ButtonsFullGrown() => IsFullGrown = true;
    public void ButtonsNotGrown() => IsFullGrown = false;
}
