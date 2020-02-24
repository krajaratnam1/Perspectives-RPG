using UnityEngine;
using UnityEditor;
using System.Collections;

public class ActionLibraryCreation : EditorWindow {
	
	private string newNetworkName = "MyNewSkillLibrary";
	
	public ActionLibraryCreation()
	{
		titleContent = new GUIContent("Action Skill Library Setup");
		
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
				EditorGUILayout.PrefixLabel("Library Name:");
				newNetworkName = EditorGUILayout.TextField(newNetworkName);
			}
			EditorGUILayout.EndHorizontal();
			
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				
				if(GUILayout.Button("Create New Skill Library",GUILayout.ExpandWidth(false)))
				{
					CreateNewAsset();
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();	
	}	
	
	public void CreateNewAsset()
	{
		var asset = ScriptableObject.CreateInstance<ActionAsset>();

		newNetworkName = newNetworkName.Replace(" ", string.Empty);
		
		string nameCopy = newNetworkName.Clone().ToString();
			
		int index = 0;

		if(System.IO.File.Exists(Application.dataPath + "/coAdjoint/Action/Libraries/" + newNetworkName + ".asset"))
		{
			++index;
			
			while(System.IO.File.Exists(Application.dataPath + "/coAdjoint/Action/Libraries/" + newNetworkName + index + ".asset"))
			{
				++index;
			}
			
			nameCopy += index.ToString();
		}
		
		asset.libraryData = (new ActionLibrary()).GetData();		
		
		AssetDatabase.CreateAsset(asset,"Assets/coAdjoint/Action/Libraries/" + nameCopy + ".asset");
		
		Selection.activeObject = asset;
		
		ActionMenu.EditLibraryAsset();
		
		this.Close();
	}
}


