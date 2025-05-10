using UnityEngine;
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

        //Determines if you are hovering over a tower (use for highlighting)
        if (HoverOverTower(hit))
            Debug.Log(hit.collider.gameObject.name + " is being hovered over");

        //Determines if you are clicking on a tower that is being hovered
        if (Input.GetMouseButtonDown(0) && HoverOverTower(hit))
        {
            Debug.Log(hit.collider.gameObject.name + " has been clicked on");
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
