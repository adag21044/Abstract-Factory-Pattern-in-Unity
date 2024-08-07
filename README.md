# Abstract Factory Pattern in Unity

This project demonstrates the implementation of the Abstract Factory pattern in Unity. The project includes various vehicle types such as Cars and Trucks, which can be either Fast or Slow. Each vehicle type is created by its respective factory, adhering to the Abstract Factory design pattern.

## Classes

### Vehicle.cs

An abstract class representing a generic vehicle.

```csharp
using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{
    private const float ContainerShipPickupLocationOnXAxis = 33.5f;
    protected float MovementSpeed = 0f;
    public Transform containerShip;
    private bool _isOnContainerShip;

    public enum VehicleType
    {
        Car,
        Truck
    }

    public abstract VehicleType GetVehicleType();

    void Update()
    {
        if (!_isOnContainerShip)
        {
            if (transform.position.x <= ContainerShipPickupLocationOnXAxis)
            {
                transform.Translate(Vector3.right * (MovementSpeed * Time.deltaTime));   
            }
            else
            {
                var ship = containerShip.GetComponent<ContainerShip>();
                if (ship.isDocked && !ship.IsFull())
                {
                    var loadPosition = ship.LoadVehicle(transform);
                    transform.position = loadPosition;
                    transform.Rotate(0f, 90f, 0f, Space.Self);
                    _isOnContainerShip = true;
                }
            }
        }
    }
}
```
### Car
A concrete implementation of `Vehicle`.
```csharp
public class Car : Vehicle
{
    public override VehicleType GetVehicleType() => VehicleType.Car;
}
```
### Truck
A concrete implementation of `Vehicle`.
```csharp
public class Truck : Vehicle
{
    public override VehicleType GetVehicleType() => VehicleType.Truck;
}
```
### FastCar and SlowCar
Concrete implementations of `Car`.
```csharp
public class FastCar : Car
{
    private void Start() => MovementSpeed = 7;
}

public class SlowCar : Car
{
    private void Start() => MovementSpeed = 4;
}
```
### FastTruck and SlowTruck
Concrete implementations of `Truck`.
```csharp
public class FastTruck : Truck
{
    private void Start() => MovementSpeed = 4;
}

public class SlowTruck : Truck
{
    private void Start() => MovementSpeed = 3;
}
```
### VehicleFactory
An abstract factory class for creating vehicles.
```csharp
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
```
### CarFactory
A factory for creating cars.
```csharp
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
```
### TruckFactory
A factory for creating trucks.
```csharp
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
```
### Client
A client class that manages the creation and assignment of factories to buildings.
```csharp
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    private const int MaxNumberOfFactoryBuildings = 3;
    private const string FactoryBuildingPrefabName = "Factory";
    private const string ContainerShipPrefabName = "ContainerShip";

    private readonly Transform[] _factoryBuildings = new Transform[MaxNumberOfFactoryBuildings];
    private readonly Transform[] _containerShips = new Transform[MaxNumberOfFactoryBuildings];

    public TMP_Dropdown buildingDropdown;
    public TMP_Dropdown factoryTypeDropdown;

    private void Start()
    {
        InvokeRepeating(nameof(ProduceVehicles), 5, 3);
    }

    public void ProduceVehicles()
    {
        for (var i = 0; i < MaxNumberOfFactoryBuildings; i++)
        {
            var factory = _factoryBuildings[i]?.GetComponent<VehicleFactory>();
            if (factory == null) continue;

            var ship = _containerShips[i]?.GetComponent<ContainerShip>();
            if (ship == null || !ship.isDocked || ship.IsFull()) continue;

            string vehicleType = Random.Range(0, 2) == 0 ? "Fast" : "Slow";
            factory.CreateVehicleInstance(vehicleType);
        }
    }

    public void AssignFactoryToBuilding()
    {
        Transform factoryBuildingTransform = null;
        Transform containerShipTransform = null;

        int buildingIndex = buildingDropdown.value;
        int zAxisPosition;

        switch (buildingIndex)
        {
            case 0:
                zAxisPosition = 48;
                break;
            case 1:
                zAxisPosition = 34;
                break;
            case 2:
                zAxisPosition = 20;
                break;
            default:
                throw new System.ArgumentException("Invalid building index");
        }

        if (_factoryBuildings[buildingIndex] == null)
        {
            _factoryBuildings[buildingIndex] = CreateFactoryBuilding(zAxisPosition);
            _containerShips[buildingIndex] = CreateContainerShip(_factoryBuildings[buildingIndex]);
        }

        factoryBuildingTransform = _factoryBuildings[buildingIndex];
        containerShipTransform = _containerShips[buildingIndex];

        if (factoryBuildingTransform == null) return;

        factoryBuildingTransform.gameObject.SetActive(false);

        Destroy(factoryBuildingTransform.GetComponent<VehicleFactory>());

        VehicleFactory factory;
        switch (factoryTypeDropdown.captionText.text)
        {
            case "CarFactory":
                factory = factoryBuildingTransform.gameObject.AddComponent<CarFactory>();
                break;
            case "TruckFactory":
                factory = factoryBuildingTransform.gameObject.AddComponent<TruckFactory>();
                break;
            default:
                throw new System.ArgumentException("Invalid factory type");
        }

        factory.containerShipTransform = containerShipTransform;
        factory.factoryBuildingTransform = factoryBuildingTransform;

        factoryBuildingTransform.gameObject.SetActive(true);
    }

    private Transform CreateContainerShip(Transform factoryBuilding)
    {
        var factoryTransformPosition = factoryBuilding.position;
        var containerShip = Resources.Load<GameObject>(ContainerShipPrefabName);
        if (containerShip == null) throw new System.ArgumentException($"{ContainerShipPrefabName} could not be found or loaded from Resources folder");

        var containerShipTransform = Instantiate(containerShip.transform, new Vector3(70, factoryTransformPosition.y - 2f, factoryTransformPosition.z), Quaternion.identity);
        containerShipTransform.Rotate(0f, 90f, 0f, Space.Self);

        return containerShipTransform;
    }

    private Transform CreateFactoryBuilding(int zAxisPosition)
    {
        var factory = Resources.Load<GameObject>(FactoryBuildingPrefabName);
        if (factory == null) throw new System.ArgumentException($"{FactoryBuildingPrefabName} could not be found or loaded from Resources folder");

        var newFactory = Instantiate(factory.transform, new Vector3(16, 1.5f, zAxisPosition), Quaternion.identity);
        return newFactory;
    }
}
```
### ContainerShip
A class representing a container ship that can load vehicles.
```csharp
using System.Collections.Generic;
using UnityEngine;

public class ContainerShip : MonoBehaviour
{
    private const int MaxCapacity = 6;
    private const float VehicleDumpLocationOnXAxis = 60;
    private readonly List<Transform> _loadedVehicles = new List<Transform>();

    public bool isDocked;

    public Vector3 LoadVehicle(Transform vehicle)
    {
        var shipPosition = transform.position;
        var position = new Vector3(
            shipPosition.x - (_loadedVehicles.Count % 2 == 0 ? -1 : 1),
            vehicle.GetComponent<Vehicle>().GetVehicleType() == Vehicle.VehicleType.Truck ? shipPosition.y + 0.95f : shipPosition.y + 0.65f,
            shipPosition.z + 1 - (((int)(_loadedVehicles.Count / 2)) * 2.3f)
        );

        _loadedVehicles.Add(vehicle);
        return position;
    }

    public bool IsFull() => MaxCapacity == _loadedVehicles.Count;

    private void Update()
    {
        if (!isDocked && !IsFull())
        {
            if (transform.position.x <= 40)
            {
                isDocked = true;
            }
            else
            {
                transform.Translate(Vector3.back * 5 * Time.deltaTime);
            }
        }

        if (IsFull())
        {
            isDocked = false;
            transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            foreach (var loadedVehicle in _loadedVehicles)
            {
                loadedVehicle.Translate(Vector3.forward * 5 * Time.deltaTime);
            }

            if (transform.position.x >= VehicleDumpLocationOnXAxis)
            {
                foreach (var loadedVehicle in _loadedVehicles)
                {
                    Destroy(loadedVehicle.gameObject);
                }
                _loadedVehicles.Clear();
            }
        }
    }
}
```
