using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerDet : MonoBehaviour
{
    public bool triggerOn = false;
    public float delayAmount = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerOn = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        StartCoroutine(delay());
        //triggerOn = false;
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(delayAmount);
        triggerOn = false;
    }
}
