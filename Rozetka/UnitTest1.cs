using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Rozetka
{
    public class Tests
    {
        private readonly By _form = By.XPath("//single-modal-window/div[3]/div/h3");
        
        private const string Email = "somemail@gmail.com";
        private const string Password = "Password_1";
        private const string SearchText = "Кросівки Ni";

        private IWebDriver _driver;
        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl("https://rozetka.com.ua/ua/");
        }

        [Test]
        public void AddProductToBasketTest()
        {
            _driver.Navigate().GoToUrl("https://rozetka.com.ua/ua/spirit-52027097245/p317135137/");
            var buyButton = _driver.FindElement(By.CssSelector(".buy-button__label"));
            buyButton.Click();
            Thread.Sleep(2000);
            
            IWebElement form;
            while (true)
            {
                try
                {
                    form = _driver.FindElement(By.ClassName("cart-page__heading"));
                    break;
                }
                catch (Exception)
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
            }

            ICollection<IWebElement> cartProduct = null;
            if (!form.Text.Equals("Кошик")) Assert.Fail("Basket page wasn't opened.");
            try
            {
                cartProduct = _driver.FindElements(
                    By.CssSelector(".cart-product__main"));
            }
            catch (Exception)
            {
                Assert.Fail("Product wasn't been added to a basket.");
            }
            
            if (cartProduct.Any(product => product.Text.Contains("Велосипед Spirit Echo 7.2 27.5")))
                Assert.Pass();
            Assert.Fail("Product wasn't been added to a basket.");
        }

        [Test]
        public void DeleteProductFromBasket()
        {
            _driver.Navigate().GoToUrl("https://rozetka.com.ua/ua/spirit-52027097245/p317135137/");
            var buyButton = _driver.FindElement(By.CssSelector(".buy-button__label"));
            buyButton.Click();
            
            Thread.Sleep(2000);
            _driver.Navigate().GoToUrl("https://rozetka.com.ua/ua/spirit-52027097245/p317135137/");
            
            Thread.Sleep(3000);
            var basket = _driver.FindElement(By.CssSelector(".header-actions__item--cart svg"));
            basket.Click();
            
            Thread.Sleep(1000);
            var options = _driver.FindElement(By.XPath("//rz-popup-menu/button"));
            options.Click();
            
            Thread.Sleep(500);
            IWebElement deleteButton = null;
            try
            {
                deleteButton = _driver.FindElement(By.XPath("//rz-trash-icon/button"));

            }
            catch (Exception)
            {
                Assert.Fail("Something went wrong.");
            }
            deleteButton.Click();
            Thread.Sleep(2000);

            try
            {
                _driver.FindElement(By.XPath("//h4[contains(.,'Кошик порожній')]"));
            }
            catch (Exception)
            {
                Assert.Fail("Basket isn't empty.");
            }
            // var emptyBasketLabel = _driver.FindElement(By.XPath("//h4[contains(.,'Кошик порожній')]"));
            Assert.Pass();
        }

        [Test]
        public void SearchTest()
        {
            var search = _driver.FindElement(By.XPath("//input[@name='search']"));
            search.SendKeys(SearchText);
            
            Thread.Sleep(1000);
            var searchButton = _driver.FindElement(By.XPath("//button[contains(.,'Знайти')]"));
            searchButton.Click();
            
            Thread.Sleep(4000);
            var productCards = _driver.FindElements(By.CssSelector(".goods-tile.ng-star-inserted"));
            if (productCards.Count==0)
            {
                IWebElement message = null;
                try
                {
                    message = _driver
                        .FindElement(
                            By.ClassName("catalog-empty"));
                }
                catch (Exception)
                {
                    Assert.Fail("Products weren't found and message about it wasn't appeared.");
                }
                Assert.Pass($"Products by search '{SearchText}' wasn't found.\n" +
                            $"[{message.Text}]");
            }
            Assert.Pass("Products were found.");
        }

        [Test]
        public void LoginTest()
        {
            var signIn = _driver.FindElement(By.CssSelector(".header-actions__item--user svg"));
            signIn.Click();
            
            Thread.Sleep(1000);
            var emailInput = _driver.FindElement(By.XPath("//input[@id='auth_email']"));
            emailInput.SendKeys(Email);
            
            Thread.Sleep(1000);
            var passwordInput = _driver.FindElement(By.XPath("//input[@id='auth_pass']"));
            passwordInput.SendKeys(Password);
            
            Thread.Sleep(1000);
            var submitButton = _driver.FindElement(By.XPath("//div[5]/button"));
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

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}