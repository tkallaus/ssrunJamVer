using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpointUpdate : MonoBehaviour
{
    public int checkpoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hurtRespawn.currentCheckpoint = checkpoint;
    }
}
