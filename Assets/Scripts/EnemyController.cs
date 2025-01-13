using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

// Enemy States
public enum EnemyState
{
    Idle,
    Wander,
    Follow,
    Attack,
    Die
};

// Enemy Type Attack
public enum EnemyType
{
    Melee,
    Ranged
};


public class EnemyController : MonoBehaviour
{

    GameObject player;


    // Enemy Stats
    public EnemyType enemyType;
    public EnemyState currState = EnemyState.Idle;
    public float range; // Vision range
    public float baseSpeed; // Base Speed enemy
    public float speed; // Base Speed * Multiplier
    public float baseAttackRange; // Base Attack Range
    public float attackRange; // Attack range * Multiplier
    public float baseHealth; // Base Health enemy
    public float health; // Health * Multiplier
    public int baseDamage; // Base Damage
    public int damage;
    public GameObject projectilePrefab;
    public float baseProjectileSpeed; // Base Projectile Speed
    public float projectileSpeed;// Projectile Speed * Multiplier
    private bool chooseDir = false;
    public bool notInRoom = false;
    private Vector3 randomDir;
    private SpriteRenderer spriteRenderer;

    // Attack Cooldown
    public float coolDown;
    private bool cdAttack = false;

    


    public int Damage { get => damage; set => damage = value; }


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UpdateStats();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer is missing on {gameObject.name}!");
        }
        if (notInRoom)
        {
            currState = EnemyState.Idle; // Configura l'estat inicial com Idle
        }
    }
    void Update()
    {
        if (notInRoom)
        {
            currState = EnemyState.Idle; // Assegura que estiguin inactius
            return; // Surt del Update si està fora de la sala
        }
        switch (currState)
        {

            case(EnemyState.Wander):
                Wander();
                break;

            case(EnemyState.Follow):
                Follow();
                break;

            case (EnemyState.Attack):
                Attack();
                break;

            case(EnemyState.Die):
                break;

        }


        // Canvia l'estat segons la posició del jugador
        if (IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }
        else if (!IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Wander;
        }
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            currState = EnemyState.Attack;
        }

        FlipSprite();
    }
    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }
    private IEnumerator ChooseDirection()
    {
        chooseDir = true;
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        randomDir = new Vector3(0, 0, Random.Range(0, 360));
        Quaternion nextRotation = Quaternion.Euler(randomDir);
        chooseDir = false;
    }
    void Idle()
    {

    }
    void Wander()
    {
        if (!chooseDir)
        {
            StartCoroutine(ChooseDirection());
        }
        transform.position += -transform.right * speed * Time.deltaTime;
        if (IsPlayerInRange(range))
        {
            currState = EnemyState.Follow;
        }
    }
    void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
    public void Attack()
    {
        if (!cdAttack)
        {
            switch (enemyType)
            {
                // Melee
                case (EnemyType.Melee):
                    GameController.DamagePlayer(damage);
                    GameController.scoreMultiplier = Mathf.Max(GameController.scoreMultiplier - 0.1f, 1.0f);
                    Debug.Log($"Score multiplier reduced to: {GameController.scoreMultiplier}");
                    StartCoroutine(CoolDown());
                    break;

                // Ranged
                case (EnemyType.Ranged):
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle)) as GameObject;

                    // Assigna la referència de l'enemic al projectil
                    ProyectilController projController = projectile.GetComponent<ProyectilController>();
                    projController.originatingEnemy = this; // 'this' fa referència a l'enemic actual
                    projController.projectileSpeed = projectileSpeed * GameController.scoreMultiplier;

                    projController.GetPlayer(player.transform);
                    projectile.AddComponent<Rigidbody2D>().gravityScale = 0;
                    projController.isEnemyProjectile = true;

                    StartCoroutine(CoolDown());
                    break;
            }

        }
    }


    private IEnumerator CoolDown()
    {
        cdAttack = true;
        yield return new WaitForSeconds(coolDown);
        cdAttack = false;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }
    public void Death()
    {

            RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
            Destroy(gameObject);
            RoomController.instance.OnEnemyDefeated(this);

        if (gameObject.CompareTag("Boss"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void FlashRed()
    {
        StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        if (spriteRenderer != null)
        {
            // Canvia el color a vermell
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f); // Espera 1 segon
            // Restableix el color original (blanc)
            spriteRenderer.color = Color.white;
        }
    }
    private void FlipSprite()
    {
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (player.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    public void UpdateStats()
    {
        float multiplier = GameController.scoreMultiplier;

        // Ajusta els valors segons el multiplicador
        speed = Mathf.Min(baseSpeed * multiplier, 10f);
        attackRange = baseAttackRange;
        health = baseHealth * multiplier;
        damage = Mathf.RoundToInt(baseDamage * multiplier);
        projectileSpeed = Mathf.Min(baseProjectileSpeed + 0.2f, 5.5f);

        Debug.Log($"Enemy stats updated: Speed={speed}, Health={health}, Damage={damage}, Projectile Speed={projectileSpeed}");
    }
}