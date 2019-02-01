﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SHATest;
using System.IO;
using System;
using System.Linq;


public class Vector2Int
{
	public int x;
	public int y;

	public Vector2Int(int a, int b)
	{
		x = a;
		y = b;
	}

	public static Vector2 operator+ (Vector2 a, Vector2Int b)
	{
		return new Vector2(a.x+b.x,a.y+b.y);
	}

	public static implicit operator Vector2Int(Vector2 a)
	{
		return new Vector2Int((int)a.x,(int)a.y);
	}

	public static implicit operator Vector2(Vector2Int a)
	{
		return new Vector2(a.x,a.y);
	}


}

public class DrawnBit
{
	public Bit bit;
	public Vector2Int pos;


	public DrawnBit()
	{
	}

	public DrawnBit(Bit aBit, int aX, int aY)
	{
		bit = aBit;
		pos = new Vector2Int(aX,aY);
	}
}

public class Drawer : MonoBehaviour {


	//List<DrawnBit> lastDrawnBits;
	Dictionary<Bit,DrawnBit> lastDrawnBits = new Dictionary<Bit, DrawnBit>();
	//List<int> depthStack = new List<int>();
	Dictionary<int,int> depthStack = new Dictionary<int,int>();
	int recursiveCall = 0;

	List<Vector3> none_Line = new List<Vector3>();
	List<Vector3> and_Line = new List<Vector3>();
	List<Vector3> or_Line = new List<Vector3>();
	List<Vector3> xor_Line = new List<Vector3>();
	List<Vector3> not_Line = new List<Vector3>();

	public List<AudioSource> sounds;

	public bool sing = true;
	public bool drawDebug = true;
	public bool drawGLLine = true;
	public Material mat;

	public Color noneCol;
	public Color andCol;
	public Color orCol;
	public Color xorCol;
	public Color notCol;
	public float alpha = 0.5f;

	private bool m_screenShotLock = false;

	int drawStepCount = -1;
	// Use this for initialization
	void Start () {
		noneCol.a = alpha;
		andCol.a = alpha;
		orCol.a = alpha;
		xorCol.a = alpha;
		notCol.a = alpha;
	}
	
	// Update is called once per frame
	void Update () {
		noneCol.a = alpha;
		andCol.a = alpha;
		orCol.a = alpha;
		xorCol.a = alpha;
		notCol.a = alpha;
		//drawStepCount = (drawStepCount+1) % 1480;

		if (sing)
			drawStepCount = (int)(Time.time*10.0f);
		Draw();



		if (Input.GetKeyDown(KeyCode.S) && !m_screenShotLock)
		{
			Debug.Log(Application.persistentDataPath);
			Debug.Log("screenshot");
			m_screenShotLock = true;
			StartCoroutine(TakeScreenShotCo());
		}
	}

	private IEnumerator TakeScreenShotCo()
	{
		yield return new WaitForEndOfFrame();

		var directory = new DirectoryInfo(Application.dataPath);
		var path = Path.Combine(directory.Parent.FullName, string.Format("Screenshot_{0}.png", DateTime.Now.ToString("yyyyMMdd_Hmmss")));
		Debug.Log("Taking screenshot to " + path);
		Application.CaptureScreenshot(path,4);
		m_screenShotLock = false;
	}

	void MakeSounds(List<DrawnBit> dBits)
	{
		for (int i=0; i<sounds.Count;i++)
			sounds[i].volume = 0f;

		//dBits = dBits.OrderByDescending( val => val.pos.y).ToList();
		//dBits = dBits.OrderByDescending( val => val.pos.x).ToList();

		for (int i=0; i<Math.Min(dBits.Count,sounds.Count);i++)
		{
			sounds[i].volume = 0.2f;
			sounds[i].pitch = dBits[i].pos.y *0.3f + 0.3f;
			//sounds[i].volume *= (5f - sounds[i].pitch);
			//sounds[i].volume /=  Mathf.Max(0.5f,sounds[i].pitch);
		}


//		int test = dBits.Count / sounds.Count;
//		test = Mathf.Max(test,1);
//
//		for (int i=0; i<Math.Min(dBits.Count,sounds.Count); i++)
//		{
//			sounds[i].volume = 0.3f;
//			sounds[i].pitch = dBits[test*i].pos.y *0.1f + 0.3f;
//
////			if (i==0)
////				Debug.Log(dBits[test*i].pos.y);
//		}
	}

	void Draw()
	{
		List<DrawnBit> actuallyDrawnBit = new List<DrawnBit>();

		none_Line.Clear();
		and_Line.Clear();
		or_Line.Clear();
		xor_Line.Clear();
		not_Line.Clear();

		if (depthStack.Count > 0)
		{
			int counter = 0;
			foreach (KeyValuePair<Bit,DrawnBit> kvp in lastDrawnBits)
			{
				counter++;
//				if ((counter % 10) != 1)
//					continue;

				Bit bit = kvp.Key;
				DrawnBit dBit = kvp.Value;

				if (drawStepCount != -1)
				{
					if (bit.stepFromMessage != drawStepCount)
						continue;
					actuallyDrawnBit.Add(dBit);
				}

				actuallyDrawnBit.Add(dBit);

				int childCount = bit.A != null ? 1 : 0;
				childCount += bit.B != null ? 1 : 0;

				if (childCount > 0)
				{
					Vector2 bitPos = new Vector2(dBit.pos.x,dBit.pos.y);

					if (childCount == 1)
					{
						Vector2 a = lastDrawnBits[bit.A].pos + new Vector2(0.4f,0.0f);
						Vector2 b = dBit.pos + new Vector2(-0.4f,0.0f);
						DrawLine(lastDrawnBits[bit.A].pos,a,bit.operation);
						DrawLine(a,b,bit.operation);
						DrawLine(b,dBit.pos,bit.operation);
					}
					else
					{
						Vector2 a = lastDrawnBits[bit.A].pos + new Vector2(0.4f,0.0f);
						Vector2 b = dBit.pos + new Vector2(-0.4f,0.04f);
						DrawLine(lastDrawnBits[bit.A].pos,a,bit.operation);
						DrawLine(a,b,bit.operation);
						DrawLine(b,dBit.pos,bit.operation);

						a = lastDrawnBits[bit.B].pos + new Vector2(0.4f,0.0f);
						b = dBit.pos + new Vector2(-0.4f,-0.04f);
						DrawLine(lastDrawnBits[bit.B].pos,a,bit.operation);
						DrawLine(a,b,bit.operation);
						DrawLine(b,dBit.pos,bit.operation);
					}
				}
			}
			if (drawStepCount != -1 && sing)
			{
				MakeSounds(actuallyDrawnBit);
			}
		}
	}

