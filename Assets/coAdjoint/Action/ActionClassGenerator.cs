using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class ActionClassGenerator
{
	public static void CreateNetworkClass(ActionNetwork actionNetwork)
	{
		string path = Application.dataPath + @"/coAdjoint/Action/ActionTest.cs";
		//string path = @"C:/temp/ActionTest.cs";		
		
	    try
	    {	
	        if (File.Exists(path))
	        {
	            File.Delete(path);
	        }
			
			string text = ClassString(actionNetwork);
	
	        // Create the file. 
	        using (FileStream fs = File.Create(path))
	        {
	            Byte[] info = new UTF8Encoding(true).GetBytes(text);
	            // Add some information to the file.
	            fs.Write(info, 0, info.Length);
	        }
	    }
	
	    catch (Exception Ex)
	    {
	        Debug.Log(Ex.ToString());
	    }	
	}
	
	public static string ClassString(ActionNetwork actionNetwork)
	{
		string text = 
			"\n\tpublic sealed class " + actionNetwork.name.RemoveSpecialCharacters() + " : ACE.Runtime.Network \n" +
			"\t{\n" +
			"\t\tpublic enum Skill \n" +
			"\t\t{ \n";

		string[] skillNames = new string[actionNetwork.skills.Count];
		
		for(int i = 0; i < actionNetwork.skills.Count; ++i)
		{
			skillNames[i] = actionNetwork.skills[i].Name;
			
			text += "\t\t\t" + skillNames[i].RemoveSpecialCharacters().Replace(" ", string.Empty) + "=" + i + ",\n";
		}
		
		text += "\t\t}\n\n";
		
		text += "\t\tprivate void Init()\n" +
			"\t\t{\n" +
			"\t\t\tmodelNo = " + actionNetwork.skills.Count.ToString() + "; \n\n" +
			"\t\t\t_stren = new double[][]" +
			"\t{";
		
		double[,] connectionMatrix = new double[actionNetwork.skills.Count,actionNetwork.skills.Count];
		
		List<ActionConnection> alreadyCoveredConnections = new List<ActionConnection>();
		
		for(int j = 0; j < actionNetwork.skills.Count; ++j)
		{
			foreach(ActionConnection actC in actionNetwork.connections)
			{
				if(!alreadyCoveredConnections.Contains(actC))
				{				
					ActionSkill actS = actionNetwork.skills[j];
					
					if(actC.Contains(actS))
					{
						alreadyCoveredConnections.Add(actC);
						
						if(actC.isTo(actS))
						{
							int index = actionNetwork.skills.IndexOf(actC.From);
							
							if(index != -1)
							{
								connectionMatrix[j,index] = actC.toValue;
								connectionMatrix[index,j] = actC.fromValue;
							}
						}
						else
						{
							int index = actionNetwork.skills.IndexOf(actC.To);
							
							if(index != -1)
							{
								connectionMatrix[index,j] = actC.toValue;
								connectionMatrix[j,index] = actC.fromValue;
							}
						}
					}
				}
			}
		}
		
		for(int j = 0; j <  actionNetwork.skills.Count; ++j)
		{
			text += "\t\n\t\t\t new double[] {";
			
			for(int k = 0; k < actionNetwork.skills.Count - 1; ++k)
			{
				text += connectionMatrix[j,k].ToString() + ", ";
			}		
			
			text += connectionMatrix[j,actionNetwork.skills.Count - 1].ToString();
			
			text += " },";
		}		
		
		text += "\t};";
		
		text += "\t\n\n";
		
		text += "\t\t\t_l = new double[" + actionNetwork.skills.Count + "];\n";
		text += "\t\t\t_exp = new double[" + actionNetwork.skills.Count + "];\n";
		text += "\t\t\t_skills = new ACE.Runtime.Skill[" + actionNetwork.skills.Count + "]{\n";
		
		for(int i = 0; i < actionNetwork.skills.Count; ++i)
		{
			if(i != 0) text += "\t\n";
			
			text += "\t\t\t\tnew ACE.Runtime.Skill( \"" + skillNames[i] + "\", this, " + i.ToString() + ", " + actionNetwork.skills[i].Q.ToString() +
				", " + actionNetwork.skills[i].B.ToString() + ", " + actionNetwork.skills[i].v.ToString() + "),";
		}
		
		text = text.Remove(text.Length-1);
		
		text += "\t};\n";
		
		text += "\t\t}\n\n";
		
		text += "\t\tpublic " + actionNetwork.name + "()\n" +
			"\t\t{\n" +
			"\t\t\tInit();\n" +
			"\t\t\tSetAllLevels(1);\n" +
			"\t\t}\n\n";
		
		text += "\t\tpublic " + actionNetwork.name + "(double level)\n" +
			"\t\t{\n" +
			"\t\t\tInit();\n" +
			"\t\t\tSetAllLevels(level);\n" +
			"\t\t}\n\n";		
		
		text += "\t\tpublic " + actionNetwork.name + "(double[] levels)\n" +
			"\t\t{\n" +
			"\t\t\tif(levels.Length != " + actionNetwork.skills.Count + ")\n" +
			"\t\t\t{\n" +
			"\t\t\t\tUnityEngine.Debug.LogError(\"Double array input length does not equal \" + " + actionNetwork.skills.Count + " + \". Calling default constructor\");\n" +
			"\t\t\t\tInit();\n" +
			"\t\t\t\tSetAllLevels(1);\n\n" +
			"\t\t\t\treturn;\n" +
			"\t\t\t}\n" +
			"\t\t\telse\n" +
			"\t\t\t{\n" +
			"\t\t\t\tInit();\n\n" +
			"\t\t\t\tfor(int i = 0; i < " + actionNetwork.skills.Count + "; ++i)\n" +
			"\t\t\t\t{\n" +
			"\t\t\t\t\tSetLevel((Skill)i, levels[i]);\n" +
			"\t\t\t\t}\n" +
			"\t\t\t}\n" +
			"\t\t}\n\n" +
			"\t\tpublic void SetAllLevels(double level)\n" +
			"\t\t{\n" +
			"\t\t\tfor(int i = 0; i < " + actionNetwork.skills.Count + "; ++i)\n" +
			"\t\t\t{\n" +
			"\t\t\t\tSetLevel((Skill)i, level);\n" +
			"\t\t\t}\n" +
			"\t\t}\n\n" +
			"\t\tpublic void SetLevel(Skill skill, double val)\n" +
			"\t\t{\n" +
			"\t\t\tint model = (int)skill;\n\n" +
			"\t\t\tRemoveSkillFromQueue(model);\n\n" +
				"\t\t\t_exp[model] = ACE.Runtime.BrentSolver.Brent(new ACE.Runtime.BrentSolver.FunctionOfOneVariable(_skills[model].Level), 0, 1000, 1e-10, val);\n" +
			"\t\t\t_l[model] = _skills[model].Level(_exp[model]);\n" +
			"\t\t}\n\n" +
			"\t\tpublic double GetLevelAsDouble(Skill skill)\n \t\t{\n\t\t\tint model = (int)skill;\n\n\t\t\tlock(_skills[model].loc)\n\t\t\t{\n\t\t\t\treturn _l[model];\n" +
			"\t\t\t}\n\t\t}\n\n" +
			"\t\tpublic float GetLevelAsFloat(Skill skill)\n \t\t{\n \t\t\treturn (float)GetLevelAsDouble(skill);\n \t\t}\n\n" +
			"\t\tpublic double GetExperienceAsDouble(Skill skill)\n \t\t{\n \t\t\tint model = (int)skill;\n\n \t\t\tlock(_skills[model].loc)\n \t\t\t{\n \t\t\t\treturn _exp[model];\n \t\t\t}\n \t\t}\n\n" +
			"\t\tpublic float GetExperienceAsFloat(Skill skill)\n\t\t{\n\t\t\treturn (float)GetExperienceAsDouble(skill);\n\t\t}\n\n" +
				"\t\tpublic void LevelUp(Skill skill, double val)\n\t\t{\n\t\t\tint model = (int)skill;\n\n\t\t\tACE.Runtime.ThreadedActionScript.instance.Action(_skills[model],val);\n \t\t}\n\n" +
			"\t\tprivate void RemoveSkillFromQueue(Skill skill)\n \t\t{\n \t\t\tint model = (int)skill;\n\n  \t\t\tRemoveSkillFromQueue(model);\n \t\t}\n\n" +
				"\t\tprivate void RemoveSkillFromQueue(int i)\n\t\t{\n \t\t\tlock(_skills[i].loc)\n \t\t\t{\n \t\t\t\t_skills[i].isBlockedFromQueue = true;\n\n  \t\t\t\tif(_skills[i].isInQueue)\n \t\t\t\t{\n \t\t\t\t\tACE.Runtime.ThreadedActionScript.instance.RemoveFromQueue(_skills[i]);\n\t\t\t\t}\n\n\t\t\t\t_skills[i].isBlockedFromQueue = false;\n \t\t\t}\n \t\t}\n\n" +
				"\t\tpublic void Remove()\n\t\t{\n \t\t\tACE.Runtime.ThreadedActionScript.instance.ClearQueue();\n \t\t}\n";
				
			return text + "\t}"; 
	}
}

public static class StringExtensions
{
	public static string RemoveSpecialCharacters(this string str)
	{
	    char[] buffer = new char[str.Length];
	    int idx = 0;
	
	    foreach (char c in str)
	    {
	        if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z')
	            || (c >= 'a' && c <= 'z') || (c == '.') || (c == '_'))
	        {
	            buffer[idx] = c;
	            idx++;
	        }
	    }
	
	    return new string(buffer, 0, idx);
	}	
	
}
