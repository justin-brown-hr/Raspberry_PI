using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Cabinet
{
	/// <summary>
	/// Hands the claw tries a player won over to the host Raspberry Pi.
	///
	/// The game runs inside Waydroid and cannot reach the Pi's GPIO, so it sends one UDP
	/// line to the host across the Waydroid bridge:  AWARD &lt;tries&gt; &lt;score&gt;
	/// `cabinet-award-listener.py` on the Pi turns that into one relay pulse per try, which
	/// is how the claw machine receives its credits.
	///
	/// A copy is also written to the app's own storage as an audit trail - handy when the
	/// client asks what a machine handed out, and for debugging a miscount.
	/// </summary>
	public static class CabinetAward
	{
		private const string FolderName = "awards";

		// Host side of the Waydroid bridge
		private const string HostAddress = "192.168.240.1";
		private const int HostPort = 47801;

		public static void Write(int tries, int score)
		{
			if (tries <= 0)
			{
				return;
			}
			SendToHost(tries, score);
			try
			{
				string dir = Path.Combine(Application.persistentDataPath, FolderName);
				Directory.CreateDirectory(dir);
				string stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture);
				string path = Path.Combine(dir, "award-" + stamp + ".json");
				string json = string.Format(CultureInfo.InvariantCulture,
					"{{\"tries\":{0},\"score\":{1},\"utc\":\"{2}\"}}", tries, score, stamp);

				// Write beside the target then move, so the watcher never sees a half-written file
				string temp = path + ".tmp";
				File.WriteAllText(temp, json);
				File.Move(temp, path);
				Debug.Log("CABINETAWARD: wrote " + path + " -> " + json);
			}
			catch (Exception e)
			{
				Debug.LogError("CABINETAWARD: could not write award file (" + e.Message + ")");
			}
		}

		// The part that actually credits the claw
		private static void SendToHost(int tries, int score)
		{
			try
			{
				string message = string.Format(CultureInfo.InvariantCulture, "AWARD {0} {1}", tries, score);
				byte[] payload = Encoding.UTF8.GetBytes(message);
				using (UdpClient client = new UdpClient())
				{
					client.Send(payload, payload.Length, HostAddress, HostPort);
				}
				Debug.Log("CABINETAWARD: sent \"" + message + "\" to " + HostAddress + ":" + HostPort);
			}
			catch (Exception e)
			{
				Debug.LogError("CABINETAWARD: could not reach the host relay (" + e.Message + ")");
			}
		}
	}
}
