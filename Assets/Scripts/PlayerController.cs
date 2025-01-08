using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigibody;

    public Text collectedText;
    public static int collectedAmount = 0;
    //Player
    public float speed;

    // Slash stats
    public GameObject slashPrefab;
    public float slashRate;
    public float size;
    public float slashSpeed;
    private float lastSlash;


    // Energy stats
    public int slashEnergy = 10; // Slash energy consum
    public float energyRegenRate;
    private float lastEnergyRegenTime;

    


    void Start()
    {
        rigibody = GetComponent<Rigidbody2D>();
        lastEnergyRegenTime = Time.time;
    }

    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        speed = GameController.MoveSpeed;
        energyRegenRate = GameController.EnergyRegenRate;

        slashRate = GameController.AttackRate;
        size = GameController.SlashSize;
        slashSpeed = GameController.SlashSpeed;

        float slashHor = Input.GetAxisRaw("SlashHorizontal");
        float slashVer = Input.GetAxisRaw("SlashVertical");


        if ((slashHor != 0 || slashVer != 0) && Time.time > lastSlash + slashRate && GameController.CurrentEnergy > slashEnergy )
        {
            Slash(slashHor, slashVer);
            GameController.EnergyAttack(slashEnergy);
            lastSlash = Time.time;
        }


        if (!Mathf.Approximately(horizontal, 0) || !Mathf.Approximately(vertical, 0))
        {
            rigibody.linearVelocity = new Vector2(horizontal * speed, vertical * speed);
        }
        else
        {
            rigibody.linearVelocity = Vector2.zero;
        }

        FlipSprite(horizontal);


        collectedText.text = "Coins: " + collectedAmount;
        if (Time.time > lastSlash + slashRate) 
        {
            RegenerateEnergy();
        }
    }

    void Slash(float x, float y)
    {
        GameObject slash = Instantiate(slashPrefab, transform.position, transform.rotation);
        Rigidbody2D slashRigidbody = slash.AddComponent<Rigidbody2D>();
        slashRigidbody.gravityScale = 0;


        slash.transform.localScale *= size;

        Vector2 slashVelocity = new Vector2(
            (x < 0) ? Mathf.Floor(x) * slashSpeed : Mathf.Ceil(x) * slashSpeed,
            (y < 0) ? Mathf.Floor(y) * slashSpeed : Mathf.Ceil(y) * slashSpeed
        );
        slashRigidbody.linearVelocity = slashVelocity;

        if (slashVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(slashVelocity.y, slashVelocity.x) * Mathf.Rad2Deg;
            slash.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void IncreaseSlashSize(float amount)
    {
        size += amount; 
    }
    void RegenerateEnergy()
    {
        if (Time.time >= lastEnergyRegenTime + 1.0f / energyRegenRate) 
        {
            GameController.CurrentEnergy = Mathf.Min(GameController.CurrentEnergy + 1, GameController.MaxEnergy); 
            lastEnergyRegenTime = Time.time; 
        }
    }
    void FlipSprite(float horizontal)
    {
        // Solo voltear si el jugador se mueve horizontalmente
        if (horizontal > 0) // Movimiento hacia la derecha
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontal < 0) // Movimiento hacia la izquierda
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}