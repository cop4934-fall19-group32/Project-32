using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    public GameObject RegisterCardPrefab;
    public GameObject StackCardPrefab;
    public GameObject QueueCardPrefab;
    public GameObject HeapCardPrefab;
    public CachedCardContainer CardHand;
    public PlayedCardContainer CardPlayArea;

    private HashSet<string> addresses = new HashSet<string>();

    // Based on the information in puzzleData, this function will respectively instantiate cards
    // into the card hand. For each card, the function generates a unique hexidecimal address and
    // attaches the necessary card script to the object.
    public void GenerateHand(PuzzleData puzzleData)
    {
        System.Random random = new System.Random(puzzleData.CardAddressSeed);
        SpawnCard(RegisterCardPrefab, puzzleData.NumRegisterCards, random);
        SpawnCard(StackCardPrefab, puzzleData.NumStackCards, random);
        SpawnCard(QueueCardPrefab, puzzleData.NumQueueCards, random);
        SpawnCard(HeapCardPrefab, puzzleData.NumHeapCards, random);
    }

    private void SpawnCard(GameObject prefab, int num, System.Random random) {
        for (int i = 0; i < num; i++) {
            GameObject cardObj = Instantiate(prefab, transform);
            string address = GenerateAddress(random);
            cardObj.name = address;
            cardObj.GetComponent<CardLogic>().Address = address;
            CardHand.AddCard(cardObj);
        }
    }

    private string GenerateAddress(System.Random random) {
        int num = random.Next(0x100, 0xFFF);

        // No duplicates!
        while (addresses.Contains(num.ToString()))
            num = random.Next(0x100, 0xFFF);

        addresses.Add(num.ToString());
        return "0x" + Convert.ToString(num, 16);
    }

}
