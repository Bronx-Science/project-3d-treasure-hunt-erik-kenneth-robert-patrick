using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class StatComponent : MonoBehaviour
{
    public float MaxHealth;
    private float Health;

    public AudioSource HurtSound;
    public AudioSource DeathSound;

    public CameraScript CameraScript;
    public Volume PostProcess;
    Vignette Effect;

    private bool bDead;
    private float TimeDead;

    // Start is called before the first frame update
    void Start()
    {
        bDead = false;

        Health = MaxHealth;

        PostProcess.profile.TryGet<Vignette>(out Vignette VIGNETTE);

        Effect = VIGNETTE;
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

        CameraScript.bDead = true;

        bDead = true;

        TimeDead = Time.time;

        Invoke(nameof(ShowEndScreen), 4);
    }

    private void FixedUpdate()
    {
        if(!bDead)
        {
            return;
        }

        if(Time.time - TimeDead < 1)
        {
            transform.Rotate(90.0f / 60.0f, 0, 0);
        }

        float bruh = Time.time - TimeDead;

        Effect.intensity.value = Mathf.Lerp(0, 1, bruh / 2);
    }

    void ShowEndScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
