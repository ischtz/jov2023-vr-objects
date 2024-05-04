using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapUpright : MonoBehaviour
{

    // Event callback: Rotates to turn object upright
    public void SetUpright() {
        transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

        Debug.Log(string.Format("Object placed at {0}", transform.position));
    }


}
