using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Idée : 
/// Faire une instal avec la visualisation entière du SHA, nommé "Found the NSA Backdoor"
/// Faire un jeu vidéo de résolution de sha
/// Faire chanter le SHA, afficher les bit par "step" jouer ds instru en fonction operation & hauteur 
/// </summary>
namespace SHATest
{

	public class Bit
	{
		public static long BitCount = 0;
		public static long maxParent = 0;
		public static long maxStep = 0;

		long id = 0;
		public long booleanInstruction = 0;
		public long stepFromMessage = -1;
		public Bit A;
		public Bit B;
		public BitValue value = BitValue.UNDEFINED;
		public BitOperation operation = BitOperation.NONE;
		List<Bit> parents = new List<Bit>();

		void Initialize()
		{
			id = BitCount++;
			if (A != null)
			{
				A.AddParent(this);
				booleanInstruction += A.booleanInstruction;

			}
			if (B != null)
			{
				B.AddParent(this);
				booleanInstruction += B.booleanInstruction;
			}

			ProcessOrGetStep();
			booleanInstruction++;
		}

		void ProcessOrGetStep()
		{
			long step = -1;
			if (A != null)
			{
				step  = A.stepFromMessage;
			}
			if (B != null)
			{
				//MAX
				step  = (A.stepFromMessage > B.stepFromMessage ? A.stepFromMessage : B.stepFromMessage);
			}

			if (step != -1)
				SetStep(++step);
		}

		public void SetStep(long step)
		{
			stepFromMessage = step;

			if (stepFromMessage > maxStep)
				maxStep = stepFromMessage;

			if (A != null && A.stepFromMessage == -1)
				A.SetStep(step-1);
			if (B != null && B.stepFromMessage == -1)
				B.SetStep(step-1);
		}


		public Bit()
		{
			Initialize();
		}

		//is original msg
		public Bit(int aValue)
		{
			stepFromMessage = 0; 
			value = (BitValue)aValue;
			Initialize();
		}

		public Bit(BitValue aValue)
		{
			value = aValue;
			Initialize();
		}

		public Bit(BitOperation aOperation, Bit aA)
		{
			operation = aOperation;
			A = aA;
			Initialize();
		}

		public Bit(BitOperation aOperation, Bit aA, Bit aB)
		{
			operation = aOperation;
			A = aA;
			B = aB;
			Initialize();
		}

		public void SetValue(BitValue val)
		{
			value = val;
		}

		public enum BitValue
		{
			ZERO = 600,
			ONE = 601,
			WHATEVER = 602,
			UNDEFINED = 1000 
		}

		public enum BitOperation
		{
			NONE = 0,
			AND = 1,
			OR = 2,
			XOR = 3,
			NOT = 4,
		}

		public void AddParent(Bit newParent)
		{
			parents.Add(newParent);
			if (parents.Count > Bit.maxParent)
				Bit.maxParent = parents.Count;
		}

		public string GetBitID()
		{
			string result = "";

			if (A != null)
			{
				result += A.GetBitID();
			}
			if (B != null)
			{
				result += "-" + B.GetBitID();
			}

			return id + "(" + result + ")";
		}

		public override string ToString() 
		{
			return ToString(BitOperation.NONE);
		}

		public string ToString(BitOperation parentBitOp) //
		{
			//This bit is a constant
			if (operation == BitOperation.NONE)
				return ValToString(value);

			//This bit is ONE parameter operation ex : (not A)
			if (B == null)
			{
				return OpToString(operation) + A.ToString(operation);
			}

			//This bit is TWO parameter operation ex : (A & B)
			string operationString = A.ToString(operation) + OpToString(operation) + B.ToString(operation);

			if (operation != parentBitOp)
				return "(" + operationString + ")";
			else
				return operationString;
				
		}

		string ValToString(BitValue val)
		{
			if ((int)val < 512)
				return ((int)val).ToString();
			
			switch (val)
			{
			case BitValue.WHATEVER :
				return "W";
			case BitValue.ZERO :
				return "F";
			case BitValue.ONE :
				return "T";
			case BitValue.UNDEFINED :
				return "?";
			}

			return "ERROR";
		}

