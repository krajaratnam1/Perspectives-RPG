using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

[Serializable]
public class CoAdjointProjectAsset
{
	private const int currentVersion = 1;
	private byte[] data;
	
	public static byte[] GetData(ActionLibrary library)
	{
		CoAdjointProjectAsset projectAsset = new CoAdjointProjectAsset();
				
		MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		
		//Serialise library to memoryStream
		binaryFormatter.Serialize(memoryStream, library);
		memoryStream.Close();
		
		//Store memory stream as byte array
		projectAsset.data = memoryStream.ToArray();
		
		memoryStream = new MemoryStream();
		binaryFormatter = new BinaryFormatter();
		
		//Seralise projectAsset to memoryStream (useful if project asset contains other data)
		binaryFormatter.Serialize(memoryStream, projectAsset);
		
		memoryStream.Close();
		
		//Return it
		return memoryStream.ToArray();
	}	
	
	public static object Load(byte[] data)
	{
		object obj = new BinaryFormatter().Deserialize(new MemoryStream(data));
		
		if(obj is CoAdjointProjectAsset)
		{
			CoAdjointProjectAsset asset = obj as CoAdjointProjectAsset;
			return new BinaryFormatter().Deserialize(new MemoryStream(asset.data)) as ActionLibrary;
		}
		else throw new ApplicationException("Unable to deserialise type " + obj.GetType());
	}

}
