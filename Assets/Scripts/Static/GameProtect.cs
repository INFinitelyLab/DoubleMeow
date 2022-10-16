using UnityEngine;
using System;
using System.Linq;

[Serializable]
public sealed class Secure
{
	[SerializeField]
	private SecureData _data;

	private static string firstKey = "3678924785692";
	private static string secondKey = "5185718955567";
	private static string thirdKey = "2652756278185";

	private string _name;

	public SecureData Encrypted
	{
		get
		{
			return _data;
		}
	}


	public Secure(string value, string name)
	{
		_data = Encrypt(value, name);

		_name = name;
	}


	public void SetData(SecureData data)
	{
		_data = data;
    }


	public static implicit operator string(Secure value) => Decrypt( value._data, value._name );

	public static implicit operator SecureInt(Secure value) => new SecureInt( int.TryParse(value, out int output)? output : 0, value._name );


	public static SecureData Encrypt(string value, string name)
	{
		SecureData data;

		value = Revert( value, name );

		data.firstValue = Revert( value, firstKey);
		data.secondValue = Revert(value, secondKey);
		data.thirdValue = Revert(value, thirdKey);

		return data;
	}


	public static string Decrypt(SecureData data, string name)
	{
		string firstDecryptValue = Revert( Revert(data.firstValue, firstKey), name);
		string secondDecryptValue = Revert(Revert(data.secondValue, secondKey), name);
		string thirdDecryptValue = Revert(Revert(data.thirdValue, thirdKey), name);

		//Debug.Log( "Hi Decrypt : name(" + name + "), : " + firstDecryptValue + " , " + secondDecryptValue + " , " + thirdDecryptValue );

		return firstDecryptValue == secondDecryptValue && secondDecryptValue == thirdDecryptValue ? firstDecryptValue : "";
	}


	public static string Revert(string input, string key)
	{
		if (input == null || key == null) return "";

		System.Text.StringBuilder builder = new System.Text.StringBuilder();

		for (int i = 0; i < input.Length; i++)
		{
			builder.Append((char)(input[i] ^ key[(i % key.Length)]));
		}

		string result = builder.ToString();

		return result; 
	}
}


public sealed class SecureInt
{
	private Secure _secure;
	private string name;


	public SecureInt(int value, string name)
	{
		_secure = new Secure(value.ToString(), name);
		
		this.name = name;
	}


	public void Increase(int value)
	{
		int newValue = this + value;

		_secure.SetData( Secure.Encrypt(newValue.ToString(), name) );
	}


	public static implicit operator int(SecureInt value)
	{
		return int.TryParse(value._secure, out int output) ? output : 0;
	}


	public static implicit operator Secure(SecureInt value)
	{
		return value._secure;
	}
}


[Serializable]
public struct SecureData
{
	public string firstValue;
	public string secondValue;
	public string thirdValue;
}