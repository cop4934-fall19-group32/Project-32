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

    public List<StarType> StarAligner(bool HasEfficiency, bool HasInstructionCount, bool HasMemory)
    {
        List<StarType> availableStars = new List<StarType>();

        // Count the number of possible stars.
        if (HasEfficiency)
        {
            availableStars.Add(StarType.EFFICIENCY);
            NumStars++;
        }
        if (HasInstructionCount)
        {
            availableStars.Add(StarType.INSTRUCTION_COUNT);
            NumStars++;
        }
        if (HasMemory)
        {
            availableStars.Add(StarType.MEMORY);
            NumStars++;
        }

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

        return availableStars;
    }

    public HashSet<StarType> StarPlacer(string puzzleName)
    {
        HashSet<StarType> earnedStars = new HashSet<StarType>();

        // Get a reference to the PlayerState object.
        GameObject playerStateObj = GameObject.Find("PlayerState");
        if (playerStateObj == null)
            return earnedStars;
        PlayerState playerState = playerStateObj.GetComponent<PlayerState>();

        if (!playerState.ContainsPuzzleSave(puzzleName))
            return earnedStars;

        bool efficiencyEarned = playerState.GetStarEarned(puzzleName, StarType.EFFICIENCY);
        bool instructionCountEarned = playerState.GetStarEarned(puzzleName, StarType.INSTRUCTION_COUNT);
        bool memoryEarned = playerState.GetStarEarned(puzzleName, StarType.MEMORY);

        int numEarned = 0;
        if (efficiencyEarned)
        {
            earnedStars.Add(StarType.EFFICIENCY);
            numEarned++;
        }
        if (instructionCountEarned)
        {
            earnedStars.Add(StarType.INSTRUCTION_COUNT);
            numEarned++;
        }
        if (memoryEarned)
        {
            earnedStars.Add(StarType.MEMORY);
            numEarned++;
        }

        if (numEarned == 0)
            return earnedStars;

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

        return earnedStars;
    }
}
