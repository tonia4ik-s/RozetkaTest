using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Rozetka
{
    public class Tests
    {
        private readonly By _signInButton = By.CssSelector(".header-actions__item--user svg");
        private readonly By _emailInput = By.XPath("//input[@id='auth_email']");
        private readonly By _passwordInput = By.XPath("//input[@id='auth_pass']");
        private readonly By _submitButton = By.XPath("//div[5]/button");
        private readonly By _form = By.XPath("//single-modal-window/div[3]/div/h3");
        
        private const string Email = "somemail@gmail.com";
        private const string Password = "Password_1";
        
        private IWebDriver _driver;
        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl("https://rozetka.com.ua/ua/");
        }

        [Test]
        public void LoginTest()
        {
            var signIn = _driver.FindElement(_signInButton);
            signIn.Click();
            
            Thread.Sleep(1000);
            var emailInput = _driver.FindElement(_emailInput);
            emailInput.SendKeys(Email);
            
            Thread.Sleep(1000);
            var passwordInput = _driver.FindElement(_passwordInput);
            passwordInput.SendKeys(Password);
            
            Thread.Sleep(1000);
            var submitButton = _driver.FindElement(_submitButton);
            submitButton.Click();

            Thread.Sleep(5000);
            while (true)
            {
                IWebElement form = null;
                try
                {
                    form = _driver.FindElement(_form);
                }
                catch (Exception)
                {
                    Assert.Positive(1);
                }
                switch (form.Text)
                {
                    case "Підтвердження номера телефону":
                        Assert.Positive(1);
                        break;
                    case "Вхід":
                        submitButton.Click();
                        Thread.Sleep(2000);
                        IWebElement errorMessage = null;
                        try
                        {
                            errorMessage = _driver.FindElement(By.CssSelector(".error-message"));
                        } 
                        catch (Exception) { /* ignored */ }

                        if (errorMessage != null)
                        {
                            Assert.Fail(errorMessage.Text);
                        }
                        Assert.Pass();
                        break;
                }
            }
        }

        [Test]
        public void AddProductToBasketTest()
        {
            _driver.Navigate().GoToUrl("https://rozetka.com.ua/ua/spirit-52027097245/p317135137/");
            var buyButton = _driver.FindElement(By.CssSelector(".button--medium > .buy-button__label"));
            buyButton.Click();
            Thread.Sleep(2000);
            
            IWebElement form;
            while (true)
            {
                try
                {
                    form = _driver.FindElement(_form);
                    break;
                }
                catch (Exception)
                {
                    var basket = _driver.FindElement(By.CssSelector(".header-actions__item--cart svg"));
                    basket.Click();
                }
            }

            if (!form.Text.Equals("Кошик")) Assert.Fail("Basket page wasn't opened.");
            try
            {
                _driver.FindElement(
                    By.XPath("//a[contains(text(),'Велосипед Spirit Echo 7.2 27.5\" рама M 2021 Лате (52027097245)')]"));
                Assert.Pass();
            }
            catch (Exception)
            {
                Assert.Fail("Product wasn't been added to a basket.");
            }
        }
        
        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}