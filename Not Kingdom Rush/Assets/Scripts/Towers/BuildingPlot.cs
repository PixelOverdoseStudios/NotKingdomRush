using UnityEngine;

public class BuildingPlot : MonoBehaviour, IObjectInteractable
{
    [Header("UI")]
    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject UICanvas;

    [Header("Misc")]
    [SerializeField] private float sizeToAdd = 0.2f;
    [SerializeField] private float growSpeed;
    public bool isBeingHovered;
    private Vector3 growSize;
    private Vector3 spriteStartSize;

    private void Start()
    {
        spriteStartSize = sprite.transform.localScale;
        growSize = spriteStartSize + new Vector3(sizeToAdd, sizeToAdd, 0);
    }

    private void Update()
    {
        if(isBeingHovered)
        {
            sprite.transform.localScale = Vector3.Lerp(sprite.transform.localScale, growSize, growSpeed * Time.deltaTime);
        }
        else if (!isBeingHovered)
        {
            sprite.transform.localScale = Vector3.Lerp(sprite.transform.localScale, spriteStartSize, growSpeed * Time.deltaTime);
        }
    }

    public void MouseClickedTower()
    {
        if(!UICanvas.activeInHierarchy)
        {
            UICanvas.SetActive(true);
        }
        else
        {
            UICanvas.SetActive(false);
        }
    }

    public void MouseEnterHoverTower()
    {
        isBeingHovered = true;
        Debug.Log("Change to outline");
    }

    public void MouseExitHoverTower()
    {
        isBeingHovered = false;
        Debug.Log("Change to default");
    }

    public void ObjectClickedOn()
    {
        UICanvas.SetActive(true);
    }

    public void ObjectClickedOff()
    {
        UICanvas.SetActive(false);
    }

    public void BuildTower(GameObject _towerToBuild)
    {
        Instantiate(_towerToBuild, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
