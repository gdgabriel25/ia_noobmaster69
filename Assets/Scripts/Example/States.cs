using UnityEngine;
using ChallengeAI;
public class States
{
    static public string THINK = "think";
    static public string CAPTURE_FLAG = "capture_flag";
    static public string DELIVER_FLAG = "deliver_flag";
    static public string HUNT_ENEMY = "hunt_enemy";
    static public string WALK = "walk";
    static public string IDLE = "idle";
    static public string[] StateNames = new string[]
    {
        THINK,
        CAPTURE_FLAG,
        DELIVER_FLAG,
        WALK,
        IDLE
    };
}
public class ThinkState : State
{
    public ThinkState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter()
    {
        //IA vai decidir o que fazer no Update
    }
    public override void Exit()
    {
        Log();
    }
    public override void Update(float deltaTime)
    {
        //IA vai decidir o que fazer
        if (!Agent.Data.HasFlag)
        {
            if (Agent.EnemyData[0].HasFlag)
            {
                if (Vector3.Distance(Agent.Data.Position, Agent.EnemyData[0].Position) > Vector3.Distance(Agent.Data.Position, (Vector3)Agent.EnemyData[0].FlagPosition))
                {
                    ChangeState("capture_flag");
                }else
                {
                    ChangeState("hunt_enemy");
                }
            }
            else
            {
                ChangeState("capture_flag");
            }
        }
        else
        {
            if (Agent.EnemyData[0].HasFlag)
            {
                ChangeState("hunt_enemy");
            }
            else
            {
                ChangeState("deliver_flag");
            }
        }
    }
}
public class IdleState : State
{
    public IdleState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    private float targetEnergy;
    public override void Enter()
    {
        Agent.Stop();
        targetEnergy = Agent.Data.Energy + 70f;
    }
    public override void Exit()
    {
        Log();
    }
    public override void Update(float deltaTime)
    {
        //Ações durante o estado
        if (!Agent.Data.IsCooldownFire && Agent.Data.Ammo > 0 && Agent.Data.HasSightEnemy)
        {
            Agent.Fire();
            ChangeState("think");
        }
        //Se já fez o que tinha que fazer
        if (Agent.Data.Energy >= targetEnergy)
        {
            ChangeState("think");
        }
        //Condições para tirar do estado
        if (Agent.Data.Energy > 80f)
        {
            ChangeState("think");
        }
        if (Agent.Data.RemainingDistance < 20f && Agent.Data.Energy > 40f)
        {
            ChangeState("think");
        }
    }
}
public class WalkState : State
{
    public WalkState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public Vector3 Destination { get; set; } = Vector3.zero;
    public override void Enter()
    {
        Agent.Move(Destination);
    }
    public override void Exit()
    {
        Log();
    }
    public override void Update(float deltaTime)
    {
        //Ações durante o estado
        if (!Agent.Data.IsCooldownFire && Agent.Data.Ammo > 0 && Agent.Data.HasSightEnemy)
        {
            Agent.Fire();
            ChangeState("think");
        }
        if (Agent.Data.PowerUps.Length != 0)
        {
            foreach (var energy in Agent.Data.PowerUps)
            {
                if (Vector3.Distance(Agent.Data.Position, energy) < 15f && Agent.Data.Energy < 50f || Vector3.Distance(Agent.Data.Position, energy) < 10f && Agent.Data.Energy < 70f)
                {
                    Destination = energy;
                    Enter();
                    break;
                }
            }
        }
        if (Agent.Data.AmmoRefill.Length != 0)
        {
            Vector3 refilPosition;
            refilPosition = Agent.Data.AmmoRefill[0];
            if (Vector3.Distance(Agent.Data.Position, refilPosition) < 10 && Agent.Data.Ammo == 0)
            {
                Destination = refilPosition;
                Enter();
            }
        }
        //Se já fez o que tinha que fazer
        if (Vector3.Distance(Agent.Data.Position, Destination) <= 1.17f || Agent.Data.RemainingDistance <= 0.05f)
        {
            ChangeState("think");
        }
        //Condições para tirar do estado
        if (Agent.Data.Energy < 10 && Agent.Data.RemainingDistance > 5)
        {
            ChangeState("idle");
        }
    }
}
public class CaptureFlagState : WalkState
{
    public CaptureFlagState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter()
    {
        Destination = (Vector3)Agent.EnemyData[0].FlagPosition;
        base.Enter();
    }
    public override void Update(float deltaTime)
    {
        //Ações durante o estado
        if (!Agent.Data.IsCooldownFire && Agent.Data.Ammo > 0 && Agent.Data.HasSightEnemy)
        {
            Agent.Fire();
            ChangeState("think");
        }
        if (Agent.Data.PowerUps.Length != 0)
        {
            foreach (var energy in Agent.Data.PowerUps)
            {
                if (Vector3.Distance(Agent.Data.Position, energy) < 15f && Agent.Data.Energy < 50f || Vector3.Distance(Agent.Data.Position, energy) < 10f && Agent.Data.Energy < 70f)
                {
                    Destination = energy;
                    base.Enter();
                    break;
                }
            }
        }
        if (Agent.Data.AmmoRefill.Length != 0)
        {
            Vector3 refilPosition;
            refilPosition = Agent.Data.AmmoRefill[0];
            if (Vector3.Distance(Agent.Data.Position, refilPosition) < 10 && Agent.Data.Ammo == 0)
            {
                Destination = refilPosition;
                base.Enter();
            }
        }
        //Se já fez o que tinha que fazer
        if (Agent.Data.RemainingDistance <= 0.05f)
        {
            ChangeState("think");
        }
        //Condições para tirar do estado
        if (Agent.Data.Energy < 10 && Agent.Data.RemainingDistance > 5)
        {
            ChangeState("idle");
        }
    }
}
public class DeliverFlagState : WalkState
{
    public DeliverFlagState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter()
    {
        Destination = (Vector3)Agent.Data.StartPosition;
        base.Enter();
    }
    public override void Update(float deltaTime)
    {
        //Ações durante o estado
        if (!Agent.Data.IsCooldownFire && Agent.Data.Ammo > 0 && Agent.Data.HasSightEnemy)
        {
            Agent.Fire();
            ChangeState("think");
        }
        if (Agent.Data.PowerUps.Length != 0)
        {
            foreach (var energy in Agent.Data.PowerUps)
            {
                if (Vector3.Distance(Agent.Data.Position, energy) < 15f && Agent.Data.Energy < 50f || Vector3.Distance(Agent.Data.Position, energy) < 10f && Agent.Data.Energy < 70f)
                {
                    Destination = energy;
                    base.Enter();
                    break;
                }
            }
        }
        if (Agent.Data.AmmoRefill.Length != 0)
        {
            Vector3 refilPosition;
            refilPosition = Agent.Data.AmmoRefill[0];
            if (Vector3.Distance(Agent.Data.Position, refilPosition) < 10 && Agent.Data.Ammo == 0)
            {
                Destination = refilPosition;
                base.Enter();
            }
        }
        //Se já fez o que tinha que fazer
        if (Agent.Data.RemainingDistance <= 0.05f)
        {
            ChangeState("think");
        }
        //Condições para tirar do estado
        if (Agent.EnemyData[0].HasFlag)
        {
            ChangeState("think");
        }
        if (Agent.Data.Energy < 10 && Agent.Data.RemainingDistance > 5)
        {
            ChangeState("idle");
        }
    }
}
public class HuntEnemyState : WalkState
{
    public HuntEnemyState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter()
    {
        Destination = Agent.EnemyData[0].Position;
        base.Enter();
    }
    public override void Update(float deltaTime)
    {
        //Ações durante o estado
        if (!Agent.Data.IsCooldownFire && Agent.Data.Ammo > 0 && Agent.Data.HasSightEnemy)
        {
            Agent.Fire();
            ChangeState("think");
        }
        if (Agent.Data.PowerUps.Length != 0)
        {
            foreach (var energy in Agent.Data.PowerUps)
            {
                if (Vector3.Distance(Agent.Data.Position, energy) < 15f && Agent.Data.Energy < 50f || Vector3.Distance(Agent.Data.Position, energy) < 10f && Agent.Data.Energy < 70f)
                {
                    Destination = energy;
                    base.Enter();
                    break;
                }
            }
        }
        if (Agent.Data.AmmoRefill.Length != 0)
        {
            Vector3 refilPosition;
            refilPosition = Agent.Data.AmmoRefill[0];
            if (Agent.Data.Ammo == 0)
            {
                Destination = refilPosition;
                base.Enter();
            }
        }
        //Se já fez o que tinha que fazer
        if (!Agent.EnemyData[0].HasFlag)
        {
            ChangeState("think");
        }
        //Condições para tirar do estado
        if (Agent.Data.Energy < 10 && Agent.Data.RemainingDistance > 5)
        {
            ChangeState("idle");
        }
    }
}