using System;
using System.Windows.Forms;
using System.Drawing;

// Afmetingen Form bepalen
int breedte = 0;
while (breedte < 300 || breedte > 900)
{
    Console.Write("Hoeveel pixels breed is het scherm (300-900): ");
    string user_input = Console.ReadLine();

    // Checken of het een geldig getal is
    try
    {
        breedte = int.Parse(user_input);
    }
    catch (Exception e)
    {
        Console.WriteLine($"'{user_input}' is geen geheel getal.");
    }
}

// Maak een Form aan
Form scherm = new Form();
scherm.Text = "Mandelbrot";
scherm.ClientSize = new Size(breedte, breedte + 90);

// Bitmap en Label aanmaken
int breedte_afb = breedte - 40;
Bitmap plaatje = new Bitmap(breedte_afb, breedte_afb);
Label afbeelding = new Label();
scherm.Controls.Add(afbeelding);
afbeelding.Image = plaatje;
afbeelding.Location = new Point(20, 110);
afbeelding.Size = new Size(breedte_afb, breedte_afb);


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

// Zet pixel coördinaten om in wiskundige coördinaten83y2498t7u2
(double, double) coördinaat(int px, int py, double x_min, double x_max, double y_min, double y_max)
{
    double x = x_min + px * (x_max - x_min) / (breedte_afb - 1);
    // (x_max - x_min) vertelt hoe breed de x-as "wiskundig" moet zijn
    // en gedeeld door (breedte_afb - 1) schaal je dat naar het aantal pixels

    double y = y_max - py * (y_max - y_min) / (breedte_afb - 1); // Hier doen we - i.p.v. +, omdat pixels boven beginnen
    return (x, y);
}

double x_min = -2.0, x_max = 2.0;
double y_min = -2.0, y_max = 2.0;

int max = 1000; // Kies hier de max herhalingen
int px = 0, py = 0; // Begin bij pixel (0,0)

while (px < breedte_afb)   // Ga langs alle x coördinaten
{
    while (py < breedte_afb) // Ga langs alle y coördinaten
    {
        (double x, double y) = coördinaat(px, py, x_min, x_max, y_min, y_max);
        int m_getal = mandelgetal(x, y, max);      // Bereken het mandelgetal van deze pixel
        if (m_getal == max)                        // Check of het mandelgetal groter is dan max
            plaatje.SetPixel(px, py, Color.Black);
        else if (m_getal % 2 == 0)                  // Kleurt even mandelgetallen zwart
            plaatje.SetPixel(px, py, Color.Black);
        else                                        // Kleurt de rest wit
            plaatje.SetPixel(px, py, Color.White);
        py++;
    }
    px++;
    py = 0; // Zet de y waarde weer terug naar 0
}
Application.Run(scherm);