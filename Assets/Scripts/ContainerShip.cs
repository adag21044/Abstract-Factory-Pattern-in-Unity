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