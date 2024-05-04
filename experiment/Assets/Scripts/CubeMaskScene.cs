using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMaskScene : MonoBehaviour
{

    // Root object - set this to an empty GameObject that cubes will be added to
    public GameObject maskSceneRoot;
    
    public int numberOfCubes = 100;
    public Vector2 distanceRange = new Vector2(1.5f, 10f);
    public Vector2 sizeRange = new Vector2(0.05f, 0.5f);
    public bool randomOrientation = true;
    public Vector3 eulerAngles = new Vector3(0f, 45f, 0f);
    public bool randomColor = false;
    public Color cubeColor = Color.blue;

    private GameObject[] cubeArray;
    
    void Start()
    {
        
        cubeArray = new GameObject[numberOfCubes];
        for(int c = 0; c < numberOfCubes; c++) {
            GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newCube.transform.parent = maskSceneRoot.transform;
            cubeArray[c] = newCube;
        }
        Randomize();

        // Hide mask scene by default
        maskSceneRoot.SetActive(false);
    }

    public void Show() {
        maskSceneRoot.SetActive(true);
    }

    public void Hide() {
        maskSceneRoot.SetActive(false);
    }

    public void Randomize() {

        foreach (GameObject cube in cubeArray) {

            // Scale
            float size = Random.Range(sizeRange.x, sizeRange.y);
            cube.transform.localScale = new Vector3(size, size, size);

            // Orientation
            if (randomOrientation) {
                cube.transform.rotation = Random.rotation;
            } else {
                cube.transform.eulerAngles = eulerAngles;
            }

            // Position
            float distance = Random.Range(distanceRange.x, distanceRange.y);
            cube.transform.position = Random.onUnitSphere * distance;

            // Color
            if (randomColor ) {
                cube.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            } else {
                cube.GetComponent<MeshRenderer>().material.color = cubeColor;
            }
        }
    }

}
