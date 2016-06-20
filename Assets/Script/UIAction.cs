using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// User interface action.
/// It controls the UI elements in the Unity Scene
/// Each element is linked to that class. UIAction receives data from the UIController and updates the UI elements
/// </summary>
public class UIAction : MonoBehaviour 
{
	private IUIController m_uiController = null;

	[SerializeField] private InputField m_inputField = null;
	[SerializeField] private Scrollbar m_scrollBar = null;
	[SerializeField] private GameObject m_startPanel = null;
	[SerializeField] private GameObject m_loaderPanel = null;
	[SerializeField] private DailyController m_dailyController = null;
	[SerializeField] private GameObject m_buttonPanel = null;
	[SerializeField] private InfoController m_infoPanel = null;
	[SerializeField] private ProductController m_productPanel = null;
	[SerializeField] private TopProductController m_topProdCtlr = null;
	[SerializeField] private GameObject m_productA = null;
	[SerializeField] private GameObject m_productB = null;
	[SerializeField] private GameObject m_productC = null;
	[SerializeField] private GameObject m_productCircle = null;

	private IEnumerable<GameObject> m_panels = null;

	/// <summary>
	/// Called from the UIController on start
	/// </summary>
	/// <param name="uiCtrl">User interface ctrl.</param>
	public void InitWithUIController (IUIController uiCtrl) 
	{
		this.m_uiController = uiCtrl;
		if(this.m_inputField == null){ throw new NullReferenceException("Missing Ref for InputField"); }
		if(this.m_dailyController == null){ throw new NullReferenceException("Missing Ref for DailyController"); }

		if(this.m_startPanel == null) { throw new NullReferenceException("Missing Start object ref"); }
		if(this.m_loaderPanel == null) { throw new NullReferenceException("Missing Loader object ref"); }
		if(m_scrollBar == null) { throw new NullReferenceException("Missing ScrollBar object ref"); }
		if(this.m_buttonPanel == null) { throw new NullReferenceException("Missing Button panel object ref"); }
		if(this.m_infoPanel == null) { throw new NullReferenceException("Missing Info panel object ref"); }
		if(this.m_productPanel == null) { throw new NullReferenceException("Missing Product panel object ref"); }
		if(this.m_topProdCtlr == null) { throw new NullReferenceException("Missing TopActionController object ref"); }

		SetPanelCollection();
		SetInitialStateUI();

		this.m_infoPanel.Init();
		this.m_productPanel.Init();
		this.m_topProdCtlr.Init();
		this.m_dailyController.Init();
	}
		
	/// <summary>
	/// Sets all panel in a collection.
	/// This enables iteration over the collection. Each item is assigned once in that method
	/// </summary>
	private void SetPanelCollection()
	{
		IList<GameObject> list = new List<GameObject>()
		{
			this.m_startPanel, this.m_buttonPanel, this.m_infoPanel.gameObject, this.m_loaderPanel, 
			this.m_dailyController.gameObject, this.m_productPanel.gameObject, 
			this.m_productA, this.m_productB, this.m_productC, this.m_productCircle
		};
		this.m_panels = list as IEnumerable<GameObject>;
	}

	/// <summary>
	/// Deactivate all panels
	/// </summary>
	private void ResetAllPanel()
	{
		foreach(GameObject go in this.m_panels)
		{
			go.SetActive(false);
		}

	}

	private void SetInitialStateUI()
	{
		ResetAllPanel();
		this.m_startPanel.SetActive(true);
	}

	/// <summary>
	/// Called to update the loading bar on start
	/// </summary>
	/// <param name="currentPercentage">Current percentage.</param>
	public void SetLoader(float currentPercentage)
	{
		this.m_scrollBar.size = currentPercentage;
	}

	/// <summary>
	///  Called to inform a new product was parsed from the json and should be considered on the UI.
	/// The data are propagated to appropriate UI elements.
	/// </summary>
	/// <param name="hour">Hour.</param>
	/// <param name="productName">Product name.</param>
	/// <param name="actionName">Action name.</param>
	public void SetNewProduct(int hour, int productName, int actionName)
	{
		this.m_dailyController.SetDailyGraph(hour);
		this.m_infoPanel.SetNewEvent();
		this.m_productPanel.SetDailyGraph(productName);
		this.m_topProdCtlr.SetNewEvent(productName, actionName);
	}

	/// <summary>
	/// Indicates a duplicate was found
	/// </summary>
	public void SetDuplicate()
	{
		this.m_infoPanel.SetDuplicate();
	}

	public void ShowButtonPanel()
	{
		ResetAllPanel();
		this.m_buttonPanel.SetActive(true);
	}

	/// <summary>
	/// Updates the progress UI Element
	/// </summary>
	/// <param name="progress">Progress.</param>
	public void SetProgress(float progress)
	{
		this.m_infoPanel.SetProgress(progress);
	}

	/// <summary>
	/// Listener to the Button starting the process
	/// </summary>
	public void ButtonAction_StartProcess()
	{
		string path = this.m_inputField.text;

		bool success = this.m_uiController.StartProcess(path);
		if(success == true)
		{
			ResetAllPanel();
			this.m_loaderPanel.SetActive(true);
			this.m_infoPanel.Init();
			return;
		}
		// No success, indicate the file does not exist
		this.m_inputField.text = "File does not exist";
	}

	/// <summary>
	/// Listener to the button displaying the Daily usage
	/// </summary>
	public void ButtonAction_ShowDaily()
	{
		ResetAllPanel();
		this.m_buttonPanel.SetActive(true);
		this.m_dailyController.gameObject.SetActive(true);
	}

	/// <summary>
	/// Listener to the button displaying the info
	/// </summary>
	public void ButtonAction_ShowInfo()
	{
		ResetAllPanel();
		this.m_buttonPanel.SetActive(true);
		this.m_infoPanel.gameObject.SetActive(true);
	}

	/// <summary>
	/// Listener to the button displaying the product usage
	/// </summary>
	public void ButtonAction_ShowProducts()
	{
		ResetAllPanel();

		this.m_buttonPanel.SetActive(true);
		this.m_productPanel.gameObject.SetActive(true);
		this.m_productCircle.SetActive(true);
	}
	/// <summary>
	/// Listener displaying the product A information
	/// </summary>
	public void ButtonAction_ProductA()
	{
		ResetAllPanel();
		this.m_buttonPanel.SetActive(true);
		this.m_productPanel.gameObject.SetActive(true);
		this.m_productA.SetActive(true);
	}
	/// <summary>
	/// Listener displaying the product B information
	/// </summary>
	public void ButtonAction_ProductB()
	{
		ResetAllPanel();
		this.m_buttonPanel.SetActive(true);
		this.m_productPanel.gameObject.SetActive(true);
		this.m_productB.SetActive(true);
	}
	/// <summary>
	/// Listener displaying the product C information
	/// </summary>
	public void ButtonAction_ProductC()
	{
		ResetAllPanel();
		this.m_buttonPanel.SetActive(true);
		this.m_productPanel.gameObject.SetActive(true);
		this.m_productC.SetActive(true);
	}

}