		string OpToString(BitOperation op)
		{
			switch (op)
			{
			case BitOperation.NONE :
				Debug.LogError("Bit doesn't have operation set A:" + (A != null) + " B:" + (B != null));
				return "Error";
			case BitOperation.OR :
				return "|";
			case BitOperation.AND :
				return "&";
			case BitOperation.XOR :
				return "^";
			case BitOperation.NOT :
				return "!";
			}

			Debug.LogError("Bit doesn't have operaion set A:" + (A != null) + " B:" + (B != null));
			return "ERROR";
		}

		public bool IsConstant()
		{
			return (value == BitValue.ZERO || value == BitValue.ONE);
		}


		public static Bit operator^ (Bit a, Bit b)
		{
			return GetResultBitOptimized(a,b,BitOperation.XOR);
		}

		public static Bit operator& (Bit a, Bit b)
		{
			return GetResultBitOptimized(a,b,BitOperation.AND);
		}

		public static Bit operator| (Bit a, Bit b)
		{
			return GetResultBitOptimized(a,b,BitOperation.OR);
		}

		public static Bit operator! (Bit a)
		{
			return GetResultBitOptimized(a,null,BitOperation.NOT);
		}

		public static Bit GetResultBitOptimized(Bit aA, Bit aB, BitOperation aOperation)
		{
			//already exists ?
			if (aA != null && aB != null)
			{
				foreach (Bit parent in aA.parents)
				{
					if (parent.operation == aOperation &&
						((parent.A == aA  && parent.B == aB) || (parent.A == aB  && parent.B == aA)))
					{
						//Debug.Log("both ok : " + aOperation + " value A : " + aA.value + " value B : " + aB.value);
						return parent;
					}
				}
			}
			else if (aA != null)
			{
				foreach (Bit parent in aA.parents)
				{
					if (parent.operation == aOperation &&
						(parent.A == aA))
					{
						//Debug.Log("one ok : " + aOperation + " value : " + aA.value);
						return parent;
					}
				}
			}

			switch (aOperation)
			{
			case BitOperation.NONE :
				Debug.LogError("Bit doesn't have operation set A:" + (aA != null) + " B:" + (aB != null));
				return new Bit(BitValue.UNDEFINED);
			case BitOperation.OR :
				if (aA.IsConstant() && aB.IsConstant())
				{
					if (aA.value == BitValue.ONE || aB.value == BitValue.ONE)
						return new Bit(BitValue.ONE);
					else
						return new Bit(BitValue.ZERO);
				}
				else if (aA.IsConstant())
				{
					if (aA.value == BitValue.ONE)
						return new Bit(BitValue.ONE);
					else
						return aB;
				}
				else if (aB.IsConstant())
				{
					if (aB.value == BitValue.ONE)
						return new Bit(BitValue.ONE);
					else
						return aA;
				}
				else
				{
					return new Bit(BitOperation.OR,aA,aB);
				}

			case BitOperation.AND :
				if (aA.IsConstant() && aB.IsConstant())
				{
					if (aA.value == BitValue.ONE && aB.value == BitValue.ONE)
						return new Bit(BitValue.ONE);
					else
						return new Bit(BitValue.ZERO);
				}
				else if (aA.IsConstant())
				{
					if (aA.value == BitValue.ONE)
						return aB;
					else
						return new Bit(BitValue.ZERO);
				}
				else if (aB.IsConstant())
				{
					if (aB.value == BitValue.ONE)
						return aA;
					else
						return new Bit(BitValue.ZERO);
				}
				else
				{
					return new Bit(BitOperation.AND,aA,aB);
				}

			case BitOperation.XOR :
				if (aA.IsConstant() && aB.IsConstant())
				{
					if (aA.value == aB.value)
						return new Bit(BitValue.ZERO);
					else
						return new Bit(BitValue.ONE);
				}
				else if (aA.IsConstant())
				{
					if (aA.value == BitValue.ONE)
						return new Bit(BitOperation.NOT,aB);
					else
						return aB;
				}
				else if (aB.IsConstant())
				{
					if (aB.value == BitValue.ONE)
						return new Bit(BitOperation.NOT,aA);
					else
						return aA;
				}
				else
				{
					return new Bit(BitOperation.XOR,aA,aB);
				}
			case BitOperation.NOT :
				if (aA.IsConstant())
				{
					if (aA.value == BitValue.ONE)
						return new Bit(BitValue.ZERO);
					else
						return new Bit(BitValue.ONE);
				}
				else
				{
					return new Bit(BitOperation.NOT,aA);
				}
			}

			Debug.LogError("Bit unrecognized " + aOperation + " operation set A:" + (aA != null) + " B:" + (aB != null));
			return new Bit(BitValue.UNDEFINED);
		}

