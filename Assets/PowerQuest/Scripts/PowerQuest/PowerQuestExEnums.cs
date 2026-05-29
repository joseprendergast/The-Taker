using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using PowerTools;

namespace PowerTools.Quest
{


public partial class PowerQuest
{

	/* Some fun random code for making enum state easier to set/query. Don't have to worry about instance name of state, just the enum
		- It's not a whole lot quicker to type SetEnum(eStateBlah.Whatever); than m_stateBlah = eStateBlah.Whatever. 
		- Also, when exclusively using these enums, you get a warning on the base.
		- in short, it's not really much easier. Better autocomplete would solve it. (eg: if "m_state = " autocompleted the 'eState' bit for you via reflection )
			- Also, making m_ selected when double clicking in QuestScript editor
	*/
	
	// Cached field infos for efficiency
	Dictionary<System.Type,FieldInfo> m_cachedGsDic = new Dictionary<System.Type, FieldInfo>();
	Dictionary<System.Type,FieldInfo> m_cachedRmDic = new Dictionary<System.Type, FieldInfo>();

	///  Returns true if this script has an enum with the specified enum value. Usage: `if ( At(eDoor.Unlocked) ) {...}`. This is experimental shorthand for: `if ( m_door == eDoor.Unlocked ) {...}`
	//public bool At<tEnum>(tEnum enumState) where tEnum : struct, System.IConvertible, System.IComparable
	//{	
	//	// Use reflection to find enum type instance and check it matches		
	//	return GetEnum<tEnum>().Equals(enumState);
	//}	
	public bool At<tEnum>(params tEnum[] enumStates) where tEnum : struct, System.IConvertible, System.IComparable
	{	
		tEnum state = GetEnum<tEnum>();
		return System.Array.Exists(enumStates, item=> state.Equals(item));		
	}	

	///  Returns true if this script has an enum with the specified enum value. Usage: `if ( In(eDoor.Unlocked) ) {...}`. This is experimental shorthand for: `if ( m_door == eDoor.Unlocked ) {...}`
	//public bool Is<tEnum>(tEnum enumState) where tEnum : struct, System.IConvertible, System.IComparable
	//{	
	//	// Use reflection to find enum type instance and check it matches		
	//	return GetEnum<tEnum>().Equals(enumState);
	//}	
	public bool Is<tEnum>(params tEnum[] enumStates) where tEnum : struct, System.IConvertible, System.IComparable
	{	
		tEnum state = GetEnum<tEnum>();
		return System.Array.Exists(enumStates, item=> state.Equals(item));		
	}

	/// Returns true if reached state (same as m_state >= eState.myState )
	public bool Reached<tEnum>(tEnum enumState) where tEnum : struct, System.IConvertible, System.IComparable
	{	
		// Use reflection to find enum type instance and check it matches		
		return GetEnum<tEnum>().CompareTo(enumState) >= 0;
	}	
	/// Returns true if passed state (same as m_state > eState.myState )
	public bool After<tEnum>(tEnum enumState) where tEnum : struct, System.IConvertible, System.IComparable
	{	
		// Use reflection to find enum type instance and check it matches		
		return GetEnum<tEnum>().CompareTo(enumState) > 0;
	}		
	/// Returns true if haven't reached state (same as m_state < eState.myState )
	public bool Before<tEnum>(tEnum enumState) where tEnum : struct, System.IConvertible, System.IComparable
	{	
		// Use reflection to find enum type instance and check it matches		
		return GetEnum<tEnum>().CompareTo(enumState) < 0;
	}

	/// Returns true if reached first state, but before second state (same as m_state >= eState.first && m_state < eState.second )
	//public bool Between<tEnum>(tEnum firstInclusive, tEnum secondExclusive) where tEnum : struct, System.IConvertible, System.IComparable
	//{	
	//	// Use reflection to find enum type instance and check it matches
	//	return Reached(firstInclusive) && Before(secondExclusive);
	//}
		
	void LazyInitEnumGsDictionary()
	{ 
		if ( m_cachedGsDic.Count == 0 )
		{ 
			FieldInfo[] fields = m_globalScript.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach( FieldInfo finfo in fields)
			{ 
				if ( m_cachedGsDic.ContainsKey(finfo.FieldType) == false )
					m_cachedGsDic.Add(finfo.FieldType, finfo);
			}
		}

		if ( m_cachedRmDic.Count == 0 && m_currentRoom != null && m_currentRoom.GetScript() != null )
		{ 
			FieldInfo[] fields = m_currentRoom.GetScript().GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			foreach( FieldInfo finfo in fields)
			{
				if ( m_cachedRmDic.ContainsKey(finfo.FieldType) == false )
					m_cachedRmDic.Add(finfo.FieldType, finfo);
			}
		}
	}

	/// Sets an enum in this script with the specified value. Usage: `SetEnum(eDoor.Unlocked);`. This is experimental shorthand for: `m_door = eDoor.Unlocked;`
	public void Set<tEnum>(tEnum enumState) where tEnum : struct, System.IConvertible
	{
		// Use reflection to find enum type instance and set it		
		LazyInitEnumGsDictionary();
		FieldInfo info = null;
		if ( m_cachedGsDic.TryGetValue(typeof(tEnum),out info) )
			info.SetValue(m_globalScript,enumState);		
		else if ( m_cachedRmDic.TryGetValue(typeof(tEnum), out info) )
			info.SetValue(m_currentRoom.GetScript(),enumState);	
		else
			Debug.Log("Failed to set enum: "+enumState.ToString());
	}


	tEnum GetEnum<tEnum>() where tEnum : struct, System.IConvertible
	{		
		LazyInitEnumGsDictionary();
		FieldInfo info = null;
		if ( m_cachedGsDic.TryGetValue(typeof(tEnum),out info) )
			return (tEnum)info.GetValue(m_globalScript);		
		// Try room
		if ( m_cachedRmDic.TryGetValue(typeof(tEnum), out info) )
			return (tEnum)info.GetValue(m_currentRoom.GetScript());
		else 
			Debug.Log("Failed to find enum: "+typeof(tEnum).ToString());
		return default(tEnum);
	}

	// Enums type data is cached and needs to be cleared when room changes or globalscript changes so it can be rebuilt (lazily)
	void ClearEnumCache(bool roomOnly = false)
	{ 
		if ( roomOnly == false )
			m_cachedGsDic.Clear();
		m_cachedRmDic.Clear();
	}
}


}
