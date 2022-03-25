using Xunit;

namespace simple_audio_editor.Tests
{
    public class FFmpegOptions_Tests
    {
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
    }
}