		public List<Bit> Solve()
		{
			List<Bit> results = new List<Bit>();

			switch (operation)
			{
			case BitOperation.NONE :
				return results;
			case BitOperation.NOT :
				A.value = value == BitValue.ONE ? BitValue.ZERO : BitValue.ONE;
				results.Add(A);
				return results;
			}

			return results;
		}
	}


	public class Word
	{
		//strongest to weakest
		public List<Bit> bits = new List<Bit>();

		public Word()
		{
		}

		public static Word FromInt(uint value)
		{
			Word result = new Word(32);

			for (int k=0; k<32; k++)
				result[31-k] = new Bit(((value >> k) & 1) > 0 ? Bit.BitValue.ONE : Bit.BitValue.ZERO);

			return result;
		}

		public Word(int length)
		{
			for (int i=0; i<length; i++)
				bits.Add(null);
		}

		public Bit this[int k]
		{
			get { return bits[k]; }
			set { bits[k] = value; }
		}

		public static Word operator^ (Word a, Word b)
		{
			Word results = new Word(a.bits.Count);

			for (int i=0; i<a.bits.Count; i++)
				results[i] = a.bits[i] ^ b.bits[i];

			return results;
		}

		public static Word operator! (Word a)
		{
			Word results = new Word(a.bits.Count);

			for (int i=0; i<a.bits.Count; i++)
				results[i] = !a.bits[i];

			return results;
		}

		public static Word operator& (Word a, Word b)
		{
			Word results = new Word(a.bits.Count);

			for (int i=0; i<a.bits.Count; i++)
				results[i] = a.bits[i] & b.bits[i];

			return results;
		}

		public static Word operator+ (Word a, Word b)
		{
			int wL = a.bits.Count;
			Word results = new Word(a.bits.Count);
			Bit carry = null;

			for (int i=wL-1; i>=0; i--)
			{
				//right weak bit
				if (i == (wL-1))
				{
					results[i] = a[i] ^ b[i];
					carry = a[i] & b[i];
				}
				else
				{
					results[i] =  a[i] ^ b[i] ^ carry;
					carry = (a[i] & b[i]) | (b[i] & carry) | (carry & a[i]);
				}
			}

			return results;
		}

		public static Word RightRotate(Word a, int shift)
		{
			int len = a.bits.Count;
			Word results = new Word(len);

			for (int i=0; i<len; i++)
			{
				int newID = ((i+len) - shift) % len;
				results[i] = a.bits[newID];
			}

			return results;
		}

		public static Word RightShift(Word a, int shift)
		{
			int len = a.bits.Count;
			Word results = new Word(len);

			for (int i=0; i<len; i++)
			{
				int newID = i - shift;

				if (newID < 0)
					results[i] = new Bit(Bit.BitValue.ZERO);
				else
					results[i] = a.bits[newID];
			}

			return results;
		}
	}

	public class SHALOL : MonoBehaviour {


		Drawer drawer;

		// Use this for initialization
		void Start () {
			//StartCoroutine(UnitTestReverse());
			drawer = GetComponent<Drawer>();

//			Dictionary<int,int> test = new Dictionary<int,int>();
//			test.Add(0,0);
//			test[0]++;
//			Debug.Log(test[0]);

			StartCoroutine(Execute());
			//StartCoroutine(UnitTestAdd());

		}
		
		// Update is called once per frame
		void Update () {
			
		}

