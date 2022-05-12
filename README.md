# simple-audio-trimmer
A basic ffmpeg wrapper with a WPF GUI created using MVVM wth commands + zero code in code-behind. - Requires .net 5.0 

I created this project to learn, and because it would be useful to me for cutting out ads from the beginning and end of podcasts.

This program uses ffmpeg to do some basic operations on audio tracks and convert to mp3.

It's a bit counterintuitive, but trim cuts out a snippet of audio to keep, the slightly confusing name comes from the ffmpeg documentations https://ffmpeg.org/ffmpeg-filters.html#atrim


The graphical user interface allows you to select an audio file, adjust volume and bitrate, and choose timeframe snippets for the final output. 


## How to use
![SAE 1](https://user-images.githubusercontent.com/70348218/164958899-d627bbc8-ce16-4c91-8dbe-81755580fd73.png)


Download from releases -->  
Unzip and open exe.  
The rest should be simple.  
Mouse over sections for some tooltips, if needed.

Currently defaults to creating an Output subfolder in inputs dir -  
"InputFolder/audio.mp3" --> "InputFolder/Output/audio.mp3"
```
├── InputFolder
│   ├── audio.mp3
|   ├── OutPut
│       └── audio.mp3 (outputted file)
```

## info
FFmpeg processes and processing happen concurrently using asynchronous methods, and starts a new ffmpeg process for each file input.
This could be problematic on slower systems if trying to batch process multiple big audio files.


The backing library wrapper generates ffmpeg args with filter-complex and the [atrim](https://ffmpeg.org/ffmpeg-filters.html#atrim) filter to create audio stream snippets from the original file,  
then concatenates them together, generating an arg such as:  

<pre> 
-y -i "inputPath" -filter_complex                                    //input path  -y means output file will be overridden, if exists
"[0:a]atrim=start=600:end=660,volume=0.6,asetpts=PTS-STARTPTS[0a];   //trim arg 1  
[0:a]atrim=start=1800:end=1860,volume=0.6,asetpts=PTS-STARTPTS[1a];  //trim arg 2, etc.  
[0:a]atrim=start=3180,volume=0.6,asetpts=PTS-STARTPTS[2a];  
[0a][1a][2a]concat=n=3:v=0:a=1[outa]" 	                       	//trim concat arg  
-b:a 128k 		                        	      	//bitrate arg  
-map [outa] 			                                //map stream select arg  
"inputPath\Output\filename.mp3" 			      	//outputs to a subfolder called Output (if doesn't exist, dir created)
  
or  without trim args
 
-y -i "inputPath -filter_complex "volume=0.3" -b:a 128k "inputPath\Output\filename.mp3"
  </pre>
  
### To Do (maybe)

* Maybe rework GUI so that it takes in time to REMOVE instead of keep, as the current trim works. -- this seems like it would be more intuitive and user-friendly.
* Save config - font size, pref. default volume, bitrate, etc.
* Let user pick output location.
* find a package to get files length, and autofill trim end to file length
* ~~Change trim times to handle doubles instead of ints.~~
