using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public float sizeIncrease = 1.5f; 

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.IncreaseSlashSize(sizeIncrease);
            }
            Destroy(gameObject);
        }
    }
}