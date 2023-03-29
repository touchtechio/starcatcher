#ifndef H_PULSELINGER
#define H_PULSELINGER

#include "Shader.h"

class PulsingLinger : 
public Shader {

public:
  inline uint32_t shade (float height, uint32_t color, uint32_t currentColor, float remaining, uint32_t secondaryColor) {

    int pulses = 4;
    float amplitude = remaining * pulses;
    amplitude = amplitude - (int)amplitude;
    float brightness = 1.0 - (abs(amplitude - 0.5) *2);
    brightness *= (0.1 + remaining);

    if (height > .90) {
      return colorByBrightness(brightness, color);
    } 

    // fadeout rest
    if(7 > random(100)) {
      return colorByBrightness(0.99, currentColor);
    }
        
    return currentColor;
      
  }

};

#endif
