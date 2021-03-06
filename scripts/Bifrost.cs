using System;
using System.Globalization;
using System.Collections.Generic;
using SeleniumProject.Utilities;
using OpenQA.Selenium;
using log4net;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Net;

namespace SeleniumProject.Function
{
	public class Script : ScriptingInterface.IScript
	{		
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public void Execute(DriverManager driver, TestStep step)
		{
			JObject jsonValue;
			JObject def;
			long order = step.Order;
			string wait = step.Wait != null ? step.Wait : "";
			List<TestStep> steps = new List<TestStep>();
			string leaderboard = "";
			IWebElement ele;
			VerifyError err = new VerifyError();
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver.GetDriver();
            OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(driver.GetDriver());
			
			string fileLocation = "https://api.foxsports.com/bifrost/v1/nascar/scoreboard/segment/2021"+step.Data +"?groupId=2&apikey=jE7yBJVRNAwdDesMgTzTXUUSx1It41Fq";
			var jsonFile = new WebClient().DownloadString(fileLocation);
			var json = JObject.Parse(jsonFile);
			jsonValue = json;
            DataManager.CaptureMap["IND_EVENTID"] = json["currentSectionId"].ToString();
			log.Info("Current Section ID from Bifrost: " + DataManager.CaptureMap["IND_EVENTID"]);
			
			foreach (JToken race in jsonValue["sectionList"]) {
				if (DataManager.CaptureMap["IND_EVENTID"] == race["id"].ToString()) {
					
					def = (JObject)(race.SelectToken("events") as JArray).First;
					DataManager.CaptureMap["IND_EVENT"] = def.Value<string>("title");
					DataManager.CaptureMap["IND_TRACK"] = def.Value<string>("subtitle");
					DataManager.CaptureMap["IND_LOC"] = def.Value<string>("subtitle2");
					DataManager.CaptureMap["IND_TIME"] = def.Value<string>("eventTime");
					DataManager.CaptureMap["IND_CHANNEL"] = def.Value<string>("tvStation");
					
					if (def.ContainsKey("leaderboard")) {
						//(string) = def.Value<string>("$..leaderboard.title");
						leaderboard = def.SelectToken("$..leaderboard.title").ToString();
						log.Info(leaderboard);
					}
				}
			}
			


		}
	}
}