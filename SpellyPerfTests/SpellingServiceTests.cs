using FluentAssertions;
using SpellyPerfApp;
using Xunit;

namespace SpellyPerfTests
{
    public class SpellingServiceTests
    {
        [Theory]
        [InlineData("protien analysis can take a long time", new[] { "protien" })]
        [InlineData("autobots roll out", new[] { "autobots" })]
        [InlineData("You're gonna need a bigger boat", new[] { "gonna" })]
        public void GivenInputTextWithSpellingMistakes_GetMisspelledWords_ReturnsMisspelledWords(string inputText, string[] expectedSpellingMistakes)
        {
            // Arrange
            var sut = new SpellingService();

            // Act
            var misspelledWords =  sut.GetMisspelledWords(inputText);

            // Assert
            misspelledWords.Should().BeEquivalentTo(expectedSpellingMistakes);
        }
        
        [Theory]
        [InlineData("cheese on toast")]
        [InlineData("swimming in the river")]
        [InlineData("Now this is a story all about how my life got flipped turned upside down")]
        public void GivenInputTextWithNoSpellingMistakes_GetMisspelledWords_ReturnsNothing(string inputText)
        {
            // Arrange
            var sut = new SpellingService();

            // Act
            var misspelledWords =  sut.GetMisspelledWords(inputText);

            // Assert
            misspelledWords.Should().BeEmpty();
        }
        
        [Fact]
        public void GivenEmptyInputText_GetMisspelledWords_ReturnsNothing()
        {
            // Arrange
            var sut = new SpellingService();

            // Act
            var misspelledWords =  sut.GetMisspelledWords(string.Empty);

            // Assert
            misspelledWords.Should().BeEmpty();
        }
    }
}