using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//namespace SHATestlol
//{
//
//	public class Bit
//	{
//		//value can be :
//		//0-512 = origianl msg bit id
//		//		public static int WHATEVER = 513;
//		//		public static int ADD = 514;
//		//		public static int XOR = 515;
//		//		public static int RIGHTSHIFT = 516;
//		//		public static int RIGHTROTATE = 517;
//
//		Bit A;
//		Bit B;
//		public BitValue value = BitValue.UNDEFINED;
//		BitOperation operation = BitOperation.NONE;
//
//		public Bit()
//		{
//		}
//
//		public Bit(int aValue)
//		{
//			value = (BitValue)aValue;
//		}
//
//		public Bit(BitValue aValue)
//		{
//			value = aValue;
//		}
//
//		public Bit(BitOperation aOperation, Bit aA)
//		{
//			operation = aOperation;
//			A = aA;
//		}
//
//		public Bit(BitOperation aOperation, Bit aA, Bit aB)
//		{
//			operation = aOperation;
//			A = aA;
//			B = aB;
//		}
//
//		public enum BitValue
//		{
//			ZERO = 600,
//			ONE = 601,
//			WHATEVER = 602,
//			UNDEFINED = 1000 
//		}
//
//		public enum BitOperation
//		{
//			NONE = 0,
//			AND = 1,
//			OR = 2,
//			XOR = 3,
//			NOT = 4,
//		}
//
//		public string ToString(BitOperation parentBitOp = BitOperation.NONE) //
//		{
//			//This bit is a constant
//			if (operation == BitOperation.NONE)
//				return ValToString(value);
//
//			//This bit is ONE parameter operation ex : (not A)
//			if (B == null)
//			{
//				return OpToString(operation) + B.ToString(operation);
//			}
//
//			//This bit is TWO parameter operation ex : (A & B)
//			string operationString = A.ToString(operation) + OpToString(operation) + B.ToString(operation);
//
//			if (operation != parentBitOp)
//				return "(" + operationString + ")";
//			else
//				return operationString;
//
//		}
//
//		string ValToString(BitValue val)
//		{
//			if ((int)val < 512)
//				return ((int)val).ToString();
//
//			switch (val)
//			{
//			case BitValue.WHATEVER :
//				return "W";
//			case BitValue.ZERO :
//				return "F";
//			case BitValue.ONE :
//				return "T";
//			case BitValue.UNDEFINED :
//				return "?";
//			}
//
//			return "ERROR";
//		}
//
//		string OpToString(BitOperation op)
//		{
//			switch (op)
//			{
//			case BitOperation.NONE :
//				Debug.LogError("Bit doesn't have operation set A:" + (A != null) + " B:" + (B != null));
//				return "Error";
//			case BitOperation.OR :
//				return "|";
//			case BitOperation.AND :
//				return "&";
//			case BitOperation.XOR :
//				return "^";
//			case BitOperation.NOT :
//				return "!";
//			}
//
//			Debug.LogError("Bit doesn't have operaion set A:" + (A != null) + " B:" + (B != null));
//			return "ERROR";
//		}
//
//		public bool IsConstant()
//		{
//			return (value == BitValue.ZERO || value == BitValue.ONE);
//		}
//
//
//		public static Bit operator^ (Bit a, Bit b)
//		{
//			return GetResultBitOptimized(a,b,BitOperation.XOR);
//		}
//
//		public static Bit operator& (Bit a, Bit b)
//		{
//			return GetResultBitOptimized(a,b,BitOperation.AND);
//		}
//
//		public static Bit operator| (Bit a, Bit b)
//		{
//			return GetResultBitOptimized(a,b,BitOperation.OR);
//		}
//
//		public static Bit operator! (Bit a)
//		{
//			return GetResultBitOptimized(a,null,BitOperation.NOT);
//		}
//
//		public static Bit GetResultBitOptimized(Bit aA, Bit aB, BitOperation aOperation)
//		{
//			switch (aOperation)
//			{
//			case BitOperation.NONE :
//				Debug.LogError("Bit doesn't have operation set A:" + (aA != null) + " B:" + (aB != null));
//				return new Bit(BitValue.UNDEFINED);
//			case BitOperation.OR :
//				if (aA.IsConstant() && aB.IsConstant())
//				{
//					if (aA.value == BitValue.ONE || aB.value == BitValue.ONE)
//						return new Bit(BitValue.ONE);
//					else
//						return new Bit(BitValue.ZERO);
//				}
//				else if (aA.IsConstant())
//				{
//					if (aA.value == BitValue.ONE)
//						return new Bit(BitValue.ONE);
//					else
//						return aB;
//				}
//				else if (aB.IsConstant())
//				{
//					if (aB.value == BitValue.ONE)
//						return new Bit(BitValue.ONE);
//					else
//						return aA;
//				}
//				else
//				{
//					return new Bit(BitOperation.OR,aA,aB);
//				}
//
//			case BitOperation.AND :
//				if (aA.IsConstant() && aB.IsConstant())
//				{
//					if (aA.value == BitValue.ONE && aB.value == BitValue.ONE)
//						return new Bit(BitValue.ONE);
//					else
//						return new Bit(BitValue.ZERO);
//				}
//				else if (aA.IsConstant())
//				{
//					if (aA.value == BitValue.ONE)
//						return aB;
//					else
//						return new Bit(BitValue.ZERO);
//				}
//				else if (aB.IsConstant())
//				{
//					if (aB.value == BitValue.ONE)
//						return aA;
//					else
//						return new Bit(BitValue.ZERO);
//				}
//				else
//				{
//					return new Bit(BitOperation.AND,aA,aB);
//				}
//
//			case BitOperation.XOR :
//				if (aA.IsConstant() && aB.IsConstant())
//				{
//					if (aA.value == aB.value)
//						return new Bit(BitValue.ZERO);
//					else
//						return new Bit(BitValue.ONE);
//				}
//				else if (aA.IsConstant())
//				{
//					if (aA.value == BitValue.ONE)
//						return new Bit(BitOperation.NOT,aB);
//					else
//						return aB;
//				}
//				else if (aB.IsConstant())
//				{
//					if (aB.value == BitValue.ONE)
//						return new Bit(BitOperation.NOT,aA);
//					else
//						return aA;
//				}
//				else
//				{
//					return new Bit(BitOperation.XOR,aA,aB);
//				}
//			case BitOperation.NOT :
//				if (aA.IsConstant())
//				{
//					if (aA.value == BitValue.ONE)
//						return new Bit(BitValue.ZERO);
//					else
//						return new Bit(BitValue.ONE);
//				}
//				else
//				{
//					return new Bit(BitOperation.NOT,aA);
//				}
//			}
//
//			Debug.LogError("Bit unrecognized " + aOperation + " operation set A:" + (aA != null) + " B:" + (aB != null));
//			return new Bit(BitValue.UNDEFINED);
//		}
//	}
//
//
//	public class Word
//	{
//		public List<Bit> bits = new List<Bit>();
//
//		public Word()
//		{
//		}
//
//		public Word(int length)
//		{
//			for (int i=0; i<length; i++)
//				bits.Add(null);
//		}
//
//		public Bit this[int k]
//		{
//			get { return bits[k]; }
//			set { bits[k] = value; }
//		}
//
//		public static Word operator^ (Word a, Word b)
//		{
//			Word results = new Word(a.bits.Count);
//
//			for (int i=0; i<a.bits.Count; i++)
//				results[i] = Bit.GetResultBitOptimized(a.bits[i],b.bits[i],Bit.BitOperation.XOR);
//
//			return results;
//		}
//
//		//		public static Word operator+ (Word a, Word b)
//		//		{
//		//			int wL = a.bits.Count;
//		//			Word results = new Word(a.bits.Count);
//		//			Bit carry = null;
//		//
//		//			for (int i=wL-1; i>=0; i--)
//		//			{
//		//				//right weak bit
//		//				if (i == (wL-1))
//		//				{
//		//					results[i] = a[i] ^ b[i];
//		//					carry = a[i] & b[i];
//		//				}
//		//				else
//		//				{
//		//					results[i] =  a[i] ^ b[i] ^ carry;
//		//					carry = (a[i] & b[i]) | (b[i] & carry) | (carry & a[i]);
//		//				}
//		//			}
//		//
//		//			return results;
//		//		}
//
//		public static Word RightRotate(Word a, int shift)
//		{
//			int len = a.bits.Count;
//			Word results = new Word(len);
//
//			for (int i=0; i<len; i++)
//			{
//				int newID = ((i+len) - shift) % len;
//				results[i] = a.bits[newID];
//			}
//
//			return results;
//		}
//
//		public static Word RightShift(Word a, int shift)
//		{
//			int len = a.bits.Count;
//			Word results = new Word(len);
//
//			for (int i=0; i<len; i++)
//			{
//				int newID = i - shift;
//
//				if (i < 0)
//					results[i] = new Bit(Bit.BitValue.ZERO);
//				else
//					results[i] = a.bits[newID];
//			}
//
//			return results;
//		}
//	}
//
//	public class TestSHA : MonoBehaviour {
//
//		// Use this for initialization
//		void Start () {
//			Execute();
//			//			UnitTestAdd();
//		}
//
//		// Update is called once per frame
//		void Update () {
//
//		}
//
//		void Execute()
//		{
//
//			List<Word> msg = new List<Word>();
//			int counter = 0;
//
//			//generate 512 bits chunk
//			for (int i=0; i<16; i++)
//			{
//				msg.Add(new Word(32));
//
//				for (int k=0; k<32; k++)
//				{
//					msg[msg.Count-1][k] = new Bit(counter);
//					counter++;
//				}
//			}
//
//			Word s0 = Word.RightRotate(msg[0],4) ^ Word.RightRotate(msg[0],9);
//
//			//Debug.Log(s0.bits[0].ToString());
//
//			//			for (int i=16; i<64; i++)
//			//			{
//			//			}
//		}
//
//		//		void UnitTestAdd()
//		//		{
//		//			Word A = new Word(16);
//		//
//		//			for (int k=0; k<16; k++)
//		//				A[15-k] = new Bit(((46976832 >> k) & 1) > 0 ? Bit.BitValue.ONE : Bit.BitValue.ZERO);
//		//			
//		//			Word B = new Word(16);
//		//
//		//			for (int k=0; k<16; k++)
//		//				B[15-k] = new Bit(((64986521 >> k) & 1) > 0 ? Bit.BitValue.ONE : Bit.BitValue.ZERO);
//		//
//		//
//		//			Word C = A + B;
//		//
//		//			string readWord = "";
//		//
//		//
//		//			for (int k=0; k<16; k++)
//		//				readWord += (C[k].value == Bit.BitValue.ONE) ? "1" : "0";
//		//
//		//			Debug.Log(readWord + " test");
//		//		}
//	}
//}
