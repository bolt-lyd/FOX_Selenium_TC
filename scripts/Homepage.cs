using System;
using System.Collections.Generic;
using SeleniumProject.Utilities;
using OpenQA.Selenium;
using log4net;
using System.Threading;
using System.Collections.ObjectModel;

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
			ReadOnlyCollection<IWebElement> elements;
			string data = "";
			string xpath = "";
			string url = "";
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver.GetDriver();
			VerifyError err = new VerifyError();
			
			if (step.Name.Equals("Verify Main Nav Link Values")) {
				string[] dataSet = {"HOME", "SCORES", "LIVE TV", "STORIES", "SEARCH", "SIGN IN", "Account"};
				elements = driver.FindElements("xpath", "//ul[@class='nav']//li[contains(@class,'desktop-show')]//span[contains(@class,'nav-item-text')]");
				
				if(dataSet.Length != elements.Count) {
					log.Error("Unexpected element count. Expected: [" + dataSet.Length + "] does not match Actual: [" + elements.Count + "]");
					err.CreateVerificationError(step, dataSet.Length.ToString(), elements.Count.ToString());
				}
				else {
					for (int i=0; i < elements.Count; i++) {
						if(dataSet[i].Equals(elements[i].GetAttribute("innerText").Trim())) {
							log.Info("Verification Passed. Expected [" + dataSet[i] + "] matches Actual [" + elements[i].GetAttribute("innerText").Trim() +"]");
						}
						else {
							log.Error("Verification FAILED. Expected: [" + dataSet[i] + "] does not match Actual: [" + elements[i].GetAttribute("innerText").Trim() + "]");
							err.CreateVerificationError(step, dataSet[i], elements[i].GetAttribute("innerText").Trim());
						}
					}
				}
			}
			
			else if (step.Name.Equals("Verify URL Contains String")) {
				url = driver.GetDriver().Url.ToString();
				if (url.Contains(step.Data)) {
					log.Info("Verification Passed. Expected [" + step.Data + "]" + " can be found in Actual URL [" + url + "]");
				}
				else {
					log.Error("Verification FAILED.*** Expected: [" + step.Data + "] is not within Actual URL [" + url + "]");
					err.CreateVerificationError(step, step.Data, url);
					driver.TakeScreenshot(DataManager.CaptureMap["TEST_ID"] + "_verification_failure_" + DataManager.VerifyErrors.Count);
				}
			}
			
			else if (step.Name.Equals("Store Sport by Data")) {
				DataManager.CaptureMap["SPORT"] = step.Data;
				log.Info("Storing " + step.Data + "to capture map as SPORT...");
			}
			
			else if (step.Name.Equals("Store Conference by Data")) {
				DataManager.CaptureMap["CONF"] = step.Data;
				log.Info("Storing " + step.Data + "to capture map as CONF...");
			}
			
			else if (step.Name.Equals("Navigate to URL by ENV")) {
				log.Info("Appending " + step.Data + " to ENV URL: " + TestParameters.GLOBAL_APP_URL);
				url = TestParameters.GLOBAL_APP_URL + step.Data;
				steps.Add(new TestStep(order, "Navigate to " + url, url, "navigate_to", "", "", wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else if (step.Name.Equals("Navigate to External Scorestrip by ENV")) {
				if(TestParameters.GLOBAL_ENV.Equals("dev")) {
					url = "dev-";
				} 
				else if (TestParameters.GLOBAL_ENV.Equals("stg")) {
					url = "stage-";
				}
					
				url = "https://" + url +"statics.foxsports.com/static/orion/scorestrip/index.html";			
				
				steps.Add(new TestStep(order, "Navigate to " + url, url, "navigate_to", "", "", wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else if (step.Name.Equals("Capture Window Handle")) {
				DataManager.CaptureMap["WINDOW_HANDLE"] = driver.GetDriver().CurrentWindowHandle;
				log.Info("Storing window handle as " + DataManager.CaptureMap["WINDOW_HANDLE"]);
			} 
			
			else if (step.Name.Equals("Switch to New Tab")) {
				ReadOnlyCollection<string> windowHandles = driver.GetDriver().WindowHandles;  
				
				log.Info("Total Count of Handles: " + windowHandles.Count);
				foreach(string handle in windowHandles) {
					log.Info("Current Handle : " + handle );
					if (!handle.Equals(DataManager.CaptureMap["WINDOW_HANDLE"])) {
						DataManager.CaptureMap["NEW_WINDOW_HANDLE"] = handle;
					}
				}
				driver.GetDriver().SwitchTo().Window(DataManager.CaptureMap["NEW_WINDOW_HANDLE"]);
				log.Info("Storing new window handle as " + DataManager.CaptureMap["NEW_WINDOW_HANDLE"] + " and switching to new window."); 
			} 
			
			else {
				throw new Exception("Test Step not found in script");
			}

		}
	}
}