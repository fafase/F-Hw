using UnityEngine;
using System.Collections;
using System;
using System.IO;
using DataAnalytics;
using DataAnalytics.ProcessData;

public interface IUIController
{
	bool StartProcess(string path);
	event EventHandler<PathEventArg> RaiseStartProcess;
	void RegisterLoadingListener(LoaderControlSystem lcs);
}

/// <summary>
/// User interface controller.
/// Events from the LoaderControllerSystem are propagated to UI elements
/// </summary>
public class UIController : IUIController
{
	private UIAction m_uiAction = null;
	private LoaderControlSystem m_lcs = null; 

	/// <summary>
	/// Ctor initializes the UIAction component
	/// </summary>
	/// <param name="program">Program.</param>
	public UIController(Program program)
	{
		// Item is found using Unity GetComponent method
		this.m_uiAction = program.GetComponent<UIAction>();	
		this.m_uiAction.InitWithUIController(this as IUIController);
	}

	public event EventHandler<PathEventArg> RaiseStartProcess;
	private void OnStartProcess(PathEventArg arg)
	{
		if(RaiseStartProcess != null)
		{
			RaiseStartProcess(this, arg);
		}
	}

	public bool StartProcess(string path)
	{
		if(File.Exists(path) == false)
		{
			return false;
		}

		OnStartProcess(new PathEventArg(path));
		return true;
	}

	/// <summary>
	/// Registers the listening methods for LoaderControlSystem
	/// This is called from Program class
	/// </summary>
	/// <param name="lcs">Lcs.</param>
	public void RegisterLoadingListener(LoaderControlSystem lcs)
	{
		this.m_lcs = lcs;
		this.m_lcs.RaiseLoadingPercentage += Lcs_RaiseLoadingPercentage;
		this.m_lcs.RaiseNewProduct += Lcs_RaiseNewProduct;
		this.m_lcs.RaiseMinAmountEventLoaded += Lcs_RaiseMinAmountEventLoaded;
		this.m_lcs.RaiseDuplicate += Lcs_RaiseDuplicate;
	}

	/// <summary>
	/// Called when a duplicate is found
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void Lcs_RaiseDuplicate (object sender, EventArgs e)
	{
		this.m_uiAction.SetDuplicate();
	}

	/// <summary>
	/// Called when 1000 events have been processed
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void Lcs_RaiseMinAmountEventLoaded (object sender, EventArgs e)
	{
		this.m_uiAction.ShowButtonPanel();
		this.m_lcs.RaiseMinAmountEventLoaded -= Lcs_RaiseMinAmountEventLoaded;
	}

	/// <summary>
	/// Called when a new json file is parsed without duplicate
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void Lcs_RaiseNewProduct (object sender, NewProductEventArg e)
	{
		DateTime dt = e.product.TimeStamp;
		int hour = dt.Hour;
		this.m_uiAction.SetNewProduct(hour, SetProductInt(e.product.ProductName), (int)e.product.ActionType);
		this.m_uiAction.SetProgress(e.progress);

	}
	private int SetProductInt(string value)
	{
		char c = value[value.Length - 1];
		return c - 97;
	}

	/// <summary>
	/// Called to update the percentage of processed events
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void Lcs_RaiseLoadingPercentage (object sender, LoadingPercentageEventArg e)
	{
		this.m_uiAction.SetLoader(e.currentPercentage);
	}
}
public class PathEventArg:EventArgs
{
	public readonly string path = null;
	public PathEventArg(string path)
	{
		this.path = path;
	}
}
