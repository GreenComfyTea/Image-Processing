<h1>Image Processing</h1>

<img src="https://i.imgur.com/sbRFma3.png" />

# Features
- Loading image from file (tested formats are PNG (with and without Alpha Channel), JPG, JPEG, BMP).
- Saving image to file.
- Show pixel coordinates, Intensity, RGBa, HSV, CMYK, YUV values.
- Preview and work with:
  - Grayscale;
  - RGB;
  - RGBa;
  - Red Channel;
  - Green Channel;
  - Blue Channel;
  - Alpha Channel;
  - HSV;
  - Hue Channel;
  - Saturation Channel;
  - Value Channel;
  - CMYK;
  - Cyan Channel;
  - Magenta Channel;
  - Yellow Channel;
  - Black Channel;
  - YUV;
  - Luma Channel;
  - Color Difference U Channel;
  - Color Difference V Channel.
- If Image pixel count is bigger than 1920 * 1080 then only RGBa and selected working mode is stored.
- Otherwise, all modes are loaded into memory.
- Histogram for grayscale (intensity), red channel, green channel and blue channel.
- Histogram Stretching (histogram mode specify channels to be modified).
- Adaptive Vertical Threshold (histogram mode specify channels to be modified).
- Manual Vertical Threshold (histogram mode specify channels to be modified).
- Manual Vertical Double Threshold (histogram mode specify channels to be modified).
- Manual Horizontal Threshold (histogram mode specify channels to be modified).
- Low Pass Filter 1:

<table>
  <tr>
    <td align="center">1</td>
    <td align="center">1</td>
    <td align="center">1</td>
  </tr>
  <tr>
     <td align="center">1</td>
    <td align="center">1</td>
    <td align="center">1</td>
  </tr>
  <tr>
     <td align="center">1</td>
    <td align="center">1</td>
    <td align="center">1</td>
  </tr>
</table>

- Low Pass Filter 2:
<table>
  <tr>
    <td align="center">1</td>
    <td align="center">1</td>
    <td align="center">1</td>
  </tr>
  <tr>
     <td align="center">1</td>
    <td align="center">2</td>
    <td align="center">1</td>
  </tr>
  <tr>
     <td align="center">1</td>
    <td align="center">1</td>
    <td align="center">1</td>
  </tr>
</table>

- Low Pass Filter 3:
<table>
  <tr>
    <td align="center">1</td>
    <td align="center">2</td>
    <td align="center">1</td>
  </tr>
  <tr>
    <td align="center">2</td>
    <td align="center">4</td>
    <td align="center">2</td>
  </tr>
  <tr>
    <td align="center">1</td>
    <td align="center">2</td>
    <td align="center">1</td>
  </tr>
</table>

- High Pass Filter 1:
<table>
  <tr>
    <td align="center">0</td>
    <td align="center">-1</td>
    <td align="center">0</td>
  </tr>
  <tr>
     <td align="center">-1</td>
    <td align="center">5</td>
    <td align="center">-1</td>
  </tr>
  <tr>
     <td align="center">0</td>
    <td align="center">1</td>
    <td align="center">0</td>
  </tr>
</table>

- High Pass Filter 2:
<table>
  <tr>
    <td align="center">-1</td>
    <td align="center">-1</td>
    <td align="center">-1</td>
  </tr>
  <tr>
     <td align="center">-1</td>
    <td align="center">9</td>
    <td align="center">-1</td>
  </tr>
  <tr>
     <td align="center">-1</td>
    <td align="center">-1</td>
    <td align="center">1-</td>
  </tr>
</table>

- High Pass Filter 3:
<table>
  <tr>
    <td align="center">0</td>
    <td align="center">-2</td>
    <td align="center">0</td>
  </tr>
  <tr>
     <td align="center">-2</td>
    <td align="center">5</td>
    <td align="center">-2</td>
  </tr>
  <tr>
     <td align="center">0</td>
    <td align="center">-2</td>
    <td align="center">0</td>
  </tr>
</table>

- Median Filter (3x3, 5x5, 7x7).
- Sobel Operator.
- Robert's Cross Operator.
- Prewitt Operator.
