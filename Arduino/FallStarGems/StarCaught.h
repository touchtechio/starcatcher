#ifndef H_CAUGHT
#define H_CAUGHT

#include "Shader.h"

class StarCaught : 
public Shader {

public:
  inline uint32_t shade (float height, uint32_t color, uint32_t currentColor, float remaining, uint32_t secondaryColor) {

    if(remaining > 0.85) {
      if(4 > random(100)) {
        return secondaryColor;
      }
    } 
/*
    if(remaining > 0.7) {
      if(2 > random(100)) {
        return secondaryColor;
      }
    } 
*/

     if(7 > random(100)) {

      return colorByBrightness(0.9, currentColor);
    }

    return currentColor;

  }  
};

#endif


/*

    if(remaining > 0.85) {
      if(3 > random(100)) {
        return color;
      }
    } 
    
    if(5 > random(100)) {

      return colorByBrightness(0.9, currentColor);
    }
    
    return currentColor;
    
    */


