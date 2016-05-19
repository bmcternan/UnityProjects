using UnityEngine;
using System.Collections;

public class MouseScrollScript : MonoBehaviour
{
    private const float CENTER_Y = 180.0f;
    private float _anchorX;
    private float _anchorY;
    private bool _MouseIsDown = false;
    private float _rX = 0.0f;
    private float _rY = 0.0f;
    private float _AnchorRX = 0.0f;
    private float _AnchorRY = 180.0f;
    private const float _maxRX = 180.0f;
    private const float _maxRY = 180.0f;
    public float Xang = 0.0f;
    public float Yang = CENTER_Y;

    public GameObject GUIVideo = null;
    public Camera ClientCamera = null;
    public Camera OculusCamera = null;
    public GameObject OculusCorrector = null;

    private Resolution res;

    static bool ENABLE_INPUT_DISPLAY = true;
    //static bool ENABLE_INPUT_DISPLAY = false;

    // Use this for initialization
    void Start()
    {
#if UNITY_STANDALONE
        Resolution[] resolutions = Screen.resolutions;
        res = resolutions[resolutions.Length - 1];
#endif
        SetCameraAspectRatio();
        //ClientCamera.enabled = true;
        //OculusCamera.enabled = false;
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();
    }

    void SetCameraAspectRatio()
    {
#if UNITY_STANDALONE
        //float targetAspectRatio = 0;
        //if (Screen.fullScreen)
        //{
        //    targetAspectRatio = (float)res.width / (float)res.height;
        //}
        //else
        //{
        //    targetAspectRatio = (float)Screen.width / (float)Screen.height;
        //}
        //Camera.main.aspect = targetAspectRatio;
        Debug.Log("**** Setting aspect");
#endif
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (!_MouseIsDown)
            {
                _anchorX = Input.mousePosition.x;
                _anchorY = Input.mousePosition.y;
                Debug.Log("mouse down at " + _anchorX + " " + _anchorY);
                _MouseIsDown = true;
            }

            float dx = Input.mousePosition.x - _anchorX;
            float dy = Input.mousePosition.y - _anchorY;

            float w = -dx / Screen.width;
            float h = dy / Screen.height;

            Xang = _AnchorRX + (h * _maxRX);
            Yang = _AnchorRY + (w * _maxRY);

            //Debug.Log("Rot " + Xang + " " + Yang + " Anc " + _AnchorRX + " " + _AnchorRY);
        }
        else if (_MouseIsDown)
        {
            _AnchorRX = Xang;
            _AnchorRY = Yang;
            _MouseIsDown = false;
            Debug.Log("UP " + _AnchorRX + " " + _AnchorRY);
        }

        //        if (Input.GetKeyDown(KeyCode.Escape))
        if (Input.GetMouseButton(2))
        {
            _AnchorRX = 0.0f;
            _AnchorRY = CENTER_Y;
            Xang = _AnchorRX;
            Yang = _AnchorRY;
            Debug.Log("ZERO Rot " + Xang + " " + Yang);
        }

        if (ENABLE_INPUT_DISPLAY)
        {
            if (GUIVideo != null)
            {
                //if (Input.GetMouseButton(1))
                //    GUIVideo.SetActive(true);
                //else
                //    GUIVideo.SetActive(false);
                if (Input.GetMouseButton(1))
                    GUIVideo.GetComponent<AVProLiveCameraGUIDisplay>().enabled = true;
                else
                    GUIVideo.GetComponent<AVProLiveCameraGUIDisplay>().enabled = false;

            }
        }
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    Debug.Log("Client");
        //    ClientCamera.enabled = true;
        //    OculusCamera.enabled = false;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    Debug.Log("HMD");
        //    ClientCamera.enabled = false;
        //    OculusCamera.enabled = true;
        //}


        transform.eulerAngles = new Vector3(Xang, Yang, 0.0f);

        //OculusCorrector.transform.rotation = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.CenterEye));
    }
    void PreCull ()
    { 
        //OculusCorrector.transform.rotation = Quaternion.Inverse(OculusCamera.transform.localRotation);
    }
}
