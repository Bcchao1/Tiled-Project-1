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

static class Program
{

    public static bool usingAutoWriter = false;

    static bool saveRegularSize = true;
    static bool saveUpscaleds = true;
    static bool saveRegularCombined = false;
    static bool saveUpscaledCombined = true;
    static bool saveflipped = true;

    static void Main(string[] args)
    {
        Dictionary<string, Bitmap> regularbitmaps = new Dictionary<string, Bitmap>();
        List<Bitmap> upscaleds = new List<Bitmap>();

        int scaling = 10;
        int tiling = 10;
        int upscaledTiling = 2;

        Console.WriteLine(args.Length);

        if (args[1] == "true")
        {
            usingAutoWriter = true;
        }
        else
        {
            usingAutoWriter = false;
        }

        Writer.CreateSamplesFile();

        Stopwatch sw = Stopwatch.StartNew();

        Random random = new Random();
        XDocument xdoc;
        if (usingAutoWriter)
        {
            xdoc = XDocument.Load("samplesauto.xml");
        }
        else
        {
            xdoc = XDocument.Load("samples.xml");
        }

        List<string> listofNames = new List<string>();

        int counter = 1;
        foreach (XElement xelem in xdoc.Root.Elements("overlapping", "simpletiled"))
        {
            Model model;
            string name = xelem.Get<string>("name");
            Console.WriteLine($"< {name}");

            if (xelem.Name == "overlapping") model = new OverlappingModel(name, xelem.Get("N", 2), xelem.Get("width", 48), xelem.Get("height", 48),
                xelem.Get("periodicInput", true), xelem.Get("periodic", false), xelem.Get("symmetry", 8), xelem.Get("ground", 0));
            else if (xelem.Name == "simpletiled") model = new SimpleTiledModel(name, xelem.Get<string>("subset"),
                xelem.Get("width", 10), xelem.Get("height", 10), xelem.Get("periodic", false), xelem.Get("black", false));
            else continue;

            for (int i = 0; i < xelem.Get("screenshots", 2); i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    Console.Write("> ");
                    int seed = random.Next();
                    bool finished = model.Run(seed, xelem.Get("limit", 0));
                    if (finished)
                    {
                        Console.WriteLine("DONE");
                        listofNames.Add($"{counter} {name} {i}");

                        
                        regularbitmaps.Add($"{counter} {name} {i}",model.Graphics());

                        if (model is SimpleTiledModel && xelem.Get("textOutput", false))
                            System.IO.File.WriteAllText($"{counter} {name} {i}.txt", (model as SimpleTiledModel).TextOutput());

                        break;
                    }
                    else Console.WriteLine("CONTRADICTION");
                }
            }

            counter++;
        }


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
                        // Console.WriteLine(orgX + "," + orgY + "  ---- " + image.Width + "x" + image.Height);
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
            foreach (Bitmap bit in upscaleds)
            {
                Bitmap combinedImage = new Bitmap(bit.Width * (tiling / 4), bit.Height * (tiling / 4));

                int height = combinedImage.Height;
                int width = combinedImage.Width;

                int orgX = 0;
                int orgY = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Console.WriteLine(orgX + "," + orgY + "  ---- " + image.Width + "x" + image.Height);
                        combinedImage.SetPixel(x, y, bit.GetPixel(orgX, orgY));

                        orgX++;

                        if (orgX >= bit.Width)
                        {
                            orgX = 0;
                        }

                    }
                    orgY++;
                    if (orgY >= bit.Height)
                    {
                        orgY = 0;
                    }
                }

                combinedImage.Save("BIGcombined " + bit + " " + count + ".png");
                count++;
            }
            Console.WriteLine("Upscaled Combineds Done!");
        }


        if (saveflipped)
        {
            Console.WriteLine("Beginning Flipped...");
            int count = 0;
            foreach (Bitmap bit in upscaleds)
            {
                Bitmap flippedImage = new Bitmap(bit.Width * upscaledTiling, bit.Height * upscaledTiling);

                int height = flippedImage.Height - 1; //is one pixel less because the resulting image won't have pixel 0 in height and width twice
                int width = flippedImage.Width - 1;

                int orgX = -bit.Width + 1;
                int orgY = -bit.Height + 1;

                for (int y = 0; y < height; y++)
                {
                    orgX = -bit.Width + 1;
                    for (int x = 0; x < width; x++)
                    {
                        // Console.WriteLine(orgX + "," + orgY + "  ---- " + image.Width + "x" + image.Height);
                        flippedImage.SetPixel(x, y, bit.GetPixel(Math.Abs(orgX), Math.Abs(orgY)));

                        // -(height / upscaledTiling); y < (height / upscaledTiling)

                        orgX++;

                        //if (orgX < 0)
                        //{
                        //    orgX = bit.Width - 1;
                        //}

                    }
                    orgY++;
                    //if (orgY < 0)
                    //{
                    //    orgY = bit.Height - 1;
                    //}
                }

                flippedImage.Save("FLIPPEDcombined " + bit + " " + count + ".png");
                count++;
            }
            Console.WriteLine("Flipped Done!");
        }






        Console.WriteLine($"time = {sw.ElapsedMilliseconds}");
    }
}
