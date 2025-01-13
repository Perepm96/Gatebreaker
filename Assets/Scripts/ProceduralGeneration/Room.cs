using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public int Width;
    public int Height;
    public int X;
    public int Y;
    public string baseName;
    public bool isCleared = false;


    private bool updatedDoors = false;

    public Room(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Door leftDoor;
    public Door rightDoor;
    public Door topDoor;
    public Door bottomDoor;

    public List<Door> doors = new List<Door>();

    // Start is called before the first frame update
    void Start()
    {
        if (RoomController.instance == null)
        {
            Debug.Log("You pressed play in the wrong scene!");
            return;
        }

        Door[] ds = GetComponentsInChildren<Door>();
        foreach (Door d in ds)
        {
            doors.Add(d);
            switch (d.doorType)
            {
                case Door.DoorType.rigth:
                    rightDoor = d;
                    break;
                case Door.DoorType.left:
                    leftDoor = d;
                    break;
                case Door.DoorType.up:
                    topDoor = d;
                    break;
                case Door.DoorType.down:
                    bottomDoor = d;
                    break;
            }
        }

        RoomController.instance.RegisterRoom(this);
    }

    void Update()
    {
        if (name.Contains("End") && !updatedDoors)
        {
            RemoveUnconnectedDoors();
            updatedDoors = true;
        }
    }

    public void RemoveUnconnectedDoors()
    {
        Debug.Log("Removing unconnected doors...");
        foreach (Door door in doors)
        {
            switch (door.doorType)
            {
                case Door.DoorType.rigth:
                    if (GetRight() == null)
                    {
                        Debug.Log($"Desactivant porta dreta {door.name}");
                        door.isConnected = false;
                        door.gameObject.SetActive(false);
                    }
                    else
                    {
                        door.isConnected = true;
                    }
                    break;
                case Door.DoorType.left:
                    if (GetLeft() == null)
                    {
                        Debug.Log($"Desactivant porta esquerra {door.name}");
                        door.isConnected = false;
                        door.gameObject.SetActive(false);
                    }
                    else
                    {
                        door.isConnected = true;
                    }
                    break;
                case Door.DoorType.up:
                    if (GetTop() == null)
                    {
                        Debug.Log($"Desactivant porta adalt {door.name}");
                        door.isConnected = false;
                        door.gameObject.SetActive(false);
                    }
                    else
                    {
                        door.isConnected = true;
                    }
                    break;
                case Door.DoorType.down:
                    if (GetBottom() == null)
                    {
                        Debug.Log($"Desactivant porta abaix {door.name}");
                        door.isConnected = false;
                        door.gameObject.SetActive(false);
                    }
                    else
                    {
                        door.isConnected = true;
                    }
                    break;
            }
        }
    }

    public Room GetRight()
    {
        if (RoomController.instance.DoesRoomExist(X + 1, Y))
        {
            return RoomController.instance.FindRoom(X + 1, Y);
        }
        return null;
    }
    public Room GetLeft()
    {
        if (RoomController.instance.DoesRoomExist(X - 1, Y))
        {
            return RoomController.instance.FindRoom(X - 1, Y);
        }
        return null;
    }
    public Room GetTop()
    {
        if (RoomController.instance.DoesRoomExist(X, Y + 1))
        {
            return RoomController.instance.FindRoom(X, Y + 1);
        }
        return null;
    }
    public Room GetBottom()
    {
        if (RoomController.instance.DoesRoomExist(X, Y - 1))
        {
            return RoomController.instance.FindRoom(X, Y - 1);
        }
        return null;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0));
    }

    public Vector3 GetRoomCentre()
    {
        return new Vector3(X * Width, Y * Height);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            RoomController.instance.OnPlayerEnterRoom(this);
        }
    }
}
