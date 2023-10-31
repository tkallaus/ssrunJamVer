using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobileButtons : MonoBehaviour
{
    public pMovement target;
    public Function func;

    private SpriteRenderer spr;
    private Collider col;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider>();
    }

    private void OnMouseDown()
    {
        switch (func)
        {
            case Function.JUMP:
                target.mobileJumpInput = true;
                break;
            case Function.STOMP:
                target.mobileStompInput = true;
                break;
        }
    }
}

public enum Function
{
    JUMP,
    STOMP
}