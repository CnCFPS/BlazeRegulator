// -----------------------------------------------------------------------------
//  <copyright file="RenLogClient.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace BlazeRegulator.Core.Net
{
	using System;
	using System.IO;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using Atlantis.Linq;
	using IO;
	using Linq;

	public class RenLogClient
    {
		#region Fields

		private readonly TcpClient _client;
		private StreamReader reader;
		private NetworkStream stream;
		private readonly Thread thread;

	    private bool _stopRequested;

		#endregion

		#region Constructors

		public RenLogClient()
		{
			_client = new TcpClient();
			thread = new Thread(ThreadCallback);
		}

		#endregion
		
		#region Events

		// ReSharper disable InconsistentNaming
		public event Action<String> RenLogEvent;
		public event Action<String> SSGMLogEvent;
		public event Action<String> GameLogEvent;
		public event Action<String> ConsoleOutputEvent;

		public event EventHandler RenLogDisconnectEvent;
		// ReSharper restore InconsistentNaming

		#endregion

		#region Properties

		public bool Connected
		{
			get { return _client != null && _client.Connected; }
		}

		#endregion

		#region Methods

		public void Start(String host, int port)
		{
			try
			{
				_client.Connect(host, port);
			}
			catch (SocketException e)
			{
				Log.Instance.Error("An error occurred: {0}", e.Message);
				RenLogDisconnectEvent.Raise(this, EventArgs.Empty);
			}

			thread.Start();
		}

		public void Stop()
		{
            _stopRequested = true;

		    Thread.Sleep(250);
		    if (thread != null && !_stopRequested)
		    {
		        thread.Abort();
		    }

			if (_client != null)
			{
				_client.Close();
			}
		}

		private static String RemoveTimestamp(String str)
		{
			return str.Substring(str.IndexOf(' ') + 1).TrimEnd();
		}

		private void OnLogReceived(String s)
		{
			if (String.IsNullOrEmpty(s))
			{
				Log.Instance.WriteLine("Null message received");
				return;
			}

			String snum = s.Substring(0, 3);
			int num;
			if (!Int32.TryParse(snum, out num)) return;

			String line = s.Substring(3);
			if (num < 3)
			{
				if (line.Split(' ').Length < 2) return;

				line = RemoveTimestamp(line);
			}

			// 000 - SSGM log messages
			// 001 - Gamelog messages
			// 002 - Renlog messages (a copy of everything written to the renlog files)
			// 003 - Console Output
			switch (num)
			{
				case 0:
				{
					SSGMLogEvent.Raise(line);
					break;
				}
				case 1:
				{
					GameLogEvent.Raise(line);
					break;
				}
				case 2:
				{
					RenLogEvent.Raise(line);
					break;
				}
				case 3:
				{
					ConsoleOutputEvent.Raise(line);
					break;
				}
			}
		}

		private void ThreadCallback(Object o)
		{
			if (_client == null)
			{
				Thread.CurrentThread.Abort();
				return;
			}

		    try
		    {
		        stream = _client.GetStream();
		        reader = new StreamReader(stream, Encoding.UTF8);

		        var sb = new StringBuilder();
		        while (Connected)
		        {
		            if (_stopRequested)
		            {
		                break;
		            }

		            if (!_client.Connected)
		            {
		                // TODO: Reconnect.
		                RenLogDisconnectEvent.Raise(this, EventArgs.Empty);
		                break;
		            }

		            if (stream.DataAvailable)
		            {
		                Int32 c;
		                while ((c = reader.Read()) >= 0)
		                {
		                    if (c == 0)
		                    {
		                        OnLogReceived(sb.ToString().Trim());
		                        sb.Clear();
		                    }
		                    else
		                    {
		                        sb.Append((char)c);
		                    }
		                }
		            }
		        }
		    }
		    catch (IOException)
		    {
		        // Omnomnom!
                _client.Close();
		    }
		    catch (SocketException e)
		    {
		        Log.Instance.Error("An error occured on the RenLog client. {0}", e.Message);
		        Log.Instance.Error("Error code: {0:0.0}", (int)e.SocketErrorCode);
		    }
		    catch (ThreadAbortException)
		    {
		        // Omnomnom!
                // We don't want to do anything here, just catch it so it doesn't propagate to the main layer.
		    }
		}

		#endregion
    }
}
