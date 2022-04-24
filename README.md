# simple-audio-editor
basic audio editor

A basic FFmpeg wrapper with a WPF GUI created using MVVM wth commands + zero code in code-behind.

This simple program uses ffmpeg to do some basic operations on audio tracks and convert to mp3.

It's a bit counterintuitive, but trim cuts out a snippet to KEEP,  
the slightly confusing name comes from the ffmpeg documentations https://ffmpeg.org/ffmpeg-filters.html#atrim

I created this project to learn, and because it would be useful to me for cutting out ads from podcasts.

The GUI is a graphical user interface that allows you to select an audio file, adjust volume and bitrate, and choose timeframe snippets for the final output.  



INSERT IMAGES AND STUFF HERE FOR SHOWING GUI











## info
FFmpeg processes and processing happen concurrently using asynchronous methods, and starts a new ffmpeg process for each file input.
This could be problematic on slower systems if trying to batch process multiple big audio files.


The backing library wrapper generates FFmpeg args with filter-complex and the atrim filter to create audio stream snippets from the original file  
then concatenates them together, generating a arg such as:  

-y -i "inputPath" -filter_complex "|									//input path  -y means output file will be overridden, if exists
[0:a]atrim=start=600:end=660,volume=0.6,asetpts=PTS-STARTPTS[0a]; 	//trim arg 1  
[0:a]atrim=start=1800:end=1860,volume=0.6,asetpts=PTS-STARTPTS[1a]; //trim arg 2, etc.  
[0:a]atrim=start=3180,volume=0.6,asetpts=PTS-STARTPTS[2a];  
[0a][1a][2a]concat=n=3:v=0:a=1[outa]" 								//trim concat arg  
-b:a 128k | 														//bitrate arg  
-map [outa] | 														//map stream select arg  
 "inputPath\Output\filename.mp3"										//outputs to a subfolder called Output (if doesn't exist, dir created)
 
 or  without trim args
 
  -y -i "inputPath -filter_complex "volume=0.3" -b:a 128k "inputPath\Output\filename.mp3"
  
### To Do

Maybe rework GUI so that it takes in time to REMOVE instead of keep, as the current trim works. -- this seems like it would be more intuitive and user-friendly.