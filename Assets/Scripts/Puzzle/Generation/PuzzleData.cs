using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleData : MonoBehaviour
{
    private void Awake() {
        PuzzleName = gameObject.name;
        var nextNode = gameObject.GetComponentInParent<MapNode>().Next;

        Debug.LogWarning("Puzzle " + PuzzleName + " does not have a 'next node'");
        NextPuzzleName = (nextNode != null) ? nextNode.gameObject.name : PuzzleName;

        if (GenerateRandomInput) {
            System.Random random = (RandomSeed == -1) ? new System.Random() : new System.Random(RandomSeed);

            for (int i = 0; i < InputStream.Length; i++) {
                InputStream[i] = random.Next(0, 99);
            }
        }

        OutputStream = OutputGenerator.GenerateSolution(InputStream);
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

    [Header("Puzzle I/O Settings")]
    public bool GenerateRandomInput = false;
    public int RandomSeed = -1;
    public int CardAddressSeed = -1;
    public int[] InputStream;

    public List<int> OutputStream { get; private set; }
}
