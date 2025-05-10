using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, destination.position) < 0.1f)
        {
            Debug.Log("On top of target");
        }
    }
}
