#ifndef H_RISEUP
#define H_RISEUP

#include "Shader.h"

class StarRiseUp : 
public Shader {

public:
  inline uint32_t shade (float height, uint32_t color, uint32_t currentColor, float remaining, uint32_t secondaryColor) {

    int scale = 1;
    height = height * scale;
    height = height - (int) height;
    height = 1.0 - height;

    float flipped = 1.0 - remaining;
    float closeness = abs(height - flipped);
    if (height < 0.9) {
      if (closeness > .80) {
        return colorByBrightness(closeness*closeness*closeness, color);
      } else if (closeness > 0.6) {
        return colorByBrightness((height - flipped) , color);
      } 
    }
    return currentColor;
  }  
};

#endif
