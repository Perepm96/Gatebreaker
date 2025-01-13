using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject lifeContainer;
    private float fillValue;
    public Text hpText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fillValue = (int)GameController.CurrentHealth;
        fillValue = fillValue / GameController.MaxHealth;
        lifeContainer.GetComponent<Image>().fillAmount = fillValue;
        hpText.text = GameController.CurrentHealth+"/"+GameController.MaxHealth;
    }
}
