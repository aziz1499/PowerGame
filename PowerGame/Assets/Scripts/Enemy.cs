using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 10;
    protected float currentHealth;
    [SerializeField] private GameObject healthDisplay;
    protected int toBurn = 0;//change to int to count how many torches touching
    protected bool toHeal = false;
    protected bool toDamagePlayer = false;
    private PlayerMovement playerX;
    [SerializeField] protected RoomLayoutDetails roomImIn;
    private bool startedMoving = false;
    [SerializeField] protected GameObject loot;
    [SerializeField] protected GameObject lightSelf;//the masked version of the character
    private bool smokeCopyMade = false;
    private bool dealsDamage = true;
    private bool deadNow = false;//used to avoid running death functions more than once
    [SerializeField] private AudioSource smokeSound;


    //  : Graphe de patrouille locale pour l'ennemi // LIGNE AJOUT�E
    private class PatrolNode // LIGNE AJOUT�E
    {
        public Vector3 position;
        public List<PatrolNode> neighbors;

        public PatrolNode(Vector3 pos)
        {
            position = pos;
            neighbors = new List<PatrolNode>();
        }
    } // LIGNE AJOUT�E

    private PatrolNode currentPatrolNode; // LIGNE AJOUT�E
    private List<PatrolNode> patrolGraph = new List<PatrolNode>(); // LIGNE AJOUT�E
    [SerializeField] private float patrolSpeed = 2f; // LIGNE AJOUT�E




    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        if(roomImIn == null)
        {
            //TODO, if bug apears, check this
            //tries to assign current room layout to parent

            

            if (transform.parent.GetComponent<RoomLayoutDetails>() != null)
            {
                roomImIn = transform.parent.GetComponent<RoomLayoutDetails>();
            }
            else
            {
                roomImIn = FindObjectOfType<RoomLayoutDetails>();
            }
        }

        InitializePatrolGraph();// LIGNE AJOUT�E (appel de la structure pour le lancement de la patrouille)

        StartAddOn();
        Invoke("StartActivity", 1.25f);
    }



   


   



