using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SHATest;

public class DuplicateKeyComparer<TKey>
	:
IComparer<TKey> where TKey : System.IComparable
{
	public int Compare(TKey x, TKey y)
	{
		int result = x.CompareTo(y);

		if (result == 0)
			return -1;   // Handle equality as beeing greater
		else
			return result;
	}
}

public class Reverser  {

	public int operationPerFrame = 200;
	HashSet<Bit> knownBits = new HashSet<Bit>();
	SortedList<long,Bit> bitsToProcess = new SortedList<long,Bit>(new DuplicateKeyComparer<long>());

	SortedList<long,string> test = new SortedList<long,string>(new DuplicateKeyComparer<long>());

	public IEnumerator Go(List<Bit> bits, List<Bit.BitValue> values)
	{
		for (int i=0; i< values.Count; i++)
		{
			bits[i].SetValue(values[i]);
		}

		foreach (Bit bit in bits)
			knownBits.Add(bit);
		
		foreach (Bit bit in bits)
			bitsToProcess.Add(bit.booleanInstruction,bit);

//		test.Add(2,"deux");
//		test.Add(1,"un demi");
//		test.Add(3,"trois");
//		test.Add(1,"un");
//
//		for (int i=0; i<test.Count; i++)
//		{
//			Debug.Log(test.Values[i]);
//		}
//
//		test.RemoveAt(1);
//		for (int i=0; i<test.Count; i++)
//		{
//			Debug.Log(test.Values[i]);
//		}

		//yield return null;
		yield return Reverse();
	}

	IEnumerator Reverse()
	{
		long processed = 0;
		long processedTotal = 0;

		while (bitsToProcess.Count > 0)
		{
			//List<Bit> newBits = bitsToProcess.Values[0];

			processedTotal++;
			processed++;

			if (processed > operationPerFrame)
			{
				processed = 0;
				Debug.Log("Processes : " + processedTotal);
				yield return null;
			}
		}
	}
}
