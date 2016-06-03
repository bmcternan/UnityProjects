using UnityEngine;
using System.Collections;
using Valve.VR;


//[RequireComponent(typeof(SteamVR_TrackedObject))]
public class WorldMover : MonoBehaviour
{
    public GameObject HeadObject = null;
    public GameObject LeftControllerObject = null;
    public GameObject RightControllerObject = null;
    public GameObject WorldCenterer = null;
    public GameObject WorldRotator = null;
    public GameObject WorldUncenterer = null;
    private SteamVR_TrackedObject LeftController = null;
    private SteamVR_TrackedObject RightController = null;
    private SteamVR_TrackedObject HMD = null;

    private Vector3 _headAnchor;
    private Vector3 _leftAnchor;
    private Vector3 _rightAnchor;
    private bool _rotatingWorld = false;
    private float _anchorAngle;
    private float _lastDeltaAngle = 0.0f;
    private Vector3 _offset ;
    private Vector3 _lastDbgPos;
    private bool _triggerDown = false;
    public GameObject Origin = null;

    public static void FromMatrix4x4(Transform transform, Matrix4x4 matrix)
    {
        transform.localScale = matrix.GetScale();
        transform.rotation = matrix.GetRotation();
        transform.position = matrix.GetPosition();
    }

    public static Quaternion GetRotation(Matrix4x4 matrix)
    {
        var qw = Mathf.Sqrt(1f + matrix.m00 + matrix.m11 + matrix.m22) / 2;
        var w = 4 * qw;
        var qx = (matrix.m21 - matrix.m12) / w;
        var qy = (matrix.m02 - matrix.m20) / w;
        var qz = (matrix.m10 - matrix.m01) / w;

        return new Quaternion(qx, qy, qz, qw);
    }

    public static Vector3 GetPosition(Matrix4x4 matrix)
    {
        var x = matrix.m03;
        var y = matrix.m13;
        var z = matrix.m23;

        return new Vector3(x, y, z);
    }

    public static Vector3 GetScale(Matrix4x4 m)
    {
        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);

