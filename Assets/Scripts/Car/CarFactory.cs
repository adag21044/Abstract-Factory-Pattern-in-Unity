using UnityEngine;
public class CarFactory : VehicleFactory
{
    private const string SlowCarPrefabName = "SlowCar";
    private const string FastCarPrefabName = "FastCar";

    public override Vehicle CreateVehicle(string type)
    {
        string prefabName = type == "Fast" ? FastCarPrefabName : SlowCarPrefabName;
        var prefab = Resources.Load<GameObject>(prefabName);
        if (prefab == null) throw new System.ArgumentException($"{prefabName} could not be found or loaded from Resources folder");
        return Instantiate(prefab).GetComponent<Car>();
    }
}