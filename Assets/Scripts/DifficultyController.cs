using UnityEngine;
using UnityEngine.UI;
//using TMPro; // Assegura't d'importar TextMeshPro

public class DifficultyController : MonoBehaviour
{
    public Text scoreMultiplierText; 

    void Start()
    {
        // Inicialització si cal
    }

    void Update()
    {
        // Actualitza el text utilitzant TextMeshPro
        scoreMultiplierText.text = $"Dificultat: {GameController.scoreMultiplier:F1}";
    }
}