using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashController : MonoBehaviour
{
    public float lifeTime;
    public bool isEnemyBullet = false;

    private Vector2 lastPos;
    private Vector2 curPos;
    private Vector2 playerPos;

    public int damage = 10; // Dany que el Slash infligeix

    void Start()
    {
        StartCoroutine(coroutineA());
        if (!isEnemyBullet)
        {
            transform.localScale = new Vector2(GameController.SlashSize, GameController.SlashSize);
        }
    }

    void Update()
    {
        if (isEnemyBullet)
        {
            curPos = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, playerPos, 5f * Time.deltaTime);
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

    IEnumerator coroutineA()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") || col.CompareTag("Boss"))
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Crida FlashRed per canviar el color a vermell
                enemy.FlashRed();
                // Infligeix dany a l'enemic
                enemy.TakeDamage(damage);
            }

            // Elimina el Slash després de col·lidir
            Destroy(gameObject);
        }
        /*else if (col.CompareTag("Wall") || col.CompareTag("Obstacle"))
        {
            // Destrueix el projectil si col·lisiona amb una paret o obstacle
            Debug.Log($"Projectile destroyed upon hitting: {col.name}");
            Destroy(gameObject);
        }*/
    }
    public void IncreaseAttack(int amount)
    {
        damage += amount;
    }
}