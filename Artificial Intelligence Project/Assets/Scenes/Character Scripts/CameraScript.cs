using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float Senx;
    public float Seny;

    public Transform Orientation;

    float xRotation;
    float yRotation;

    public bool bDead;

    // Start is called before the first frame update
    void Start()
    {
        bDead = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(bDead)
        {
            return;
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * Senx;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * Seny;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
