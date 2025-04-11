using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class SquidController : MonoBehaviour
{
    // Body parts
    public GameObject frame;
    private Transform body;
    private Transform head;
    private Transform leftLeg;
    private Transform rightLeg;
    private Transform frontLeg;

    private Transform leftKnee;
    private Transform rightKnee;
    private Transform frontKnee;

    private Transform leftArm;
    private Transform rightArm;
    private Transform leftElbow;
    private Transform rightElbow;

    private Transform[] eyes;

    // Internal State
    private Boolean turning;
    private Boolean walking;
    private Boolean dying;
    private int walkTime;

    // Movement Settings
    public float walkSpeed = 4;
    public float turnSpeed = 4;
    public float deathSpeed = 4;
    public float hipRange = 60;
    public float neckRange = 30;
    public float neckMult = 2f;
    public int walkOffset;

    void Start()
    {
        frame = gameObject;
        body = frame.transform;
        leftLeg = body.Find("LeftLeg");
        rightLeg = body.Find("RightLeg");
        frontLeg = body.Find("FrontLeg");

        leftKnee = leftLeg.Find("Knee");
        rightKnee = rightLeg.Find("Knee");
        frontKnee = frontLeg.Find("Knee");

        leftArm = body.Find("LeftArm");
        rightArm = body.Find("RightArm");

        leftElbow = leftArm.Find("Knee");
        rightElbow = rightArm.Find("Knee");

        head = body.Find("Neck");

        eyes = new Transform[] { head.Find("Head/LeftEye/Eyeball"), head.Find("Head/RightEye/Eyeball")};

        walkOffset = Mathf.FloorToInt(hipRange * 2f / 3f);
    }

    void FixedUpdate()
    {
        if (turning)
        {
            body.localRotation *= Quaternion.Euler(0, turnSpeed, 0);
        }
        if (walking)
        {
            walkTime++;

            float headAngle = Mathf.PingPong(walkTime * walkSpeed / 2, neckRange * 2) - neckRange;
            head.localRotation = Quaternion.Euler(-headAngle, -headAngle * neckMult, headAngle);

            // First step
            float kneeAngle = WalkAngle(0);
            leftLeg.localRotation = Quaternion.Euler(kneeAngle, 0, 0);
            leftKnee.localRotation = Quaternion.Euler(-Math.Abs(kneeAngle), 0, 0);

            // Second step
            kneeAngle = WalkAngle(walkOffset);
            frontLeg.localRotation = Quaternion.Euler(kneeAngle, 0, 0);
            frontKnee.localRotation = Quaternion.Euler(-Math.Abs(kneeAngle), 0, 0);
            // Arms
            leftArm.localRotation = Quaternion.Euler(kneeAngle, 0, 20);
            rightArm.localRotation = Quaternion.Euler(-kneeAngle, 0, -20);
            leftElbow.localRotation = Quaternion.Euler(90, 0, 0);
            rightElbow.localRotation = Quaternion.Euler(90, 0, 0);

            // Third step
            kneeAngle = WalkAngle(2 * walkOffset);
            rightLeg.localRotation = Quaternion.Euler(kneeAngle, 0, 0);
            rightKnee.localRotation = Quaternion.Euler(-Math.Abs(kneeAngle), 0, 0);
        }
        if (dying)
        {
            foreach (Transform eye in eyes) {
                eye.localRotation *= Quaternion.Euler(0, deathSpeed, 0);
            }
        }
    }

    private float WalkAngle(int offset)
    {
        return Mathf.PingPong(walkTime * walkSpeed + offset, hipRange * 2) - (hipRange * 0.75f);
    }

    public void Turn()
    {
        turning = !turning;
    }

    public void Walk()
    {
        walking = !walking;
    }

    public void Die()
    {
        dying = !dying;
    }

    public void Reset()
    {
        dying = false;
        walking = false;
        turning = false;

        body.localRotation = Quaternion.Euler(0, 0, 0);

        leftLeg.localRotation = Quaternion.Euler(0, 0, 0);
        rightLeg.localRotation = Quaternion.Euler(0, 0, 0);
        frontLeg.localRotation = Quaternion.Euler(0, 0, 0);

        leftKnee.localRotation = Quaternion.Euler(0, 0, 0);
        rightKnee.localRotation = Quaternion.Euler(0, 0, 0);
        frontKnee.localRotation = Quaternion.Euler(0, 0, 0);

        leftArm.localRotation = Quaternion.Euler(0, 0, 20);
        rightArm.localRotation = Quaternion.Euler(0, 0, -20);

        leftElbow.localRotation = Quaternion.Euler(0, 0, -20);
        rightElbow.localRotation = Quaternion.Euler(0, 0, 20);

        head.localRotation = Quaternion.Euler(0, 0, 0);

        foreach (Transform eye in eyes) {
            eye.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }
}
