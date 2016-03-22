using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Plugin.Settings.Abstractions;
using Plugin.Settings;
using System.Threading.Tasks;
using Q42.HueApi.Interfaces;
using Q42.HueApi;
using System.Linq;
using Q42.HueApi.Models;
using System.Diagnostics;
using Toasts;
using Plugin.Compass;
using Plugin.TextToSpeech;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;

namespace XamLights
{
	public partial class MainPage : ContentPage
	{

		public IEnumerable<Light> MyLights;
		public IEnumerable<Scene> MyScenes;
		public IHueClient client;
		public MainPage ()
		{
			InitializeComponent ();

			SetColorButton.Clicked += async (sender, e) => 
			{
				if (client !=null){
					var random = new Random();
					var colorText = ColorEntry.Text;
					var notificator = DependencyService.Get<IToastNotificator>();

					try{
						var color =  String.IsNullOrEmpty(colorText) ? String.Format("{0:X6}", random.Next(0x1000000)) : colorText;
						var command = GetColorCommand(color);
						var idx = LightList.SelectedIndex;
						await client.SendCommandAsync(command, new [] {
							MyLights.ElementAt(idx).Id
						}); 
						
					}catch(Exception exc){
					}
				}
			};
				
		}

		double lastHeading = 0;
		protected async override void OnAppearing ()
		{
			CrossDeviceMotion.Current.SensorValueChanged += (sender, e) => {
				CompassLabel.Text = ""+e.Value;
			};

			CrossCompass.Current.CompassChanged += async (sender, e) => {
				CompassLabel.Text = ""+e.Heading;

				if (Math.Abs(e.Heading - lastHeading) < 5){
					return;
				}
				lastHeading = e.Heading;

				try{
					
					if (e.Heading > 180 && e.Heading < 360){
						var quadracticValue = (e.Heading - 360) * (e.Heading - 180) * -1/8100.0;
						var command = GetBrightnesCommand((byte)( quadracticValue * 255));
						var idx = LightList.SelectedIndex;
						await client.SendCommandAsync(command, new [] {
							MyLights.ElementAt(idx).Id
						}); 
					}else{
						var command = GetBrightnesCommand(0);
						await client.SendCommandAsync(command, new [] {"4"});
					}
					}catch(Exception ayy){
					CrossTextToSpeech.Current.Speak("Hey! Slow down. Bro.");
				}
			};

			if (client == null) {
				client = await GetClient ();
				if (client == null) {
					return;
				}
			}

			#if !DEBUG
			if (MyScenes == null) {
				
				MyScenes = await client.GetScenesAsync ();
				foreach (var light in MyScenes) {
					SceneList.Items.Add (light.Name);
				}
				SceneList.SelectedIndex = 0;
				SceneList.SelectedIndexChanged += async (object sender, EventArgs e) => {
					var scene = MyScenes.ElementAt(SceneList.SelectedIndex);
				};

			}
			#endif

			if (MyLights == null){
				MyLights = await client.GetLightsAsync ();
				foreach (var light in MyLights) {
					LightList.Items.Add (light.Name);
				}
				LightList.SelectedIndex = 0;
				CrossCompass.Current.Start();
				//CrossDeviceMotion.Current.Start(MotionSensorType.Gyroscope, MotionSensorDelay.Ui);
			}



		}

		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}


		private const string HueKey = "XAMLIGHTSKEY";

		public async Task<ILocalHueClient> GetClient(){

			IBridgeLocator locator = new HttpBridgeLocator();
			var notificator = DependencyService.Get<IToastNotificator>();
			var key = AppSettings.GetValueOrDefault<string> (HueKey);

			//Means this is the first time this app is being used with this bridge
			if (key == null) {
				await notificator.Notify (ToastNotificationType.Info, 
					             "Action", "Tap the big round button on the hue bridge", TimeSpan.FromSeconds (5));
			}

			//Get the ip address of the bridge
			var bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));

			#if DEBUG
			ILocalHueClient client = new LocalHueClient ("10.0.8.159:80"); //use ifconfig |grep inet
			#else
			ILocalHueClient client = new LocalHueClient (bridgeIPs.FirstOrDefault());
			#endif

			#if DEBUG
				client.Initialize ("newdeveloper");
			#else
			//Store the key if we don't have it, otherwise just use the existing one.
			if (key == null) {
				try {
					var appKey = await client.RegisterAsync ("XamLights", "313");
					AppSettings.AddOrUpdateValue (HueKey, appKey);
				} catch (Exception e){
						await notificator.Notify(ToastNotificationType.Error, 
							"Error", "Failed to connect to bridge. Please restart the app and try again.", TimeSpan.FromSeconds(3));
					return null;
				}
			} else {
				client.Initialize (key);
			}
			#endif

			await notificator.Notify(ToastNotificationType.Success, 
				"Connected", "Successfully connected to the bridge!", TimeSpan.FromSeconds(2));

			return client;
		}

		public LightCommand GetColorCommand(String color){
			var command = new LightCommand();
			command.On = true;
			command.TurnOn ().SetColor (color);
			return command;
		}

		public LightCommand GetBrightnesCommand(byte brightness){
			var command = new LightCommand();
			command.On = true;
			command.Brightness = brightness;
			return command;
		}

		public Scene CreateScene(string name, List<string> lights ){
			var scene = new Scene ();
			scene.Name = name;
			scene.Lights = lights;
			return scene;
		}

	}
}

