using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

[CustomEditor (typeof (ActionAsset))]
public class ActionAssetEditor : Editor {
	
	private ActionLibrary currentLibrary;
	
	public override void OnInspectorGUI()
	{
		GUILayout.TextArea("This asset contains your Action Library. The library contains one or more" +
		 	" networks for use in your Unity3D project. To edit the library, press the 'Edit Library' button below.");
		EditorGUILayout.Separator();
		
		ActionAsset highlightedAsset = Selection.activeObject as ActionAsset;
		
		try
		{
			currentLibrary = ActionLibrary.LoadData(highlightedAsset.libraryData);
		}
		catch{}	
		
		GUILayout.Label("This library contains " + currentLibrary.networks.Length + " networks.");
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Edit Library",GUILayout.MaxWidth(100f)))
			{
				ActionMenu.EditLibraryAsset();
			}
			GUILayout.FlexibleSpace();
			
		}
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10f);
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Generate Runtime Library from Asset"))
			{
				//ActionClassGenerator.CreateNetworkClass(Selection.activeObject as ActionAsset);
				ActionCompiler.GenerateRTLibrary(Selection.activeObject as ActionAsset);
			}
			GUILayout.FlexibleSpace();
			
		}
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10f);
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Export Library"))
			{
				SerializeToXML(currentLibrary);
				//SerializeToBin(currentLibrary);
			}
			GUILayout.FlexibleSpace();
			
		}
		GUILayout.EndHorizontal();
		
	}
	
	static public void SerializeToBin(ActionLibrary library)
	{
		IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		Stream stream = new FileStream(Application.dataPath + @"/coAdjoint/Action/Exports/" + Selection.activeObject.name + ".ActLib", FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize(stream, library);
		stream.Close();		
	}
		
	static public void SerializeToXML(ActionLibrary library)
	{
		XmlWriterSettings settings = new XmlWriterSettings()
		{
			Indent = true,
			IndentChars = "\t",
			NewLineOnAttributes = true
		};

		using(XmlWriter writer = XmlWriter.Create(Application.dataPath + @"/coAdjoint/Action/Exports/" + Selection.activeObject.name + ".xml",settings))
		{
			writer.WriteStartDocument();
			{
				writer.WriteStartElement("ActionLibrary");
				{
					writer.WriteAttributeString("Name", Selection.activeObject.name);

					foreach(ActionNetwork actN in library.networks)
					{
						writer.WriteStartElement("Network");
						{
							writer.WriteAttributeString("Name", actN.name);

							writer.WriteStartElement("Skills");
							{
								foreach(ActionSkill actS in actN.skills)
								{
									writer.WriteStartElement("Skill");
									{
										writer.WriteAttributeString("Name", actS.Name);

										writer.WriteElementString("Q", actS.Q.ToString());
										writer.WriteElementString("B", actS.B.ToString());
										writer.WriteElementString("v", actS.v.ToString());
										writer.WriteElementString("Position","(" + actS.Position.X.ToString() + "," + actS.Position.Y.ToString() + ")");
									}
									writer.WriteEndElement();
								}
							}
							writer.WriteEndElement();

							writer.WriteStartElement("Connections");
							{
								foreach(ActionConnection actC in actN.connections)
								{
									writer.WriteStartElement("Connection");
									{
										writer.WriteElementString("ToSkill", actC.To.Name);
										writer.WriteElementString("FromSkill", actC.From.Name);
										writer.WriteElementString("ToValue", actC.toValue.ToString());
										writer.WriteElementString("FromValue",actC.fromValue.ToString());									
									}
									writer.WriteEndElement();
								}
							}
							writer.WriteEndElement();

						}
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			writer.WriteEndDocument();
		}
	}
	
	
}
