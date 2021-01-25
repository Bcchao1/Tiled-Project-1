/*
The MIT License(MIT)
Copyright(c) mxgmn 2016.
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
The software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.
*/

using System;
using System.Xml.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

static class Program
{
    public static bool usingAutoWriter = false;

    static bool saveRegularSize = true;
    static bool saveUpscaleds = true;
    static bool saveRegularCombined = false;
    static bool saveUpscaledCombined = true;
    static bool saveflipped = true;

    public static Dictionary<string, Bitmap> regularbitmaps = new Dictionary<string, Bitmap>(); //the first 2 are written to from WFC. not the cleanest but whatever
    public static List<string> listofNames = new List<string>();
    public static List<Bitmap> upscaleds = new List<Bitmap>();

    static void Main(string[] args)
    {
        ReadSettings();

        int scaling = 10;
        int tiling = 10;
        int upscaledTiling = 2;

        if (args[1] == "true") //arg 0 is "WaveFunctionCollapse.csproj" so we look at arg[1]
        {
            usingAutoWriter = true;
            Writer.CreateSamplesFile();
        }
        else
        {
            usingAutoWriter = false;
        }

        Stopwatch sw = Stopwatch.StartNew();

        WFC.Run(); // Runs the WFC


        if (saveRegularSize)
        {
            foreach (KeyValuePair<string, Bitmap> kvp in regularbitmaps)
            {
                kvp.Value.Save(kvp.Key + ".png");
            }
        }

        if(saveUpscaleds || saveUpscaledCombined)
        {
            Console.WriteLine("Beginning Upscaling...");

            foreach (string name in listofNames)
            {
                Bitmap image = regularbitmaps[name];

                Bitmap upscaledImage = new Bitmap(image.Width * scaling, image.Height * scaling);

                int height = upscaledImage.Height;
                int width = upscaledImage.Width;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        upscaledImage.SetPixel(x, y, image.GetPixel(x / scaling, y / scaling));
                    }
                }

                if (saveUpscaleds)
                {
                    upscaledImage.Save("upscaled " + name + ".png");
                }
                upscaleds.Add(upscaledImage);
            }

            Console.WriteLine("Upscales Done!");
        }

        if (saveRegularCombined)
        {
            Console.WriteLine("Beginning Combineds...");
            foreach (string name in listofNames)
            {
                Bitmap image = regularbitmaps[name];

                Bitmap combinedImage = new Bitmap(image.Width * tiling, image.Height * tiling);

                int height = combinedImage.Height;
                int width = combinedImage.Width;

                int orgX = 0;
                int orgY = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        combinedImage.SetPixel(x, y, image.GetPixel(orgX, orgY));

                        orgX++;

                        if (orgX >= image.Height)
                        {
                            orgX = 0;
                        }

                    }
                    orgY++;
                    if (orgY >= image.Width)
                    {
                        orgY = 0;
                    }
                }

                combinedImage.Save("combined " + name + ".png");
            }

            Console.WriteLine("Combined Done!");
        }

        if (saveUpscaledCombined)
        {
            Console.WriteLine("Beginning Upscaled Combineds...");
            int count = 0;
            foreach (string name in listofNames)
            {
                Bitmap combinedImage = new Bitmap(upscaleds[count].Width * (tiling / 4), upscaleds[count].Height * (tiling / 4));

                int height = combinedImage.Height;
                int width = combinedImage.Width;

                int orgX = 0;
                int orgY = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        combinedImage.SetPixel(x, y, upscaleds[count].GetPixel(orgX, orgY));

                        orgX++;

                        if (orgX >= upscaleds[count].Width)
                        {
                            orgX = 0;
                        }

                    }
                    orgY++;
                    if (orgY >= upscaleds[count].Height)
                    {
                        orgY = 0;
                    }
                }

                combinedImage.Save("BIGcombined " + name + ".png");
                count++;
            }
            Console.WriteLine("Upscaled Combineds Done!");
        }

        if (saveflipped)
        {
            Console.WriteLine("Beginning Flipped...");
            int count = 0;
            foreach (string name in listofNames)
            {
                Bitmap flippedImage = new Bitmap(upscaleds[count].Width * upscaledTiling, upscaleds[count].Height * upscaledTiling);

                int height = flippedImage.Height - 1; //is one pixel less because the resulting image won't have pixel 0 in height and width twice
                int width = flippedImage.Width - 1;

                int orgX = -upscaleds[count].Width + 1;
                int orgY = -upscaleds[count].Height + 1;

                for (int y = 0; y < height; y++)
                {
                    orgX = -upscaleds[count].Width + 1;
                    for (int x = 0; x < width; x++)
                    {
                        flippedImage.SetPixel(x, y, upscaleds[count].GetPixel(Math.Abs(orgX), Math.Abs(orgY)));

                        orgX++;

                    }
                    orgY++;
                }

                flippedImage.Save("FLIPPEDcombined " + name + ".png");
                count++;
            }
            Console.WriteLine("Flipped Done!");
        }



        Console.WriteLine($"time = {sw.ElapsedMilliseconds}");
    }


    static void ReadSettings()
    {
        XDocument settings = XDocument.Load("settings.xml");

        Console.WriteLine("s");
        List<XNode> nodes = settings.Nodes().ToList();

        saveRegularSize = settings.Root.Element("saveRegularSize").Get("value", true);
        saveUpscaleds = settings.Root.Element("saveUpscaleds").Get("value", true);
        saveRegularCombined = settings.Root.Element("saveRegularCombined").Get("value", true);
        saveUpscaledCombined = settings.Root.Element("saveUpscaledCombined").Get("value", true);
        saveflipped = settings.Root.Element("saveflipped").Get("value", true);

    }


}
