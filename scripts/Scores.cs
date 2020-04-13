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
			string title;
			IWebElement ele;
			string date;
			IJavaScriptExecutor js = (IJavaScriptExecutor)driver.GetDriver();
			
			if (step.Name.Equals("Verify Displayed Day on Top Scores")) {
				TimeSpan time = DateTime.Now.TimeOfDay;
				var now = time.Hours;
				if (now < 11)
					step.Data = "YESTERDAY";
				else 
					step.Data = "TODAY";
				
				steps.Add(new TestStep(order, "Verify Displayed Day on Top Scores", step.Data, "verify_value", "xpath", "//div[contains(@class,'scores-date')]//div[contains(@class,'sm')]", wait));
				TestRunner.RunTestSteps(driver, null, steps);
				steps.Clear();
			}
			
			else if (step.Name.Equals("Scroll Top Scores Page to Yesterday")) {
				title = "//div[contains(@class,'scores-date')]//div[contains(@class,'sm')]";
				ele = driver.FindElement("xpath", title);
				date = ele.GetAttribute("innerText");
				
				if (date.Equals("TODAY")) {
					js.ExecuteScript("window.scrollBy(0,-250)");
					log.Info("Scrolling up on YESTERDAY...");
					steps.Add(new TestStep(order, "Verify Displayed Day on Top Scores", "YESTERDAY", "verify_value", "xpath", title, wait));
					TestRunner.RunTestSteps(driver, null, steps);
					steps.Clear();
				}
				else {
					log.Info("Page defaulted to YESTERDAY");
				}
			}
			
			else if (step.Name.Equals("Scroll Top Scores Page to Today")) {
				title = "//div[contains(@class,'scores-date')]//div[contains(@class,'sm')]";
				ele = driver.FindElement("xpath", title);
				date = ele.GetAttribute("innerText");
				
				if (!date.Equals("TODAY")) {
					do {
						js.ExecuteScript("window.scrollBy(0,250)");
						log.Info("Scrolling down on page...");
						Thread.Sleep(1000);
						ele = driver.FindElement("xpath", title);
						date = ele.GetAttribute("innerText");
					}
					while (date.Equals("YESTERDAY"));
				}
			}
			
			else if (step.Name.Equals("Scroll Top Scores Page to Tomorrow")) {
				title = "//div[contains(@class,'scores-date')]//div[contains(@class,'sm')]";
				ele = driver.FindElement("xpath", title);
				date = ele.GetAttribute("innerText");
				
				if (date.Equals("TODAY")) {
					js.ExecuteScript("window.scrollBy(0,-250)");
					log.Info("Scrolling up on page...");
					steps.Add(new TestStep(order, "Verify Displayed Day on Top Scores", "YESTERDAY", "verify_value", "xpath", title, wait));
					TestRunner.RunTestSteps(driver, null, steps);
					steps.Clear();
				}
				else {
					do {
						js.ExecuteScript("window.scrollBy(0,250)");
						log.Info("Scrolling down on page...");
						Thread.Sleep(1000);
						ele = driver.FindElement("xpath", title);
						date = ele.GetAttribute("innerText");
					}
					while (date.Equals("YESTERDAY"));

				}
			}
			
			else {
				throw new Exception("Test Step not found in script");
			}

		}
	}
}