using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiScript : MonoBehaviour
{
    public TMP_Text StaminaText;
    public TMP_Text FlashlightAbilityCharges;
    public GameObject Player;
    private CharacterMovement PlayerCharacterMovement;
    private FlashlightAbility PlayerFlashlightAbility;
    // Start is called before the first frame update
    void Start()
    {
        PlayerCharacterMovement = Player.GetComponent<CharacterMovement>();
        PlayerFlashlightAbility = Player.GetComponent<FlashlightAbility>();
    }

    // Update is called once per frame
    void Update()
    {
        StaminaText.SetText("Stamina: " + ((int)PlayerCharacterMovement.GetStamina()).ToString());
        FlashlightAbilityCharges.SetText("Flashlight Charges: " + ((int)PlayerFlashlightAbility.GetCharges()).ToString());
    }
}
