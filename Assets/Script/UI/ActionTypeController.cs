using UnityEngine;
using System.Collections;
using DataAnalytics;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Action type controller. is a component controlling the display of the action columns
/// It inherits from DisplayController to avoid duplicate.
/// </summary>
public class ActionTypeController : DisplayController 
{
	public Text [] m_texts;

	protected override void Awake()
	{
		this.m_container = new Container(this as IDisplay);
		InvokeRepeating("RedrawColumnGfx", 1.0f,1.0f);
		InvokeRepeating("RedrawText", 1.0f, 1.0f);
	}

	private void RedrawText()
	{
		this.m_container.RedrawTextWithValues(this.m_texts as IEnumerable<Text>);
	}
}
