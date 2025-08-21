using UnityEngine;

public class ExtendOrShrink : Collectable
{
    public float NewWidth = 2.5f;

    protected override void ApplyEffect()
    {
        if(Paddle.Instance != null && !Paddle.Instance.PaddleIsTranforming)
        {
            Paddle.Instance.StartWithAnimation(NewWidth);
        }
    }
}
