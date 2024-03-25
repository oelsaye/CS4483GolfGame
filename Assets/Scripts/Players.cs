using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class Players : MonoBehaviour
{
    public bool startOver = false;
    public bool increasePar = false;
    public bool cancelCharging = false;
    public bool maxCharge = false;
    public bool startCharge = false;

    public int attempts = 0;

    public bool completedLevel = false;

    public KeyCode chargeKey = KeyCode.Mouse0;
    public KeyCode switchModeKey = KeyCode.Mouse1;
    public KeyCode cancelChargeKey = KeyCode.E;

    private Rigidbody rigidB;
    [SerializeField] private Transform cameraObject;
    [SerializeField] public Transform sphere;

    [SerializeField] private GameObject playerRing;
    [SerializeField] private GameObject DashLight;
    [SerializeField] private GameObject DashArc;

    [SerializeField] private GameObject respawnObj;
    [SerializeField] private GameObject start;
    //public GameObject ArcSource;

    [SerializeField] private GameObject hat;
    [SerializeField] private TextMeshPro name3DText;
    [SerializeField] private AudioSource hitSound;

    private float speed = .5f;
    public bool lockCursor = true;

    private float SAND_PIT = 1f;

    private bool jumped = false;
    private bool inAir = false;
    private int jumpCounter = 0;

    public bool charging = false;
    private float dashForce = 1f;
    private Vector3 lastVelocity;
    private float velocityLimit = 1f;
    private bool dashed = false;
    public bool cancelDash = false;
    private Vector3 rotateVector = Vector3.zero;
    private float rotationAngle = 1;
    private bool flyDash = false;
    private Vector3 lastDirection;
    public float dashMultiply = 1f;
    private float sandPitDebuff = 1;

    public string shootMode = "Putt";
    public string[] allModes = { "Putt", "Launch" };
    public int modeIndex = 0;

    private Vector3 rightVector = Vector3.zero;


    [SerializeField] private cameraMove myCamera;
    private float holdZoom = 0f;

    [SerializeField] private GameObject spawnPoint;
    Vector3 spawnPointPosition;

    [SerializeField] private GameObject icon;

    public bool lockedCursor = false;
    public bool notMovable = false;
    public bool forceUnlock = false;
    
    void Start()
    {
        
        rigidB = GetComponent<Rigidbody>();
        speed = .5f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        lockedCursor = true;
        velocityLimit = 1f;
        spawnPointPosition = spawnPoint.transform.position;
        sandPitDebuff = 1f;
    }

    void Update()
    {

        Physics.IgnoreLayerCollision(6, 6);

        if (Input.GetKey(KeyCode.Q))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            lockCursor = false;
            lockedCursor = false;
            forceUnlock = true;
        }
        if (Cursor.visible == true && Input.GetKey(KeyCode.R))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            lockCursor = true;
            lockedCursor = true;
            forceUnlock = false;
        }

        lastVelocity = rigidB.velocity;

        Vector3 dashLightPosition = new Vector3(sphere.transform.position.x - 0.01999183f, sphere.transform.position.y - 0.016f, sphere.transform.position.z - 0.03000187f);
        DashLight.transform.position = dashLightPosition;

        playerRing.transform.position = new Vector3(sphere.transform.position.x, sphere.transform.position.y - 0.033f, sphere.transform.position.z);

        icon.transform.position = new Vector3(sphere.transform.position.x, sphere.transform.position.y + 0.356f, sphere.transform.position.z);

        float velLimitY = 0.55f;
        if (lastVelocity.y <= velLimitY && lastVelocity.y >= -velLimitY && inAir == false)
        {
            jumped = false;
        }

        if (inAir == false)
        {
            jumpCounter = jumpCounter + 1;
        }

        if (dashForce <= 1)
        {
            holdZoom = myCamera.zoomDistance;
        }

        Move();

        if (lastVelocity.x <= velocityLimit && lastVelocity.z >= -velocityLimit && lastVelocity.x >= -velocityLimit && lastVelocity.z <= velocityLimit)
        {
            dashed = false;
        }

        if (Input.GetKeyUp(switchModeKey))
        {
            modeIndex = modeIndex + 1;
            if (modeIndex >= allModes.Length)
            {
                modeIndex = 0;
            }
            shootMode = allModes[modeIndex];
        }

    }

    public void setPlayerPosition(Vector3 playerPos)
    {
        gameObject.transform.position = playerPos;
    }

    private void Move()
    {
        Vector3 cameraPosition = cameraObject.position;
        Vector3 spherePosition = sphere.position;

        Vector3 lookVector = spherePosition - cameraPosition;

        Vector3 dashVector = lookVector;
        dashVector.y = .3f;

        lookVector.y = 0;

        dashVector = dashVector.normalized;
        lookVector = lookVector.normalized;


        rightVector = Vector3.Cross(lookVector, Vector3.up);

        DashLight.transform.rotation = Quaternion.LookRotation(lookVector);
        icon.transform.rotation = Quaternion.LookRotation(lookVector);


        if (inAir == true && flyDash == true && lastDirection != null)
        {
            //ROTATING IN THE AIR
            //sphere.transform.Rotate(-lookVector);
            //sphere.transform.rotation = Quaternion.AngleAxis(-rotationAngle, rotateVector);
            rotationAngle = rotationAngle + 1;
            rigidB.GetComponent<ConstantForce>().torque = Vector3.Cross(new Vector3(lastDirection.x, 0, lastDirection.z), new Vector3(0, -1, 0));
        }
        else
        {
            rigidB.GetComponent<ConstantForce>().torque = new Vector3(0, 0, 0);
        }

        if (lastVelocity.x <= velocityLimit && lastVelocity.z >= -velocityLimit && lastVelocity.x >= -velocityLimit && lastVelocity.z <= velocityLimit
            && jumped == false && dashed == false && inAir == false && lockedCursor == true && notMovable == false)
        {
            if (playerRing.activeInHierarchy == true)
            {
                if (cancelDash == true)
                {
                    rigidB.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY;
                }
                else
                {
                    rigidB.constraints = RigidbodyConstraints.None;
                }
                Dash(dashVector, lookVector);
            }
        }


        if (lastVelocity.x <= 0.01 && lastVelocity.z >= -0.01 && lastVelocity.x >= -0.01 && lastVelocity.z <= 0.01 && inAir == false)
        {
            playerRing.SetActive(true);
            rigidB.velocity = Vector3.zero;
            rigidB.angularVelocity = Vector3.zero;
        }
        else
        {
            playerRing.SetActive(false);
        }

        if (charging == false && lastVelocity.x <= 0.01 && lastVelocity.z >= -0.01 && lastVelocity.x >= -0.01 && lastVelocity.z <= 0.01 && inAir == false)
        {
            
            if (playerRing.activeSelf == true)
            {
                icon.SetActive(true);
            }
        }
        else
        {
            icon.SetActive(false);
        }
    }

    [SerializeField] private GameObject chargeBar;
    private float chargeUIAmount = 1f;

    private void Dash(Vector3 dashVector, Vector3 lookVector)
    {
        float dashMultiplier = 1.5f;
        if (Input.GetKeyDown(chargeKey))
        {
            cancelDash = false;
            if (dashForce <= 1)
            {
                startCharge = true;
                dashMultiply = 1f;
            }
        }

        if (Input.GetKey(chargeKey) && cancelDash == false)
        {
            if (charging == false)
            {
                if (dashForce <= 1)
                {
                    startCharge = true;
                    dashMultiply = 1f;
                }
            }
            cancelDash = false;
            chargeBar.gameObject.transform.Find("bar").gameObject.SetActive(true);
            charging = true;
            rotateVector = dashVector;
            playerRing.SetActive(true);
            dashForce = 1.7f * 260 * Time.deltaTime * dashMultiply + dashForce;
            DashLight.transform.localScale = new Vector3(1.990944f, dashForce / 200, 1.289973f);
            rigidB.velocity = Vector3.zero;
            rigidB.angularVelocity = Vector3.zero;
            myCamera.zoomDistance = 1;
            myCamera.zoomMax = 1;
            cancelCharging = true;
            if (Input.GetKeyDown(cancelChargeKey) || notMovable == true)
            {
                cancelCharging = false;
                cancelDash = true;
            }
        }


        chargeUIAmount = dashForce / 1300;
        chargeUIAmount = (float)(266.5998 * chargeUIAmount);
        chargeBar.gameObject.GetComponent<RectTransform>().localScale = new Vector3(chargeUIAmount, 1f, 1f);
        chargeBar.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-43.1f - chargeUIAmount,
            chargeBar.gameObject.GetComponent<RectTransform>().localPosition.y, chargeBar.gameObject.GetComponent<RectTransform>().localPosition.z);

        if (dashForce >= 500)
        {
            dashMultiply = 1.3f;
        }

        if (dashForce >= 800)
        {
            dashMultiply = 1.4f;
        }

        if (dashForce >= 1000)
        {
            dashMultiply = 1.5f;
        }

        if (dashForce >= 1300)
        {
            dashMultiply = 1.7f;
            dashForce = 1300;
            maxCharge = true;
        }

        if (Input.GetKeyUp(chargeKey))
        {
            if (cancelDash == false)
            {
                spawnPointPosition = new Vector3(sphere.transform.position.x, sphere.transform.position.y, sphere.transform.position.z);
                rotateVector = rightVector;
                if (dashForce < 400)
                {
                    //dashVector.y = 0f;
                    //flyDash = false;
                    dashMultiplier = 2f;
                    flyDash = true;
                }
                else
                {
                    flyDash = true;
                    dashMultiplier = 2.75f;
                }
                if (shootMode == "Putt")
                {
                    PuttShot(dashVector, dashMultiplier);
                }
                else if (shootMode == "Launch")
                {
                    LaunchShot(dashVector, dashMultiplier);
                }
                //rigidB.AddForce((dashVector * (dashForce * dashMultiplier) * sandPitDebuff));
                dashed = true;
                myCamera.zoomMax = 2.7f;
                myCamera.zoomDistance = 2f;
                icon.SetActive(false);
                increasePar = true;
            }
            dashForce = 1f;
            cancelDash = false;
            charging = false;
            cancelCharging = false;
            startCharge = false;
            lastDirection = lookVector;
            DashLight.transform.localScale = new Vector3(0.02069181f, 1, 0.03625f);
            chargeBar.gameObject.transform.Find("bar").gameObject.SetActive(false);
        }

        if (cancelDash == true)
        {
            dashForce = 1f;
            myCamera.zoomMax = 2.7f;
            myCamera.zoomDistance = holdZoom;
            charging = false;
            icon.SetActive(true);
            startCharge = false;
            DashLight.transform.localScale = new Vector3(0.02069181f, 1, 0.03625f);
            chargeBar.gameObject.transform.Find("bar").gameObject.SetActive(false);
        }
    }

    private void PuttShot(Vector3 theDirection, float theMultiplier)
    {
        rigidB.constraints = RigidbodyConstraints.None;
        theDirection.y = 0f;
        theDirection.x = theDirection.x * 1.5f;
        theDirection.z = theDirection.z * 1.5f;
        flyDash = false;
        rigidB.AddForce((theDirection * (dashForce * theMultiplier) * sandPitDebuff));
        attempts = attempts + 1;
    }
    private void LaunchShot(Vector3 theDirection, float theMultiplier)
    {
        rigidB.constraints = RigidbodyConstraints.None;
        flyDash = true;
        theDirection.y = .3f;
        theDirection.x = theDirection.x * .6f;
        theDirection.z = theDirection.z * .6f;
        rigidB.AddForce((theDirection * (dashForce * theMultiplier) * sandPitDebuff));
        attempts = attempts + 1;
    }

    private void Respawn()
    {
        if (sphere.position.y <= -1)
        {
            spawnPoint.transform.position = spawnPointPosition;
            sphere.transform.position = spawnPoint.transform.position;
            rigidB.velocity = Vector3.zero;
        }
        rigidB.velocity = Vector3.zero;
        spawnPoint.transform.position = spawnPointPosition;
        sphere.transform.position = spawnPoint.transform.position;
        rigidB.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX
                    | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY;
    }

    private void inHole()
    {
        rigidB.velocity = Vector3.zero;
        //spawnPoint.transform.position = start.transform.position;
        //sphere.transform.position = start.transform.position;
        startOver = true;
        completedLevel = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Respawn")
        {
            Respawn();
            rigidB.velocity = Vector3.zero;
        }

        else if (collision.gameObject.tag == "Finish" && Input.GetKeyDown(KeyCode.T))
        {
            inHole();
            rigidB.velocity = Vector3.zero;
        }

        else if (collision.gameObject.tag == "Earth")
        {

        }

        else if (collision.gameObject.tag == "sandpit")
        {
            sandPitDebuff = SAND_PIT;
        }

        else
        {
            if (rigidB.velocity.y >= 1f || true)
            {
                float speeds = (float)(lastVelocity.magnitude / 1.3);
                float volume = Mathf.Clamp(speeds / 25, 0.3F, 1F);
                hitSound.volume = volume;
                Debug.Log(volume);
                hitSound.Play();
                var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
                rigidB.velocity = direction * Mathf.Max(speeds, 0f);
                speed = 50;
            }
            inAir = false;
            sandPitDebuff = 1;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Respawn")
        {
            Respawn();
            rigidB.velocity = Vector3.zero;
        }

        else if (collision.gameObject.tag == "Finish") // && Input.GetKeyDown(KeyCode.Q))
        {
            inHole();
            rigidB.velocity = Vector3.zero;
        }

        else
        {
            speed = 50f;
            inAir = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        speed = 50f;
        jumped = false;
        dashed = false;
        inAir = true;
    }
}
