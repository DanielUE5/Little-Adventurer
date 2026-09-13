using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform currentTarget;

    Vector2 beginPos;
    float Z;

    Vector2 beginCamPos => (Vector2)cam.transform.position - beginPos;
    float targetDistance => transform.position.z - currentTarget.transform.position.z;
    float renderLimit => cam.transform.position.z + (targetDistance > 0 ? cam.farClipPlane : cam.nearClipPlane);
    float parallaxValue => Mathf.Abs(targetDistance) / renderLimit;

    void Start()
    {
        beginPos = transform.position;
        Z = transform.position.z;
    }

    void Update()
    {
        Vector2 newPosition = beginPos + beginCamPos * parallaxValue;

        transform.position = new Vector3(newPosition.x, newPosition.y, Z);
    }
}
