using UnityEngine;
using System.Collections;
using DataAnalytics;
using System;
using System.Collections.Generic;
using UnityEngine.UI;


namespace DataAnalytics
{
	/// <summary>
	/// Display controller is used to update the UI elements, graph, text or circle
	/// </summary>
	public abstract class DisplayController : MonoBehaviour, IDisplay 
	{
		public RectTransform [] images;
		public IEnumerable<RectTransform> Images { get { return this.images as IEnumerable<RectTransform>; } }
		protected Container m_container = null;
		public int Length {get{ return this.images.Length;} }

		protected virtual void Awake()
		{
			this.m_container = new Container(this as IDisplay);
			InvokeRepeating("RedrawColumnGfx", 1.0f,1.0f);
		}
		protected void RedrawColumnGfx()
		{
			this.m_container.RedrawColumnGFX();
		}
		protected void RedrawCircleGfx()
		{
			this.m_container.RedrawCircleGfx();
		}
		public void SetDailyGraph(int hour)
		{
			this.m_container.SetImageWithValue(hour);
		}
	}

	public interface IDisplay
	{
		IEnumerable<RectTransform>Images { get; }
		int Length{ get; }
	}

	/// <summary>
	/// Container stores the values of each column and the highest value
	/// </summary>
	[Serializable]
	public class Container
	{
		private IDisplay m_display = null;
		private IEnumerable<RectTransform> m_images = null;
		private int [] m_dataStorage = null;
		private int m_maxValue = 0;

		public Container(IDisplay display)
		{
			this.m_display= display;
			this.m_images = this.m_display.Images;
			m_dataStorage = new int[this.m_display.Length];
			for(int i = 0; i < this.m_dataStorage.Length; i++)
			{
				m_dataStorage[i] = 0;
			}
		}

		public void SetImageWithValue(int value)
		{
			try
			{
				int i = ++this.m_dataStorage[value];
				if(i  > m_maxValue)
				{
					m_maxValue = i;
				}
			}catch(Exception){ Debug.Log(value);}
		}

		/// <summary>
		/// Redraws the column from the diagram
		/// </summary>
		public void RedrawColumnGFX()
		{
			int index = 0;
			// For each element, get the corresponding value from the array
			// anchor is clamped between 0 and 1.
			// Using the current element over the biggest element, we get a percentage
			foreach(RectTransform image in this.m_images)
			{
				int ds = this.m_dataStorage[index];
				Vector2 anchor = image.anchorMax;
				anchor.y = (float)ds / (float)this.m_maxValue;
				image.anchorMax = anchor;
				index++;
			}
		}

		public void RedrawTextWithValues(IEnumerable<Text> texts)
		{
			int index = 0;
			foreach(Text text in texts)
			{
				int ds = this.m_dataStorage[index];
				text.text = ds.ToString();
				index++;
			}
		}

		/// <summary>
		/// To draw the pie diagram, there is 3 circles (for each product)
		/// they are partially filled with the amount, then next is placed against the end edge with its own share,
		/// then last geos against that previous. This could repeat but we only have one more.
		/// Since all shares add up to 360 degrees, we get a full circle. 
		/// </summary>
		public void RedrawCircleGfx()
		{ 
			float max = 0;
			foreach(int i in this.m_dataStorage)
			{
				max += (float)i; // get the amount of events
			}
			if(max <= 0.0f){ return; }
			int k = 0;
			float angle = 0.0f;
			foreach(RectTransform image in this.m_images)
			{
				float temp = ((float)this.m_dataStorage[k] / max); // Divide the amount for each product by the total
				image.GetComponent<Image>().fillAmount = temp; // fill amount fills the circle in a rotating process (value is 0 to 1)
				image.localRotation = Quaternion.Euler(0f, 0f, 360f - angle); // Rotate the image to align with the outer edge of the previous
				k++;
				angle += temp * 360f;
			}
		}
	}

}
