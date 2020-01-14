using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class PriorityQueue
 * @brief Implementation of a MinHeap, becuase for 
 *        some mind-fucking reason C# doesn't have one.
 * @tparam T The type stored by the heap
 * @tparam U The type used to prioritize entries
 */
public class PriorityQueue<T> {

	/**
	 * Underlying array-based beap
	 */
	private List<Tuple<T, float>> Heap = new List<Tuple<T, float>>();

	/**
	 * Number of elements in heap
	 */
	public int Count { get { return Heap.Count; } }

	/**
	 * Whether or not the heap is empty
	 */
	public bool IsEmpty { get { return Heap.Count == 0; } }

	public T Top {
		get {
			if (Heap.Count == 0) {
				throw new InvalidOperationException();
			}

			var top = Heap[0].Item1;

			return top;
		}
	}

	/**
	 * Constructor
	 */
	public PriorityQueue() { 
	
	}

	public void Push(T entry, float priority) {
		Heap.Add(new Tuple<T,float>(entry, priority));
		var curr = Heap.Count -  1;

		//Percolate new entry upward as a minheap
		while (curr != 0) {
			var parent = GetParent(Heap.Count - 1);
			if (Heap[parent].Item2.CompareTo(priority) > 0) {
				var temp = Heap[parent];
				Heap[parent] = Heap[curr];
				Heap[curr] = temp;
				curr = parent;
			}
			else {
				break;
			}
		}
	}

	public void Pop() {
		if (Heap.Count == 0) {
			throw new InvalidOperationException();
		}

		Heap[0] = Heap[Heap.Count - 1];
		Heap.RemoveAt(Heap.Count - 1);
		int curr = 0;

		while (curr < Heap.Count - 1) {
			var currPriority = Heap[curr].Item2;

			var lst = GetLeftChild(curr);
			var rst = GetRightChild(curr);
			
			var leftPriority = (lst < Heap.Count) ? Heap[GetLeftChild(curr)].Item2 : float.MaxValue;
			var rightPriority = (rst < Heap.Count) ? Heap[GetRightChild(curr)].Item2 : float.MaxValue;


			if (currPriority > leftPriority &&
				leftPriority < rightPriority) {

				var temp = Heap[curr];
				Heap[curr] = Heap[lst];
				Heap[lst] = temp;
				curr = lst;
			}
			else if (currPriority > rightPriority &&
				rightPriority <= leftPriority) {

				var temp = Heap[curr];
				Heap[curr] = Heap[rst];
				Heap[rst] = temp;
				curr = rst;
			}
			else {
				//Cannot percolate further
				break;
			}
		}
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