		IEnumerator Execute()
		{

			List<Word> w = new List<Word>();
			int counter = 0;

			//generate 512 bits chunk
			for (int i=0; i<16; i++)
			{
				w.Add(new Word(32));

				for (int j=0; j<32; j++)
				{
					w[w.Count-1][j] = new Bit(counter);
					counter++;
				}
			}

			for (int i=16; i<64; i++)
			{
				Word s0 = Word.RightRotate(w[i-15],7) ^ Word.RightRotate(w[i-15],18) ^ Word.RightShift(w[i-15],3);
				Word s1 = Word.RightRotate(w[i-2],17) ^ Word.RightRotate(w[i-2],19)  ^ Word.RightShift(w[i-2],10);
				w.Add(w[i-16] + s0 + w[i-7] + s1);



//				for (int k=0; k<32; k++)
//				{
//					Debug.Log("bits["+(msg.Count-1)+"]["+k+"]=" + msg[msg.Count-1][k].booleanInstruction.ToString());
				//				}
				Debug.Log(Bit.BitCount);
				yield return null;
				Debug.Log("Max parent : " + Bit.maxParent);
				Debug.Log("Max step : " + Bit.maxStep);
				drawer.ProcessDraw(w);
			}


			Word h0 = Word.FromInt(0x6a09e667);
			Word h1 = Word.FromInt(0xbb67ae85);
			Word h2 = Word.FromInt(0x3c6ef372);
			Word h3 = Word.FromInt(0xa54ff53a);
			Word h4 = Word.FromInt(0x510e527f);
			Word h5 = Word.FromInt(0x9b05688c);
			Word h6 = Word.FromInt(0x1f83d9ab);
			Word h7 = Word.FromInt(0x5be0cd19);

			Word a = Word.FromInt(0x6a09e667);
			Word b = Word.FromInt(0xbb67ae85);
			Word c = Word.FromInt(0x3c6ef372);
			Word d = Word.FromInt(0xa54ff53a);
			Word e = Word.FromInt(0x510e527f);
			Word f = Word.FromInt(0x9b05688c);
			Word g = Word.FromInt(0x1f83d9ab);
			Word h = Word.FromInt(0x5be0cd19);


			Word[] k = new Word[64]  {
				Word.FromInt(0x428A2F98), Word.FromInt(0x71374491), Word.FromInt(0xB5C0FBCF), Word.FromInt(0xE9B5DBA5), Word.FromInt(0x3956C25B), Word.FromInt(0x59F111F1), Word.FromInt(0x923F82A4), Word.FromInt(0xAB1C5ED5),
				Word.FromInt(0xD807AA98), Word.FromInt(0x12835B01), Word.FromInt(0x243185BE), Word.FromInt(0x550C7DC3), Word.FromInt(0x72BE5D74), Word.FromInt(0x80DEB1FE), Word.FromInt(0x9BDC06A7), Word.FromInt(0xC19BF174),
				Word.FromInt(0xE49B69C1), Word.FromInt(0xEFBE4786), Word.FromInt(0x0FC19DC6), Word.FromInt(0x240CA1CC), Word.FromInt(0x2DE92C6F), Word.FromInt(0x4A7484AA), Word.FromInt(0x5CB0A9DC), Word.FromInt(0x76F988DA),
				Word.FromInt(0x983E5152), Word.FromInt(0xA831C66D), Word.FromInt(0xB00327C8), Word.FromInt(0xBF597FC7), Word.FromInt(0xC6E00BF3), Word.FromInt(0xD5A79147), Word.FromInt(0x06CA6351), Word.FromInt(0x14292967),
				Word.FromInt(0x27B70A85), Word.FromInt(0x2E1B2138), Word.FromInt(0x4D2C6DFC), Word.FromInt(0x53380D13), Word.FromInt(0x650A7354), Word.FromInt(0x766A0ABB), Word.FromInt(0x81C2C92E), Word.FromInt(0x92722C85),
				Word.FromInt(0xA2BFE8A1), Word.FromInt(0xA81A664B), Word.FromInt(0xC24B8B70), Word.FromInt(0xC76C51A3), Word.FromInt(0xD192E819), Word.FromInt(0xD6990624), Word.FromInt(0xF40E3585), Word.FromInt(0x106AA070),
				Word.FromInt(0x19A4C116), Word.FromInt(0x1E376C08), Word.FromInt(0x2748774C), Word.FromInt(0x34B0BCB5), Word.FromInt(0x391C0CB3), Word.FromInt(0x4ED8AA4A), Word.FromInt(0x5B9CCA4F), Word.FromInt(0x682E6FF3),
				Word.FromInt(0x748F82EE), Word.FromInt(0x78A5636F), Word.FromInt(0x84C87814), Word.FromInt(0x8CC70208), Word.FromInt(0x90BEFFFA), Word.FromInt(0xA4506CEB), Word.FromInt(0xBEF9A3F7), Word.FromInt(0xC67178F2)};

			//Debug.Log(s0.bits[0].ToString());

			for (int i=0; i<64; i++)
			{
				Word S1 = Word.RightRotate(e,6) ^ Word.RightRotate(e,11) ^ Word.RightRotate(e,25);
				Word ch = (e & f) ^ ((!e) & g);
				Word temp1 = h + S1 + ch + k[i] + w[i];
				Word S0 = Word.RightRotate(a,2) ^ Word.RightRotate(a,13) ^ Word.RightRotate(a,22);
				Word maj = (a & b) ^ (a & c) ^ (b & c);
				Word temp2 = S0 + maj;

				h = g;
				g = f;
				f = e;
				e = d + temp1;
				d = c;
				c = b;
				b = a;
				a = temp1 + temp2;

				Debug.Log(Bit.BitCount);
				yield return null;
				Debug.Log("Max parent : " + Bit.maxParent);
				Debug.Log("Max step : " + Bit.maxStep);
			}

			List<Word> result = new List<Word>(){
				h0 + a,
				h1 + b,
				h2 + c,
				h3 + d,
				h4 + e,
				h5 + f,
				h6 + g,
				h7 + h,
			};

			result.InsertRange(0,w);
			drawer.ProcessDraw(result);
		}

