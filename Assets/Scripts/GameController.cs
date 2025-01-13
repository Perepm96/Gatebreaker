using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public static GameController instance;

    //Player
    private static int currentHealth = 100;
    private static int maxHealth = 100;
    private static int currentEnergy = 150;
    private static int maxEnergy = 150;
    private static float energyregen = 5f; 
    private static float moveSpeed = 5f;

    //Slash
    private static float attackRate = 0.1f;
    private static float slashSize = 0.5f;
    private static float slashSpeed = 8f;

    //Projectile
    private static float projectileSize = 0.5f;

    // Score Multiplier

    public static float scoreMultiplier = 1.0f;



    public static int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public static int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public static int CurrentEnergy { get => currentEnergy; set => currentEnergy = value; }
    public static int MaxEnergy { get => maxEnergy; set => maxEnergy = value; }
    public static float EnergyRegenRate { get => energyregen; set => energyregen = value; }
    public static float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public static float AttackRate { get => attackRate; set => attackRate = value; }
    public static float SlashSize { get => slashSize; set => slashSize = value; }
    public static float SlashSpeed { get => slashSpeed; set => slashSpeed = value; }

    public static float ProjectileSize { get => projectileSize; set => projectileSize = value; }



    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    void Update()
    {
        
    }
    
    public static void DamagePlayer(int damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            KillPlayer();
        }

    }
    public static void EnergyAttack(int energy)
    {
        currentEnergy -= energy;
    }

    public static void RechargeEnergy(int recEnergy)
    {
        currentEnergy += recEnergy;
    }

    public static void HealPlayer(int healAmount)
    {
        maxHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth + (healAmount*2));
    }
    public void IncreaseSlashSize(float amount)
    {
        slashSize += amount;
    }
    public void IncreaseSlashSpeed(float amount)
    {
        slashSpeed += amount;
    }
    public void IncreaseSpeed(float amount)
    {
        moveSpeed += amount;
    }
    private static void KillPlayer()
    {
        Debug.Log("Player has died!");
       
        SceneManager.LoadScene(0); 
    }
}
