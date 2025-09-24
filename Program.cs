using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.VisualBasic.Logging;


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
scherm.ClientSize = new Size(hoogte + 200, hoogte);

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
knop.Location = new Point(20, 230);
knop.Text = "GO";
knop.Size = new Size(120, 50);


// Beginwaardes
double schaal = 4.0 / breedte_afb;
double x = 0.0, y = 0.0;
int max = 300;
int baseMax = max;

int rood_multiplier = 3;
int groen_multiplier = 7;
int blauw_multiplier = 5;


// Tekstbox maken
TextBox tekstbox_schaal = new TextBox();
scherm.Controls.Add(tekstbox_schaal);
tekstbox_schaal.Location = new Point(70, 80);
tekstbox_schaal.Size = new Size(130, 50);

TextBox tekstbox_x = new TextBox();
scherm.Controls.Add(tekstbox_x);
tekstbox_x.Location = new Point(70, 120);
tekstbox_x.Size = new Size(130, 50);

TextBox tekstbox_y = new TextBox();
scherm.Controls.Add(tekstbox_y);
tekstbox_y.Location = new Point(70, 160);
tekstbox_y.Size = new Size(130, 50);

TextBox tekstbox_max = new TextBox();
scherm.Controls.Add(tekstbox_max);
tekstbox_max.Location = new Point(70, 200);
tekstbox_max.Size = new Size(130, 50);


// text voor de tekstbox maken
Label schaaltekst = new Label();
scherm.Controls.Add(schaaltekst);
schaaltekst.Location = new Point(10, 80);
schaaltekst.Text = "schaal:";

Label middenxtekst = new Label();
scherm.Controls.Add(middenxtekst);
middenxtekst.Location = new Point(10, 120);
middenxtekst.Text = "midden x:";

Label middenytekst = new Label();
scherm.Controls.Add(middenytekst);
middenytekst.Location = new Point(10, 160);
middenytekst.Text = "midden y:";

Label maxtekst = new Label();
scherm.Controls.Add(maxtekst);
maxtekst.Location = new Point(10, 200);
maxtekst.Text = "max aantal:";

//Slider maken
TrackBar slider_rood = new TrackBar();
scherm.Controls.Add(slider_rood);
slider_rood.Minimum = 0;
slider_rood.Maximum = 255;
slider_rood.Value = rood_multiplier;
slider_rood.TickFrequency = 1;
slider_rood.Location = new Point(30, 30);




//plaatje en textboxen updaten
void update()
{
    generate(x, y, schaal, max);
    afbeelding.Invalidate();

    tekstbox_schaal.Text = $"{schaal}";
    tekstbox_schaal.Invalidate();

    tekstbox_x.Text = $"{x}";
    tekstbox_x.Invalidate();

    tekstbox_y.Text = $"{y}";
    tekstbox_y.Invalidate();

    tekstbox_max.Text = $"{max}";
    tekstbox_max.Invalidate();
}

void go(object o, EventArgs e)
{
    schaal = double.Parse(tekstbox_schaal.Text);
    x = double.Parse(tekstbox_x.Text);
    y = double.Parse(tekstbox_y.Text);
    max = int.Parse(tekstbox_max.Text);
    update();
}

knop.Click += go;

// Berekent het Mandelgetal van punt (x, y) en geeft reele en imaginaire deler (a en b) terug voor smoothcolouring
 (double, double, int) mandelgetal(double x, double y, int max)
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
    return (a, b, t);
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
            var (a, b, m_getal) = mandelgetal(x2, y2, max);      // Bereken het mandelgetal van deze pixel + geef a en b mee

            if (m_getal == max)                        // Check of het mandelgetal groter is dan max
                plaatje.SetPixel(px, py, Color.Black);
            else
            {
                // -----Eenvoudige kleurtoekenning (niet meer gebruikt)-----
                //int kleurwaarde = m_getal;
                //Color kleur = Color.FromArgb(kleurwaarde % 256, kleurwaarde % 256, kleurwaarde % 256);
                //plaatje.SetPixel(px, py, kleur);

                double afstand_oorsprong_punt = Math.Sqrt(a * a + b * b);
                double smooth_kleurwaarde = m_getal + 1 - Math.Log(Math.Log(afstand_oorsprong_punt)) / Math.Log(2.0);
                int kleurwaarde = (int)(255.0 * smooth_kleurwaarde / max);
                Color kleur = Color.FromArgb(kleurwaarde * rood_multiplier % 256, kleurwaarde * groen_multiplier % 256, kleurwaarde * blauw_multiplier % 256);
                plaatje.SetPixel(px, py, kleur);
            }

        }
    }
}



// Registreer muis inputs en zoom in of uit
void muisKlik(object s, MouseEventArgs ea)
{
    double domein = breedte_afb * schaal; // Assenstelsel lengte van het domein/bereik

    // Min en max van x en y berekenen m.b.v. domein en middelpunt
    double x_min = x - domein / 2;
    double x_max = x + domein / 2;
    double y_min = y - domein / 2;
    double y_max = y + domein / 2;

    // Bereken (x, y) van klik en zet daar het middelpunt
    (double x_klik, double y_klik) = coördinaat(ea.X, ea.Y, x_min, x_max, y_min, y_max);

    x = x_klik;
    y = y_klik;

    // Zoom in of uit
    if (ea.Button == MouseButtons.Left)
    {
        schaal *= 0.5;
        max = (int)(baseMax * Math.Log(1.0 / (120 * schaal)));   // past max iteraties logaritmisch aan zodat detail bij inzoomen bewaart blijft
    }
    if (ea.Button == MouseButtons.Right)
        {
            schaal *= 2.0;
            max = (int)(baseMax * Math.Log(1.0 / (120 * schaal)));  
        }
    update();
}

afbeelding.MouseClick += muisKlik;

update();
Application.Run(scherm);