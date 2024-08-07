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