using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    public GameObject Player;
    private Transform PlayerTransform;
    public float RotSpeed = 3.0f, MoveSpeed = 3.0f;

    // Use this for initialization
    void Start()
    {
        PlayerTransform = Player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        /* Look at Player*/
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(PlayerTransform.position - transform.position), RotSpeed * Time.deltaTime);

        /* Move at Player*/
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;
    }
}

