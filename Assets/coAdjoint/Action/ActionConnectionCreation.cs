using UnityEngine;
using UnityEditor;
using System.Collections;

public class ActionConnectionCreation : EditorWindow {
	
	private string skill1;
	private string skill2;
	private string skill1Toskill2 = "1";
	private string skill2Toskill1 = "1";
	private double s1s2Value;
	private double s2s1Value;
	
	public ActionConnectionCreation()
	{
		skill1 = ActionNetworkEditor.instance.skillsToConnect[0].Name;
		skill2 = ActionNetworkEditor.instance.skillsToConnect[1].Name;
		
		titleContent = new GUIContent("Create connection between " + skill1 + " and " + skill2);

		position = new Rect(200f,200f,350f,125f);
		maxSize = new Vector2(600f,125f);
		minSize = new Vector2(350f,125f);		
	}
	
	void OnGUI()
	{
		EditorGUILayout.BeginVertical();
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Define the connection strengths from:",new GUILayoutOption[]
				{
					GUILayout.Width(this.maxSize.x)
				});
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();			
			
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField(skill1 + " to " + skill2,new GUILayoutOption[]
				{
					GUILayout.Width(200f)
				});
				GUILayout.FlexibleSpace();
				skill1Toskill2 = EditorGUILayout.TextField(skill1Toskill2, new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
			}
			EditorGUILayout.EndHorizontal();
			
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField(skill2 + " to " + skill1,new GUILayoutOption[]
				{
					GUILayout.Width(200f)
				});
				GUILayout.FlexibleSpace();
				skill2Toskill1 = EditorGUILayout.TextField(skill2Toskill1, new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
			}
			EditorGUILayout.EndHorizontal();
			
			GUILayout.FlexibleSpace();			
			
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				
				if(GUILayout.Button("Create New Skill Connection",GUILayout.ExpandWidth(false)) || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
				{					
					if(!double.TryParse(skill1Toskill2,out s1s2Value) || !double.TryParse(skill2Toskill1, out s2s1Value) || (s1s2Value == 0 && s2s1Value == 0))
					{
						EditorUtility.DisplayDialog("Error","A connection strength value is incorrectly formatted, or both strengths are zero.","Okay");
					}					
					else CreateConnection();
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();		
	}
	
	void OnDestroy()
	{
		ActionNetworkEditor.instance.skillsToConnect.Clear();		
	}
	
	private void CreateConnection()
	{		
		ActionNetworkEditor.instance.selectedConnection = ActionEditor.instance.currentLibraryNetwork.connections.IndexOf(ActionEditor.instance.currentLibraryNetwork.AddConnection(ActionNetworkEditor.instance.skillsToConnect[0],ActionNetworkEditor.instance.skillsToConnect[1],s1s2Value,s2s1Value));
		ActionNetworkEditor.instance.skillsToConnect.Clear();	
		
		ActionNetworkEditor.instance.currentContent = ActionNetworkEditor.instance.empty;

		ActionEditor.instance.SaveLibrary();
		this.Close();
	}
}
