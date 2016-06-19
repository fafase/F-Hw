using UnityEngine;
using System.Collections;
using DataAnalytics;
using DataAnalytics.JSONData;

/// <summary>
/// Program. It is the entry class for the program.
/// Inehriting from MonoBehaviour makes it a specific class for Unity, the engine is able to run lifecycle methods.
/// Unity has a frame system, so each frame, the Update is run. For this case, no update is used.
/// Awake is a replacement for the ctor of a class. Due to internal functioning, Unity does not allow to use new and the ctor.
/// Awake or Start are used for initialisation.
/// 
/// The script is attached to a game object in the scene for the engine to register it as an instance of the class.
/// </summary>
public class Program : MonoBehaviour 
{
	private IUIController m_uiCtrl = null;

	// Used for initialization
	// Is called automatically by the engine
	private void Awake () 
	{
		this.m_uiCtrl = new UIController(this) as IUIController;
		this.m_uiCtrl.RaiseStartProcess += UiAction_RaiseStartProcess;
	}

	// Listener for the UIController to propagate the event.
	private void UiAction_RaiseStartProcess (object sender, PathEventArg e)
	{
		string path = e.path;
		// IFileProcess parses the json
		IFileProcess fileProcess = new FileProcess(path);
		// Parse the json into actual data
		LoaderControlSystem lcs = new LoaderControlSystem(this, path, fileProcess, 1000);
		this.m_uiCtrl.RegisterLoadingListener(lcs);
	}
}
