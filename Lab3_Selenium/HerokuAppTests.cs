using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Xunit;
 
namespace Lab3_Selenium
{
    public class HerokuAppTests : BaseTest
    {
        /// <summary>перевірка початкового стану чекбоксів і зміни стану після кліку</summary>
        [Fact]
        public void Checkboxes_ClickChangesState()
        {
            // Arrange
            Open("/checkboxes");
            var boxes = Driver.FindElements(By.CssSelector("#checkboxes input[type='checkbox']"));
 
            // Assert 
            Assert.Equal(2, boxes.Count);
            Assert.False(boxes[0].Selected);
            Assert.True(boxes[1].Selected);
 
            // Act
            boxes[0].Click();
            boxes[1].Click();
 
            // Assert
            Assert.True(boxes[0].Selected);
            Assert.False(boxes[1].Selected);
        }
 
        /// <summary>додавання 3 кнопок Delete і видалення однієї з них</summary>
        [Fact]
        public void AddRemoveElements_AddThreeThenRemoveOne()
        {
            // Arrange
            Open("/add_remove_elements/");
            var addButton = Driver.FindElement(By.XPath("//button[text()='Add Element']"));
 
            // Act
            for (int i = 0; i < 3; i++)
                addButton.Click();
            var deleteButtons = Driver.FindElements(By.CssSelector("#elements .added-manually"));
 
            // Assert
            Assert.Equal(3, deleteButtons.Count);
            Assert.All(deleteButtons, b => Assert.Equal("Delete", b.Text));
 
            // Act
            deleteButtons[0].Click();
 
            // Assert
            Assert.Equal(2, Driver.FindElements(By.CssSelector("#elements .added-manually")).Count);
        }
 
        /// <summary>перевірка значення за замовчуванням і вибору опції</summary>
        [Fact]
        public void Dropdown_SelectOption2()
        {
            // Arrange
            Open("/dropdown");
            var select = new SelectElement(Driver.FindElement(By.Id("dropdown")));
            Assert.Equal("Please select an option", select.SelectedOption.Text);
 
            // Act
            select.SelectByText("Option 2");
 
            // Assert
            Assert.Equal("Option 2", select.SelectedOption.Text);
            Assert.Equal("2", select.SelectedOption.GetDomProperty("value"));
        }
 
        /// <summary>введення числа і збільшення його клавішею стрілка вгору</summary>
        [Fact]
        public void Inputs_EnterNumberAndIncrement()
        {
            // Arrange
            Open("/inputs");
            var input = Driver.FindElement(By.CssSelector("input[type='number']"));
 
            // Act
            input.SendKeys("42");
            var afterTyping = input.GetDomProperty("value");
            input.SendKeys(Keys.ArrowUp);
 
            // Assert
            Assert.Equal("42", afterTyping);
            Assert.Equal("43", input.GetDomProperty("value"));
        }
 
        /// <summary>перехід за кожним посиланням відкриває сторінку з відповідним кодом</summary>
        [Theory]
        [InlineData("200")]
        [InlineData("301")]
        [InlineData("404")]
        [InlineData("500")]
        public void StatusCodes_LinkOpensCorrectPage(string code)
        {
            // Arrange
            Open("/status_codes");
 
            // Act
            Driver.FindElement(By.LinkText(code)).Click();
            Wait.Until(d => d.Url.EndsWith($"/status_codes/{code}"));
 
            // Assert
            var text = Driver.FindElement(By.CssSelector("#content p")).Text;
            Assert.Contains($"This page returned a {code} status code", text);
        }
 
        /// <summary>перемикання між вкладеними фреймами і перевірка їх тексту</summary>
        [Fact]
        public void NestedFrames_ReadTextFromMiddleAndBottom()
        {
            // Arrange
            Open("/nested_frames");
 
            // Act
            Driver.SwitchTo().Frame("frame-top").SwitchTo().Frame("frame-middle");
            var middleText = Driver.FindElement(By.Id("content")).Text;
 
            // Act
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("frame-bottom");
            var bottomText = Driver.FindElement(By.TagName("body")).Text;
 
            // Assert
            Assert.Equal("MIDDLE", middleText);
            Assert.Equal("BOTTOM", bottomText);
            Driver.SwitchTo().DefaultContent();
        }
 
        /// <summary>успішний вхід із правильними даними і вихід із системи</summary>
        [Fact]
        public void FormAuthentication_ValidLogin_ShowsSecureArea()
        {
            // Arrange
            Open("/login");
 
            // Act
            Driver.FindElement(By.Id("username")).SendKeys("tomsmith");
            Driver.FindElement(By.Id("password")).SendKeys("SuperSecretPassword!");
            Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Wait.Until(d => d.Url.Contains("/secure"));
 
            // Assert
            var flash = Driver.FindElement(By.Id("flash")).Text;
            Assert.Contains("You logged into a secure area!", flash);
            Assert.True(Driver.FindElement(By.LinkText("Logout")).Displayed);
 
            // Act
            Driver.FindElement(By.LinkText("Logout")).Click();
            Wait.Until(d => d.Url.EndsWith("/login"));
 
            // Assert
            Assert.Contains("You logged out of the secure area!", Driver.FindElement(By.Id("flash")).Text);
        }
 
        /// <summary>на сторінці 3 блоки, і після оновлення їх текст змінюється</summary>
        [Fact]
        public void DynamicContent_TextChangesAfterRefresh()
        {
            // Arrange
            Open("/dynamic_content");
            var textBlocks = By.XPath("//div[@id='content']//img/../following-sibling::div");
            var before = Driver.FindElements(textBlocks).Select(e => e.Text).ToList();
 
            // Act
            Driver.Navigate().Refresh();
            var after = Driver.FindElements(textBlocks).Select(e => e.Text).ToList();
 
            // Assert
            Assert.Equal(3, before.Count);
            Assert.Equal(3, after.Count);
            Assert.All(after, t => Assert.False(string.IsNullOrWhiteSpace(t)));
            Assert.NotEqual(string.Join("|", before), string.Join("|", after));
        }
 
        /// <summary>основні пункти меню завжди є, а Gallery може з'являтися і зникати</summary>
        [Fact]
        public void DisappearingElements_MainMenuItemsAlwaysPresent()
        {
            // Arrange
            Open("/disappearing_elements");
 
            // Act
            var items = Driver.FindElements(By.CssSelector("ul li a")).Select(a => a.Text).ToList();
 
            // Assert
            Assert.Contains("Home", items);
            Assert.Contains("About", items);
            Assert.Contains("Contact Us", items);
            Assert.Contains("Portfolio", items);
            Assert.True(items.Count == 4 || items.Count == 5,$"Очікувалось 4 або 5 пунктів меню, а знайдено {items.Count}");
        }
 
        /// <summary>підпис користувача прихований і з'являється після наведення курсора</summary>
        [Fact]
        public void Hover_ShowsUserCaption()
        {
            // Arrange
            Open("/hovers");
            var figure = Driver.FindElements(By.CssSelector(".figure"))[0];
            var caption = figure.FindElement(By.CssSelector(".figcaption"));
            Assert.False(caption.Displayed);
 
            // Act
            new Actions(Driver).MoveToElement(figure).Perform();
            Wait.Until(_ => caption.Displayed);
 
            // Assert
            Assert.True(caption.Displayed);
            Assert.Equal("name: user1", caption.FindElement(By.TagName("h5")).Text);
            Assert.NotNull(caption.FindElement(By.LinkText("View profile")));
        }
    }
}
 