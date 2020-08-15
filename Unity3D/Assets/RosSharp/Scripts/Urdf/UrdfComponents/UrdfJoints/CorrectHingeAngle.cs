// Calculate the angle and angular velocity between two rigidbodies about the axis of a hingejoint.
// Initial angle is 0, and should only be calculate once per FixedUpdate().
// 2020, Tyler Stephans (tbs5111@psu.edu)

using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class CorrectHingeAngle : MonoBehaviour
{
    private Vector3 _axis;  //local space

    private Vector3 dial;   // reference vector perpendicular to hinge axis (local frame)
    private Vector3 dial0;  // initial perpendicular reference vector (parent frame)

    private HingeJoint _hingeJoint;
    private float timeofUpdate = 0f;    // fixedTime when last UpdateAngle() was called
    public float currentAngle { get; private set; } = 0f;   // Do not reference outside CorrectHingeAngle. Is only public for use with CorrectHingeAngleEditor
    private float previousAngle;
    public float currentVelocity { get; private set; } = 0f; // Is only public for use with CorrectHingeAngleEditor. Do not reference from other scripts!!!

    void Start()
    {
        _hingeJoint = GetComponent<HingeJoint>();
        _axis = _hingeJoint.axis;

        // Find a good perpendicular vector to use as a reference
        Vector3 tempfwd = Vector3.Cross(_axis, Vector3.right);
        Vector3 tempup = Vector3.Cross(_axis, Vector3.up);
        if (tempfwd.magnitude > tempup.magnitude)
            dial = tempfwd.normalized;
        else
            dial = tempup.normalized;

        dial0 = ConnectedTransformVector(dial);
    }

    // Update the angle every FixedUpdate, but not if some other function has already called Angle() or Velocity()
    void FixedUpdate()
    {
        if (Time.fixedTime - timeofUpdate >= Time.fixedDeltaTime * 0.9)     // Not sure how likely it is for fixedTime - previous fixedTime != fixedDeltaTime, so I lower the bar
            UpdateAngle();
    }

    public float Angle()
    {
        if (Time.fixedTime - timeofUpdate >= Time.fixedDeltaTime * 0.9)
            UpdateAngle();
        
        return currentAngle;
    }

    public float Velocity()
    {
        if (Time.fixedTime - timeofUpdate >= Time.fixedDeltaTime * 0.9)
            UpdateAngle();

        return currentVelocity;
    }

    private void UpdateAngle()
    {
        previousAngle = currentAngle;

        Vector3 connectedAxis = ConnectedTransformVector(_axis);
        Vector3 currentDial = ConnectedTransformVector(dial);

        timeofUpdate = Time.fixedTime;
        currentAngle = Vector3.SignedAngle(dial0, currentDial, connectedAxis);
        currentVelocity = (currentAngle - previousAngle) / Time.fixedDeltaTime;
        // Calculate angular velocity using rigidbody angular velocities
        //Vector3 worldAxis = _hingeJoint.transform.TransformVector(_axis);
        //currentVelocity = Vector3.Dot(HingeAngVel(_hingeJoint), worldAxis.normalized)*180/Mathf.PI;
    }

    // convert vector to connected body reference frame
    private Vector3 ConnectedTransformVector(Vector3 vector)
    {
        return _hingeJoint.connectedBody.transform.InverseTransformVector(_hingeJoint.transform.TransformVector(vector));
    }

    private Vector3 HingeAngVel(HingeJoint hinge)
    {
        Vector3 connectedAngVel = Vector3.zero;
        //if (hinge.connectedBody != null && hinge.connectedBody.TryGetComponent<HingeJoint>(out var connectedHinge))
        if (hinge.connectedBody != null)
            connectedAngVel = hinge.connectedBody.angularVelocity;
        return hinge.GetComponent<Rigidbody>().angularVelocity - connectedAngVel;
    }
}
