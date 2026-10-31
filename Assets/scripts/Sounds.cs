using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Footsteps")]
    public AudioClip[] stepSounds;
    public float step1Time = 0.1f; 
    public float step2Time = 0.6f; 

    [Header("Attack")]
    public AudioClip attackSound;  
    public string attackStateName = "Attack"; 

    private Animator anim;
    private bool hasPlayedStep1;
    private bool hasPlayedStep2;
    private bool hasPlayedAttackSound;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
       
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

       
        if (stateInfo.IsName(attackStateName))
        {
            if (!hasPlayedAttackSound)
            {
                PlayAttackSound();
                hasPlayedAttackSound = true;
            }
        }
        else
        {
            
            hasPlayedAttackSound = false;
        }

       
        float currentSpeed = anim.GetFloat("Speed");

        if (currentSpeed < 0.1f)
        {
            hasPlayedStep1 = false;
            hasPlayedStep2 = false;
            return; 
        }

        
        float normalizedTime = stateInfo.normalizedTime % 1f;

      
        if (normalizedTime >= step1Time && normalizedTime < step1Time + 0.1f && !hasPlayedStep1)
        {
            PlayStep();
            hasPlayedStep1 = true;
        }
        else if (normalizedTime < step1Time || normalizedTime > step1Time + 0.1f)
        {
            hasPlayedStep1 = false;
        }

        
        if (normalizedTime >= step2Time && normalizedTime < step2Time + 0.1f && !hasPlayedStep2)
        {
            PlayStep();
            hasPlayedStep2 = true;
        }
        else if (normalizedTime < step2Time || normalizedTime > step2Time + 0.1f)
        {
            hasPlayedStep2 = false;
        }
    }

    void PlayStep()
    {
        if (audioSource == null || stepSounds.Length == 0) return;

        int randomIndex = Random.Range(0, stepSounds.Length);
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(stepSounds[randomIndex]);
    }

    void PlayAttackSound()
    {
        if (audioSource == null || attackSound == null) return;

     
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(attackSound);
    }
}