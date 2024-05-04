using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR;

public class LaserPointer : MonoBehaviour
{

    // Laser pointer script, inspired by SteamVR_LaserPointer.cs

    public GameObject origin;
    public GameObject laserDot;

    public SteamVR_Action_Boolean triggerAction; 
    public float thickness = 0.002f;
    public float dotSize = 0.01f;
    public Color laserColor = Color.red;

    private GameObject laser;
    private Vector3 lastPos = Vector3.zero;
    private float length = 100f;
    private float defaultLength = 100f;
    private bool visible = false;

    void Start()
    {
        if (origin == null) {
            origin = new GameObject();
            origin.transform.parent = this.transform;
            origin.transform.localPosition = Vector3.zero;
            origin.transform.localRotation = Quaternion.identity;
        }

        laser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        laser.transform.parent = origin.transform;
        laser.transform.localScale = new Vector3(thickness, thickness, length);
        laser.transform.localPosition = new Vector3(0f, 0f, length/2f);
        laser.transform.localRotation = Quaternion.identity;
        laser.GetComponent<MeshRenderer>().material.color = laserColor;
        DestroyImmediate(laser.GetComponent<BoxCollider>());

        if (laserDot == null) {
            laserDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            DestroyImmediate(laserDot.GetComponent<SphereCollider>());
            laserDot.transform.localScale = new Vector3(dotSize, dotSize, dotSize);
            laserDot.transform.localPosition = new Vector3(0f, 0f, 50f);
            laserDot.transform.localRotation = Quaternion.identity;
            laserDot.GetComponent<MeshRenderer>().material.color = laserColor;
        }

        if (triggerAction == null) triggerAction = SteamVR_Actions._default.InteractUI;
        
        // Invisible by default
        SetVisible(false);
    }

    // Update is called once per frame
    void Update()
    {
            // Only update when laser is actually visible
            if (visible) {
                Ray raycast = new Ray(origin.transform.position, transform.forward);
                RaycastHit hit;
                bool bHit = Physics.Raycast(raycast, out hit);

                if (bHit) {

                    // Raycast successful, update pointer length
                    laserDot.transform.position = hit.point;
                    length = Vector3.Distance(origin.transform.position, hit.point);

                } else {
                    
                    // Raycast invalid, set to default length
                    length = defaultLength;
                }

                laser.transform.localScale = new Vector3(thickness, thickness, length);
                laser.transform.localPosition = new Vector3(0f, 0f, length/2f);
                laser.transform.localRotation = Quaternion.identity;
            }
    }

    public IEnumerator WaitForClick() {

        while(true) {
            if (triggerAction.GetStateDown(SteamVR_Input_Sources.Any)) {
                lastPos = laserDot.transform.position;
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public Vector3 GetLastClickedPosition() {
        if (visible) {
            return lastPos;
        } else {
            return Vector3.zero;
        }
        
    }

    public void SetVisible(bool newVisbility) {
        
        if (newVisbility == true) {
            laser.GetComponent<MeshRenderer>().enabled = true;
            laserDot.GetComponent<MeshRenderer>().enabled = true;
            visible = true;
        } else {
            laser.GetComponent<MeshRenderer>().enabled = false;
            laserDot.GetComponent<MeshRenderer>().enabled = false;            
            visible = false;
        }
    }

}
