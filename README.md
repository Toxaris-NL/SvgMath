# SvgMath
C# (minimal effort)port if the Python SvgMath utility found at http://sourceforge.net/projects/svgmath/. Originally ported by Acivux (https://github.com/acivux/SvgMath). That version has been converted to a .Net Standard 1.3 library.

Some additional changes were made. The need for external font files was completely removed from the original port, removing the need to generate font files. Now the font metrics will be read from the font files instead. Only TrueType (.ttf) fonts are supported, not OpenType.
Another change is that the conversion now also works for countries that use a comma as a decimal separator instead of a point.

The library works with a configuration file, *svgmath.xml*. In this file additional font families can be added to be used. Also in the configuration file are default values, which can be changed of course. Change with care.
By default the serif font (called normal in the configuration file) will be used. If another font needs to be used, add the parameter *mathvariant* to the default section of the congfiguration file. The value should be equal to the name of the mathvariant to be used that is defined in the configuration file.

# Version
##### 1.0
* This is the first adapted version of the fork from the original port 

### License
All credit goes to the original creator Nikolai Grigoriev <svgmath@grigoriev.ru>

SVGMath is released under MIT open-source license. See the file "LICENSE.txt" for full licensing info.
