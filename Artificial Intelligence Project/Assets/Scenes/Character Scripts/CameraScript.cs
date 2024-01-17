using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public Slider slider;

    public float Sens;

    public Transform Orientation;

    float xRotation;
    float yRotation;

    public bool bDead;

    // Start is called before the first frame update
    void Start()
    {
        bDead = false;

        Sens = PlayerPrefs.GetFloat("currentSens", 100);
        slider.value = Sens / 100;

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

        PlayerPrefs.SetFloat("currentSens", Sens);
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * Sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * Sens;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void AdjustSpeed(float newSpeed)
    {
        Sens = newSpeed * 10;
    }
}
