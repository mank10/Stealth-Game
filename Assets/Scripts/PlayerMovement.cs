using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSmoothing = 0.2f;
    public float speedDampTime = 0.1f;
    public float runSpeed = 10f;
    public float sneakSpeed = 5f;

    public GameController gameController;

    public AudioSource runningAudio;
    public AudioSource whistleAudio;

    public bool shouting = false;

    private Animator animator;
    private Rigidbody rbPlayer;
    private Transform mainCameraT;

    private float turnSmoothVelocity;
    private float currentSpeed;
    private float speedDampVelocity;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        mainCameraT = Camera.main.transform;
        rbPlayer = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 inputDir = input.normalized;

        if(inputDir != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + mainCameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothing);

        }

        bool sneak = Input.GetButton("Sneak");

        float speed = (sneak? sneakSpeed : runSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, speed, ref speedDampVelocity, speedDampTime);

        if(currentSpeed > 7 && !runningAudio.isPlaying)
        {
            runningAudio.PlayOneShot(runningAudio.clip, 0.1f);
        }
        else if(currentSpeed < 6 && runningAudio.isPlaying)
        {
            runningAudio.Stop();
        }

        if(shouting && !whistleAudio.isPlaying)
        {
            whistleAudio.PlayOneShot(whistleAudio.clip);
        }
        else if(!shouting && whistleAudio.isPlaying)
        {
            whistleAudio.Stop();
        }

        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        //Debug.Log(currentSpeed);

        animator.SetBool("Sneak", sneak);
        
        animator.SetFloat("Speed", currentSpeed);

        shouting = Input.GetButton("Attract");

        animator.SetBool("Shouting", shouting);

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Running") || animator.GetCurrentAnimatorStateInfo(1).IsName("Shouting"))
        {
            gameController.playerShoutRun = true;
        }
        else
        {
            gameController.playerShoutRun = false;
        }
    }


}
