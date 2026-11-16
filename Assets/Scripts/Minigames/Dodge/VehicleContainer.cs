using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;
using ImGuiNET;

public class VehicleContainer : MonoBehaviour
{
    [SerializeField] Vehicle vehiclePrefab;
    
    [SerializeField] GameObject lanesContainer;
    List<Lane> lanes = new();

    List<Vehicle> vehicles = new();

    int lastSpawnedLane = 0;

    int toSpawn = 24;
    int numDestroyed = 0;

    int gameStage = 0;
    List<int> stageThresholds = new() { 4,8,12,16,20 };

    void Start()
    {
        Assert.IsNotNull(vehiclePrefab);

        Assert.IsNotNull(lanesContainer);
        foreach (Transform child in lanesContainer.transform)
        {
            var obj = child.gameObject;
            var lane = obj.GetComponent<Lane>();
            Assert.IsNotNull(lane);
            lanes.Add(lane);
        }

        stageThresholds = stageThresholds.OrderByDescending(ks => ks).ToList();
    }

    float VehicleVelocityOffset()
    {
        return gameStage switch
        {
            2 => 2f,
            3 => 3.5f,
            4 => 5f,
            5 => 7f,
            _ => 0f
        };
    }

    float SecondVehicleSpawnDelay()
    {
        return gameStage switch
        {
            1 => Random.Range(3f,5f),
            2 => Random.Range(1f,3f),
            3 => Random.Range(1f,2f),
            4 => Random.Range(0.5f,1f),
            _ => Random.Range(0.5f,0.8f)
        };
    }

    void UpdateGameStage()
    {
        foreach (var (i, t) in stageThresholds.WithIndex())
        {
            if (numDestroyed >= t)
            {
                gameStage = stageThresholds.Count-i;
                break;
            }
        }
    }

    Vehicle CreateVehicle()
    {
        var laneNum = 0;
        do
        {
            laneNum = Random.Range(0,lanes.Count);
        } while (laneNum == lastSpawnedLane);
        lastSpawnedLane = laneNum;
        var lane = lanes[laneNum];
        var vehicle = Instantiate(vehiclePrefab);
        vehicle.transform.SetParent(transform);
        vehicle.SpawnAt(lane);
        vehicle.SetMoveVelocity(Vehicle.BaseVelocity + VehicleVelocityOffset());
        vehicle.destroyed += (v) =>
        {
            vehicles.Remove(v);
            numDestroyed++;
            UpdateGameStage();
        };
        vehicles.Add(vehicle);
        return vehicle;
    }

    public void OnLose()
    {
        foreach (var v in vehicles)
            Destroy(v.gameObject);
        vehicles.Clear();
        lanes.Clear();
    }

    public void SpawnVehicle()
    {
        var vehicle = CreateVehicle();
        vehicle.destroyed += (v) =>
        {
            if (numDestroyed < toSpawn)
                SpawnVehicle();
        };
        vehicle.Move();
        StartCoroutine(SpawnSecondVehicle());
    }

    System.Collections.IEnumerator SpawnSecondVehicle()
    {
        yield return new WaitForSeconds(SecondVehicleSpawnDelay());
        Vehicle v = CreateVehicle();
        v.Move();
    }

    #if DEBUG
    void OnEnable() => DebugUI.Add(this, "Vehicle Container", DrawDebug);
    void OnDisable() => DebugUI.Remove(this);

    void DrawDebug()
    {
        ImGui.Text($"Game Stage: {gameStage}");
        ImGui.Text($"Num Destroyed: {numDestroyed}");
        ImGui.Text($"Vehicle Speed: {Vehicle.BaseVelocity+VehicleVelocityOffset():F2}");
    }
    #endif
}
