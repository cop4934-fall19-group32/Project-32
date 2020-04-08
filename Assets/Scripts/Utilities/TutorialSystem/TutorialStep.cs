using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStep
{
    public bool Complete { get { return IsComplete(); } }
    public string Description { get; set; }
    private List<Action> BeginBehaviors;
    private List<Func<bool>> CompleteChecks;
    private List<Action> EndBehaviors;

    /**
     * Defualt constructor
     */
    public TutorialStep() {
        BeginBehaviors = new List<Action>();
        CompleteChecks = new List<Func<bool>>();
        EndBehaviors = new List<Action>();
    }

    /**
     *  Copy Constructor
     */
    public TutorialStep(TutorialStep other) : this() {
        BeginBehaviors = other.BeginBehaviors;
        CompleteChecks = other.CompleteChecks;
        EndBehaviors = other.EndBehaviors;
    }

    public void AddBeginBehavior(Action lambda) {
        BeginBehaviors.Add(lambda);
    }

    public void AddCompletionCondition(Func<bool> lambda) {
        CompleteChecks.Add(lambda);
    }

    public void AddEndBehavior(Action lambda) {
        EndBehaviors.Add(lambda);
    }

    public void Begin() {
        foreach (var lambda in BeginBehaviors) {
            lambda.Invoke();
        }
    }

    private bool IsComplete() {
        foreach (var check in CompleteChecks) {
            if (!check.Invoke()) {
                return false;
            }
        }
        return true;
    }

    public void End() {
        foreach (var lambda in EndBehaviors) {
            lambda.Invoke();
        }
    }
}
