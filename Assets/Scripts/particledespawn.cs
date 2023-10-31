using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particledespawn : MonoBehaviour
{
    public float timerMax;
    private float despawnTimer;
    private SpriteRenderer spr;
    private Color startRef;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        despawnTimer = timerMax;
        startRef = spr.color;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        spr.flipX = p.GetComponent<SpriteRenderer>().flipX;
        transform.localScale = p.transform.localScale;
    }

    private void Update()
    {
        spr.color = Color.Lerp(startRef, Color.clear, 1-despawnTimer/timerMax);
        despawnTimer -= Time.deltaTime;
        if(despawnTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
