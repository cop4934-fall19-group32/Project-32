using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleData : MonoBehaviour
{
    private void Awake() {
        PuzzleName = transform.parent.name;
        var nextNode = gameObject.GetComponentInParent<MapNode>().Next;

        NextPuzzleName = (nextNode != null) ? nextNode.gameObject.name : PuzzleName;

        if (GenerateRandomInput) {
            System.Random random = (RandomSeed == -1) ? new System.Random() : new System.Random(RandomSeed);

            for (int i = 0; i < InputStream.Length; i++) {
                InputStream[i] = random.Next(0, 99);
            }
        }

        if (OutputGenerator != null) { 
            OutputStream = OutputGenerator.GenerateSolution(InputStream);
        }
    }

    /** Data gleaned from Level Select map structure */
    public string PuzzleName { get; set; }
    public string NextPuzzleName { get; set; }
    
    /** Data set by level designer */
    [Header("General Puzzle Data")]
    public string Description;
    public SolutionGenerator OutputGenerator;
    public List<string> Hints;

    [Header("Player Tool Allowance")]
    public int NumRegisterCards;
    public int NumStackCards;
    public int NumQueueCards;
    public int NumHeapCards;

    [Header("Star Settings")]
    public bool HasEfficiency;
    public float EfficiencyRequirement = -1f;
    public bool HasInstructionCount;
    public int InstructionCountRequirement = -1;
    public bool HasMemory;
    public float MemoryRequirement = -1f;

    [Header("Puzzle I/O Settings")]
    public bool GenerateRandomInput = false;
    public int RandomSeed = -1;
    public int CardAddressSeed = -1;
    public int[] InputStream;

    [Header("Instruction Filtering Settings")]
    public bool Input = false;
    public bool Output = false;
    public bool Jump = false;
    public bool JumpIfNull = false;
    public bool JumpIfLess = false;
    public bool JumpIfGreater = false;
    public bool JumpIfEqual = false;
    public bool MoveTo = false;
    public bool CopyTo = false;
    public bool MoveFrom = false;
    public bool CopyFrom = false;
    public bool Add = false;
    public bool Subtract = false;

    [Header("Tutorialization")]
    public InteractiveTutorial Tutorial;

    public List<int> OutputStream { get; private set; }
}
