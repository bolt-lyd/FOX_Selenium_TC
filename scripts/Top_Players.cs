using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SeleniumProject.Utilities;
using OpenQA.Selenium;
using log4net;
using System.Threading;

namespace SeleniumProject.Function
{
	public class Script : ScriptingInterface.IScript
	{		
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public void Execute(DriverManager driver, TestStep step)
		{
			long order = step.Order;
			string wait = step.Wait != null ? step.Wait : "";
			List<TestStep> steps = new List<TestStep>();
			List<string> confTeams = new List<string>();
			Random random = new Random();
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver.GetDriver();
			VerifyError err = new VerifyError();
			int count = 0;
		string[,] topPlayers = new string[10, 2]{ {"", ""}, { "", ""}, { "", ""}, { "", ""}, { "", ""}, { "", ""}, { "", ""}, { "", ""}, { "", ""}, { "", ""} };
			
			if (step.Name.Contains("Choose Top Player by Sport")) {
				// set teams for each conference
				switch(step.Data) {
					case "MLB":
						topPlayers = new string[10, 2] { { "Mike Trout", "Los Angeles Angels"}, { "Fernando Tatis Jr.", "San Diego Padres"}, { "Mookie Betts", "Los Angeles Dodgers"}, { "Bryce Harper", "Philadelphia Phillies"}, { "Aaron Judge", "New York Yankees"}, { "Freddie Freeman", "Atlanta Braves"}, { "Shane Bieber", "Cleveland Indians"}, { "Clayton Kershaw", "Los Angeles Dodgers"}, { "Trevor Bauer", "Cincinnati Reds"}, { "Gerrit Cole", "New York Yankees"} };
						break;
					case "NBA":
						topPlayers = new string[10, 2] { { "James Harden", "Brooklyn Nets"}, { "Giannis Antetokounmpo", "Milwaukee Bucks"}, { "LeBron James", "Los Angeles Lakers"}, { "Luka Doncic", "Dallas Mavericks"}, { "Kawhi Leonard", "LA Clippers"}, { "Trae Young", "Atlanta Hawks"}, { "Anthony Davis", "Los Angeles Lakers"}, { "Anthony Edwards", "Minnesota Timberwolves"}, { "LaMelo Ball", "Charlotte Hornets"}, { "James Wiseman", "Golden State Warriors"} };
						break;
					case "NFL":
						topPlayers = new string[10, 2] { { "Patrick Mahomes II", "Kansas City Chiefs"}, { "Russell Wilson", "Seattle Seahawks"}, { "Dalvin Cook", "Minnesota Vikings"}, { "Derrick Henry", "Tennessee Titans"}, { "DK Metcalf", "Seattle Seahawks"}, { "Travis Kelce", "Kansas City Chiefs"}, { "Aaron Donald", "Los Angeles Rams"}, { "Tyrann Mathieu", "Kansas City Chiefs"}, { "Justin Tucker", "Baltimore Ravens"}, { "Joe Burrow", "Cincinnati Bengals"} };
						break;
					case "NHL":
						topPlayers = new string[10, 2] { { "Alex Ovechkin", "Washington Capitals"}, { "David Pastrnak", "Boston Bruins"}, { "Patrick Kane", "Chicago Blackhawks"}, { "Nikita Kucherov", "Tampa Bay Lightning"}, { "Artemi Panarin", "New York Rangers"}, { "Tuukka Rask", "Boston Bruins"}, { "Marc-Andre Fleury", "Vegas Golden Knights"}, { "Leon Draisaitl", "Edmonton Oilers"}, { "Andrei Vasilevskiy", "Tampa Bay Lightning"}, { "Carey Price", "Montreal Canadiens"} };
						break;
					default :
						
						break;
				} 
				
				// if ran once before, choose a different number
				if(DataManager.CaptureMap.ContainsKey("COUNT")) {
					do {
					   count = random.Next(0, 10);
					   log.Info("Random number [" + count + "] vs Stored Number [" + DataManager.CaptureMap["COUNT"] + "]");
					} while (count == Int32.Parse(DataManager.CaptureMap["COUNT"]));	
				}
				else {
					count = random.Next(0, 10);					
				}

				DataManager.CaptureMap["COUNT"] = count.ToString();
				DataManager.CaptureMap["TOP_PLAYER"] = topPlayers[count, 0];
				DataManager.CaptureMap["TOP_PLAYER_UP"] = DataManager.CaptureMap["TOP_PLAYER"].ToUpper();;
				DataManager.CaptureMap["TOP_PLAYER_TEAM"] = topPlayers[count, 1];
				DataManager.CaptureMap["TOP_PLAYER_TEAM_UP"] = DataManager.CaptureMap["TOP_PLAYER_TEAM"].ToUpper();;
			}
			
			else {
				throw new Exception("Test Step not found in script");
			}
		}
	}
}