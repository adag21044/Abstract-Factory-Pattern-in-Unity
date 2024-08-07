using UnityEngine;

public abstract class VehicleFactory : MonoBehaviour
{
    public Transform factoryBuildingTransform;
    public Transform containerShipTransform;

    public abstract Vehicle CreateVehicle(string type);

    public void CreateVehicleInstance(string type)
    {
        var vehicle = CreateVehicle(type);
        vehicle.containerShip = containerShipTransform;
        vehicle.transform.position = GetFactorySpawnPosition(vehicle);
    }

    private Vector3 GetFactorySpawnPosition(Vehicle vehicle)
    {
        var pos = factoryBuildingTransform.position;
        return new Vector3(pos.x, pos.y - 2 + (vehicle.GetVehicleType() == Vehicle.VehicleType.Car ? 0.4f : 0.66f), pos.z);
    }
}