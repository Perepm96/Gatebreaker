using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilController : MonoBehaviour
{
    public float lifeTime;
    public bool isEnemyProjectile = false;

    public float projectileSpeed;
    private Vector2 lastPos;
    private Vector2 curPos;
    private Vector2 playerPos;
    public EnemyController originatingEnemy;



    void Start()
    {
        StartCoroutine(DeathDelay());
        if (!isEnemyProjectile)
        {
            transform.localScale = new Vector2(GameController.ProjectileSize, GameController.ProjectileSize);
        }
    }

    void Update()
    {
        if (isEnemyProjectile)
        {
            curPos = transform.position;

            
            transform.position = Vector2.MoveTowards(transform.position, playerPos, projectileSpeed * Time.deltaTime);

            if (curPos == lastPos)
            {
                Destroy(gameObject);
            }
            lastPos = curPos;
        }
    }

    public void GetPlayer(Transform player)
    {
        playerPos = player.position;
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && isEnemyProjectile)
        {
            if (originatingEnemy != null)
            {
                int adjustedDamage = Mathf.RoundToInt(originatingEnemy.damage);
                GameController.DamagePlayer(adjustedDamage);
                Debug.Log($"Projectile hit player with damage: {adjustedDamage}");
                GameController.scoreMultiplier = Mathf.Max(GameController.scoreMultiplier - 0.1f, 1.0f);
                Debug.Log($"Score multiplier reduced to: {GameController.scoreMultiplier}");
            }

            Destroy(gameObject);
        }
        else if (col.CompareTag("Wall") || col.CompareTag("Obstacle"))
        {
            // Destrueix el projectil si col·lisiona amb una paret o obstacle
            Debug.Log($"Projectile destroyed upon hitting: {col.name}");
            Destroy(gameObject);
        }
    }
}
