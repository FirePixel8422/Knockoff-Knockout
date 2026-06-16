


/// <summary>
/// Sub Player system handler class that is responsible for performing and tracking attack states.
/// </summary>
[System.Serializable]
public class PlayerAttackHandler
{
    private readonly PlayerInputHandler inputHandler;
    private readonly PlayerStateMachine stateMachine;
    private readonly PlayerColliderHandler colliderHandler;
    private readonly PlayerMovementHandler movementHandler;

    private ActionSequenceTimeline<CombatState> sequenceTimeline;
    private StateSequence<CombatState> attackSequence;

    public AttackData ActiveAttack { get; private set; }


    public PlayerAttackHandler(PlayerStateMachine stateMachine, PlayerInputHandler inputHandler, PlayerColliderHandler colliderHandler, PlayerMovementHandler movementHandler)
    {
        this.stateMachine = stateMachine;
        this.inputHandler = inputHandler;
        this.colliderHandler = colliderHandler;
        this.movementHandler = movementHandler;

        sequenceTimeline = new ActionSequenceTimeline<CombatState>(new ((CombatState.Idle, 0)));
        attackSequence = new StateSequence<CombatState>(
            (CombatState.ActionStartup, 0),
            (CombatState.AttackActive, 0),
            (CombatState.Recovering, 0),
            (CombatState.Idle, 0));

        stateMachine.OnStunned += OnStunned;
    }
    private PlayerAttackHandler() { }


    #region Attack Hit Detection and Impact

