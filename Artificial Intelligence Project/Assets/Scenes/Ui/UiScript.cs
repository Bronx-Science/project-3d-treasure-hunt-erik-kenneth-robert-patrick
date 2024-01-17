using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.TestTools;

public class UiScript : MonoBehaviour
{
    public TMP_Text StaminaText;
    public TMP_Text FlashlightAbilityCharges;
    public GameObject Player;
    public CharacterMovement PlayerCharacterMovement;
    public FlashlightAbility PlayerFlashlightAbility;
    public StatComponent PlayerStats;
    // Start is called before the first frame update
    void Start()
    {
        //PlayerCharacterMovement = Player.GetComponent<CharacterMovement>();
        //PlayerFlashlightAbility = Player.GetComponent<FlashlightAbility>();
        //PlayerStats = Player.GetComponent<StatComponent>();

        ChangeStamina();
        ChangeCharges();
    }

    public void ChangeStamina()
    {
        StaminaText.SetText("Stamina: " + ((int)PlayerCharacterMovement.GetStamina()).ToString());
    }

    public void ChangeCharges()
    {
        FlashlightAbilityCharges.SetText("Flashlight Charges: " + PlayerFlashlightAbility.GetCharges().ToString());
    }

    public void ChangeScore()
    {
        FlashlightAbilityCharges.SetText("Score: " + ((int)PlayerStats.GetScore()).ToString());
    }
}
