<h1>Image Processing</h1>

<img src="https://i.imgur.com/sbRFma3.png" />

# Prerequisites
- .Net Framework 4.7.2
- Extended.Wpf.Toolkit 3.5.0 (included)
- Microsoft.Maps.MapControl.WPF 1.0.0.3 (included)
- mpolewaczyk.InteractiveDataDisplay.WPF 1.1.0 (included)
- System.Reactive 4.1.6 (included)
- WriteableBitmapEx 1.6.2 (included)

# Features
- Loading image from file (tested formats are PNG (with and without alpha channel), JPG, JPEG, BMP).
- Saving image to file.
- Show pixel coordinates, intensity, RGBa, HSV, CMYK, YUV values.
- Preview and work with:
  - Grayscale;
  - RGB;
  - RGBa;
  - Red channel;
  - Green channel;
  - Blue channel;
  - Alpha channel;
  - HSV;
  - Hue channel;
  - Saturation channel;
  - Value channel;
  - CMYK;
  - Cyan channel;
  - Magenta channel;
  - Yellow channel;
  - Black channel;
  - YUV;
  - Luma channel;
  - Color difference U channel;
  - Color difference V channel.
- If image pixel count is bigger than 1920 * 1080 then only RGBa and selected working mode is stored.
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
    <td align="center">-1</td>
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
