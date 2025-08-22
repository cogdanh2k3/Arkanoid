public class LightningBall : Collectable
{
    protected override void ApplyEffect()
    {
        BallsManager.Instance.ActiveLightningBall(10f);
    }
}
