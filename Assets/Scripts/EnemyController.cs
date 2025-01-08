using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

// Enemy States
public enum EnemyState
{
    Inactive, // IDLE TO DO
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
    public EnemyState currState = EnemyState.Wander;
    public float range; // Vision range
    public float speed;
    public float attackRange; // Attack range
    public float health;
    public int damage;
    private bool chooseDir = false;
    private Vector3 randomDir;

    // Attack Cooldown
    public float coolDown;
    private bool cdAttack = false;

    //Enemy Stats Ranged Prjectile
    public GameObject projectilePrefab;
    public float projectileSpeed;
    
    
    


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        switch(currState)
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
                // TO DO
                break;

        }

        //State changer

        if(IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }
        else if(!IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Wander;
        }
        if(Vector3.Distance(transform.position, player.transform.position) <= attackRange)
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
                    GameController.DamagePlayer(1);
                    StartCoroutine(CoolDown());
                    break;

                // Ranged
                case (EnemyType.Ranged):
                    
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle)) as GameObject;
                    projectile.GetComponent<ProyectilController>().GetPlayer(player.transform);
                    projectile.AddComponent<Rigidbody2D>().gravityScale = 0;
                    projectile.GetComponent<ProyectilController>().isEnemyProjectile = true;
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
    public void Death()
    {
        Destroy(gameObject);
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
}