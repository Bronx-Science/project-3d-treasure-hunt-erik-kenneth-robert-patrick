using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterMovement cm;
    public AudioSource walkAudio;
    public AudioSource runAudio;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cm = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.magnitude > 2 && cm.grounded)
        {
            if(cm.Sprinting)
            {
                runAudio.enabled = true;
                walkAudio.enabled = false;
            }

            else
            {
                walkAudio.enabled = true;
                runAudio.enabled = false;
            }
        }

        else
        {
            walkAudio.enabled = false;
            runAudio.enabled = false;
        }
    }
}
