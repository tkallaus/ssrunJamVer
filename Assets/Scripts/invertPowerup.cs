using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invertPowerup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        pMovement.gravityInverted = !pMovement.gravityInverted;
        gameObject.SetActive(false);
    }
}
