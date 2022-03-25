using Xunit;

namespace simple_audio_editor.Tests
{
    public class FFmpegOptions_Tests
    {
        #region Adding Trim Tests

        [Fact]
        public void AddTrimSection_AddingTrimAddsToStartAndEndTrimList()
        {
            //Arrange
            var options = new FFmpegOptions("input", "output");
            var expectedCount = 1;
            var expectedStartTime = 0;
            var expectedEndTime = 1;

            //Act
            options.AddTrimSection(expectedStartTime, expectedEndTime);
            /*  var actualStartCount = options.TrimStart.Count;
              var actuaEndCount = options.TrimEnd.Count;*/

            var actualTrimTimeCount = options.TrimTimes.Count;

            var actualStart = options.TrimTimes[0].Start;
            var actualEnd = options.TrimTimes[0].End;

            //Assert
            //test list count
            /* Assert.Equal(expectedCount, actualStartCount);
             Assert.Equal(expectedCount, actuaEndCount);*/
            Assert.Equal(expectedCount, actualTrimTimeCount);

            //test list value
            Assert.Equal(expectedStartTime, actualStart);
            Assert.Equal(expectedEndTime, actualEnd);
        }

        [Fact]
        public void AddTrimSection_AddingTrimWithoutEndTimeAddsToStartAndEndTrimListWithEndAsZero()
        {
            //Arrange
            var options = new FFmpegOptions("input", "output");
            var expectedCount = 1;
            var expectedStartTime = 50;
            var expectedEndTime = 0;

            //Act
            options.AddTrimSection(expectedStartTime);
            var actualStartCount = options.TrimTimes.Count;
            var actuaEndCount = options.TrimTimes.Count;

            var actualStart = options.TrimTimes[0].Start;
            var actualEnd = options.TrimTimes[0].End;

            //Assert
            //test list count
            Assert.Equal(expectedCount, actualStartCount);
            Assert.Equal(expectedCount, actuaEndCount);

            //actual end should be default value of 0
            Assert.Equal(expectedStartTime, actualStart);
            Assert.Equal(expectedEndTime, actualEnd);
        }

        [Fact]
        public void AddTrimSection_AddingTrimSetsTrimFlagToTrue()
        {
            //Arrange
            var options = new FFmpegOptions("input", "output");
            var expected = true;

            //Act
            options.AddTrimSection(99);
            var actual = options.TrimFlag;

            //Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        #region Removing Trim Tests

        [Theory]
        [InlineData(5, 10)]
        [InlineData(60)]
        [InlineData(560, 420)]
        [InlineData(25, 333)]
        public void RemoveTrimSection_RemovingExistingTrimAsStartEndInts_ReturnsTrue(int start, int end = 0)
        {
            //Arrange
            var expected = 0;
            var opt = new FFmpegOptions("", "");

            if (end == 0) opt.AddTrimSection(start);
            else opt.AddTrimSection(start, end);

            //Act
            var boolResult = opt.RemoveTrimSection(start, end);
            var actual = opt.TrimTimes.Count;

            //Assert
            Assert.Equal(expected, actual);
            Assert.True(boolResult);

        }

        [Theory]
        [InlineData(5, 10)]
        [InlineData(60)]
        [InlineData(560, 420)]
        [InlineData(25, 333)]
        public void RemoveTrimSection_RemovingExistingTrimAsTrimTime_ReturnsTrue(int start, int end = 0)
        {
            //Arrange
            var expected = 0;
            var opt = new FFmpegOptions("", "");

            if (end == 0) opt.AddTrimSection(start);
            else opt.AddTrimSection(start, end);

            //Act
            var toRemove = end == 0 ? new TrimTime(start) : new TrimTime(start, end);
            var boolResult = opt.RemoveTrimSection(toRemove);
            var actual = opt.TrimTimes.Count;

            //Assert
            Assert.Equal(expected, actual);
            Assert.True(boolResult);
        }

        [Fact]
        public void RemoveTrimSection_RemovingNonExistingTrimAsStartEndInts_ReturnsFalse()
        {
            //Arrange
            var opt = new FFmpegOptions("", "");
            var expected = 1;
            opt.AddTrimSection(12, 88);

            //Act
            var boolResult = opt.RemoveTrimSection(5, 50);
            var actual = opt.TrimTimes.Count;

            //Assert
            Assert.Equal(expected, actual);
            Assert.False(boolResult);
        }

        [Fact]
        public void RemoveTrimSection_RemovingNonExistingTrimAsTrimTime_ReturnsFalse()
        {

            //Arrange
            var opt = new FFmpegOptions("", "");
            var expected = 1;
            opt.AddTrimSection(12, 88);

            //Act
            var boolResult = opt.RemoveTrimSection(new TrimTime(5, 50));
            var actual = opt.TrimTimes.Count;

            //Assert
            Assert.Equal(expected, actual);
            Assert.False(boolResult);


            #endregion
        }
    }
}