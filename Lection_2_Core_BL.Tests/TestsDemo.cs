namespace Lection2_Core_BL.Tests
{
    public class TestsDemo
    {
        public int Add(int a, int b)
        {
            try
            {
                checked
                {
                    return a + b;
                }
            }
            catch (OverflowException)
            {
                throw new ArgumentException();
            }
        }

        [SetUp]
        public void Setup()
        {
        }

        [TestCase(-5, -10, -15)]
        [TestCase(-5, 10, 5)]
        [TestCase(5, -10, -5)]
        [TestCase(5, 10, 15)]
        public void Add_WhenAAndBNotTooBig_ShouldReturnSummTwoNumbers(
            int a,
            int b,
            int expected)
        {
            //Act
            int actual = Add(a, b);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Add_WhenATooBig_ShouldThrowArgumentException()
        {
            //Arrange
            int a = int.MaxValue;
            int b = 1;
            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => Add(a, b));
        }
    }
}