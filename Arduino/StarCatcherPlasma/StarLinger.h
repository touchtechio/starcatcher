#ifndef H_LINGER
#define H_LINGER

#include "Shader.h"

class StarLinger :
  public Shader {

  public:
    inline uint32_t shade (float height, uint32_t color, uint32_t currentColor, float remaining, uint32_t secondaryColor) {

      uint32_t peach = 0xF1AF92;
      
      if (height > .90) {
        //if (remaining > 0.2) {
//          return colorByBrightness(1.0, peach);
          return colorByBrightness(1.0, color);
//          return colorByBrightness(1.0, secondaryColor);
        //}
        //else {
        //  return colorByBrightness(1.0 - remaining, currentColor);
        //}

      } else {
//        return currentColor;
              return colorByBrightness(0.0, color);

      }


    }


};

#endif //H_LINGER
