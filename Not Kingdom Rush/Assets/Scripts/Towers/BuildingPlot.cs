using UnityEngine;
using UnityEngine.UI;

public class BuildingPlot : MonoBehaviour, IObjectInteractable
{
    [Header("UI")]
    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject UICanvas;

    [Header("Misc")]
    [SerializeField] private float sizeToAdd = 0.2f;
    [SerializeField] private float growSpeed;
    [SerializeField] private Animator buttonCollectionAnimator;
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

    public void ObjectClickedOn()
    {
        UICanvas.SetActive(true);
    }

    public void ObjectClickedOff()
    {
        buttonCollectionAnimator.SetTrigger("despawnButtons");
    }

    public void BuildTower(GameObject _towerToBuild)
    {
        Instantiate(_towerToBuild, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    public void ObjectIsBeingHovered() { }
}
