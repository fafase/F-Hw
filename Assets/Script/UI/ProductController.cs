using UnityEngine;
using System.Collections;
using DataAnalytics;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Controls the display of the product section. It inherits from DisplayController to control the drawing of the Column/Circle
/// </summary>
public class ProductController : DisplayController
{
	protected override void Awake()
	{
		this.m_container = new Container(this as IDisplay);
		InvokeRepeating("RedrawCircleGfx", 1.0f,1.0f);
	}
}
	
