using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class DodgeMinigame : MonoBehaviour
{
    [SerializeField] VehicleContainer vehicleContainer;

    void Start()
    {
        Assert.IsNotNull(vehicleContainer);
    
        StartGame();
    }

    void StartGame()
    {
        vehicleContainer.SpawnVehicle();
    }

    public void LoseGame()
    {
        vehicleContainer.OnLose();

        SceneManager.LoadScene("MinigameSelect");
    }
}
