using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

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

    public FlashlightAbility Flashlight;

    public GameObject LaserBlasterWin;
    public GameObject LaserBlasterPerson;

    public GameObject[] possibleSpawnLocations;

    public GameObject AI;
    public GameObject AI1;
    //public GameObject AI2;

    public Transform Playerpos;
    public CameraShake PlayerCameraShake;
    public StatComponent PlayerStats;

    // Start is called before the first frame update
    void Start()
    {
        bDead = false;

        Health = MaxHealth;

        PostProcess.profile.TryGet<Vignette>(out Vignette VIGNETTE);

        Effect = VIGNETTE;

        Flashlight = GetComponent<FlashlightAbility>();
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

        Invoke(nameof(ShowDeathScreen), 3);
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
        if (other.gameObject.tag == "pick up")
        {
            LaserBlasterWin.SetActive(false);
            LaserBlasterPerson.SetActive(true);
            Flashlight.HasLaserBlaster = true;

            MaxHealth = 500;
            Health = 500;

            Damage(1);

            SpawnAi();
        }
    }

    void SpawnAi()
    {
        for (int i = 0; i < possibleSpawnLocations.Length; i++)
        {
            int value = Random.Range(0, 2);

            if(value == 0)
            {
                ChaserAIScript Chaser = Instantiate(AI, possibleSpawnLocations[i].transform.position, Quaternion.identity).GetComponent<ChaserAIScript>();
                Chaser.Playerpos = Playerpos;
                Chaser.playerCameraShake = PlayerCameraShake;
                Chaser.playerStats = PlayerStats;
            }

            else
            {
                CrawlerAIScript Crawler = Instantiate(AI1, possibleSpawnLocations[i].transform.position, Quaternion.identity).GetComponent<CrawlerAIScript>();
                Crawler.Playerpos = Playerpos;
                Crawler.playerCameraShake = PlayerCameraShake;
                Crawler.playerStats = PlayerStats;
            }
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