    /// <summary>
    /// Check if any move is active from this fighter (attacker perpective), check collision between any active hurtboxes with the opponents hitboxes.
    /// </summary>
    public bool CheckAttackIntersection(PlayerController target)
    {
        colliderHandler.RecalculateActiveHurtBoxes();

        // Check if any opponent hitbox is hit
        return CollisionUtils.CheckIntersection(target.ColliderHandler.HitBoxOBBs, colliderHandler.HurtBoxOBBs);
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
    public AttackResult GetInboundAttackResult(AttackLevel attackLevel, bool doesKnockDown)
    {
        FighterState defenderState = stateMachine.State;

        // If defender is knocked down, they dodge highs but lose to mids and highs
        if (defenderState.StanceState == StanceState.KnockedDownStomach ||
            defenderState.StanceState == StanceState.KnockedDownBack ||
            defenderState.StanceState == StanceState.Wakeup)
        {
            return attackLevel switch
            {
                AttackLevel.High or AttackLevel.Mid =>
                    AttackResult.Missed,

                _ => doesKnockDown ?
                    AttackResult.KnockDown :
                    AttackResult.Hit,
            };
        }

        // If defender cant block or the incoming attack is unblockable, the defender gets hit OR interrupted
        if ((defenderState.CanBlock() == false) || attackLevel == AttackLevel.Unblockable)
        {
            if (attackLevel == AttackLevel.High && stateMachine.State.StanceState == StanceState.Crouching)
            {
                return AttackResult.Missed;
            }
            if (doesKnockDown)
            {
                return AttackResult.KnockDown;
            }
            if (defenderState.CombatState == CombatState.ActionStartup)
            {
                return AttackResult.CounterHit;
            }
            return AttackResult.Hit;
        }

        // If defender is crouching, they blocks lows, dodge highs but lose to mids
        if (defenderState.StanceState == StanceState.Crouching)
        {
            return attackLevel switch
            {
                AttackLevel.Low =>
                    AttackResult.CrouchBlocked,

                AttackLevel.High =>
                    AttackResult.Missed,

                _ => doesKnockDown ?
                    AttackResult.KnockDown :
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
                    attackLevel switch
                    {
                        AttackLevel.Mid or AttackLevel.High =>
                            AttackResult.StandingBlocked,

                        _ => doesKnockDown ?
                            AttackResult.KnockDown :
                            AttackResult.Hit,
                    },

                _ => doesKnockDown ?
                    AttackResult.KnockDown :
                    AttackResult.Hit,
            };
        }

        DebugLogger.Log("Gatcha");
        // Should be unreachable, so "Missed" to allow quicker debugging.
        return AttackResult.Missed;
    }

    #endregion


    /// <summary>
    /// Update any active attack action sequence
    /// </summary>
    public void TickUpdateAttackSequence()
    {
        if (!sequenceTimeline.IsActive) return;

        UpdateActiveAttackAction();
    }
    public void TickUpdateAttackInput(bool isInCombatLock)
    {
        if (sequenceTimeline.IsActive)
        {
            if (sequenceTimeline.ElapsedTicks > ActiveAttack.FrameData.Startup &&
                sequenceTimeline.ElapsedTicks == ActiveAttack.FrameData.Startup + ActiveAttack.FrameData.CancelWindow)
            {
                if (!inputHandler.TryReadStringAttack(ActiveAttack.BakedStringTransitions, out AttackData attackData)) return;

                StartNewAttackSequence(attackData);
            }
            return;
        }
        
        // If input for an attack was found in inputbuffer, start an attack sequence
        if (isInCombatLock || !inputHandler.TryReadAttack(out AttackData targetAttack)) return;

        StartNewAttackSequence(targetAttack);
    }

    /// <summary>
    /// TickUpdate active attack sequence and send state changes to the <see cref="PlayerStateMachine"/>. Also handle lunge movement if active attack has lunge data.
    /// </summary>
    private void UpdateActiveAttackAction()
    {
        // Tick update attack sequence and update colliders based on state changes
        if (sequenceTimeline.TickUpdateState(out CombatState newState, out int elapsedSequenceTicks))
        {
            stateMachine.SetCombatState(newState);

            if (newState == CombatState.AttackActive)
            {
                colliderHandler.EnableTargetHurtBoxes(ActiveAttack.HurtBoxIds);

                switch (ActiveAttack.Type)
                {
                    case AttackType.Punch:
                        AudioManager.PlayPunchSFX();
                        break;

                    case AttackType.Kick:
                        AudioManager.PlayKickSFX();
                        break;
                }
            }
            else if (newState == CombatState.Recovering)
            {
                colliderHandler.DisableAllHurtBoxes();
            }
        }

        LungeData lungeData = ActiveAttack.Lunge;
        if (lungeData.Power != 0 &&
            elapsedSequenceTicks >= lungeData.Window.x &&
            elapsedSequenceTicks <= lungeData.Window.y)
        {
            float t = (elapsedSequenceTicks - lungeData.Window.x) / (float)(lungeData.Window.y - lungeData.Window.x);
            float tPrev = (elapsedSequenceTicks - 1 - lungeData.Window.x) / (float)(lungeData.Window.y - lungeData.Window.x);

            float prevLunge = lungeData.Curve.Evaluate(tPrev);
            float currentLunge = lungeData.Curve.Evaluate(t);

            movementHandler.AddForwardForce((currentLunge - prevLunge) * lungeData.Power);
        }
    }

    /// <summary>
    /// Start a new attack sequence.
    /// </summary>
    private void StartNewAttackSequence(AttackData targetAttack)
    {
        ActiveAttack = targetAttack;

        stateMachine.PlayAnimation(targetAttack.AttackAnimData);

        stateMachine.SetCombatState(CombatState.ActionStartup);
        stateMachine.SetStanceState(targetAttack.Stance);

        movementHandler.RealignFighter();

        attackSequence.Sequence[0] = (CombatState.ActionStartup, targetAttack.FrameData.Startup);
        attackSequence.Sequence[1] = (CombatState.AttackActive, targetAttack.FrameData.Active);
        attackSequence.Sequence[2] = (CombatState.Recovering, targetAttack.FrameData.Recovery);
        //attackSequence.Sequence[3] = (CombatState.Idle, 0);

        sequenceTimeline = new ActionSequenceTimeline<CombatState>(attackSequence);
        movementHandler.OnAttackStart();
    }

    /// <summary>
    /// Called by the <see cref="PlayerController"/> when the current active attack hits a target.
    /// </summary>
    public void OnActiveAttackConnected()
    {
        int activeTicksLeft = sequenceTimeline.AdvanceState();

        stateMachine.TickAdvanceAnimation(activeTicksLeft);
    }

    /// <summary>
    /// Called when this fighter gets stunned by a parry or an attack
    /// </summary>
    public void OnStunned()
    {
        // When fighter gets stunned by an attack, clear their input buffer to avoid unintended buffered inputs after hitstun wears off.
        // This to ensure the player doesnt accidentally do a buffer spammed move that makes them even more vulnerable after getting hit.
        inputHandler.ClearInputBuffer();
        sequenceTimeline.Cancel();
    }
}