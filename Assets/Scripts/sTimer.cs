using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTimer : MonoBehaviour
{
    public function func;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (func)
        {
            case function.START:
                pMovement.speedrunTimer.Start();
                //Camera.main.GetComponent<AudioSource>().Play();
                break;
            case function.END:
                pMovement.speedrunTimer.Stop();
                //Camera.main.GetComponent<AudioSource>().Stop();
                pMovement.gameOver = true;
                break;
        }
        gameObject.SetActive(false);
    }
}

public enum function
{
    START,
    END
}
