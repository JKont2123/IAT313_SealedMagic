using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public EnemyHealth eh;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MagicStrike")
        {
            eh.TakeDamage(2f);
        }
    }
}
