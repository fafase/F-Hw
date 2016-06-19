using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace DataAnalytics
{
	/// <summary>
	/// IFileProcess is an IEnumerable for the json file.
	/// It allows iteration of each event contained in the file.
	/// </summary>
	public interface IFileProcess: IEnumerable<string>
	{
		long FileLength  { get; }
		float Progress { get; }
	}
	public sealed class FileProcess :  IFileProcess, IDisposable
	{
		private string m_path = null;
		private StreamReaderEnum m_stream = null;
		public long FileLength 
		{
			get
			{
				if(this.m_stream == null){ return -1; }
				return this.m_stream.FileLength;
			}
		}
		public float Progress 
		{
			get
			{
				if(this.m_stream == null){ return -1f; }
				return this.m_stream.Progress;
			} 
		}
		public FileProcess(string path)
		{
			this.m_path = path;
			this.m_stream = new StreamReaderEnum(this.m_path);
		}

		#region IEnumerable implementation
		public IEnumerator<string> GetEnumerator ()
		{
			return this.m_stream;
		}
		#endregion
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator();
		}
		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			this.m_stream = null;
		}

		#endregion
	}

	internal class StreamReaderEnum : IEnumerator<string> , IDisposable
	{
		private StreamReader m_sr = null;
		private string m_current = null;

		internal long FileLength 
		{
			get 
			{  
				if(this.m_sr == null){ return -1; }
				return this.m_sr.BaseStream.Length;
			} 
		}
		internal float Progress
		{
			get
			{
				if(this.m_sr == null) { return -1f; }
				return (float)this.m_sr.BaseStream.Position / (float)this.m_sr.BaseStream.Length;
			}
		}

		public string Current
		{
			get
			{
				if(this.m_sr == null || this.m_current == null)
				{
					throw new InvalidOperationException();
				}
				return this.m_current;
			}
		}

		public StreamReaderEnum(string filePath)
		{
			this.m_sr = new StreamReader(filePath);
		}

		#region IEnumerator implementation

		// This reads the next line in the json
		// Basically, it moves the cursor to the next json object in the file.
		public bool MoveNext ()
		{
			this.m_current = this.m_sr.ReadLine();
			return (this.m_current != null);
		}

		// We reached the end or wished to restart
		public void Reset ()
		{
			this.m_sr.DiscardBufferedData();
			this.m_sr.BaseStream.Seek(0, SeekOrigin.Begin);
			this.m_current = null;
		}

		object IEnumerator.Current {
			get {
				return this.m_current;
			}
		}

		#endregion

		#region IDisposable implementation
		private bool m_disposedValue = false;
		public void Dispose ()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (this.m_disposedValue == false)
			{
				if (disposing) { /* removing managed resources, none for now */ }
				this.m_current = null;
				if (this.m_sr != null) 
				{
					this.m_sr.Close();
					this.m_sr.Dispose();
				}
			}
			this.m_disposedValue = true;
		}

		~StreamReaderEnum()
		{
			Dispose(false);
		}
		#endregion
	}
}
	
