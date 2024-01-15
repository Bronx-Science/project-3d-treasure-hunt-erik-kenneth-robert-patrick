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
    public AudioSource WinSound;

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
        Health -= damage;

        Effect.color.Override(Color.Lerp(Color.red, Color.black, Health / MaxHealth));

        if(Health < 0)
        {
            Effect.color.Override(Color.red);

            EndGame();
        }

        HurtSound.Play();
    }

    public void EndGame()
    {
        DeathSound.Play();

        CameraScript.bDead = true;

        bDead = true;

        TimeDead = Time.time;

        Invoke(nameof(ShowDeathScreen), 4);
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

    void ShowDeathScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "win")
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        WinSound.Play();

        Invoke(nameof(ShowWinScreen), 1);
    }

    void ShowWinScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
