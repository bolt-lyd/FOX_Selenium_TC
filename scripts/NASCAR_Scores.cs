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
			IWebElement ele;
			DateTime today;
			string data = "";
			string xpath = "";
			VerifyError err = new VerifyError();
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver.GetDriver();
            OpenQA.Selenium.Interactions.Actions actions = new OpenQA.Selenium.Interactions.Actions(driver.GetDriver());

			string[] nascarGroups = {"CUP SERIES", "GANDER RV & OUTDOORS TRUCK SERIES", "XFINITY SERIES"};
			
			if (step.Name.Equals("Verify Golf Groups")) {
				data = "//div[contains(@class,'scores-home-container')]//div[contains(@class,'active')]//ul";
				steps.Add(new TestStep(order, "Open Conference Dropdown", "", "click", "xpath", "//a[@class='dropdown-menu-title']", wait));
				steps.Add(new TestStep(order, "Verify Dropdown is Displayed", "", "verify_displayed", "xpath", data, wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
				
				data = data + "//li";
				steps.Add(new TestStep(order, "Verify Number of Groups", "3", "verify_count", "xpath", data, wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
				
				var groups = driver.FindElements("xpath", data); 
				for (int i = 0; i < groups.Count; i++) {
					if (nascarGroups[i].Equals(groups[i].GetAttribute("innerText"))) {
						log.Info("Success. " + nascarGroups[i] + " matches " + groups[i].GetAttribute("innerText"));
					}
					else {
						log.Error("***Verification FAILED. Expected data [" + nascarGroups[i] + "] does not match actual data [" + groups[i].GetAttribute("innerText") + "] ***");
						err.CreateVerificationError(step, nascarGroups[i], groups[i].GetAttribute("innerText"));
					}
				}
			}
			
			else if (step.Name.Equals("Determine Current Race")) {
				today = DateTime.Today;
				// determine week of season by today's date and time
				if (today >= DateTime.Parse("02/01/2021") && today < DateTime.Parse("02/09/2021 11:00:00")) {
					data = "4469";
					DataManager.CaptureMap["NASCAR_EVENT"] = "BUSCH CLASH AT DAYTONA";
					DataManager.CaptureMap["NASCAR_TRACK"] = "Daytona International Speedway";
					DataManager.CaptureMap["NASCAR_LOC"] = "Daytona Beach, FL";
				}
				else if (today >= DateTime.Parse("02/09/2021 11:00:01") && today < DateTime.Parse("02/15/2021 11:00:00")) {
					data = "4431";
					DataManager.CaptureMap["NASCAR_EVENT"] = "DAYTONA 500";
					DataManager.CaptureMap["NASCAR_TRACK"] = "Daytona International Speedway";
					DataManager.CaptureMap["NASCAR_LOC"] = "Daytona Beach, FL";
				}
				else if (today >= DateTime.Parse("02/15/2021 11:00:01") && today < DateTime.Parse("02/22/2021 11:00:00")) {
					data = "4503";
					DataManager.CaptureMap["NASCAR_EVENT"] = "O'REILLY AUTO PARTS 253 AT DAYTONA";
					DataManager.CaptureMap["NASCAR_TRACK"] = "Daytona International Speedway";
					DataManager.CaptureMap["NASCAR_LOC"] = "Daytona Beach, FL";
				}
				else if (today >= DateTime.Parse("02/22/2021 11:00:01") && today < DateTime.Parse("03/01/2021 11:00:00")) {
					data = "4465";
					DataManager.CaptureMap["NASCAR_EVENT"] = "DIXIE VODKA 400";
					DataManager.CaptureMap["NASCAR_TRACK"] = "Homestead-Miami Speedway";
					DataManager.CaptureMap["NASCAR_LOC"] = "Homestead, FL";
				}
				else if (today >= DateTime.Parse("03/01/2021 11:00:01") && today < DateTime.Parse("03/08/2021 11:00:00")) {
					data = "4437";
					DataManager.CaptureMap["NASCAR_EVENT"] = "PENNZOIL 400 PRESENTED BY JIFFY LUBE";
					DataManager.CaptureMap["NASCAR_TRACK"] = "Las Vegas Motor Speedway";
					DataManager.CaptureMap["NASCAR_LOC"] = "Las Vegas, NV";
				}
				else if (today >= DateTime.Parse("03/08/2021 11:00:01") && today < DateTime.Parse("03/15/2021 11:00:00")) {
					data = "4435";
					DataManager.CaptureMap["NASCAR_EVENT"] = "NASCAR CUP SERIES AT PHOENIX";
					DataManager.CaptureMap["NASCAR_TRACK"] = "Phoenix Raceway";
					DataManager.CaptureMap["NASCAR_LOC"] = "Avondale, AZ";
				}
				else if (today >= DateTime.Parse("03/15/2021 11:00:01") && today < DateTime.Parse("03/22/2021 11:00:00")) {
					data = "4463";
					DataManager.CaptureMap["NASCAR_EVENT"] = "FOLDS OF HONOR QUIKTRIP 500";
					DataManager.CaptureMap["NASCAR_TRACK"] = "Atlanta Motor Speedway";
					DataManager.CaptureMap["NASCAR_LOC"] = "Hampton, GA";
				}
				else {
					data = "4436";
					DataManager.CaptureMap["NASCAR_EVENT"] = "NASCAR CUP SERIES CHAMPIONSHIP";
					DataManager.CaptureMap["NASCAR_TRACK"] = "Phoenix Raceway";
					DataManager.CaptureMap["NASCAR_LOC"] = "Avondale, AZ";
				}		
				
				DataManager.CaptureMap["NASCAR_EVENTID"] = data;
				xpath = "//div[contains(@id,'"+ data +"')]";
				ele = driver.FindElement("xpath", xpath);
                //js.ExecuteScript("arguments[0].scrollIntoView(true);", ele);
                actions.MoveToElement(ele).Perform();
				log.Info("*TEMPORARY FIX* : Scroll to Score Chip " + data);
			}
			
			else if (step.Name.Equals("Verify Event Title")) {
				xpath = "//div[contains(@id,'"+ DataManager.CaptureMap["NASCAR_EVENTID"] +"')]//div[contains(@class,'scorechip-title')]";
				steps.Add(new TestStep(order, "Verify Event Title", step.Data, "verify_value", "xpath", xpath, wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else if (step.Name.Equals("Verify Event Raceway")) {
				xpath = "//div[contains(@id,'"+ DataManager.CaptureMap["NASCAR_EVENTID"] +"')]//div[contains(@class,'scorechip-sub1')]";
				steps.Add(new TestStep(order, "Verify Event Raceway", step.Data, "verify_value", "xpath", xpath, wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else if (step.Name.Equals("Verify Event Location")) {
				xpath = "//div[contains(@id,'"+ DataManager.CaptureMap["NASCAR_EVENTID"] +"')]//div[contains(@class,'scorechip-sub2')]";
				steps.Add(new TestStep(order, "Verify Event Location", step.Data, "verify_value", "xpath", xpath, wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}			
			
			else {
				throw new Exception("Test Step not found in script");
			}

		}
	}
}