using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils
{

    public static void SetUICameraParma(Camera camera)
    {
        camera.clearFlags = CameraClearFlags.Nothing;
        camera.cullingMask = 1 << 5;
        camera.depth = 9000;
        camera.orthographic = true;
        camera.transform.position = Vector3.back * 10000;
    }
}
