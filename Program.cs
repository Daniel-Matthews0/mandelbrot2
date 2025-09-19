using System;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualBasic.Devices;
using System.Security.Cryptography.X509Certificates;

// Afmetingen Form bepalen
int hoogte = 0;
while (hoogte < 300 || hoogte > 900)
{
    Console.Write("Hoeveel pixels hoog is het scherm (300-900): ");
    string user_input = Console.ReadLine();

    // Checken of het een geldig getal is
    try
    {
        hoogte = int.Parse(user_input);
    }
    catch (Exception e)
    {
        Console.WriteLine($"'{user_input}' is geen geheel getal.");
    }
}

// Maak een Form aan
Form scherm = new Form();
scherm.Text = "Mandelbrot";
scherm.ClientSize = new Size(hoogte+200, hoogte);

// Bitmap en Label aanmaken
int breedte_afb = hoogte - 20;
Bitmap plaatje = new Bitmap(breedte_afb, breedte_afb);
Label afbeelding = new Label();
scherm.Controls.Add(afbeelding);
afbeelding.Image = plaatje;
afbeelding.Location = new Point(210, 10);
afbeelding.Size = new Size(breedte_afb, breedte_afb);


// Buttons aanmaken
Button knop = new Button();
scherm.Controls.Add(knop);
knop.Location = new Point(20, 300);
knop.Text = "GO";
knop.Size = new Size(120, 50);

// Tekstbox maken
TextBox tekstbox_schaal = new TextBox();
scherm.Controls.Add(tekstbox_schaal);
tekstbox_schaal.Location = new Point(80, 100);

TextBox tekstbox_middenx = new TextBox();
scherm.Controls.Add(tekstbox_middenx);
tekstbox_middenx.Location = new Point(80, 140);

TextBox tekstbox_middeny = new TextBox();
scherm.Controls.Add(tekstbox_middeny);
tekstbox_middeny.Location = new Point(80, 180);

TextBox tekstbox_max = new TextBox();
scherm.Controls.Add(tekstbox_max);
tekstbox_max.Location = new Point(80, 220);

// text voor de tekstbox maken
Label schaaltekst = new Label();
scherm.Controls.Add(schaaltekst);
schaaltekst.Location = new Point(10, 100);
schaaltekst.Text = "schaal:";

Label middenxtekst = new Label();
scherm.Controls.Add(middenxtekst);
middenxtekst.Location = new Point(10, 140);
middenxtekst.Text = "midden x:";

Label middenytekst = new Label();
scherm.Controls.Add(middenytekst);
middenytekst.Location = new Point(10, 180);
middenytekst.Text = "midden y:";


Label maxtekst = new Label();
scherm.Controls.Add(maxtekst);
maxtekst.Location = new Point(10, 220);
maxtekst.Text = "max aantal:";


void go(object o, EventArgs e)
{


}


knop.Click += go;

// Berekent het Mandelgetal van punt (x, y).
int mandelgetal(double x, double y, int max)
{
    double a = 0, b = 0;// start (a,b) = (0,0)
    int t = 0;
    while ((a * a + b * b) <= 4 && t < max) // Stopt als de afstand groter is dan 2 of als "max" is bereikt
    {
        // Bereken nieuwe (a, b)
        double an = a * a - b * b + x;
        double bn = 2 * a * b + y;

        // Vervang oude (a, b) met nieuwe ("an", "bn")
        a = an;
        b = bn;
        t++;
    }
    return t;
}

// Zet pixel coördinaten om in wiskundige coördinaten
(double, double) coördinaat(int px, int py, double x_min, double x_max, double y_min, double y_max)
{
    double x = x_min + px * (x_max - x_min) / (breedte_afb - 1);
    // (x_max - x_min) vertelt hoe breed de x-as "wiskundig" moet zijn
    // en gedeeld door (breedte_afb - 1) schaal je dat naar het aantal pixels

    double y = y_max - py * (y_max - y_min) / (breedte_afb - 1); // Hier doen we - i.p.v. +, omdat pixels boven beginnen
    return (x, y);
}


double schaal = 0.01;
double x_min = schaal *-200, x_max = schaal * 200;
double y_min = schaal * -200, y_max = schaal * 200;

// Genereer de mandelbrot met een bepaald middenpunt
void generate(double x, double y, double schaal, int max)
{
    double domein = breedte_afb * schaal;

    double x_min = x - 0.5 * domein;
    double x_max = x + 0.5 * domein;
    double y_min = y - 0.5 * domein;
    double y_max = y + 0.5 * domein;

    for (int px = 0; px < breedte_afb; px++)   // Ga langs alle x coördinaten
    {
        for (int py = 0; py < breedte_afb; py++) // Ga langs alle y coördinaten
        {
            (double x2, double y2) = coördinaat(px, py, x_min, x_max, y_min, y_max);
            int m_getal = mandelgetal(x2, y2, max);      // Bereken het mandelgetal van deze pixel
            if (m_getal == max)                        // Check of het mandelgetal groter is dan max
                plaatje.SetPixel(px, py, Color.Black);
            else if (m_getal % 2 == 0)                  // Kleurt even mandelgetallen zwart
                plaatje.SetPixel(px, py, Color.Black);
            else                                        // Kleurt de rest wit
                plaatje.SetPixel(px, py, Color.White);
        }
    }
}

// Registreer muis inputs en zoom in of uit
void muisZoom(object o, MouseEventArgs ea, double schaal, int max)
{
    Console.WriteLine($"x: {ea.X} y: {ea.Y}");
    double x = (ea.X - breedte_afb / 2) * schaal;
    double y = (ea.Y - breedte_afb / 2) * -schaal;
    Console.WriteLine($"x: {x} y: {y}");
    generate(x, y, schaal, max);
    afbeelding.Invalidate();
}

double x_begin = 0.0, y_begin = 0.0;
double schaal = 4.0 / breedte_afb;

afbeelding.MouseClick += (s, ea) => schaal *= 0.5;
afbeelding.MouseClick += (s, ea) => muisZoom(s, ea, schaal, 1000);

generate(x_begin, y_begin, schaal, 1000);

Application.Run(scherm);
