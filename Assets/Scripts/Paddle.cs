using System;
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

    private float paddleInitialY;
    private float defaultPaddleWidthInPixel = 200;
    private float defaultLeftClamp = 135;
    private float defaultRightClamp = 410;

    private void Start()
    {
        mainCamera = Camera.main;
        paddleInitialY = this.transform.position.y;
        sr = GetComponent<SpriteRenderer>();

    }
    private void Update()
    {
        PaddleMovement();
    }

    private void PaddleMovement()
    {
        float paddleShift = (defaultPaddleWidthInPixel - ((defaultPaddleWidthInPixel / 2) * this.sr.size.x)) / 2;

        float leftClamp = defaultLeftClamp - paddleShift;
        float rightClamp = defaultRightClamp + paddleShift;

        float mousePositionPixels = Mathf.Clamp(Input.mousePosition.x, leftClamp, rightClamp);
        float mousePositionWorldX = mainCamera.ScreenToWorldPoint(new Vector3(mousePositionPixels, 0, 0)).x;

        this.transform.position = new Vector3(mousePositionWorldX, paddleInitialY, 0);

        //Debug.Log("Mouse pixel position: " + Input.mousePosition);
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
}