		IEnumerator UnitTestAdd()
		{
			Word A = new Word(32);

			for (int k=0; k<32; k++)
				A[31-k] = new Bit(((46976832 >> k) & 1) > 0 ? Bit.BitValue.ONE : Bit.BitValue.ZERO);

			Word B = new Word(32);

			for (int k=0; k<32; k++)
				B[31-k] = new Bit(((64986521 >> k) & 1) > 0 ? Bit.BitValue.ONE : Bit.BitValue.ZERO);


			Word C = A + B;

			string readWord = "";


			for (int k=0; k<32; k++)
				readWord += (C[k].value == Bit.BitValue.ONE) ? "1" : "0";

			Debug.Log(readWord + " test");
			Debug.Log(Bit.BitCount);

			yield break;
		}

		IEnumerator UnitTestOpAndRendundance()
		{
			Word A = new Word(16);

			for (int k=0; k<16; k++)
				A[15-k] = new Bit(k);

			Word B = new Word(16);

			for (int k=0; k<16; k++)
				B[15-k] = new Bit(k+16);


			Word C = A + B;

			string readWord = "";


			for (int k=0; k<16; k++)
				readWord += (C[k].value == Bit.BitValue.ONE) ? "1" : "0";

			Debug.Log(readWord + " test");
			Debug.Log(Bit.BitCount);

			for (int i=0; i<16; i++)
			{
				Debug.Log(C[15-i]);
				Debug.Log(C[15-i].GetBitID());
				yield return null;
			}
		}

		IEnumerator UnitTestReverse()
		{
			Word A = new Word(16);

			for (int k=0; k<16; k++)
				A[15-k] = new Bit(k);

			Word B = new Word(16);

			for (int k=0; k<16; k++)
				B[15-k] = new Bit(((64986521 >> k) & 1) > 0 ? Bit.BitValue.ONE : Bit.BitValue.ZERO);


			Word C = A + B;

			string readWord = "";


			for (int k=0; k<16; k++)
				readWord += (C[k].value == Bit.BitValue.ONE) ? "1" : "0";

			Debug.Log(readWord + " test");
			Debug.Log(Bit.BitCount);

			yield return Reverse(C,111963353);

//			for (int i=0; i<16; i++)
//			{
//				Debug.Log(C[15-i]);
//				Debug.Log(C[15-i].GetBitID());
//				Debug.Log(C[15-i].booleanInstruction);
//				yield return null;
//			}
		}

		IEnumerator Reverse(Word w, int data)
		{
			int len = w.bits.Count;
			List<Bit.BitValue> values = new List<Bit.BitValue>();

			for (int k=0; k<len; k++)
				values.Add(((data >> k) & 1) > 0 ? Bit.BitValue.ONE : Bit.BitValue.ZERO);

			Reverser reverser = new Reverser();

			yield return  reverser.Go(w.bits,values);
		}
	}
}
