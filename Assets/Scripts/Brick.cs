using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Brick : MonoBehaviour
{
    private SpriteRenderer sr;
    private BoxCollider2D boxCollider;

    public int Hitpoints = 1;
    public ParticleSystem DestroyEffect;
    public static event Action<Brick> OnBrickDestruction;


    private void Awake()
    {
        sr = this.GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        Ball.OnLightningBallEnable += OnLightningBallEnable;
        Ball.OnLightningBallDisable += OnLightningBallDisable;

    }

    private void OnLightningBallDisable(Ball ball)
    {
        if (this != null)
        {
            this.boxCollider.isTrigger = false;
        }
    }

    private void OnLightningBallEnable(Ball ball)
    {
        if(this != null)
        {
            this.boxCollider.isTrigger = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();

        ApplyCollisionLogic(ball);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        ApplyCollisionLogic(ball);
    }

    private void ApplyCollisionLogic(Ball ball)
    {
        Hitpoints--;
        if (Hitpoints <= 0 || (ball != null && ball.isLightningBall))
        {
            BricksManager.Instance.RemainingBricks.Remove(this);
            OnBrickDestruction?.Invoke(this);
            OnBrickDestroy();
            SpawnDestroyEffect();
            Destroy(this.gameObject);
        }
        else
        {
            //change sprite
            sr.sprite = BricksManager.Instance.brickSprites[Hitpoints - 1];
        }
    }

    private void OnBrickDestroy()
    {
        float buffSpawnChance = UnityEngine.Random.Range(0, 100f);
        float debuffSpawnChance = UnityEngine.Random.Range(0, 100f);

        bool alreadySpawned = false;

        if(buffSpawnChance <= CollectableManager.Instance.BuffChance)
        {
            alreadySpawned = true;
            Collectable newBuff = this.SpawnCollectable(true);
        }

        if(debuffSpawnChance <= CollectableManager.Instance.DebuffChance && !alreadySpawned)
        {
            Collectable newDebuff = this.SpawnCollectable(false); 
        }
    }

    private Collectable SpawnCollectable(bool isBuff)
    {
        List<Collectable> collection;
        if (isBuff)
        {
            collection = CollectableManager.Instance.AvailableBuffs;
        }
        else
        {
            collection = CollectableManager.Instance.AvailableDebuffs;
        }

        int buffIndex = UnityEngine.Random.Range(0, collection.Count);
        Collectable prefabCollectable = collection[buffIndex];
        Collectable newCollectable = Instantiate(prefabCollectable, this.transform.position, Quaternion.identity) as Collectable;

        return newCollectable;
    }

    private void SpawnDestroyEffect()
    {
        Vector3 brickPos = gameObject.transform.position;
        Vector3 spawnPos = new Vector3(brickPos.x, brickPos.y, brickPos.z - 0.2f);

        GameObject effect = Instantiate(DestroyEffect.gameObject, spawnPos, Quaternion.identity);

        MainModule mn = effect.GetComponent<ParticleSystem>().main;
        mn.startColor = this.sr.color;
        Destroy(effect, DestroyEffect.main.startLifetime.constant);
    }

    public void Init(Transform containerTransform, Sprite sprite, Color color, int hitpoint)
    {
        transform.SetParent(containerTransform);
        sr.sprite = sprite;
        sr.color = color;
        Hitpoints = hitpoint;
    }

    private void OnDisable()
    {
        Ball.OnLightningBallEnable -= OnLightningBallEnable;
        Ball.OnLightningBallDisable -= OnLightningBallDisable;
    }
}
