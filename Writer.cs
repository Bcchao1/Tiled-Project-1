using System;
using System.Linq;
using System.Drawing;
using System.Xml.Linq;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;

public static class Writer
{
    public static void CreateSamplesFile()
    {
       
        string[] names = Directory.GetFiles("newsamples");
        //string[] names = Directory.GetFiles("C:/Users/bjark/Documents/Tiled-Project-1/newsamples");

        for (int i = 0; i < names.Length; i++)
        {
            Console.WriteLine(names[i]);
            names[i] = names[i].Replace("newsamples\\", ""); //removing exraneous information from the filename
            names[i] = names[i].Replace(".png", ""); //algorithm adds .png back on...
        }

        Random rand = new Random();

        List<string> lines = new List<string>();
        lines.Add("<samples>");

        foreach(string s in names)
        {
            lines.Add("<overlapping name=\"" + s + "\" N=\"3\" symmetry=\"2\" periodic=\"True\" width=\"100\" height=\"100\"/>");
            lines.Add("<overlapping name=\"" + s + "\" N=\"3\" symmetry=\"8\" periodic=\"True\" width=\"100\" height=\"100\"/>");
            lines.Add("<overlapping name=\"" + s + "\" N=\"4\" symmetry=\"2\" periodic=\"True\" width=\"100\" height=\"100\"/>");
            lines.Add("<overlapping name=\"" + s + "\" N=\"" + rand.Next(2,6) + "\" symmetry=\"" + rand.Next(1, 9) + "\" periodic=\"True\" width=\"100\" height=\"100\" limit=\"5\"/>");
        }
        lines.Add("</samples>");

        System.IO.File.WriteAllLines(@"samples.xml", lines);
    }
}