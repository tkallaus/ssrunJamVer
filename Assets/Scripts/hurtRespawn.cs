using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hurtRespawn : MonoBehaviour
{
    public Vector2[] checkpointLoc;
    public bool[] checkpointGrav;
    public static int currentCheckpoint = 0;

    public Transform player;
    public GameObject[] thingsToReset;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        player.position = checkpointLoc[currentCheckpoint];
        pMovement.gravityInverted = checkpointGrav[currentCheckpoint];
        foreach (GameObject g in thingsToReset)
        {
            g.SetActive(true);
        }
    }
}
