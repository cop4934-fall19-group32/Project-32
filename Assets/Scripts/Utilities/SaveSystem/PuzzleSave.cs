using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StarType { 
	EFFICIENCY,
	INSTRUCTION_COUNT,
	MEMORY
}

/**
 * @class PuzzleSave
 * @brief Class used to serialize all of the information needed to
 *        record the state of a puzzle
 */
[Serializable]
public class PuzzleSave
{
	[SerializeField]
	public string LevelName;

	/** Field to track whether the level has been completed */
	[SerializeField]
	public bool Completed;

    /** Field to store player's solution for the level */
    [SerializeField]
	public List<CachedCommand> CachedInstructions;

    /** Field to store player's cached cards for the level */
    [SerializeField]
    public List<CachedCard> CachedCards;

    /** Property to track whether the player earned an efficiency star for this level */
    [SerializeField]
	public bool EarnedEfficiencyStar;

	/** Property to track whether the player earned an instruction count star for this level */
	[SerializeField]
	public bool EarnedInstructionCountStar;

	/** Property to track whether the player earned a memory star for this level */
	[SerializeField]
	public bool EarnedMemoryStar;

	/**
	 * Constructor
	 */
	public PuzzleSave(string name) {
		LevelName = name;
		Completed = false;
		CachedInstructions = new List<CachedCommand>();
        CachedCards = new List<CachedCard>();

        EarnedEfficiencyStar = false;
		EarnedInstructionCountStar = false;
		EarnedMemoryStar = false;
	}
}
