using System;
using System.Collections.Generic;
using Xunit;

namespace simple_audio_editor.Tests
{
    public class FFmpegProcess_Tests
    {
        [Fact]
        public void AddToQueue_AddsFFmpegOptionToOptionsQueueList()
        {
            //Arrange
            var opt = new FFmpegOptions("", "");
            var ffmpeg = new FFmpegProcess(new List<FFmpegOptions>());
            var expected = 1;

            //Act
            ffmpeg.AddToQueue(opt);
            var actual = ffmpeg.OptionsQueue.Count;

            //Assert
            Assert.Equal(expected, actual);
        }

        #region RemoveFromQueue Tests

        [Fact]
        public void RemoveFromQueue_Removes_FFmpegOption_FromOptionsQueueList()
        {
            //Arrange
            var opt = new FFmpegOptions("", "");
            var opt2 = new FFmpegOptions("", "");
            var optList = new List<FFmpegOptions> {opt, opt2};
            var ffmpeg = new FFmpegProcess(optList);
            var expected = 1;

            //Act
            ffmpeg.RemoveFromQueue(opt);
            var actual = ffmpeg.OptionsQueue.Count;

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveFromQueue_Removes_ListOfFFmpegOptions_FromOptionsQueueList()
        {
            //Arrange
            var opt = new FFmpegOptions("", "");
            var opt2 = new FFmpegOptions("", "");
            var optList = new List<FFmpegOptions> {opt, opt2};
            var ffmpeg = new FFmpegProcess(optList);
            var expected = 0;

            //Act
            ffmpeg.RemoveFromQueue(optList);
            var actual = ffmpeg.OptionsQueue.Count;

            //Assert
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void RemoveFromQueue_Removes_ListOfConversionResults_FromOptionsQueueList()
        {
            //Arrange
            var opt = new FFmpegOptions("input1", "output1");
            var opt2 = new FFmpegOptions("input2", "output2");
            var opt3 = new FFmpegOptions("input3", "output3");
            var optList = new List<FFmpegOptions> {opt, opt2, opt3};
            var ffmpeg = new FFmpegProcess(optList);
            var time = DateTime.UtcNow;

            var conversionResults = new List<ConversionResult>()
            {
                new ConversionResult(){Input = "input1", Output = "output1",Succeeded = true, StartTime = time, EndTime = time},
                new ConversionResult(){Input = "input2", Output = "output2",Succeeded = true, StartTime = time, EndTime = time},
                new ConversionResult(){Input = "input3", Output = "output3",Succeeded = true, StartTime = time, EndTime = time}
            };

            var expected = 0;

            //Act
            ffmpeg.RemoveFromQueue(conversionResults);
            var actual = ffmpeg.OptionsQueue.Count;

            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void RemoveFromQueue_Removes_ListOfConversionResults_ExcludesFailedConversionFromOptionsQueueList()
        {
            //Arrange
            var opt = new FFmpegOptions("input1", "output1");
            var opt2 = new FFmpegOptions("input2", "output2");
            var opt3 = new FFmpegOptions("input3", "output3");
            var optList = new List<FFmpegOptions> {opt, opt2, opt3};
            var ffmpeg = new FFmpegProcess(optList);
            var time = DateTime.UtcNow;

            var conversionResults = new List<ConversionResult>()
            {
                new ConversionResult(){Input = "input1", Output = "output1",Succeeded = true, StartTime = time, EndTime = time},
                new ConversionResult(){Input = "input2", Output = "output2",Succeeded = true, StartTime = time, EndTime = time},
                new ConversionResult(){Input = "input3", Output = "output3",Succeeded = false, StartTime = time, EndTime = time}
            };

            var expected = 1;

            //Act
            ffmpeg.RemoveFromQueue(conversionResults);
            var actual = ffmpeg.OptionsQueue.Count;

            //Assert
            Assert.Equal(expected, actual);
        }

        #endregion

        [Fact]
        public void ClearQueue_EmptiesList_ReturnsTrue()
        {
            //Arrange
            var opt = new FFmpegOptions("", "");
            var opt2 = new FFmpegOptions("", "");
            var optList = new List<FFmpegOptions> { opt, opt2 };
            var ffmpeg = new FFmpegProcess(optList);
            var expected = 0;

            //Act
            ffmpeg.ClearQueue();
            var actual = ffmpeg.OptionsQueue.Count;

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}