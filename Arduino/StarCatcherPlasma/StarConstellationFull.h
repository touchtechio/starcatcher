#ifndef H_CFULL
#define H_CFULL

#include "Shader.h"

class StarConstellationFull:
  public Shader {

  public:
    inline uint32_t shade (float height, uint32_t color, uint32_t currentColor, float remaining, uint32_t secondaryColor) {

      uint32_t mustard = 0xF1BA46;
      
      if (height > .90) {
        if (remaining > 0.6) {
          return colorByBrightness(remaining, mustard);
        }
        else {
          return colorByBrightness(1.0 * remaining, mustard);
        }

      } else {
        return colorByBrightness(0.0, mustard);
      }

    }

};

#endif //H_CFULL
