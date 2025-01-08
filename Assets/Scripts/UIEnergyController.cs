using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIEnergyController : MonoBehaviour
{
    public GameObject energyContainer;
    private float fillValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fillValue = (int)GameController.CurrentEnergy;
        fillValue = fillValue / GameController.MaxEnergy;
        energyContainer.GetComponent<Image>().fillAmount = fillValue;
    }
}
