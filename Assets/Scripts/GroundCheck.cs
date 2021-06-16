using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    [SerializeField]
    GameObject dust;
    void OnTriggerEnter2D (Collider2D col)
    {
        if(col.gameObject.tag.Equals("Ground"))
        Instantiate (dust, transform.position, dust.transform.rotation);
    }
}
