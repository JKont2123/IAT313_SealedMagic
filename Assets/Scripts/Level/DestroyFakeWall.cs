using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFakeWall : MonoBehaviour
{
    public GameObject trigger;

    void Update()
    {
        if(trigger == null)
        {
            this.gameObject.SetActive(false);
        }
    }
}
