using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Tilemaps;

public class RoomInfo
{
    public string name;
    public int X;
    public int Y;
}


public class RoomController : MonoBehaviour
{
    public static RoomController instance;

    string currentWorldName = "Underground";

    RoomInfo currentLoadRoomData;
    public bool roomComplete = false;


    Room currRoom;

    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();

    public List<Room> loadedRooms = new List<Room>();

    bool isLoadingRoom = false;
    bool spawnedBossRoom = false;
    bool updatedRooms = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {


    }

    void Update()
    {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        if (isLoadingRoom)
        {
            return;
        }

        if (loadRoomQueue.Count == 0)
        {
            if (!spawnedBossRoom)
            {
                StartCoroutine(SpawnBossRoom());
            }
            else if (spawnedBossRoom && !updatedRooms)
            {
                foreach (Room room in loadedRooms)
                {
                    room.RemoveUnconnectedDoors();
                }
                UpdateRooms();
                updatedRooms = true;
            }
            return;
        }

        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;

        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));
    }

    IEnumerator SpawnBossRoom()
    {
        spawnedBossRoom = true;
        yield return new WaitForSeconds(0.5f);
        if (loadRoomQueue.Count == 0)
        {
            Room bossRoom = loadedRooms[loadedRooms.Count - 1];
            Room tempRoom = new Room(bossRoom.X, bossRoom.Y);
            Destroy(bossRoom.gameObject);
            var roomToRemove = loadedRooms.Single(r => r.X == tempRoom.X && r.Y == tempRoom.Y);
            loadedRooms.Remove(roomToRemove);
            LoadRoom("End", tempRoom.X, tempRoom.Y);
        }
    }

    public void LoadRoom(string name, int x, int y)
    {
        if (DoesRoomExist(x, y) == true)
        {
            return;
        }

        RoomInfo newRoomData = new RoomInfo();
        newRoomData.name = name;
        newRoomData.X = x;
        newRoomData.Y = y;

        loadRoomQueue.Enqueue(newRoomData);
    }

    IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        string roomName = currentWorldName + info.name;

        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);

        while (loadRoom.isDone == false)
        {
            yield return null;
        }
    }
    public void OnPlayerEnterRoom(Room room)
    {
        CameraController.instance.currRoom = room;
        currRoom = room;

        StartCoroutine(RoomCoroutine());
    }

    public IEnumerator RoomCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        UpdateRooms();
    }


    public void UpdateRooms()
    {
        foreach (Room room in loadedRooms)
        {
            if (room == currRoom) // Només tracta la sala actual
            {
                EnemyController[] enemies = room.GetComponentsInChildren<EnemyController>();

                if (enemies.Length > 0) // Hi ha enemics presents
                {
                    foreach (EnemyController enemy in enemies)
                    {
                        enemy.notInRoom = false; // Els enemics estan actius
                    }

                    foreach (Door door in room.GetComponentsInChildren<Door>())
                    {
                        door.gameObject.SetActive(false); // Desactiva les portes
                    }

                    Debug.Log($"Sala {room.name} té enemics. Les portes estan tancades.");
                }
                else // Tots els enemics han estat derrotats
                {
                    if (!room.isCleared && (room.baseName == "Basic1" || room.baseName == "Basic2")) // Només sales Basic1 i Basic2
                    {
                        GameController.scoreMultiplier += 0.2f; // Augmenta el multiplicador
                        room.isCleared = true; // Marca la sala com a netejada
                        Debug.Log($"Sala {room.name} netejada per primera vegada. Multiplicador actualitzat a: {GameController.scoreMultiplier}");
                        roomComplete = true;

                    }

                    foreach (Door door in room.GetComponentsInChildren<Door>(true))
                    {
                        if (door.isConnected) // Només actua sobre les portes connectades
                        {
                            door.gameObject.SetActive(true); // Activa la porta
                            Debug.Log($"Porta activada: {door.name} (Connectada: {door.isConnected})");
                        }
                        else
                        {
                            Debug.Log($"Porta no activada: {door.name} (No connectada)");
                        }
                    }

                    Debug.Log($"Sala {room.name} netejada. Les portes estan obertes.");
                }
            }
            else
            {
                // Configura els enemics de les altres sales com "fora de sala"
                EnemyController[] enemies = room.GetComponentsInChildren<EnemyController>();
                foreach (EnemyController enemy in enemies)
                {
                    enemy.notInRoom = true;
                }
            }
        }
    }
    public void OnEnemyDefeated(EnemyController enemy)
    {
        Debug.Log($"Enemic derrotat: {enemy.name}");
        UpdateRooms(); // Actualitza les sales després de derrotar un enemic
    }

    public void RegisterRoom(Room room)
    {
        if (!DoesRoomExist(currentLoadRoomData.X, currentLoadRoomData.Y))
        {
            room.transform.position = new Vector3(
                currentLoadRoomData.X * room.Width,
                currentLoadRoomData.Y * room.Height,
                0
            );

            room.X = currentLoadRoomData.X;
            room.Y = currentLoadRoomData.Y;
            room.baseName = currentLoadRoomData.name;
            room.name = currentWorldName + "-" + currentLoadRoomData.name + " " + room.X + ", " + room.Y;
            room.transform.parent = transform;

            isLoadingRoom = false;

            if (loadedRooms.Count == 0)
            {
                CameraController.instance.currRoom = room;
            }

            loadedRooms.Add(room);
        }
        else
        {
            Destroy(room.gameObject);
            isLoadingRoom = false;
        }
    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(item => item.X == x && item.Y == y) != null;
    }

    public Room FindRoom(int x, int y)
    {
        return loadedRooms.Find(item => item.X == x && item.Y == y);
    }

    public string GetRandomRoomName()
    {
        string[] possibleRooms = new string[] {
            "Basic1",
            "Basic2"
        };

        return possibleRooms[Random.Range(0, possibleRooms.Length)];
    }


    public Room FindRoomByName(string roomName)
    {
        foreach (Room room in loadedRooms)
        {
            if (room.baseName == roomName) // Busca pel nom base
            {
                return room;
            }
        }
        Debug.LogError($"Room with name {roomName} not found!");
        return null; // Retorna null si no es troba
    }

    IEnumerator GenerateRooms()
    {
        // Aquí es generen les sales
        yield return new WaitForSeconds(2.0f); // Simula la generació

        Debug.Log("Rooms have been generated!");
    }
    public void UpdateAllEnemiesStats()
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemies)
        {
            enemy.UpdateStats();
        }
    }
    public void OnPlayerEnterDoor(Door door, GameObject player)
    {
        Vector3 offset = Vector3.zero;

        // Determina el desplaçament segons el tipus de porta
        switch (door.doorType)
        {
            case Door.DoorType.left:
                offset = new Vector3(-5, 0, 0);
                break;
            case Door.DoorType.rigth:
                offset = new Vector3(5, 0, 0);
                break;
            case Door.DoorType.up:
                offset = new Vector3(0, 5, 0);
                break;
            case Door.DoorType.down:
                offset = new Vector3(0, -5, 0);
                break;
        }

        // Desplaça el jugador
        player.transform.position += offset;
    }
    public Room FindRoomByPlayerPosition(Vector3 position)
    {
        foreach (Room room in loadedRooms)
        {
            Bounds roomBounds = new Bounds(room.transform.position, new Vector3(room.Width, room.Height, 0));
            if (roomBounds.Contains(position))
            {
                return room;
            }
        }
        return null;
    }
}