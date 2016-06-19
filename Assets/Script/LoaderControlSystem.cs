using UnityEngine;
using System.Collections;
using System;
using DataAnalytics.JSONData;
using DataAnalytics.ProcessData;

namespace DataAnalytics
{
	/// <summary>
	/// Used to propagate data from the DataProcessor and IFileProcess instance
	/// </summary>
	public class LoaderControlSystem 
	{
		public event EventHandler<EventArgs> RaiseMinAmountEventLoaded;
		private void OnMinAmountEventLoaded(EventArgs arg)
		{
			if(RaiseMinAmountEventLoaded != null)
			{
				RaiseMinAmountEventLoaded(this, arg);
			}
		}

		public event EventHandler<LoadingPercentageEventArg> RaiseLoadingPercentage;
		private void OnLoadingPercentage(LoadingPercentageEventArg arg)
		{
			if(RaiseLoadingPercentage != null)
			{
				RaiseLoadingPercentage(this, arg);
			}
		}

		public event EventHandler<NewProductEventArg> RaiseNewProduct;
		private void OnNewProduct(NewProductEventArg arg)
		{
			if(RaiseNewProduct != null)
			{
				RaiseNewProduct(this, arg);
			}
		}

		public event EventHandler<EventArgs> RaiseDuplicate;
		private void OnDuplicate(EventArgs arg)
		{
			if(RaiseDuplicate != null)
			{
				RaiseDuplicate(this, arg);
			}
		}

		private IFileProcess m_fileProcess = null;
		private int m_processedEvents = 0;
		private int m_amountLimit = 0;

		private float Percentage
		{
			get
			{
				return (float)this.m_processedEvents / (float) this.m_amountLimit;
			}
		}

		/// <summary>
		/// Ctor needs a MonoBehaviour instance to run the coroutine.
		/// </summary>
		/// <param name="mb">Mb.</param>
		/// <param name="path">Path.</param>
		/// <param name="fileProcess">File process.</param>
		/// <param name="amountLimit">Amount limit.</param>
		public LoaderControlSystem(MonoBehaviour mb,string path, IFileProcess fileProcess, int amountLimit)
		{
			if(fileProcess == null) {  throw new NullReferenceException(); }
			this.m_fileProcess = fileProcess;

			this.m_amountLimit = amountLimit;
			// Start the coroutine attached to the Program instance
			mb.StartCoroutine(StartParse(path));
		}

		/// <summary>
		/// Coroutine are specific to Unity, they allow asynchronous actions.
		/// In this case, each frame, one event is treated, this allows to use the application without waiting for all events to be done.
		/// </summary>
		/// <returns>The parse.</returns>
		/// <param name="path">Path.</param>
		private IEnumerator StartParse(string path)
		{
			// Create a DataProcessor 
			DataProcessor dp = new DataProcessor();
			// Iterate the file one event at a time
			foreach(string json in this.m_fileProcess)
			{
				// Get a product
				Product product = dp.ProcessProduct(json);
				if(product == null) // if null, it was a duplicate
				{
					OnDuplicate(null);
					continue;
				}
				// Increase processed event to display when min amount is reached
				if(++this.m_processedEvents >= this.m_amountLimit)
				{
					OnMinAmountEventLoaded(null);
				}
				// Update percentage
				OnLoadingPercentage(new LoadingPercentageEventArg(Percentage));
				// Propagate new product
				OnNewProduct(new NewProductEventArg(product, this.m_fileProcess.Progress));
				// Return the control to program, next frame it will start again from here
				yield return null;
			}
		}
	}

	public class LoadingPercentageEventArg : EventArgs
	{
		public readonly float currentPercentage = 0.0f;
		public LoadingPercentageEventArg(float newPercentage)
		{
			this.currentPercentage = newPercentage;
		}
	}
	public class NewProductEventArg : EventArgs
	{
		public readonly Product product = null;
		public readonly float progress = 0.0f;
		public NewProductEventArg(Product product, float progress)
		{
			this.product = product;
			this.progress = progress;
		}
	}
}
