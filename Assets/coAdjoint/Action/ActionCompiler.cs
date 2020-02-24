using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.IO;

public class ActionCompiler
{
	public static ActionNetwork AreSkillNamesUnique(ActionLibrary library)
	{
		for(int i=0; i < library.networks.Length; ++i)
		{
			string[] names = new string[library.networks[i].skills.Count];
			
			for(int j=0; j < library.networks[i].skills.Count; ++j)
			{
				names[j] = library.networks[i].skills[j].Name.RemoveSpecialCharacters();
			}
			
			for(int j=0; j < library.networks[i].skills.Count; ++j)
			{
				for(int k=j+1; k < library.networks[i].skills.Count; ++k)
				{
					if(names[j].Equals(names[k]))
					{
						return library.networks[i];
					}
				}
			}
		}
		
		return null;
	}
	
	public static void GenerateRTLibrary(ActionAsset asset)
	{
		ActionLibrary library = ActionLibrary.LoadData(asset.libraryData);
		
		ActionNetwork illegalNetwork = AreSkillNamesUnique(library);
		
		StringBuilder text = new StringBuilder();
		
		if(illegalNetwork != null)
		{
			EditorUtility.DisplayDialog("Error","At least two skills in the network " + illegalNetwork.name + " contained in the currently " +
				"selected asset have the same name after special characters have been removed from their names. Please change their names to ensure " +
				"they will be unique when special characters are removed.","Ok");
			
			return;
		}
		else
		{
			for(int i = 0; i < library.networks.Length; ++i)
			{
				if(library.networks[i].skills.Count > 0)
					text.Append(ActionClassGenerator.ClassString(library.networks[i]) + "\n\n");
			}

			string TEMPADDRESS = string.Empty;

			if(Application.platform != RuntimePlatform.WindowsEditor)
			{
				TEMPADDRESS = Path.Combine(Path.Combine(Path.Combine(EditorApplication.applicationContentsPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), "Frameworks"), "Managed")
				             , "UnityEngine.dll");
			}
			else
			{
				TEMPADDRESS = Path.Combine(Path.Combine(EditorApplication.applicationContentsPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), "Managed")
				             , "UnityEngine.dll");
			}

			using(StreamWriter writer = new StreamWriter("output.txt", false))
			{
				writer.WriteLine(text);
			}		

			CompilerParameters  compilerParameters = new CompilerParameters();
			compilerParameters.ReferencedAssemblies.Add(TEMPADDRESS);
			compilerParameters.ReferencedAssemblies.Add("system.dll");
			compilerParameters.ReferencedAssemblies.Add(Application.dataPath + @"/coAdjoint/Action/ACE.dll");
			compilerParameters.OutputAssembly = Application.dataPath + "/coAdjoint/Action/Builds/" + asset.name + "Build.dll";
					
			CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("CSharp");			
			
			CompilerResults cr = codeDomProvider.CompileAssemblyFromSource(compilerParameters, new string[]
			{
				text.ToString()
			});
			
			if(cr.Errors.Count > 0)
		    {
		        foreach(CompilerError ce in cr.Errors)
		        {
		            throw new ApplicationException(ce.ToString());
		        }
		    }	
			
			AssetDatabase.Refresh();
			
			
		}
		
	}
}
