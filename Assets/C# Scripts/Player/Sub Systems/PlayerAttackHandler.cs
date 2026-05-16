using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for performing and tracking attack states.
/// </summary>
[System.Serializable]
public class PlayerAttackHandler
{
    private readonly PlayerInputHandler inputHandler;
    private readonly PlayerStateMachine stateMachine;
    private readonly PlayerColliderHandler colliderHandler;

    [SerializeField] private AttackSequence currentSequence;
    public AttackData CurrentActiveAttack => currentSequence.Attack;


    public PlayerAttackHandler(PlayerStateMachine stateMachine, PlayerInputHandler inputHandler, PlayerColliderHandler colliderHandler)
    {
        this.stateMachine = stateMachine;
        this.inputHandler = inputHandler;
        this.colliderHandler = colliderHandler;
    }


    #region Attack Hit Detection and Impact

    /// <summary>
    /// Check if any move is active from this fighter (attacker perpective), check collision between any active hurtboxes with the opponents hitboxes.
    /// </summary>
    public bool CheckAttackIntersection(PlayerController target, out AttackData activeAttack)
    {
        activeAttack = CurrentActiveAttack;

        // Check if any opponent hitbox is hit
        return CollisionUtils.CheckAABBIntersection(target.ColliderHandler.HitBoxAABBs, colliderHandler.HurtBoxAABBs);
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
    //
    // When an attack connects, instantly advance from active to recovery state.

    /// <summary>
    /// Get <see cref="AttackResult"/> based on what type of attack hit this fighter (defender perspective) in what state
    /// </summary>
    public AttackResult GetInboundAttackResult(AttackLevel attackType)
    {
        FighterState defenderState = stateMachine.State;

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
        if (defenderState.StanceState == StanceState.Crouching)
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
        if (defenderState.StanceState == StanceState.Standing)
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
    /// Tick down active attack sequence or try creating a new one if there is no active on anymore. (PostTickUpdate)
    /// </summary>
    public void TickUpdateAttackSequence()
    {
        bool isSequenceActive = currentSequence.State != CombatState.Idle;

        if (isSequenceActive)
        {
            // Tick attack sequence
            if (currentSequence.TickUpdateState(out CombatState newState))
            {
                stateMachine.SetCombatState(currentSequence.State);

                if (newState == CombatState.AttackActive)
                {
                    colliderHandler.EnableTargetHurtBoxes(CurrentActiveAttack.HurtBoxIds);
                }
                else if (newState == CombatState.Recovering)
                {
                    colliderHandler.DisableAllHurtBoxes();
                }
            }
            return;
        }

        // If input for an attack was found in inputbuffer, start an attack sequence
        if (inputHandler.TryReadAttack(out AttackData targetAttack))
        {
            stateMachine.PlayAnimation(targetAttack.GeneratedAnimHash, 2);
            stateMachine.SetCombatState(CombatState.AttackStartup);

            currentSequence = new AttackSequence(targetAttack);
        }
    }
    
    /// <summary>
    /// Called by the <see cref="PlayerController"/> when the current active attack hits a target.
    /// </summary>
    public void OnActiveAttackConnected()
    {
        int activeTicksLeft = currentSequence.EndAttackActiveState();

        stateMachine.TickAdvanceAnimation(activeTicksLeft);
        stateMachine.SetCombatState(CombatState.Recovering);
    }

    /// <summary>
    /// Called when this fighter gets stunned by a parry or an attack
    /// </summary>
    public void OnStunned()
    {
        // When fighter gets stunned by an attack, clear their input buffer to avoid unintended buffered inputs after hitstun wears off.
        // This to ensure the player doesnt accidentally do a buffer spammed move that makes them even more vulnerable after getting hit.
        inputHandler.ClearInputBuffer();
        currentSequence.Interrupt();
    }
}