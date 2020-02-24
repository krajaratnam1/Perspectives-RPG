using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class ActionSkillCreation : EditorWindow {
	
	private string newSkillName = String.Empty;
	
	private string Q = "0";
	private string B = "0";
	private string v = "0";
	
	private bool init = false;
	
	public ActionSkillCreation()
	{
		titleContent = new GUIContent("Add Skill to Network");
		
		position = new Rect(200f,200f,250f,135f);
		maxSize = new Vector2(250f,135f);
		minSize = new Vector2(250f,135f);	
	}	

	void OnGUI()
	{		
		if(!init)
		{			
			int i = 0;
			
			string copy = "NewEmptySkill";
				
			if(ActionEditor.instance.currentLibraryNetwork.Get(copy) != null)
			{				
				while(ActionEditor.instance.currentLibraryNetwork.Get(copy + i) != null)
				{
					++i;
				}
					
				copy += i;
			}	
			
			newSkillName = copy;
			
			init = true;
		}
		
		EditorGUILayout.BeginVertical();
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("Skill Name:");
				newSkillName = EditorGUILayout.TextField(newSkillName);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("Q:");
				Q = EditorGUILayout.TextField(Q);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("B:");
				B = EditorGUILayout.TextField(B);
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("v:");
				v = EditorGUILayout.TextField(v);
			}
			EditorGUILayout.EndHorizontal();			
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				
				if(GUILayout.Button("Create New Skill",GUILayout.ExpandWidth(false)) || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
				{
					double skillQ = 10;
					double skillB = 10;
					double skillV = 10;
					
					if(!double.TryParse(Q,out skillQ) || !double.TryParse(B, out skillB) || ! double.TryParse(v, out skillV))
					{
						EditorUtility.DisplayDialog("Error","A skill parameter is incorrectly formatted.","Okay");
					}
					else if(ActionEditor.instance.currentLibraryNetwork.Get(newSkillName) != null)
					{
						EditorUtility.DisplayDialog("Error",ActionEditor.instance.currentLibraryNetwork.name + " already contains a skill called '" + newSkillName + "'" +
							". Please choose a different name.","Okay");
					}
					else
					{						 
						ActionSkill newSkill = ActionNetworkEditor.instance.skillBeingDrawn =  ActionEditor.instance.currentLibraryNetwork.AddSkill(newSkillName.Replace(" ", string.Empty),skillQ,skillB,skillV, new EditorTwoVector(Event.current.mousePosition.x, Event.current.mousePosition.y));
						ActionLibraryExplorer.instance.highlightedItemType = ActionLibraryExplorer.ItemType.ActionSkill;
						ActionLibraryExplorer.instance.highlightedSkill = newSkill;
						
						ActionNetworkEditor.instance.positioningSkill = true;						
						
						this.Close();
						
						ActionEditor.instance.SaveLibrary();
						
						ActionNetworkEditor.instance.creatingNewSkill = false;
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();	
	}	
	
}
