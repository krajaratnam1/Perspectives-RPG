using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ActionNetworkEditor))]
public class ActionNetworkEditorInspector : Editor {

	new public void Repaint()
	{
		base.Repaint();
	}
	
	public override void OnInspectorGUI ()
	{	
		if (ActionEditor.instance == null || ActionEditor.instance.currentAssetLibrary == null || ActionEditor.instance.currentLibraryNetwork == null)
		{
			GUILayout.Label("No Action Network currently loaded, please selected an Action Library from the project view" +
			 	" and press 'Edit Library' in the inspector.",ActionResources.inspectorLabel, new GUILayoutOption[0]);
			return;
		}
		else
		{
			GUILayout.Space(15f);
						
			GUILayout.BeginHorizontal("toolbar",new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			{
				GUILayout.Label("Action Skill Editor",ActionResources.boldLabel, new GUILayoutOption[0]);
				
			} GUILayout.EndHorizontal();
			
			GUILayout.BeginVertical();
			{
				if(ActionNetworkEditor.instance.selectedConnection.HasValue)
				{
					ActionConnection actC = ActionEditor.instance.currentLibraryNetwork.connections[ActionNetworkEditor.instance.selectedConnection.Value];
					
					GUILayout.Space(2.5f);
					GUILayout.Label(actC.From.Name + "-" + actC.To.Name + " Connection",ActionResources.boldLabel, new GUILayoutOption[0]);
					GUILayout.Space(2.5f);
					
					GUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField(actC.From.Name + " to " + actC.To.Name + ":");
						
						double.TryParse(EditorGUILayout.TextField(actC.fromValue.ToString()),out actC.fromValue);

//						if(actC.fromValue == 0 && actC.toValue == 0)
//						{
//							ActionConnection connectionToDelete = ActionEditor.instance.currentLibraryNetwork.connections
//								[ActionNetworkEditor.instance.selectedConnection.Value];
//							
//							ActionNetworkEditor.instance.selectedConnection = null;
//							
//							ActionEditor.instance.currentLibraryNetwork.connections.Remove(connectionToDelete);
//							
//							ActionEditor.instance.SaveLibrary();
//						}
						
					}GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField(actC.To.Name + " to " + actC.From.Name + ":");
						
						double.TryParse(EditorGUILayout.TextField(actC.toValue.ToString()),out actC.toValue);

//						if(actC.fromValue == 0 && actC.toValue == 0)
//						{
//							ActionConnection connectionToDelete = ActionEditor.instance.currentLibraryNetwork.connections
//								[ActionNetworkEditor.instance.selectedConnection.Value];
//							
//							ActionNetworkEditor.instance.selectedConnection = null;
//							
//							ActionEditor.instance.currentLibraryNetwork.connections.Remove(connectionToDelete);
//							
//							ActionEditor.instance.SaveLibrary();
//						}
						
					}GUILayout.EndHorizontal();
					
					GUILayout.Space(2.5f);	
					
					GUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();
						
						if(GUILayout.Button("Delete Connection"))
						{
							ActionConnection connectionToDelete = ActionEditor.instance.currentLibraryNetwork.connections
								[ActionNetworkEditor.instance.selectedConnection.Value];
							
							ActionNetworkEditor.instance.selectedConnection = null;
				
							ActionEditor.instance.currentLibraryNetwork.connections.Remove(connectionToDelete);
							
							ActionEditor.instance.SaveLibrary();
						}
						
						GUILayout.Space(10f);
						
					} GUILayout.EndHorizontal();
					
					ActionResources.ActionSeparator();
					
				}
				
				foreach(int i in ActionNetworkEditor.instance.selectedSkills)
				{
					ActionSkill actS = ActionEditor.instance.currentLibraryNetwork.skills[i];
					
					GUILayout.Space(2.5f);
					GUILayout.Label(actS.Name,ActionResources.boldLabel, new GUILayoutOption[0]);
					GUILayout.Space(2.5f);
					
					GUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Name: ");
						actS.Name = EditorGUILayout.TextField(actS.Name).Replace(" ", string.Empty);
					
					}GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("Q: ");
						
						double.TryParse(EditorGUILayout.TextField(actS.Q.ToString()),out actS.Q);						
						
					}GUILayout.EndHorizontal();		
					
					GUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("B: ");
						
						double.TryParse(EditorGUILayout.TextField(actS.B.ToString()),out actS.B);						
						
					}GUILayout.EndHorizontal();		
					
					GUILayout.BeginHorizontal();
					{
						EditorGUILayout.PrefixLabel("v: ");
						
						double.TryParse(EditorGUILayout.TextField(actS.v.ToString()),out actS.v);						
						
					}GUILayout.EndHorizontal();	
										
					GUILayout.Space(2.5f);
					GUILayout.Label("Connections",ActionResources.boldLabel, new GUILayoutOption[0]);
					GUILayout.Space(2.5f);
					
					foreach(ActionConnection actC in ActionEditor.instance.currentLibraryNetwork.connections)
					{
						if(actC.Contains(actS))
						{
							ActionSkill otherEnd = actC.OtherEnd(actS);
							
							bool isToEnd = actC.isTo(actS);
							
							GUILayout.BeginHorizontal();
							{
								EditorGUILayout.LabelField(actS.Name + " to " + otherEnd.Name + ":");
								
								if(isToEnd)
								{
									double.TryParse(EditorGUILayout.TextField(actC.toValue.ToString()),out actC.toValue);
								}
								else
								{
									double.TryParse(EditorGUILayout.TextField(actC.fromValue.ToString()),out actC.fromValue);
								}
								
							}GUILayout.EndHorizontal();
							
							GUILayout.BeginHorizontal();
							{
								EditorGUILayout.LabelField(otherEnd.Name + " to " + actS.Name + ":");
								
								if(isToEnd)
								{
									double.TryParse(EditorGUILayout.TextField(actC.fromValue.ToString()),out actC.fromValue);
								}
								else
								{
									double.TryParse(EditorGUILayout.TextField(actC.toValue.ToString()),out actC.toValue);
								}								
								
							}GUILayout.EndHorizontal();	
							
							GUILayout.Space(2.5f);
							
						}	
					}
					
					GUILayout.Space(2.5f);
					
					GUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();
						
						if(GUILayout.Button("Preview skill", new GUILayoutOption[]
						{
							GUILayout.Width(125f)
						}))
						{
							ActionMenu.ShowSkillPreview(actS);
						}					
						
						GUILayout.Space(10f);
				
					} GUILayout.EndHorizontal();
					
					ActionResources.ActionSeparator();
				}
				
			}GUILayout.EndVertical();
		}
		
		HandleEvents();
	}
	
	private void HandleEvents()
	{
		switch(Event.current.type)
		{
		case EventType.MouseDown:			
			ActionNetworkEditor.instance.Repaint();

			ActionEditor.instance.SaveLibrary();
			
			break;
		case EventType.Repaint:
			ActionNetworkEditor.instance.Repaint();
			break;
		}
		
		if(Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
		{
			ActionEditor.instance.SaveLibrary();
			Event.current.Use ();
		}
	}
}
