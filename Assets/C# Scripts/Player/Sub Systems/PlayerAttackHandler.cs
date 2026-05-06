using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for performinmg and tracking attack states.
/// </summary>
[System.Serializable]
public class PlayerAttackHandler
{
    private readonly PlayerInputHandler inputHandler;
    private readonly PlayerStateMachine stateMachine;
    private readonly PlayerColliderHandler colliderHandler;

    [SerializeField] private AttackSequence currentSequence;
    [SerializeField] private bool isSequenceActive;
    public AttackData CurrentActiveAttack => currentSequence.Attack;


    public PlayerAttackHandler(PlayerInputHandler inputHandler, PlayerStateMachine stateMachine, PlayerColliderHandler colliderHandler)
    {
        this.inputHandler = inputHandler;
        this.stateMachine = stateMachine;
        this.colliderHandler = colliderHandler;
    }

    #region TickUpdate AttackIntersection and Attack Result Handling

    /// <summary>
    ///If any move is active from this fighter (attacker perpective), check collision between any active hurtboxes with the opponents hitboxes.
    /// </summary>
    public bool CheckAttackIntersection(PlayerController target, out AttackResult attackResult)
    {
        attackResult = AttackResult.Missed;
        if (stateMachine.State.CombatState != CombatState.AttackActive) return false;

        // Check if any opponent hitbix is hit
        if (CollisionUtils.CheckAABBIntersection(target.ColliderHandler.HitBoxes, colliderHandler.HurtBoxes))
        {
            // hit opponent and send Attack Level (Low/Mid/High)
            attackResult = target.OnAttackImpact(CurrentActiveAttack.Level);
            return true;
        }

        return false;
    }

    // Mental/Logic Notes:
    //
    // Inputs are always between ticks, either before or after, NEVER during. (This is because inputs get collected in Update and Update runs before and sepperately from Tick)
    //
    // Crouching (State) transitions to sidestep (State) when the down input gets pressed + released within 1-3ish frames.
    // When sidestep is inputted, animation cancels from wherever crouch was into sidestep INSTANTLY and the fighter is now INSTANTLY considered standing
    //
    // When fighter presses down before next tick, they are considered crouched in that next tick.
    // And so when fighter releases down before next tick, they are considered standing in that next tick.
    //
    // When an attack is inputted, on the next frame it gets executed, that tick counts towards the attacks duration (That tick is the first tick of the attack startup).

    /// <summary>
    /// Get <see cref="AttackResult"/> based on what type of attack hit the defender in what state
    /// </summary>
    public static AttackResult GetAttackResult(AttackLevel attackType, FighterState defenderState)
    {
        // If the defender is in an active parry
        if (defenderState.CombatState == CombatState.ParryHigh)
        {
            return attackType == AttackLevel.High
                ? AttackResult.Parried
                : AttackResult.Hit;
        }
        if (defenderState.CombatState == CombatState.ParryLow)
        {
            return attackType == AttackLevel.Low
                ? AttackResult.Parried
                : AttackResult.Hit;
        }

        // If defender cant block or the incoming attack is unblockable, the defender gets hit OR interrupted
        if ((defenderState.CanBlock() == false) || attackType == AttackLevel.Unblockable)
        {
            return defenderState.CombatState == CombatState.AttackStartup ?
                AttackResult.CounterHit :
                AttackResult.Hit;
        }

        // If defender is crouching, they blocks lows, duck highs but lose to mids
        if (defenderState.GroundState == GroundState.Crouching)
        {
            return attackType switch
            {
                AttackLevel.Low =>
                 AttackResult.LowBlocked,

                AttackLevel.High =>
                    AttackResult.Missed,
                _ =>
                    AttackResult.Hit,
            };
        }
        // If defender is standing
        if (defenderState.GroundState == GroundState.Standing)
        {
            return defenderState.MovementState switch
            {
                // If defender is standing still or walking backwards, they blocks mids and highs and lose to lows
                MovementState.Idle or MovementState.Retreating =>
                    attackType switch
                    {
                        AttackLevel.Mid or AttackLevel.High =>
                            AttackResult.StandingBlocked,

                        _ =>
                            AttackResult.Hit,
                    },

                _ =>
                    AttackResult.Hit,
            };
        }

        // Should be unreachable, so parry to allow quicker debugging.
        return AttackResult.Parried;
    }

    #endregion


    /// <summary>
    /// Tick down active attack sequence or try creating a new one if there is no active on anymore.
    /// </summary>
    public void CheckAndResolveAttackInput()
    {
        if (isSequenceActive)
        {
            currentSequence.TickUpdate(out bool isStillActive);
            isSequenceActive = isStillActive;

            if (isStillActive) return;
        }

        TryCreateNewAttackSequence();
    }
    private void TryCreateNewAttackSequence()
    {
        if (inputHandler.TryReadAttack(out AttackData targetAttack))
        {
            currentSequence = new AttackSequence(targetAttack);
        }
    }
}