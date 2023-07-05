using AltTester.AltTesterUnitySDK.Driver;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace Appium_TicTacToe;

public class Tests
{
    AltDriver altDriver;
    private AndroidDriver<IWebElement> _driver;
    [SetUp]
    public void Setup()
    {
        AppiumSetup();

        Thread.Sleep(5000);
        Console.WriteLine("Appium driver started");

        AltReversePortForwarding.ReversePortForwardingAndroid();
        altDriver = new AltDriver();
        altDriver.AddNotificationListener<bool>(AltTester.AltTesterUnitySDK.Driver.Notifications.NotificationType.APPLICATION_PAUSED, ApplicationPaused, true);
        // altDriver.SetCommandResponseTimeout(300)  // You can increase the duration of the command timeout in case application is paused for more than 1 min, by default the timeout duration is 60 seconds
    }
    [TearDown]
    public void TearDown()
    {
        altDriver.Stop();
        AltReversePortForwarding.RemoveReversePortForwardingAndroid();
        _driver.CloseApp();
    }

    [Test]
    public void Test1()
    {
        altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, "AddButton").Tap();//After this action ApplicationPause should be called
        altDriver.FindObject(AltTester.AltTesterUnitySDK.Driver.By.NAME, "PlayButton").Tap();//This action will not be executed while the application is paused
        Assert.Pass();
    }
    private void ApplicationPaused(bool isPaused)
    {
        if (!isPaused)//This method is also called when the application is back in focus but we don't want to do anything with appium in this instance
            return;
        Thread.Sleep(20000);//Wait for close button to appear
        _driver.FindElementByXPath("/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.RelativeLayout/android.webkit.WebView/android.webkit.WebView/android.view.View/android.view.View/android.view.View[1]/android.widget.Button").Click();//Find with appium and click to close the add
    }
    private void AppiumSetup()
    {
        var driverOptions = new AppiumOptions();
        driverOptions.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
        driverOptions.AddAdditionalCapability(MobileCapabilityType.DeviceName, "device");
        driverOptions.AddAdditionalCapability("appium:app", @"D:\github\Appium-TicTacToe\AltTicTacToe.apk");//Put your path
        driverOptions.AddAdditionalCapability("appium:automationName", "UiAutomator2");

        driverOptions.AddAdditionalCapability("autoGrantPermissions", true);
        driverOptions.AddAdditionalCapability("allowSessionOverride", true);

        _driver = new AndroidDriver<IWebElement>(new Uri("http://localhost:4723"), driverOptions);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
    }

}