using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FlashlightAbility : MonoBehaviour
{
    public Light LightSource;
    public float Intensity;
    public float Duration;
    public float Cooldown;
    public KeyCode AbilityButton = KeyCode.Mouse0;
    public AudioSource Sound;
    public int Count;

    public Transform playerTransform;
    public Camera playerCamera;
    public float Reach;
    public float StunDuration;

    private float DefaultIntensity;
    private bool CanActivate;
    private bool Active;
    private float StartTime;

    // Start is called before the first frame update
    void Start()
    {
        CanActivate = true;

        DefaultIntensity = LightSource.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanActivate && Count > 0)
        {
            if (Input.GetKey(AbilityButton))
            {
                CanActivate = false;

                Invoke(nameof(ResetAbility), Cooldown);

                Count--;

                Active = true;
                StartTime = Time.time;

                Invoke(nameof(EndAbility), Duration);

                Stun();

                //Sound.Play();
            }
        }

        if(Active)
        {
            float TimeElapsed = Time.time - StartTime;

            if(TimeElapsed < Duration / 10)
            {
                LightSource.intensity = Mathf.Lerp(DefaultIntensity, Intensity, TimeElapsed / (Duration / 10));
            }

            else
            {
                LightSource.intensity = Mathf.Lerp(Intensity, DefaultIntensity, (TimeElapsed - (Duration / 10)) / ((Duration / 10) * 9));
            }
                
        }
    }

    void Stun()
    {
        Vector3 StartPosition = playerTransform.position;
        Vector3 StartDirection = playerCamera.transform.forward;

        RaycastHit[] Hits = Physics.RaycastAll(StartPosition, StartDirection * Reach, Reach);

        foreach (RaycastHit i in Hits)
        {
            if (i.transform.TryGetComponent<ChaserAIScript>(out ChaserAIScript ChaserClass))
            {
                ChaserClass.Stun(StunDuration);
            }

            if (i.transform.TryGetComponent<CrawlerAIScript>(out CrawlerAIScript CrawlerClass))
            {
                CrawlerClass.Stun(StunDuration);
            }
        }
    }

    void EndAbility()
    {
        Active = false;

        LightSource.intensity = DefaultIntensity;
    }

    void ResetAbility()
    {
        CanActivate = true;
    }
}
