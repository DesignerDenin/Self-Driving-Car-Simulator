using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    [SerializeField] public float maxMotorTorque;
    [SerializeField] public float maxSteeringAngle;
    [SerializeField] private float topSpeed;
    [SerializeField] private float maxHandbrakeTorque;
    [SerializeField] private float torqueOverAllWheels;
    [SerializeField] private float reverseTorque;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float downForce = 100f;

    new Rigidbody rigidbody;

    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentSteeringAngle;
    public Vector3 centreOfMass;

    private float currentTorque;
    private float steer_angle;
    public float AccelInput { get; set; }
    public float BrakeInput { get; private set; }

    GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centreOfMass;
        currentTorque = torqueOverAllWheels;
    }

    public IEnumerator RecordSample()
    {
        yield return new WaitForSeconds(0.15f);
        CarSample sample = new CarSample();

        sample.position = transform.position;
        sample.rotation = transform.rotation;
        sample.speed = currentSpeed / 3.6f;
        sample.steeringAngle = currentSteeringAngle / maxSteeringAngle;

        gameController.carSamples.Enqueue(sample);
        sample = null;

        if (gameController.isRecording)
            StartCoroutine(RecordSample());
    }

    public void Move(float steering, float accel, float footbrake, float handbrake)
    {
        steering = Mathf.Clamp(steering, -1, 1);
        AccelInput = accel = Mathf.Clamp(accel, 0, 1);
        BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        steer_angle = steering * maxSteeringAngle;
        axleInfos[0].leftWheel.steerAngle = steer_angle;
        axleInfos[0].rightWheel.steerAngle = steer_angle;

        ApplyDrive(accel, footbrake);
        CapSpeed();

        if (handbrake > 0f)
        {
            var hbTorque = handbrake * maxHandbrakeTorque;
            axleInfos[1].leftWheel.brakeTorque = hbTorque;
            axleInfos[1].rightWheel.brakeTorque = hbTorque;

        }

        currentSpeed = rigidbody.velocity.magnitude * 3.6f;
        currentSteeringAngle = steer_angle;

        AddDownForce();
        ApplyLocalPositionToVisuals(axleInfos[0].leftWheel);
        ApplyLocalPositionToVisuals(axleInfos[0].rightWheel);
        ApplyLocalPositionToVisuals(axleInfos[1].leftWheel);
        ApplyLocalPositionToVisuals(axleInfos[1].rightWheel);

    }

    private void ApplyDrive(float accel, float footbrake)
    {
        float thrustTorque;

        thrustTorque = accel * (currentTorque / 2f);
        axleInfos[0].leftWheel.motorTorque = axleInfos[0].rightWheel.motorTorque = thrustTorque;

        for (int i = 0; i < 2; i++)
        {
            if (currentSpeed > 5 && Vector3.Angle(transform.forward, rigidbody.velocity) < 50f)
            {
                axleInfos[i].leftWheel.brakeTorque = brakeTorque * footbrake;
                axleInfos[i].rightWheel.brakeTorque = brakeTorque * footbrake;
            }
            else if (footbrake > 0)
            {
                axleInfos[i].leftWheel.brakeTorque = 0f;
                axleInfos[i].rightWheel.brakeTorque = 0f;
                axleInfos[i].leftWheel.motorTorque = -reverseTorque * footbrake;
                axleInfos[i].rightWheel.motorTorque = -reverseTorque * footbrake;
            }
        }
    }

    private void CapSpeed()
    {
        float speed = rigidbody.velocity.magnitude;
        speed *= 3.6f;
        if (speed > topSpeed)
            rigidbody.velocity = (topSpeed / 3.6f) * rigidbody.velocity.normalized;
    }

    // this is used to add more grip in relation to speed
    private void AddDownForce()
    {
        axleInfos[0].leftWheel.attachedRigidbody.AddForce(-transform.up * downForce *
        axleInfos[0].leftWheel.attachedRigidbody.velocity.magnitude);
    }


    private void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
