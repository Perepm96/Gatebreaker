using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        left, rigth, up, down
    }

    public DoorType doorType;
    public bool isConnected = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player")) // Comprova si el jugador col·lisiona amb la porta
        {
            RoomController.instance.OnPlayerEnterDoor(this, col.gameObject); // Gestiona el moviment
            Debug.Log($"Player on door");
        }
    }
}