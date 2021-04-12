using System;
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
			int size;
			int upper;
			string data = "";
			VerifyError err = new VerifyError();
			
			if (step.Name.Equals("Verify Scorechip Count")) {
				try {
					upper = Int32.Parse(step.Data);
				}
				catch (Exception e){
					log.Error("Expected data to be a numeral. Setting data to 0.");
					upper = 0;
				}
				size = driver.FindElements("xpath", "//div[contains(@class,'scorestrip')]//div[contains(@class,'score-chip')]").Count;
				if (size > 0 && size <= upper) {
					log.Info("Verification Passed. " + size + " is between 1 and " + upper); 
				}
				else {
					err.CreateVerificationError(step, "Number Between 1 and " + upper.ToString(), size.ToString());
					driver.TakeScreenshot(DataManager.CaptureMap["TEST_ID"] + "_verification_failure_" + DataManager.VerifyErrors.Count);
				}
			}
			
			else if(step.Name.Equals("Capture Team Info from Scorestrip")) {
				data = step.Data;
				steps.Add(new TestStep(order, "Capture Away Team Abbreviation", "AWAY_TEAM_ABB"+ data, "capture", "xpath", "((//a[contains(@class,'score-chip')])["+ data +"]//div[@class='teams']//div[contains(@class,'abbreviation')])[1]", wait));
				steps.Add(new TestStep(order, "Capture Away Team", "AWAY_TEAM"+ data, "capture", "xpath", "((//a[contains(@class,'score-chip')])["+ data +"]//div[@class='teams']//div[contains(@class,' team')])[1]", wait));
				steps.Add(new TestStep(order, "Capture Home Team Abbreviation", "HOME_TEAM_ABB"+ data, "capture", "xpath", "((//a[contains(@class,'score-chip')])["+ data +"]//div[@class='teams']//div[contains(@class,'abbreviation')])[2]", wait));
				steps.Add(new TestStep(order, "Capture Home Team", "HOME_TEAM"+ data, "capture", "xpath", "((//a[contains(@class,'score-chip')])["+ data +"]//div[@class='teams']//div[contains(@class,' team')])[2]", wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else {
				throw new Exception("Test Step not found in script");
			}
		}
	}
}