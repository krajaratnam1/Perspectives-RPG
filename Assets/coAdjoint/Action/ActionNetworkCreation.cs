using UnityEngine;
using UnityEditor;
using System.Collections;

public class ActionNetworkCreation : EditorWindow {

	private string newNetworkName = "MyNewNetwork";
	
	public ActionNetworkCreation()
	{
		titleContent = new GUIContent("Add Network to Library");
		
		position = new Rect(200f,200f,300f,75f);
		maxSize = new Vector2(300f,75f);
		minSize = new Vector2(300f,75f);			
	}	
	
	void OnGUI()
	{		
		EditorGUILayout.BeginVertical();
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("Network Name:");
				newNetworkName = EditorGUILayout.TextField(newNetworkName);
			}
			EditorGUILayout.EndHorizontal();
			
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				
				if(GUILayout.Button("Create New Skill Network",GUILayout.ExpandWidth(false)))
				{
					CreateNewNetwork();
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();	
	}	
	
	public void CreateNewNetwork()
	{
		newNetworkName = newNetworkName.Replace(" ",string.Empty);

		ActionEditor.instance.CreateNetwork(newNetworkName);

		this.Close();
	}
}
