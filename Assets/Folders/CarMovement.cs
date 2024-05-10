using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarMovement : MonoBehaviour
{
    Rigidbody2D body;

    float steeringInput;
    float maxSpeed;
    float acceleration;
    float rotationAngle;
    float VelocityUp;
    float driftTimer;
    float horizontalInput;

    bool canDrive = true;
    bool offRoad;
    bool isDrifting;
    bool driftActivated;
    [SerializeField] float accellerationPower;
    [SerializeField] float steeringPower;
    [SerializeField] float boostForce;
    [SerializeField] float driftPower;
    [SerializeField] float maxSpeedOnRoad;
    [SerializeField] float maxSpeedOffRoad;
    [SerializeField] float maxSpeedWhileDrifting;
    [SerializeField] float maxSpeedWithBoost;
    [SerializeField] ParticleSystem driftParticles;
    [SerializeField] TextMeshProUGUI Controls;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        driftParticles.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        acceleration = Input.GetAxis("Vertical");
        Controls.text = "Drift Activated: " + driftActivated;
        if (offRoad)
        {
            AdjustForOffroad();
        }
        else maxSpeed = maxSpeedOnRoad;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDrifting = true;
            maxSpeed = maxSpeedWhileDrifting;
            
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            isDrifting = false;
            if (offRoad)
            {
                maxSpeed = maxSpeedOffRoad;
            }
            else
            {
                maxSpeed = maxSpeedOnRoad;
            }
        }
    }

     void FixedUpdate()
    {
        if (canDrive)
        {
            ApplyStearing();

            ApplyEngineForce();

            KillOrthoginalVelocity();

            if (VelocityUp > maxSpeed)
            {
                SlowDownToMaxSpeed();
            }
            if (isDrifting && !driftActivated && steeringInput != 0)
            {
                ActivateDrift();
            }
            else if (!isDrifting && driftActivated || steeringInput == 0)
            {
                DeactivateDrift();
            }
            if (driftActivated)
            {
                if (driftTimer > 5f)
                {
                    driftParticles.startColor = Color.red;
                }
                else driftParticles.startColor = Color.yellow;
                driftTimer += 0.1f;
            }

            else
            {
                if(driftTimer > 5f)
                {
                    ApplySpeedBoost();
                }
                driftTimer = 0;
            }
        }
        else {  body.velocity = new Vector2 (0, 0);}
        

    }
    

    public void ApplySpeedBoost()
    {
        body.AddForce(transform.up * boostForce, ForceMode2D.Impulse);
    }

    private void ActivateDrift()
    {
        driftPower = 0.9f;
        steeringPower = 2.5f;
        driftActivated = true;  
        driftParticles.gameObject.SetActive(true);
    }
    void DeactivateDrift()
    {
        driftPower = 0.5f;
        steeringPower = 1.5f;
        driftActivated = false;
        driftParticles.gameObject.SetActive(false);
    }
    private void AdjustForOffroad()
    {
        maxSpeed = maxSpeedOffRoad;
    }

    void ApplyStearing()
    {
        if (horizontalInput < 0 && steeringInput < 0 && ((acceleration <= 0 && VelocityUp > 0) || (acceleration >= 0) && VelocityUp < 0))
        {
            steeringInput = Mathf.Lerp(steeringInput, -1, Time.fixedDeltaTime);
        }
        else if (horizontalInput > 0 &&steeringInput < 0 && ((acceleration <= 0 && VelocityUp > 0 ) || acceleration>= 0 && VelocityUp < 0))
        {
            steeringInput = Mathf.Lerp(steeringInput, 1 , Time.fixedDeltaTime);
        }
        else { steeringInput = horizontalInput; }

        float minSpeedForTurn = (body.velocity.magnitude) / 8;

        minSpeedForTurn = Mathf.Clamp01(minSpeedForTurn);

        if(acceleration <= 0 && VelocityUp > 0 || acceleration >= 0 && VelocityUp < 0) {
            steeringPower = Mathf.Lerp(steeringPower, 0.0f, Time.fixedDeltaTime * 2);
            
        }
        else if (driftActivated)
        {
            steeringPower = 2.5f;
        }
        else { steeringPower = 1.5f; }

        if(VelocityUp <= 0)
        {
            rotationAngle += steeringInput * steeringPower *minSpeedForTurn;
        }
        else { rotationAngle -= steeringInput * steeringPower * minSpeedForTurn;}

        body.rotation = rotationAngle;
    }

    void ApplyEngineForce()
    {
        VelocityUp = Vector2.Dot(transform.up, body.velocity);

        if(VelocityUp >= maxSpeed && acceleration > 0)
        {
            return;
        }
        if(VelocityUp <= -maxSpeed * 0.5f && acceleration < 0) { return; }

        if (acceleration == 0 || (acceleration < 0 && VelocityUp > 0)) {
            body.drag = Mathf.Lerp(body.drag, 5.0f, Time.fixedDeltaTime * 3);
        }
        else { body.drag = 0;

            Vector2 engineForce = transform.up * acceleration * accellerationPower;

            body.AddForce(engineForce, ForceMode2D.Force);
        }
    }

    void KillOrthoginalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(body.velocity, transform.up);
        Vector2 rightVelocity = transform.right *Vector2.Dot(body.velocity, transform.right);

        body.velocity = forwardVelocity + rightVelocity * driftPower;
    }

    void SlowDownToMaxSpeed()
    {
        Vector2 slowDownForce = -transform.up * accellerationPower * 2;

        body.AddForce(slowDownForce, ForceMode2D.Force);
    }
}