        return new Vector3(x, y, z);
    }

    void Awake ()
    {
        if ((LeftControllerObject == null) || (RightControllerObject == null) || (HeadObject == null))
            return;
        LeftController = LeftControllerObject.GetComponent<SteamVR_TrackedObject>();
        RightController = RightControllerObject.GetComponent<SteamVR_TrackedObject>();
        HMD = HeadObject.GetComponent<SteamVR_TrackedObject>();

        _offset = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void SetGridVisibility(bool visible)
    {
        var compositor = OpenVR.Compositor;
        if (compositor != null)
            compositor.ClearSkyboxOverride();
        compositor.FadeGrid(1.0f, visible);
    }

    void FixedUpdate ()
    {
        if ((LeftController == null) || (RightController == null) || (HMD == null))
            return;
        if (HeadObject == null)
            return;
        if (WorldRotator == null)
            return;
        if (WorldCenterer == null)
            return;
        if (WorldUncenterer == null)
            return;

        if ((LeftController.index != SteamVR_TrackedObject.EIndex.None) &&
            (RightController.index != SteamVR_TrackedObject.EIndex.None) &&
            (HMD.index != SteamVR_TrackedObject.EIndex.None))
        {
            SteamVR_Controller.Device leftDevice = SteamVR_Controller.Input((int)LeftController.index);
            SteamVR_Controller.Device rightDevice = SteamVR_Controller.Input((int)RightController.index);
            if (leftDevice.GetTouch(SteamVR_Controller.ButtonMask.Grip) && rightDevice.GetTouch(SteamVR_Controller.ButtonMask.Grip))
            {
                SteamVR_Controller.Device HMDDevice = SteamVR_Controller.Input((int)HMD.index);
                if (_rotatingWorld)
                {
                    Vector3 headPos = this.transform.InverseTransformPoint(HeadObject.transform.position);
                    Vector3 leftPos = this.transform.InverseTransformPoint(LeftControllerObject.transform.position);
                    Vector3 rightPos = this.transform.InverseTransformPoint(RightControllerObject.transform.position);

                    headPos.y = 0.0f;
                    leftPos.y = 0.0f;
                    rightPos.y = 0.0f;

                    _offset = rightPos - _rightAnchor;

                    Vector3 delta = leftPos - rightPos ;
                    delta.y = 0.0f;
                    delta.Normalize();
                    float dx = delta.x;
                    float dy = delta.z;
                    float angle = 180.0f * Mathf.Atan2(dy, dx) / Mathf.PI;

                    float deltaAngle = _anchorAngle - angle;

                                        WorldCenterer.transform.localPosition = rightPos;
                                        WorldRotator.transform.localEulerAngles = new Vector3(0.0f, deltaAngle + _lastDeltaAngle, 0.0f);
                                        WorldUncenterer.transform.localPosition = (-1.0f * rightPos) +_offset ;

                    //Matrix4x4 centerMat = new Matrix4x4 () ;
                    //Matrix4x4 rotateMat = new Matrix4x4();
                    //Matrix4x4 uncenterMat = new Matrix4x4();

                    //centerMat.SetTRS(rightPos, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));

                    //Quaternion rotation = Quaternion.identity;
                    //rotation.eulerAngles = new Vector3(0, deltaAngle + _lastDeltaAngle, 0);
                    //rotateMat.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), rotation, new Vector3(1.0f, 1.0f, 1.0f));

                    //Vector3 shift = new Vector3();
                    //shift = (-1.0f * rightPos) + _offset;
                    //uncenterMat.SetTRS(shift, Quaternion.identity, new Vector3(1.0f, 1.0f, 1.0f));

                    //Matrix4x4 result = new Matrix4x4();
                    //result = centerMat * rotateMat * uncenterMat;
                    //FromMatrix4x4(WorldUncenterer.transform, result);

                    if (rightDevice.GetTouch(SteamVR_Controller.ButtonMask.Trigger) && leftDevice.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
                    {
                        Debug.Log("Zeroing");
                        this.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                        WorldRotator.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                        WorldCenterer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        WorldUncenterer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    }

                }
                else
                {
                    _headAnchor = this.transform.InverseTransformPoint(HeadObject.transform.position);
                    _leftAnchor = this.transform.InverseTransformPoint(LeftControllerObject.transform.position);
                    _rightAnchor = this.transform.InverseTransformPoint(RightControllerObject.transform.position);
                    _headAnchor.y = 0.0f;
                    _leftAnchor.y = 0.0f;
                    _rightAnchor.y = 0.0f;
                    Vector3 delta = _leftAnchor - _rightAnchor;
                    delta.y = 0.0f;
                    delta.Normalize();
                    float dx = delta.x;
                    float dy = delta.z;
                    _anchorAngle = 180.0f * Mathf.Atan2(dy, dx) / Mathf.PI;

                    _rotatingWorld = true;

                    //Debug.Log("Enter pos World " + this.transform.localPosition.ToString());
                    //Debug.Log("       Centerer " + WorldCenterer.transform.localPosition.ToString());
                    //Debug.Log("     Uncenterer " + WorldUncenterer.transform.localPosition.ToString()); 
                    //Debug.Log("         origin " + Origin.transform.localPosition.ToString());

                    SetGridVisibility(true);
                }
            }
            else
            {
                if (_rotatingWorld)
                {
                    //Debug.Log("Preexit pos World " + this.transform.localPosition.ToString());
                    //Debug.Log("         Centerer " + WorldCenterer.transform.localPosition.ToString());
                    //Debug.Log("       Uncenterer " + WorldUncenterer.transform.localPosition.ToString());
                    //Debug.Log("           origin " + Origin.transform.localPosition.ToString());

                    Matrix4x4 mat = WorldUncenterer.transform.localToWorldMatrix;

                    FromMatrix4x4(this.transform, mat);

                    //WorldUncenterer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    //WorldUncenterer.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

                                       WorldRotator.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                                       WorldCenterer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                                       WorldUncenterer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

                    //Debug.Log("Exit pos World " + this.transform.localPosition.ToString());
                    //Debug.Log("      Centerer " + WorldCenterer.transform.localPosition.ToString());
                    //Debug.Log("    Uncenterer " + WorldUncenterer.transform.localPosition.ToString());
                    //Debug.Log("        origin " + Origin.transform.localPosition.ToString());

                    SetGridVisibility(false);
                }
                _rotatingWorld = false;
            }
            if (_triggerDown && !rightDevice.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
                _triggerDown = false;
        }

    }
}
