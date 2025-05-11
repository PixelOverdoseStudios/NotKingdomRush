using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseTracker : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mousePosition;
    public GameObject towerHovered;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if(HoverOverTower(hit))
            {
                if (towerHovered == null)
                {
                    towerHovered = hit.collider.gameObject;
                    towerHovered.GetComponent<ITowerInteractable>().TowerClickedOn();
                }
                else if (towerHovered != null)
                {
                    towerHovered.GetComponent<ITowerInteractable>().TowerClickedOff();
                    towerHovered = hit.collider.gameObject;
                    towerHovered.GetComponent<ITowerInteractable>().TowerClickedOn();
                }
            }
            else if(!HoverOverTower(hit))
            {
                if(towerHovered != null)
                {
                    towerHovered.GetComponent<ITowerInteractable>().TowerClickedOff();
                    towerHovered = null;
                }
            }
        }
    }

    private bool HoverOverTower(RaycastHit2D _hit)
    {
        if(_hit.collider != null)
        {
            if (_hit.collider.gameObject.CompareTag("Tower")) return true;
            else return false;
        }
        else return false;
    }
}
