using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;

public class PlayerController : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundedLayer;
    [SerializeField] private float speedF = 200;
    [SerializeField] private float speedUpF = 200;
    [SerializeField] private float jumpForce = 350;
    [SerializeField] private float hpPlayer = 350;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform kunaiPoint;
    [SerializeField] private Transform portalA;
    [SerializeField] private Transform portalB;
    [SerializeField] private GameObject attackArena;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isActtack = false;
    private bool isLadder = false;
    private bool isTele = false;
    private bool isHealer = false;

    private float horizontal;
    private float horizontalKey;
    private float speed;
    private float speedUp;
    private float timeCount;
    private float timeStartHealer = 0f;
    private float timeStartEternal = 0f;
    private float timer = 0f;

    private int coinTotal = 0;

    private Vector3 savePoint;

    internal bool isEternal = false;

    private void Awake()
    {
        coinTotal = PlayerPrefs.GetInt("coin");
    }

    // Update is called once per frame
    void Update()
    {
        if(isDeath)
        {
            return;
        }

        isGrounded = CheckGrounded();

        horizontalKey = Input.GetAxisRaw("Horizontal");

        if (isActtack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = speedF + (speedF / 2);
            speedUp = speedUpF + (speedUpF / 2);
        } else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = speedF;
            speedUp = speedUpF;
        }

        if(isGrounded)
        {
            if (isJumping)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                Attack();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                Throw();
            }
            else if (Mathf.Abs(horizontalKey) > 0.1f)
            {
                Run(horizontalKey);
            }
            else if (Mathf.Abs(horizontal) > 0.1f)
            {
                Run(horizontal);
            }
            else
            {
                ChangeAnim("is_idle");
                rb.velocity = Vector2.zero;
            }
            rb.gravityScale = 1f;
        }

        if(isTele)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if(Mathf.Round(Vector2.Distance(transform.position, portalA.position)) <= 1)
                {
                    transform.position = portalB.position;
                } else if (Mathf.Round(Vector2.Distance(transform.position, portalB.position)) <= 1)
                {
                    transform.position = portalA.position;
                }
            }
        }

        if (!isGrounded && rb.velocity.y < 0 && !Input.GetKey(KeyCode.S))
        {
            ChangeAnim("is_jumpOut");
            if (Mathf.Abs(horizontalKey) > 0.1f)
            {
                Moving(horizontalKey);
            }
            else if (Mathf.Abs(horizontal) > 0.1f)
            {
                Moving(horizontal);
            }
            isJumping = false;
        }

        if (isLadder)
        {
            if (Input.GetKey(KeyCode.W))
            {
                ChangeAnim("is_climb");
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(rb.velocity.x, speedUp * Time.fixedDeltaTime);
            }

            if (Input.GetKey(KeyCode.S) && !isGrounded)
            {
                ChangeAnim("is_climb");
                rb.gravityScale = 0.5f;
                rb.velocity = new Vector2(rb.velocity.x, -speedUp * Time.fixedDeltaTime);
            }

            if (Input.GetKeyUp(KeyCode.A) 
                || Input.GetKeyUp(KeyCode.W)
                || Input.GetKeyUp(KeyCode.S)
                || Input.GetKeyUp(KeyCode.D))
            {
                ChangeAnim("is_idle");
                rb.gravityScale = 0f;
                rb.velocity = new Vector2();
                transform.position = new Vector2(transform.position.x, transform.position.y);
            }

            if (Input.GetKey(KeyCode.A) && !isGrounded)
            {
                ChangeAnim("is_climb");
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(-speedUp * Time.fixedDeltaTime, rb.velocity.y);
            }

            if (Input.GetKey(KeyCode.D) && !isGrounded)
            {
                ChangeAnim("is_climb");
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(speedUp * Time.fixedDeltaTime, rb.velocity.y);
            }
        } else
        {
            rb.gravityScale = 1f;
        }

        if(isHealer)
        {
            timeCount = Time.time - timeStartHealer;
            if(OnWaited(1))
            {
                OnHealer(timeCount);
            }
        }

        if (isEternal && (Time.time - timeStartEternal) >= 10)
        {
            isEternal = false;
        }
    }

    public override void OnInit()
    {
        base.hp = hpPlayer;
        base.OnInit();
        UIManager.instance.SetPoint(0);
        ChangeAnim("is_idle");
        isActtack = false;
        DeActiveAttackArena();
        transform.position = savePoint;
        SavePoint();
        speed = speedF;
        speedUp = speedUpF;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    public void Attack()
    {
        if (CheckGrounded())
        {
            ChangeAnim("is_attack");
            isActtack = true;
            Invoke(nameof(ResetAttack), 0.5f);
            ActiveAttackArena();
            Invoke(nameof(DeActiveAttackArena), 0.5f);
        }
    }

    public void Throw()
    {
        if (CheckGrounded())
        {
            ChangeAnim("is_throw");
            isActtack = true;
            Invoke(nameof(ResetAttack), 0.5f);

            Instantiate(kunaiPrefab, kunaiPoint.position, kunaiPoint.rotation);
        }
    }

    public void Jump()
    {
        if (CheckGrounded())
        {
            isJumping = true;
            ChangeAnim("is_jumpin");
            rb.AddForce(jumpForce * Vector2.up);
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundedLayer);

        return hit.collider != null;
    }

    private void Run(float horizontalMoving)
    {
        ChangeAnim("is_run");
        Moving(horizontalMoving);
    }

    private void Moving(float horizontalMoving)
    {
        rb.velocity = new Vector2(horizontalMoving * Time.fixedDeltaTime * speed, rb.velocity.y);
        transform.rotation = Quaternion.Euler(new Vector3(0, horizontalMoving > 0 ? 0 : 180, 0));
    }

    private void ResetAttack()
    {
        isActtack = false;
        ChangeAnim("is_idle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            coinTotal++;
            PlayerPrefs.SetInt("coin", coinTotal);
            UIManager.instance.SetPoint(coinTotal);
            Destroy(collision.gameObject);
        }

        if(collision.tag == "DeathZone")
        {
            base.hp = 0;
            ChangeAnim("is_die");
            Invoke(nameof(OnInit), 1f);
        }

        if(collision.CompareTag("Rope"))
        {
            isLadder = true;
        }

        if (collision.CompareTag("PortalTele"))
        {
            isTele = true;
        }

        if (collision.CompareTag("Eternal"))
        {
            isEternal = true;
            timeStartEternal = Time.time;
        }

        if (collision.CompareTag("Healer"))
        {
            isHealer = true;
            timeStartHealer = Time.time;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            isLadder = false;
        }

        if (collision.CompareTag("PortalTele"))
        {
            isTele = false;
        }
    }

    private void ActiveAttackArena()
    {
        attackArena.SetActive(true);
    }

    private void DeActiveAttackArena()
    {
        attackArena.SetActive(false);
    }

    private void OnHealer(float timeCount)
    {
        if (timeCount <= 10f && base.hp < hpPlayer)
        {
            base.hp = base.hp + 10f;
            base.healthBar.OnDrawHealBar(base.hp, hpPlayer);
        } else {
            timeCount = 0f;
            timeStartHealer = 0f;
            isHealer = false;
        }
    }

    private bool OnWaited(float timeWait)
    {
        timer += Time.deltaTime;

        if (timer >= timeWait)
        {
            timer = 0;
            return true;
        }
        return false;
    }

    internal void SavePoint()
    {
        savePoint = transform.position;
    }
}