	void DrawLine(Vector2 posA, Vector2 posB, Bit.BitOperation operation)
	{
		if (drawDebug)
		{
			Color col = Color.white;
			switch (operation)
			{
			case Bit.BitOperation.NONE:
				col = noneCol;
				break;
			case Bit.BitOperation.AND:
				col = andCol;
				break;
			case Bit.BitOperation.OR:
				col = orCol;
				break;
			case Bit.BitOperation.XOR:
				col = xorCol;
				break;
			case Bit.BitOperation.NOT:
				col = notCol;
				break;
			}

			col.a = 0.5f;
			Debug.DrawLine(
				new Vector3(posA.x,posA.y*10.0f,0f),
				new Vector3(posB.x,posB.y*10.0f,0f),
				col);
		}
		if (drawGLLine)
		{
			List<Vector3> list = null;

			switch (operation)
			{
			case Bit.BitOperation.NONE:
				list = none_Line;
				break;
			case Bit.BitOperation.AND:
				list = and_Line;
				break;
			case Bit.BitOperation.OR:
				list = or_Line;
				break;
			case Bit.BitOperation.XOR:
				list = xor_Line;
				break;
			case Bit.BitOperation.NOT:
				list = not_Line;
				break;
			}

			list.Add(new Vector3(posA.x,posA.y*10.0f,0f));
			list.Add(new Vector3(posB.x,posB.y*10.0f,0f));
		}
	}


	void OnPostRender()
	{
		if (drawGLLine)
		{
			
			if (!mat)
			{
				Debug.LogError("Please Assign a material on the inspector");
				return;
			}
			GL.PushMatrix();
			GL.LoadProjectionMatrix(GetComponent<Camera>().projectionMatrix);
			GL.modelview = GetComponent<Camera>().worldToCameraMatrix;
			mat.SetPass(0);
			GL.Begin(GL.LINES);


			GL.Color(noneCol);
			foreach (Vector3 vec in none_Line)
				GL.Vertex(vec);
			
			GL.Color(andCol);
			foreach (Vector3 vec in and_Line)
				GL.Vertex(vec);
			
			GL.Color(orCol);
			foreach (Vector3 vec in or_Line)
				GL.Vertex(vec);
			
			GL.Color(xorCol);
			foreach (Vector3 vec in xor_Line)
				GL.Vertex(vec);
			
			GL.Color(notCol);
			foreach (Vector3 vec in not_Line)
				GL.Vertex(vec);


			GL.End();

			GL.PopMatrix();
		}
	}

	public void ProcessDraw(List<Word> words)
	{
		int depth = 0;

		lastDrawnBits.Clear();
		depthStack.Clear();
		recursiveCall = 0;

		for (int i=0; i<words.Count; i++)
		{
			Word w = words[i];
			for (int k=w.bits.Count-1; k>=0; k--)
			{
				depthStack.Add(depth,10);

				DrawnBit newDrawn = new DrawnBit(w[k],depth,0);
				lastDrawnBits.Add(w[k],newDrawn);
				depth+=2;
			}
		}

		for (int i=0; i<words.Count; i++)
		{
			Word w = words[i];
			for (int k=w.bits.Count-1; k>=0; k--)
			{
				DrawBitChild(w[k]);
			}
		}
		Debug.Log("done draw with recursive : " + recursiveCall);

		int maxX = 0;
		int maxY = 0;

		foreach (KeyValuePair<int,int> kv in depthStack)
		{
			maxX = Mathf.Max(maxX,kv.Key);
			maxY = Mathf.Max(maxY,kv.Value);
		}
		Debug.Log("size " + maxX + "," + maxY);
	}

	void DrawBitChild(Bit bit)
	{
		if (bit.A != null)
			DrawBit(bit.A);
		if (bit.B != null)
			DrawBit(bit.B);
	}

	DrawnBit DrawBit(Bit bit)
	{
		recursiveCall++;

		if (lastDrawnBits.ContainsKey(bit))
			return lastDrawnBits[bit];

		int positionX = 0;
		if (bit.A != null)
		{
			DrawnBit dA = DrawBit(bit.A);
			positionX = dA.pos.x;
		}
		if (bit.B != null)
		{
			DrawnBit dB = DrawBit(bit.B);
			positionX = Mathf.Max(dB.pos.x,positionX);
		}

		positionX++;
		if (!depthStack.ContainsKey(positionX))
			depthStack.Add(positionX,10);
		else
			depthStack[positionX]++;

		DrawnBit drawnBit = new DrawnBit(bit,positionX,depthStack[positionX]);
		lastDrawnBits.Add(bit,drawnBit);

		return drawnBit;
	}
}
