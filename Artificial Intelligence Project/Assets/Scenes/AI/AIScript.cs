using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIScript : MonoBehaviour
{
    public GameObject Player;
    private Transform PlayerTransform;
    public float RotSpeed = 3.0f, MoveSpeed = 3.0f;

    public AudioSource DeathSound;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        DeathSound.Play();

        Destroy(gameObject);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

