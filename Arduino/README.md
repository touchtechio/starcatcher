****PROJECTS****

- StarCatcherPlasma - 64 led stars of 16 (with a 32 length at the start of each chain)




**OSCMessages**

***Main Unity OSC Controls***

/allfall
- hits all stars w preset duration and behavior

/starfall
/starlinger
/starcaught
/constellationfull
 * accepts in from 0 to 64 as starId
 * accepts 0.0 to 1.0 for duration of 6sec
 * set behavior/duration, then hit starId


***Utility Functions***


/duration
 * accepts float from 0.0 to 1.0 as duration
 * percent of 6sec


/color
- /color/hue + float 0.0 to 1.0
- /saturation + float 0.0 to 1.0
- /brightness + float 0.0 to 1.0
- /skew
- /hitsync


/behavior
- set shader for all gems hits

/info
- output fps debug on serial console

/duration

/neighbors


