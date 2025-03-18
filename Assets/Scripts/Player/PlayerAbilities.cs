using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    Animator anim;

    //Ability 1: Magic Strike Variables
    public bool hasMagicStrike = false;
    public GameObject magicStrike;
    public Transform magicStrikePos;
    public float magicStrikeCooldown;
    float magicStrikeTimer;

    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        magicStrikeTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasMagicStrike)
        {
            if (magicStrikeTimer > 0)
            {
                magicStrikeTimer -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Q) && FindObjectOfType<PlayerMovement>().isGrounded)
            {
                CastAbility();
            }
        }
    }

    public void CastAbility()
    {
        if (magicStrikeTimer > 0)
            return;

        magicStrikeTimer = magicStrikeCooldown;
        anim.SetTrigger("isShooting");
        FindObjectOfType<AudioManager>().Play("MagicStrike");
    }

    public void SpawnMagicStrike()
    {
        Instantiate(magicStrike, magicStrikePos.position, magicStrikePos.rotation);
    }

    public void ResetMagicStrike()
    {
        anim.ResetTrigger("isShooting");
    }
}
