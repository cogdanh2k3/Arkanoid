using System;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isLightningBall;

    private SpriteRenderer sr;
    public ParticleSystem lightningBallEffect;
    public float lightningBallEffectDuration = 10f;

    public static event Action<Ball> OnBallDeath;
    public static event Action<Ball> OnLightningBallEnable;
    public static event Action<Ball> OnLightningBallDisable;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();   
    }

    public void Die()
    {
        OnBallDeath?.Invoke(this);
        Destroy(gameObject, 1);
    }

    public void StartLightningBall()
    {
        if (!this.isLightningBall)
        {
            isLightningBall = true;
            sr.enabled = false;

            lightningBallEffect.gameObject.SetActive(true);
            StartCoroutine(StopLightningBallAfterTime(lightningBallEffectDuration));

            OnLightningBallEnable?.Invoke(this);
        }
    }

    private IEnumerator StopLightningBallAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        StopLightningBall();
    }

    private void StopLightningBall()
    {
        if (isLightningBall)
        {
            this.isLightningBall = false;
            sr.enabled = true;
            lightningBallEffect.gameObject.SetActive(false);

            OnLightningBallDisable?.Invoke(this);
        }
    }
}
