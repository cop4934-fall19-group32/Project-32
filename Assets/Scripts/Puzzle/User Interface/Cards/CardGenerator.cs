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
    private HashSet<string> addresses = new HashSet<string>();

    public void InitializeHand(PuzzleData puzzleData) {
        InstantiateCards(puzzleData);
    }

    // Based on the information in puzzleData, this function will respectively instantiate cards
    // into the card hand. For each card, the function generates a unique hexidecimal address and
    // attaches the necessary card script to the object.
    private void InstantiateCards(PuzzleData puzzleData)
    {
        SpawnCard(RegisterCardPrefab, puzzleData.NumRegisterCards);
        SpawnCard(StackCardPrefab, puzzleData.NumStackCards);
        SpawnCard(QueueCardPrefab, puzzleData.NumQueueCards);
        SpawnCard(HeapCardPrefab, puzzleData.NumHeapCards);
    }

    private void SpawnCard(GameObject prefab, int num) {
        for (int i = 0; i < num; i++) {
            GameObject cardObj = Instantiate(prefab);
            string address = GenerateAddress();
            cardObj.name = address;
            cardObj.GetComponent<CardLogic>().Address = address;
            cardObj.transform.SetParent(this.transform);
            cardObj.transform.localScale = GetComponent<CardContainer>().containerScale;
        }
    }

    private string GenerateAddress() {
        System.Random random = new System.Random();
        int num = random.Next(0x100, 0xFFF);

        // No duplicates!
        while (addresses.Contains(num.ToString()))
            num = random.Next(0x100, 0xFFF);

        addresses.Add(num.ToString());
        return "0x" + Convert.ToString(num, 16);
    }

}
