using Microsoft.Win32;

namespace NibblePoker.Library.RegistryHelpers;

/// <summary>
///   Static class that contains all the functions of this library
/// </summary>
public class RegistryHelpers {
	/// <summary>
	///   Lists the values' names of a given <see cref="RegistryKey">RegistryKey</see>
	///    as a <see cref="List{T}">List&lt;string&gt;</see>
	///    instead of a <c>string[]</c>.
	/// </summary>
	/// <param name="searchedKey">
	///   <see cref="RegistryKey">RegistryKey</see> whose values' names will be returned.
	/// </param>
	/// <returns>
	///   The key's values as a <see cref="List{T}">List&lt;string&gt;</see>.
	/// </returns>
	/// <exception cref="System.Security.SecurityException">
	///   The user does not have the permissions required to read from the registry key.<br/>
	///   Thrown by <see cref="RegistryKey.GetValueNames">RegistryKey.GetValueNames</see>.
	/// </exception>
	/// <exception cref="ObjectDisposedException">
	///   The RegistryKey being manipulated is closed (closed keys cannot be accessed).<br/>
	///   Thrown by <see cref="RegistryKey.GetValueNames">RegistryKey.GetValueNames</see>.
	/// </exception>
	/// <exception cref="UnauthorizedAccessException">
	///   The user does not have the necessary registry rights.<br/>
	///   Thrown by <see cref="RegistryKey.GetValueNames">RegistryKey.GetValueNames</see>.
	/// </exception>
	/// <exception cref="IOException">
	///   A system error occurred; for example, the current key has been deleted.<br/>
	///   Thrown by <see cref="RegistryKey.GetValueNames">RegistryKey.GetValueNames</see>.
	/// </exception>
	private static List<string> _GetValuesNameList(RegistryKey searchedKey) {
		return new List<string>(searchedKey.GetValueNames());
	}
	
	/// <summary>
	///   Lists the values' names of a given <see cref="RegistryKey">RegistryKey</see>
	///    as a <see cref="List{T}">List&lt;string&gt;</see>
	///    instead of a <c>string[]</c>.
	/// </summary>
	/// <param name="rootKey">
	///   Root <see cref="RegistryKey">RegistryKey</see> whose given subkey will be processed.
	/// </param>
	/// <param name="mainKey">
	///   Main key to search in.
	/// </param>
	/// <returns>
	///   The key's values as a <see cref="List{T}">List&lt;string&gt;</see>.<br/>
	///   If any error occured, the list will be empty.
	/// </returns>
	/// <warning>If any error occured, the returned list will be empty.</warning>
	public static List<string> GetValuesNameList(RegistryKey rootKey, string mainKey) {
		List<string> keyValuesName = new List<string>();
		
		try {
			using RegistryKey? regKey = rootKey.OpenSubKey(mainKey);
			
			if(regKey == null) {
				throw new NullReferenceException("Failed to open '"+mainKey+"' !");
			}
			
			keyValuesName = _GetValuesNameList(regKey);
		} catch (Exception) {
			// We don't really care about this one.
		}
		
		return keyValuesName;
	}
	
	/// <summary>
	///   Reads the content of a given root <see cref="RegistryKey">RegistryKey</see> and its given subkey, and returns
	///   a <see cref="Dictionary{T, T}">Dictionary&lt;string, string&gt;</see> containing its
	///   <see cref="RegistryValueKind.String">String</see> keys and their value.
	/// </summary>
	/// <param name="rootKey">
	///   Root <see cref="RegistryKey">RegistryKey</see> whose given subkey will be processed.
	/// </param>
	/// <param name="mainKey">
	///   Main key to search in.
	/// </param>
	/// <returns>
	///   The content of the given <see cref="RegistryKey">RegistryKey</see> as a
	///   <see cref="Dictionary{T, T}">Dictionary&lt;string, string&gt;</see> that represents all
	///   <see cref="RegistryValueKind.String">String</see> keys and their value.
	/// </returns>
	/// <warning>If any error occured, the returned dictionary will be empty.</warning>
	// ReSharper disable once MemberCanBePrivate.Global
	public static Dictionary<string, string> GetMappedKeyNameStringValue(RegistryKey rootKey, string mainKey) {
		Dictionary<string, string> readPairs = new Dictionary<string, string>();
		
		try {
			using RegistryKey? regKey = rootKey.OpenSubKey(mainKey);
			
			if(regKey == null) {
				throw new NullReferenceException("Failed to open '"+mainKey+"' !");
			}
			
			foreach(string valueName in _GetValuesNameList(regKey)) {
				if(regKey.GetValueKind(valueName) != RegistryValueKind.String) {
					continue;
				}
				
				object? valueData = regKey.GetValue(valueName);
				
				if(valueData != null) {
					readPairs.Add(valueName, (valueData as string)!);
				}
			}
		} catch (Exception) {
			// We don't really care about this one here.
		}
		
		return readPairs;
	}
	
	/// <summary>
	///   Reads the content of a given root <see cref="RegistryKey">RegistryKey</see> and its given subkey, and returns
	///   a <see cref="List{T}">List&lt;string&gt;</see> containing its
	///   <see cref="RegistryValueKind.String">String</see> keys' values.
	/// </summary>
	/// <param name="rootKey">
	///   Root <see cref="RegistryKey">RegistryKey</see> whose given subkey will be processed.
	/// </param>
	/// <param name="mainKey">
	///   Main key to search in.
	/// </param>
	/// <returns>
	///   The content of the given <see cref="RegistryKey">RegistryKey</see> as a
	///   <see cref="List{T}">List&lt;string&gt;</see> that represents all
	///   <see cref="RegistryValueKind.String">String</see> keys' values.
	/// </returns>
	/// <warning>If any error occured, the returned list will be empty.</warning>
	public static List<string> GetKeyValues(RegistryKey rootKey, string mainKey) {
		List<string> keyValues = new List<string>();
		
		foreach(string value in GetMappedKeyNameStringValue(rootKey, mainKey).Values) {
			keyValues.Add(value);
		}
		
		return keyValues;
	}
	
	/// <summary>
	///   Reads the content of a given root <see cref="RegistryKey">RegistryKey</see> and its given subkey, and returns
	///   a <see cref="List{T}">List&lt;string&gt;</see> containing its
	///   <see cref="RegistryValueKind.String">String</see> keys' name.
	/// </summary>
	/// <param name="rootKey">
	///   Root <see cref="RegistryKey">RegistryKey</see> whose given subkey will be processed.
	/// </param>
	/// <param name="mainKey">
	///   Main key to search in.
	/// </param>
	/// <param name="prependMainKey">
	///   Prepends the given main key to the returned keys.
	/// </param>
	/// <returns>
	///   The content of the given <see cref="RegistryKey">RegistryKey</see> as a
	///   <see cref="List{T}">List&lt;string&gt;</see> that represents all
	///   <see cref="RegistryValueKind.String">String</see> keys' name.
	/// </returns>
	/// <warning>If any error occured, the returned list will be empty.</warning>
	public static List<string> GetSubKeys(RegistryKey rootKey, string mainKey, bool prependMainKey = false) {
		List<string> subKeys = new List<string>();
		
		try {
			using RegistryKey? regKey = rootKey.OpenSubKey(mainKey);
			
			if(regKey == null) {
				throw new NullReferenceException("Failed to open '"+mainKey+"' !");
			}
			
			subKeys.AddRange(regKey.GetSubKeyNames().Select(
				subKeyName => (prependMainKey ? mainKey + "\\" : "") + subKeyName)
			);
		} catch (Exception) {
			// We don't really care about this one here.
		}
		
		return subKeys;
	}
}
