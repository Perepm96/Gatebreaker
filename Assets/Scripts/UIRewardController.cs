using UnityEngine;
using UnityEngine.UI;

public class RewardController : MonoBehaviour
{
    public GameObject rewardsUI; // Interfície de recompenses

    void Update()
    {
        // Comprova si la sala està neta
        if (RoomController.instance != null && RoomController.instance.roomComplete)
        {
            ShowRewardsUI();
        }
        else
        {
            HideRewardsUI();
        }
    }

    void ShowRewardsUI()
    {
        if (!rewardsUI.activeSelf)
        {
            rewardsUI.SetActive(true); // Mostra la interfície de recompenses
            RoomController.instance.roomComplete = false; // Reseteja la booleana
        }
    }
    void HideRewardsUI()
    {
        if (!rewardsUI.activeSelf)
        {
            rewardsUI.SetActive(false); // Mostra la interfície de recompenses
        }
    }
}
