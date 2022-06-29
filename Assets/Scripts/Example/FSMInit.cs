using ChallengeAI;
public class FSMInit : FSMInitializer
{
    public override string Name => "No0bMaSt3r69";
    public override void Init()
    {
        RegisterState<ThinkState>(States.THINK);
        RegisterState<CaptureFlagState>(States.CAPTURE_FLAG);
        RegisterState<DeliverFlagState>(States.DELIVER_FLAG);
        RegisterState<HuntEnemyState>(States.HUNT_ENEMY);
        RegisterState<WalkState>(States.WALK);
        RegisterState<IdleState>(States.IDLE);
    }
}