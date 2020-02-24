using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ActionSkill
{
	public string Name;
	
	public ActionNetwork owner
	{
		get 
		{
			return ActionEditor.instance.currentLibraryNetwork;
		}
	}
	
	private bool _IsDirty;
	public bool IsDirty
	{
		get
		{
			return _Position.IsDirty || _IsDirty;
		}
		set
		{
			_IsDirty = value;
			if (!value)
			{
				_Position.IsDirty = false;
			}
		}
	}
	
	private EditorTwoVector _Position = new EditorTwoVector();	
	public EditorTwoVector Position
	{
		get
		{
			return _Position;
		}
		set
		{
			if (_Position.Equals(value))
			{
				return;
			}
			_Position = new EditorTwoVector(value);
			IsDirty = true;
		}
	}
	
	public double Q;
	public double B;
	public double v;
	
	private float minWidth;
	private float maxWidth;	
	
	private const float skillBoxWidth = 100f;
	private const float skillBoxHeight = 82.5f;
	
	[NonSerializedAttribute]
	public Rect drawPos;
	
	[Obsolete("For XML Serialization Only", true)]
	public ActionSkill(){}
	
	public ActionSkill(string Name, ActionNetwork myOwner)
	{
		this.Name = Name;
	}
	
	public ActionSkill(string Name, double _Q, double _B, double _v)
	{
		this.Name = Name;

		this.Q = _Q;
		this.B = _B;
		this.v = _v;
	}
	
	public ActionSkill(string Name, double _Q, double _B, double _v, EditorTwoVector position)
	{
		this.Name = Name;

		this.Q = _Q;
		this.B = _B;
		this.v = _v;
		
		Position = new EditorTwoVector(position);
	}	
	
	public void DeltaMove (Vector2 delta)
	{
		Position.X += delta.x;
		Position.Y += delta.y;
	}	
		
	private static GUIStyle WindowStyle {
		get 
		{
			GUIStyle sw = new GUIStyle(
				EditorGUIUtility.GetBuiltinSkin(EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector).window);

			if(!EditorGUIUtility.isProSkin)
				sw.normal.textColor = Color.white;

			return sw;
		}
	}	
	
	public void Draw(bool selected,  bool beingDrawn)
	{
		GUI.skin.box.alignment = TextAnchor.UpperLeft;

		if(beingDrawn)
		{
			Color halfAlpha = GUI.color;
			halfAlpha.a = 0.5f;
			GUI.color = halfAlpha;
			Position.X = Event.current.mousePosition.x;
			Position.Y = Event.current.mousePosition.y;
			
			drawPos = new Rect(Position.X,Position.Y,skillBoxWidth,skillBoxHeight);	
			
			halfAlpha.a = 1f;
			GUI.color = halfAlpha;
		}
		else
		{
			drawPos = new Rect(Position.X + ActionNetworkEditor.instance.offset.X,Position.Y + ActionNetworkEditor.instance.offset.Y,skillBoxWidth,skillBoxHeight);
		}
		
		if(selected)
		{	
			//GUI.backgroundColor = Color.gray;
			GUI.backgroundColor = (new Color(0.4f,0.9f,0.9f,1f));
			//GUI.backgroundColor = new Color(1f,120f/255f,0f,2f);
		}
		else if(!EditorGUIUtility.isProSkin)
		{
			GUI.backgroundColor = Color.gray;
		}
				
		var windowName = new GUIContent (Name);
			
		var largestNumberCharLen = ((Math.Max(Q, Math.Max(B,v))).ToString() + "Q: ");
		
		if(Name.Length > largestNumberCharLen.Length)
			WindowStyle.CalcMinMaxWidth(windowName, out minWidth, out maxWidth);
		else
			WindowStyle.CalcMinMaxWidth(new GUIContent(largestNumberCharLen), out minWidth, out maxWidth);
		
		if(minWidth > 0)
			drawPos.width = maxWidth > drawPos.width ? maxWidth : drawPos.width;

		GUI.Box (drawPos, windowName, WindowStyle);

		Color currentColor = GUI.skin.label.normal.textColor;

		if(!EditorGUIUtility.isProSkin)
			GUI.skin.label.normal.textColor = Color.white;

		//Draw Q,B,v and values
		if (!beingDrawn) {
			GUI.skin.label.alignment = TextAnchor.UpperLeft;

			GUI.Label(new Rect(Position.X + ActionNetworkEditor.instance.offset.X + 1f,Position.Y + ActionNetworkEditor.instance.offset.Y + 20f,30f,30f),"Q:");
			GUI.Label(new Rect(Position.X + ActionNetworkEditor.instance.offset.X + 1f,Position.Y + ActionNetworkEditor.instance.offset.Y + 40f,30f,30f),"B:");		
			GUI.Label(new Rect(Position.X + ActionNetworkEditor.instance.offset.X + 1f,Position.Y + ActionNetworkEditor.instance.offset.Y + 60f,30f,30f),"v:");
			
			GUI.skin.label.alignment = TextAnchor.UpperRight;
			
			GUI.Label(new Rect(Position.X + ActionNetworkEditor.instance.offset.X,Position.Y + ActionNetworkEditor.instance.offset.Y + 20f,drawPos.width - 4f,30f),Q.ToString());
			GUI.Label(new Rect(Position.X + ActionNetworkEditor.instance.offset.X,Position.Y + ActionNetworkEditor.instance.offset.Y + 40f,drawPos.width - 4f,30f),B.ToString());		
			GUI.Label(new Rect(Position.X + ActionNetworkEditor.instance.offset.X,Position.Y + ActionNetworkEditor.instance.offset.Y + 60f,drawPos.width - 4f,30f),v.ToString());
		}
		
		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.skin.label.normal.textColor = currentColor;
		GUI.backgroundColor = Color.white;		
	}
	
	public bool Contains(Vector2 position)
	{		
		return drawPos.Contains(position);
	}
}
