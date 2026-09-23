using System.Text;
using UnityEngine;

public class JSONSerializer<T>
{
	public string GenerateString(T data)
	{
		string empty = string.Empty;
		return JsonUtility.ToJson(data);
	}

	public T GetRequiredData(string jsonString)
	{
		T val = default(T);
		return JsonUtility.FromJson<T>(jsonString);
	}

	public byte[] CodeToBinary(string jsonString)
	{
		return Encoding.UTF8.GetBytes(jsonString);
	}

	public string EncodeBinary(byte[] byteArray)
	{
		return Encoding.UTF8.GetString(byteArray);
	}
}
