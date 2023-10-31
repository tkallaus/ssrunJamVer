using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class pMovement : MonoBehaviour
{
    private Rigidbody2D rig;
    public GameObject afterimageOBJ;
    public GameObject stompOBJ;
    public LayerMask stompLM;

    private SpriteRenderer spr;
    private Animator animController;
    private float walkCycle = 0.2f;

    public float baseAcceleration = 1250;
    private float acceleration = 1250;
    public float baseSpeedCap = 7;
    private float speedCap = 7;
    public float baseJumpHeight = 120;
    private float jumpHeight = 120;
    public int doubleJumpCap = 1;
    private int doubleJumpCurrent = 0;
    //private int kickCap = 1;
    private int kickCurrent = 0;

    public triggerDet regjumpies;
    public triggerDet lWalljumpies;
    public triggerDet rWalljumpies;
    public BoxCollider2D kickBox2D;

    private bool landFrame = false;
    private float velocityAnimSnapshot = 0;

    private bool jumpInput = false;
    private bool walljumpInput = false;
    private bool doubleJumpInput = false;
    private bool stompInput = false;
    private bool heldLandingInput = false;
    private bool reverseInput = false;
    private bool kickinInput = false;
    public bool mobileJumpInput = false;
    public bool mobileStompInput = false;

    private bool heldStomp = false;
    private bool stopAccel = false;

    public static bool gravityInverted = false;
    public static bool gameOver = false;
    public AudioClip[] sfx;
    private AudioSource soundPlayer;

    public static Stopwatch speedrunTimer;
    public Text t;
    void Start()
    {
        gravityInverted = false;
        rig = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        speedrunTimer = new Stopwatch();
        soundPlayer = GetComponent<AudioSource>();

        jumpHeight = baseJumpHeight;
    }

    void Update()
    {
        animController.SetBool("idle", false);
        animController.SetBool("walkin", false);
        animController.SetBool("jumpin", false);
        animController.SetBool("jumpin2", false);
        animController.SetBool("stoppin", false);
        animController.SetBool("kickin", false);
        if (!regjumpies.triggerOn && !landFrame)
        {
            landFrame = true;
            animController.SetBool("jumpin", true);
        }
        if (regjumpies.triggerOn && landFrame)
        {
            landFrame = false;
            animController.SetBool("walkin", true);
        }
        //test code to detect when the stopping anim should play, prolly change >= to > when i add a fully stopped anim
        if (Mathf.Abs(velocityAnimSnapshot) > Mathf.Abs(rig.velocity.x)
            && (rig.velocity.x-velocityAnimSnapshot >= 0 && !reverseInput || rig.velocity.x - velocityAnimSnapshot <= 0 && reverseInput)
            && regjumpies.triggerOn)
        {
            animController.SetBool("stoppin", true);
        }
        velocityAnimSnapshot = rig.velocity.x;
        if(regjumpies.triggerOn && Mathf.Abs(rig.velocity.x) < 1f)
        {
            animController.SetBool("idle", true);
        }
        //Normal jump on ground, Walljump in air against a wall, double/air jump in freefall
        if (Input.GetKeyDown(KeyCode.X)) //|| mobileJumpInput)
        {
            if (regjumpies.triggerOn)
            {
                jumpInput = true;
                animController.SetBool("jumpin", true);
            }
            else if (lWalljumpies.triggerOn || rWalljumpies.triggerOn)
            {
                walljumpInput = true;
                animController.SetBool("jumpin", true);
            }
            else if(doubleJumpCurrent < doubleJumpCap)
            {
                doubleJumpInput = true;
                doubleJumpCurrent++;
                animController.SetBool("jumpin2", true);
            }
            //mobileJumpInput = false;
        }

        //Stomp if in air and holding down
        if (Input.GetKeyDown(KeyCode.Z)) //|| mobileStompInput )
        {
            if (!regjumpies.triggerOn)
            {
                if(Input.GetAxis("Vertical") < 0)
                {
                    stompInput = true;
                    animController.SetBool("jumpin2", true);
                }
                else if(!kickinInput && kickCurrent < 1)
                {
                    kickinInput = true;
                    walkCycle = 0.15f;
                    animController.SetBool("kickin", true);
                    kickCurrent++;
                }
            }
            //mobileStompInput = false;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            heldLandingInput = true;
        }
        else
        {
            heldLandingInput = false;
        }

        //tada, traditional input
        float horiInput = Input.GetAxis("Horizontal");
        if(horiInput > 0f)
        {
            reverseInput = false;
        }
        else if(horiInput < 0f)
        {
            reverseInput = true;
        }

        //Just here in case, you know how it is.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void FixedUpdate()
    {
        //decides velocity variables; acceleration, speedcap
        MomentumFunc(stopAccel);
        
        //If gravity inverts, invert gravity (and flip player)
        if (gravityInverted)
        {
            rig.gravityScale = -3;
            transform.localScale = new Vector2(1, -1);
        }
        else
        {
            rig.gravityScale = 3;
            transform.localScale = new Vector2(1, 1);
        }

        //Normal jump
        if (jumpInput)
        {
            soundPlayer.PlayOneShot(sfx[1], 0.5f);
            if (!gravityInverted)
            {
                rig.velocity = new Vector2(rig.velocity.x, Mathf.Sqrt(jumpHeight * 2f));
            }
            else
            {
                rig.velocity = new Vector2(rig.velocity.x, -Mathf.Sqrt(jumpHeight * 2f));
            }
            jumpInput = false;
        }

        //Wall jump
        if (walljumpInput)
        {
            soundPlayer.PlayOneShot(sfx[1], 0.5f);

            //Disabled double jump replenish for now because of possible kick exploit
            //doubleJumpCurrent = 0;

            //Makes sure the wallboost doesn't get overridden by current momentum
            //unsure if good thing
            rig.velocity = Vector2.zero;
            //boosts the character off the wall a bit
            if (lWalljumpies.triggerOn)
            {
                rig.AddForce(transform.right * baseAcceleration * 15 * Time.fixedDeltaTime);
                reverseInput = false;
            }
            else
            {
                rig.AddForce(transform.right * baseAcceleration * -15 * Time.fixedDeltaTime);
                reverseInput = true;
            }

            if (!gravityInverted)
            {
                rig.velocity = new Vector2(rig.velocity.x, Mathf.Sqrt(jumpHeight * 2f));
            }
            else
            {
                rig.velocity = new Vector2(rig.velocity.x, -Mathf.Sqrt(jumpHeight * 2f));
            }
            walljumpInput = false;
        }

        //Air jump
        if (doubleJumpInput)
        {
            soundPlayer.PlayOneShot(sfx[3]);
            if (!gravityInverted)
            {
                rig.velocity = new Vector2(rig.velocity.x, Mathf.Sqrt(jumpHeight * 2f)/2);
            }
            else
            {
                rig.velocity = new Vector2(rig.velocity.x, -Mathf.Sqrt(jumpHeight * 2f)/2);
            }
            Instantiate(afterimageOBJ, transform.position, transform.rotation);
            doubleJumpInput = false;

            kickCurrent = 0;
        }

        //Stomp
        if (stompInput)
        {
            float stompVelocity = -100f;
            if (gravityInverted)
            {
                stompVelocity = -stompVelocity;
            }
            rig.AddForce(transform.up * stompVelocity);
            rig.velocity = new Vector2(0, rig.velocity.y);

            if(walkCycle < 0)
            {
                walkCycle = 0.1f;
                Instantiate(afterimageOBJ, transform.position, transform.rotation);
            }
            walkCycle -= Time.fixedDeltaTime;
        }

        //Boost if releasing the stomp button after touching the ground
        if (heldStomp)
        {
            if (!heldLandingInput || !regjumpies.triggerOn)
            {
                if (reverseInput)
                {
                    rig.AddForce(transform.right * baseAcceleration * -30 * Time.fixedDeltaTime);
                    heldStomp = false;
                }
                else
                {
                    rig.AddForce(transform.right * baseAcceleration * 30 * Time.fixedDeltaTime);
                    heldStomp = false;
                }
            }
        }

        //time to get weird
        if (kickinInput)
        {
            rig.velocity *= 0.5f;
            rig.gravityScale = 0;
            //Put kick windup animation here
            if(walkCycle < 0)
            {
                rig.AddForce(transform.right * acceleration * 45 * Time.fixedDeltaTime + transform.up * 200);
                kickinInput = false;
            }
            walkCycle -= Time.fixedDeltaTime;
        }
        //Expensive? idk
        if (animController.GetCurrentAnimatorStateInfo(0).IsTag("kick"))
        {
            kickBox2D.enabled = true;
        }
        else
        {
            kickBox2D.enabled = false;
        }

        //Built some brakes into your legs
        if (heldLandingInput && regjumpies.triggerOn)
        {
            rig.velocity = new Vector2(rig.velocity.x / 1.2f, rig.velocity.y);
            stopAccel = true;
        }
        else
        {
            stopAccel = false;
        }

        //if touching floor
        if (regjumpies.triggerOn)
        {
            //resets doublejump and triggers landing after a stomp input
            doubleJumpCurrent = 0;
            kickCurrent = 0;

            if (stompInput)
            {
                soundPlayer.PlayOneShot(sfx[2], 0.5f);
                Instantiate(stompOBJ, transform.position, transform.rotation);
                stompInput = false;
                if (heldLandingInput)
                {
                    heldStomp = true;
                }
            }

            //Decelerates past a certain speed
            if (speedCap > 0)
            {
                if (rig.velocity.x > speedCap)
                {
                    rig.AddForce(transform.right * -acceleration * 1.5f * Time.fixedDeltaTime);
                }
            }
            else
            {
                if (rig.velocity.x < speedCap)
                {
                    rig.AddForce(transform.right * -acceleration * 1.5f * Time.fixedDeltaTime);
                }
            }
        }
        else
        {
            //Decelerates past a certain speed, midair
            if (speedCap > 0)
            {
                if (rig.velocity.x > speedCap)
                {
                    rig.AddForce(transform.right * -acceleration * 1.1f * Time.fixedDeltaTime);
                }
            }
            else
            {
                if (rig.velocity.x < speedCap)
                {
                    rig.AddForce(transform.right * -acceleration * 1.1f * Time.fixedDeltaTime);
                }
            }
        }

        //the actual line that pushes the character normally
        rig.AddForce(transform.right * acceleration * Time.fixedDeltaTime);

        //If you beat the game, put final time and remove controls
        if (gameOver)
        {
            acceleration = 0;
            rig.velocity = Vector2.zero;
            regjumpies.triggerOn = false;
            
            t.text += speedrunTimer.Elapsed.ToString().Substring(0, 11);
            gameOver = false;
        }
    }

    private void MomentumFunc(bool stopAcceleration)
    {
        if (!stopAcceleration)
        {
            if (reverseInput)
            {
                acceleration = -baseAcceleration;
                speedCap = -baseSpeedCap;
                spr.flipX = true;
            }
            else
            {
                acceleration = baseAcceleration;
                speedCap = baseSpeedCap;
                spr.flipX = false;
            }
            if (!regjumpies.triggerOn)
            {
                acceleration /= 3;
            }
        }
        else
        {
            if (reverseInput)
            {
                acceleration = 0;
                speedCap = -baseSpeedCap;
                spr.flipX = true;
            }
            else
            {
                acceleration = 0;
                speedCap = baseSpeedCap;
                spr.flipX = false;
            }
        }
    }
}
