using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilController : MonoBehaviour
{
    public float lifeTime;
    public bool isEnemyProjectile = false;

    private Vector2 lastPos;
    private Vector2 curPos;
    private Vector2 playerPos;


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

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy" && !isEnemyProjectile)
        {
            col.gameObject.GetComponent<EnemyController>().Death();
            Destroy(gameObject);
        }

        if (col.tag == "Player" && isEnemyProjectile)
        {
            GameController.DamagePlayer(1);
            Destroy(gameObject);
        }
    }
}