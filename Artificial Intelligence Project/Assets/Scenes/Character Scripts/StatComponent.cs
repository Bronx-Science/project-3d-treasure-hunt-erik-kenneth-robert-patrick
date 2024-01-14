using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatComponent : MonoBehaviour
{
    public float MaxHealth;
    private float Health;

    public AudioSource HurtSound;
    public AudioSource DeathSound;

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
    }

    public void Damage(float damage)
    {
        HurtSound.Play();

        Health -= damage;

        if(Health < 0)
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
