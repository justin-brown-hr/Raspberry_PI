using Assets.SimpleAndroidNotifications;
using Logic;
using System;
using System.Collections;
using UnityEngine;

public class BYV_Notification : MonoBehaviour
{
	public static BYV_Notification instance;

	private string _id_notification = "timeLeft";

	private string _message = "Enemies are attacking! There is almost no hope!";

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		switch (UnityEngine.Random.Range(1, 8))
		{
		case 1:
			_message = "Enemies are attacking! There is almost no hope!";
			break;
		case 2:
			_message = "We need you, the best shooter from the catapult!";
			break;
		case 3:
			_message = "The battle is coming, near. You need ...";
			break;
		case 4:
			_message = "There is no one behind the catapult!";
			break;
		case 5:
			_message = "The battle has already begun!You can...!";
			break;
		case 6:
			_message = "We do not know what to do.There are a lot of enemies!";
			break;
		case 7:
			_message = "You need around catapult!";
			break;
		}
	}

	public void StartNotificationWork()
	{
		if (NewDataController.instance.GetPlayerNotifications() != 0)
		{
			Inspection_TimeLeft();
		}
	}

	public void StopNotificataionWork()
	{
		if (NewDataController.instance.GetPlayerNotifications() == 0)
		{
			int @int = PlayerPrefs.GetInt(_id_notification);
			UnityEngine.Debug.Log("------- Какой ID Глушим из Памяти ------->" + @int);
			NotificationManager.Cancel(@int);
		}
	}

	private void Inspection_TimeLeft()
	{
		if (!PlayerPrefs.HasKey(_id_notification))
		{
			ScheduleCustom();
			return;
		}
		int @int = PlayerPrefs.GetInt(_id_notification);
		UnityEngine.Debug.Log("------- Какой ID Глушим из Памяти ------->" + @int);
		NotificationManager.Cancel(@int);
		ScheduleCustom();
	}

	public void ScheduleCustom()
	{
		DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 15, 0);
		dateTime = dateTime.AddDays(1.0);
		TimeSpan timeSpan = dateTime - DateTime.Now;
		NotificationParams notificationParams = new NotificationParams();
		notificationParams.Id = UnityEngine.Random.Range(0, int.MaxValue);
		notificationParams.Delay = timeSpan;
		notificationParams.Title = "The Catapult (Come to the game)";
		notificationParams.Message = _message;
		notificationParams.Ticker = "The Catapult (Come to the game)";
		notificationParams.Sound = true;
		notificationParams.CustomSound = "notifithecatapult";
		notificationParams.Vibrate = true;
		notificationParams.Light = true;
		notificationParams.SmallIcon = NotificationIcon.byve;
		notificationParams.LargeIcon = "app_icon";
		NotificationParams notificationParams2 = notificationParams;
		NotificationManager.SendCustom(notificationParams2);
		PlayerPrefs.SetInt(_id_notification, notificationParams2.Id);
		UnityEngine.Debug.Log("------------ Задача на уведомление установленна --------дата>" + timeSpan + "------- ID ------>" + notificationParams2.Id + " ---- Mesage ----->" + _message);
	}

	private void RegisterForNotif()
	{
		StartCoroutine(Inspection_TimeLeft_IOS());
	}

	private IEnumerator Inspection_TimeLeft_IOS()
	{
		yield return new WaitForSecondsRealtime(5f);
		UnityEngine.Debug.Log("--------> START-INSPECTION-IOS ----> ");
		ScheduleNotification_IOS();
	}

	private void ScheduleNotification_IOS()
	{
		DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 15, 0);
		dateTime = dateTime.AddDays(1.0);
		TimeSpan timeSpan = dateTime - DateTime.Now;
	}
}
