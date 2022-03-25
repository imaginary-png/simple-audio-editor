using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple_audio_editor;
using Xunit;

namespace simple_audio_editor.Tests
{
    public class FFmpegArgsBuilder_Tests
    {
        #region No Trim Tests

        [Fact]
        public void Create_NoTrimCreatesCorrectArgString()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"volume=1\" -b:a 128k \"OUTPUT.mp3\"";
            //Act
            string actual = FFmpegArgsBuilder.Create(new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3"));

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_NoTrimCreatesCorrectArgStringWithSetVolume()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"volume=0.6\" -b:a 128k \"OUTPUT.mp3\"";
            //Act
            string actual = FFmpegArgsBuilder.Create(new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3", 0.6));

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_NoTrimCreatesCorrectArgStringWithSetBitRate()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"volume=1\" -b:a 320k \"OUTPUT.mp3\"";
            //Act
            string actual = FFmpegArgsBuilder.Create(new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3", bitRate: 320));

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_NoTrimCreatesCorrectArgStringWithSetVolumeAndBitRate()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"volume=0.5\" -b:a 320k \"OUTPUT.mp3\"";
            //Act
            string actual = FFmpegArgsBuilder.Create(new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3", volume: 0.5, bitRate: 320));

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion

        #region Trim Tests

        [Fact]
        public void Create_HasTrimCreatesCorrectArgString()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"" +
                              $"[0:a]atrim=start=100:end=150,volume=1,asetpts=PTS-STARTPTS[0a];" +
                              $"[0:a]atrim=start=200:end=250,volume=1,asetpts=PTS-STARTPTS[1a];" +
                              $"[0:a]atrim=start=300:end=350,volume=1,asetpts=PTS-STARTPTS[2a];" +
                              $"[0a][1a][2a]concat=n=3:v=0:a=1[outa]" +
                              $"\" -b:a 128k -map [outa] \"OUTPUT.mp3\"";

            //Act
            var trimTimes = new List<TrimTime>()
            {
                new TrimTime(100, 150),
                new TrimTime(200,250),
                new TrimTime(300,350)
            };
            var options = new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3", trimTimes);
            string actual = FFmpegArgsBuilder.Create(options);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_HasTrimWithNoEndValueCreatesCorrectArgString()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"" +
                              $"[0:a]atrim=start=100:end=150,volume=1,asetpts=PTS-STARTPTS[0a];" +
                              $"[0:a]atrim=start=200:end=250,volume=1,asetpts=PTS-STARTPTS[1a];" +
                              $"[0:a]atrim=start=300,volume=1,asetpts=PTS-STARTPTS[2a];" +
                              $"[0a][1a][2a]concat=n=3:v=0:a=1[outa]" +
                              $"\" -b:a 128k -map [outa] \"OUTPUT.mp3\"";

            //Act
            var trimTimes = new List<TrimTime>()
            {
                new TrimTime(100, 150),
                new TrimTime(200,250),
                new TrimTime(300)
            };
            var options = new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3", trimTimes);
            string actual = FFmpegArgsBuilder.Create(options);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_HasTrimCreatesCorrectArgStringWithSetVolume()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"" +
                              $"[0:a]atrim=start=100:end=150,volume=0.4,asetpts=PTS-STARTPTS[0a];" +
                              $"[0:a]atrim=start=200:end=250,volume=0.4,asetpts=PTS-STARTPTS[1a];" +
                              $"[0:a]atrim=start=300:end=350,volume=0.4,asetpts=PTS-STARTPTS[2a];" +
                              $"[0a][1a][2a]concat=n=3:v=0:a=1[outa]" +
                              $"\" -b:a 128k -map [outa] \"OUTPUT.mp3\"";

            //Act
            var trimTimes = new List<TrimTime>()
            {
                new TrimTime(100, 150),
                new TrimTime(200,250),
                new TrimTime(300,350)
            };
            var options = new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3", trimTimes, 0.4);
            string actual = FFmpegArgsBuilder.Create(options);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_HasTrimCreatesCorrectArgStringWithSetBitRate()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"" +
                              $"[0:a]atrim=start=100:end=150,volume=1,asetpts=PTS-STARTPTS[0a];" +
                              $"[0:a]atrim=start=200:end=250,volume=1,asetpts=PTS-STARTPTS[1a];" +
                              $"[0:a]atrim=start=300:end=350,volume=1,asetpts=PTS-STARTPTS[2a];" +
                              $"[0a][1a][2a]concat=n=3:v=0:a=1[outa]" +
                              $"\" -b:a 180k -map [outa] \"OUTPUT.mp3\"";

            //Act
            var trimTimes = new List<TrimTime>()
            {
                new TrimTime(100, 150),
                new TrimTime(200,250),
                new TrimTime(300,350)
            };
            var options = new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3", trimTimes, bitRate: 180);
            string actual = FFmpegArgsBuilder.Create(options);

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Create_HasTrimCreatesCorrectArgStringWithSetVolumeAndBitRate()
        {
            //Arrange
            string expected = $"-y -i \"INPUT.mp3\" -filter_complex \"" +
                              $"[0:a]atrim=start=100:end=150,volume=0.8,asetpts=PTS-STARTPTS[0a];" +
                              $"[0:a]atrim=start=200:end=250,volume=0.8,asetpts=PTS-STARTPTS[1a];" +
                              $"[0:a]atrim=start=300:end=350,volume=0.8,asetpts=PTS-STARTPTS[2a];" +
                              $"[0a][1a][2a]concat=n=3:v=0:a=1[outa]" +
                              $"\" -b:a 180k -map [outa] \"OUTPUT.mp3\"";

            //Act
            var trimTimes = new List<TrimTime>()
            {
                new TrimTime(100, 150),
                new TrimTime(200,250),
                new TrimTime(300,350)
            };
            var options = new FFmpegOptions("INPUT.mp3", "OUTPUT.mp3", trimTimes, 0.8, 180);
            string actual = FFmpegArgsBuilder.Create(options);

            //Assert
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
