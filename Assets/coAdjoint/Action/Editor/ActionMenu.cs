using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Xml.Linq;
using System.Linq;

public class ActionMenu : ScriptableObject {
	
	public static ActionEditor libEditorInstance
	{
		get
		{
			if(ActionEditor.instance == null)
			{
				ActionEditor actionEditor = CreateInstance<ActionEditor>();
				actionEditor.Init();
			}			
			
			return ActionEditor.instance;
		}
	}	
	
	[MenuItem("Assets/Create/Action Skill Library")]
	public static void CreateSkillLibrary()
	{
		ActionLibraryCreation.GetWindow(typeof(ActionLibraryCreation),true);	
	}

	public static void EditLibraryAsset()
	{
		EditLibrary();
	}	

	[MenuItem("Window/Import Action Library")]
	public static void OpenConverterWindow()
	{
		ImportActionLibrary();
	}

	[MenuItem("Window/Action Network Editor")]
	public static void OpenNetworkEditor()
	{
		ShowNetworkEditor();
	}
	
	[MenuItem("Window/Action Library Explorer")]
	public static void OpenLibraryExplorer()
	{
		ShowExplorer();
	}	

	public static void EditLibrary()
	{		
		libEditorInstance.currentAsset = Selection.activeObject as ActionAsset;
		
		ShowExplorer();
		ShowNetworkEditor();
	}
	
	public static void ShowExplorer()
	{
		if (ActionLibraryExplorer.instance == null)
		{
			Debug.LogError ("Failed to set up Action Library Explorer");
			return;
		}			
		
		ActionLibraryExplorer.instance.Show();
		ActionLibraryExplorer.instance.Focus();
		ActionLibraryExplorer.instance.Repaint();
		
		if(ActionEditor.instance != null)
		{
			ActionLibraryExplorer.instance.highlightedItemType = ActionLibraryExplorer.ItemType.ActionNetwork;
			
			if(ActionEditor.instance.currentAssetLibrary.networks.Length > 0)
				ActionEditor.instance.currentLibraryNetwork = ActionEditor.instance.currentAssetLibrary.networks[0];
		}
	}
	
	public static void ShowNetworkEditor()
	{
		if (ActionNetworkEditor.instance == null)
		{
			Debug.LogError ("Failed to set up Action Network Window");
			return;
		}		
		
		ActionNetworkEditor.instance.Show ();
		ActionNetworkEditor.instance.Focus();
		ActionNetworkEditor.instance.Repaint();		
		
		ActionNetworkEditor.instance.currentContent = ActionNetworkEditor.instance.empty;
	}

	public static void ImportActionLibrary()
	{
		string file = EditorUtility.OpenFilePanel("Select Action Library to import", "", "xml");
		
		if (file == null || file.Length == 0) return;
		
		XDocument doc = XDocument.Load(file); 
		
		try 
		{
			ActionLibrary importedLibrary = new ActionLibrary();
			
			foreach(var network in doc.Root.Elements("Network"))
			{
				ActionNetwork actN = new ActionNetwork(network.Attribute("Name").Value);
				
				importedLibrary.Add(actN);
				
				foreach(var skill in network.Elements("Skills").Elements("Skill"))
				{
					string[] position = skill.Element("Position").Value.Replace("(","").Replace(")","").Split(',');
					
					EditorTwoVector pos = new EditorTwoVector(float.Parse(position[0]),float.Parse(position[1]));
					
					actN.AddSkill(skill.Attribute("Name").Value,double.Parse(skill.Element("Q").Value),double.Parse(skill.Element("B").Value),
					              double.Parse(skill.Element("v").Value), pos);
				}
				
				foreach(var connection in network.Elements("Connections").Elements("Connection"))
				{
					actN.AddConnection(actN.Get(connection.Element("ToSkill").Value),actN.Get(connection.Element("FromSkill").Value),
					                   double.Parse(connection.Element("ToValue").Value),double.Parse(connection.Element("FromValue").Value));
				}
			}
			
			string fileName = Path.GetFileNameWithoutExtension(file);
			
			string nameCopy = fileName.Clone().ToString();
			
			int index = 0;
			
			if(System.IO.File.Exists(Application.dataPath + "/coAdjoint/Action/Libraries/" + fileName + ".asset"))
			{
				++index;
				
				while(System.IO.File.Exists(Application.dataPath + "/coAdjoint/Action/Libraries/" + fileName + index + ".asset"))
				{
					++index;
				}
				
				nameCopy += index.ToString();
			}
			
			var asset = ScriptableObject.CreateInstance<ActionAsset>();
			
			asset.libraryData = importedLibrary.GetData();	
			
			AssetDatabase.CreateAsset(asset,"Assets/coAdjoint/Action/Libraries/" + nameCopy + ".asset");
			
			Selection.activeObject = asset;
			
			ActionMenu.EditLibraryAsset();			
			
		}
		catch (Exception e) 
		{
			Debug.LogError("Failed to deserialize. Reason: " + e.Message);
			throw;
		}
		
	}
	
	
	public static void ShowSkillPreview(ActionSkill actS)
	{
		if(ActionSkillPreview.instance == null)
		{
			Debug.LogError("Failed to set up Action Skill Preview Window");
			return;
		}
		
		ActionSkillPreview.instance.Show();
		ActionSkillPreview.instance.Focus();
		ActionSkillPreview.instance.Repaint();	
		ActionSkillPreview.instance.currentSkill = actS;
	}
	
}
