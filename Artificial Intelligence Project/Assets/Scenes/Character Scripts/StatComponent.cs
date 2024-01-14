using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatComponent : MonoBehaviour
{
    public float MaxHealth;
    private float Health;

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
    }

    public void Damage(float damage)
    {
        Health -= damage;

        if(Health < 0)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
