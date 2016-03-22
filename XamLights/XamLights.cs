using System;
using System.Linq;
using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Settings.Abstractions;
using Plugin.Settings;
using Plugin.Compass;

namespace XamLights
{
	public class App : Application
	{
		public App ()
		{
			// The root page of your application
			MainPage = new XamLights.MainPage();
		}

		protected override void OnStart ()
		{

		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

