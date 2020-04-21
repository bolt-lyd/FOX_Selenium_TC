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
			string data;
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver.GetDriver();
			VerifyError err = new VerifyError();
			
			if (step.Name.Equals("Verify Main Nav Link Values")) {
				string[] dataSet = {"HOME", "SCORES", "LIVE TV", "STORIES", "EXPLORE", "MORE", "SIGN IN", "Account"};
				elements = driver.FindElements("xpath", "//ul[@class='nav']//li[contains(@class,'desktop-show')]//span[contains(@class,'nav-item-text')]");
				
				if(dataSet.Length != elements.Count) {
					err.CreateVerificationError(step, dataSet.Length.ToString(), elements.Count.ToString());
				}
				else {
					for (int i=0; i < elements.Count; i++) {
						if(dataSet[i].Equals(elements[i].GetAttribute("innerText").Trim())) {
							log.Info("Verification Passed. Expected [" + dataSet[i] + "] matches Actual [" + elements[i].GetAttribute("innerText").Trim() +"]");
						}
						else {
							err.CreateVerificationError(step, dataSet[i], elements[i].GetAttribute("innerText").Trim());
						}
					}
				}
			}
			
			else {
				throw new Exception("Test Step not found in script");
			}

		}
	}
}