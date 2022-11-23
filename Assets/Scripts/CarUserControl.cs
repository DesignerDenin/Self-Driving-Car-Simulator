using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarUserControl : MonoBehaviour
{
    private Steering steering;

    private CarController car;

    private void Awake()
    {
        car = GetComponent<CarController>();
        steering = new Steering();
        steering.Start();
    }

    private void FixedUpdate()
    {
        steering.UpdateValues();
        car.Move(steering.H, steering.V, steering.V, 0f);

    }
}