//enable movement after room loading
private void StartActivity()
    {
        startedMoving = true;
    }

    //Allow Inheritors to add on to start by overriding this
    public virtual void StartAddOn()
    {

    }


    void FixedUpdate()
    {
        //recieve healing
        if(toHeal)
        {
            currentHealth = currentHealth + (1.0f * Time.fixedDeltaTime);
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            healthDisplay.transform.localScale = new Vector3(currentHealth/maxHealth, 1, 1);
            healthDisplay.transform.localPosition = new Vector3(-(1-(currentHealth/maxHealth))/2f, 0, 0);
        }
        //take damage
        if(toBurn >= 1)
        {
            currentHealth = currentHealth - (4.5f * Time.fixedDeltaTime);
            if(currentHealth < 0)
            {
                currentHealth = 0;
                if(!deadNow)
                {
                    deadNow = true;
                    //drop a coin and destroy enemy
                    if(loot != null)
                    {
                        loot.transform.parent = transform.parent;
                        loot.transform.localScale = new Vector3(Mathf.Abs(loot.transform.localScale.x), Mathf.Abs(loot.transform.localScale.y), Mathf.Abs(loot.transform.localScale.z));
                        loot.SetActive(true);
                    }
                    if(roomImIn != null)
                    {
                        roomImIn.Death();
                    }
                    //if has death animation, do it
                    if(GetComponent<Animator>() != null)
                    {
                        if((GetComponent<Animator>().HasState(0, Animator.StringToHash("Base Layer.SmokeAndDie"))) && (lightSelf.GetComponent<Animator>().HasState(0, Animator.StringToHash("Base Layer.Smoke"))))
                        {
                            GetComponent<SpriteRenderer>().color = new Color(0.3333333f, 0.3333333f, 0.3333333f, 1);
                            lightSelf.GetComponent<Animator>().Play("Base Layer.Smoke");
                            GetComponent<Animator>().Play("Base Layer.SmokeAndDie");
                            //play smoke sound
                            if(smokeSound != null)
                            {
                                //create a temporary object to play sound
                                GameObject audioHolder = new GameObject();
                                audioHolder.transform.position = transform.position;
                                audioHolder.transform.parent = null;
                                audioHolder.AddComponent<AudioSource>();
                                AudioSource audSou = audioHolder.GetComponent<AudioSource>();
                                audSou.clip = smokeSound.clip;
                                audSou.playOnAwake = false;
                                audSou.volume = 0.5f;
                                audSou.Play();
                                Destroy(audioHolder, 5);
                            }
                        }
                    }
                    else//otherwise create a copy of the death animation to use
                    {
                        //smoke for death animation of enemies that dont use an animator
                        //spare smoke backup is stored as first grandchild of palette
                        GameObject spareSmoke = FindObjectOfType<RoomPalette>().transform.GetChild(0).transform.GetChild(0).gameObject;
                        if(!smokeCopyMade)
                        {
                            smokeCopyMade = true;
                            GameObject deathSmoke = Instantiate(spareSmoke);
                            deathSmoke.transform.parent = transform;
                            deathSmoke.transform.localPosition = new Vector3(0, 0, -1);
                            deathSmoke.SetActive(true);
                            deathSmoke.transform.parent = null;
                            if(deathSmoke.GetComponent<Animator>() != null)
                            {
                                if((deathSmoke.GetComponent<Animator>().HasState(0, Animator.StringToHash("Base Layer.Smoke"))) && (deathSmoke.transform.GetChild(0).GetComponent<Animator>().HasState(0, Animator.StringToHash("Base Layer.Smoke"))))
                                {
                                    //play smoke sound
                                    if(smokeSound != null)
                                    {
                                        //create a temporary object to play sound
                                        GameObject audioHolder = new GameObject();
                                        audioHolder.transform.position = transform.position;
                                        audioHolder.transform.parent = null;
                                        audioHolder.AddComponent<AudioSource>();
                                        AudioSource audSou = audioHolder.GetComponent<AudioSource>();
                                        audSou.clip = smokeSound.clip;
                                        audSou.playOnAwake = false;
                                        audSou.volume = 0.5f;
                                        audSou.Play();
                                        Destroy(audioHolder, 5);
                                    }
                                    deathSmoke.GetComponent<Animator>().Play("Base Layer.Smoke");
                                    deathSmoke.transform.GetChild(0).GetComponent<Animator>().Play("Base Layer.Smoke");
                                    SelfDestruct();
                                }
                            }
                        }
                    }
                    startedMoving = false;
                    dealsDamage = false;
                    //SelfDestruct();
                }
            }
            healthDisplay.transform.parent.gameObject.SetActive(true);
            healthDisplay.SetActive(true);
            healthDisplay.transform.localScale = new Vector3(currentHealth/maxHealth, 1, 1);
            healthDisplay.transform.localPosition = new Vector3(-(1-(currentHealth/maxHealth))/2f, 0, 0);
        }
        //deal damage
        if(toDamagePlayer)
        {
            if(dealsDamage)
            {
                playerX.TakeDamage();
            }
        }
        //Allow Inheritors to add on to fixed update
        if(startedMoving)
        {
            FixedUpdateAddOn();
        }
    }

    public void SelfDestruct()
    {
        GameObject self = gameObject;
        Destroy(self);
    }

    //Allow Inheritors to add on to fixed update by overriding this
    public virtual void FixedUpdateAddOn()
    {

    }

    //set whether or not an enemy should take damage
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.name.Equals("TorchLight"))
        {
            toBurn++;
        }
        //if heal
        if(col.name.Equals("TorchAntiLight"))
        {
            toHeal = true;
        }
        //if hit player, deal damage
        if(col.GetComponent<PlayerMovement>() != null)
        {
            playerX = col.GetComponent<PlayerMovement>();
            toDamagePlayer = true;
        }

        //Allow Inheritors to add on
        if(startedMoving)
        {
            OnTriggerEnter2DAddOn(col);
        }
    }

    //Allow Inheritors to add on to OnTriggerEnter2D by overriding this
    public virtual void OnTriggerEnter2DAddOn(Collider2D col)
    {

    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.name.Equals("TorchLight"))
        {
            toBurn--;
        }
        if(col.name.Equals("TorchAntiLight"))
        {
            toHeal = false;
        }
        if(col.GetComponent<PlayerMovement>() != null)
        {
            toDamagePlayer = false;
        }
        
        //Allow Inheritors to add on
        if(startedMoving)
        {
            OnTriggerExit2DAddOn(col);
        }
    }

    //Allow Inheritors to add on to OnTriggerExit2D by overriding this
    public virtual void OnTriggerExit2DAddOn(Collider2D col)
    {

    }

    public float GetHealthPercent()
    {
        return(currentHealth/maxHealth);
    }
}
