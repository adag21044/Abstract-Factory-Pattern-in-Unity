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
