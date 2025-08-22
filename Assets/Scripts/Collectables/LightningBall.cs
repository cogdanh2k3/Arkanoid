public class LightningBall : Collectable
{
    protected override void ApplyEffect()
    {
        foreach (Ball ball in BallsManager.Instance.Balls)
        {
            ball.StartLightningBall();
        }
    }
}
