using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class VehicleContainer : MonoBehaviour
{
    [SerializeField] Vehicle vehiclePrefab;
    
    [SerializeField] GameObject lanesContainer;
    List<Lane> lanes = new();

    List<Vehicle> vehicles = new();

    float vehicleSpawnY = 8f;

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
        var lane = lanes[Random.Range(0,lanes.Count)];
        var vehicle = Instantiate(vehiclePrefab);
        vehicle.transform.localPosition = new Vector2(
            lane.transform.localPosition.x,
            vehicleSpawnY
        );
        vehicle.destroyed += (v) =>
        {
            vehicles.Remove(v);
        };
        vehicle.Move();

        vehicles.Add(vehicle);
    }
}
