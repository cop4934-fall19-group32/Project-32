using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    private int NumStars = 0;
    public GameObject emptyStar1;
    public GameObject emptyStar2;
    public GameObject emptyStar3;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public void StarAligner(bool HasEfficiency, bool HasInstructionCount, bool HasMemory)
    {
        // Count the number of possible stars.
        if (HasEfficiency)
            NumStars++;
        if (HasInstructionCount)
            NumStars++;
        if (HasMemory)
            NumStars++;

        // Align the empty stars accordingly.
        if (NumStars == 1)
        {
            emptyStar2.SetActive(true);
        }
        else if (NumStars == 2)
        {
            emptyStar1.SetActive(true);
            emptyStar3.SetActive(true);
        }
        else if (NumStars == 3)
        {
            emptyStar1.SetActive(true);
            emptyStar2.SetActive(true);
            emptyStar3.SetActive(true);
        }
    }

    public void StarPlacer(string puzzleName)
    {
        // Get a reference to the PlayerState object.
        GameObject playerStateObj = GameObject.Find("PlayerState");
        if (playerStateObj == null)
            return;
        PlayerState playerState = playerStateObj.GetComponent<PlayerState>();

        if (!playerState.ContainsPuzzleSave(puzzleName))
            return;

        bool efficiencyEarned = playerState.GetStarEarned(puzzleName, StarType.EFFICIENCY);
        bool instructionCountEarned = playerState.GetStarEarned(puzzleName, StarType.INSTRUCTION_COUNT);
        bool memoryEarned = playerState.GetStarEarned(puzzleName, StarType.MEMORY);

        int numEarned = 0;
        if (efficiencyEarned)
        {
            // Update description.
            numEarned++;
        }
        if (instructionCountEarned)
        {
            // Update description.
            numEarned++;
        }
        if (memoryEarned)
        {
            // Update description.
            numEarned++;
        }

        if (numEarned == 0)
            return;

        if (NumStars == 1)
        {
            if (numEarned > 0)
            {
                star2.SetActive(true);
            }
        }
        else if (NumStars == 2)
        {
            if (numEarned > 0)
            {
                star1.SetActive(true);
            }
            if (numEarned > 1)
            {
                star3.SetActive(true);
            }
        }
        else if (NumStars == 3)
        {
            if (numEarned > 0)
            {
                star1.SetActive(true);
            }
            if (numEarned > 1)
            {
                star2.SetActive(true);
            }
            if (numEarned > 2)
            {
                star3.SetActive(true);
            }
        }
    }
}
