using Microsoft.Win32;

namespace NibblePoker.Library.RegistryHelpers;

/// <summary>
/// Static class that contains all the functions of this library
/// </summary>
public class RegistryHelpers {
	/// <summary>
	/// Lists the values' names of a given key as a <c>List</c> instead of an <c>Array</c>.
	/// </summary>
	/// <param name="searchedKey">Searched registry key combination of the root and main keys.</param>
	/// <returns>The key's values as a <c>List</c>.</returns>
	private static List<string> GetKeyValuesNameFromKey(RegistryKey searchedKey) {
		return new List<string>(searchedKey.GetValueNames());
	}
	
	/// <summary>
	/// ???
	/// </summary>
	/// <param name="rootKey">Used registry's root key.</param>
	/// <param name="mainKey">Main key to search in.</param>
	/// <returns>???</returns>
	/// <exception cref="NullReferenceException">Returned if the given root and main key combination couldn't be opened.</exception>
	public static List<string> GetKeyValuesName(RegistryKey rootKey, string mainKey) {
		List<string> keyValuesName = new List<string>();
		
		try {
			using RegistryKey? regKey = rootKey.OpenSubKey(mainKey);
			
			if(regKey == null) {
				throw new NullReferenceException("Failed to open '"+mainKey+"' !");
			}
			
			keyValuesName = GetKeyValuesNameFromKey(regKey);
		} catch (Exception) {
			// We don't really care about this one.
		}
		
		return keyValuesName;
	}
	
	/// <summary>
	/// ???
	/// </summary>
	/// <param name="rootKey">Used registry's root key.</param>
	/// <param name="mainKey">Main key to search in.</param>
	/// <returns>???</returns>
	/// <exception cref="NullReferenceException">Returned if the given root and main key combination couldn't be opened.</exception>
	public static Dictionary<string, string> GetMappedKeyNameStringValue(RegistryKey rootKey, string mainKey) {
		Dictionary<string, string> readPairs = new Dictionary<string, string>();
		
		try {
			using RegistryKey? regKey = rootKey.OpenSubKey(mainKey);
			
			if(regKey == null) {
				throw new NullReferenceException("Failed to open '"+mainKey+"' !");
			}
			
			foreach(string valueName in GetKeyValuesNameFromKey(regKey)) {
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
	/// ???
	/// </summary>
	/// <param name="rootKey">Used registry's root key.</param>
	/// <param name="mainKey">Main key to search in.</param>
	/// <returns>???</returns>
	public static List<string> GetKeyValues(RegistryKey rootKey, string mainKey) {
		List<string> keyValues = new List<string>();
		
		foreach(string value in GetMappedKeyNameStringValue(rootKey, mainKey).Values) {
			keyValues.Add(value);
		}
		
		return keyValues;
	}
	
	/// <summary>
	/// ???
	/// </summary>
	/// <param name="rootKey">Used registry's root key.</param>
	/// <param name="mainKey">Main key to search in.</param>
	/// <param name="prependMainKey">Prepends the given main key to the returned keys.</param>
	/// <returns>???</returns>
	/// <exception cref="NullReferenceException">Returned if the given root and main key combination couldn't be opened.</exception>
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
