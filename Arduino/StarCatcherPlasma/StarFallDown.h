#ifndef H_FALL
#define H_FALL

#include "Shader.h"

class StarFallDown : 
public Shader {

public:
  inline uint32_t shade (float height, uint32_t color, uint32_t currentColor, float remaining, uint32_t secondaryColor) {

    int scale = 1;
    height = height * scale;
    height = height - (int) height;
    height = 1.0 - height;
    
/*
    float closeness = 1.0 - abs(height - remaining);
    if (closeness > .80) {
      return colorByBrightness(closeness, color);
    } else {
     return colorByBrightness(closeness*.3, currentColor);
    }
*/
    float closeness = 1.0 - abs(height - remaining);
    if (closeness > 0.8) {
      return colorByBrightness(closeness*closeness*closeness, color);
    } else if (closeness > 0.6) {
     return colorByBrightness((height - remaining) , color);
    } 

  }  
};

#endif
