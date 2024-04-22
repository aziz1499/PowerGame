using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private static float movementCooldown;//cooldown for movement after passing a door
    [SerializeField] private float movementSpeed = 6;
    private float speedX, speedY;
    private Rigidbody2D rb;

    //torch movement
    [SerializeField] private GameObject torch;
    [SerializeField] private GameObject torchLight;
    [SerializeField] private Image batteryMask;
    [SerializeField] private Image batteryIcon;
    [SerializeField] private Image batteryMask2;
    [SerializeField] private Image batteryIcon2;
    [SerializeField] private Image batteryMask3;
    [SerializeField] private Image batteryIcon3;
    [SerializeField] private Image batteryMask4;
    [SerializeField] private Image batteryIcon4;
    [SerializeField] private Image batteryMask5;
    [SerializeField] private Image batteryIcon5;
    private float batteryMax = 100;
    private float batteryPercent = 100;
    private float batteryDecayRate = 50;
    private float batteryChargeRate = 35;

    //hp
    [SerializeField] private Text healthText;
    private float healthMax = 5;
    private float healthCurrent = 5;
    private bool isImmune = false;
    private float immuneCooldown;
    [SerializeField] private EyeMovement eyeMov;//used for invincibility animation

    //money
    [SerializeField] private Text moneyText;
    private float moneyCurrent = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthCurrent = healthMax;
        batteryPercent = batteryMax;
        moneyCurrent = 0;
        healthText.text = " " + healthCurrent;
        moneyText.text = " " + moneyCurrent;
        batteryMask.GetComponent<RectTransform>().localPosition = new Vector3(-8, 0, 0);
        batteryIcon.GetComponent<RectTransform>().localPosition = new Vector3(8, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //if player not crossing rooms
        if(movementCooldown <= 0)
        {
            //player movement
            speedX = Input.GetAxisRaw("Horizontal");
            speedY = Input.GetAxisRaw("Vertical");
            rb.velocity = movementSpeed * new Vector2(speedX, speedY).normalized;
            //torch controls
            if(Input.GetKey(KeyCode.Mouse0))
            {
                batteryPercent = batteryPercent - (batteryDecayRate * Time.deltaTime);
                if(batteryPercent < 0)
                {
                    batteryPercent = 0;
                    torchLight.SetActive(false);
                }
                else
                {
                    torchLight.SetActive(true);
                }
            }
            else
            {
                torchLight.SetActive(false);
                batteryPercent = batteryPercent + (batteryChargeRate * Time.deltaTime);
                if(batteryPercent > batteryMax)
                {
                    batteryPercent = batteryMax;
                }
            }
        }
        else
        {
            torchLight.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        //put movement on cooldown after a door used
        if(movementCooldown > 0)
        {
            movementCooldown = movementCooldown - Time.fixedDeltaTime;
            rb.velocity = new Vector3(0,0,0);
        }
        //torch point at mouse
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
        torch.transform.right = mouseWorldPosition - transform.position;
        //battery visuals
        batteryMask.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110))) + (batteryMask.GetComponent<RectTransform>().rect.size.x/2f), (batteryMask.GetComponent<RectTransform>().rect.size.y/2f), 0);
        batteryIcon.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Abs(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110)))), 0, 0);
        batteryMask2.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110 - 102))) + (batteryMask2.GetComponent<RectTransform>().rect.size.x/2f), (batteryMask2.GetComponent<RectTransform>().rect.size.y/2f), 0);
        batteryIcon2.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Abs(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110 - 102)))), 0, 0);
        batteryMask3.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110 - 102 - 102))) + (batteryMask3.GetComponent<RectTransform>().rect.size.x/2f), (batteryMask3.GetComponent<RectTransform>().rect.size.y/2f), 0);
        batteryIcon3.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Abs(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110 - 102 - 102)))), 0, 0);
        batteryMask4.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110 - 102 - 102 - 102))) + (batteryMask3.GetComponent<RectTransform>().rect.size.x/2f), (batteryMask3.GetComponent<RectTransform>().rect.size.y/2f), 0);
        batteryIcon4.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Abs(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110 - 102 - 102 - 102)))), 0, 0);
        batteryMask5.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110 - 102 - 102 - 102 - 102))) + (batteryMask3.GetComponent<RectTransform>().rect.size.x/2f), (batteryMask3.GetComponent<RectTransform>().rect.size.y/2f), 0);
        batteryIcon5.GetComponent<RectTransform>().localPosition = new Vector3(Mathf.Abs(Mathf.Max(-110, Mathf.Min(-8, ((1.02f * batteryPercent) - 110 - 102 - 102 - 102 - 102)))), 0, 0);
        //immunity cooldown
        immuneCooldown = immuneCooldown - Time.fixedDeltaTime;
        if(immuneCooldown <= 0)
        {
            isImmune = false;
            torch.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(torch.transform.GetChild(0).GetComponent<SpriteRenderer>().color.r, torch.transform.GetChild(0).GetComponent<SpriteRenderer>().color.g, torch.transform.GetChild(0).GetComponent<SpriteRenderer>().color.b, 1);            
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1);
            eyeMov.InvinceSet(false);        
        }
        //do immunity animation
        else
        {
            torch.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(torch.transform.GetChild(0).GetComponent<SpriteRenderer>().color.r, torch.transform.GetChild(0).GetComponent<SpriteRenderer>().color.g, torch.transform.GetChild(0).GetComponent<SpriteRenderer>().color.b, 0.2f + 0.8f * Mathf.Sin(Time.timeSinceLevelLoad * 32));
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0.2f + 0.8f * Mathf.Sin(Time.timeSinceLevelLoad * 32));
        }
    }

    //when loading new room, time to wait before enabling movement
    public void SetMovementCooldown(float tim)
    {
        movementCooldown = tim;
    }


    public void TakeDamage()
    {
        if(!isImmune)
        {
            healthCurrent = healthCurrent - 1;
            healthText.text = " " + healthCurrent;
            if(healthCurrent <= 0)
            {
                Debug.Log("Death");//TODO add proper death
            }
            isImmune = true;
            eyeMov.InvinceSet(true);
            immuneCooldown = 2;
        }
    }

    public void GainHP(int amt)
    {
        healthCurrent = healthCurrent + amt;
        healthText.text = " " + healthCurrent;
    }

    public void GainBattery()
    {
        batteryMax = batteryMax + 25;
    }

    public void GainGold(int amt)
    {
        moneyCurrent = moneyCurrent + amt;
        moneyText.text = " " + moneyCurrent;
    }

    public bool CanLoseGold(int amt)
    {
        return(amt >= moneyCurrent);
    }

    public void LoseGold(int amt)
    {
        moneyCurrent = moneyCurrent - amt;
        moneyText.text = " " + moneyCurrent;
    }
}
