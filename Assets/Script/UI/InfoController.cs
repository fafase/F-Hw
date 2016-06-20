using UnityEngine;
using System.Collections;
using DataAnalytics;
using UnityEngine.UI;
using System;

/// <summary>
/// Controls the info display. Receives info from UIAction and update the display of the info
/// Contains amount of events, percentage done, duplicates and percentage of duplicates.
/// </summary>
public class InfoController : MonoBehaviour , IInfo
{
	[SerializeField] private Text m_duplicate = null;
	[SerializeField] private Text m_duplicatePercentage = null;
	[SerializeField] private Text m_eventsProcessed = null;
	[SerializeField] private Text m_eventsProcessedPercentage = null;

	private InfoContainer m_info = null;

	public void Init()
	{
		this.m_info = new InfoContainer();
		InvokeRepeating("SetInfoGfx", 1.0f, 1.0f);
	}

	public void SetDuplicate()
	{
		this.m_info.SetDuplicate();
	}
	public void SetNewEvent()
	{
		this.m_info.SetNewEvent();
	}

	private void SetInfoGfx()
	{
		int duplicate = this.m_info.Duplicate;
		this.m_duplicate.text = duplicate.ToString();
		int processedEvent = this.m_info.EventsProcessed;
		this.m_eventsProcessed.text = processedEvent.ToString();
		this.m_duplicatePercentage.text = (((float)duplicate / (float)processedEvent) * 100f).ToString("0.00") + "%";
		this.m_eventsProcessedPercentage.text = ((this.m_info.Progress) * 100f).ToString("0.0") + "%";
	}
	public void SetProgress(float progress)
	{
		this.m_info.SetProgress(progress);
	}
}
namespace DataAnalytics
{
	public interface IInfo
	{
		
	}
	[Serializable]
	public class InfoContainer
	{
		private int m_duplicate = 0;
		public int Duplicate{ get{ return this.m_duplicate; } }
		private int m_eventProcessed = 0;
		public int EventsProcessed { get { return this.m_eventProcessed;} }
		private float m_progress = 0.0f;
		public float Progress { get { return this.m_progress; } }
		public InfoContainer()
		{
			
		}

		public void SetDuplicate()
		{
			this.m_duplicate++;
		}

		public void SetNewEvent()
		{
			++this.m_eventProcessed;
		}
		public void SetProgress(float progress)
		{
			this.m_progress = progress;
		}
	}
}