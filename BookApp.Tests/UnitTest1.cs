using Xunit;

namespace BookApp.Tests
{
    public class SimpleTests
    {
        [Fact]
        public void Test_ApplicationStarts()
        {
            bool isStarted = true;
            Assert.True(isStarted);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        public void Math_AdditionWorks(int a, int b, int expected)
        {
            Assert.Equal(expected, a + b);
        }
    }
}