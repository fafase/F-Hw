using UnityEngine;
using System.Collections;
using System;
using Boomlagoon.JSON;
using System.Collections.Generic;

namespace DataAnalytics.JSONData
{
	/// <summary>
	/// Uses a json parser to convert json to data
	/// </summary>
	public sealed class JsonSerializer
	{
		public enum ObjectType
		{
			None, String, Number, Boolean, Object
		}
		/// <summary>
		/// Gets the json, the ID and the type to return.
		/// </summary>
		/// <returns>The object from json with identifier.</returns>
		/// <param name="json">Json.</param>
		/// <param name="id">Identifier.</param>
		/// <param name="type">Type.</param>
		public static object GetObjectFromJsonWithId(string json, string id, ObjectType type = ObjectType.None ) 
		{
			if(string.IsNullOrEmpty(json) == true || string.IsNullOrEmpty(id))
			{
				throw new System.Exception("Missing values for json or id");
			}
			// Create a JSON Object from the json
			JSONObject obj = JSONObject.Parse(json);
			// If null, non valid json
			if(obj == null) { return null; }
			// Split the id parameter from slashes
			string[] splits = id.Split(new char[]{'/'});
			JSONObject temp = obj;
			// the loop will iterate deeper in the json 
			// Each iteration is one level 
			// first_id/second_id/third_id
			// this will run three round (if all previous are found)
			for(int i = 0; i <splits.Length; i++)
			{
				// this the last run, so the deepest level required
				if(i == splits.Length - 1)
				{
					string str = splits[i];
					// Get the type to be returned
					// This is due to the used plugin for parsing json
					switch(type)
					{
					case ObjectType.None:
						return temp.GetValue(str);
					case ObjectType.Boolean:
						return temp.GetBoolean(str);
					case ObjectType.Number:
						return temp.GetNumber(str);
					case ObjectType.String:
						return temp.GetString(str);
					case ObjectType.Object:
						return temp.GetObject(str);
					}
				}
				// Get the current level object
				temp = temp.GetObject(splits[i]);
				// if null, it was not found in the file
				if(temp == null)
				{
					break;
				}
			}
			// Something went wrong
			return null;
		}
	}
}