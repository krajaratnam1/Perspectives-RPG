using UnityEngine;
using UnityEditor;
using System;

public class ActionSkillPreview : EditorWindow {
	
    public static Texture2D aaLineTex = null;
    public static Texture2D lineTex = null;	
	
	public ActionSkill currentSkill;
	
	private static ActionSkillPreview _instance;
	public static ActionSkillPreview instance
	{
		get
		{
			if(_instance == null)
			{
				GetWindow(typeof(ActionSkillPreview),true);
			}
			
			return _instance;
		}
	}
	
	public void Update()
	{
		Repaint();
	}
		
	public ActionSkillPreview()
	{
		hideFlags = HideFlags.DontSave;
		
		if (_instance != null)
		{
			Debug.LogError ("Instance already exists");
			DestroyImmediate (this);
			return;
		}
		
		_instance = this;
		
		titleContent = new GUIContent("Skill preview");
		
		position = new Rect(600f,300f,250f,200f);
		
		minSize = new Vector2(250f,190f);
		maxSize = new Vector2(3000f,190f);		
		
	}

	static Vector2 offset = new Vector2(30f,30f);
	
	void OnGUI()
	{		
		GUI.DrawTexture(new Rect(0f,0f,Screen.width,Screen.height),ActionResources.line,ScaleMode.StretchToFill);
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			GUI.contentColor = Color.blue;
			GUILayout.Label(currentSkill.Name,new GUILayoutOption[]
			{
			});
			GUI.contentColor = Color.white;
			
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
		
		DrawLine(new Vector2(offset.x,Screen.height-offset.y),new Vector2(Screen.width-offset.x,Screen.height-offset.y),Color.black,2f,false);
		DrawLine(new Vector2(offset.x,Screen.height-offset.y),new Vector2(offset.x,offset.y),Color.black,2f,false);				
		
		Handles.color = Color.blue;
		
		if(ActionNetworkEditor.instance != null & ActionNetworkEditor.instance.selectedSkills.Count > 0)
		{
			ActionSkill curSkil = currentSkill;			
			HandlesDrawCurve(curSkil.Q,curSkil.B,curSkil.v,100);						
		}
		
		int labelNo = (int)((Screen.width-2f*offset.x)/50f);

		Color curCol = GUI.skin.label.normal.textColor;

		GUI.skin.label.normal.textColor = Color.black;

		for(int i = 0; i < labelNo+1; ++i)
		{
			GUI.Label(new Rect(offset.x + 50f*i-5f,Screen.height-22.5f,25f,20f),(50*i).ToString());
		}

		GUI.skin.label.normal.textColor = curCol;
		
	}
	
	public static void HandlesDrawCurve(double Q, double B, double v, int segments)
	{
		Vector3 fro = new Vector3(offset.x, Screen.height-offset.y);
		
		for(int i = 1; i < segments;++i)
		{
			float x = offset.x + (float)i*(Screen.width-2f*offset.x)/(float)segments;
			float y = Screen.height-offset.y;
			
			y-= (float)Level(Q,B,v,(double)x);
			
			Vector3 to = new Vector3(x,y);
			
			Handles.DrawLine(fro,to);
			
			fro = to;
		}		
	}

	public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
    {
        Color savedColor = GUI.color;
        Matrix4x4 savedMatrix = GUI.matrix;
        
        if (!lineTex)
        {
            lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, true);
            lineTex.SetPixel(0, 1, Color.white);
            lineTex.Apply();
        }
        if (!aaLineTex)
        {
            aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, true);
            aaLineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
            aaLineTex.SetPixel(0, 1, Color.white);
            aaLineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
            aaLineTex.Apply();
        }
        if (antiAlias) width *= 3;
        float angle = Vector3.Angle(pointB - pointA, Vector2.right) * (pointA.y <= pointB.y?1:-1);
        float m = (pointB - pointA).magnitude;
        if (m > 0.001f)
        {
            Vector3 dz = new Vector3(pointA.x, pointA.y, 0);

            GUI.color = color;
            GUI.matrix = translationMatrix(dz) * GUI.matrix;
            GUIUtility.ScaleAroundPivot(new Vector2(m, width), new Vector3(-0.5f, 0, 0));
            GUI.matrix = translationMatrix(-dz) * GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, Vector2.zero);
            GUI.matrix = translationMatrix(dz + new Vector3(width / 2, -m / 2) * Mathf.Sin(angle * Mathf.Deg2Rad)) * GUI.matrix;

            if (!antiAlias)
                GUI.DrawTexture(new Rect(0, 0, 1, 1), lineTex);
            else
                GUI.DrawTexture(new Rect(0, 0, 1, 1), aaLineTex);
        }
        GUI.matrix = savedMatrix;
        GUI.color = savedColor;
    }
	
	public static void LevelCurve(double Q, double B, double v, int segments, float start, float end, Color color, float width, bool antiAlias)
	{		
		Vector2 last = new Vector2(0f,(float)Level(Q,B,v,start));
		
        for (int i = 1; i <= segments; ++i)
        {
			double x = start + (end-start)*(double)(i/segments);
            Vector2 pos = new Vector2((float)x*(Screen.width-40f)/end,(float)Level (Q,B,v, x)*(Screen.height-40f)/100f);
					
			//Vector2 pos = new Vector2((float)x*(Screen.width-40f)/end,-10f*Xsquared((float)x));
			
            DrawLine(
                last,
                pos,
                color, width, antiAlias);
            last = pos;
        }		
	}
			
	private static double Level(double Q, double B, double v, double x)
	{
		return (100*Math.Pow((1+Q*Math.Exp(-B*x)),(1/v))-100*Math.Pow((1+Q),(1/v)))/((1-Math.Pow((1+Q),(1/v)))*Math.Pow((1+Q*Math.Exp(-B*x)),(1/v)));

	}

    private static Matrix4x4 translationMatrix(Vector3 v)
    {
        return Matrix4x4.TRS(v,Quaternion.identity,Vector3.one);
    }	
	
	
}
