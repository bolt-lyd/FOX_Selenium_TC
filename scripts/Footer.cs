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
			ReadOnlyCollection<IWebElement> elements = new ReadOnlyCollection<IWebElement>();
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver.GetDriver();
			VerifyError err = new VerifyError();
			
			if (step.Name.Equals("Verify Footer Links") || step.Name.Equals("Verify Footer Links 2")) {
				if (step.Name.Equals("Verify Footer Links")) {
					string[] dataSet = {"Help", "Press", "Advertise With Us", "Jobs", "FOX Cincy", "RSS", "Sitemap"};
					elements = driver.FindElements("xpath", "//div[@class='footer-links-1']//a");					
				}
				else if (step.Name.Equals("Verify Footer Links 2")) {
					string[] dataSet = {"FS1", "Fox", "Fox News", "Fox Corporation", "Fox Supports", "Fox Deportes"};
					elements = driver.FindElements("xpath", "//div[@class='footer-links-2']//a");					
				}

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