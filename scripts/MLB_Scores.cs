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
			string status = "";
			
			if (step.Name.Equals("Verify MLB Date")) {
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
					log.Warn("No IN_SEASON variable available. Using data.");
				}
				steps.Add(new TestStep(order, "Verify Displayed Day on MLB", step.Data, "verify_value", "xpath", "//div[contains(@class,'scores-app-root')]/div[not(@style='display: none;')]//div[contains(@class,'week-selector')]//button/span[contains(@class,'title')]", wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else if (step.Name.Equals("Verify MLB Event")) {
				if(DataManager.CaptureMap.ContainsKey("IN_SEASON")) {
					DataManager.CaptureMap.Add("GAME", step.Data);
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
					log.Warn("No IN_SEASON variable available. Using data.");
				}
				steps.Add(new TestStep(order, "Run Event Template", step.Data, "run_template", "xpath", "", wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else {
				throw new Exception("Test Step not found in script");
			}

		}
	}
}