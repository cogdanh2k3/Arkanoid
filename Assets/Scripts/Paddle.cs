using System;
using System.Collections;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    #region Singleton

    private static Paddle _instance;
    public static Paddle Instance => _instance; // property để bên ngoài truy cập vào, khi goị GameManager.Instance -> trả vè _instance

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    private Camera mainCamera;
    private SpriteRenderer sr;
    private BoxCollider2D boxCol;
    private Rigidbody2D paddleRb;

    public GameObject leftMuzzle;
    public GameObject rightMuzzle;
    public Projectile bulletPrefab;
    public Collider2D leftWall;
    public Collider2D rightWall;



    private float paddleInitialY;

    public float extendShrinkDuration = 10;
    public float paddleWidth = 2;
    public float paddleHeight = 0.28f;

    public bool PaddleIsTranforming { get; set; }
    public bool PaddleIsShooting { get; set; }


    private void Start()
    {
        mainCamera = Camera.main;
        paddleInitialY = this.transform.position.y;
        sr = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        paddleRb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        PaddleMovement();
        UpdateMuzzlePosition();
    }

    private void UpdateMuzzlePosition()
    {
        leftMuzzle.transform.position = new Vector3(this.transform.position.x - (this.sr.size.x / 2) + 0.1f, this.transform.position.y + 0.2f, this.transform.position.z);
        rightMuzzle.transform.position = new Vector3(this.transform.position.x + (this.sr.size.x / 2) - 0.153f, this.transform.position.y + 0.2f, this.transform.position.z);
    }

    public void StartWithAnimation(float newWidth)
    {
        StartCoroutine(AnimatePaddleWidth(newWidth));
    }

    public IEnumerator AnimatePaddleWidth(float width)
    {
        PaddleIsTranforming = true;
        StartCoroutine(ResetPaddleWidthAfterTime(extendShrinkDuration));

        if (width > sr.size.x)
        {
            float currentWidth = sr.size.x;
            while(currentWidth < width)
            {
                currentWidth += Time.deltaTime * 2;
                sr.size = new Vector2(currentWidth,paddleHeight);
                boxCol.size = new Vector2(currentWidth, paddleHeight);
                yield return null;
            }
        }
        else
        {
            float currentWidth = sr.size.x;
            while(currentWidth > width)
            {
                currentWidth -= Time.deltaTime * 2;
                sr.size = new Vector2(currentWidth, paddleHeight);
                boxCol.size = new Vector2(currentWidth, paddleHeight);
                yield return null;
            }
        }

        PaddleIsTranforming = false;
    }

    private IEnumerator ResetPaddleWidthAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.StartWithAnimation(this.paddleWidth);
    }

    private void PaddleMovement()
    {
        Vector2 mouseScreenPos = GameInput.Instance.GetMousePos();
        
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y));

        float leftLimit = leftWall.bounds.max.x + (sr.size.x / 2f);
        float rightLimit = rightWall.bounds.min.x - (sr.size.x / 2f);

        float clampedX = Mathf.Clamp(mouseWorldPos.x, leftLimit, rightLimit);

        Vector2 targetPos = new Vector2(clampedX, paddleInitialY);

        paddleRb.MovePosition(targetPos);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 paddleCenter = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y);

            ballRb.linearVelocity = Vector2.zero;

            float distancePoint = paddleCenter.x - hitPoint.x;

            if (hitPoint.x < paddleCenter.x)
            {
                ballRb.AddForce(new Vector2(-(Mathf.Abs(distancePoint * 200)), BallsManager.Instance.initialBallSpeed));
            }
            else
            {
                ballRb.AddForce(new Vector2(Mathf.Abs(distancePoint * 200), BallsManager.Instance.initialBallSpeed));
            }
        }
    }

    public void StartShooting()
    {
        if (!PaddleIsShooting)
        {
            PaddleIsShooting = true;
            StartCoroutine(StartShootingRoutine());
        }
    }

    public IEnumerator StartShootingRoutine()
    {
        float fireCooldown = .5f;
        float fireCooldownLeft = 0;

        float shootingDuration = 5f;
        float shootingDurationLeft = shootingDuration;

        //Debug.Log("START SHOOTING!");

        while (shootingDurationLeft > 0)
        {
            fireCooldownLeft -= Time.deltaTime;
            shootingDurationLeft -= Time.deltaTime;

            if(fireCooldownLeft <= 0)
            {
                this.Shoot();
                fireCooldownLeft = fireCooldown;
                //Debug.Log($"Shoot at {Time.time}");
            }

            yield return null;
        }

        //Debug.Log("STOP SHOOTING!");
        this.PaddleIsShooting = false;
        leftMuzzle.SetActive(false);
        rightMuzzle.SetActive(false);

    }

    private void Shoot()
    {
        leftMuzzle.SetActive(false);
        rightMuzzle.SetActive(false);


        leftMuzzle.SetActive(true);
        rightMuzzle.SetActive(true);

        //Spawn bullet
        SpawnBullet(leftMuzzle);
        SpawnBullet(rightMuzzle);


    }

    private void SpawnBullet(GameObject muzzle)
    {
        Vector3 spawnPosition = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y + 0.2f, muzzle.transform.position.z);
        Projectile bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(new Vector2(0, 450f));
    }
}
