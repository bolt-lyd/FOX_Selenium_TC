using System;
using System.Globalization;
using System.Collections.Generic;
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
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver.GetDriver();
			VerifyError err = new VerifyError();
			Random random = new Random();
			bool in_season = false;
			int games = 0;
			int scrolls = 20;
			string status = "";
			string date = "";
			int loc;
			int rand;
			int months;
			
			if (step.Name.Equals("Verify MLB Date")) {
				if (String.IsNullOrEmpty(step.Data)) {
					if(DataManager.CaptureMap.ContainsKey("IN_SEASON")) {
						in_season = bool.Parse(DataManager.CaptureMap["IN_SEASON"]);
						if(in_season) {
							TimeSpan time = DateTime.UtcNow.TimeOfDay;
							int now = time.Hours;
							int et = now - 4;
							if (et >= 0 && et < 11){
								log.Info("Current Eastern Time hour is " + et + ". Default to Yesterday.");
								step.Data = "YESTERDAY";
							}
							else {
								log.Info("Current Eastern Time hour is " + et + ". Default to Today.");
								step.Data = "TODAY";
							}				
						}
						else {
							step.Data = "WORLD SERIES";
						}
					}
					else {
						log.Warn("No IN_SEASON variable available.");
					}
				}

				steps.Add(new TestStep(order, "Verify Displayed Day on MLB", step.Data, "verify_value", "xpath", "//div[contains(@class,'scores-app-root')]/div[not(@style='display: none;')]//div[contains(@class,'week-selector')]//button/span[contains(@class,'title')]", wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else if (step.Name.Equals("Verify MLB Event")) {
				if (DataManager.CaptureMap.ContainsKey("IN_SEASON")) {
					DataManager.CaptureMap["GAME"] = step.Data;

					games = driver.FindElements("xpath", "(//a[@class='score-chip'])[" + step.Data +"]//div[contains(@class,'pregame-info')]").Count; 
					if (games > 0) {
						step.Data = "TeamSport_FutureEvent";
					}
					else {
						status = driver.FindElement("xpath", "(//a[@class='score-chip'])[" + step.Data +"]//div[contains(@class,'status-text')]").Text; 
						log.Info("Event status: " + status);
						if (status.Equals("POSTPONED")) {
							step.Data = "TeamSport_PostponedEvent";
						}
						else if (status.Contains("FINAL")) {
							step.Data = "TeamSport_PastEvent";
						}
						else {
							step.Data = "TeamSport_LiveEvent";
						}
					}
				}
				else {
					log.Warn("No IN_SEASON variable available or data is populated. Using data.");
				}
				
				steps.Add(new TestStep(order, "Run Event Template", step.Data, "run_template", "xpath", "", wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else if (step.Name.Equals("Select Regular Season MLB Date")) {
				string[] regularSeason = new string[] {"February", "March", "April", "May", "June", "July", "August", "September"};
				string[] choice = new string[] {"Left", "Right"};
				DateTime now = DateTime.Now;
				date = now.ToString("MMMM");
				
				// check if current month is in the regular season
				if(DataManager.CaptureMap.ContainsKey("IN_SEASON")) {
					in_season = bool.Parse(DataManager.CaptureMap["IN_SEASON"]);
					if(in_season) {
						loc = Array.IndexOf(regularSeason, date);
						if (loc == 0 || loc == regularSeason.Length-1) {
							// current month is start or end of regular season. can only click one way on arrows.
							months = random.Next(2, regularSeason.Length);
							if(loc == 0) {
								for (int i = 0; i < months; i++) {
									steps.Add(new TestStep(order, "Click Arrow Right", "", "click", "xpath", "//div[@class='qs-arrow qs-right']", wait));
									TestRunner.RunTestSteps(driver, null, steps);
									steps.Clear();									
								}
							}
							else {
								for (int i = 0; i < months; i++) {
									steps.Add(new TestStep(order, "Click Arrow Left", "", "click", "xpath", "//div[@class='qs-arrow qs-left']", wait));
									TestRunner.RunTestSteps(driver, null, steps);
									steps.Clear();									
								}								
							}
						}
						else {
							// current month is inside limits of regular season. can click both arrows.
							log.Info(loc);
							rand = random.Next(choice.Length);
							if(choice[rand].Equals("Left")) {
								months = random.Next(1, regularSeason.Length - loc);
							}
							else {
								months = random.Next(1, loc - 1);
							}
							for (int i = 0; i < months; i++) {
								steps.Add(new TestStep(order, "Click Arrow " + choice[rand], "", "click", "xpath", "//div[@class='qs-arrow qs-"+ choice[rand].ToLower() +"']", wait));
								TestRunner.RunTestSteps(driver, null, steps);
								steps.Clear();									
							}
						}							
						steps.Add(new TestStep(order, "Capture Month", "MONTH", "capture", "xpath", "//span[contains(@class,'qs-month')]", wait));
						TestRunner.RunTestSteps(driver, null, steps);
						steps.Clear();
					}
					else {
						// month is not in regular season. 
						// navigate to most recent month of season. assume end of last season.
						//div[@class='qs-arrow qs-left']
						// check if current month is in regular season
					}
					months = driver.FindElements("xpath", "//div[contains(@class,'qs-num')]").Count; 
					months = random.Next(1, months+1);
					steps.Add(new TestStep(order, "Capture Date", "DATE", "capture", "xpath", "(//div[contains(@class,'qs-num')])["+ months +"]", wait));
					steps.Add(new TestStep(order, "Select Date", "", "click", "xpath", "(//div[contains(@class,'qs-num')])["+ months +"]", wait));
					TestRunner.RunTestSteps(driver, null, steps);
					steps.Clear();	
				}
			}
			
			else {
				throw new Exception("Test Step not found in script");
			}

		}
	}
}