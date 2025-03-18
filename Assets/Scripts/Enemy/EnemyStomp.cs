using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStomp : MonoBehaviour
{
    public EnemyHealth eh;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "WeakPoint")
        {
            eh.TakeDamage(1f);
        }
    }
}
