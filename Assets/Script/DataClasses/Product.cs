using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DataAnalytics;
using DataAnalytics.JSONData;

namespace DataAnalytics.ProcessData
{
	/// <summary>
	/// Data processor is used to convert the json file into .NET data
	/// It also detects duplicate
	/// </summary>
	public class DataProcessor
	{
		private HashSet<string> m_set = null;

		public DataProcessor()
		{
			this.m_set = new HashSet<string>();
		}

		public Product ProcessProduct(string json)
		{
			if(this.m_set == null){ throw new Exception("Null dictionary"); }
			if(json == null) { throw new NullReferenceException(); }

			// Convert data
			string eventId =  (string)JsonSerializer.GetObjectFromJsonWithId(json,"event_id", JsonSerializer.ObjectType.String);
			string productName = (string)JsonSerializer.GetObjectFromJsonWithId(json,"source", JsonSerializer.ObjectType.String);
			double unixTS = (double)JsonSerializer.GetObjectFromJsonWithId(json,"timestamp", JsonSerializer.ObjectType.Number);
			string productType = (string)JsonSerializer.GetObjectFromJsonWithId(json, "type", JsonSerializer.ObjectType.String);
			DateTime dt = ConvertTimestampUnixToDateTime(unixTS);

			// Check if hash set contains the event ID
			if(this.m_set.Contains(eventId) == true)
			{
				return null;
			}
			this.m_set.Add(eventId);
			return new Product(productName,eventId, dt, productType);
		}

		private static DateTime ConvertTimestampUnixToDateTime(double unixTimeStamp)
		{
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, 
				DateTimeKind.Utc);
			dt = dt.AddMilliseconds(unixTimeStamp);
			return dt;
		}
	}
	/// <summary>
	/// Product class represents the data of a single event.
	/// </summary>
	[Serializable]
	public class Product  
	{
		private static IDictionary<string, ProductEnum> s_typeDict = new Dictionary<string, ProductEnum>()
		{
			{ "view", 	ProductEnum.View },
			{ "metric", ProductEnum.Metric },
			{ "launch", ProductEnum.Launch },
			{ "user_interaction", ProductEnum.UserInteraction },
			{ "purchase", ProductEnum.Purchase },
			{ "suspend", ProductEnum.Suspend },
			{ "resume", ProductEnum.Resume },
			{ "marketing", ProductEnum.Marketing },
			{ "application_action", ProductEnum.ApplicationAction },
			{ "signin", ProductEnum.Signin },
			{ "rating", ProductEnum.Rating } 
		};

		private readonly string m_eventId = null;
		public string EventId { get { return this.m_eventId; } }
		private readonly string m_productName = null;
		public string ProductName { get { return this.m_productName; } }
		private DateTime m_timeStamp;
		public DateTime TimeStamp { get { return this.m_timeStamp; } }
		private ProductEnum m_productType = ProductEnum.None;
		public ProductEnum ActionType { get { return this.m_productType; } }

		public Product(string productName, string eventId, DateTime timeStamp, string type)
		{
			this.m_eventId = eventId;
			this.m_productName = productName;
			this.m_timeStamp = timeStamp;
			if(s_typeDict.ContainsKey(type) == false)
			{
				Debug.Log("Missing "+type);
			}
			else
			{
				this.m_productType = s_typeDict[type];
			} 
			if(productName != "product-a" && productName != "product-b" && productName != "product-c" ){
				Debug.Log(productName);
			}
		}
	}

	/// <summary>
	/// Type of action detected in the events
	/// </summary>
	public enum ProductEnum
	{
		None = -1, Launch, Signin, Resume, View, Metric, UserInteraction, Purchase, Marketing, ApplicationAction,  Rating, Suspend,
	}
}