using UnityEngine;

public class TruckFactory : VehicleFactory
{
    private const string SlowTruckPrefabName = "SlowTruck";
    private const string FastTruckPrefabName = "FastTruck";

    public override Vehicle CreateVehicle(string type)
    {
        string prefabName = type == "Fast" ? FastTruckPrefabName : SlowTruckPrefabName;
        var prefab = Resources.Load<GameObject>(prefabName);
        if (prefab == null) throw new System.ArgumentException($"{prefabName} could not be found or loaded from Resources folder");
        return Instantiate(prefab).GetComponent<Truck>();
    }
}