using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{

    [HideInInspector]
    public int playerID;
    public float speed;
    private int stuns; //C'est un int comme ça on peut se faire stun plusieur fois
    public int maxLife;
    public int currentLife;
    private bool parryOn;

    public Animator[] animators_lifeSlots;
    public Animator[] animators_lifePulses;

    public enum State {
        ILDE,
        LEFT,
        RIGHT,
        ATK,
        DEF,
    }
    public State state;

    public enum Player {
        PLAYER_ONE,
        PLAYER_TWO
    }
    public Player player;

    private KeyCode inputRight;
    private KeyCode inputLeft;
    private KeyCode inputAtk;
    private KeyCode inputDef;
    
    
    private Animator[] animatorList;
    private Animator animator;
    private Animator animatorFX;

    void Start()
    {
        animatorList = GetComponentsInChildren<Animator>();
        animator = animatorList[0];
        animatorFX = animatorList[1];

        currentLife = maxLife;
        PlayerSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if(stuns == 0)
        {
            InputDetection();
            PerformeMovement();
        }
    }

    //C'est ici qu'on change de State
    public void InputDetection()
    {
        if (Input.GetKey(inputRight))
        {
            state = State.RIGHT;
        }

        if (Input.GetKeyUp(inputRight))
        {
            state = State.ILDE;
        }

        if (Input.GetKey(inputLeft))
        {
            state = State.LEFT;
        }

        if (Input.GetKeyUp(inputLeft))
        {
            state = State.ILDE;
        }

        //ATK
        if (Input.GetKeyDown(inputAtk))
        {
            state = State.ATK;
        }

        //DEF
        if (Input.GetKeyDown(inputDef))
        {
            state = State.DEF;
        }
    }

    public void PerformeMovement()
    {
        switch (state)
        {
            case State.ILDE:
                animator.SetBool("isMoving", false);
                break;

            case State.LEFT:
                animator.SetBool("isMoving", true);
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                break;

            case State.RIGHT:
                animator.SetBool("isMoving", true);
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                break;

            case State.ATK:
                StartCoroutine(Attack());
                break;

            case State.DEF:
                StartCoroutine(Defense());
                break;

            default:
                break;
        }
    }

    public void PlayerSetUp()
    {
        if(player == Player.PLAYER_ONE)
        {
            inputRight = KeyCode.D;
            inputLeft = KeyCode.Q;
            inputAtk = KeyCode.B;
            inputDef = KeyCode.N;
        }

        if (player == Player.PLAYER_TWO)
        {
            inputRight = KeyCode.RightArrow;
            inputLeft = KeyCode.LeftArrow;
            inputAtk = KeyCode.Keypad1;
            inputDef = KeyCode.Keypad2;
        }
    }

    public IEnumerator Attack()
    {
        stuns++;
        animator.SetTrigger("atk");
        yield return new WaitForSeconds(1f);
        state = State.ILDE;
        stuns--;
    }

    public IEnumerator Defense()
    {
        stuns++;
        animator.SetTrigger("def");
        yield return new WaitForSeconds(0.30f);
        parryOn = true;
        yield return new WaitForSeconds(0.60f);
        parryOn = false;
        state = State.ILDE;
        stuns--;
    }

    public IEnumerator Hit(){
        stuns++;
        DamageLifeUI();
        if (SetLife(-1) == true){
            KO();
        }
        else
        {
            animator.SetTrigger("hit");
        }
        yield return new WaitForSeconds(0.5f); // Pas exactement la bonne valeur
        stuns--;
    }

    public void KO()
    {
        stuns++;
        animator.SetTrigger("ko");
        GameManager.instance.WhoWin(playerID);
    }

    public IEnumerator Countered()
    {
        stuns++;
        animator.SetTrigger("counter");
        animatorFX.SetTrigger("hitStun");
        yield return new WaitForSeconds(1.5f);
        stuns--;
    }

    public bool SetLife(int damage)
    {
        currentLife += damage;
        bool ko = false;
        if(currentLife <= 0){
            currentLife = 0;
            ko = true;
        }

        if(currentLife > maxLife){
            currentLife = maxLife;
        }
        return ko;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("hitBox"))
        {
            if(this.parryOn)
            {
                Character ennemy = col.GetComponentInParent<Character>();
                Debug.Log("parry succses");
                animatorFX.SetTrigger("parry");
                StartCoroutine(ennemy.Countered());
            }
            else
            {
                  StartCoroutine(Hit());
            }           
        }

        if(col.CompareTag("pnjFlower"))
        {
            GameManager.instance.AnimFlower();
        }
    }

    public void Win()
    {
        stuns++;
        animator.SetTrigger("win");
    }

    public void DamageLifeUI()
    {
        animators_lifePulses[currentLife-1].SetTrigger("hit");
        animators_lifeSlots[currentLife-1].SetTrigger("hit");
    }


}
