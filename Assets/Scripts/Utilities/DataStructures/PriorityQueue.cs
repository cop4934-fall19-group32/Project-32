using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class PriorityQueue
 * @brief Implementation of a MinHeap, becuase for 
 *        some mind-fucking reason C# doesn't have one.
 * @tparam T The type stored by the heap
 */
public class PriorityQueue<T> where T : IComparable<T> {

	/**
	 * Built-in sorting function for priority queue users
	 * @returns true if left is greater than right
	 */
	public Func<T, T, bool> PrioritizeGreater = 
		(T left, T right) => { 
			return left.CompareTo(right) > 0; 
		}
	;

	/**
	 * Built-in sorting function for priority queue users
	 * @returns true if left is less than right
	 */
	public Func<T, T, bool> PrioritizeLess =
		(T left, T right) => {
			return left.CompareTo(right) < 0;
		}
	;

	/** Underlying array-based beap */
	private List<T> Heap = new List<T>();

	/** Sorting function that is expected to return true if argument 1 is higher priority than argument 2 */
	private Func<T, T, bool> SortingFunction;

	/** Number of elements in heap */
	public int Count { get { return Heap.Count; } }

	/** Whether or not the heap is empty */
	public bool IsEmpty { get { return Heap.Count == 0; } }

	/** Returns top element of heap */
	public T Top {
		get {
			if (Heap.Count == 0) {
				throw new InvalidOperationException();
			}

			var top = Heap[0];

			return top;
		}
	}

	/**
	 * Constructor
	 * @note defualt behavior is that of a min-heap operating on T
	 */
	public PriorityQueue() {
		SortingFunction = PrioritizeLess;
	}

	/**
	 * Constructor
	 * @param sortingFunction
	 * @note result of sortingFunction should return "is left argument higher priority than right?"
	 */
	public PriorityQueue(Func<T, T, bool> sortingFunction) {
		SortingFunction = sortingFunction;
	}

	/**
	 * Allows caller to add entry to priority queue
	 * @param entry The item to add and prioritize
	 */
	public void Push(T entry) {
		Heap.Add(entry);
		var curr = Heap.Count -  1;

		//Percolate new entry upward as a minheap
		while (curr != 0) {
			var parent = GetParent(curr);

			//If curr has a higher priority than parent, percolate upward
			if (SortingFunction(Heap[curr], Heap[parent]))
			{
				Swap(curr, parent);
				curr = parent;
			}
			else 
			{
				break;
			}
		}
	}

	/**
	 * Removes the highest priority item from the heap and maintains the heap invariant.
	 */
	public void Pop() {
		if (Heap.Count == 0) {
			throw new InvalidOperationException();
		}

		Heap[0] = Heap[Heap.Count - 1];
		Heap.RemoveAt(Heap.Count - 1);
		int curr = 0;

		while (curr < Heap.Count - 1) {
			var currentNode = Heap[curr];
			var lstIndex = GetLeftChild(curr);
			var rstIndex = GetRightChild(curr);

			if (lstIndex < Heap.Count && SortingFunction(Heap[lstIndex], currentNode)) {
				//If the lst exists and has a higher priority, swap
				Swap(curr, lstIndex);
				curr = lstIndex;
			}
			else if (rstIndex < Heap.Count && SortingFunction(Heap[rstIndex], currentNode)) {
				//Else if the rst exists and has a higher priority, swap
				Swap(curr, rstIndex);
				curr = lstIndex;
			}
			else {
				break;
			}
		}
	}

	private void Swap(int i, int j) {
		var temp = Heap[i];
		Heap[i] = Heap[j];
		Heap[j] = temp;
	}

	private int GetParent(int i) {
		return (i-1)/2;
	}

	private int GetLeftChild(int i) {
		return (2 * i) + 1;
	}

	private int GetRightChild(int i) {
		return (2 * i) + 2;
	}
}
