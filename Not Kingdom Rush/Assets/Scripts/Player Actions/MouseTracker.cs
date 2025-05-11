using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseTracker : MonoBehaviour
{
    static MouseTracker instance;

    private Camera mainCamera;
    private Vector3 mousePosition;
    public GameObject towerHovered;

    //Hero logic
    public Hero hero;
    bool selectedHero = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        mainCamera = Camera.main;
    }

    private void Update()
    {
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        //Left click
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if(HoverOverTower(hit))
            {
                if (towerHovered == null)
                {
                    towerHovered = hit.collider.gameObject;
                    towerHovered.GetComponent<IObjectInteractable>().ObjectClickedOn();
                }
                else if (towerHovered != null)
                {
                    towerHovered.GetComponent<IObjectInteractable>().ObjectClickedOff();
                    towerHovered = hit.collider.gameObject;
                    towerHovered.GetComponent<IObjectInteractable>().ObjectClickedOn();
                }
            }
            else if(!HoverOverTower(hit))
            {
                if(towerHovered != null)
                {
                    towerHovered.GetComponent<IObjectInteractable>().ObjectClickedOff();
                    towerHovered = null;
                }
            }
            
            //Check if hovered on Hero
            if (HoverOverHero(hit))
            {
                if (hero == null)
                {
                    hero = hit.collider.gameObject.GetComponent<Hero>();
                }
                selectedHero = true;
            }
            //If not hovered and selected move hero
            else if(!HoverOverHero(hit) && selectedHero)
            {
                if(hero != null)
                {
                    hero.MoveToPosition(mousePosition);
                    hero.ResetEnemyState();
                    
                    hero = null;
                    selectedHero = false;
                }
            }
        }

        //Right click (For cancelling Hero Actions)
        if(Input.GetMouseButtonDown(1))
        {
            if (selectedHero)
            {
                //Cancel selection

                selectedHero = false;
            }
        }
    }


    private bool HoverOverTower(RaycastHit2D _hit)
    {
        if(_hit.collider != null)
        {
            if (_hit.collider.gameObject.CompareTag("Interactable")) return true;
            else return false;
        }
        else return false;
    }


    //Hover over hero using "Hero" tag
    private bool HoverOverHero(RaycastHit2D _hit)
    {
        if(_hit.collider != null)
        {
            if (_hit.collider.gameObject.CompareTag("Hero")) return true;
            else return false;
        }
        else return false;
    }
}
