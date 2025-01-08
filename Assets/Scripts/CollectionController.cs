using UnityEngine;

public class CollectionController : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PlayerController.collectedAmount++;
            Destroy(gameObject);
            
        }
        
    }
}
