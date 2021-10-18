using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRMScript : MonoBehaviour
{
    //Movement Vars
    CharacterController cc;
    [SerializeField] bool rawMovement; // on for raw movement else off for lerp movement
    [SerializeField] float movementSpeed; // 8 [SerializeField]
    [SerializeField] float movementAir; 
    [SerializeField] float jumpSpeed; // 3
    [SerializeField] float jumpCurve; // 3
    [SerializeField] float gravity; // 9
    [SerializeField] int outgoingDamage; // 20
    float finalJumpCalc;
    [SerializeField] bool isJumping;
    Vector3 velocity;

    [SerializeField] bool isAttackStart;



    //anims
    Animator playerAnimator;
    Vector2 input;
    public Vector3 rootMotion;

    //slopefix downforces
    float slopeForce; //0.1f best value
    [SerializeField] float slopeForceRayLength;
    [SerializeField] float slideDownSpeed;
    [SerializeField] float slideFriction; //0.3
    bool ccIsSlope;
    RaycastHit slopeHit;
    RaycastHit ccHit;

    //turning & cam refs
    float turnSmoothVelocity;
    [SerializeField] float turnSmoothTime = 0.2f;
    [SerializeField] Transform cameraRig;

    [SerializeField] GameObject sphereColl;

    [SerializeField] bool god;
    [SerializeField] int health;

    [SerializeField] HPBar hpBar;
    public void TakeDamageFromEnemy(int incDmg)
    {
        if (god)
        {

        }
        else
        {
            health -= incDmg;
            if (health <= 0)
            {
                this.enabled = false;
            }
        }
        if (hpBar != null)
        {
            hpBar.SetHealth(health);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (hpBar != null)
        {
            health = 200;
            hpBar.SetMaxHealth(health);
        }
        slopeForce = 0.1f;// best value rn dont change
        cc = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        finalJumpCalc = Mathf.Sqrt(2 * gravity * jumpSpeed);
    }

    private void OnAnimatorMove()
    {
        rootMotion += playerAnimator.deltaPosition;
    }

    void Update()
    {

        if (rawMovement)    //!!!DISABLE SNAP IN INPUT PROJ SETTINGS FOR BETTER TURNING WHEN IT COMES TO RM OR ***USE SNAP & DONT USE RAW FOR BETTER RESULTS***
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isAttackStart)
        {
            PlayerJump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isJumping)
        {
            if (!isAttackStart)
            {
                //playerAnimator.applyRootMotion = false;
                isAttackStart = true;
                playerAnimator.SetTrigger("isAttacking");
                rootMotion = Vector3.zero;
            }
        }

        float movementDir = Mathf.Clamp01(input.magnitude);
        movementDir = Mathf.Clamp(movementDir, 0, 1);
        playerAnimator.SetFloat("rmVelocity", movementDir);
    }

    void LateUpdate()//fixed update results in jerkiness for some reason with RMs
    {
        if (OnSteepSlope())
        {
            cc.Move(SteepSlopeSlide() + Vector3.down * slopeForce);
            rootMotion = Vector3.zero;
            isJumping = true;
            //playerAnimator.SetBool("isJumping", true);
        }
        else if (isJumping) //or also in air
        {
            AirUpdate();
        }
        else //isgrounded
        {
            GroundedUpdate();
        }

        RotationTransformCamera();
    }

    Collider[] hitColliders;
    void DebugLogAttack()
    {
        Debug.Log("height attack");
        //MIGHT USE ANOTHER TYPE OF COLLISION LOGIC HERE THIS IS PLACE HOLDER

        //damage
        //-5
        hitColliders = Physics.OverlapSphere(sphereColl.transform.position, sphereColl.transform.localScale.x/3);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Enemy")
            {
                Debug.Log("I just hit an enemey");
                //hitCollider.GetComponent<SimpleEnemy>().TakeDamageFromPlayer(outgoingDamage);
                hitCollider.GetComponent<EnemyProtoVersion>().TakeDamageFromPlayer(outgoingDamage);
            }
            if(hitCollider.tag == "Orb")
            {
                Debug.Log("Boss hit");
                hitCollider.GetComponent<RotateFast>().damageComingFromPlayer = outgoingDamage/2;
                hitCollider.GetComponent<RotateFast>().isHit = true;
            }
        }
    }

    void EndOfAttack()
    {
        isAttackStart = false;
    }

    private void GroundedUpdate()
    {

        Vector3 movementForward = rootMotion * movementSpeed;
        Vector3 downSlopeFix = Vector3.down * slopeForce;
        cc.Move(movementForward + downSlopeFix);
        rootMotion = Vector3.zero;

        if (!cc.isGrounded)
        {
            SetInAir(0);
        }

    }

    private void AirUpdate()
    {
        velocity.y -= gravity * Time.deltaTime;
        Vector3 displacement = velocity * Time.deltaTime;
        displacement += AirMovement();
        cc.Move(displacement);
        isJumping = !cc.isGrounded;
        rootMotion = Vector3.zero;
        playerAnimator.SetBool("isJumping", isJumping);
    }

    #region old slope fix
    /* in update 
    if (!ccIsSlope)
        {
            velocity.x += (1f - ccHit.normal.y) * ccHit.normal.x* (1f - slideFriction);
            velocity.z += (1f - ccHit.normal.y) * ccHit.normal.z* (1f - slideFriction);
            AirUpdate2();
}
        else if(isJumping) //or also in air
        {
            AirUpdate();
        }
        else //isgrounded
        {
            GroundedUpdate();
        }

        ccIsSlope = (Vector3.Angle(Vector3.up, ccHit.normal) <= cc.slopeLimit);

    /*
    private void AirUpdate2()
    {play with grav * # <---
        velocity.y -= ( 3 * gravity) * 2 * (Time.deltaTime);
        Vector3 displacement = velocity * Time.deltaTime;
        displacement += AirMovement();
        cc.Move(displacement);
        isJumping = !cc.isGrounded;
        rootMotion = Vector3.zero;
        playerAnimator.SetBool("isJumping", isJumping);
    }*/
    #endregion

    void PlayerJump()
    {
        if (!isJumping)
        {
            SetInAir(finalJumpCalc);
        }
    }

    void SetInAir(float jumpVelo)
    {
        isJumping = true;
        velocity = playerAnimator.velocity * jumpCurve * movementSpeed;
        velocity.y = jumpVelo;
        playerAnimator.SetBool("isJumping", true);
    }

    Vector3 AirMovement()
    {
        return ((cameraRig.transform.forward * input.y) + (cameraRig.transform.right * input.x)) * movementAir * Time.deltaTime;
    }

    bool OnSteepSlope()
    {
        if (!cc.isGrounded)
        {
            return false;
        }

        if (Physics.Raycast(transform.position + (transform.forward * 0.1f), Vector3.down, out slopeHit, (cc.height/2) + slopeForceRayLength))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            if(slopeAngle > cc.slopeLimit)
            {
                return true;
            }
        }
        return false;
    }

    Vector3 SteepSlopeSlide()
    {
        Vector3 slopeDir = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        float slideSpeed = slideDownSpeed + Time.deltaTime;

        Vector3 moveDir = slopeDir * -slideSpeed;
        moveDir.y = moveDir.y - slopeHit.point.y;
        return moveDir * Time.deltaTime;
    }

    bool oneRun;

    void RotationTransformCamera()
    {
        if (!isAttackStart)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;
            //rotation transform
            oneRun = false;
            if (inputDir != Vector2.zero)
            {
                float targetRot = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraRig.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, turnSmoothTime);
            }
             
        }
        else
        {
            if (!oneRun)
            {
                float targetRot = cameraRig.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref turnSmoothVelocity, 0);
                oneRun = true;
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ccHit.normal = hit.normal;
    }
}
