using boxs.Services;

namespace TestSolution
{
    public class TestValidateSpinResult
    {
        [Fact]
        public void TestboxWins_PDFExample_27Wins()
        {
            // arrange
            ValidateSpinResult validator = new ValidateSpinResult();

            int[,] myArray = new int[,]
            {
                { 3, 3, 3, 4, 5 },
                { 2, 3, 2, 3, 3 },
                { 1, 2, 3, 3, 3 }
            };

            // act
            var totalWins = validator.GetTotalWins(myArray);

            // assert
            Assert.Equal(27, totalWins);
        }

        [Fact]
        public void TestboxWins_LineOnly_12Wins()
        {
            // arrange
            ValidateSpinResult validator = new ValidateSpinResult();

            int[,] myArray = new int[,]
            {
                { 2, 2, 2 },
                { 3, 3, 4 },
                { 5, 1, 1 }
            };

            // act
            var totalWins = validator.GetTotalWins(myArray);

            // assert
            Assert.Equal(12, totalWins);
        }

        [Fact]
        public void TestboxWins_WaveOnly_39Wins()
        {
            // arrange
            ValidateSpinResult validator = new ValidateSpinResult();

            int[,] myArray = new int[,]
            {
                { 5, 1, 5 },
                { 3, 5, 3 },
                { 5, 3, 5 }
            };

            // act
            var totalWins = validator.GetTotalWins(myArray);

            // assert
            Assert.Equal(39, totalWins);
        }

        [Fact]
        public void TestboxWins_RandomResults_54Wins()
        {
            // arrange
            ValidateSpinResult validator = new ValidateSpinResult();

            int[,] myArray = new int[,]
            {
                { 5, 1, 5, 1, 5 },
                { 3, 5, 3, 5, 3 },
                { 5, 3, 5, 5, 3 }
            };

            // act
            var totalWins = validator.GetTotalWins(myArray);

            // assert
            Assert.Equal(54, totalWins);
        }
    }
}